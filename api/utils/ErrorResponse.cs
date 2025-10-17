using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Utils
{
    /// <summary>
    /// Exceção personalizada para padronizar erros na aplicação.
    /// 🔹 Ideal para APIs REST, permitindo retornar código HTTP e detalhes do erro.
    /// 🔹 Pode ser usada em serviços, controllers, filtros ou middleware.
    /// </summary>
    public class ErrorResponse : Exception
    {
        /// <summary>
        /// Código HTTP associado ao erro (ex: 400 Bad Request, 401 Unauthorized, 404 Not Found, 500 Internal Server Error)
        /// 🔹 Permite ao middleware ou controller retornar a resposta adequada.
        /// </summary>
        public int HttpCode { get; }

        /// <summary>
        /// Informações adicionais sobre o erro
        /// 🔹 Pode conter objeto com detalhes, mensagens internas ou campos específicos.
        /// </summary>
        public object Error { get; }

        /// <summary>
        /// Nome da exceção, útil para logs ou retorno padronizado
        /// 🔹 Aqui sempre retorna "ErrorResponse"
        /// </summary>
        public string Name => "ErrorResponse";

        /// <summary>
        /// Construtor da classe ErrorResponse
        /// 🔹 Recebe código HTTP, mensagem de erro e detalhes adicionais
        /// 🔹 Herdando de Exception, permite usar try/catch normalmente
        /// </summary>
        /// <param name="httpCode">Código HTTP que será retornado ao cliente</param>
        /// <param name="message">Mensagem principal do erro (visível ao cliente ou log)</param>
        /// <param name="error">Informações adicionais sobre o erro (opcional, pode ser objeto com detalhes)</param>
        public ErrorResponse(int httpCode, string message, object error)
            : base(message) // chama o construtor da classe base Exception para manter a mensagem
        {
            // 1️⃣ Armazena o código HTTP
            this.HttpCode = httpCode;

            // 2️⃣ Armazena informações adicionais do erro
            this.Error = error;

            // 3️⃣ A classe agora encapsula tanto a mensagem quanto detalhes e status HTTP
            //    Podendo ser utilizada no middleware para padronizar a resposta JSON
        }
    }
}
