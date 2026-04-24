using System.Text;
using System.Text.Json;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MediSync.Infrastructure.Persistence;
using MediSync.API.Middleware;

var builder = WebApplication.CreateBuilder(args);
var config  = builder.Configuration;

// ── 1. Controllers + JSON ──────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(o => {
        o.JsonSerializerOptions.PropertyNamingPolicy        = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.DefaultIgnoreCondition      =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// ── 2. PostgreSQL ──────────────────────────────────
var connectionString =
    config.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

    if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("Database connection string is missing!");
}

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connectionString));

// ── 3. Redis ───────────────────────────────────────
builder.Services.AddStackExchangeRedisCache(opt =>
    opt.Configuration = config.GetConnectionString("Redis"));

// ── 3.1 HttpClient ─────────────────────────────────  ← أضفه هنا
builder.Services.AddHttpClient();

// ── 4. JWT Authentication ──────────────────────────
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt => {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = config["Jwt:Issuer"],
            ValidAudience            = config["Jwt:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Jwt:Secret"]!))
        };
    });

builder.Services.AddAuthorization();

// ── 5. MediatR ─────────────────────────────────────
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(MediSync.Application.Features.Auth.Commands.LoginCommand).Assembly));

// ── 6. FluentValidation ────────────────────────────
builder.Services.AddFluentValidationAutoValidation();

// ── 7. AutoMapper ──────────────────────────────────
//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// ── 8. CORS ────────────────────────────────────────
builder.Services.AddCors(opt =>
    opt.AddPolicy("AllowFrontend", p =>
        p.WithOrigins("http://localhost:3000", "https://medisync.vercel.app")
         .AllowAnyHeader()
         .AllowAnyMethod()
         .AllowCredentials()));

// ── 9. Swagger ─────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title       = "MediSync API",
        Version     = "v1",
        Description = "منصة إدارة الأدوية الذكية"
    });

    opt.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        In           = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description  = "Bearer {token}"
    });

    opt.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {{
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Reference = new Microsoft.OpenApi.Models.OpenApiReference
            {
                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                Id   = "Bearer"
            }
        },
        Array.Empty<string>()
    }});
});

// ── 10. Register Services ──────────────────────────
builder.Services.AddScoped<MediSync.Application.Services.ITokenService,
                            MediSync.Infrastructure.Services.TokenService>();
                            
builder.Services.AddScoped<MediSync.Application.Persistence.IApplicationDbContext>(
    provider => provider.GetRequiredService<MediSync.Infrastructure.Persistence.AppDbContext>());                          

// ── Build ──────────────────────────────────────────
var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(o => {
        o.SwaggerEndpoint("/swagger/v1/swagger.json", "MediSync API v1");
        o.EnableTryItOutByDefault();
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health Endpoint
app.MapGet("/health", () => Results.Ok(new { status = "ok", time = DateTime.UtcNow }));

// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
