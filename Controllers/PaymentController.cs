using ChapeauHerkansing.Interfaces;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories;
using ChapeauHerkansing.Services;
using ChapeauHerkansing.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace ChapeauHerkansing.Controllers
{
    [Authorize(Roles = "Waiter")]
    public class PaymentController : Controller
    {
        private readonly OrderRepository _orderRepo; 
        private readonly IPaymentService _paymentService; // repos moeten in de services

        public PaymentController(IPaymentService paymentService, IConfiguration config)
        {
            _orderRepo = new OrderRepository(config); 
            _paymentService = paymentService;         
        }


        [HttpGet]
        public IActionResult Create(int orderId) // methodenaam wijzigen
        {
            List<Order> orders = _orderRepo.GetAll().Where(o => !o.IsDeleted).ToList();
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
            Order order = _orderRepo.GetAll().FirstOrDefault(o => o.OrderID == viewModel.OrderId);
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
                    
            _paymentService.FinalizePayment(payment, order);
            return RedirectToAction("Index", "Tableoverview");
        }
    }
}
