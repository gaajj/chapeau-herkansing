using ChapeauHerkansing.Models;
using ChapeauHerkansing.Repositories;
using System;
using System.Collections.Generic;

namespace ChapeauHerkansing.Services
{
    public class FinancialService
    {
        private readonly FinancialRepository _repository;

        public FinancialService(FinancialRepository repository)
        {
            _repository = repository;
        }

        public List<FinancialData> GetFinancialOverview(DateTime start, DateTime end)
        {
            return _repository.GetFinancialData(start, end);
        }
    }
}
