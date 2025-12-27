using Microsoft.Extensions.Caching.Memory;
using ProgramadorRaizBlogCore.Helpers;
using TabNewsClientCore;
using TabNewsClientCore.Entities;

namespace ProgramadorRaizBlogCore.Services;

public interface ITabNewsAuthService
{
    Task<string> GetSessionIdAsync();
    Task<TabNewsUser> GetAuthenticatedUserAsync();
}

public class TabNewsAuthService : ITabNewsAuthService
{
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TabNewsAuthService> _logger;
    private readonly ITokenStorageService _tokenStorage;
    private const string SessionCacheKey = "TabNewsSessionId";
    private const string UserCacheKey = "TabNewsUser";

    public TabNewsAuthService(
        IMemoryCache cache,
        IConfiguration configuration,
        ILogger<TabNewsAuthService> logger,
        ITokenStorageService tokenStorage)
    {
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
        _tokenStorage = tokenStorage;
    }

    public async Task<string> GetSessionIdAsync()
    {
        // 1. Priorizar token configurado em appsettings.json ou environment variables
        var configuredToken = _configuration["TabNews:SessionToken"];
        if (!string.IsNullOrEmpty(configuredToken) && !configuredToken.StartsWith("OBTER_EM_"))
        {
            _logger.LogInformation("Usando SessionToken configurado");
            return configuredToken;
        }

        // 2. Tentar obter do cache (mais rápido)
        if (_cache.TryGetValue(SessionCacheKey, out string? cachedSessionId) && !string.IsNullOrEmpty(cachedSessionId))
        {
            _logger.LogInformation("Session ID obtido do cache");
            return cachedSessionId;
        }

        // 3. Tentar obter do arquivo (token salvo pelo admin)
        var storedToken = await _tokenStorage.GetTokenAsync();
        if (!string.IsNullOrEmpty(storedToken))
        {
            _logger.LogInformation("Session ID obtido do arquivo tokens.json");
            
            // Colocar no cache também
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(23));
            _cache.Set(SessionCacheKey, storedToken, cacheOptions);
            
            return storedToken;
        }

        // 4. Falhar com instrução clara
        throw new InvalidOperationException(
            "Token não encontrado. Configure em:\n" +
            "1. User Secrets: dotnet user-secrets set \"TabNews:SessionToken\" \"seu-token\"\n" +
            "2. Ou acesse /admin/login e faça login no seu PC (que não é bloqueado pelo Cloudflare)");
    }

    public async Task<TabNewsUser> GetAuthenticatedUserAsync()
    {
        // Tentar obter do cache
        if (_cache.TryGetValue(UserCacheKey, out TabNewsUser? cachedUser) && cachedUser != null)
        {
            _logger.LogInformation("Usuário obtido do cache");
            return cachedUser;
        }

        // Se não estiver no cache, obter do TabNews usando username público
        _logger.LogInformation("Usuário não encontrado no cache. Obtendo do TabNews...");
        var username = _configuration["TabNews:NomeUsuario"] ?? "programadorraiz";
        
        var user = await Task.Run(() => TabNewsApi.GetUser(username));
        
        // Armazenar no cache por 23 horas
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromHours(23));
        
        _cache.Set(UserCacheKey, user, cacheOptions);
        
        return user;
    }

    private async Task<string> LoginAndCacheAsync()
    {
        try
        {
            var email = _configuration["TabNews:Email"];
            var password = _configuration["TabNews:Password"];

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                throw new InvalidOperationException("Credenciais do TabNews não configuradas. Faça login em /admin/login");
            }

            _logger.LogInformation("Tentando login com email: {Email}", email);

            // Fazer login usando o helper com User-Agent e cookies
            var userSession = await TabNewsHttpHelper.LoginUserAsync(email, password);

            if (string.IsNullOrEmpty(userSession?.Token))
            {
                throw new InvalidOperationException("Login falhou: Token não retornado");
            }

            _logger.LogInformation("Login realizado com sucesso! Token: {Token}", userSession.Token.Substring(0, 10) + "...");

            // Salvar em arquivo para próximas requisições
            await _tokenStorage.SaveTokenAsync(userSession.Token);

            // Armazenar no cache por 23 horas
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(23));
            _cache.Set(SessionCacheKey, userSession.Token, cacheOptions);
            
            _logger.LogInformation("Token armazenado em cache e arquivo.");

            return userSession.Token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer login no TabNews. Acesse /admin/login para autenticar");
            throw;
        }
    }
}
