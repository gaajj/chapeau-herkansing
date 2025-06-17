using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.ViewModels.Management;

namespace ChapeauHerkansing.Repositories.Interfaces
{
    public interface IMenuItemRepository
    {
        Menu GetAllMenuItems();
        Menu GetMenuItemsByFilter(MenuType menuType, MenuCategory? category, bool includeDeleted);
        int InsertMenuItem(MenuItemCreateViewModel model);
        MenuItem GetMenuItemById(int id);
        void UpdateMenuItem(int id, MenuItemCreateViewModel model);
        bool ToggleActive(int id);
        Menu GetMenuItemsByMenuType(MenuType menuType);
        void UpdateStock(int menuItemId, int amountChange);
    }
}
