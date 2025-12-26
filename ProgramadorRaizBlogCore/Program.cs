
using ProgramadorRaizBlogCore.Services;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao container
builder.Services.AddControllersWithViews();

// Adicionar cache em memória
builder.Services.AddMemoryCache();

// Registrar serviços de autenticação do TabNews
builder.Services.AddSingleton<ITokenStorageService, FileTokenStorageService>();
builder.Services.AddSingleton<ITabNewsAuthService, TabNewsAuthService>();

var app = builder.Build();

// Configurar o pipeline de requisições HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
