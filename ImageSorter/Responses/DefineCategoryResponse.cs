using System.Text.Json.Serialization;

namespace ImageSorter.Responses;

public class Concept
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("value")]
    public required double Value { get; set; }

    [JsonPropertyName("app_id")]
    public required string AppId { get; set; }
}

public class ResponseData
{
    [JsonPropertyName("concepts")]
    public ICollection<Concept> Concepts { get; set; } = new List<Concept>();
}

public class Output
{
    [JsonPropertyName("data")]
    public required ResponseData Data { get; set; }
}

public class DefineCategoryResponse
{
    [JsonPropertyName("outputs")]
    public ICollection<Output> Outputs { get; set; } = new List<Output>();
}