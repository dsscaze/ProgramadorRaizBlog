using Microsoft.AspNetCore.Mvc;
using ProgramadorRaizBlogCore.Models;
using ProgramadorRaizBlogCore.Helpers;
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

    public IActionResult Details(string slug)
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

        // Formatar o corpo do post
        post.Body = MarkdownHelper.FormatarCorpoPost(post.Body);

        return View(post);
    }
}
