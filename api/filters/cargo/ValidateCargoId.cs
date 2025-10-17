using Microsoft.AspNetCore.Mvc.Filters;
using Api.Utils;
using System;

namespace Api.Filters
{
    /// <summary>
    /// Filtro de ação responsável por validar o parâmetro "idCargo"
    /// passado na URL ou na rota de um endpoint.
    /// 
    /// 🔹 ActionFilterAttribute permite executar lógica antes ou depois
    /// de um método do controller ser chamado.
    /// </summary>
    public class ValidateCargoId : ActionFilterAttribute
    {
        /// <summary>
        /// Executado antes do método do controller.
        /// Valida se o ID do cargo foi passado, se é um número inteiro
        /// e se é maior que zero.
        /// </summary>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("🔶 ValidateCargoId.OnActionExecuting()");

            // 1️⃣ Tenta obter o argumento "idCargo" passado na requisição
            // Pode vir da rota, query string ou body dependendo do endpoint
            if (!context.ActionArguments.TryGetValue("idCargo", out var idObj) || idObj == null)
            {
                // 2️⃣ Se não existir, lança erro com status 400
                throw new ErrorResponse(
                    400,
                    "O parâmetro 'idCargo' é obrigatório!",
                    new { detalhe = "Parâmetro ausente na requisição" }
                );
            }

            // 3️⃣ Converte o valor para inteiro de forma segura
            int idCargo;
            try
            {
                idCargo = Convert.ToInt32(idObj);
            }
            catch
            {
                // 4️⃣ Se a conversão falhar, significa que não é um número válido
                throw new ErrorResponse(
                    400,
                    "ID inválido",
                    new { detalhe = "O parâmetro 'idCargo' deve ser um número inteiro" }
                );
            }

            // 5️⃣ Valida se o ID é positivo
            if (idCargo <= 0)
            {
                // 6️⃣ Se não for positivo, retorna erro
                throw new ErrorResponse(
                    400,
                    "ID inválido",
                    new { detalhe = "O parâmetro 'idCargo' deve ser maior que zero" }
                );
            }

            // 7️⃣ Se chegar aqui, o ID é considerado válido e o fluxo segue para o controller
        }
    }
}
