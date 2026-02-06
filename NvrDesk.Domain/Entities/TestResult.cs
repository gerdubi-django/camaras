namespace NvrDesk.Domain.Entities;

public sealed class TestResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;

    public static TestResult Success(string message) => new() { IsSuccess = true, Message = message };
    public static TestResult Failure(string message) => new() { IsSuccess = false, Message = message };
}
