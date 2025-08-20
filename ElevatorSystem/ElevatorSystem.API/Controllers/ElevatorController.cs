using ElevatorSystem.API.Models;
using ElevatorSystem.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElevatorSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ElevatorController : ControllerBase
    {
        private readonly IElevatorController _elevatorController;
        private readonly ILogger<ElevatorController> _logger;

        public ElevatorController(IElevatorController elevatorController, ILogger<ElevatorController> logger)
        {
            _elevatorController = elevatorController;
            _logger = logger;
        }

        [HttpPost("request")]
        public async Task<ActionResult<ElevatorRequest>> RequestElevator([FromBody] ElevatorRequestDto request)
        {
            try
            {
                var elevatorRequest = await _elevatorController.RequestElevatorAsync(
                    request.Floor, 
                    request.Direction, 
                    request.DestinationFloor);
                
                return Ok(elevatorRequest);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Invalid request: {Message}", ex.Message);
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError("Operation failed: {Message}", ex.Message);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("status")]
        public async Task<ActionResult<ElevatorStatus>> GetStatus()
        {
            try
            {
                var status = await _elevatorController.GetElevatorStatusAsync();
                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting elevator status");
                return StatusCode(500, new { error = "Failed to get elevator status" });
            }
        }

        [HttpGet("requests")]
        public async Task<ActionResult<List<ElevatorRequest>>> GetActiveRequests()
        {
            try
            {
                var requests = await _elevatorController.GetActiveRequestsAsync();
                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active requests");
                return StatusCode(500, new { error = "Failed to get active requests" });
            }
        }

        [HttpPost("load-test")]
        public async Task<ActionResult<LoadTestResult>> LoadTest([FromBody] LoadTestRequest request)
        {
            try
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                var tasks = new List<Task<ElevatorRequest>>();
                var random = new Random();

                // Create concurrent requests
                for (int i = 0; i < request.NumberOfRequests; i++)
                {
                    var floor = random.Next(1, 11);
                    var direction = random.Next(2) == 0 ? Direction.UP : Direction.DOWN;
                    var destinationFloor = random.Next(1, 11);
                    
                    tasks.Add(_elevatorController.RequestElevatorAsync(floor, direction, destinationFloor));
                }

                // Wait for all requests to be processed
                await Task.WhenAll(tasks);
                stopwatch.Stop();

                var result = new LoadTestResult
                {
                    TotalRequests = request.NumberOfRequests,
                    TotalTimeMs = stopwatch.ElapsedMilliseconds,
                    AverageTimePerRequestMs = stopwatch.ElapsedMilliseconds / (double)request.NumberOfRequests,
                    SuccessfulRequests = tasks.Count(t => t.IsCompletedSuccessfully),
                    FailedRequests = tasks.Count(t => t.IsFaulted)
                };

                _logger.LogInformation("Load test completed: {Result}", result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during load test");
                return StatusCode(500, new { error = "Load test failed" });
            }
        }
    }

    public class ElevatorRequestDto
    {
        public int Floor { get; set; }
        public Direction Direction { get; set; }
        public int? DestinationFloor { get; set; }
    }

    public class LoadTestRequest
    {
        public int NumberOfRequests { get; set; } = 100;
    }

    public class LoadTestResult
    {
        public int TotalRequests { get; set; }
        public long TotalTimeMs { get; set; }
        public double AverageTimePerRequestMs { get; set; }
        public int SuccessfulRequests { get; set; }
        public int FailedRequests { get; set; }
    }
}
