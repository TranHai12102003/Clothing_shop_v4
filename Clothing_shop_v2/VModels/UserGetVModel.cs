namespace Clothing_shop_v2.VModels
{
    public class UserGetVModel
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }

    }
    public class UserUpdateVModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? UserName { get; set; }    
        public string? Gender { get; set; } 
        public string? Address { get; set; }
        public DateOnly? CreatedDate { get; set; }
    }
}
