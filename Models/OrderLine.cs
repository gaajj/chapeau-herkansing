using ChapeauHerkansing.Models.Enums;

namespace ChapeauHerkansing.Models
{
    public class OrderLine
    {
        public int OrderLineID { get; set; }
        public Order Order { get; set; }
        public MenuItem MenuItem { get; set; }
        public Staff Staff { get; set; }
        public int Amount { get; set; }
        public DateTime? OrderTime { get; set; }
        public string Note { get; set; }
        public OrderStatus OrderStatus { get; set; }


        public decimal VAT //constanten maken van de 0.21 en 0.09
        {
            get
            {
                if (MenuItem?.Price == null || Amount == null)
                    return 0;

                decimal percentage = MenuItem.IsAlcoholic ? 0.21m : 0.09m;
                return MenuItem.Price * percentage * Amount;
            }
        }


        public OrderLine(int orderLineID, Order order, MenuItem menuItem, Staff staff, int amount, DateTime? orderTime, string note, OrderStatus orderStatus)
        {
            OrderLineID = orderLineID;
            Order = order;
            MenuItem = menuItem;
            Staff = staff;
            Amount = amount;
            OrderTime = orderTime;
            Note = note;
            this.OrderStatus = orderStatus;
        }
    }
}

