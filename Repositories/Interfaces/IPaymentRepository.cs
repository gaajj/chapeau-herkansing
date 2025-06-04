using ChapeauHerkansing.Models;

namespace ChapeauHerkansing.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        void InsertPayment(Payment payment);
    }
}
