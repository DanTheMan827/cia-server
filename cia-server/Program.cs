using cia_server.Data;
using cia_server.Shared;
using cia_server.Shared.CIA;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;

Debug.WriteLine(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot"));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

AppPaths.SetEnvironment(app.Environment);
CiaList.InitializeShared();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".cia"] = "application/octet-stream";
provider.Mappings[".js"] = "application/x-javascript";

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(AppPaths.CiaServerPath),
    ContentTypeProvider = provider,
    RequestPath = "/cia"
});

app.Run();
