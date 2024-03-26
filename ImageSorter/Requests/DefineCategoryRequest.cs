using System.Text.Json.Serialization;

namespace ImageSorter.Requests;

public class Data
{
    [JsonPropertyName("image")]
    public required Image Image { get; init; }
}

public class Image
{
    [JsonPropertyName("url")]
    public required string Url { get; init; }
}

public class Input
{
    [JsonPropertyName("data")]
    public required Data Data { get; init; }
}

public class DefineCategoryRequest
{
    [JsonPropertyName("inputs")]
    public ICollection<Input> Inputs { get; init; } = new List<Input>();
}