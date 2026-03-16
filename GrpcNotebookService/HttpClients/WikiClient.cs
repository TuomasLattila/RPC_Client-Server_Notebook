using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace GrpcNotebookService.HttpClients
{
    public class WikiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://en.wikipedia.org/api/rest_v1";

        public WikiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("User-agent", "notebookApp");
        }

        public async Task<ArticleReply> GetOneArticle(string articleName)
        {
            //var url = $"{_baseUrl}?action=opensearch&search={Uri.EscapeDataString(articleName)}&limit=1&namespace=0&format=json";
            var url = $"{_baseUrl}/page/summary/{Uri.EscapeDataString(articleName)}";
            try
            {
                var res = await _httpClient.GetAsync(url);
                res.EnsureSuccessStatusCode();
                var content = await res.Content.ReadFromJsonAsync<JsonElement>();
                ArticleReply reply = new();
                reply.Content = content.GetProperty("extract").GetString();
                reply.Link = content.GetProperty("content_urls").GetProperty("desktop").GetProperty("page").GetString();
                return reply;
            }
            catch (Exception e) { Console.WriteLine($"Exception oquired: {e.Message}"); return new ArticleReply(); }
        }
    }
}
