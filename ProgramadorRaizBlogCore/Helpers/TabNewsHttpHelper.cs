using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using TabNewsClientCore.Entities;

namespace ProgramadorRaizBlogCore.Helpers;

public static class TabNewsHttpHelper
{
    private static readonly HttpClient HttpClient;
    private const string BaseUrl = "https://www.tabnews.com.br/api/v1";

    static TabNewsHttpHelper()
    {
        // Criar handler com suporte a cookies (OBRIGATÓRIO para Cloudflare)
        var cookieContainer = new CookieContainer();
        var handler = new HttpClientHandler
        {
            CookieContainer = cookieContainer,
            UseCookies = true,
            AllowAutoRedirect = true
        };

        HttpClient = new HttpClient(handler);

        // Adicionar headers para parecer um navegador real e passar pelo Cloudflare
        HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
        HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
        HttpClient.DefaultRequestHeaders.Add("Accept-Language", "pt-BR,pt;q=0.9,en-US;q=0.8,en;q=0.7");
        HttpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
        HttpClient.DefaultRequestHeaders.Add("DNT", "1");
        HttpClient.DefaultRequestHeaders.Add("Origin", "https://www.tabnews.com.br");
        HttpClient.DefaultRequestHeaders.Add("Referer", "https://www.tabnews.com.br/");
        HttpClient.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
        HttpClient.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
        HttpClient.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
        HttpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
        HttpClient.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");
    }

    public static async Task<TabNewsUserSession?> LoginUserAsync(string email, string password)
    {
        try
        {
            var payload = new { email, password };
            var content = new StringContent(
                JsonConvert.SerializeObject(payload),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await HttpClient.PostAsync($"{BaseUrl}/sessions", content);
            
            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Login falhou com status {response.StatusCode}: {responseContent.Substring(0, Math.Min(200, responseContent.Length))}");
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var session = JsonConvert.DeserializeObject<TabNewsUserSession>(jsonResponse);

            return session;
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro ao fazer login: {ex.Message}", ex);
        }
    }
}
