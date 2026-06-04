using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using TesteVeste.Application.Interfaces;
using TesteVeste.Application.Notifications;
using TesteVeste.Application.Services;
using TesteVeste.Domain.Interfaces;
using TesteVeste.Infrastructure.Data;
using TesteVeste.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// OpenAPI spec (gerada pelo Swashbuckle) + configuração de Bearer para o Scalar
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "TesteVeste API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "Bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "Informe o token JWT.\nObtido em: POST /api/auth/token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey  = jwtSection["SecretKey"]
    ?? throw new InvalidOperationException("Jwt:SecretKey não configurado.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtSection["Issuer"],
            ValidAudience            = jwtSection["Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

builder.Services.AddAuthorization();

// Banco de dados em memória (não requer SQL Server instalado)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TesteVesteDb"));

// Repositórios
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();

// Notificações (escopo por requisição)
builder.Services.AddScoped<INotificationService, NotificationService>();

// Serviços de aplicação
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();

var app = builder.Build();

// Seed de dados iniciais
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DataSeeder.Seed(db);
}

// Gera o JSON da spec OpenAPI em /swagger/v1/swagger.json
app.UseSwagger();

// Scalar — UI moderna de documentação e teste de API
// Acesse: http://localhost:5285/scalar/v1
app.MapScalarApiReference(options =>
{
    options.Title              = "TesteVeste API";
    options.Theme              = ScalarTheme.Purple;
    options.DefaultHttpClient  = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
    options.Authentication     = new ScalarAuthenticationOptions
    {
        PreferredSecuritySchemes = ["Bearer"]
    };
    // Aponta para a spec gerada pelo Swashbuckle
    options.OpenApiRoutePattern = "/swagger/v1/swagger.json";
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
