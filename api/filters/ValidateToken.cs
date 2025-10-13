using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Api.Utils;

namespace Api.Filters
{
    /// <summary>
    /// Filtro de autorização que valida tokens JWT usando a classe MeuTokenJWT.
    /// Pode ser aplicado em controladores ou métodos específicos.
    /// </summary>
    public class ValidateToken : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            Console.WriteLine("🔐 ValidateTokenAttribute executando...");

            // Obtém o cabeçalho Authorization
            string? authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(authHeader))
            {
                context.Result = new JsonResult(new
                {
                    erro = "Cabeçalho 'Authorization' ausente."
                })
                { StatusCode = 401 };
                return;
            }

            // Remove o prefixo "Bearer" e espaços extras
            string token = authHeader.Replace("Bearer", "").Trim();

            if (string.IsNullOrWhiteSpace(token))
            {
                context.Result = new JsonResult(new
                {
                    erro = "Token JWT vazio ou inválido."
                })
                { StatusCode = 401 };
                return;
            }

            // Usa sua classe personalizada para validar o token
            MeuTokenJWT jwt = new MeuTokenJWT();
            bool valido = jwt.ValidarToken(token);

            if (!valido)
            {
                context.Result = new JsonResult(new
                {
                    erro = "Token inválido ou expirado."
                })
                { StatusCode = 401 };
                return;
            }

            // Exemplo: adiciona informações do usuário no contexto HTTP
            context.HttpContext.Items["Email"] = jwt.Email;
            context.HttpContext.Items["IdFuncionario"] = jwt.IdFuncionario;
            context.HttpContext.Items["Role"] = jwt.Role;

            Console.WriteLine($"✅ Token válido para: {jwt.Email}");
        }
    }
}
