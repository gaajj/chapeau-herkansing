using ChapeauHerkansing.Models;
using System.Collections.Generic;

// Interface voor tafelrepository
public interface ITableRepository
{
    List<Table> GetAllTables();
}
