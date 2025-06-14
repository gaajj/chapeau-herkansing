using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using System.Globalization;

namespace ChapeauHerkansing.ViewModels
{
    public class PaymentViewModel
    {
        public int OrderId { get; set; }

        public Order? Order { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();

        public decimal AmountPaid { get; set; }
        public decimal Tip { get; set; }

        public PaymentMethod? PaymentMethodEnum { get; set; }

        public string? Feedback { get; set; }

        public int? SplitBetween { get; set; }


        public decimal TotalAmount => Order?.OrderLines?.Sum(ol =>
        {
            decimal price = ol.MenuItem?.Price ?? 0m;
            int amount = ol.Amount;
            return (price * amount) + ol.VAT;
        }) ?? 0m;



        public decimal? SplitAmount =>
            SplitBetween.HasValue && SplitBetween > 1
                ? Math.Round(TotalAmount / SplitBetween.Value, 2)
                : null;

        public decimal VatAmount { get; set; }

    }

}
