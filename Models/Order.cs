namespace ChapeauHerkansing.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public Table? Table { get; set; }
     
        public bool IsDeleted { get; set; }
        public List<OrderLine> OrderLines { get; set; } = new List<OrderLine>();

        // Basic Constructor
        public Order(int orderID, Table table, bool isDeleted)
        {
            OrderID = orderID;
            Table = table;
          
            IsDeleted = isDeleted;

            OrderLines = new List<OrderLine>();
        }

        public Order() { }

    }
}

