namespace TodoApp.Domain.Entities {
    public class User {
        public Guid Id { get; private set; }
        public string UserName { get; private set; }
        public User(Guid id, string userName) {
            Id = id;
            UserName = userName;

        }
    }
}
