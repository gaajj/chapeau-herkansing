using ChapeauHerkansing.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ChapeauHerkansing.ViewModels.Management
{
    public class StaffEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Voornaam is verplicht.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Achternaam is verplicht.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Gebruikersnaam is verplicht.")]
        public string Username { get; set; }

        public string? Password { get; set; } // Leeg laten = niet aanpassen

        [Required(ErrorMessage = "Rol is verplicht.")]
        public Role Role { get; set; }

        public bool IsDeleted { get; set; }
    }
}
