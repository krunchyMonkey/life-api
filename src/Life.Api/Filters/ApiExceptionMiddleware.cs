using FluentValidation;
using System.Net;

namespace Life.Api.Filters
{
    public sealed class ApiExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ApiExceptionMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext ctx)
        {
            try { await _next(ctx); }
            catch (ValidationException ex)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await ctx.Response.WriteAsJsonAsync(new { title = "Validation failed", errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }) });
            }
            catch (KeyNotFoundException)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await ctx.Response.WriteAsJsonAsync(new { title = "Not Found" });
            }
        }
    }
}
