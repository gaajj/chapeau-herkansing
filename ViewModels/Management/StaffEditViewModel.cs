using ChapeauHerkansing.Models.Enums;

namespace ChapeauHerkansing.ViewModels.Management
{
    public class StaffEditViewModel
    {
        public int Id { get; set; }  // nodig voor update
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string? Password { get; set; }
        public Role Role { get; set; }
        public bool IsDeleted { get; set; }
    }
}
