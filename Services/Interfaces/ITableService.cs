using System.Collections.Generic;
using ChapeauHerkansing.Models;
using ChapeauHerkansing.Models.Enums;

namespace ChapeauHerkansing.Services.Interfaces
{
    public interface ITableService
    {
        Table? GetTableById(int tableId);
        List<Table> GetAllTables();
        int GetReadyOrdersCount(int tableId);
        List<string> GetRunningOrderStatuses(int tableId);
        void ServeOrdersForTable(int tableId);
        bool HasUnfinishedOrders(int tableId);
        void UpdateTableStatus(int tableId, TableStatus status);
    }
}
