namespace carwash.Service.DTOs.Common;

public class ServiceResult<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Error { get; init; }
    public IEnumerable<string> Errors { get; init; } = [];

    public static ServiceResult<T> Ok(T data) => new() { Success = true, Data = data };

    public static ServiceResult<T> Fail(string error) => new() { Success = false, Error = error };

    public static ServiceResult<T> Fail(IEnumerable<string> errors) =>
        new() { Success = false, Errors = errors, Error = errors.FirstOrDefault() };
}
