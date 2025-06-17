using Microsoft.AspNetCore.Mvc;
using ChapeauHerkansing.Repositories;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Services;
using ChapeauHerkansing.ViewModels.Tables;
using ChapeauHerkansing.ViewModels;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Services.Interfaces;




namespace ChapeauHerkansing.Controllers
{
    [Authorize(Roles = "Waiter")]
    public class TableOverviewController : Controller
    {
        private readonly ITableService _tableService;

        public TableOverviewController(ITableService tableService)
        {
            _tableService = tableService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            List<Table> tables = _tableService.GetAllTables();

            Dictionary<int, int> readyCounts = tables.ToDictionary(
                t => t.TableID,
                t => _tableService.GetReadyOrdersCount(t.TableID)
            );

            Dictionary<int, List<string>> statuses = tables.ToDictionary(
                t => t.TableID,
                t => _tableService.GetRunningOrderStatuses(t.TableID)
            );

            TableOverviewViewModel vm = new TableOverviewViewModel
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
            // Controleer of de string newStatus zonder hoofdlettergevoeligheid overeenkomt met een TableStatus, en sla deze op in status; ga het if-blok in als dat mislukte.
            if (!Enum.TryParse<TableStatus>(newStatus, true, out var status)) // 
            {
                TempData["ErrorMessage"] = "Invalid status.";
                return RedirectToAction(nameof(Index));
            }
            bool hasUnfinished = _tableService.HasUnfinishedOrders(tableId);

            if (status == TableStatus.Free && hasUnfinished)
            {
                TempData["ErrorMessage"] = "Cannot free table with unfinished orders.";
                return RedirectToAction(nameof(Index));
            }

            if (status == TableStatus.Reserved && hasUnfinished)
            {
                TempData["ErrorMessage"] = "Cannot reserve table with running orders.";
                return RedirectToAction(nameof(Index));
            }

            _tableService.UpdateTableStatus(tableId, status);
            return RedirectToAction(nameof(Index));
        }

    }
}
