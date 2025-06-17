using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories.Interfaces;
using ChapeauHerkansing.Services.Interfaces;

namespace ChapeauHerkansing.Services
{
    public class Bar_KitchenService : IBar_KitchenService
    {
        private readonly IBar_KitchenRepository _bar_KitchenRepository;

        public Bar_KitchenService(IBar_KitchenRepository Bar_Kitchenrepository)
        {
            _bar_KitchenRepository = Bar_Kitchenrepository;
        }

        public List<Order> GetOngoingOrders(Role role)
        {
            return _bar_KitchenRepository.GetAllOrdersByStatusAndCategory(OrderStatus.Ordered, role);
        }

        public List<Order> GetFinishedOrders(Role role)
        {
            return _bar_KitchenRepository.GetAllOrdersByStatusAndCategory(OrderStatus.Ready, role);
        }

        public void ToggleOrderLineStatus(int orderlineId)
        {
            _bar_KitchenRepository.ToggleOrderLineStatus(orderlineId);
        }
    }
}
