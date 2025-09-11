using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Printing;
using System.Reflection;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace TisztaVaros
{
    /// <summary>
    /// Interaction logic for TV_Admin.xaml
    /// </summary>
    public partial class TV_Admin : Window
    {
        ServerConnection connection;
        string colorUnSelectButton = "#6FB1A5";
        string colorSelectButton = "#FF1298BB";
        string bus_Yellow = "#FFFFD700";
        string bus_Akt = "#F80";
        List<Button> b_all = new List<Button>();
        List<StackPanel> sp_h_all = new List<StackPanel>();
        List<StackPanel> sp_m_all = new List<StackPanel>();
        List<TV_User> list_user = new List<TV_User>();
        List<TV_User> list_workers = new List<TV_User>();
        List<TV_Inst> list_inst = new List<TV_Inst>();
        bool[] order_user_y = new bool[10] { true, true, true, true, true, true, true, true, true, true };
        bool[] order_inst_y = new bool[5] { true, true, true, true, true };
        List<string> inst_Names = new List<string>();
        List<string> allRoles = new List<string>() { "user", "institution", "inspector", "admin" };
        List<string> allStatus = new List<string>() { "active", "inactive", "archived" };
        TV_User sel_user;
        TV_Inst sel_inst;
        public static string new_user_psw = "";
        bool chk_udata_y = false, chk_idata_y = false;
        string c_gray = "#FFDDDDDD";
        bool hold_workers = false;

        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
        const uint MF_GRAYED = 0x00000001;
        const uint MF_ENABLED = 0x00000000;
        const uint SC_CLOSE = 0xF060;
        public TV_Admin()
        {
            InitializeComponent();
            connection = new ServerConnection();
            LocationChanged += new EventHandler(Win_Mozog);
            b_all = [B_Users, B_Cats, B_Institutions, B_Reports, B_Challenges];
            sp_h_all = [Header_Ini, Header_Users, Header_Cats, Header_Institutions, Header_Reports, Header_Challenges];
            sp_m_all = [Main_Ini, Main_Users, Main_Cats, Main_Institutions, Main_Reports, Main_Challenges];
            Stack_Main_Ini();
            Get_Inst_List();
            HTTP_Local.IsChecked = App.local_y;
            CB_U_User_Role.ItemsSource = allRoles;
            CB_U_User_Status.ItemsSource = allStatus;
        }
        private void Logo_Click(object sender, EventArgs e)
        {
            Stack_Main_Ini();
        }

        private void Stack_Main_Ini()
        {
            ReColor_All("Ini");
            Get_All_db();
        }

        private async void Get_All_db()
        {
            int user_db = await connection.Server_Get_db("/api/admin/user_db");
            User_Number_N.Content = user_db.ToString();
            int report_db = await connection.Server_Get_db("/api/reports/report_db");
            Report_Number_N.Content = report_db.ToString();
            int vote_db = await connection.Server_Get_db("/api/votes/vote_db");
            Vote_Number_N.Content = vote_db.ToString();
        }

        private void Get_Admin_Users(object sender, RoutedEventArgs e)
        {
            Button a_Button = sender as Button;
            ReColorButtons(a_Button);
        }

        private void Get_Admin_Categories(object sender, RoutedEventArgs e)
        {
            Button a_Button = sender as Button;
            ReColorButtons(a_Button);
            //OldalSav.Visibility = Visibility.Visible;
        }

        void ReColorButtons(Button a_Button)
        {
            string a_sel = a_Button.Name.Split('_')[1];
            ReColor_All(a_sel);
            a_Button.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorSelectButton));
        }
        void ReColor_All(string a_sel)
        {
            foreach (Button b in b_all)
            {
                b.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorUnSelectButton));
            }
            foreach (StackPanel sp in sp_h_all)
            {
                sp.Visibility = Visibility.Hidden;
                if (sp.Name.Split('_')[1] == a_sel)
                {
                    sp.Visibility = Visibility.Visible;
                }
            }
            foreach (StackPanel sp in sp_m_all)
            {
                sp.Visibility = Visibility.Hidden;
                if (sp.Name.Split('_')[1] == a_sel)
                {
                    sp.Visibility = Visibility.Visible;
                }
            }
        }
        private void HTTP_Local_Click(object sender, RoutedEventArgs e)
        {
            App.local_y = (bool)HTTP_Local.IsChecked;
        }

        private void Get_Admin_Reports(object sender, RoutedEventArgs e)
        {
            Button a_Button = sender as Button;
            ReColorButtons(a_Button);
        }

        private void Get_Admin_Institutions(object sender, RoutedEventArgs e)
        {
            Button a_Button = sender as Button;
            ReColorButtons(a_Button);
        }

        private void Get_Admin_Challenges(object sender, RoutedEventArgs e)
        {
            Button a_Button = sender as Button;
            ReColorButtons(a_Button);
        }
        private void User_Search(object sender, RoutedEventArgs e)
        {
            Get_User_List(S_User_Name.Text, S_User_Email.Text);
        }

        private async void Get_User_List(string s_name, string s_email)
        {
            list_user = await connection.Search_User(s_name, s_email);
            TV_Inst a_found;
            foreach (TV_User item in list_user)
            {
                item.createdAt = item.createdAt.Substring(0, 10);
                a_found = list_inst.Find(i => i.id == item.institutionId);
                if (a_found != null)
                {
                    item.institution = a_found.name;
                }
                else
                {
                    item.institution = "";
                }
            }
            ListView_User.ItemsSource = list_user.OrderBy(u => u.username).ToList();
        }
        private async void Get_Inst_List()
        {
            list_inst = await connection.Get_Institutions();
            inst_Names = new List<string>() { "" };
            foreach (TV_Inst item in list_inst)
            {
                item.createdAt = item.createdAt.Substring(0, 10);
                inst_Names.Add(item.name);
            }
            ListView_Inst.ItemsSource = list_inst.OrderBy(u => u.name).ToList();
            inst_Names = inst_Names.OrderBy(n => n).ToList();
            CB_U_User_Inst.ItemsSource = inst_Names;
        }

        private void User_SortHeaderClick(object sender, RoutedEventArgs e)
        {
            string[] haedText = ["Név", "Email", "Státusz", "Pontok", "Szerepkör", "Regisztráció", "Zip", "City", "Intézmény"];
            string[] propText = ["username", "email", "isActive", "points", "role", "createdAt", "zipCode", "city", "institution"];

            string a_head = ((GridViewColumnHeader)e.OriginalSource).Column.Header.ToString();
            for (int i = 0; i < haedText.Length; i++)
            {
                if (a_head == haedText[i])
                {
                    switch (a_head)
                    {
                        case "Intézmény":
                            if (order_user_y[i])
                            {
                                ListView_User.ItemsSource = list_user.OrderBy(u => u.institution).ToList();
                            }
                            else
                            {
                                ListView_User.ItemsSource = list_user.OrderByDescending(u => u.institution).ToList();
                            }
                            break;
                        case "City":
                            if (order_user_y[i])
                            {
                                ListView_User.ItemsSource = list_user.OrderBy(u => u.city).ToList();
                            }
                            else
                            {
                                ListView_User.ItemsSource = list_user.OrderByDescending(u => u.city).ToList();
                            }
                            break;
                        case "Zip":
                            if (order_user_y[i])
                            {
                                ListView_User.ItemsSource = list_user.OrderBy(u => u.zipCode).ToList();
                            }
                            else
                            {
                                ListView_User.ItemsSource = list_user.OrderByDescending(u => u.zipCode).ToList();
                            }
                            break;
                        case "Regisztráció":
                            if (order_user_y[i])
                            {
                                ListView_User.ItemsSource = list_user.OrderBy(u => u.createdAt).ToList();
                            }
                            else
                            {
                                ListView_User.ItemsSource = list_user.OrderByDescending(u => u.createdAt).ToList();
                            }
                            break;
                        case "Szerepkör":
                            if (order_user_y[i])
                            {
                                ListView_User.ItemsSource = list_user.OrderBy(u => u.role).ToList();
                            }
                            else
                            {
                                ListView_User.ItemsSource = list_user.OrderByDescending(u => u.role).ToList();
                            }
                            break;
                        case "Pontok":
                            if (order_user_y[i])
                            {
                                ListView_User.ItemsSource = list_user.OrderBy(u => u.points).ToList();
                            }
                            else
                            {
                                ListView_User.ItemsSource = list_user.OrderByDescending(u => u.points).ToList();
                            }
                            break;
                        case "Státusz":
                            if (order_user_y[i])
                            {
                                ListView_User.ItemsSource = list_user.OrderBy(u => u.isActive).ToList();
                            }
                            else
                            {
                                ListView_User.ItemsSource = list_user.OrderByDescending(u => u.isActive).ToList();
                            }
                            break;
                        case "Email":
                            if (order_user_y[i])
                            {
                                ListView_User.ItemsSource = list_user.OrderBy(u => u.email).ToList();
                            }
                            else
                            {
                                ListView_User.ItemsSource = list_user.OrderByDescending(u => u.email).ToList();
                            }
                            break;
                        case "Név":
                            if (order_user_y[i])
                            {
                                ListView_User.ItemsSource = list_user.OrderBy(u => u.username).ToList();
                            }
                            else
                            {
                                ListView_User.ItemsSource = list_user.OrderByDescending(u => u.username).ToList();
                            }
                            break;
                        default:
                            break;
                    }
                    order_user_y[i] = !order_user_y[i];
                }
            }
            return;
        }
        private void Inst_SortHeaderClick(object sender, RoutedEventArgs e)
        {
            string[] haedText = ["Intézmény Neve", "Email", "Leírás", "Reg. Dátum", "Elérhetőségek"];
            string[] propText = ["name", "email", "description", "createdAt", "contactInfo"];

            string a_head = ((GridViewColumnHeader)e.OriginalSource).Column.Header.ToString();
            for (int i = 0; i < haedText.Length; i++)
            {
                if (a_head == haedText[i])
                {
                    switch (a_head)
                    {
                        case "Intézmény Neve":
                            if (order_inst_y[i])
                            {
                                ListView_Inst.ItemsSource = list_inst.OrderBy(u => u.name).ToList();
                            }
                            else
                            {
                                ListView_Inst.ItemsSource = list_inst.OrderByDescending(u => u.name).ToList();
                            }
                            break;
                        case "Email":
                            if (order_inst_y[i])
                            {
                                ListView_Inst.ItemsSource = list_inst.OrderBy(u => u.email).ToList();
                            }
                            else
                            {
                                ListView_Inst.ItemsSource = list_inst.OrderByDescending(u => u.email).ToList();
                            }
                            break;
                        case "Leírás":
                            if (order_inst_y[i])
                            {
                                ListView_Inst.ItemsSource = list_inst.OrderBy(u => u.description).ToList();
                            }
                            else
                            {
                                ListView_Inst.ItemsSource = list_inst.OrderByDescending(u => u.description).ToList();
                            }
                            break;
                        case "Reg. Dátum":
                            if (order_inst_y[i])
                            {
                                ListView_Inst.ItemsSource = list_inst.OrderBy(u => u.createdAt).ToList();
                            }
                            else
                            {
                                ListView_Inst.ItemsSource = list_inst.OrderByDescending(u => u.createdAt).ToList();
                            }
                            break;
                        case "Elérhetőségek":
                            if (order_inst_y[i])
                            {
                                ListView_Inst.ItemsSource = list_inst.OrderBy(u => u.contactInfo).ToList();
                            }
                            else
                            {
                                ListView_Inst.ItemsSource = list_inst.OrderByDescending(u => u.contactInfo).ToList();
                            }
                            break;
                        default:
                            break;
                    }
                    order_inst_y[i] = !order_inst_y[i];
                }
            }
        }

        private void User_Name_Clear(object sender, RoutedEventArgs e)
        {
            S_User_Name.Text = "";
        }
        private void User_Email_Clear(object sender, RoutedEventArgs e)
        {
            S_User_Email.Text = "";
        }
        private void User_Clear(object sender, RoutedEventArgs e)
        {
            User_ClearData();
        }
        private void User_ClearData()
        {
            chk_udata_y = false;
            U_User_Name.Text = "";
            U_User_Email.Text = "";
            U_User_Zip.Text = "";
            U_User_City.Text = "";
            U_User_Address.Text = "";
            CB_U_User_Status.SelectedItem = "active";
            CB_U_User_Role.SelectedItem = "user";
            CB_U_User_Inst.SelectedItem = "";
            sel_user = new TV_User();
        }
        private async void User_SaveAsNew(object sender, RoutedEventArgs e)
        {
            string e_user = await connection.Check_ExistUser(U_User_Email.Text, U_User_Name.Text);
            if (e_user == "00")
            {
                InputBox psw_input = new InputBox();
                new_user_psw = "x";
                psw_input.Closed += Bezarka;

                var hwnd = new WindowInteropHelper(this).Handle;
                IntPtr hMenu = GetSystemMenu(hwnd, false);
                EnableMenuItem(hMenu, SC_CLOSE, MF_GRAYED);

                this.IsEnabled = false;
                psw_input.Top = this.Top + this.Height / 2 - psw_input.Height / 2;
                psw_input.Left = this.Left + this.Width / 2 - psw_input.Width / 2;
                psw_input.Show();
                while (new_user_psw == "x")
                {
                    await Task.Delay(500);
                }
                if (new_user_psw != "xx" && new_user_psw != "x")
                {
                    App.postit += "PSW for '" + U_User_Email.Text + "': '" + new_user_psw + "'\n";
                    string new_userId = await connection.Admin_AddNewUser(U_User_Email.Text, U_User_Name.Text, new_user_psw);
                    sel_user.id = new_userId;
                    Update_User();
                    User_ClearData();
                    Get_User_List(S_User_Name.Text, S_User_Email.Text);
                }
            }
            else
            {
                if (e_user == "01")
                {
                    MessageBox.Show("Email már létezik!");
                    return;
                }
                if (e_user == "11")
                {
                    MessageBox.Show("Név és Email már létezik!");
                    return;
                }
                if (e_user == "10")
                {
                    MessageBox.Show("Név már létezik!");
                    return;
                }
                MessageBox.Show("Egészen más Hiba!!");
            }
        }
        private async void User_Save(object sender, RoutedEventArgs e)
        {
            bool do_y = await Changed_UserData();
            if (do_y)
            {
                User_Update();
            }
        }
        private async void User_Update()
        {
            sel_user.username = U_User_Name.Text;
            sel_user.email = U_User_Email.Text;
            if (U_User_Zip.Text != "")
            {
                sel_user.zipCode = U_User_Zip.Text;
            }
            else
            {
                sel_user.zipCode = null;
            }
            sel_user.city = U_User_City.Text;
            sel_user.address = U_User_Address.Text;
            sel_user.role = CB_U_User_Role.SelectedItem.ToString();
            sel_user.isActive = CB_U_User_Status.SelectedItem.ToString();
            if (CB_U_User_Inst.SelectedItem.ToString() != "")
            {
                TV_Inst a_found = list_inst.Find(i => i.name == CB_U_User_Inst.SelectedItem.ToString());
                if (a_found != null)
                {
                    sel_user.institutionId = a_found.id;
                }
            }
            else
            {
                sel_user.institutionId = null;
            }
            bool upd_y = await connection.AdminUpdate_User(sel_user);
            if (upd_y)
            {
                int a_index = list_user.FindIndex(u => u.id == sel_user.id);
                if (a_index > -1)
                {
                    Get_User_List(S_User_Name.Text, S_User_Email.Text);
                    User_ClearData();
                    MessageBox.Show("Elvileg Frissítve");
                }
                else
                {
                    MessageBox.Show("Update Hiba!");
                }
            }
            else { MessageBox.Show("Hiba"); }

        }
        private async Task<bool> Changed_UserData()
        {
            bool do_y = false;
            if (chk_udata_y)
            {
                do_y = do_y || U_User_Name.Text != sel_user.username || U_User_Email.Text != sel_user.email || U_User_Zip.Text != sel_user.zipCode
                    || U_User_Zip.Text != sel_user.zipCode || U_User_City.Text != sel_user.city || U_User_Address.Text != sel_user.address
                    || CB_U_User_Role.SelectedItem.ToString() != sel_user.role || CB_U_User_Status.SelectedItem.ToString() != sel_user.isActive
                    || CB_U_User_Inst.SelectedItem.ToString() != sel_user.institution;

                if (U_User_Name.Text != sel_user.username && U_User_Email.Text != sel_user.email)
                {
                    string e_user = await connection.Check_ExistUser(U_User_Email.Text, U_User_Name.Text);
                    if (e_user == "00")
                    {
                        U_User_SaveNew.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF9DD24"));
                    }
                    else
                    {
                        U_User_SaveNew.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(c_gray));
                    }
                }
                else
                {
                    U_User_SaveNew.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(c_gray));
                }
            }
            return do_y;
        }
        private async void Update_User()
        {
            sel_user.username = U_User_Name.Text;
            sel_user.email = U_User_Email.Text;
            if (U_User_Zip.Text != "")
            {
                sel_user.zipCode = U_User_Zip.Text;
            }
            else
            {
                sel_user.zipCode = null;
            }
            sel_user.city = U_User_City.Text;
            sel_user.address = U_User_Address.Text;
            sel_user.role = CB_U_User_Role.SelectedItem.ToString();
            sel_user.isActive = CB_U_User_Status.SelectedItem.ToString();
            if (CB_U_User_Inst.SelectedItem.ToString() != "")
            {
                TV_Inst a_found = list_inst.Find(i => i.name == CB_U_User_Inst.SelectedItem.ToString());
                if (a_found != null)
                {
                    sel_user.institutionId = a_found.id;
                }
            }
            else
            {
                sel_user.institutionId = null;
            }
            bool upd_y = await connection.AdminUpdate_User(sel_user);
            if (upd_y)
            {
                int a_index = list_user.FindIndex(u => u.id == sel_user.id);
                if (a_index > -1)
                {
                    list_user[a_index] = sel_user;
                    ListView_User.Items.Refresh();
                }
            }
            else { MessageBox.Show("Hiba"); }
        }
        private void User_OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (sender.Equals(S_User_Name) || sender.Equals(S_User_Email))
            {
                if (e.Key == Key.Return)
                {
                    Get_User_List(S_User_Name.Text, S_User_Email.Text);
                }
            }
        }
        private async void Bezarka(object sender, EventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            IntPtr hMenu = GetSystemMenu(hwnd, false);
            EnableMenuItem(hMenu, SC_CLOSE, MF_ENABLED);
            this.IsEnabled = true;
        }
        private void Win_Mozog(object sender, EventArgs e)
        {
            this.Title = "Tiszta Város - Admin (Top: " + this.Top + " Left: " + this.Left + ")";
        }
        private void SelUser_DataChanged(object sender, TextChangedEventArgs e)
        {
            SelUser_CHK_Modified();
        }
        private async void SelUser_CHK_Modified()
        {
            bool do_y = await Changed_UserData();
            if (do_y)
            {
                U_User_Save.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF6EF525"));
            }
            else
            {
                U_User_Save.Background = U_User_Search.Background;
            }
        }
        private void SelUser_CBChanged(object sender, SelectionChangedEventArgs e)
        {
            SelUser_CHK_Modified();
        }

        private void Admin_Postit(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(App.postit, "Admin Post-it:");
        }
        private async void Hold_WorkerList(object sender, RoutedEventArgs e)
        {
            if (ListView_Inst.SelectedItem != null)
            {
                hold_workers = !hold_workers;
                if (hold_workers)
                {
                    B_Inst_Workers.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bus_Yellow));
                    TV_Inst sel_inst = ListView_Inst.SelectedItem as TV_Inst;
                    B_Inst_Workers.Content = sel_inst.name;
                }
                else
                {
                    B_Inst_Workers.Content = "Intézmény Munkatársai:";
                    B_Inst_Workers.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(c_gray));
                    Get_WorkerList(sender, e);
                }
            }
        }
        private async void Get_WorkerList(object sender, RoutedEventArgs e)
        {
            if (!hold_workers)
            {
                TV_Inst sel_instw = ListView_Inst.SelectedItem as TV_Inst;
                if (sel_instw != null) {
                    list_workers = await connection.Get_Workers(sel_instw.id);
                    ListView_Workers.ItemsSource = list_workers.OrderBy(u => u.username).ToList();
                }
            }
        }
        private void Inst_ListView_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                chk_idata_y = false;
                sel_inst = item as TV_Inst;
                U_Inst_Name.Text = sel_inst.name;
                U_Inst_Email.Text = sel_inst.email;
                U_Inst_Desc.Text = sel_inst.description;
                string addr = sel_inst.contactInfo;
                if (addr == null)
                {
                    U_Inst_Zip.Text = "";
                    U_Inst_City.Text = "";
                    U_Inst_Address.Text = "";
                }
                else
                {
                    U_Inst_Zip.Text = addr.Split(' ')[1].Trim();
                    U_Inst_City.Text = addr.Split(',')[0].Split(' ')[2].Trim();
                    U_Inst_Address.Text = addr.Split('|')[0].Split(',')[1].Trim();
                    U_Inst_Tel.Text = addr.Split('|')[1].Trim();
                }
                chk_idata_y = true;
            }
        }
        private void Workers_ListView_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {

            }
        }
        private void User_ListView_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                chk_udata_y = false;
                sel_user = item as TV_User;
                U_User_Name.Text = sel_user.username;
                U_User_Email.Text = sel_user.email;
                U_User_Zip.Text = sel_user.zipCode;
                U_User_City.Text = sel_user.city;
                U_User_Address.Text = sel_user.address;
                if (sel_user.institutionId != null)
                {
                    CB_U_User_Inst.SelectedItem = sel_user.institution;
                }
                else
                {
                    CB_U_User_Inst.SelectedItem = "";
                }
                CB_U_User_Status.SelectedItem = sel_user.isActive;
                CB_U_User_Role.SelectedItem = sel_user.role;
                chk_udata_y = true;
                U_User_Save.Background = U_User_Search.Background;
            }
        }
        private void SelInst_DataChanged(object sender, TextChangedEventArgs e)
        {
            SelInst_CHK_Modified();
        }
        private async void SelInst_CHK_Modified()
        {
            bool do_y = await Changed_InstData();
            if (do_y)
            {
                U_Inst_Save.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF6EF525"));
            }
            else
            {
                U_Inst_Save.Background = U_User_Search.Background;
            }
        }
        private async Task<bool> Changed_InstData()
        {
            //Változott e valami Hatósági adatokban??
            bool do_y = false;
            if (chk_idata_y)
            {
                string full_addr = "Cím: " + U_Inst_Zip.Text + " " + U_Inst_City.Text + ", " + U_Inst_Address.Text;
                if (U_Inst_Tel.Text != "")
                {
                    full_addr += " | " + U_Inst_Tel.Text;
                }
                do_y = do_y || U_Inst_Name.Text != sel_inst.name || U_Inst_Email.Text != sel_inst.email
                    || U_Inst_Desc.Text != sel_inst.description || full_addr != sel_inst.contactInfo;
                bool chk_y = true;
                if (sel_inst != null)
                {
                    chk_y = true;
                }
                else
                {
                    chk_y = U_Inst_Name.Text != sel_inst.name && U_Inst_Email.Text != sel_inst.email;
                }
                if (chk_y)
                {
                    string e_inst = await connection.Check_ExistInst(U_Inst_Email.Text, U_Inst_Name.Text);
                    if (e_inst == "00")
                    {
                        U_Inst_SaveNew.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF9DD24"));
                    }
                    else
                    {
                        U_Inst_SaveNew.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(c_gray));
                    }
                }
                else
                {
                    U_Inst_SaveNew.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(c_gray));
                }
            }
            return do_y;
        }
    }
}
