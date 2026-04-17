using System.Net;
using System.Text.Json;

namespace TodoApp.Api.Middleware {
    public class ExceptionHandlingMiddleware {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger) {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context) {
            try {
                // Передаем запрос дальше по конвейеру (в контроллеры)
                await _next(context);
            }
            catch (Exception ex) {
                // Если где-то в Service или Repository будет exception, управление вернется сюда.
                // Логируем.
                _logger.LogError(ex, "Exception occurred: {Message}", ex.Message);
                // Формируем ответ для клиента.
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception) {
            // Устанавливаем значения по умолчанию (для непредвиденных ошибок)
            var code = HttpStatusCode.InternalServerError;
            var message = "An internal server error occurred.";

            // Маппинг типов исключений на HTTP статусы
            switch (exception) {
                case UnauthorizedAccessException:
                    code = HttpStatusCode.Forbidden; // 403
                    message = exception.Message;
                    break;
                case KeyNotFoundException:
                    code = HttpStatusCode.NotFound; // 404
                    message = exception.Message;
                    break;
                case ArgumentException:
                    code = HttpStatusCode.BadRequest; // 400
                    message = exception.Message;
                    break;
            }

            var result = JsonSerializer.Serialize(new { errorMsg = message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }
}