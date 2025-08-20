using ElevatorSystem.API.Models;
using System.Collections.Concurrent;

namespace ElevatorSystem.API.Services
{
    public class ElevatorController : IElevatorController
    {
        private readonly Elevator _elevator;
        private readonly ILogger<ElevatorController> _logger;
        private readonly ConcurrentDictionary<int, ElevatorRequest> _activeRequests;
        private readonly Thread _processingThread;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly object _lockObject = new object();

        public ElevatorController(ILogger<ElevatorController> logger, ILogger<Elevator> elevatorLogger)
        {
            _logger = logger;
            _elevator = new Elevator(10, elevatorLogger); // 10 floors as per requirements
            _activeRequests = new ConcurrentDictionary<int, ElevatorRequest>();
            _cancellationTokenSource = new CancellationTokenSource();
            
            // Start processing thread
            _processingThread = new Thread(() => ProcessRequestsLoop(_cancellationTokenSource.Token))
            {
                IsBackground = true,
                Name = "ElevatorProcessingThread"
            };
            _processingThread.Start();
        }

        public async Task<ElevatorRequest> RequestElevatorAsync(int floor, Direction direction, int? destinationFloor = null)
        {
            if (floor < 1 || floor > 10)
            {
                throw new ArgumentException($"Invalid floor: {floor}. Must be between 1 and 10.");
            }

            if (destinationFloor.HasValue && (destinationFloor.Value < 1 || destinationFloor.Value > 10))
            {
                throw new ArgumentException($"Invalid destination floor: {destinationFloor.Value}. Must be between 1 and 10.");
            }

            var request = new ElevatorRequest(floor, direction, RequestType.PICKUP, destinationFloor);
            
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            lock (_lockObject)
            {
                if (!_elevator.AddRequest(request))
                {
                    throw new InvalidOperationException($"Failed to add elevator request for floor {floor}");
                }
                
                _activeRequests.TryAdd(request.Id, request);
                _logger.LogInformation("Elevator request added: ID {RequestId}, Floor {Floor}, Direction {Direction}", 
                    request.Id, floor, direction);
            }
            
            stopwatch.Stop();
            
            if (stopwatch.ElapsedMilliseconds > 100)
            {
                _logger.LogWarning("Elevator assignment took {ElapsedMs}ms, exceeding 100ms requirement", 
                    stopwatch.ElapsedMilliseconds);
            }

            return await Task.FromResult(request);
        }

        public async Task<ElevatorStatus> GetElevatorStatusAsync()
        {
            return await Task.FromResult(_elevator.GetStatus());
        }

        public async Task<List<ElevatorRequest>> GetActiveRequestsAsync()
        {
            return await Task.FromResult(_activeRequests.Values.ToList());
        }

        private void ProcessRequestsLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _elevator.ProcessRequests();
                    
                    // Clean up completed requests
                    var completedRequests = _activeRequests.Values
                        .Where(r => r.IsCompleted)
                        .ToList();
                    
                    foreach (var request in completedRequests)
                    {
                        _activeRequests.TryRemove(request.Id, out _);
                        _logger.LogInformation("Request {RequestId} completed and removed from active requests", request.Id);
                    }
                    
                    // Small delay to prevent busy waiting
                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in elevator processing loop");
                }
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _processingThread.Join(TimeSpan.FromSeconds(5));
            _cancellationTokenSource.Dispose();
        }
    }

    public interface IElevatorController
    {
        Task<ElevatorRequest> RequestElevatorAsync(int floor, Direction direction, int? destinationFloor = null);
        Task<ElevatorStatus> GetElevatorStatusAsync();
        Task<List<ElevatorRequest>> GetActiveRequestsAsync();
    }
}
