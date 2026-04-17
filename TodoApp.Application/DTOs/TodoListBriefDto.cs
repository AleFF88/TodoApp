namespace TodoApp.Application.DTOs {
    /// <summary>
    /// A condensed representation of a todo list, typically used for lists and grids.
    /// </summary>
    /// <param name="Id">The unique identifier of the todo list.</param>
    /// <param name="Title">The display title of the todo list.</param>
    public record TodoListBriefDto(Guid Id, string Title);
}
