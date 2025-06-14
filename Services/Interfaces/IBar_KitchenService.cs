using ChapeauHerkansing.Models;

namespace ChapeauHerkansing.Services.Interfaces
{
    public interface IBar_KitchenService
    {
        List<Order> GetOngoingOrders();
        List<Order> GetFinishedOrders();
        void ToggleOrderLineStatus(int orderlineId);

    }
}
