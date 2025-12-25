using Microsoft.AspNetCore.Mvc;
using ProgramadorRaizBlogCore.Helpers;
using ProgramadorRaizBlogCore.Models;
using ProgramadorRaizBlogCore.Services;

namespace ProgramadorRaizBlogCore.Controllers;

public class AutorController : Controller
{
    private readonly ILogger<AutorController> _logger;
    private readonly ITabNewsAuthService _authService;

    public AutorController(ILogger<AutorController> logger, ITabNewsAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var usuario = await _authService.GetAuthenticatedUserAsync();

            // Criar ViewModel para não modificar o objeto cacheado
            var viewModel = new AutorViewModel
            {
                Username = usuario.Username,
                Description = !string.IsNullOrEmpty(usuario.Description) 
                    ? MarkdownHelper.FormatarCorpoPost(usuario.Description)
                    : string.Empty,
                TabCoins = usuario.TabCoins,
                TabCash = usuario.TabCash,
                CreatedAt = usuario.CreatedAt
            };

            return View(viewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao carregar dados do autor");
            return View(new AutorViewModel { Description = "Erro ao carregar informações do autor." });
        }
    }
}