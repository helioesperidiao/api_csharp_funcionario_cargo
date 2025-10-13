using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using Api.Utils;

namespace Api.Filters
{
    public class ValidateCargoBody : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("🔶 ValidateCargoBody.OnActionExecuting()");

            if (!context.ActionArguments.TryGetValue("requestBody", out var bodyObj) || bodyObj == null)
            {
                throw new ErrorResponse(
                    400,
                    "O objeto 'cargo' é obrigatório!",
                    new { detalhe = "Corpo da requisição ausente ou incorreto" }
                );
            }

            // Converte para JsonElement
            if (bodyObj is not JsonElement json)
            {
                throw new ErrorResponse(
                    400,
                    "Formato inválido",
                    new { detalhe = "O corpo da requisição não é um JSON válido" }
                );
            }

            // Verifica propriedade "cargo"
            if (!json.TryGetProperty("cargo", out JsonElement cargoElem))
            {
                throw new ErrorResponse(
                    400,
                    "O objeto 'cargo' é obrigatório!",
                    new { detalhe = "Propriedade 'cargo' ausente" }
                );
            }

            // Verifica "nomeCargo"
            if (!cargoElem.TryGetProperty("nomeCargo", out JsonElement nomeElem) ||
                string.IsNullOrWhiteSpace(nomeElem.GetString()))
            {
                throw new ErrorResponse(
                    400,
                    "O campo 'nomeCargo' é obrigatório!",
                    new { detalhe = "Campo vazio ou não informado" }
                );
            }
        }
    }
}
