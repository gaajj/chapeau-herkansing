using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories;
using ChapeauHerkansing.Services;
using ChapeauHerkansing.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using ChapeauHerkansing.Repositories.Interfaces;



namespace ChapeauHerkansing.Controllers
{
    [Authorize(Roles = "Waiter")]
    public class PaymentController : Controller
    {
        private readonly IOrderRepository _orderRepo;
        private readonly PaymentService _paymentService;

        public PaymentController(IConfiguration config)
        {
            _orderRepo = new OrderRepository(config);
            PaymentRepository paymentRepo = new PaymentRepository(config);
            TableRepository tableRepo = new TableRepository(config);
            _paymentService = new PaymentService(paymentRepo, tableRepo);
        }

        [HttpGet]
        public IActionResult Create(int orderId)
        {
            List<Order> orders = _orderRepo.GetAllOrders().Where(o => !o.IsDeleted).ToList();
            //filteren in de query zelf vgm 
            Order order = orders.FirstOrDefault(o => o.OrderID == orderId);

            PaymentViewModel model = new PaymentViewModel
            {
                Orders = orders,
                Order = order,
                OrderId = orderId,
                VatAmount = order?.OrderLines.Sum(o => o.VAT) ?? 0
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(PaymentViewModel viewModel)
        {
            Order order = _orderRepo.GetAllOrders().FirstOrDefault(o => o.OrderID == viewModel.OrderId);
            if (order == null) return NotFound();

            decimal total = _paymentService.CalculateTotal(order);

            Payment payment = new Payment
            {
                Order = order,
                AmountPaid = viewModel.AmountPaid,
                Tip = viewModel.Tip,
                paymentMethodEnum = viewModel.PaymentMethodEnum.Value,
                Feedback = viewModel.Feedback
            };

            if (!_paymentService.ValidatePayment(payment, total))
            {
                ModelState.AddModelError("AmountPaid", "Het betaalde bedrag is te laag.");
                return View(viewModel);
            }

            _paymentService.FinalizePayment(payment);
            _orderRepo.SoftDeleteOrder(order.OrderID);

            return RedirectToAction("Index", "Tableoverview");
        }
    }
}
