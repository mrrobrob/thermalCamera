namespace thermalCamera.Hubs
{
    public class Frame
    {
        public required List<string> Pixels { get; init; }
        public required string Max { get; init; }
        public required string Min { get; init; }
    }
}
