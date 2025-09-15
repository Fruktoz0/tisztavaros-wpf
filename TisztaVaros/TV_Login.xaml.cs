using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;


namespace TisztaVaros
{
    /// <summary>
    /// Interaction logic for TV_Login.xaml
    /// </summary>
    public partial class TV_Login : Window
    {
        ServerConnection connection;
        List<TextBox> tb_list = new List<TextBox>();
        List<Label> l_list = new List<Label>();
        bool hide_y = true;
        string a_psw = "", t_psw;
        TV_Admin aWindow;
        public static double a_top, a_left;

        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
        const uint MF_GRAYED = 0x00000001;
        const uint MF_ENABLED = 0x00000000;
        const uint SC_CLOSE = 0xF060;
        public TV_Login()
        {
            InitializeComponent();
            connection = new ServerConnection();
            LocationChanged += new EventHandler(Win_Mozog);
            Inp_Name.Focus();
            tb_list.Add(Inp_Name);
            tb_list.Add(Inp_PSW);
            l_list.Add(txtEmailPlaceholder);
            l_list.Add(txtPswPlaceholder);
            HTTP_Local.IsChecked = App.local_y;
        }
        private async void Server_Login(object sender, RoutedEventArgs e)
        {
            string a_emil = Inp_Name.Text;
            if (a_emil == "" && a_psw == "")
            {
                a_emil = "admin@admin.hu";
                if (App.local_y)
                {
                    a_psw = "admin";
                    a_psw = "admin123";
                }
                else { a_psw = "admin"; }
            }
            if (a_emil == "" && a_psw == "")
            {
                Inp_Name.Focus();
            }
            else
            {
                TV_User loggedUser = await connection.LoginUser(a_emil, a_psw);
                if (loggedUser.role == "admin")
                {
                    //MessageBox.Show("Welcome, " + loggedUser.role + "!", "User Login:");
                    aWindow = new TV_Admin();
                    var myLoc = this.PointToScreen(new Point(0, 0));
                    aWindow.Closed += Bezarka;

                    var hwnd = new WindowInteropHelper(this).Handle;
                    IntPtr hMenu = GetSystemMenu(hwnd, false);
                    EnableMenuItem(hMenu, SC_CLOSE, MF_GRAYED);
                    a_top = this.Top;
                    a_left = this.Left;

                    double BarWidth = SystemParameters.VirtualScreenWidth - SystemParameters.WorkArea.Width;
                    double BarHeight = SystemParameters.VirtualScreenHeight - SystemParameters.WorkArea.Height;
                    aWindow.Show();
                    if (this.Left < 0)
                    {
                        aWindow.Left = -(BarWidth + aWindow.ActualWidth)/2;
                    }
                    else
                    {
                        aWindow.Left = (SystemParameters.VirtualScreenWidth - aWindow.ActualWidth - BarWidth) / 2;
                    }
                    aWindow.Top = (SystemParameters.VirtualScreenHeight - aWindow.ActualHeight - BarHeight) / 2;

                    this.Hide();
                }
                else if (loggedUser.role == "inspector")
                {
                    MessageBox.Show("Welcome, " + loggedUser.role + "!", "User Login:");
                }
                else
                {
                    if (ServerConnection.login_ok)
                    {
                        MessageBox.Show("Nincs megfelelő jogosultságod ide belépni!", "User Login:");
                    }
                    else
                    {
                        MessageBox.Show(loggedUser.message, "User Login:");
                    }
                }
                a_psw = "";
            }
        }
        private void T_Changed(object sender, RoutedEventArgs e)
        {
            TextBox a_TB = sender as TextBox;
            int a = tb_list.IndexOf(a_TB);
            var margin = l_list[a].Margin;
            if (a_TB.Text == "")
            {
                margin.Left = 40;
                margin.Top = 22;
            }
            else
            {
                margin.Left = 22;
                margin.Top = 1;
                t_psw = Inp_PSW.Text;
                if (hide_y)
                {
                    if (t_psw.Length > a_psw.Length)
                    {
                        string n_txt = t_psw.Replace("*", "");
                        a_psw = a_psw + n_txt;
                    }
                    else if (t_psw != "")
                    {
                        a_psw = a_psw.Substring(0, t_psw.Length);
                    }
                    if (a_psw != "")
                    {
                        Inp_PSW.Text = "*********************".Substring(0, a_psw.Length);
                    }
                    Inp_PSW.Select(a_psw.Length, 0);
                }
                else
                {
                    a_psw = t_psw;
                    Inp_PSW.Text = a_psw;
                }
                ;
            }
            l_list[a].Margin = margin;
        }
        private void PswOnOff(object sender, RoutedEventArgs e)
        {
            hide_y = !hide_y;
            if (hide_y)
            {
                Inp_PSW.Text = "*********************".Substring(0, a_psw.Length);
            }
            else
            {
                Inp_PSW.Text = a_psw;
            }
        }
        private void HTTP_Local_Click(object sender, RoutedEventArgs e)
        {
            App.local_y = (bool)HTTP_Local.IsChecked;
        }

        private async void Bezarka(object sender, EventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            IntPtr hMenu = GetSystemMenu(hwnd, false);
            EnableMenuItem(hMenu, SC_CLOSE, MF_ENABLED);
            this.Top = a_top;
            this.Left = a_left;
            HTTP_Local.IsChecked = App.local_y;
            this.Show();
        }
        private void Win_Mozog(object sender, EventArgs e)
        {
            MainWindow.a_top = this.Top;
            MainWindow.a_left = this.Left;
            //this.Title = "Tiszta Város - Login (Top: " + this.Top + " Left: " + this.Left + ")";
        }
    }
}


