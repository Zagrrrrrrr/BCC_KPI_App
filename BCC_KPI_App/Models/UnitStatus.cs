using System.ComponentModel.DataAnnotations;

namespace BCC_KPI_App.Models
{
    public class UnitStatus
    {
        [Key]
        public int StatusID { get; set; }
        public string StatusName { get; set; }

        public UnitStatus()
        {
            StatusName = "";
        }
    }
}