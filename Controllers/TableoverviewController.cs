using Microsoft.AspNetCore.Mvc;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Services;
using ChapeauHerkansing.ViewModels.Tables;


namespace ChapeauHerkansing.Controllers
{
    [Authorize(Roles = "Waiter")]
    public class TableOverviewController : Controller
    {
        private readonly TableService _tableService;

        public TableOverviewController(TableService tableService)
        {
            _tableService = tableService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var tables = _tableService.GetAllTables();
            var readyCounts = tables.ToDictionary(
                t => t.TableID,
                t => _tableService.GetReadyOrdersCount(t.TableID)
            );
            var statuses = tables.ToDictionary(
                t => t.TableID,
                t => _tableService.GetRunningOrderStatuses(t.TableID)
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
            _tableService.ServeOrdersForTable(tableId);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult SetTableStatus(int tableId, string newStatus)
        {
            if (!Enum.TryParse<TableStatus>(newStatus, true, out var status))
            {
                TempData["ErrorMessage"] = "Invalid status.";
                return RedirectToAction(nameof(Index));
            }
            if (status == TableStatus.Free && _tableService.HasUnfinishedOrders(tableId))
            {
                TempData["ErrorMessage"] = "Cannot free table with unfinished orders.";
                return RedirectToAction(nameof(Index));
            }
            _tableService.UpdateTableStatus(tableId, status);
            return RedirectToAction(nameof(Index));
        }

    }
}
