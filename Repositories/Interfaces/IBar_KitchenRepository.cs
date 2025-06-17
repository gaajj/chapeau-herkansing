using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Models;

namespace ChapeauHerkansing.Repositories.Interfaces
{
    public interface IBar_KitchenRepository
    {
        List<Order> GetAllOrdersByStatusAndCategory(OrderStatus orderStatus, Role role);
        void ToggleOrderLineStatus(int orderLineId);


    }
}
