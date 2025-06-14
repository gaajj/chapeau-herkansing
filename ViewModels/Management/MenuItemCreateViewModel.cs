using ChapeauHerkansing.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ChapeauHerkansing.ViewModels.Management
{
    public class MenuItemCreateViewModel
    {
        [Required(ErrorMessage = "Naam is verplicht.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Prijs is verplicht.")]
        [Range(0.01, 1000, ErrorMessage = "Prijs moet tussen 0.01 en 1000 liggen.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Categorie is verplicht.")]
        public MenuCategory Category { get; set; }

        public bool IsAlcoholic { get; set; }

        [Required(ErrorMessage = "Voorraad is verplicht.")]
        [Range(0, 1000, ErrorMessage = "Voorraad moet 0 of meer zijn.")]
        public int StockAmount { get; set; }

        [Required(ErrorMessage = "MenuType is verplicht.")]
        public MenuType MenuType { get; set; }
    }
}
