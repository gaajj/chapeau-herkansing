using ChapeauHerkansing.Models;

namespace ChapeauHerkansing.Repositories
{
    public interface IPaymentRepository
    {
        void InsertPayment(Payment payment);
    }
}
