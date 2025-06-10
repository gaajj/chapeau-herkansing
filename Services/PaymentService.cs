using ChapeauHerkansing.Interfaces;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Repositories;
using System;
using System.Linq;

namespace ChapeauHerkansing.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly TableRepository _tableRepo;
        private readonly OrderRepository _orderRepo;

        public PaymentService(IPaymentRepository paymentRepo, TableRepository tableRepo, OrderRepository orderRepo)
        {
            _paymentRepo = paymentRepo;
            _tableRepo = tableRepo;
            _orderRepo = orderRepo;
        }

        public decimal CalculateTotal(Order order)
        {
            return order.OrderLines.Sum(ol => (ol.MenuItem?.Price ?? 0m) * (ol.Amount));
        }

        public bool ValidatePayment(Payment payment, decimal totalAmount)
        {
            return payment.AmountPaid >= totalAmount;
        }

        public void FinalizePayment(Payment payment, Order order)
        {
            _paymentRepo.InsertPayment(payment);
            _tableRepo.UpdateTableStatus(payment.Order.Table.TableID, TableStatus.Free);
            _orderRepo.SoftDeleteOrder(order.OrderID);
        }
    }
}
