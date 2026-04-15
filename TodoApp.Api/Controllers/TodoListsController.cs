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

        [HttpPost("{id}/shares")]
        public async Task<IActionResult> AddShare(
            Guid id,
            [FromBody] ShareListRequest request,
            [FromHeader(Name = "X-User-Id")] Guid userId) {
            await _todoListService.AddUserLinkAsync(id, userId, request.TargetUserId);
            return NoContent();
        }

        [HttpGet("{id}/shares")]
        public async Task<ActionResult<IEnumerable<Guid>>> GetShares(
            Guid id,
            [FromHeader(Name = "X-User-Id")] Guid userId) {
            var sharedUsers = await _todoListService.GetSharedUsersAsync(id, userId);
            return Ok(sharedUsers);
        }

        [HttpDelete("{id}/shares/{targetUserId}")]
        public async Task<IActionResult> RemoveShare(
            Guid id,
            Guid targetUserId,
            [FromHeader(Name = "X-User-Id")] Guid userId) {
            await _todoListService.RemoveUserLinkAsync(id, userId, targetUserId);
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoListBriefDto>>> GetPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromHeader(Name = "X-User-Id")] Guid userId = default) {

            // Проверка корректности параметров пагинации
            if (page < 1)
                page = 1;
            if (pageSize < 1 || pageSize > 100)
                pageSize = 10;

            var lists = await _todoListService.GetBriefListsAsync(userId, page, pageSize);
            return Ok(lists);
        }
    }
}