using System.Text.Json;

namespace ProgramadorRaizBlogCore.Services;

public interface ITokenStorageService
{
    Task<string?> GetTokenAsync();
    Task SaveTokenAsync(string token);
    Task<bool> IsTokenExpiredAsync();
}

public class FileTokenStorageService : ITokenStorageService
{
    private readonly string _tokenFilePath;
    private readonly ILogger<FileTokenStorageService> _logger;

    public FileTokenStorageService(ILogger<FileTokenStorageService> logger)
    {
        _logger = logger;
        // Armazenar em /var/www/programadorraiz.com.br/tokens.json em produção
        _tokenFilePath = Path.Combine(AppContext.BaseDirectory, "tokens.json");
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            if (!File.Exists(_tokenFilePath))
            {
                _logger.LogInformation("Arquivo de tokens não encontrado");
                return null;
            }

            var json = await File.ReadAllTextAsync(_tokenFilePath);
            var tokenData = JsonSerializer.Deserialize<TokenData>(json);

            if (tokenData?.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning("Token expirado");
                return null;
            }

            _logger.LogInformation("Token carregado do arquivo");
            return tokenData?.Token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao ler arquivo de tokens");
            return null;
        }
    }

    public async Task SaveTokenAsync(string token)
    {
        try
        {
            var tokenData = new TokenData
            {
                Token = token,
                SavedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7) // Tokens do TabNews duram ~30 dias, mas renovar a cada 7
            };

            var json = JsonSerializer.Serialize(tokenData, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_tokenFilePath, json);

            // Definir permissões apenas para leitura (security)
            File.SetAttributes(_tokenFilePath, FileAttributes.Normal);

            _logger.LogInformation("Token salvo com sucesso. Expira em: {ExpiresAt}", tokenData.ExpiresAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar token");
            throw;
        }
    }

    public async Task<bool> IsTokenExpiredAsync()
    {
        var token = await GetTokenAsync();
        return string.IsNullOrEmpty(token);
    }

    private class TokenData
    {
        public string? Token { get; set; }
        public DateTime SavedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
