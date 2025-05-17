using Microsoft.AspNetCore.Mvc;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories;
using System.Linq;

namespace ChapeauHerkansing.Controllers
{
    public class TableOverviewController : Controller
    {
        private readonly TableRepository _tableRepo;
        public TableOverviewController(TableRepository tableRepo)
        {
            _tableRepo = tableRepo;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var tables = _tableRepo.GetAllTables();
            var readyCounts = tables.ToDictionary(
                t => t.TableID,
                t => _tableRepo.GetReadyOrdersCount(t.TableID)
            );
            var statuses = tables.ToDictionary(
                t => t.TableID,
                t => _tableRepo.GetRunningOrderStatuses(t.TableID)
            );

            var vm = new TableOverviewViewModel
            {
                Tables = tables,
                ReadyOrderCounts = readyCounts,
                RunningOrderStatuses = statuses,
                ErrorMessage = TempData["ErrorMessage"] as string
            };
            return View(vm);
        }


        [HttpPost]
        public IActionResult ServeOrders(int tableId)
        {
            _tableRepo.ServeOrdersForTable(tableId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult SetTableStatus(int tableId, string newStatus)
        {
            if (!Enum.TryParse<TableStatus>(newStatus, true, out var status))
            {
                TempData["ErrorMessage"] = "Invalid status.";
                return RedirectToAction("Index");
            }
            if (status == TableStatus.Free && _tableRepo.HasUnfinishedOrders(tableId))
            {
                TempData["ErrorMessage"] = "Cannot free table with unfinished orders.";
                return RedirectToAction("Index");
            }
            _tableRepo.UpdateTableStatus(tableId, status);
            return RedirectToAction("Index");
        }




    }
}
