namespace api.Repositories.Entities
{
    public class User
    {
        public required long Id { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
    }
}
