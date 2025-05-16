namespace ChapeauHerkansing.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public Order Order { get; set; }
        public decimal? AmountPaid { get; set; }
        public string PaymentMethod { get; set; }
        public decimal? Tip { get; set; }

        // Basic Constructor
        public Payment(int paymentID, Order order, decimal? amountPaid, string paymentMethod, decimal? tip)
        {
            PaymentID = paymentID;
            Order = order;
            AmountPaid = amountPaid;
            PaymentMethod = paymentMethod;
            Tip = tip;
        }
    }
}

