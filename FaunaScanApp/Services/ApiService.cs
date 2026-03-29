namespace FaunaScanApp.Services;

public class ApiService
{
    private readonly HttpClient _httpClient = new();

    private const string API_URL = "http://10.74.17.14:8000/detect";

    public async Task<AnimalResult> AnalyzeImageAsync(string imagePath)
    {
        using var form = new MultipartFormDataContent();

        using var fileStream = File.OpenRead(imagePath);
        using var content = new StreamContent(fileStream);
        content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        // 🔥 IMPORTANT
        form.Add(content, "image", Path.GetFileName(imagePath));

        var url = $"{API_URL}?username=tom";

        var response = await _httpClient.PostAsync(url, form);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();

        Console.WriteLine("===== API RESPONSE =====");
        Console.WriteLine(json);
        Console.WriteLine("========================");

        return JsonSerializer.Deserialize<AnimalResult>(json);
    }
}