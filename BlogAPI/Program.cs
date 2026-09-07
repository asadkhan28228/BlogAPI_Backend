using BlogApi.BLL.Interface;
using BlogApi.BLL.Interfaces;
using BlogApi.BLL.Services;
using BlogApi.BLL.Validators.Auth;
using BlogApi.DAL.InterfacesRepositories;
using BlogApi.DAL.Repositories;
using BlogAPI.BLL.Services;
using BlogAPI.DAL.Data;
using BlogAPI.DAL.Repositories;
using BlogAPI.Filters;
using BlogAPI.Middleware;
using EMSBLL.Interfaces;
using EMSBLL.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Text;

// ==========================================================
// BOOTSTRAP LOGGER - startup ke crashes bhi catch karta hai
// ==========================================================
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting BlogAPI...");

    var builder = WebApplication.CreateBuilder(args);

    // ==========================================================
    // SERILOG - appsettings.json se poori config load karta hai
    // ==========================================================
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext());

    // Add services to the container.

    builder.Services.AddControllers(options =>
    {
        // Har [FromBody] DTO ko uske FluentValidation validator se check karega
        options.Filters.Add<ValidationFilter>();
    });

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    builder.Services.AddOpenApi();

    // DbContext register karna
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    // FluentValidation - BLL assembly ke andar sab AbstractValidator<T> dhoondh ke register karta hai
    builder.Services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();

    // Jwt Configuration
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,

            ValidateAudience = true,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],      // "BlogAPI"
            ValidAudience = builder.Configuration["JwtSettings:Audience"],  // "BlogAPIUsers"

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["JwtSettings:SecretKey"]!
                )
            )
        };
    });

    builder.Services.AddAuthorization();


    builder.Services.AddScoped<IAuthService, AuthService>();

    builder.Services.AddScoped<IJwtService, JwtService>();
    builder.Services.AddScoped<IPostService, PostService>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<ICommentService, CommentService>();
    builder.Services.AddScoped<ITagRepository, TagRepository>();
    builder.Services.AddScoped<IPostTagRepository, PostTagRepository>();

    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IPostTagService, PostTagService>();
    builder.Services.AddScoped<ITagService, TagService>();
    builder.Services.AddScoped<ICommentRepository, CommentRepository>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
    builder.Services.AddScoped<IPostRepository, PostRepository>();

    var app = builder.Build();

    // ==========================================================
    // GLOBAL EXCEPTION HANDLER - pipeline ka SABSE PEHLA middleware
    // ==========================================================
    app.UseGlobalExceptionHandler();

    // Har HTTP request (method, path, status, time) log karta hai
    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "BlogAPI terminated unexpectedly on startup.");
}
finally
{
    Log.CloseAndFlush();
}