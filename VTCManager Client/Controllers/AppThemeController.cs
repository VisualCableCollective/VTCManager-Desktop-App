using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VTCManager_Client.Controllers
{
    public enum Theme
    {
        Autumn,
        Default
    }

    public class AppThemeController
    {
        public static readonly Theme CurrentTheme = Theme.Autumn;

        public static string GetDashboardBackground()
        {
            switch (CurrentTheme)
            {
                case Theme.Default:
                    return "pack://application:,,,/Resources/Images/UI/Backgrounds/Default.jpg";

                case Theme.Autumn:
                    return "pack://application:,,,/Resources/Images/UI/Backgrounds/autumn.jpg";

                default:
                    return "pack://application:,,,/Resources/Images/UI/Backgrounds/Default.jpg";
            }
        }
    }
}
