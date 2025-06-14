using System.Collections.Generic;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Repositories;
using ChapeauHerkansing.Repositories.Interfaces;
using ChapeauHerkansing.Services.Interfaces;
using ChapeauHerkansing.ViewModels.Management;

namespace ChapeauHerkansing.Services
{
    public class MenuService : IMenuService
    {
        private readonly IMenuItemRepository _menuItemRepository;

        public MenuService(IMenuItemRepository menuItemRepo)
        {
            _menuItemRepository = menuItemRepo;
        }

        public Menu GetAllMenu()
        {
            return _menuItemRepository.GetAllMenuItems();
        }


        public Menu GetFilteredMenu(MenuType menuType, MenuCategory? category, bool includeDeleted = false)
        {
            return _menuItemRepository.GetMenuItemsByFilter(menuType, category, includeDeleted);
        }



        public void AddMenuItem(MenuItemCreateViewModel model)
        {
            _menuItemRepository.InsertMenuItem(model);
        }

        public MenuItem GetMenuItemById(int id)
        {
            return _menuItemRepository.GetMenuItemById(id);
        }

        public void UpdateMenuItem(int id, MenuItemCreateViewModel model)
        {
            _menuItemRepository.UpdateMenuItem(id, model);
        }

        public bool ToggleMenuItemActive(int id)
        {
            return _menuItemRepository.ToggleActive(id);
        }

        public Menu GetMenuItemsByMenuType(MenuType menuType)
        {
            return _menuItemRepository.GetMenuItemsByMenuType(menuType);
        }


    }
}
