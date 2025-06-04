using System.Collections.Generic;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;

namespace ChapeauHerkansing.Repositories.Interfaces
{
    public interface ITableRepository
    {
        List<Table> GetAllTables();
        int GetReadyOrdersCount(int tableId, string orderStatus, bool includeDeleted);
        void ServeOrdersForTable(int tableId);
        bool HasUnfinishedOrders(int tableId);
        void UpdateTableStatus(int tableId, TableStatus status);
        List<string> GetRunningOrderStatuses(int tableId);
        void SetTableFree(int tableId);
    }
}
