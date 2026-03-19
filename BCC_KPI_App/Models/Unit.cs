using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BCC_KPI_App.Models
{
    public class Unit
    {
        [Key]
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public string UnitType { get; set; }
        public string Status { get; set; }
        public string City { get; set; }

        public virtual ICollection<KpiTarget> KpiTargets { get; set; }
        public virtual ICollection<KpiActual> KpiActuals { get; set; }

        public Unit()
        {
            UnitName = "";
            UnitType = "";
            Status = "";
            City = "";
            KpiTargets = new HashSet<KpiTarget>();
            KpiActuals = new HashSet<KpiActual>();
        }
    }
}