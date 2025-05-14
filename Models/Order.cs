namespace ChapeauHerkansing.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public int? TableID { get; set; }
        public string? Feedback { get; set; }
        public bool? IsDeleted { get; set; }

        // Basic Constructor
        public Order(int orderID, int? tableID, string? feedback, bool? isDeleted)
        {
            OrderID = orderID;
            TableID = tableID;
            Feedback = feedback;
            IsDeleted = isDeleted;
        }
    }
}

