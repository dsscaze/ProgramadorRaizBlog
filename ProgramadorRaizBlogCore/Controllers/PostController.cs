using Microsoft.AspNetCore.Mvc;
using ProgramadorRaizBlogCore.Models;
using System.Diagnostics;
using TabNewsClientCore;

namespace ProgramadorRaizBlogCore.Controllers;

public class PostController : Controller
{
    private readonly ILogger<PostController> _logger;
    private readonly IConfiguration _configuration;


    public PostController(ILogger<PostController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<IActionResult> Details(string slug)
    {
        if (string.IsNullOrEmpty(slug))
        {
            return NotFound();
        }

        var nomeUsuarioTabNews = _configuration["TabNews:NomeUsuario"] ?? "programadorraiz";
        var post = TabNewsApi.GetContent(nomeUsuarioTabNews, slug);

        if (post == null)
        {
            return NotFound();
        }

        return View(post);
    }
}
