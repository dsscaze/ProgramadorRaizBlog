using Microsoft.AspNetCore.Mvc;
using ProgramadorRaizBlogCore.Helpers;
using ProgramadorRaizBlogCore.Services;

namespace ProgramadorRaizBlogCore.Controllers;

public class AdminController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdminController> _logger;
    private readonly ITokenStorageService _tokenStorage;

    public AdminController(
        IConfiguration configuration, 
        ILogger<AdminController> logger,
        ITokenStorageService tokenStorage)
    {
        _configuration = configuration;
        _logger = logger;
        _tokenStorage = tokenStorage;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        try
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email e senha são obrigatórios");
                return View();
            }

            _logger.LogInformation("Tentando fazer login com email: {Email}", email);

            // Fazer login usando o helper
            var userSession = await TabNewsHttpHelper.LoginUserAsync(email, password);

            if (userSession?.Token == null)
            {
                ModelState.AddModelError("", "Falha ao fazer login");
                return View();
            }

            _logger.LogInformation("Login bem-sucedido! Token: {Token}", userSession.Token.Substring(0, 10) + "...");

            // Salvar o token em arquivo
            await _tokenStorage.SaveTokenAsync(userSession.Token);

            ViewBag.Token = userSession.Token;
            ViewBag.Message = "✅ Login bem-sucedido! Token salvo com sucesso.";

            return View("LoginSuccess", userSession);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao fazer login");
            ModelState.AddModelError("", $"❌ Erro: {ex.Message}");
            return View();
        }
    }
}
