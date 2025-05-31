using System;
using System.Collections.Generic;

namespace ChapeauHerkansing.ViewModels.Management
{
    public class FinancialOverviewViewModel
    {
        public string SelectedPeriod { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Dictionary<string, int> TotalSalesByType { get; set; }  // b.v. {"lunch": 14, "dinner": 8}
        public Dictionary<string, decimal> TotalIncomeByType { get; set; } // b.v. {"drinks": 120.50M}
        public decimal TotalTipAmount { get; set; }
    }
}
