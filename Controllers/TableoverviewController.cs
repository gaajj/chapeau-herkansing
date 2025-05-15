using Microsoft.AspNetCore.Mvc;
using ChapeauHerkansing.Repositories;
using ChapeauHerkansing.Models;

namespace ChapeauHerkansing.Controllers
{
    public class TableOverviewController : Controller
    {
        private readonly TableRepository _tableRepository;

        // Constructor voor ophalen van repository via dependency injection
        public TableOverviewController(TableRepository tableRepository)
        {
            _tableRepository = tableRepository;
        }

        // Haalt alle tafels op en toont deze in de view
        public IActionResult Index()
        {
            List<Table> tables = _tableRepository.GetAllTables();
            return View(tables);
        }
    }
}
