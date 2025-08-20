namespace ElevatorSystem.API.Models
{
    public class ElevatorRequest
    {
        public int Id { get; set; }
        public int Floor { get; set; }
        public Direction Direction { get; set; }
        public RequestType Type { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsCompleted { get; set; }
        public int? DestinationFloor { get; set; }

        public ElevatorRequest(int floor, Direction direction, RequestType type, int? destinationFloor = null)
        {
            Id = Guid.NewGuid().GetHashCode();
            Floor = floor;
            Direction = direction;
            Type = type;
            Timestamp = DateTime.UtcNow;
            IsCompleted = false;
            DestinationFloor = destinationFloor;
        }
    }

    public enum Direction
    {
        UP,
        DOWN
    }

    public enum RequestType
    {
        PICKUP,
        DESTINATION
    }
}
