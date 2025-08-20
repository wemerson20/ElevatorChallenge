using ElevatorSystem.API.Models;
using ElevatorSystem.API.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ElevatorSystem.Tests
{
    public class ElevatorControllerTests : IDisposable
    {
        private readonly Mock<ILogger<ElevatorController>> _mockControllerLogger;
        private readonly Mock<ILogger<Elevator>> _mockElevatorLogger;
        private readonly ElevatorController _controller;

        public ElevatorControllerTests()
        {
            _mockControllerLogger = new Mock<ILogger<ElevatorController>>();
            _mockElevatorLogger = new Mock<ILogger<Elevator>>();
            _controller = new ElevatorController(_mockControllerLogger.Object, _mockElevatorLogger.Object);
        }

        [Fact]
        public async Task RequestElevatorAsync_WithValidRequest_ShouldReturnRequest()
        {
            // Arrange
            var floor = 5;
            var direction = Direction.UP;
            var destinationFloor = 8;

            // Act
            var result = await _controller.RequestElevatorAsync(floor, direction, destinationFloor);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(floor, result.Floor);
            Assert.Equal(direction, result.Direction);
            Assert.Equal(destinationFloor, result.DestinationFloor);
            Assert.Equal(RequestType.PICKUP, result.Type);
            Assert.False(result.IsCompleted);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(11)]
        [InlineData(-1)]
        public async Task RequestElevatorAsync_WithInvalidFloor_ShouldThrowArgumentException(int floor)
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _controller.RequestElevatorAsync(floor, Direction.UP));
        }

        [Fact]
        public async Task RequestElevatorAsync_WithInvalidDestinationFloor_ShouldThrowArgumentException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => 
                _controller.RequestElevatorAsync(5, Direction.UP, 11));
        }

        [Fact]
        public async Task GetElevatorStatusAsync_ShouldReturnStatus()
        {
            // Act
            var status = await _controller.GetElevatorStatusAsync();

            // Assert
            Assert.NotNull(status);
            Assert.Equal(1, status.CurrentFloor);
            Assert.Equal(ElevatorState.IDLE, status.State);
        }

        [Fact]
        public async Task GetActiveRequestsAsync_ShouldReturnActiveRequests()
        {
            // Arrange
            await _controller.RequestElevatorAsync(5, Direction.UP, 8);

            // Act
            var requests = await _controller.GetActiveRequestsAsync();

            // Assert
            Assert.NotNull(requests);
            Assert.Single(requests);
            Assert.Equal(5, requests[0].Floor);
        }

        [Fact]
        public async Task RequestElevatorAsync_ShouldCompleteWithin100ms()
        {
            // Arrange
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act
            await _controller.RequestElevatorAsync(5, Direction.UP, 8);
            stopwatch.Stop();

            // Assert
            Assert.True(stopwatch.ElapsedMilliseconds < 100, 
                $"Request took {stopwatch.ElapsedMilliseconds}ms, should be less than 100ms");
        }

        [Fact]
        public async Task ConcurrentRequests_ShouldHandleMultipleRequestsSafely()
        {
            // Arrange
            var tasks = new List<Task<ElevatorRequest>>();
            var random = new Random();

            // Act - Create 50 concurrent requests
            for (int i = 0; i < 50; i++)
            {
                var floor = random.Next(1, 11);
                var direction = random.Next(2) == 0 ? Direction.UP : Direction.DOWN;
                var destinationFloor = random.Next(1, 11);
                
                tasks.Add(_controller.RequestElevatorAsync(floor, direction, destinationFloor));
            }

            // Wait for all requests to complete
            var results = await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(50, results.Length);
            Assert.All(results, request => Assert.NotNull(request));
            Assert.All(results, request => Assert.True(request.Floor >= 1 && request.Floor <= 10));
        }

        [Fact]
        public async Task LoadTest_ShouldHandle100PlusRequests()
        {
            // Arrange
            var tasks = new List<Task<ElevatorRequest>>();
            var random = new Random();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act - Create 150 concurrent requests
            for (int i = 0; i < 150; i++)
            {
                var floor = random.Next(1, 11);
                var direction = random.Next(2) == 0 ? Direction.UP : Direction.DOWN;
                var destinationFloor = random.Next(1, 11);
                
                tasks.Add(_controller.RequestElevatorAsync(floor, direction, destinationFloor));
            }

            // Wait for all requests to complete
            var results = await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Assert
            Assert.Equal(150, results.Length);
            Assert.All(results, request => Assert.NotNull(request));
            
            // Performance assertion - average time per request should be reasonable
            var averageTimePerRequest = stopwatch.ElapsedMilliseconds / 150.0;
            Assert.True(averageTimePerRequest < 10, 
                $"Average time per request: {averageTimePerRequest}ms, should be less than 10ms");
        }

        [Fact]
        public async Task ThreadSafety_ShouldHandleConcurrentStatusRequests()
        {
            // Arrange
            var tasks = new List<Task<ElevatorStatus>>();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // Act - Create 100 concurrent status requests
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(_controller.GetElevatorStatusAsync());
            }

            // Wait for all requests to complete
            var results = await Task.WhenAll(tasks);
            stopwatch.Stop();

            // Assert
            Assert.Equal(100, results.Length);
            Assert.All(results, status => Assert.NotNull(status));
            Assert.All(results, status => Assert.True(status.CurrentFloor >= 1 && status.CurrentFloor <= 10));
            
            // All status requests should return the same current floor (thread safety)
            var currentFloor = results[0].CurrentFloor;
            Assert.All(results, status => Assert.Equal(currentFloor, status.CurrentFloor));
        }

        public void Dispose()
        {
            _controller?.Dispose();
        }
    }
}
