using System.Collections.Generic;
using System.Linq;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Repositories.Interfaces;
using ChapeauHerkansing.Repositories;
using ChapeauHerkansing.Services.Interfaces;

namespace ChapeauHerkansing.Services
{
    public class TableService : ITableService
    {
        private readonly ITableRepository _tableRepo;

        public TableService(ITableRepository tableRepo)
        {
            _tableRepo = tableRepo;
        }

        public List<Table> GetAllTables()
            => _tableRepo.GetAllTables();

        public int GetReadyOrdersCount(int tableId)
    => _tableRepo.GetReadyOrdersCount(tableId, "ready", false);


        public List<string> GetRunningOrderStatuses(int tableId)
            => _tableRepo.GetRunningOrderStatuses(tableId);

        public void ServeOrdersForTable(int tableId)
            => _tableRepo.ServeOrdersForTable(tableId);

        public bool HasUnfinishedOrders(int tableId)
            => _tableRepo.HasUnfinishedOrders(tableId);

        public void UpdateTableStatus(int tableId, TableStatus status)
            => _tableRepo.UpdateTableStatus(tableId, status);
    }
}
