using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Repositories;

namespace TodoApp.Application.Services {
    public class TodoListService {
        private readonly ITodoListRepository _repository;

        public TodoListService(ITodoListRepository repository) {
            _repository = repository;
        }

        // Создание нового списка
        public async Task<Guid> CreateListAsync(string title, Guid ownerId) {
            var id = Guid.NewGuid();
            var todoList = new TodoList(id, title, ownerId);
            await _repository.AddAsync(todoList);
            return id;
        }


        // Удаление списка
        public async Task DeleteListAsync(Guid listId, Guid currentUserId) {
            var todoList = await GetListOrThrowAsync(listId);
            
            if (todoList.OwnerId != currentUserId) {
                throw new UnauthorizedAccessException("Only the owner can delete the list.");
            }

            await _repository.DeleteAsync(todoList.Id);
        }

        // Редактирование название списка
        public async Task UpdateListTitleAsync(Guid listId, string newTitle, Guid currentUserId) {
            var todoList = await GetListOrThrowAsync(listId);

            if (todoList.OwnerId != currentUserId && !todoList.SharedUserIds.Contains(currentUserId)) {
                throw new UnauthorizedAccessException("You don't have permission to edit this list.");
            }

            todoList.UpdateTitle(newTitle);
            await _repository.UpdateAsync(todoList);
        }

        // Получение перечня всех списков пользователя
        public async Task<IEnumerable<TodoListBriefDto>> GetBriefListsAsync(Guid currentUserId, int page, int pageSize) { 
            var todoLists = await _repository.GetPagedAsync(currentUserId, page, pageSize);

            // Делаем маппинг из доменной сущности в DTO для передачи клиенту.
            // Как я писал в описании, тут я получил полную сущность, но беру только необходимое.
            //   Если потом мне понадобятся другие поля, я просто добавлю их в DTO и допишу маппинг здесь.
            return todoLists.Select(l => new TodoListBriefDto(l.Id, l.Title));
        }

        // Добавление связи пользователя с списком (для совместного доступа)
        public async Task AddUserLinkAsync(Guid listId, Guid currentUserId, Guid targetUserId) {
            var todoList = await GetListOrThrowAsync(listId);

            if (todoList.OwnerId != currentUserId && !todoList.SharedUserIds.Contains(currentUserId)) {
                throw new UnauthorizedAccessException("Access denied.");
            }

            todoList.AddSharedUser(targetUserId);
            await _repository.UpdateAsync(todoList);
        }

        // Удаление связи пользователя с списком (для совместного доступа)
        public async Task RemoveUserLinkAsync(Guid listId, Guid currentUserId, Guid targetUserId) {
            var todoList = await GetListOrThrowAsync(listId);

            if (todoList.OwnerId != currentUserId && !todoList.SharedUserIds.Contains(currentUserId)) {
                throw new UnauthorizedAccessException("Access denied.");
            }

            todoList.RemoveSharedUser(targetUserId);
            await _repository.UpdateAsync(todoList);
        }

        private async Task<TodoList> GetListOrThrowAsync(Guid listId) {
            var todoList = await _repository.GetByIdAsync(listId);
            if (todoList == null) {
                throw new KeyNotFoundException($"TodoList {listId} not found.");
            }
            return todoList;
        }
    }
}