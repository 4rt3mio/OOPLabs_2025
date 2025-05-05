using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using OOPtl.Application.DTOs;
using OOPtl.Domain.Factories;

namespace OOPtl.DataAccess.Api
{
    public class QuoteApiAdapter
    {
        private readonly HttpClient _httpClient;

        public QuoteApiAdapter()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    (sender, cert, chain, sslPolicyErrors) => true
            };

            _httpClient = new HttpClient(handler);
        }

        public async Task<QuoteDTO> GetRandomQuoteAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("https://api.quotable.io/random");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var data = JsonSerializer.Deserialize<QuoteApiResponse>(json, options);

                return QuoteFactory.Create(data.Content, data.Author);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
                return new QuoteDTO("No quote available", "System");
            }
        }

        private class QuoteApiResponse
        {
            [JsonPropertyName("content")]
            public string Content { get; set; }

            [JsonPropertyName("author")]
            public string Author { get; set; }
        }
    }
}