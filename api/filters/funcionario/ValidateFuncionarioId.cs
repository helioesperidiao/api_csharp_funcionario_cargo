using Microsoft.AspNetCore.Mvc.Filters;
using Api.Utils;
using System;

namespace Api.Filters
{
    /// <summary>
    /// Filtro de ação responsável por validar o parâmetro "idFuncionario"
    /// passado na URL ou na rota de um endpoint.
    /// 
    /// 🔹 ActionFilterAttribute permite executar lógica antes ou depois
    /// de um método do controller ser chamado.
    /// </summary>
    public class ValidateFuncionarioId : ActionFilterAttribute
    {
        /// <summary>
        /// Executado antes do método do controller.
        /// Valida se o ID do funcionário foi passado, se é um número inteiro
        /// e se é maior que zero.
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("🔶 ValidateFuncionarioId.OnActionExecuting()");

            // 1️⃣ Tenta obter o argumento "idFuncionario" passado na requisição
            // Pode vir da rota, query string ou body dependendo do endpoint
            if (!context.ActionArguments.TryGetValue("idFuncionario", out var idObj) || idObj == null)
            {
                // 2️⃣ Se não existir, lança erro com status 400
                throw new ErrorResponse(
                    400,
                    "O parâmetro 'idFuncionario' é obrigatório!",
                    new { detalhe = "Parâmetro ausente na requisição" }
                );
            }

            // 3️⃣ Converte o valor para inteiro de forma segura
            int idFuncionario;
            try
            {
                idFuncionario = Convert.ToInt32(idObj);
            }
            catch
            {
                // 4️⃣ Se a conversão falhar, significa que não é um número válido
                throw new ErrorResponse(
                    400,
                    "ID inválido",
                    new { detalhe = "O parâmetro 'idFuncionario' deve ser um número inteiro" }
                );
            }

            // 5️⃣ Valida se o ID é positivo
            if (idFuncionario <= 0)
            {
                // 6️⃣ Se não for positivo, retorna erro
                throw new ErrorResponse(
                    400,
                    "ID inválido",
                    new { detalhe = "O parâmetro 'idFuncionario' deve ser maior que zero" }
                );
            }

            // 7️⃣ Se chegar aqui, o ID é considerado válido e o fluxo segue para o controller
        }
    }
}
