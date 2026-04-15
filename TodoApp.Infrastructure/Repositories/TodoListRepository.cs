using MongoDB.Driver;
using TodoApp.Domain.Entities;
using TodoApp.Domain.Repositories;

namespace TodoApp.Infrastructure.Repositories {

    public class TodoListRepository : ITodoListRepository {
        private readonly IMongoCollection<TodoList> _collection;

        public TodoListRepository(MongoDbContext context) {
            _collection = context.TodoLists;
        }

        public async Task AddAsync(TodoList todoList) {
            await _collection.InsertOneAsync(todoList);
        }

        public async Task UpdateAsync(TodoList todoList) {
            await _collection.ReplaceOneAsync(l => l.Id == todoList.Id, todoList);
        }

        public async Task DeleteAsync(Guid id) {
            await _collection.DeleteOneAsync(l => l.Id == id);
        }

        public async Task<TodoList?> GetByIdAsync(Guid id) {
            return await _collection.Find(l => l.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TodoList>> GetPagedAsync(Guid userId, int pageNumber, int pageSize) {
            var filter = Builders<TodoList>.Filter.Or(
                Builders<TodoList>.Filter.Eq(l => l.OwnerId, userId),
                Builders<TodoList>.Filter.AnyEq("SharedUserIds", userId)
            );

            return await _collection.Find(filter)
                .SortByDescending(l => l.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Limit(pageSize)
                .ToListAsync();
        }
    }
}