namespace ChapeauHerkansing.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public int? OrderID { get; set; }
        public decimal? AmountPaid { get; set; }
        public string PaymentMethod { get; set; }
        public decimal? Tip { get; set; }

        // Basic Constructor
        public Payment(int paymentID, int? orderID, decimal? amountPaid, string paymentMethod, decimal? tip)
        {
            PaymentID = paymentID;
            OrderID = orderID;
            AmountPaid = amountPaid;
            PaymentMethod = paymentMethod;
            Tip = tip;
        }
    }
}

