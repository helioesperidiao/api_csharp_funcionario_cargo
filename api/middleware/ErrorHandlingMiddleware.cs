using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Api.Utils; // Para ErrorResponse

namespace Api.Middleware
{
    /// <summary>
    /// Middleware responsável por capturar e tratar exceções lançadas
    /// durante o processamento das requisições HTTP.
    /// 
    /// 🔹 Permite centralizar o tratamento de erros, retornando respostas
    /// padronizadas para o cliente e evitando exposição de detalhes sensíveis.
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        // 1️⃣ RequestDelegate representa o próximo middleware na pipeline
        private readonly RequestDelegate _next;

        /// <summary>
        /// Construtor do middleware recebe o próximo delegate da pipeline.
        /// </summary>
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// Método chamado automaticamente pelo ASP.NET Core
        /// para processar cada requisição HTTP.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // 2️⃣ Tenta executar o próximo middleware ou endpoint
                await _next(context);
            }
            catch (ErrorResponse ex) // 3️⃣ Captura exceções customizadas
            {
                Console.WriteLine("🟡 ErrorResponse capturada no middleware");

                // 4️⃣ Define o tipo da resposta HTTP como JSON
                context.Response.ContentType = "application/json";

                // 5️⃣ Usa o código HTTP da exceção personalizada
                context.Response.StatusCode = ex.HttpCode;

                // 6️⃣ Monta objeto de resposta padronizado
                Object resposta = new
                {
                    success = false,
                    message = ex.Message, // Mensagem amigável
                    error = new { message = ex.Error } // Detalhe do erro
                };

                // 7️⃣ Envia resposta JSON para o cliente
                await context.Response.WriteAsJsonAsync(resposta);
            }
            catch (Exception ex) // 8️⃣ Captura exceções gerais não previstas
            {
                Console.WriteLine("🟡 Exceção geral capturada no middleware");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400; // Bad Request

                // 9️⃣ Monta objeto de resposta para erros gerais
                Object resposta = new
                {
                    success = false,
                    message = "Ocorreu um erro interno no servidor",
                    //data = new { stack = ex.StackTrace }, // opcional para depuração
                    error = new { message = ex.Message } // Mensagem do erro real
                };

                // 10️⃣ Loga a resposta no console para análise
                Console.Error.WriteLine(resposta);

                // 11️⃣ Envia resposta JSON para o cliente
                await context.Response.WriteAsJsonAsync(resposta);
            }
        }
    }
}
