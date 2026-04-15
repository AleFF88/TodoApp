using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.DTOs;
using TodoApp.Application.Services;

namespace TodoApp.Api.Controllers {
    [ApiController]
    [Route("api/[controller]")] // Путь будет api/todolists
    public class TodoListsController : ControllerBase {
        private readonly TodoListService _todoListService;

        public TodoListsController(TodoListService todoListService) {
            _todoListService = todoListService;
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(
            [FromBody] CreateTodoListRequest request,
            [FromHeader(Name = "X-User-Id")] Guid userId) {

            // Вызываем сервис бизнес-логики
            var listId = await _todoListService.CreateListAsync(request.Title, userId);

            return Ok(listId);
        }

    }
}