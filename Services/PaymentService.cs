using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Repositories;
using ChapeauHerkansing.Repositories.Interfaces;
using ChapeauHerkansing.Services.Interfaces;
using System;
using System.Linq;

namespace ChapeauHerkansing.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly ITableRepository _tableRepo;

        public PaymentService(IPaymentRepository paymentRepo, ITableRepository tableRepo)
        {
            _paymentRepo = paymentRepo;
            _tableRepo = tableRepo;
        }

        public decimal CalculateTotal(Order order)
        {
            return order.OrderLines.Sum(ol => (ol.MenuItem?.Price ?? 0m) * (ol.Amount));
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
