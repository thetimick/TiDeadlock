namespace TiDeadlock.Extensions.Exception;

public static class ExceptionExtensions
{
    public static string? GetShortStackTrace(this System.Exception exception)
    {
        return $"{exception.Message}\n\n{string.Join("", exception.StackTrace?.Take(800) ?? [])}";
    }
}