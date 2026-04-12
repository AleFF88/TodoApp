using MongoDB.Bson.Serialization;
using System.Reflection;
using TodoApp.Domain.Entities;

namespace TodoApp.Infrastructure.Configuration {
    public static class MongoDbMapping {
        public static void Register() {
            BsonClassMap.RegisterClassMap<TodoList>(cm => {
                // Выполняет базовое сканирование класса для сопоставления публичных свойств. 
                // В данный момент у нас таких нет, но лучше не отключать.
                cm.AutoMap();

                // Явно связывает свойство Id с системным полем _id в BSON-документе.
                // Предотвращает дублирование идентификаторов при сохранении сущности.
                cm.MapIdField(c => c.Id);

                // Позволяет драйверу записать данные в _sharedUserIds,
                //   не смотря на то, что это IReadOnlyCollection (не поддерживает запись).
                cm.MapField("_sharedUserIds")       // Доступ к полю через рефлексию
                  .SetElementName("SharedUserIds"); // Имя ключа в BSON документе (в БД)


                // Находит приватный конструктор для восстановления данных из базы.
                //   BindingFlags необходимы что бы найти приватный конструктор,
                //   так как по умолчанию BsonClassMap ищет только публичные.
                // 1. Находим приватный конструктор с полными параметрами
                //   (включая CreatedAt и sharedUserIds)
                var ctor = typeof(TodoList).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0];
                // 2. Регистрирует специальный конструктор
                //   и устанавливает связь параметров конструктора с полями BSON-документа.
                cm.MapConstructor(ctor, "Id", "Title", "OwnerId", "CreatedAt", "SharedUserIds");
            });
        }
    }
}