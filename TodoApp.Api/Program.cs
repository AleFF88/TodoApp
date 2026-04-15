using TodoApp.Application.Services;
using TodoApp.Domain.Repositories;
using TodoApp.Infrastructure;
using TodoApp.Infrastructure.Configuration;
using TodoApp.Infrastructure.Repositories;

namespace TodoApp.Api {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            // Глобальная настройка обработки Guid для всей системы
            MongoDB.Bson.Serialization.BsonSerializer.RegisterSerializer(
                new MongoDB.Bson.Serialization.Serializers.GuidSerializer(MongoDB.Bson.GuidRepresentation.Standard));

            MongoDbMapping.Register();

            // Получение настроек из секции MongoDbSettings в appsettings.json
            var mongoSettings = builder.Configuration.GetSection("MongoDbSettings");
            var connectionString = mongoSettings["ConnectionString"];
            var databaseName = mongoSettings["DatabaseName"];

            builder.Services.AddSingleton(sp => new MongoDbContext(connectionString!, databaseName!));
            builder.Services.AddScoped<ITodoListRepository, TodoListRepository>();
            builder.Services.AddScoped<TodoListService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Настройка конвейера обработки запросов (Middleware Pipeline)
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.MapControllers();
            app.Run();
        }
    }
}