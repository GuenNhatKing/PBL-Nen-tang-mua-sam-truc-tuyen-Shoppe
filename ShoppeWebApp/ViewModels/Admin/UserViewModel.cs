namespace ShoppeWebApp.ViewModels.Admin
{
    public class UserViewModel
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public int Status { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Cccd { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? AvatarUrl { get; set; } 
    }
}