using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Models;

namespace ChapeauHerkansing.ViewModels.Bar_Kitchen
{
    public class OrdersViewModel
    {
        public List<Order> Orders { get; set; }
        public Role Role { get; set; }
        public Boolean isFinished { get; set; } = false;

        public String Title {
            get
            {
                return (isFinished ? "Finished" : "Ongoing") + (Role == Role.Chef ? " Kitchen" : " Bar") + " Orders";
            }
        }

    }
}
