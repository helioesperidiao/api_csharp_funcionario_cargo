using Microsoft.AspNetCore.Mvc.Filters;
using Api.Utils;
using System;

namespace Api.Filters
{
    public class ValidateCargoId : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("🔶 ValidateCargoId.OnActionExecuting()");

            // Tenta obter o argumento "idCargo" da ação
            if (!context.ActionArguments.TryGetValue("idCargo", out var idObj) || idObj == null)
            {
                throw new ErrorResponse(
                    400,
                    "O parâmetro 'idCargo' é obrigatório!",
                    new { detalhe = "Parâmetro ausente na requisição" }
                );
            }

            // Converte para inteiro
            int idCargo;
            try
            {
                idCargo = Convert.ToInt32(idObj);
            }
            catch
            {
                throw new ErrorResponse(
                    400,
                    "ID inválido",
                    new { detalhe = "O parâmetro 'idCargo' deve ser um número inteiro" }
                );
            }

            // Valida se o ID é positivo
            if (idCargo <= 0)
            {
                throw new ErrorResponse(
                    400,
                    "ID inválido",
                    new { detalhe = "O parâmetro 'idCargo' deve ser maior que zero" }
                );
            }

            // Se chegar aqui, o ID é válido
        }
    }
}
