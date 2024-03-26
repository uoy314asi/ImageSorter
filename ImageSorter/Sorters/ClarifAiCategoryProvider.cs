using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using ImageSorter.Requests;
using ImageSorter.Responses;

namespace ImageSorter.Sorters;

public sealed class ClarifAiCategoryProvider : IImageCategoryProvider, IDisposable
{
    private readonly HttpClient _httpClient;

    public ClarifAiCategoryProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> DefineCategoryAsync(string filePath, CancellationToken cancellationToken = default)
    {
        try
        {
            var uploadedUrl = await UploadImageAsync(filePath, cancellationToken);
            using var request = new HttpRequestMessage(HttpMethod.Post, new Uri("https://api.clarifai.com/v2/users/clarifai/apps/main/models/general-image-recognition/versions/aa7f35c01e0642fda5cf400f543e7c40/outputs"));
            var body = new DefineCategoryRequest
            {
                Inputs = new List<Input>
                {
                    new()
                    {
                        Data = new Data
                        {
                            Image = new Image
                            {
                                Url = uploadedUrl
                            }
                        }
                    }
                }
            };
            request.Content = JsonContent.Create(body);
            request.Headers.Add("Authorization", "Key 482334ea6a8e4d76880d769a184d2c76");
            request.Headers.Add("User-Agent", "PostmanRuntime/7.35.0");
            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var responseBody = JsonSerializer.Deserialize<DefineCategoryResponse>(json);
            return responseBody is null
                ? "undefined"
                : responseBody.Outputs.First().Data.Concepts.OrderByDescending(e => e.Value).First().Name;
        }
        catch
        {
            return "undefined";
        }
    }

    private async Task<string> UploadImageAsync(string path, CancellationToken cancellationToken = default)
    {
        const string uploadUrl = "https://api.imgbb.com/1/upload?key=beffed62f672d0ad45f89fa3fffa07b5&expiration=1800";
        using var request = new HttpRequestMessage(HttpMethod.Post, uploadUrl);
        var content = new MultipartFormDataContent();
        content.Add(new StreamContent(File.OpenRead(path)), "image", Path.GetFileName(path));
        request.Content = content;
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var body = JsonSerializer.Deserialize<UploadImageResponse>(json);
        return body!.Data.Url;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}