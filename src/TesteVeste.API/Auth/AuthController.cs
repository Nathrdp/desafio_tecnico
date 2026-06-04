using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace TesteVeste.API.Auth;

/// <summary>
/// Gera um token JWT para autenticar nas demais rotas.
/// POST /api/auth/token  →  { "usuario": "admin", "senha": "123456" }
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;

    // Credenciais fixas para o desafio (não use isso em produção!)
    private const string UsuarioValido = "admin";
    private const string SenhaValida   = "123456";

    public AuthController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Autentica com usuário e senha e retorna um Bearer token JWT.
    /// Use: usuario = "admin" / senha = "123456"
    /// </summary>
    [HttpPost("token")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Token([FromBody] LoginRequest request)
    {
        if (request.Usuario != UsuarioValido || request.Senha != SenhaValida)
            return Unauthorized(new { message = "Usuário ou senha inválidos." });

        var token = GerarToken(request.Usuario);
        return Ok(new { token, tipo = "Bearer" });
    }

    private string GerarToken(string usuario)
    {
        var secretKey  = _configuration["Jwt:SecretKey"]
            ?? throw new InvalidOperationException("Jwt:SecretKey não configurado.");
        var issuer     = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Jwt:Issuer não configurado.");
        var audience   = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Jwt:Audience não configurado.");
        var expiration = int.TryParse(_configuration["Jwt:ExpirationMinutes"], out var min) ? min : 60;

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, usuario),
            new Claim(ClaimTypes.Role, "candidato"),
        };

        var jwt = new JwtSecurityToken(
            issuer:             issuer,
            audience:           audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(expiration),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
