using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Reflection;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Configuration {
    public static class MongoDbMapping {
        public static void Register() {
            if (BsonClassMap.IsClassMapRegistered(typeof(TodoList)))
                return;

            BsonClassMap.RegisterClassMap<TodoList>(cm => {
                // Явно связывает свойство Id с системным полем _id в BSON-документе.
                // Предотвращает дублирование идентификаторов при сохранении сущности.
                cm.MapIdField(c => c.Id);

                // Публичные свойства с приватными сеттерами
                cm.MapProperty(c => c.Title);
                cm.MapProperty(c => c.OwnerId);
                cm.MapProperty(c => c.CreatedAt);

                // Позволяет драйверу записать данные в _sharedUserIds,
                //   не смотря на то, что это IReadOnlyCollection (не поддерживает запись).
                cm.MapField("_sharedUserIds")       // Доступ к полю через рефлексию
                  .SetElementName("SharedUserIds");  // Имя ключа в BSON документе (в БД)

                // Регистрирует конструктора для десериализации
                // Ищем конструктор с 5 параметрами (id, title, ownerId, createdAt, sharedUserIds)
                // Находит приватный конструктор с 5 параметрами (id, title, ownerId,
                //   createdAt, sharedUserIds) для восстановления данных из базы.
                // BindingFlags необходимы что бы найти приватный конструктор,
                //   так как по умолчанию BsonClassMap ищет только публичные.
                var ctor = typeof(TodoList).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                    .FirstOrDefault(c => c.GetParameters().Length == 5);

                if (ctor == null) {
                    throw new InvalidOperationException($"Private constructor with 5 parameters not found for {nameof(TodoList)}.");
                }

                // Регистрирует специальный конструктор
                //   и устанавливает связь параметров конструктора с полями BSON-документа.
                cm.MapConstructor(ctor, "Id", "Title", "OwnerId", "CreatedAt", "_sharedUserIds");
            });
        }
    }
}