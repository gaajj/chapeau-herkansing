using ChapeauHerkansing.Models;

namespace ChapeauHerkansing.Services.Interfaces
{
    public interface IPaymentService
    {
        decimal CalculateTotal(Order order);
        bool ValidatePayment(Payment payment, decimal totalAmount);
        void FinalizePayment(Payment payment);
    }
}
