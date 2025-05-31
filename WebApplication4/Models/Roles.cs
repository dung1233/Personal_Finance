namespace WebApplication4.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; } = null!;

        public ICollection<User> Users { get; set; } = new List<User>();
    }

}