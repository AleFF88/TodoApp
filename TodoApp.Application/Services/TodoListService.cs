using System.Collections.Generic;
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

        private async Task<TodoList> GetListOrThrowAsync(Guid listId) {
            var todoList = await _repository.GetByIdAsync(listId);
            if (todoList == null) {
                throw new KeyNotFoundException($"TodoList {listId} not found.");
            }
            return todoList;
        }
    }
}