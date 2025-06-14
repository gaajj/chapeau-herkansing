using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories.Interfaces;
using ChapeauHerkansing.Services.Interfaces;

namespace ChapeauHerkansing.Services
{
    public class Bar_KitchenService : IBar_KitchenService
    {
        private readonly IOrderRepository _orderRepository;

        public Bar_KitchenService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public List<Order> GetOngoingOrders()
        {
            return _orderRepository.GetAllOrdersByStatus(OrderStatus.Ordered);
        }

        public List<Order> GetFinishedOrders()
        {
            return _orderRepository.GetAllOrdersByStatus(OrderStatus.Ready);
        }

        public void ToggleOrderLineStatus(int orderlineId)
        {
            _orderRepository.ToggleOrderLineStatus(orderlineId);
        }
    }
}
