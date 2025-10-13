using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using Api.Utils;
using System;

namespace Api.Filters
{
    public class ValidateFuncionarioBody : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("🔶 ValidateFuncionarioBody.OnActionExecuting()");

            // Verifica se o corpo da requisição existe
            if (!context.ActionArguments.TryGetValue("requestBody", out var bodyObj) || bodyObj == null)
            {
                throw new ErrorResponse(
                    400,
                    "O objeto 'funcionario' é obrigatório!",
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

            // Verifica propriedade "funcionario"
            if (!json.TryGetProperty("funcionario", out JsonElement funcElem))
            {
                throw new ErrorResponse(
                    400,
                    "O objeto 'funcionario' é obrigatório!",
                    new { detalhe = "Propriedade 'funcionario' ausente" }
                );
            }

            // Valida "nomeFuncionario"
            if (!funcElem.TryGetProperty("nomeFuncionario", out JsonElement nomeElem) ||
                string.IsNullOrWhiteSpace(nomeElem.GetString()))
            {
                throw new ErrorResponse(
                    400,
                    "O campo 'nomeFuncionario' é obrigatório!",
                    new { detalhe = "Campo vazio ou não informado" }
                );
            }

            // Valida "email"
            if (!funcElem.TryGetProperty("email", out JsonElement emailElem) ||
                string.IsNullOrWhiteSpace(emailElem.GetString()))
            {
                throw new ErrorResponse(
                    400,
                    "O campo 'email' é obrigatório!",
                    new { detalhe = "Campo vazio ou não informado" }
                );
            }

            // Valida "senha"
            if (!funcElem.TryGetProperty("senha", out JsonElement senhaElem) ||
                string.IsNullOrWhiteSpace(senhaElem.GetString()))
            {
                throw new ErrorResponse(
                    400,
                    "O campo 'senha' é obrigatório!",
                    new { detalhe = "Campo vazio ou não informado" }
                );
            }

            // Valida "recebeValeTransporte" como 0 ou 1
            if (!funcElem.TryGetProperty("recebeValeTransporte", out JsonElement vtElem) ||
                vtElem.ValueKind != JsonValueKind.Number)
            {
                throw new ErrorResponse(
                    400,
                    "O campo 'recebeValeTransporte' é obrigatório e deve ser 0 ou 1!",
                    new { detalhe = "Apenas valores inteiros 0 ou 1 são permitidos" }
                );
            }

            // Valida "cargo.idCargo"
            if (!funcElem.TryGetProperty("cargo", out JsonElement cargoElem) ||
                !cargoElem.TryGetProperty("idCargo", out JsonElement idCargoElem) ||
                idCargoElem.ValueKind != JsonValueKind.Number ||
                idCargoElem.GetInt32() <= 0)
            {
                throw new ErrorResponse(
                    400,
                    "O campo 'cargo.idCargo' é obrigatório e deve ser um inteiro positivo",
                    new { detalhe = "Ex.: { \"cargo\": { \"idCargo\": 1 } }" }
                );
            }

            // Todos os campos obrigatórios presentes e válidos
        }
    }
}
