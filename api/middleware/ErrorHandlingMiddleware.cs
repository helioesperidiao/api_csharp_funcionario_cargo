using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Api.Utils; // Para ErrorResponse

namespace Api.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ErrorResponse ex) // Exceção customizada
            {
                Console.WriteLine("🟡 ErrorResponse capturada no middleware");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex.HttpCode;

                var resposta = new
                {
                    success = false,
                    message = ex.Message,
                    error = new { message = ex.Error }
                };

                await context.Response.WriteAsJsonAsync(resposta);
            }
            catch (Exception ex) // Exceções gerais
            {
                Console.WriteLine("🟡 ErrorResponse capturada no middleware");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = 400;

                var resposta = new
                {
                    success = false,
                    message = "Ocorreu um erro interno no servidor",
                    //data = new { stack = ex.StackTrace },
                    error = new { message = ex.Message }
                };

                Console.Error.WriteLine(resposta);
                await context.Response.WriteAsJsonAsync(resposta);
            }
        }
    }
}
