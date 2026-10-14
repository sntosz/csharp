using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceApi.Api.Middlewares;

/// <summary>
/// Middleware global para tratamento de exceções não capturadas e erros de validação.
/// Retorna padronizado no formato ProblemDetails (RFC 7807) utilizado no mercado.
/// </summary>
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

    public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Erro de validação capturado no middleware.");

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/problem+json";

            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            var validationProblemDetails = new HttpValidationProblemDetails(errors)
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Erro de validação de dados.",
                Detail = "Um ou mais campos contêm valores inválidos."
            };

            await context.Response.WriteAsJsonAsync(validationProblemDetails);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu um erro não tratado no servidor.");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/problem+json";

            var problemDetails = new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Erro interno no servidor",
                Detail = "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."
            };

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
