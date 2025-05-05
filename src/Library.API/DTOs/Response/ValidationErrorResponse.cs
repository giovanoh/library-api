namespace Library.API.DTOs.Response;

public record ValidationErrorResponse
{
    public List<ValidationError> Errors { get; init; } = [];
}

public record ValidationError
{
    public string Field { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
} 