using System.Collections.Generic;
using ChapeauHerkansing.Models;

namespace ChapeauHerkansing.ViewModels.Tables
{
    public class TableOverviewViewModel
    {
        public List<Table> Tables { get; set; }
        public Dictionary<int, int> ReadyOrderCounts { get; set; }
        public Dictionary<int, List<string>> RunningOrderStatuses { get; set; }
        public string ErrorMessage { get; set; }
    }
}