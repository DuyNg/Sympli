namespace Sympli.Searching.API.Middleware
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
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                // Serialize the response object to JSON to ensure a non-null string is passed to WriteAsync  
                var errorResponse = new
                {
                    context.Response.StatusCode,
                    Message = "An unexpected error occurred.",
                    Details = ex.Message
                };

                var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponse);
                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
