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

        public OrderLine(int orderLineID, Order order, MenuItem menuItem, Staff staff, int amount, DateTime? orderTime, string note)
        {
            OrderLineID = orderLineID;
            Order = order;
            MenuItem = menuItem;
            Staff = staff;
            Amount = amount;
            OrderTime = orderTime;
            Note = note;
        }
    }
}

