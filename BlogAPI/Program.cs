using BlogApi.BLL.Interface;
using BlogApi.BLL.Interfaces;
using BlogApi.BLL.Services;
using BlogApi.BLL.Validators.Auth;
using BlogApi.DAL.Entities;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;
using System.Text;

// ==========================================================
// BOOTSTRAP LOGGER
// ==========================================================

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting BlogAPI...");

    var builder = WebApplication.CreateBuilder(args);

    // ==========================================================
    // SERILOG
    // ==========================================================

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.FromLogContext());

    // ==========================================================
    // CONTROLLERS
    // ==========================================================

    builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidationFilter>();
    });

    // ==========================================================
    // OPEN API
    // ==========================================================

    builder.Services.AddOpenApi();

    // ==========================================================
    // DATABASE
    // ==========================================================

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString(
                "DefaultConnection")));

    // ==========================================================
    // FLUENT VALIDATION
    // ==========================================================

    builder.Services.AddValidatorsFromAssemblyContaining<
        RegisterDtoValidator>();

    // ==========================================================
    // JWT AUTHENTICATION
    // ==========================================================

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;

            options.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,

                    ValidateAudience = true,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,

                    ValidIssuer =
                        builder.Configuration[
                            "JwtSettings:Issuer"],

                    ValidAudience =
                        builder.Configuration[
                            "JwtSettings:Audience"],

                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                builder.Configuration[
                                    "JwtSettings:SecretKey"]!
                            )
                        )
                };
        });

    builder.Services.AddAuthorization();

    // ==========================================================
    // BLL SERVICES
    // ==========================================================

    builder.Services.AddScoped<IAuthService, AuthService>();

    builder.Services.AddScoped<IJwtService, JwtService>();

    builder.Services.AddScoped<IPostService, PostService>();

    builder.Services.AddScoped<ICategoryService, CategoryService>();

    builder.Services.AddScoped<ICommentService, CommentService>();

    builder.Services.AddScoped<IUserService, UserService>();

    builder.Services.AddScoped<IPostTagService, PostTagService>();

    builder.Services.AddScoped<ITagService, TagService>();

    // ==========================================================
    // REPOSITORIES
    // ==========================================================

    builder.Services.AddScoped<ITagRepository, TagRepository>();

    builder.Services.AddScoped<IPostTagRepository, PostTagRepository>();

    builder.Services.AddScoped<ICommentRepository, CommentRepository>();

    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

    builder.Services.AddScoped<IUserRepository, UserRepository>();

    builder.Services.AddScoped<IPostRepository, PostRepository>();

    // ==========================================================
    // BUILD APPLICATION
    // ==========================================================

    var app = builder.Build();

    // ==========================================================
    // DATABASE MIGRATION
    // ==========================================================

    using (var scope = app.Services.CreateScope())
    {
        var db =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

        // Existing migrations automatically apply hongi.
        await db.Database.MigrateAsync();

        // ======================================================
        // DEFAULT ADMIN ACCOUNT
        // ======================================================

        const string adminEmail =
            "admin@blogapi.com";

        const string adminUsername =
            "admin";

        const string adminPassword =
            "Admin@123";

        var adminExists =
            await db.Users.AnyAsync(
                u => u.Email == adminEmail);

        if (!adminExists)
        {
            var admin = new User
            {
                Username = adminUsername,

                Email = adminEmail,

                Role = "Admin",

                CreatedAt = DateTime.UtcNow
            };

            var passwordHasher =
                new PasswordHasher<User>();

            admin.PasswordHash =
                passwordHasher.HashPassword(
                    admin,
                    adminPassword);

            db.Users.Add(admin);

            await db.SaveChangesAsync();

            Log.Information(
                "Default Admin account created: {Email}",
                adminEmail);
        }
    }

    // ==========================================================
    // GLOBAL EXCEPTION HANDLER
    // ==========================================================

    app.UseGlobalExceptionHandler();

    // ==========================================================
    // REQUEST LOGGING
    // ==========================================================

    app.UseSerilogRequestLogging();

    // ==========================================================
    // OPEN API / SCALAR
    // ==========================================================

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();

        app.MapScalarApiReference();
    }

    // ==========================================================
    // HTTP PIPELINE
    // ==========================================================

    app.UseHttpsRedirection();

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();

    // ==========================================================
    // RUN
    // ==========================================================

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(
        ex,
        "BlogAPI terminated unexpectedly on startup.");
}
finally
{
    Log.CloseAndFlush();
}
