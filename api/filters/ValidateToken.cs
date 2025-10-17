using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Api.Utils;

namespace Api.Filters
{
    /// <summary>
    /// Filtro de autorização que valida tokens JWT usando a classe MeuTokenJWT.
    /// Pode ser aplicado em controladores ou métodos específicos.
    /// 
    /// 🔹 IAuthorizationFilter permite executar lógica antes do controller ser chamado,
    /// garantindo que apenas requisições autorizadas continuem.
    /// </summary>
    public class ValidateToken : Attribute, IAuthorizationFilter
    {
        /// <summary>
        /// Executado antes do controller, valida o token JWT presente no cabeçalho Authorization.
        /// </summary>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            Console.WriteLine("🔐 ValidateTokenAttribute executando...");

            // 1️⃣ Obtém o cabeçalho Authorization da requisição
            string? authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            // 2️⃣ Se o cabeçalho estiver ausente ou vazio, retorna 401 Unauthorized
            if (string.IsNullOrWhiteSpace(authHeader))
            {
                context.Result = new JsonResult(new
                {
                    erro = "Cabeçalho 'Authorization' ausente."
                })
                { StatusCode = 401 };
                return;
            }

            // 3️⃣ Remove o prefixo "Bearer" e espaços extras para extrair o token puro
            string token = authHeader.Replace("Bearer", "").Trim();

            // 4️⃣ Se o token estiver vazio após a limpeza, retorna 401 Unauthorized
            if (string.IsNullOrWhiteSpace(token))
            {
                context.Result = new JsonResult(new
                {
                    erro = "Token JWT vazio ou inválido."
                })
                { StatusCode = 401 };
                return;
            }

            // 5️⃣ Valida o token usando a classe personalizada MeuTokenJWT
            MeuTokenJWT jwt = new MeuTokenJWT();
            bool valido = jwt.ValidarToken(token);

            // 6️⃣ Se o token for inválido ou expirado, retorna 401 Unauthorized
            if (!valido)
            {
                context.Result = new JsonResult(new
                {
                    erro = "Token inválido ou expirado."
                })
                { StatusCode = 401 };
                return;
            }

            // 7️⃣ Se o token for válido, adiciona informações do usuário no contexto HTTP
            // Isso permite que o controller acesse Email, IdFuncionario e Role do usuário
            context.HttpContext.Items["Email"] = jwt.Email;
            context.HttpContext.Items["IdFuncionario"] = jwt.IdFuncionario;
            context.HttpContext.Items["Role"] = jwt.Role;

            Console.WriteLine($"✅ Token válido para: {jwt.Email}");
        }
    }
}
