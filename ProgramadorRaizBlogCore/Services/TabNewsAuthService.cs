using Microsoft.Extensions.Caching.Memory;
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
    private const string SessionCacheKey = "TabNewsSessionId";
    private const string UserCacheKey = "TabNewsUser";

    public TabNewsAuthService(
        IMemoryCache cache,
        IConfiguration configuration,
        ILogger<TabNewsAuthService> logger)
    {
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> GetSessionIdAsync()
    {
        // Tentar obter do cache
        if (_cache.TryGetValue(SessionCacheKey, out string? cachedSessionId) && !string.IsNullOrEmpty(cachedSessionId))
        {
            _logger.LogInformation("Session ID obtido do cache");
            return cachedSessionId;
        }

        // Se não estiver no cache, fazer login
        _logger.LogInformation("Session ID não encontrado no cache. Fazendo login...");
        var sessionId = await LoginAndCacheAsync();
        return sessionId;
    }

    public async Task<TabNewsUser> GetAuthenticatedUserAsync()
    {
        // Tentar obter do cache
        if (_cache.TryGetValue(UserCacheKey, out TabNewsUser? cachedUser) && cachedUser != null)
        {
            _logger.LogInformation("Usuário obtido do cache");
            return cachedUser;
        }

        // Se não estiver no cache, obter do TabNews
        _logger.LogInformation("Usuário não encontrado no cache. Obtendo do TabNews...");
        var sessionId = await GetSessionIdAsync();
        
        var user = await Task.Run(() => TabNewsApi.GetUser(sessionId));
        
        // Armazenar no cache por 23 horas (menos que a sessão para garantir renovação)
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
                throw new InvalidOperationException("Credenciais do TabNews não configuradas no appsettings.json");
            }

            // Fazer login de forma assíncrona
            var userSession = await Task.Run(() => TabNewsApi.LoginUser(email, password));

            if (string.IsNullOrEmpty(userSession?.Token))
            {
                throw new InvalidOperationException("Login falhou: Token não retornado");
            }

            // Armazenar no cache por 23 horas (sessões do TabNews duram ~24h)
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromHours(23))
                .RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    _logger.LogInformation("Session ID removido do cache. Motivo: {Reason}", reason);
                });

            _cache.Set(SessionCacheKey, userSession.Token, cacheOptions);
            
            _logger.LogInformation("Login realizado com sucesso. Session ID armazenado no cache.");

            return userSession.Token;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer login no TabNews");
            throw;
        }
    }
}
