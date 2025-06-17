using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories.Interfaces;
using ChapeauHerkansing.Services.Interfaces;
using ChapeauHerkansing.Repositories;

namespace ChapeauHerkansing.Services
{
    public class Bar_KitchenService : IBar_KitchenService
    {
        private readonly IBar_KitchenRepository _bar_KitchenRepository;

        public Bar_KitchenService(IBar_KitchenRepository bar_KitchenRepository)
        {

            _bar_KitchenRepository = bar_KitchenRepository ?? throw new ArgumentNullException(nameof(bar_KitchenRepository));
        }

        public List<Order> GetOngoingOrders(Role role)
        {
            if (!Enum.IsDefined(typeof(Role), role))
                throw new InvalidOperationException("Ongeldige gebruikersrol.");

            try
            {
                return _bar_KitchenRepository.GetAllOrdersByStatusAndCategory(OrderStatus.Ordered, role);
            }
            catch
            {

                throw new InvalidOperationException("Kon de lopende bestellingen niet ophalen. Probeer het later opnieuw.");
            }


        }

        public List<Order> GetFinishedOrders(Role role)
        {
            if (!Enum.IsDefined(typeof(Role), role))
                throw new InvalidOperationException("Ongeldige gebruikersrol.");


            try
            {
                return _bar_KitchenRepository.GetAllOrdersByStatusAndCategory(OrderStatus.Ready, role);
            }
            catch
            {
                throw new InvalidOperationException("Kon de lopende bestellingen niet ophalen. Probeer het later opnieuw.");
            }
        }
        public void ToggleOrderLineStatus(int orderlineId)
        {
            if (orderlineId <= 0)
                throw new InvalidOperationException("Ongeldige orderline.");

            try
            {
                _bar_KitchenRepository.ToggleOrderLineStatus(orderlineId);
            }
            catch {
                throw new InvalidOperationException(
          "Kon de status van de orderregel niet bijwerken. Probeer het later opnieuw.");
            }
        }
    }
}
