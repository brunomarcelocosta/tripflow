namespace Tripflow.Application.DTOs.Common;

public class Result<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Error { get; init; }
    public bool IsForbidden { get; init; }

    private Result(bool success, T? data, string? error, bool isForbidden = false)
    {
        Success = success;
        Data = data;
        Error = error;
        IsForbidden = isForbidden;
    }

    public static Result<T> Ok(T data) => new(true, data, null);
    public static Result<T> Failure(string error) => new(false, default, error);
    public static Result<T> Failure(IEnumerable<string> errors) => new(false, default, string.Join("; ", errors));
    public static Result<T> Forbidden(string? message = "Acesso negado.") => new(false, default, message, true);
}

