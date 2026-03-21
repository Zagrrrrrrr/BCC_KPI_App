using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BCC_KPI_App.Models
{
    public class Unit
    {
        [Key]
        public int UnitId { get; set; }

        [Required]
        public string UnitName { get; set; }

        public string City { get; set; }

        public virtual ICollection<KpiTarget> KpiTargets { get; set; }
        public virtual ICollection<KpiActual> KpiActuals { get; set; }

        public Unit()
        {
            UnitName = "";
            City = "";
            KpiTargets = new HashSet<KpiTarget>();
            KpiActuals = new HashSet<KpiActual>();
        }
    }
}