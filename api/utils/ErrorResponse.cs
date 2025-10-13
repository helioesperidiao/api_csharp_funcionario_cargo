using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
namespace Api.Utils
{
    /// <summary>
    /// Exceção personalizada para padronizar erros na aplicação.
    /// Contém código HTTP e informações adicionais sobre o erro.
    /// </summary>
    public class ErrorResponse : Exception
    {
        /// <summary>
        /// Código HTTP associado ao erro (ex: 400, 404, 500)
        /// </summary>
        public int HttpCode { get; }

        /// <summary>
        /// Informações adicionais sobre o erro
        /// </summary>
        public object Error { get; }

        /// <summary>
        /// Nome da exceção
        /// </summary>
        public  string Name => "ErrorResponse";

        /// <summary>
        /// Construtor da classe ErrorResponse
        /// </summary>
        /// <param name="httpCode">Código HTTP</param>
        /// <param name="message">Mensagem de erro</param>
        /// <param name="error">Informações adicionais (opcional)</param>
        public ErrorResponse(int httpCode, string message, object error)
            : base(message)
        {
            this.HttpCode = httpCode;
            this.Error = error;
        }
    }
}
