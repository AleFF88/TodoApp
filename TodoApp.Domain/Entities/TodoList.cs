namespace TodoApp.Domain.Entities {
    public class TodoList {
        private readonly List<Guid> _sharedUserIds = new List<Guid>();

        public Guid Id { get; private set; }
        public string Title { get; private set; } 
        public Guid OwnerId { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public TodoList(Guid id, string title, Guid ownerId) {
            ValidateTitle(title);

            Id = id;
            Title = title;
            OwnerId = ownerId;
            CreatedAt = DateTime.UtcNow;    
        }

        public IReadOnlyCollection<Guid> SharedUserIds => _sharedUserIds.AsReadOnly();

        public void AddSharedUser(Guid userId) {
            if (!_sharedUserIds.Contains(userId) && userId != OwnerId) {
                _sharedUserIds.Add(userId);
            }
        }

        public void RemoveSharedUser(Guid userId) {
            _sharedUserIds.Remove(userId);
        }

        public void UpdateTitle(string newTitle) {
            ValidateTitle(newTitle);
            Title = newTitle;
        }

        private static void ValidateTitle(string title) {
            if (string.IsNullOrWhiteSpace(title) || title.Length > 255) {
                throw new ArgumentException("Title must be between 1 and 255 characters.");
            }
        }
    }
}
