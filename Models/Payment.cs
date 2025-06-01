namespace ChapeauHerkansing.Models
{
    public class Payment
    {
        public int PaymentID { get; set; }
        public Order Order { get; set; }
        public decimal AmountPaid { get; set; }
        public PaymentMethod paymentMethodEnum { get; set; }
        public decimal Tip { get; set; }
        public string? Feedback { get; set; }


        public Payment(int paymentID, Order order, decimal amountPaid, PaymentMethod paymentMethodEnum, decimal tip, string feedback)
        {
            PaymentID = paymentID;
            Order = order;
            AmountPaid = amountPaid;
            this.paymentMethodEnum = paymentMethodEnum;
            Tip = tip;
            Feedback = feedback;
        }

        public Payment() { }
    }
}
