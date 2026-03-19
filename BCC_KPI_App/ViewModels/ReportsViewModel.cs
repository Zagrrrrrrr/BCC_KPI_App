using System;
using System.Collections.Generic;
using System.Linq;
using BCC_KPI_App.Models;

namespace BCC_KPI_App.ViewModels
{
    public class ReportsViewModel : BaseViewModel
    {
        private readonly SalesContext _context;
        private DateTime _startDate;
        private DateTime _endDate;

        public ReportsViewModel()
        {
            _context = new SalesContext();
            _startDate = new DateTime(DateTime.Now.Year, 1, 1);
            _endDate = DateTime.Now;
        }

        public DateTime StartDate
        {
            get => _startDate;
            set { _startDate = value; OnPropertyChanged(); }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set { _endDate = value; OnPropertyChanged(); }
        }
    }
}