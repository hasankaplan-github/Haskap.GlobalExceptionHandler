
namespace Haskap.GlobalExceptionHandler;
public class Envelope<T>
{
    public T? Result { get; set; }
    public string? ExceptionMessage { get; set; }
    public string? ExceptionStackTrace { get; set; }
    public DateTime TimeGenerated { get; }
    public bool HasError 
    { 
        get
        {
            return !string.IsNullOrWhiteSpace(ExceptionMessage);
        }
    }
    public string? ExceptionType { get; set; }

    protected internal Envelope(T? result, string? exceptionMessage, string? exceptionStackTrace, string? exceptionType)
    {
        Result = result;
        ExceptionMessage = exceptionMessage;
        ExceptionStackTrace = exceptionStackTrace;
        ExceptionType = exceptionType;
        TimeGenerated = DateTime.UtcNow;
    }
}

public sealed class Envelope : Envelope<object>
{
    private Envelope(string? exceptionMessage, string? exceptionStackTrace, string? exceptionType)
        : base(null, exceptionMessage, exceptionStackTrace, exceptionType)
    {
    }

    public static Envelope<T> Ok<T>(T result)
    {
        return new Envelope<T>(result, null, null, null);
    }

    public static Envelope Ok()
    {
        return new Envelope(null, null, null);
    }

    public static Envelope Error(string? exceptionMessage, string? exceptionStackTrace, string? exceptionType)
    {
        return new Envelope(exceptionMessage, exceptionStackTrace, exceptionType);
    }
}
