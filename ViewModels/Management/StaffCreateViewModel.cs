using ChapeauHerkansing.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ChapeauHerkansing.ViewModels.Management
{
    public class StaffCreateViewModel
    {
        [Required(ErrorMessage = "Voornaam is verplicht.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Achternaam is verplicht.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Gebruikersnaam is verplicht.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Wachtwoord is verplicht.")]
        [MinLength(4, ErrorMessage = "Wachtwoord moet minimaal 4 tekens zijn.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Rol is verplicht.")]
        public Role Role { get; set; }
    }
}
