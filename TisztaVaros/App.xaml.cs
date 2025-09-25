using System.Configuration;
using System.Data;
using System.Runtime.InteropServices;
using System.Windows;

namespace TisztaVaros
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool vanNet_y = IsInternetAvailable();
        public static bool local_y = !vanNet_y;
        public static string postit = "";

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int description, int reservedValue);

        public static bool IsInternetAvailable()
        {
            int description;
            return InternetGetConnectedState(out description, 0);
        }
    }
}
