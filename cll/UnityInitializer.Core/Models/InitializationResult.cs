namespace UnityInitializer.Core.Models
{
    public class InitializationResult
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public string? ActualVersion { get; init; }
    }
}