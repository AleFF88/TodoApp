using System.ComponentModel.DataAnnotations;

namespace TodoApp.Api.DTOs {
    
    // То, что приходит от клиента при создании
    public class CreateTodoListRequest {
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;
    }

    // То, что приходит при обновлении
    public class UpdateTodoListRequest {
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;
    }

    // То, что мы отдаем клиенту (согласно ТЗ: только ID и название)
    public class TodoListResponse {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }

    // Для добавления связи
    public class ShareListRequest {
        [Required]
        public Guid TargetUserId { get; set; }
    }
}
