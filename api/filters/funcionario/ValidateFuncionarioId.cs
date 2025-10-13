using Microsoft.AspNetCore.Mvc.Filters;
using Api.Utils;
using System;

namespace Api.Filters
{
    public class ValidateFuncionarioId : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("🔶 ValidateFuncionarioId.OnActionExecuting()");

            // Tenta obter o argumento "idFuncionario" da ação
            if (!context.ActionArguments.TryGetValue("idFuncionario", out var idObj) || idObj == null)
            {
                throw new ErrorResponse(
                    400,
                    "O parâmetro 'idFuncionario' é obrigatório!",
                    new { detalhe = "Parâmetro ausente na requisição" }
                );
            }

            // Converte para inteiro
            int idFuncionario;
            try
            {
                idFuncionario = Convert.ToInt32(idObj);
            }
            catch
            {
                throw new ErrorResponse(
                    400,
                    "ID inválido",
                    new { detalhe = "O parâmetro 'idFuncionario' deve ser um número inteiro" }
                );
            }

            // Valida se o ID é positivo
            if (idFuncionario <= 0)
            {
                throw new ErrorResponse(
                    400,
                    "ID inválido",
                    new { detalhe = "O parâmetro 'idFuncionario' deve ser maior que zero" }
                );
            }

            // Se chegar aqui, o ID é válido
        }
    }
}
