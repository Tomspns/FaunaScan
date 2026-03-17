using FaunaScanApp.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace FaunaScanApp.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://IP_SERVEUR:PORT/");
        }

        public async Task<AnimalResult> PredictAnimal(Stream imageStream)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StreamContent(imageStream), "file", "animal.jpg");

            var response = await _httpClient.PostAsync("predict", content);

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<AnimalResult>(json);
            return result ?? new AnimalResult();
        }
    }

}
