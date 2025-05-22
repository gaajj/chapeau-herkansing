using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;



namespace ChapeauHerkansing.Controllers
{
    [Authorize(Roles = "Waiter")]
    public class PaymentController : Controller
    {
        private readonly OrderRepository _orderRepo;
        private readonly PaymentRepository _paymentRepo;
        private readonly TableRepository _tableRepo;

        public PaymentController(IConfiguration configuration)
        {
            _orderRepo = new OrderRepository(configuration);
            _paymentRepo = new PaymentRepository(configuration);
            _tableRepo = new TableRepository(configuration);
        }

        [HttpGet]
        public IActionResult Create(int? orderId)
        {
            ViewBag.Orders = _orderRepo.GetAll();
            Payment payment = new Payment();

            if (orderId.HasValue)
                payment.Order = GetOrderById(orderId.Value);

            return View(payment);
        }

        [HttpPost]
        public IActionResult Create(Payment payment, decimal totalPaid, decimal? tip, int? splitBetween)
        {
            ViewBag.Orders = _orderRepo.GetAll();

            if (!ModelState.IsValid)
                return View(payment);

            payment.Order = GetOrderById(payment.Order?.OrderID);
            if (payment.Order == null) return NotFound();

            decimal totalAmount = GetTotalPrice(payment.Order);
            SetPaymentValues(payment, totalPaid, tip, totalAmount);

            if (!IsValidPayment(payment, totalAmount))
                return View(payment);

            HandleSplit(splitBetween, payment.AmountPaid);
            SaveAndFinalize(payment);

            return RedirectToAction("Index", "Tableoverview");
        }



        private Order? GetOrderById(int? orderId)
        {
            return _orderRepo.GetAll().FirstOrDefault(o => o.OrderID == orderId);
        }

        private static decimal GetTotalPrice(Order order)
        {
            return order.OrderLines.Sum(ol => (ol.MenuItem?.Price ?? 0m) * (decimal?)ol.Amount ?? 0m);
        }

        private void SetPaymentValues(Payment payment, decimal? totalPaid, decimal? tip, decimal total)
        {
            if (totalPaid.HasValue && totalPaid.Value >= total)
            {
                payment.AmountPaid = totalPaid.Value;
                payment.Tip = totalPaid.Value - total;
            }
            else if (tip.HasValue)
            {
                payment.Tip = tip.Value;
                payment.AmountPaid = total + tip.Value;
            }
        }

        private bool IsValidPayment(Payment payment, decimal total)
        {
            if (payment.AmountPaid < total)
            {
                ModelState.AddModelError("AmountPaid", "Het betaalde bedrag mag niet lager zijn dan het totaal.");
                return false;
            }
            return true;
        }

        private void HandleSplit(int? splitBetween, decimal total)
        {
            if (splitBetween.HasValue && splitBetween.Value > 1)
                ViewBag.SplitAmount = Math.Round(total / splitBetween.Value, 2);
        }

        private void SaveAndFinalize(Payment payment)
        {
            _paymentRepo.InsertPayment(payment);
            _tableRepo.SetTableFree(payment.Order.Table.TableID);
        }
    }
}
