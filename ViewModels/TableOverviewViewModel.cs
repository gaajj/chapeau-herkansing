using ChapeauHerkansing.Models;
using System.Collections.Generic;

namespace ChapeauHerkansing.ViewModels
{
    public class TableOverviewViewModel
    {
        public List<Table> Tables { get; set; }
        public Dictionary<int, int> ReadyOrderCounts { get; set; }

        public Dictionary<int, List<string>> RunningOrderStatuses { get; set; }

        public string ErrorMessage { get; set; }
    }
}
