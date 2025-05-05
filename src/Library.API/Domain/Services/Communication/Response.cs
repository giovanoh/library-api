namespace Library.API.Domain.Services.Communication;

public record Response<T>
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public ErrorType? Error { get; init; }
    public T? Model { get; init; }

    private Response(bool success, string? message, ErrorType? error, T? model)
    {
        Success = success;
        Message = message;
        Error = error;
        Model = model;
    }

    public static Response<T> Ok(T model) => 
        new(true, null, null, model);

    public static Response<T> Fail(string message, ErrorType error) => 
        new(false, message, error, default);

    public static Response<T> NotFound(string message = "Resource not found") => 
        new(false, message, ErrorType.NotFound, default);
}