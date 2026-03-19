using System;
using System.ComponentModel.DataAnnotations;

namespace BCC_KPI_App.Models
{
    public class KpiActual
    {
        [Key]
        public int ActualId { get; set; }
        public int UnitId { get; set; }
        public decimal ActualValue { get; set; }
        public DateTime SaleDate { get; set; }
        public string Description { get; set; }

        public virtual Unit Unit { get; set; }

        public KpiActual()
        {
            SaleDate = DateTime.Now;
            Description = "";
            Unit = null;
        }
    }
}