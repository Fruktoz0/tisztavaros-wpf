using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace TisztaVaros
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ServerConnection connection;
        TV_Registration rWindow;
        TV_Login lWindow;

        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
        const uint MF_GRAYED = 0x00000001;
        const uint MF_ENABLED = 0x00000000;
        const uint SC_CLOSE = 0xF060;

        public static int g_selected = -1;
        public static bool do_y;
        public static double a_top, a_left;

        public MainWindow()
        {
            InitializeComponent();
            connection = new ServerConnection();
            Window_Login_Auto();
        }

        private void Window_Login(object sender, RoutedEventArgs e)
        {
            Window_Login_Auto();
        }
        private void Window_Login_Auto()
        {
            lWindow = new TV_Login();
            //var myLoc = this.PointToScreen(new Point(0, 0));
            lWindow.Closed += Bezarka;

            var hwnd = new WindowInteropHelper(this).Handle;
            IntPtr hMenu = GetSystemMenu(hwnd, false);
            EnableMenuItem(hMenu, SC_CLOSE, MF_GRAYED);

            lWindow.Top = this.Top;
            lWindow.Left = this.Left;
            lWindow.Show();
            this.Hide();
        }

        private void Window_Registration(object sender, RoutedEventArgs e)
        {
            rWindow = new TV_Registration();
            var myLoc = this.PointToScreen(new Point(0, 0));
            rWindow.Closed += Bezarka;

            var hwnd = new WindowInteropHelper(this).Handle;
            IntPtr hMenu = GetSystemMenu(hwnd, false);
            EnableMenuItem(hMenu, SC_CLOSE, MF_GRAYED);

            rWindow.Top = this.Top;
            rWindow.Left = this.Left;
            rWindow.Show();
            this.Hide();
        }
        private void Bezarka(object sender, EventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            IntPtr hMenu = GetSystemMenu(hwnd, false);
            EnableMenuItem(hMenu, SC_CLOSE, MF_ENABLED);
            this.Top = a_top;
            this.Left = a_left;
            this.Show();
        }
    }
}