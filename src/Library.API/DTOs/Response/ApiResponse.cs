using System.Text.Json.Serialization;

namespace Library.API.DTOs.Response;

public record ApiResponse<T>
{
    [JsonPropertyName("data")]
    public T? Data { get; init; }

    [JsonPropertyName("meta")]
    public Dictionary<string, object>? Meta { get; init; }

    [JsonConstructor]
    private ApiResponse(T data, Dictionary<string, object>? meta = null)
    {
        Data = data;
        Meta = meta;
    }

    public static ApiResponse<T> Ok(T data, Dictionary<string, object>? meta = null) =>
        new(data, meta);
}
