using System.ComponentModel.DataAnnotations;

namespace BCC_KPI_App.Models
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        public string Username { get; set; }

        public string PasswordHash { get; set; }

        public string FullName { get; set; }

        public int UserRoleID { get; set; }

        public string Role { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }
    }
}