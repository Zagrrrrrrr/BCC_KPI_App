using System.ComponentModel.DataAnnotations;

namespace BCC_KPI_App.Models
{
    public class Holding
    {
        [Key]
        public int HoldingID { get; set; }
        public string HoldingName { get; set; }

        public Holding()
        {
            HoldingName = "";
        }
    }
}