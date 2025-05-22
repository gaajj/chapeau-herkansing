using ChapeauHerkansing.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ChapeauHerkansing.ViewModels.Management
{
    public class MenuItemCreateViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [Range(0.1, 1000)]
        public decimal Price { get; set; }

        [Required]
        public MenuCategory Category { get; set; }

        public bool IsAlcoholic { get; set; }

        [Required]
        public int StockAmount { get; set; }

        [Required]
        public MenuType MenuType { get; set; }
    }
}
