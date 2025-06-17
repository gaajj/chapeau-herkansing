using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Models;

namespace ChapeauHerkansing.ViewModels.Bar_Kitchen
{
    public class OrdersViewModel
    {
        public List<Order> Orders { get; set; }
        public Role Role { get; set; }
        public Boolean isFinished { get; set; } = false;

        public String Title
        {
            get
            {
                return (isFinished ? "Finished" : "Ongoing") + (Role == Role.Chef ? " Kitchen" : " Bar") + " Orders";
            }
        }

        public string GetCategoryColor(MenuCategory category)
        {
            return category switch
            {
                MenuCategory.Voorgerecht => "#4CAF50",      // green
                MenuCategory.Tussengerecht => "#FFC107", // amber
                MenuCategory.Hoofdgerecht => "#F44336",         // red
                MenuCategory.Nagerecht => "#FFEB3B",      // yellow
                MenuCategory.Dranken => "#2196F3",         // blue
                _ => "#E0E0E0"   //light gray
            };
        }
    }

    }

