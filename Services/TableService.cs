using System.Collections.Generic;
using System.Linq;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;
using ChapeauHerkansing.Repositories;

namespace ChapeauHerkansing.Services
{
    public class TableService
    {
        private readonly TableRepository _tableRepo;

        public TableService(TableRepository tableRepo)
        {
            _tableRepo = tableRepo;
        }

        public List<Table> GetAllTables()
            => _tableRepo.GetAllTables();

        public int GetReadyOrdersCount(int tableId)
            => _tableRepo.GetReadyOrdersCount(tableId);

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
