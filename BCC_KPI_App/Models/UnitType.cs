using System.ComponentModel.DataAnnotations;

namespace BCC_KPI_App.Models
{
    public class UnitType
    {
        [Key]
        public int TypeID { get; set; }
        public string TypeName { get; set; }

        public UnitType()
        {
            TypeName = "";
        }
    }
}