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
    }
}