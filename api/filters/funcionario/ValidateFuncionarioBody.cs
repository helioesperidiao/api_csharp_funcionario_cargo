using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
using Api.Utils;
using System;

namespace Api.Filters
{
    /// <summary>
    /// Filtro de ação responsável por validar o corpo da requisição
    /// para operações relacionadas à entidade Funcionario.
    /// 
    /// 🔹 ActionFilterAttribute permite executar lógica antes ou depois
    /// de um endpoint do controller ser chamado.
    /// </summary>
    public class ValidateFuncionarioBody : ActionFilterAttribute
    {
        /// <summary>
        /// Executado antes do método do controller.
        /// Valida se o corpo da requisição contém o objeto funcionario
        /// e se os campos obrigatórios estão presentes e corretos.
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("🔶 ValidateFuncionarioBody.OnActionExecuting()");

            // 1️⃣ Verifica se o ActionArguments contém o corpo da requisição
            // "requestBody" é o nome do parâmetro esperado no controller
            if (!context.ActionArguments.TryGetValue("requestBody", out var jsonBody) || jsonBody == null)
            {
                throw new ErrorResponse(
                    400,
                    "O objeto 'funcionario' é obrigatório!",
                    new { detalhe = "Corpo da requisição ausente ou incorreto" }
                );
            }

            // 2️⃣ Converte o objeto recebido para JsonElement para manipulação
            if (jsonBody is not JsonElement json)
            {
                throw new ErrorResponse(
                    400,
                    "Formato inválido",
                    new { detalhe = "O corpo da requisição não é um JSON válido" }
                );
            }

            // 3️⃣ Verifica se existe a propriedade "funcionario" no JSON
            if (!json.TryGetProperty("funcionario", out JsonElement funcElem))
            {
                throw new ErrorResponse(
                    400,
                    "O objeto 'funcionario' é obrigatório!",
                    new { detalhe = "Propriedade 'funcionario' ausente" }
                );
            }

            // 4️⃣ Valida campo obrigatório "nomeFuncionario"
            if (!funcElem.TryGetProperty("nomeFuncionario", out JsonElement nomeElem) ||
                string.IsNullOrWhiteSpace(nomeElem.GetString()))
            {
                throw new ErrorResponse(
                    400,
                    "O campo 'nomeFuncionario' é obrigatório!",
                    new { detalhe = "Campo vazio ou não informado" }
                );
            }

            // 5️⃣ Valida campo obrigatório "email"
            if (!funcElem.TryGetProperty("email", out JsonElement emailElem) ||
                string.IsNullOrWhiteSpace(emailElem.GetString()))
            {
                throw new ErrorResponse(
                    400,
                    "O campo 'email' é obrigatório!",
                    new { detalhe = "Campo vazio ou não informado" }
                );
            }

            // 6️⃣ Valida campo obrigatório "senha"
            if (!funcElem.TryGetProperty("senha", out JsonElement senhaElem) ||
                string.IsNullOrWhiteSpace(senhaElem.GetString()))
            {
                throw new ErrorResponse(
                    400,
                    "O campo 'senha' é obrigatório!",
                    new { detalhe = "Campo vazio ou não informado" }
                );
            }

            // 7️⃣ Valida campo "recebeValeTransporte" como número 0 ou 1
            if (!funcElem.TryGetProperty("recebeValeTransporte", out JsonElement vtElem) ||
                vtElem.ValueKind != JsonValueKind.Number)
            {
                throw new ErrorResponse(
                    400,
                    "O campo 'recebeValeTransporte' é obrigatório e deve ser 0 ou 1!",
                    new { detalhe = "Apenas valores inteiros 0 ou 1 são permitidos" }
                );
            }

            // 8️⃣ Valida campo "cargo.idCargo" como inteiro positivo
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

            // 9️⃣ Todos os campos obrigatórios presentes e válidos
            // 🔹 Fluxo segue para o controller se nenhuma exceção for lançada
        }
    }
}
