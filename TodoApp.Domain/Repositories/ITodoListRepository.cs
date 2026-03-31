using TodoApp.Domain.Entities;

namespace TodoApp.Domain.Repositories {
    public interface ITodoListRepository {
        Task AddAsync(TodoList todoList);
        Task UpdateAsync(TodoList todoList);
        Task DeleteAsync(Guid id);
        Task<TodoList?> GetByIdAsync(Guid id);
        Task<IEnumerable<TodoList>> GetPagedAsync(Guid userId, int pageNumber, int pageSize);
    }
}
