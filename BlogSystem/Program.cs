using System.Text;
using System.Text.Json.Serialization;
using BlogSystem.Common.Mappings;
using BlogSystem.Infrastructure.ImageService;
using BlogSystem.Shared.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SixLabors.ImageSharp.Web.Caching;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Middleware;
using SixLabors.ImageSharp.Web.Providers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(PostMappingProfile));

// Register Authentication Service
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["JWT_SecretKey"] ?? throw new InvalidOperationException("JWT_SecretKey is not configured"))
            ),
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireClaim("Role", "Admin"));
    options.AddPolicy("Editor", policy => policy.RequireClaim("Role", "Editor"));
});

// Configure JSON serialization options
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Configure ImageSharp
builder.Services.AddImageSharp()
    .AddProvider<PostImageProvider>()
    .RemoveProvider<PhysicalFileSystemProvider>()
    .AddProvider<PhysicalFileSystemProvider>()
    .Configure<PhysicalFileSystemCacheOptions>(options =>
    {
        options.CacheRootPath = Path.Combine("Content", "cache");
    })
    .Configure<ImageSharpMiddlewareOptions>(options =>
    {
        options.OnPrepareResponseAsync = context =>
        {
            context.Response.Headers.CacheControl = "public, max-age=31536000"; // Cache for 1 year
            return Task.CompletedTask;
        };
    });


// Add services to the container.
builder.Services.AddServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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
