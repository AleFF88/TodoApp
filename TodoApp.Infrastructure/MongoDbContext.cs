using MongoDB.Driver;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure {
    public class MongoDbContext {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName) {
            // Инициализация клиента, стока подключения указана в конфигурации 
            var client = new MongoClient(connectionString);
            // Подключение к базе данных, имя базы данных указано в конфигурации
            _database = client.GetDatabase(databaseName);
        }

        // Доступ к коллекции документов TodoList
        public IMongoCollection<TodoList> TodoLists =>
            _database.GetCollection<TodoList>("TodoLists");
    }
}
