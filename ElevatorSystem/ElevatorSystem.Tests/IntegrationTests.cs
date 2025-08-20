using ElevatorSystem.API.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace ElevatorSystem.Tests
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public IntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RequestElevator_WithValidRequest_ShouldReturnOk()
        {
            // Arrange
            var request = new
            {
                Floor = 5,
                Direction = Direction.UP,
                DestinationFloor = 8
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/elevator/request", content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var elevatorRequest = JsonSerializer.Deserialize<ElevatorRequest>(responseContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            Assert.NotNull(elevatorRequest);
            Assert.Equal(5, elevatorRequest.Floor);
            Assert.Equal(Direction.UP, elevatorRequest.Direction);
            Assert.Equal(8, elevatorRequest.DestinationFloor);
        }

        [Fact]
        public async Task RequestElevator_WithInvalidFloor_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new
            {
                Floor = 11,
                Direction = Direction.UP,
                DestinationFloor = 8
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/elevator/request", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetStatus_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/elevator/status");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetActiveRequests_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/elevator/requests");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
