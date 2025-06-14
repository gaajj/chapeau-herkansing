using System;
using System.Collections.Generic;
using ChapeauHerkansing.Models;

namespace ChapeauHerkansing.ViewModels.Management
{
    public class FinancialOverviewViewModel
    {
        public List<FinancialData> ReportItems { get; set; } = new();
        public string SelectedPeriod { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
