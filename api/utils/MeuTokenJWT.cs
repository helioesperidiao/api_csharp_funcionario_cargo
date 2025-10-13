// Importa namespaces necessários para manipulação de tokens JWT e segurança
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Api.Utils
{
    // Define a classe MeuTokenJWT que será responsável por gerar e validar tokens JWT
    public class MeuTokenJWT
    {
        // Propriedades que armazenam informações relacionadas ao token JWT
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string IdFuncionario { get; set; } = string.Empty;

        // Propriedades relacionadas à geração do token JWT
        public string SecretKey { get; set; } = "x9S4q0v+V0IjvHkG20uAxaHx1ijj+q1HWjHKv+ohxp/oK+77qyXkVj/l4QYHHTF3";
        public string Algorithm { get; set; } = SecurityAlgorithms.HmacSha256;
        public string Issuer { get; set; } = "http://localhost";
        public string Audience { get; set; } = "http://localhost";
        public string Subject { get; set; } = "acesso_sistema";
        public TimeSpan TempoDuracaoToken { get; set; } = TimeSpan.FromDays(30);

        // Método que gera um token JWT baseado em um dicionário de claims
        public string GerarToken(Dictionary<string, object> claims)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(SecretKey));
            var credentials = new SigningCredentials(securityKey, Algorithm);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Iss, Issuer),
                    new Claim(JwtRegisteredClaimNames.Aud, Audience),
                    new Claim(JwtRegisteredClaimNames.Sub, Subject),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                    new Claim(JwtRegisteredClaimNames.Exp, DateTime.UtcNow.Add(TempoDuracaoToken).ToString()),
                    new Claim("jti", Guid.NewGuid().ToString()),
                }),
                Expires = DateTime.UtcNow.Add(TempoDuracaoToken),
                SigningCredentials = credentials
            };

            // Adiciona claims personalizadas, garantindo que o valor nunca seja nulo
            foreach (var claim in claims)
            {
                var claimValue = claim.Value?.ToString() ?? string.Empty;
                tokenDescriptor.Subject.AddClaim(new Claim(claim.Key, claimValue));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        // Método que valida um token JWT recebido
        public bool ValidarToken(string tokenString)
        {
            if (string.IsNullOrWhiteSpace(tokenString))
                return false;

            tokenString = tokenString.Replace("Bearer", "").Trim();
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(tokenString, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(SecretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = Issuer,
                    ValidateAudience = true,
                    ValidAudience = Audience,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                // Cast seguro usando 'is not null'
                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    Email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? string.Empty;
                    Role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? string.Empty;
                    Name = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? string.Empty;
                    IdFuncionario = jwtToken.Claims.FirstOrDefault(c => c.Type == "idFuncionario")?.Value ?? string.Empty;
                }

                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                Console.WriteLine("Token expired");
            }
            catch (SecurityTokenException)
            {
                Console.WriteLine("Invalid token");
            }
            catch (Exception ex)
            {
                Console.WriteLine("General error: " + ex.Message);
            }

            return false;
        }
    }
}
