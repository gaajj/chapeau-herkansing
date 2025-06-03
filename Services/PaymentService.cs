using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories;
using System;
using System.Linq;

namespace ChapeauHerkansing.Services
{
    public class PaymentService
    {
        private readonly PaymentRepository _paymentRepo;
        private readonly TableRepository _tableRepo;

        public PaymentService(PaymentRepository paymentRepo, TableRepository tableRepo)
        {
            _paymentRepo = paymentRepo;
            _tableRepo = tableRepo;
        }

        public decimal CalculateTotal(Order order)
        {
            return order.OrderLines.Sum(ol => (ol.MenuItem?.Price ?? 0m) * (ol.Amount ?? 0m));
        }

        public bool ValidatePayment(Payment payment, decimal totalAmount)
        {
            return payment.AmountPaid >= totalAmount;
        }

        public void FinalizePayment(Payment payment)
        {
            _paymentRepo.InsertPayment(payment);
            _tableRepo.UpdateTableStatus(payment.Order.Table.TableID, TableStatus.Free);
        }
    }
}
