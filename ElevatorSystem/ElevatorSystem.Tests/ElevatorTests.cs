using ElevatorSystem.API.Models;
using ElevatorSystem.API.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ElevatorSystem.Tests
{
    public class ElevatorTests
    {
        private readonly Mock<ILogger<Elevator>> _mockLogger;
        private readonly Elevator _elevator;

        public ElevatorTests()
        {
            _mockLogger = new Mock<ILogger<Elevator>>();
            _elevator = new Elevator(10, _mockLogger.Object);
        }

        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Assert
            Assert.Equal(1, _elevator.CurrentFloor);
            Assert.Equal(ElevatorState.IDLE, _elevator.State);
            Assert.Equal(0, _elevator.RequestCount);
        }

        [Theory]
        [InlineData(1, Direction.UP)]
        [InlineData(5, Direction.DOWN)]
        [InlineData(10, Direction.UP)]
        public void AddRequest_WithValidFloor_ShouldReturnTrue(int floor, Direction direction)
        {
            // Arrange
            var request = new ElevatorRequest(floor, direction, RequestType.PICKUP);

            // Act
            var result = _elevator.AddRequest(request);

            // Assert
            Assert.True(result);
            Assert.Equal(1, _elevator.RequestCount);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(11)]
        [InlineData(-1)]
        public void AddRequest_WithInvalidFloor_ShouldReturnFalse(int floor)
        {
            // Arrange
            var request = new ElevatorRequest(floor, Direction.UP, RequestType.PICKUP);

            // Act
            var result = _elevator.AddRequest(request);

            // Assert
            Assert.False(result);
            Assert.Equal(0, _elevator.RequestCount);
        }

        [Fact]
        public void AddRequest_WithInvalidDestinationFloor_ShouldReturnFalse()
        {
            // Arrange
            var request = new ElevatorRequest(5, Direction.UP, RequestType.PICKUP, 11);

            // Act
            var result = _elevator.AddRequest(request);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetStatus_ShouldReturnCorrectStatus()
        {
            // Arrange
            var request = new ElevatorRequest(5, Direction.UP, RequestType.PICKUP);
            _elevator.AddRequest(request);

            // Act
            var status = _elevator.GetStatus();

            // Assert
            Assert.Equal(1, status.CurrentFloor);
            Assert.Equal(ElevatorState.IDLE, status.State);
            Assert.Equal(1, status.PendingRequests);
            Assert.Contains(5, status.TargetFloors);
        }
    }
}
