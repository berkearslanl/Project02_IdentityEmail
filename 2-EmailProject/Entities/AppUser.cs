using Microsoft.AspNetCore.Identity;

namespace _2_EmailProject.Entities
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string? ImageUrl { get; set; }
        public string? About { get; set; }
        public string ConfirmCode { get; set; }
    }
}
