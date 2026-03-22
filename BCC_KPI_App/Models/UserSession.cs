namespace BCC_KPI_App.Models
{
    public static class UserSession
    {
        public static string UserName { get; set; }
        public static string FullName { get; set; }
        public static int UserRoleID { get; set; }

        public static bool IsAdmin => UserRoleID == 1;

        public static void Clear()
        {
            UserName = null;
            FullName = null;
            UserRoleID = 0;
        }
    }
}