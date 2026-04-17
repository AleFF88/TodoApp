using System.ComponentModel.DataAnnotations;

namespace TodoApp.Api.DTOs {

    /// <summary>
    /// Data required to create a new todo list.
    /// </summary>
    public class CreateTodoListRequest {
        /// <summary>
        /// The title of the todo list. Must be between 1 and 255 characters.
        /// </summary>
        /// <example>My Private Tasks</example>
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data required to update an existing todo list's information.
    /// </summary>
    public class UpdateTodoListRequest {
        /// <summary>
        /// The new title for the todo list. Must be between 1 and 255 characters.
        /// </summary>
        /// <example>Updated Project Title</example>
        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;
    }

    /// <summary>
    /// Data required to share a todo list with another user.
    /// </summary>
    public class ShareListRequest {
        /// <summary>
        /// The unique identifier of the user to whom access will be granted.
        /// </summary>
        [Required]
        public Guid TargetUserId { get; set; }
    }
}