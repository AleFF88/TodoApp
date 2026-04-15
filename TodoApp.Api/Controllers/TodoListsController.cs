using Microsoft.AspNetCore.Mvc;
using TodoApp.Api.DTOs;
using TodoApp.Application.DTOs;
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

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoListBriefDto>> GetById(
            Guid id,
            [FromHeader(Name = "X-User-Id")] Guid userId) {

            // Вызываем сервис бизнес-логики. 
            // Если доступа нет или списка нет — он выбросит исключение.
            var todoList = await _todoListService.GetListByIdAsync(id, userId);

            // Маппим доменную сущность в ваш существующий TodoListBriefDto
            return Ok(new TodoListBriefDto(todoList.Id, todoList.Title));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateTodoListRequest request,
            [FromHeader(Name = "X-User-Id")] Guid userId) {

            await _todoListService.UpdateListTitleAsync(id, request.Title, userId);

            // Возвращаем 204 No Content, так как при успешном обновлении 
            // передавать дополнительные данные клиенту не требуется.
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            Guid id,
             [FromHeader(Name = "X-User-Id")] Guid userId) {

            await _todoListService.DeleteListAsync(id, userId);

            // Возвращаем 204 No Content. Ресурс удален, передавать в теле ответа больше нечего.
            return NoContent();
        }
    }
}