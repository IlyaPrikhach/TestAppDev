namespace TestAppDev.Models;

public class ExceptionJournal
{
    public Guid Id { get; set; }
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string ExceptionType { get; set; } = null!;
    public string? QueryParameters { get; set; }
    public string? BodyParameters { get; set; }
    public string StackTrace { get; set; } = null!;
    public string ExceptionMessage { get; set; } = null!;
}
