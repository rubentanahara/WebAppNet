namespace api.DTOS
{
    public class UserDTO
    {
        public long Id { get; set; }
        public required string SystemName { get; set; }
        public required string Password { get; set; }
    }
}
