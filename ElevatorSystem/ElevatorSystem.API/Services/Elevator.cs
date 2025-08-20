using ElevatorSystem.API.Models;
using System.Collections.Concurrent;

namespace ElevatorSystem.API.Services
{
    public class Elevator
    {
        private readonly object _lockObject = new object();
        private readonly ConcurrentQueue<ElevatorRequest> _requestQueue;
        private readonly HashSet<int> _targetFloors;
        private readonly int _maxFloors;
        private readonly ILogger<Elevator> _logger;

        public int CurrentFloor { get; private set; }
        public ElevatorState State { get; private set; }
        public int RequestCount => _requestQueue.Count;
        public IReadOnlyCollection<int> TargetFloors => _targetFloors.ToList().AsReadOnly();

        public Elevator(int maxFloors, ILogger<Elevator> logger)
        {
            _maxFloors = maxFloors;
            _logger = logger;
            CurrentFloor = 1;
            State = ElevatorState.IDLE;
            _requestQueue = new ConcurrentQueue<ElevatorRequest>();
            _targetFloors = new HashSet<int>();
        }

        public bool AddRequest(ElevatorRequest request)
        {
            if (request.Floor < 1 || request.Floor > _maxFloors)
            {
                _logger.LogWarning("Invalid floor request: {Floor}. Max floors: {MaxFloors}", request.Floor, _maxFloors);
                return false;
            }

            if (request.DestinationFloor.HasValue && 
                (request.DestinationFloor.Value < 1 || request.DestinationFloor.Value > _maxFloors))
            {
                _logger.LogWarning("Invalid destination floor: {DestinationFloor}. Max floors: {MaxFloors}", 
                    request.DestinationFloor.Value, _maxFloors);
                return false;
            }

            lock (_lockObject)
            {
                _requestQueue.Enqueue(request);
                _targetFloors.Add(request.Floor);
                
                if (request.DestinationFloor.HasValue)
                {
                    _targetFloors.Add(request.DestinationFloor.Value);
                }

                _logger.LogInformation("Added request: Floor {Floor}, Direction {Direction}, Type {Type}", 
                    request.Floor, request.Direction, request.Type);
            }

            return true;
        }

        public void ProcessRequests()
        {
            while (true)
            {
                ElevatorRequest? request = null;
                
                lock (_lockObject)
                {
                    if (_requestQueue.TryDequeue(out request))
                    {
                        _logger.LogInformation("Processing request: Floor {Floor}, Direction {Direction}", 
                            request.Floor, request.Direction);
                    }
                }

                if (request != null)
                {
                    ProcessRequest(request);
                }
                else
                {
                    // No requests, go to idle state
                    lock (_lockObject)
                    {
                        if (_requestQueue.IsEmpty)
                        {
                            State = ElevatorState.IDLE;
                            _logger.LogInformation("Elevator is now IDLE at floor {Floor}", CurrentFloor);
                        }
                    }
                    break;
                }
            }
        }

        private void ProcessRequest(ElevatorRequest request)
        {
            // Move to the requested floor
            MoveToFloor(request.Floor);
            
            // Open door
            OpenDoor();
            
            // Mark pickup as completed
            if (request.Type == RequestType.PICKUP)
            {
                request.IsCompleted = true;
                _logger.LogInformation("Pickup completed at floor {Floor}", request.Floor);
                
                // If there's a destination, add it as a new request
                if (request.DestinationFloor.HasValue)
                {
                    var destinationRequest = new ElevatorRequest(
                        request.DestinationFloor.Value, 
                        request.Direction, 
                        RequestType.DESTINATION);
                    
                    AddRequest(destinationRequest);
                }
            }
            else
            {
                request.IsCompleted = true;
                _logger.LogInformation("Destination reached at floor {Floor}", request.Floor);
            }
            
            // Close door
            CloseDoor();
            
            // Remove from target floors
            lock (_lockObject)
            {
                _targetFloors.Remove(request.Floor);
            }
        }

        private void MoveToFloor(int targetFloor)
        {
            if (CurrentFloor == targetFloor)
            {
                _logger.LogInformation("Already at floor {Floor}", targetFloor);
                return;
            }

            if (CurrentFloor < targetFloor)
            {
                State = ElevatorState.MOVING_UP;
                _logger.LogInformation("Moving UP from floor {CurrentFloor} to {TargetFloor}", CurrentFloor, targetFloor);
                
                while (CurrentFloor < targetFloor)
                {
                    Thread.Sleep(1000); // Simulate movement time
                    CurrentFloor++;
                    _logger.LogDebug("Reached floor {Floor}", CurrentFloor);
                }
            }
            else
            {
                State = ElevatorState.MOVING_DOWN;
                _logger.LogInformation("Moving DOWN from floor {CurrentFloor} to {TargetFloor}", CurrentFloor, targetFloor);
                
                while (CurrentFloor > targetFloor)
                {
                    Thread.Sleep(1000); // Simulate movement time
                    CurrentFloor--;
                    _logger.LogDebug("Reached floor {Floor}", CurrentFloor);
                }
            }
        }

        private void OpenDoor()
        {
            State = ElevatorState.DOOR_OPEN;
            _logger.LogInformation("Opening door at floor {Floor}", CurrentFloor);
            Thread.Sleep(2000); // Simulate door opening time
        }

        private void CloseDoor()
        {
            _logger.LogInformation("Closing door at floor {Floor}", CurrentFloor);
            Thread.Sleep(2000); // Simulate door closing time
            State = ElevatorState.IDLE;
        }

        public ElevatorStatus GetStatus()
        {
            lock (_lockObject)
            {
                return new ElevatorStatus
                {
                    CurrentFloor = CurrentFloor,
                    State = State,
                    PendingRequests = _requestQueue.Count,
                    TargetFloors = _targetFloors.ToList()
                };
            }
        }
    }

    public class ElevatorStatus
    {
        public int CurrentFloor { get; set; }
        public ElevatorState State { get; set; }
        public int PendingRequests { get; set; }
        public List<int> TargetFloors { get; set; } = new List<int>();
    }
}
