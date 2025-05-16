namespace ChapeauHerkansing.Models
{
    public class OrderLine
    {
        public int OrderLineID { get; set; }
        public Order Order { get; set; }
        public MenuItem MenuItem { get; set; }
        public Staff User { get; set; }
        public int? Amount { get; set; }
        public DateTime? OrderTime { get; set; }
        public string Instruction { get; set; }

        // Basic Constructor
        public OrderLine(int orderLineID, Order order, MenuItem menuItem, Staff user, int? amount, DateTime? orderTime, string instruction)
        {
            OrderLineID = orderLineID;
            Order = order;
            MenuItem = menuItem;
            User = user;
            Amount = amount;
            OrderTime = orderTime;
            Instruction = instruction;
        }
    }
}

