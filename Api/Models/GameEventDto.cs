namespace Api.Models
{
    public class PositionDto
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }

    public class GameEventDto
    {
        public string PlayerId { get; set; }
        public string EventType { get; set; }
        public DateTime Timestamp { get; set; }
        public PositionDto Position { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}
