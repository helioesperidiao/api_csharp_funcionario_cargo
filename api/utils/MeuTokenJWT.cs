// Importa namespaces necessários para manipulação de tokens JWT e segurança
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace Api.Utils
{
    /// <summary>
    /// Classe responsável por gerar e validar tokens JWT para autenticação.
    /// 🔹 Pode ser usada em filtros de autorização ou serviços de login.
    /// </summary>
    public class MeuTokenJWT
    {
        // -------------------- Informações do usuário armazenadas no token --------------------
        public string Email { get; set; } = string.Empty;          // Email do usuário
        public string Role { get; set; } = string.Empty;           // Cargo ou perfil do usuário
        public string Name { get; set; } = string.Empty;           // Nome completo do usuário
        public string IdFuncionario { get; set; } = string.Empty; // Identificador único do funcionário

        // -------------------- Configurações do token JWT --------------------
        public string SecretKey { get; set; } = "x9S4q0v+V0IjvHkG20uAxaHx1ijj+q1HWjHKv+ohxp/oK+77qyXkVj/l4QYHHTF3"; // Chave secreta para assinatura
        public string Algorithm { get; set; } = SecurityAlgorithms.HmacSha256; // Algoritmo de assinatura
        public string Issuer { get; set; } = "http://localhost";   // Emissor do token
        public string Audience { get; set; } = "http://localhost"; // Destinatário do token
        public string Subject { get; set; } = "acesso_sistema";    // Assunto do token
        public TimeSpan TempoDuracaoToken { get; set; } = TimeSpan.FromDays(30); // Validade do token

        // -------------------- Geração de token JWT --------------------
        /// <summary>
        /// Gera um token JWT baseado em um dicionário de claims (informações do usuário)
        /// 🔹 Claims são pares chave-valor que serão embutidos no token
        /// </summary>
        /// <param name="claims">Dicionário com informações adicionais</param>
        /// <returns>Token JWT como string</returns>
        public string GerarToken(Dictionary<string, object> claims)
        {
            // Cria a chave secreta e credenciais de assinatura
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(SecretKey));
            SigningCredentials credentials = new SigningCredentials(securityKey, Algorithm);

            // Cria o descriptor do token, que contém informações básicas e claims
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                // Claims padrão do token
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Iss, Issuer), // Emissor
                    new Claim(JwtRegisteredClaimNames.Aud, Audience), // Destinatário
                    new Claim(JwtRegisteredClaimNames.Sub, Subject), // Assunto
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()), // Data de emissão
                    new Claim(JwtRegisteredClaimNames.Exp, DateTime.UtcNow.Add(TempoDuracaoToken).ToString()), // Expiração
                    new Claim("jti", Guid.NewGuid().ToString()), // ID único do token
                }),
                Expires = DateTime.UtcNow.Add(TempoDuracaoToken), // Expiração real do token
                SigningCredentials = credentials // Credenciais de assinatura
            };

            // Adiciona claims personalizadas fornecidas pelo serviço
            foreach (var claim in claims)
            {
                string claimValue = claim.Value?.ToString() ?? string.Empty;
                tokenDescriptor.Subject.AddClaim(new Claim(claim.Key, claimValue));
            }

            // Cria o token JWT
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            // Retorna o token como string
            return tokenHandler.WriteToken(token);
        }

        // -------------------- Validação de token JWT --------------------
        /// <summary>
        /// Valida um token JWT recebido.
        /// 🔹 Verifica assinatura, emissor, audiência e expiração
        /// </summary>
        /// <param name="tokenString">Token JWT como string</param>
        /// <returns>true se válido, false caso contrário</returns>
        public bool ValidarToken(string tokenString)
        {
            if (string.IsNullOrWhiteSpace(tokenString))
                return false;

            // Remove prefixo "Bearer" caso exista
            tokenString = tokenString.Replace("Bearer", "").Trim();
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                // Parâmetros de validação
                tokenHandler.ValidateToken(tokenString, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(SecretKey)),
                    ValidateIssuer = true,
                    ValidIssuer = Issuer,
                    ValidateAudience = true,
                    ValidAudience = Audience,
                    ClockSkew = TimeSpan.Zero // Sem tolerância de tempo
                }, out SecurityToken validatedToken);

                // Extrai claims do token validado
                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    Email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? string.Empty;
                    Role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value ?? string.Empty;
                    Name = jwtToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? string.Empty;
                    IdFuncionario = jwtToken.Claims.FirstOrDefault(c => c.Type == "idFuncionario")?.Value ?? string.Empty;
                }

                return true; // Token válido
            }
            catch (SecurityTokenExpiredException)
            {
                Console.WriteLine("Token expired"); // Token expirado
            }
            catch (SecurityTokenException)
            {
                Console.WriteLine("Invalid token"); // Token inválido
            }
            catch (Exception ex)
            {
                Console.WriteLine("General error: " + ex.Message); // Outros erros
            }

            return false; // Token inválido ou erro
        }
    }
}
