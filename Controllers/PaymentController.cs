using Microsoft.AspNetCore.Mvc;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories;

namespace ChapeauHerkansing.Controllers
{
    public class PaymentController : Controller
    {
        private readonly OrderRepository _orderRepo;
        private readonly PaymentRepository _paymentRepo;

        public PaymentController(IConfiguration configuration)
        {
            _orderRepo = new OrderRepository(configuration);
            _paymentRepo = new PaymentRepository(configuration);
        }


        public IActionResult Create(int orderId)
        {
            var order = _orderRepo.GetAll().Last();
            if (order == null)
                return NotFound();

            Payment payment = new Payment
            {
                Order = order
            };

            return View(payment);
        }

        [HttpPost]

        public IActionResult Create(Payment payment)
        {
            Console.WriteLine("POST aangeroepen");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState invalid!");
                foreach (var e in ModelState)
                {
                    Console.WriteLine($"{e.Key}: {string.Join(", ", e.Value.Errors.Select(err => err.ErrorMessage))}");
                }
                return View(payment);
            }

            Console.WriteLine("OrderID ontvangen: " + payment.Order?.OrderID);

            var fullOrder = _orderRepo.GetAll().FirstOrDefault(o => o.OrderID == payment.Order.OrderID);
            if (fullOrder == null)
            {
                Console.WriteLine("Order niet gevonden!");
                return NotFound();
            }

            payment.Order = fullOrder;

            Console.WriteLine("Insert starten...");
            _paymentRepo.InsertPayment(payment);
            Console.WriteLine("Insert gedaan");

            return RedirectToAction("Index", "Home");
        }


    }
}
