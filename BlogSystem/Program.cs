using BlogSystem.Shared.Extensions;
using DotNetEnv;
using SixLabors.ImageSharp.Web.DependencyInjection;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Add services to the container.
builder.Services.AddServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "BlogSystem API V1");
    });
}

if (!Directory.Exists("Content"))
{
    Directory.CreateDirectory("Content");
}

app.UseImageSharp();
app.UseFileServer();

app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();

app.Run();
