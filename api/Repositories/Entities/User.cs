namespace api.Repositories.Entities
{
    public class User
    {
        public required long Id { get; set; }
        public required string Password { get; set; }
        public required string Username { get; set; }
    }
}
