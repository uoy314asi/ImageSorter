using System.Text.Json.Serialization;

namespace ImageSorter.Responses;

public record UploadImageResponse
{
    [JsonPropertyName("data")]
    public required UploadImageDataResponse Data { get; init; }
}

public record UploadImageDataResponse
{
    [JsonPropertyName("url")]
    public required string Url { get; init; }
}