using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.ViewModels.Management;

namespace ChapeauHerkansing.Services.Interfaces
{
    public interface IMenuService
    {
        Menu GetAllMenu();
        Menu GetFilteredMenu(MenuType menuType, MenuCategory? category, bool includeDeleted);
        void AddMenuItem(MenuItemCreateViewModel model);
        MenuItem GetMenuItemById(int id);
        void UpdateMenuItem(int id, MenuItemCreateViewModel model);
        bool ToggleMenuItemActive(int id);
        Menu GetMenuItemsByMenuType(MenuType menuType);
    }
}
