namespace ChapeauHerkansing.Models
{
    public class OrderLine
    {
        public int OrderLineID { get; set; }
        public int? OrderID { get; set; }
        public int? MenuItemID { get; set; }
        public int? UserID { get; set; }
        public int? Amount { get; set; }
        public DateTime? OrderTime { get; set; }
        public string Instruction { get; set; }

        // Basic Constructor
        public OrderLine(int orderLineID, int? orderID, int? menuItemID, int? userID, int? amount, DateTime? orderTime, string instruction)
        {
            OrderLineID = orderLineID;
            OrderID = orderID;
            MenuItemID = menuItemID;
            UserID = userID;
            Amount = amount;
            OrderTime = orderTime;
            Instruction = instruction;
        }
    }
}

