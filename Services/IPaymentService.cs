using ChapeauHerkansing.Models;

namespace ChapeauHerkansing.Interfaces
{
    public interface IPaymentService
    {
        decimal CalculateTotal(Order order);
        bool ValidatePayment(Payment payment, decimal total);
        void FinalizePayment(Payment payment, Order order);
    }
}
