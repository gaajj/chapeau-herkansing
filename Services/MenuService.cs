using System.Collections.Generic;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Repositories;
using ChapeauHerkansing.ViewModels.Management;

namespace ChapeauHerkansing.Services
{
    public class MenuService
    {
        private readonly MenuItemRepository _menuItemRepository;
        private readonly StockService _stockService;
        private readonly MenuItemMenuRepository _menuItemMenuRepo;

        public MenuService(MenuItemRepository menuItemRepo, StockService stockService, MenuItemMenuRepository menuItemMenuRepo)
        {
            _menuItemRepository = menuItemRepo;
            _stockService = stockService;
            _menuItemMenuRepo = menuItemMenuRepo;
        }


        public MenuService(MenuItemRepository menuItemRepository, StockRepository stockRepo, StockService stockService)
        {
            _menuItemRepository = menuItemRepository;
            _stockService = stockService;
        }


        public List<MenuItem> GetAllMenuItems()
        {
            return _menuItemRepository.GetAllMenuItems();
        }

        public List<MenuItem> GetFilteredMenuItems(MenuType menuType, MenuCategory? category, bool includeDeleted = false)
        {
            return _menuItemRepository.GetMenuItemsByFilter(menuType, category, includeDeleted);
        }


        public void AddMenuItem(MenuItemCreateViewModel model)
        {
            int stockId = _stockService.CreateInitialStock(model.StockAmount);
            int menuItemId = _menuItemRepository.InsertMenuItem(model, stockId);
            _menuItemMenuRepo.LinkMenuItemToMenu((int)model.MenuType, menuItemId);

        }

        public MenuItem GetMenuItemById(int id)
        {
            return _menuItemRepository.GetMenuItemById(id);
        }

        public void UpdateMenuItem(int id, MenuItemCreateViewModel model)
        {
            _stockService.UpdateStock(id, model.StockAmount);
            _menuItemRepository.UpdateMenuItem(id, model);
        }

        public bool ToggleMenuItemActive(int id)
        {
            return _menuItemRepository.ToggleActive(id);
        }



    }
}
