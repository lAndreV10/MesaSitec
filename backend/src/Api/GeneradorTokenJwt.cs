using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Dominio.Entidades;
using Microsoft.IdentityModel.Tokens;

namespace Api;

public static class GeneradorTokenJwt
{
    public const int ExpiraEnSegundos = 8 * 60 * 60;

    public static string Crear(
        Usuario usuario,
        byte[] claveJwt)
    {
        var claims = new[]
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                usuario.Id.ToString()),
            new Claim("tenantId", usuario.TenantId.ToString()),
            new Claim("rol", usuario.Rol),
            new Claim(
                JwtRegisteredClaimNames.Email,
                usuario.Email)
        };

        var claveFirma = new SymmetricSecurityKey(claveJwt);

        var credencialesFirma = new SigningCredentials(
            claveFirma,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credencialesFirma);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
