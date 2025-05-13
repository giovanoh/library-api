using System.Text.Json.Serialization;

namespace Library.API.DTOs.Response;

public record ApiProblemDetails
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = "https://tools.ietf.org/html/rfc7231#section-6.5.1";

    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    [JsonPropertyName("status")]
    public int Status { get; init; }

    [JsonPropertyName("detail")]
    public string Detail { get; init; } = string.Empty;

    [JsonPropertyName("instance")]
    public string? Instance { get; init; }

    [JsonPropertyName("errors")]
    public Dictionary<string, string[]>? Errors { get; init; }

    [JsonPropertyName("correlationId")]
    public string? CorrelationId { get; init; }
}
