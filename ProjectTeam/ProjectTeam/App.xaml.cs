using System.Configuration;
using System.Data;
using System.Windows;

namespace ProjectTeam
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static int CurrentUserId { get; set; }
        public static string CurrentUserName { get; set; }
        public static string CurrentUserEmail { get; set; }
        public static int CurrentRoleId { get; set; }
        public static string CurrentRoleName { get; set; }

        public static bool IsStaff => CurrentRoleName == "Staff";
        public static bool IsDriver => CurrentRoleName == "Driver";
    }

}
