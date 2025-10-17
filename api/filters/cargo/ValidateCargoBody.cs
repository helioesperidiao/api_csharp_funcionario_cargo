using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using Api.Utils;

namespace Api.Filters
{
    /// <summary>
    /// Filtro de ação responsável por validar o corpo da requisição
    /// para operações relacionadas à entidade Cargo.
    /// 
    /// 🔹 ActionFilterAttribute permite executar lógica antes ou depois
    /// de um endpoint do controller ser chamado.
    /// </summary>
    public class ValidateCargoBody : ActionFilterAttribute
    {
        /// <summary>
        /// Executado antes do método do controller.
        /// Aqui validamos se o corpo da requisição contém o objeto cargo
        /// e se os campos obrigatórios estão presentes.
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("🔶 ValidateCargoBody.OnActionExecuting()");

            // 1️⃣ Verifica se o ActionArguments contém o corpo da requisição
            // "requestBody" é o nome do parâmetro esperado no controller
            if (!context.ActionArguments.TryGetValue("requestBody", out var jsonBody) || jsonBody == null)
            {
                // 2️⃣ Lança exceção personalizada se o corpo não estiver presente
                throw new ErrorResponse(
                    400,
                    "O objeto 'cargo' é obrigatório!",
                    new { detalhe = "Corpo da requisição ausente ou incorreto" }
                );
            }

            // 3️⃣ Converte o objeto recebido para JsonElement para manipulação
            if (jsonBody is not JsonElement json)
            {
                // 4️⃣ Se não for JSON válido, retorna erro
                throw new ErrorResponse(
                    400,
                    "Formato inválido",
                    new { detalhe = "O corpo da requisição não é um JSON válido" }
                );
            }

            // 5️⃣ Verifica se a propriedade "cargo" existe dentro do JSON
            if (!json.TryGetProperty("cargo", out JsonElement cargoElem))
            {
                // 6️⃣ Se não existir, retorna erro
                throw new ErrorResponse(
                    400,
                    "O objeto 'cargo' é obrigatório!",
                    new { detalhe = "Propriedade 'cargo' ausente" }
                );
            }

            // 7️⃣ Verifica se "cargo" possui a propriedade obrigatória "nomeCargo"
            if (!cargoElem.TryGetProperty("nomeCargo", out JsonElement nomeElem) ||
                string.IsNullOrWhiteSpace(nomeElem.GetString()))
            {
                // 8️⃣ Se estiver ausente ou vazio, retorna erro
                throw new ErrorResponse(
                    400,
                    "O campo 'nomeCargo' é obrigatório!",
                    new { detalhe = "Campo vazio ou não informado" }
                );
            }

            // 9️⃣ Se todas validações passarem, o fluxo segue para o controller
            // Não é necessário chamar base.OnActionExecuting, pois não há lógica adicional
        }
    }
}
