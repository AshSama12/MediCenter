namespace mediCenter.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // Doctor, Admin, etc.
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
