using System;
using System.ComponentModel.DataAnnotations;

namespace BCC_KPI_App.Models
{
    public class KpiTarget
    {
        [Key]
        public int TargetId { get; set; }
        public int UnitId { get; set; }
        public decimal TargetValue { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }

        public virtual Unit Unit { get; set; }

        public KpiTarget()
        {
            Unit = null;
        }
    }
}