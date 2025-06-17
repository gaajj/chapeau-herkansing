using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;

namespace ChapeauHerkansing.Services.Interfaces
{
    public interface IBar_KitchenService
    {
        List<Order> GetOngoingOrders(Role role);
        List<Order> GetFinishedOrders(Role role);
        void ToggleOrderLineStatus(int orderlineId);

    }
}
