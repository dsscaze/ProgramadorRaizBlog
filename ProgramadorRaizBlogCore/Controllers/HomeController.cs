using Microsoft.AspNetCore.Mvc;
using ProgramadorRaizBlogCore.Models;
using System.Diagnostics;
using TabNewsClientCore;

namespace ProgramadorRaizBlogCore.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<IActionResult> Index()
    {
        var nomeUsuarioTabNews = _configuration["TabNews:NomeUsuario"] ?? "programadorraiz";
        var posts = TabNewsApi.Get10LastedPosts("programadorraiz");

        return View(posts);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

