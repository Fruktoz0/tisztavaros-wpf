using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Linq;
using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Printing;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Intrinsics.X86;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Xml.Linq;
using TisztaVaros.Class_s;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

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
        List<TV_Cats> list_cats = new List<TV_Cats>();
        List<TV_Report> list_reports = new List<TV_Report>();
        List<TV_Challenges> list_challanges = new List<TV_Challenges>();
        List<TV_ForwardingLogs> list_ForwardingLogs = new List<TV_ForwardingLogs>();
        List<TV_StatusHistory> list_StatusHistory = new List<TV_StatusHistory>();
        List<TV_StatusHistory> list_HistoryAll = new List<TV_StatusHistory>();
        bool[] order_user_y = new bool[10] { true, true, true, true, true, true, true, true, true, true };
        bool[] order_reps_y = new bool[10] { true, true, true, true, true, true, true, true, true, true };
        bool[] order_challs_y = new bool[10] { true, true, true, true, true, true, true, true, true, true };
        bool[] order_inst_y = new bool[5] { true, true, true, true, true };
        bool[] order_cats_y = new bool[5] { true, true, true, true, true };
        List<string> inst_Names = new List<string>();
        List<string> allRoles = new List<string>() { "user", "institution", "inspector", "admin" };
        List<string> allStatus = new List<string>() { "active", "inactive", "archived" };
        TV_User sel_user;
        TV_Inst sel_inst, sel_instw;
        TV_Cats sel_cat;
        TV_Challenges sel_challenge, sel_challenge_last;
        static TV_Report sel_report, sel_report_last;
        public static string new_vmi_psw = "";
        public static List<string> a_img_urls = new();
        bool chk_udata_y = true, chk_idata_y = false, chk_cdata_y = false;
        bool start_inst_y = true, start_cats_y = true, start_report_y = true, start_challenge_y = true;
        bool already_exist = false, hw_now_y = false;
        string bc_Gray = "#FFDDDDDD", bc_LightGreen = "#6FB1A5", bc_Green = "#FF6EF525", tb_NotEmpty = "#FF4EFFD2", bc_Yellow = "#FFF9DD24";
        string tb_LightRed = "#FFFFACAC";
        Border cb_defBorder;
        public static int view_rep_pict, max_rep_pict, sel_rep_idx;
        public static string rep_pict_url = "https://tisztavaros.hu";
        double[] pict_arany = new double[5];
        string[] last_n_days = new string[] { "1 nap", "3 nap", "1 hét", "2 hét", "1 hónap", "1 év", "Összes" };
        int[] last_ndays_n = new int[] { -1, -3, -7, -14, -30, -365, -20000 };
        string a_sel_menu = "";

        bool hold_workers = false;
        public static string inst_logo_url = "";
        TV_Logo_Input logoURLWindow;

        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
        const uint MF_GRAYED = 0x00000001;
        const uint MF_ENABLED = 0x00000000;
        const uint SC_CLOSE = 0xF060;
        BitmapImage empty_logo = new BitmapImage(new Uri("https://smd.hu/Team/Empty_Logo.gif"));
        public static BitmapImage rep_Picture;

        string[] db_status = new string[7] { "open", "accepted", "forwarded", "in_progress", "rejected", "resolved", "reopened" };
        string[] db_statusH = new string[7] { "Megnyitva", "Befogadva", "Továbbítva", "Folyamatban", "Elutasítva", "Megoldva", "Újranyitva" };

        string e_user, psw_input_using = "";
        bool u_save_y, u_savenew_y, u_fromlist_y, c_save_y, c_savenew_y;
        internal static string pict_path;
        bool langEng_y = true;

        public TV_Admin()
        {
            InitializeComponent();

            //pict_path = Directory.GetCurrentDirectory().Substring(0, 3) + "TV_Picts";
            pict_path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\TV_Picts";
            Directory.CreateDirectory(pict_path + "\\Logo");
            Directory.CreateDirectory(pict_path + "\\uploads");
            Directory.CreateDirectory(pict_path + "\\uploads\\challenges");
            rep_pict_url = Get_Pict_URL();
            if (App.local_y) { Admin_Logo.Source = new BitmapImage(new Uri(pict_path + "\\tisztavaros_logo_Middle_NoText.png")); }

            Panel.SetZIndex(Report_Detail, 0);
            connection = new ServerConnection();
            LocationChanged += new EventHandler(Win_Mozog);
            b_all = [B_Users, B_Categories, B_Institutions, B_Reports, B_Challenges, B_New_PSW];
            sp_h_all = [Header_Ini, Header_Users, Header_Categories, Header_Institutions, Header_Reports, Header_Challenges];
            sp_m_all = [Main_Ini, Main_Users, Main_Categories, Main_Institutions, Main_Reports, Main_Challenges];
            Stack_Main_Ini();
            Get_Inst_List();
            Get_Categories_List();
            HTTP_Local.IsChecked = App.local_y;
            CB_H_User_Role.ItemsSource = allRoles;
            CB_H_User_Status.ItemsSource = allStatus;

            datePicker_From.SelectedDate = DateTime.Now;
            datePicker_Until.SelectedDate = DateTime.Now.AddDays(-14);
            CB_H_Report_DLong.ItemsSource = last_n_days;
            CB_H_Report_DLong.SelectedValue = "2 hét";

            datePickerC_From.SelectedDate = DateTime.Now;
            datePickerC_Until.SelectedDate = DateTime.Now.AddDays(-14);
            CB_H_Challenge_DLong.ItemsSource = last_n_days;
            CB_H_Challenge_DLong.SelectedValue = "2 hét";
        }
        private void TV_Admin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (e.Key == Key.Q && Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.LeftShift))
                {
                    langEng_y = !langEng_y;
                    if (langEng_y) { ToLangENG(); }
                    else { ToLangHUN(); }
                }
                else if (e.Key == Key.Q && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    if (A_Err.Visibility == Visibility.Hidden) { A_Err.Visibility = Visibility.Visible; Q_Pict.Visibility = Visibility.Visible; }
                    else { A_Err.Visibility = Visibility.Hidden; Q_Pict.Visibility = Visibility.Hidden; }
                }
            }
        }
        private void ToLangENG()
        {
            Q_Pict.Source = new BitmapImage(new Uri("https://www.smd.hu/Team/Q.gif"));
            S_User_Name_L.Content = "User Name:";
            S_User_Name_L.Width = 120;
            Thickness margin = S_User_Name.Margin; margin.Left = 5; S_User_Name.Margin = margin;
            S_User_Email_L.Width = 100;
            margin = S_User_Email.Margin; margin.Left = 5; S_User_Email.Margin = margin;

            H_User_Name_L.Content = "User Name::";
            H_User_Name_L.Width = 110;
            margin = H_User_Name_L.Margin; margin.Left = 5; H_User_Name_L.Margin = margin;
            margin = H_User_Name.Margin; margin.Left = 120; H_User_Name.Margin = margin;

            H_User_Zip_L.Content = "Zip:";
            H_User_City_L.Content = "City:";
            margin = H_User_City_L.Margin; margin.Left = 140; margin.Right = -76; H_User_City_L.Margin = margin;
            margin = H_User_City.Margin; margin.Left = 201; margin.Right = -245; H_User_City.Margin = margin;
            H_User_Address_L.Content = "Address:";

            H_User_Search.Content = "Search";
            H_User_Delete.Content = "Delete";
            H_User_SaveNew.Content = "Save as New";
            H_User_Save.Content = "Save";

            H_Reports_ReLoad.Content = "ReLoad";

            H_Categories_ReLoad.Content = "ReLoad";
            H_Category_Delete.Content = "Dalete";
            H_Category_SaveNew.Content = "Save as New";
            H_Category_Save.Content = "Save";

            H_Inst_ReLoad.Content = "Reload";
            H_Inst_Delete.Content = "Delete";
            H_Inst_SaveNew.Content = "Save as New";
            H_Inst_Save.Content = "Save";

            H_Challenges_ReLoad.Content = "Betölt";
        }
        private void ToLangHUN()
        {
            Q_Pict.Source = new BitmapImage(new Uri("https://www.smd.hu/Team/Q2.gif"));
            S_User_Name_L.Content = "Név:";
            S_User_Name_L.Width = 80;
            Thickness margin = S_User_Name.Margin; margin.Left = -35; S_User_Name.Margin = margin;
            S_User_Email_L.Width = 80;
            margin = S_User_Email.Margin; margin.Left = -35; S_User_Email.Margin = margin;

            H_User_Name_L.Content = "Név:";
            H_User_Name_L.Width = 55;
            margin = H_User_Name_L.Margin; margin.Left = 0; H_User_Name_L.Margin = margin;
            margin = H_User_Name.Margin; margin.Left = 60; H_User_Name.Margin = margin;

            H_User_Zip_L.Content = "Irsz:";
            H_User_City_L.Content = "Város:";
            margin = H_User_City_L.Margin; margin.Left = 140; margin.Right = -86; H_User_City_L.Margin = margin;
            margin = H_User_City.Margin; margin.Left = 210; margin.Right = -277; H_User_City.Margin = margin;
            H_User_Address_L.Content = "Cím:";

            H_User_Search.Content = "Keres";
            H_User_Delete.Content = "Törlés";
            H_User_SaveNew.Content = "Új Felhasználó";
            H_User_Save.Content = "Mentés";

            H_Reports_ReLoad.Content = "Betölt";

            H_Categories_ReLoad.Content = "Betölt";
            H_Category_Delete.Content = "Törlés";
            H_Category_SaveNew.Content = "Új ketegória";
            H_Category_Save.Content = "Mentés";

            H_Inst_ReLoad.Content = "Betölt";
            H_Inst_Delete.Content = "Törlés";
            H_Inst_SaveNew.Content = "Új Intézmény";
            H_Inst_Save.Content = "Mentés";

            H_Challenges_ReLoad.Content = "Betölt";
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
            B_New_PSW.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFAC6AE6"));
        }

        void ReColorButtons(Button a_Button)
        {
            a_sel_menu = a_Button.Name.Split('_')[1];
            ReColor_All(a_sel_menu);
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
            if (start_report_y)
            {
                Get_Report_List();
            }
            else
            {
                ListView_Reports.Focus();
            }
        }
        private void Get_Admin_Challenges(object sender, RoutedEventArgs e)
        {
            Button a_Button = sender as Button;
            ReColorButtons(a_Button);
            if (start_challenge_y)
            {
                Get_Challenge_List();
                start_challenge_y = false;
            }
            else
            {
                ListView_Challenges.Focus();
            }
        }
        private void Get_Admin_Categories(object sender, RoutedEventArgs e)
        {
            Button a_Button = sender as Button;
            ReColorButtons(a_Button);
            ListView_Categories.Focus();
        }
        private void Get_Admin_Institutions(object sender, RoutedEventArgs e)
        {
            Button a_Button = sender as Button;
            ReColorButtons(a_Button);
            if (start_inst_y)
            {
                Inst_ReLoad_Auto();
                start_inst_y = false;
            }
            ListView_Inst.Focus();
        }
        private async void User_Search(object sender, RoutedEventArgs e)
        {
            ListView_User.Visibility = Visibility.Hidden;
            Get_User_List(S_User_Name.Text, S_User_Email.Text);
            await Task.Delay(100);
            ListView_User.Visibility = Visibility.Visible;
        }
        private async void Get_User_List(string s_name, string s_email)
        {
            list_user = await connection.Search_User(s_name, s_email);
            TV_Inst a_found;
            foreach (TV_User item in list_user)
            {
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
            list_user = list_user.OrderBy(u => u.username).ToList();
            ListView_User.ItemsSource = list_user;
        }
        private async void Get_Inst_List()
        {
            list_inst = await connection.Get_Institutions();
            inst_Names = new List<string>() { "" };
            foreach (TV_Inst item in list_inst)
            {
                inst_Names.Add(item.name);
                if (item.logoUrl != "" && item.logoUrl != null) { item.logo = "Logo"; }
            }
            inst_Names.Sort();
            inst_Names = inst_Names.OrderBy(n => n).ToList();
            CB_H_User_Inst.ItemsSource = inst_Names;
            CB_H_Category_Inst.ItemsSource = inst_Names;
            CB_H_Report_Inst.ItemsSource = inst_Names;
            CB_H_Challenge_Inst.ItemsSource = inst_Names;

            ListView_Inst.ItemsSource = list_inst.OrderBy(u => u.name).ToList();
            ListView_Inst.SelectedItem = 0;
            ListView_Inst.Focus();
        }
        private void User_SortHeaderClick(object sender, RoutedEventArgs e)
        {
            string[] headText = ["Név", "Email", "Státusz", "Pontok", "Szerepkör", "Regisztráció", "Zip", "City", "Intézmény"];

            string a_head = ((GridViewColumnHeader)e.OriginalSource).Column.Header.ToString();
            for (int i = 0; i < headText.Length; i++)
            {
                if (a_head == headText[i])
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
            string[] headText = ["Intézmény Neve", "Email", "Leírás", "Reg. Dátum", "Elérhetőségek"];
            string a_head = "";
            try { a_head = ((GridViewColumnHeader)e.OriginalSource).Column.Header.ToString(); }
            catch (Exception ex) { return; }
            for (int i = 0; i < headText.Length; i++)
            {
                if (a_head == headText[i])
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
            u_fromlist_y = false;
            H_User_Name.Text = "";
            H_User_Email.Text = "";
            H_User_Zip.Text = "";
            H_User_City.Text = "";
            H_User_Address.Text = "";

            CB_H_User_Status.SelectedIndex = -1;
            Change_ComboBoxBorderColor(CB_H_User_Status, "");
            CB_H_User_Role.SelectedIndex = -1;
            Change_ComboBoxBorderColor(CB_H_User_Role, "");
            CB_H_User_Inst.SelectedIndex = -1;
            Change_ComboBoxBorderColor(CB_H_User_Inst, "");

            sel_user = new TV_User();
            chk_udata_y = true;
        }
        private async void User_SaveAsNew(object sender, RoutedEventArgs e)
        {
            H_User_Email.Text = H_User_Email.Text.Trim();
            H_User_Name.Text = H_User_Name.Text.Trim();
            if (u_savenew_y)
            {
                bool ok_emal_y = Valid_Email(H_User_Email.Text);
                if (H_User_Name.Text.Length >= 4 && H_User_Email.Text.Length >= 6 && ok_emal_y)
                {
                    e_user = await connection.Check_ExistUser(H_User_Name.Text, H_User_Email.Text);
                    if (e_user == "00")
                    {
                        psw_input_using = "SaveAsNew";
                        Get_New_PSW();
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
                else if (H_User_Name.Text.Length < 4)
                {
                    MessageBox.Show("A név túl rövid!");
                    return;
                }
                else if (H_User_Email.Text.Length < 6 || !ok_emal_y)
                {
                    MessageBox.Show("Az email cím nem megfelelő!");
                    return;
                }
            }
        }
        private void Get_New_PSW()
        {
            TV_PSW_Input psw_input = new TV_PSW_Input();
            new_vmi_psw = "x";
            psw_input.Closed += Bezarka;

            var hwnd = new WindowInteropHelper(this).Handle;
            IntPtr hMenu = GetSystemMenu(hwnd, false);
            EnableMenuItem(hMenu, SC_CLOSE, MF_GRAYED);

            this.IsEnabled = false;
            psw_input.Top = this.Top + this.Height / 2 - psw_input.Height / 2;
            psw_input.Left = this.Left + this.Width / 2 - psw_input.Width / 2;
            psw_input.Show();
        }
        private void User_Save(object sender, RoutedEventArgs e)
        {
            if (u_save_y)
            {
                User_Update();
            }
        }
        private async void Changed_UserData()
        {
            string chk = "---";
            bool do_y = false;
            u_savenew_y = false;
            u_save_y = false;
            if (chk_udata_y)
            {
                chk = "-1-";
                string a_user_name = H_User_Name.Text.Trim();
                string a_user_emil = H_User_Email.Text.Trim();

                if ((a_user_name == "" || a_user_emil == "") && !u_fromlist_y)
                {
                    CB_H_User_Status.SelectedIndex = -1;
                    Change_ComboBoxBorderColor(CB_H_User_Status, "");
                    CB_H_User_Role.SelectedIndex = -1;
                    Change_ComboBoxBorderColor(CB_H_User_Role, "");
                    CB_H_User_Inst.SelectedIndex = -1;
                    Change_ComboBoxBorderColor(CB_H_User_Inst, "");
                    H_User_SaveColor();
                    return;
                }
                if (!u_fromlist_y)
                {
                    chk = "New";
                    CB_H_User_Status.SelectedItem = "active";
                    CB_H_User_Role.SelectedItem = "user";
                    e_user = await connection.Check_ExistUser(a_user_name, a_user_emil);
                    if (e_user == "00")
                    {
                        u_savenew_y = true;
                    }
                    else
                    {
                        if (e_user[0] == '1') { H_User_Name.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tb_LightRed)); }
                        if (e_user[1] == '1') { H_User_Email.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tb_LightRed)); }
                    }
                }
                else
                {
                    e_user = await connection.Check_ExistUser(a_user_name, a_user_emil);
                    do_y = H_User_Zip.Text != NL(sel_user.zipCode) || H_User_Zip.Text != NL(sel_user.zipCode) || H_User_City.Text != NL(sel_user.city)
                        || H_User_Address.Text != NL(sel_user.address) || CB_H_User_Role.SelectedItem.ToString() != sel_user.role
                        || CB_H_User_Status.SelectedItem.ToString() != sel_user.isActive
                        || CB_H_User_Inst.SelectedItem.ToString() != NL(sel_user.institution);
                    if ((a_user_name == "" || a_user_emil == "") && chk_udata_y)
                    {
                        if (a_user_name == "")
                        {
                            H_User_Name.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tb_LightRed));
                        }
                        if (a_user_emil == "")
                        {
                            H_User_Email.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tb_LightRed));
                        }
                    }
                    else if (a_user_name == sel_user.username && a_user_emil == sel_user.email)
                    {
                        chk = "==,==";
                        H_User_Name.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tb_NotEmpty));
                        H_User_Email.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tb_NotEmpty));
                        u_save_y = do_y;
                    }
                    else
                    {
                        if (a_user_name != sel_user.username && a_user_emil == sel_user.email)
                        {
                            chk = "!=,==";
                            if (e_user[0] == '1')
                            {
                                H_User_Name.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tb_LightRed));
                            }
                            else
                            {
                                u_save_y = true;
                            }
                        }
                        else if (a_user_name == sel_user.username && a_user_emil != sel_user.email && e_user[1] == '0')
                        {
                            chk = "==,!=";
                            if (e_user[1] == '1')
                            {
                                H_User_Email.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tb_LightRed));
                            }
                            else { u_save_y = true; }
                        }
                        else //(H_User_Name.Text != sel_user.username && H_User_Email.Text != sel_user.email)
                        {
                            chk = "!=,!=";
                            if (e_user == "00")
                            {
                                u_savenew_y = true;
                                u_save_y = true;
                            }
                            else
                            {
                                u_savenew_y = false;
                                u_save_y = false;
                                if (e_user[0] == '1') { H_User_Name.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tb_LightRed)); }
                                if (e_user[1] == '1') { H_User_Email.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tb_LightRed)); }
                            }
                        }
                    }
                }
            }
            A_Err.Text = "[" + chk + "] AsNew=" + u_savenew_y.ToString() + " | Save=" + u_save_y.ToString()
                + " | Udata=" + chk_udata_y.ToString() + " | 4List=" + u_fromlist_y.ToString();
            H_User_SaveColor();
        }
        private string NL(string inp)
        {
            if (inp == null)
            {
                return "";
            }
            return inp;
        }
        private void H_User_SaveColor()
        {

            if (u_save_y) { H_User_Save.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Green)); }
            else { H_User_Save.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray)); }

            if (u_savenew_y) { H_User_SaveNew.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF9DD24")); }
            else { H_User_SaveNew.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray)); }
        }
        private void Category_Save(object sender, RoutedEventArgs e)
        {
            //SelCategory_CHK_Modified();
            if (c_save_y)
            {
                Category_Update();
            }
        }
        private async void Category_Update()
        {
            string a_inst = CB_H_Category_Inst.SelectedItem.ToString();
            if (a_inst == "")
            {
                MessageBox.Show("Valamilyen intézményt választani Kell!");
                return;
            }
            if (sel_cat != null)
            {
                TV_Inst i_found = list_inst.Find(i => i.name == a_inst);
                string mod_cat_id = await connection.Server_ModifyCategory(sel_cat.id, i_found.id);
                if (mod_cat_id == "22")
                {
                    MessageBox.Show("Nem sikerült az Új kategória hozzáadása!");
                    return;
                }
                Get_Categories_List();
                //Server_ModifywCategory(string name, string def_instId)
            }
        }
        private async void User_Update()
        {
            if (sel_user == null) { sel_user = new TV_User(); }

            sel_user.username = H_User_Name.Text.Trim();
            sel_user.email = H_User_Email.Text.Trim();
            H_User_Zip.Text = H_User_Zip.Text.Trim();

            if (H_User_Zip.Text != "") { sel_user.zipCode = H_User_Zip.Text; }
            else { sel_user.zipCode = null; }

            sel_user.city = H_User_City.Text.Trim();
            sel_user.address = H_User_Address.Text.Trim();
            sel_user.role = CB_H_User_Role.SelectedItem.ToString();
            sel_user.isActive = CB_H_User_Status.SelectedItem.ToString();

            if (CB_H_User_Inst.SelectedIndex > 0)
            {
                TV_Inst a_found = list_inst.Find(i => i.name == CB_H_User_Inst.SelectedItem.ToString());
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
                    MessageBox.Show("Elvileg Frissítve");
                    a_index = list_user.FindIndex(u => u.id == sel_user.id);
                    if (a_index > -1)
                    {
                        ListView_User.SelectedIndex = a_index;
                        User_ClearData();
                    }
                }
                else
                {
                    // MessageBox.Show("Update Hiba!");
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
        private void Win_Mozog(object sender, EventArgs e)
        {
            this.Title = "Tiszta Város - Admin (Top: " + this.Top + " Left: " + this.Left + ")";
        }
        private void SelCatInst_CBChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!hw_now_y)
            {
                ComboBox a_cb = sender as ComboBox;
                if (a_cb.SelectedItem != "")
                {
                    Change_ComboBoxBorderColor(a_cb, tb_NotEmpty);
                    TV_Inst a_found = list_inst.Find(i => i.name == CB_H_Category_Inst.SelectedItem.ToString());
                    H_CategoryInst_Desc.Text = a_found.description;
                    H_CategoryInst_Desc.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tb_NotEmpty));
                    H_CategoryInst_Contact.Text = a_found.contactInfo;
                    H_CategoryInst_Contact.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tb_NotEmpty));
                }
                else
                {
                    Change_ComboBoxBorderColor(a_cb, "");
                    H_CategoryInst_Desc.Text = "";
                    H_CategoryInst_Desc.Background = new SolidColorBrush(Colors.White);
                    H_CategoryInst_Contact.Text = "";
                    H_CategoryInst_Contact.Background = new SolidColorBrush(Colors.White);
                }
                SelCategory_CHK_Modified();
            }
        }
        private void Get_DefCB_Color()
        {
            if (cb_defBorder == null)
            {
                var comboBoxTemplate = CB_H_Default.Template;
                var toggleButton = comboBoxTemplate.FindName("toggleButton", CB_H_Default) as ToggleButton;
                var toggleButtonTemplate = toggleButton.Template;
                cb_defBorder = toggleButtonTemplate.FindName("templateRoot", toggleButton) as Border;
            }
        }
        private void Change_ComboBoxBorderColor(object sender, string newColor)
        {
            var comboBox = sender as ComboBox;
            if (cb_defBorder == null)
            {
                Get_DefCB_Color();
            }
            var comboBoxTemplate = comboBox.Template;
            var toggleButton = comboBoxTemplate.FindName("toggleButton", comboBox) as ToggleButton;
            var toggleButtonTemplate = toggleButton.Template;
            var border = toggleButtonTemplate.FindName("templateRoot", toggleButton) as Border;
            if (newColor == "")
            {
                border.Background = CB_H_Default.Background;
            }
            else
            {
                border.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(newColor));
            }
        }
        private void SelUser_CBChanged(object sender, SelectionChangedEventArgs e)
        {
            Color_CBChanged(sender, e);
            Changed_UserData();
        }
        private void SH_ReportInst_CBChanged(object sender, SelectionChangedEventArgs e)
        {
            Color_CBChanged(sender, e);
            string a_inst = CB_H_Report_Inst.SelectedItem.ToString();
            if (a_inst == "")
            {
                ListView_Reports.ItemsSource = list_reports;
            }
            else
            {
                ListView_Reports.ItemsSource = list_reports.Where(r => r.institution.name == a_inst).ToList();
            }
            if (ListView_Reports.Items.Count == 0) { Panel.SetZIndex(Report_Detail, 0); }
            else { ListView_Reports.SelectedItem = ListView_Reports.Items[0]; Report_ListView_Click_Auto(); }
        }
        private void SH_ChallengeInst_CBChanged(object sender, SelectionChangedEventArgs e)
        {
            Color_CBChanged(sender, e);
            string a_inst = CB_H_Challenge_Inst.SelectedItem.ToString();
            if (a_inst == "")
            {
                ListView_Challenges.ItemsSource = list_challanges;
            }
            else
            {
                ListView_Challenges.ItemsSource = list_challanges.Where(r => r.inst_name == a_inst).ToList();
            }
            if (ListView_Challenges.Items.Count == 0) { Panel.SetZIndex(Report_Detail, 0); }
            else { ListView_Challenges.SelectedItem = ListView_Reports.Items[0]; Report_ListView_Click_Auto(); }
        }
        private void Call_GoogleMaps(object sender, EventArgs s)
        {
            string url = "https://www.google.com/maps/search/?api=1&query=" + Conv_Coords(sel_report.locationLat.ToString()) + "%2C"
                + Conv_Coords(sel_report.locationLng.ToString()) + "&zoom=0"; ;
            // https://www.google.com/maps/search/?api=1&query=47.5951518%2C-122.3316393
            //  47°41'17.5"N+19°10'06.6"E
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });


            string url2 = "https://www.google.com/maps/@?api=1&map_action=map&center=" + Conv_Coords(sel_report.locationLat.ToString()) + "%2C"
                + Conv_Coords(sel_report.locationLat.ToString()) + "&zoom=10&basemap=terrain"; ;
            // https://www.google.com/maps/@?api=1&map_action=map&center=-33.712206%2C150.311941&zoom=12&basemap=terrain
            //Process.Start(new ProcessStartInfo(url2) { UseShellExecute = true });
            //MessageBox.Show(url);
        }
        private string Conv_Coords(string inp)
        {
            string outka = inp.Replace('.', ',');
            string out_int = outka.Split(',')[0] + "°";
            string out_fract = outka.Split(',')[1] + "000000";
            return out_int + out_fract.Substring(0, 2) + "'" + out_fract.Substring(2, 2) + '"' + out_fract.Substring(4, 2) + "." + out_fract.Substring(6, 1); ;
        }
        private void Color_CBChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox a_cb = sender as ComboBox;
            if (a_cb.SelectedItem != "") { Change_ComboBoxBorderColor(a_cb, tb_NotEmpty); }
            else { Change_ComboBoxBorderColor(a_cb, ""); }
        }
        private void Admin_Postit(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(App.postit, "Admin Post-it [Password Reminder]:");
        }
        private async void Hold_WorkerList(object sender, RoutedEventArgs e)
        {
            if (ListView_Inst.SelectedItem != null)
            {
                Freeze_Workerlist();
                Get_WorkerList_Auto();
            }
        }
        private void Freeze_Workerlist()
        {
            hold_workers = !hold_workers;
            if (hold_workers)
            {
                B_Inst_Workers.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(colorSelectButton));
                B_Inst_Workers.Foreground = new SolidColorBrush(Colors.White);
                TV_Inst sel_inst = ListView_Inst.SelectedItem as TV_Inst;
                B_Inst_Workers.Content = sel_inst.name;
            }
            else
            {
                B_Inst_Workers.Content = "Intézmény Munkatársai:";
                B_Inst_Workers.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray));
                B_Inst_Workers.Foreground = new SolidColorBrush(Colors.Black);
            }
        }
        private async void Get_WorkerList_Key(object sender, RoutedEventArgs e)
        {
            sel_instw = ListView_Inst.SelectedItem as TV_Inst;
            Inst_Sel_Stat_Auto();
        }
        private async void Get_WorkerList(object sender, RoutedEventArgs e)
        {
            Get_WorkerList_Auto();
        }
        private async void Get_WorkerList_Auto()
        {
            int a_idx = 0;
            if (start_inst_y)
            {
                sel_instw = ListView_Inst.Items[0] as TV_Inst;
            }
            else
            {
                a_idx = ListView_Inst.SelectedIndex;
                sel_instw = ListView_Inst.SelectedItem as TV_Inst;
            }

            if (!hold_workers)
            {
                if (sel_instw != null)
                {
                    list_workers = await connection.Get_Workers(sel_instw.id, "Get_WorkerList_Auto");
                    Del_Workers.ItemsSource = list_workers.OrderBy(u => u.username).ToList();
                }
            }
            Inst_Sel_Stat_Auto();
            ListView_Inst.Focus();
            ListView_Inst.SelectedIndex = a_idx;
        }
        private void Inst_Sel_Stat(object sender, RoutedEventArgs e)
        {
            Inst_Sel_Stat_Auto();
        }
        private async void Inst_Sel_Stat_Auto()
        {
            string a_id = "";
            string a_logo_Url = "";
            if (sel_instw != null)
            {
                a_id = sel_instw.id;
                a_logo_Url = sel_instw.logoUrl;
            }

            if (a_id != null && a_id != "")
            {
                List<TV_User> sel_workers = await connection.Get_Workers(a_id, "Inst_Sel_Stat_Auto");
                Del_Workers.ItemsSource = sel_workers.OrderBy(u => u.username).ToList();
                int workers_db = sel_workers.Count;
                if (workers_db > 0)
                {
                    B_Inst_Detach.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bus_Yellow));
                }
                else
                {
                    B_Inst_Detach.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray));
                }
                SetColor_Label(workers_db, Inst_WorkersNum);
                int rep_db = await connection.Server_Get_InstValami_db("/api/reports/report_Inst_db", a_id);
                SetColor_Label(rep_db, Inst_ReportNum);
                int news_db = await connection.Server_Get_InstValami_db("/api/news/news_Inst_db", a_id);
                SetColor_Label(news_db, Inst_NewsNum);
                if (a_logo_Url != null && a_logo_Url != "")
                {
                    if (App.vanNet_y)
                    {
                        Inst_LogoImg_Del.Source = new BitmapImage(new Uri(a_logo_Url));
                        DWL_Pict(a_logo_Url);
                    }
                    else { Inst_LogoImg_Del.Source = new BitmapImage(new Uri(pict_path + "\\Logo\\" + a_logo_Url.Split("/").Last())); }
                }
                else { Inst_LogoImg_Del.Source = new BitmapImage(new Uri("http://smd.hu/Team/Empty_Logo.gif")); }
            }
        }
        private async void Report_ListView_Click(object sender, RoutedEventArgs e)
        {
            Report_ListView_Click_Auto();
        }
        private void Challenge_ListView_Click(object sender, SelectionChangedEventArgs e)
        {
            Challenge_ListView_Click_Auto();
        }
        private async void Challenge_ListView_Click(object sender, RoutedEventArgs e)
        {
            Challenge_ListView_Click_Auto();
        }
        private async void Challenge_ListView_Click_Auto()
        {
            sel_challenge = ListView_Challenges.SelectedItem as TV_Challenges;
            if (sel_challenge != null)
            {
                if (sel_challenge != sel_challenge_last || Panel.GetZIndex(Challenge_Detail) == 0)
                {
                    Report_Detail.Visibility = Visibility.Visible;
                    Panel.SetZIndex(Challenge_Detail, 2);
                    ChallengeDetail_Cím.Text = sel_challenge.title;
                    ChallengeDetail_Desc.Text = sel_challenge.description;
                    ChallengeDetail_cosPoints.Content = sel_challenge.costPoints;
                    ChallengeDetail_rewardPoints.Content = sel_challenge.rewardPoints;
                    ChallengeDetail_startDate.Content = sel_challenge.startDateHH;
                    ChallengeDetail_endDate.Content = sel_challenge.endDateHH;
                    ChallengeDetail_Pict.Source = new BitmapImage(new Uri(Corr_URL(rep_pict_url + sel_challenge.image)));
                    DWL_Pict(sel_challenge.image);
                    List<TV_UserChallenges> u_callenges = await connection.Server_Get_UserChallenges(sel_challenge.id);
                    ListView_UserChallenges.ItemsSource = u_callenges;
                }
            }
        }
        private async void Report_ListView_Click_Auto()
        {
            sel_report = ListView_Reports.SelectedItem as TV_Report;
            if (sel_report != null)
            {
                if (sel_report != sel_report_last || Panel.GetZIndex(Report_Detail) == 0)
                {
                    a_img_urls = new List<string>();
                    foreach (var item in sel_report.reportImages)
                    {
                        a_img_urls.Add(item.imageUrl);
                        DWL_Pict(item.imageUrl);
                    }
                    Report_Detail.Visibility = Visibility.Visible;
                    Panel.SetZIndex(Report_Detail, 2);
                    ReportDetail_City.Content = sel_report.city;
                    ReportDetail_Cím.Text = sel_report.title;
                    ReportDetail_Desc.Text = sel_report.description;
                    ReportDetail_User.Content = sel_report.username;
                    ReportDetail_Category.Content = sel_report.category.categoryName;
                    ReportDetail_Address.Content = sel_report.address + " " + sel_report.zipCode;
                    ReportDetail_Coords.Content = sel_report.locationLat + " " + sel_report.locationLng;
                    ReportDetail_Inst.Content = sel_report.institution.name;
                    ReportDetail_Azon.Content = sel_report.createdAt.Substring(0, 4) + "/" + sel_report.id;
                    if (sel_report.confirmed == null)
                    {
                        ReportDetail_Confirmed.Content = "0x";
                    }
                    else
                    {
                        ReportDetail_Confirmed.Content = sel_report.confirmed + "x";
                    }
                    max_rep_pict = sel_report.reportImages.Count - 1;
                    if (sel_report.reportImages.Count <= 1)
                    {
                        B_Prev_Pict.Visibility = Visibility.Hidden;
                        B_Next_Pict.Visibility = Visibility.Hidden;
                    }
                    else
                    {
                        B_Prev_Pict.Visibility = Visibility.Hidden;
                        B_Next_Pict.Visibility = Visibility.Visible;
                    }
                    if (sel_report != sel_report_last)
                    {
                        view_rep_pict = 0;
                        rep_Picture = new BitmapImage(new Uri(rep_pict_url + sel_report.reportImages[view_rep_pict].imageUrl));
                        while (rep_Picture.Width == 1)
                        {
                            await Task.Delay(100);

                        }
                        Thickness margin = ReportDetail_Pict.Margin;
                        margin.Top = await Get_Pict_MarginTop();
                        ReportDetail_Pict.Margin = margin;
                    }

                    ReportDetail_Pict.Source = rep_Picture;
                    ReportDetail_Status.Content = sel_report.status;
                    ReportDetail_Date.Content = sel_report.createdAtHH;
                    list_StatusHistory = await connection.Server_Get_StatusHistory(sel_report.id);
                    list_HistoryAll = list_StatusHistory;

                    list_ForwardingLogs = await connection.Server_Get_ForwardingLogs(sel_report.id);
                    foreach (var item in list_ForwardingLogs)
                    {
                        TV_StatusHistory new_item = new();
                        new_item.status = "forwarded";
                        new_item.changedAt = item.forwardedAt;
                        new_item.changedBy = new TVR_User();
                        new_item.changedBy.username = item.forwardedByUser.username;
                        new_item.comment = "[ " + item.forwardedFrom.name + " -> " + item.forwardedTo.name + " ]\n" + item.reason;

                        list_HistoryAll.Add(new_item);
                    }

                    List<TV_StatusHistory> open_found = list_HistoryAll.Where(l => l.status == "open").ToList();
                    if (open_found.Count == 0)
                    {
                        TV_StatusHistory new_open = new();
                        new_open.status = "open";
                        new_open.changedAt = sel_report.createdAt;
                        new_open.changedBy = new TVR_User();
                        new_open.changedBy.username = sel_report.user.username;
                        //new_open.comment = "'X' akta megnyitva, beküldve.";
                        new_open.comment = "A bejelentés létrehozva";
                        list_HistoryAll.Add(new_open);
                    }

                    ListView_History.ItemsSource = list_HistoryAll.OrderBy(u => u.changedAt).ToList();
                    if (list_HistoryAll.Count > 0)
                    {
                        ListView_History.SelectedItem = ListView_History.Items[list_HistoryAll.Count - 1];
                        ListView_History.UpdateLayout();
                    }
                    sel_report_last = sel_report;
                    if (start_report_y)
                    {
                        start_report_y = false;
                    }
                }
            }
        }
        private void DWL_Pict(string a_url)
        {
            if (a_url.Substring(0, 4) == "http")
            {
                string a_pict = a_url.Split("/").Last();
                if (!File.Exists(pict_path + "\\Logo\\" + a_pict))
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(a_url, pict_path + "\\Logo\\" + a_pict);
                    }
                }
            }
            else
            {
                string web_url = rep_pict_url + a_url;
                if (!File.Exists(pict_path + a_url))
                {
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(web_url, pict_path + a_url);
                    }
                }
            }
        }
        private async Task<double> Get_Pict_MarginTop()
        {
            while (rep_Picture.Width == 1)
            {
                await Task.Delay(100);

            }
            double mtop = (255 - rep_Picture.Height / rep_Picture.Width * 400) / 2 + 20;
            if (mtop > 0.0) { mtop = 0.0; }
            return mtop;
        }
        private void View_ReportPict_Prev(object sender, RoutedEventArgs e)
        {
            if (view_rep_pict > 0)
            {
                view_rep_pict--;
                if (view_rep_pict == 0)
                {
                    B_Prev_Pict.Visibility = Visibility.Hidden;
                }
                B_Next_Pict.Visibility = Visibility.Visible;
                Show_NewPicture();
            }
        }
        private async void Show_NewPicture()
        {
            rep_Picture = new BitmapImage(new Uri(rep_pict_url + sel_report.reportImages[view_rep_pict].imageUrl));
            Thickness margin = ReportDetail_Pict.Margin;
            margin.Top = await Get_Pict_MarginTop();
            ReportDetail_Pict.Source = rep_Picture;
            ReportDetail_Pict.Margin = margin;
        }
        private async void View_ReportPict_Next(object sender, RoutedEventArgs e)
        {
            if (view_rep_pict < max_rep_pict)
            {
                view_rep_pict++;
                if (view_rep_pict == max_rep_pict)
                {
                    B_Next_Pict.Visibility = Visibility.Hidden;
                }
                B_Prev_Pict.Visibility = Visibility.Visible;
                Show_NewPicture();
            }
        }
        private async void Challenge_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up) { Csel_Cursor_OFF(); Csel_Control_Over_Up.Visibility = Visibility.Visible; return; }
            if (e.Key == Key.Down) { Csel_Cursor_OFF(); Csel_Control_Over_Down.Visibility = Visibility.Visible; return; }
            else if (e.Key == Key.Escape) { Panel.SetZIndex(Challenge_Detail, 0); }
        }
        private async void Report_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.RightCtrl))
            {
                Rep_Cursor_OFF();
                Rep_Control_Over_CTRL.Visibility = Visibility.Visible;
                Show_BigPicture(sender, e);
            }
            if (e.Key == Key.Left) { View_ReportPict_Prev(sender, e); Rep_Cursor_OFF(); Rep_Control_Over_Left.Visibility = Visibility.Visible; return; }
            if (e.Key == Key.Right) { View_ReportPict_Next(sender, e); Rep_Cursor_OFF(); Rep_Control_Over_Right.Visibility = Visibility.Visible; return; }
            if (e.Key == Key.Up) { Rep_Cursor_OFF(); Rep_Control_Over_Up.Visibility = Visibility.Visible; return; }
            if (e.Key == Key.Down) { Rep_Cursor_OFF(); Rep_Control_Over_Down.Visibility = Visibility.Visible; return; }
            if (e.Key == Key.NumPad8)
            {
                Thickness margin = ReportDetail_Pict.Margin;
                margin.Top = margin.Top - 10;
                ReportDetail_Pict.Margin = margin;
            }
            else if (e.Key == Key.NumPad5)
            {
                Thickness margin = ReportDetail_Pict.Margin;
                margin.Top = await Get_Pict_MarginTop();
                margin.Left = 0;
                ReportDetail_Pict.Margin = margin;
            }
            else if (e.Key == Key.NumPad2)
            {
                Thickness margin = ReportDetail_Pict.Margin;
                if (margin.Top < 0)
                {
                    margin.Top = margin.Top + 10;
                    ReportDetail_Pict.Margin = margin;
                }
            }
            else if (e.Key == Key.Escape) { Panel.SetZIndex(Report_Detail, 0); }
        }
        internal void Csel_Cursor_OFF()
        {
            Csel_Control_Over_Left.Visibility = Visibility.Hidden;
            Csel_Control_Over_Right.Visibility = Visibility.Hidden;
            Csel_Control_Over_Up.Visibility = Visibility.Hidden;
            Csel_Control_Over_Down.Visibility = Visibility.Hidden;
        }
        internal void Rep_Cursor_OFF()
        {
            Rep_Control_Over_Left.Visibility = Visibility.Hidden;
            Rep_Control_Over_Right.Visibility = Visibility.Hidden;
            Rep_Control_Over_Up.Visibility = Visibility.Hidden;
            Rep_Control_Over_Down.Visibility = Visibility.Hidden;
        }
        private void Back2Challenge_List(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(Challenge_Detail, 0);
        }
        private void Back2Report_List(object sender, RoutedEventArgs e)
        {
            Panel.SetZIndex(Report_Detail, 0);
        }
        private async void Category_ListView_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                H_Category_Delete.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray));
                hw_now_y = true;
                chk_cdata_y = true;
                sel_cat = item as TV_Cats;
                H_Category_Name.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tb_NotEmpty));
                H_Category_Name.Text = sel_cat.categoryName;
                CB_H_Category_Inst.SelectedIndex = -1;
                hw_now_y = false;
                CB_H_Category_Inst.SelectedItem = sel_cat.institution.name;
                int a_db = await connection.Server_Get_ReportCat_db(sel_cat.id);
                H_Category_Reports_N.Text = a_db.ToString();
                H_Category_Save.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray));
                if (a_db > 0)
                {
                    H_Category_Reports_N.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Yellow));
                }
                else
                {
                    H_Category_Reports_N.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Green));
                }
                SelCategory_CHK_Modified();
            }
        }
        private void Inst_ListView_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as ListView).SelectedItem;
            if (item != null)
            {
                H_Inst_Delete.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray));
                chk_idata_y = false;
                sel_inst = item as TV_Inst;
                H_Inst_Name.Text = sel_inst.name;
                H_Inst_Email.Text = sel_inst.email;
                H_Inst_Desc.Text = sel_inst.description;
                string addr = sel_inst.contactInfo;
                if (addr == null)
                {
                    H_Inst_Zip.Text = "";
                    H_Inst_City.Text = "";
                    H_Inst_Address.Text = "";
                }
                else
                {
                    H_Inst_Zip.Text = addr.Split(' ')[1].Trim();
                    H_Inst_City.Text = addr.Split(',')[0].Split(' ')[2].Trim();
                    H_Inst_Address.Text = addr.Split('|')[0].Split(',')[1].Trim();
                    if (addr.Contains(" | "))
                    {
                        if (addr.Contains("Tel:"))
                        {
                            H_Inst_Tel.Text = addr.Split('|')[1].Trim().Split(":")[1].Trim();
                        }
                        else
                        {
                            H_Inst_Tel.Text = addr.Split('|')[1].Trim();
                        }

                    }
                }
                inst_logo_url = sel_inst.logoUrl;
                Logo_Actual();
                chk_idata_y = true;
                Inst_Sel_Stat_Edit();
            }
        }
        private async void Inst_Sel_Stat_Edit()
        {
            string e_id = sel_inst.id;
            string e_logo_Url = sel_inst.logoUrl;
            if (hold_workers)
            {
                Freeze_Workerlist();
            }
            List<TV_User> edit_workers = await connection.Get_Workers(e_id, "Inst_Sel_Stat_Edit");
            ListView_Workers.ItemsSource = edit_workers;
            int eworkers_db = edit_workers.Count;
            SetColor_Label(eworkers_db, Inst_WorkersNum_Edit);
            int erep_db = await connection.Server_Get_InstValami_db("/api/reports/report_Inst_db", e_id);
            SetColor_Label(erep_db, Inst_ReportNum_Edit);
            int enews_db = await connection.Server_Get_InstValami_db("/api/news/news_Inst_db", e_id);
            SetColor_Label(enews_db, Inst_NewsNum_Edit);
            Freeze_Workerlist();
            if (e_logo_Url != null && e_logo_Url != "")
            {
                if (App.vanNet_y)
                {
                    Inst_LogoImg_Edit.Source = new BitmapImage(new Uri(e_logo_Url));
                    DWL_Pict(e_logo_Url);
                }
                else { Inst_LogoImg_Edit.Source = new BitmapImage(new Uri(pict_path + "\\Logo\\" + e_logo_Url.Split("/").Last())); }
            }
            else { Inst_LogoImg_Edit.Source = new BitmapImage(new Uri("http://smd.hu/Team/Empty_Logo.gif")); }
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
                u_fromlist_y = true;
                sel_user = item as TV_User;
                H_User_Name.Text = sel_user.username;
                H_User_Email.Text = sel_user.email;
                H_User_Zip.Text = sel_user.zipCode;
                H_User_City.Text = sel_user.city;
                H_User_Address.Text = sel_user.address;

                CB_H_User_Status.SelectedItem = "";
                //Change_ComboBoxBorderColor(CB_H_User_Status, "");
                CB_H_User_Role.SelectedItem = "";

                if (sel_user.institutionId != null)
                {
                    CB_H_User_Inst.SelectedItem = sel_user.institution;
                }
                else
                {
                    CB_H_User_Inst.SelectedItem = "";
                }
                CB_H_User_Status.SelectedItem = sel_user.isActive;
                CB_H_User_Role.SelectedItem = sel_user.role;
                chk_udata_y = true;
                H_User_Save.Background = H_User_Search.Background;
            }
        }
        private void SelCategory_DataChanged(object sender, TextChangedEventArgs e)
        {
            SelCategory_CHK_Modified();
            //sender.focus()
        }
        private void SelCategory_CHK_Modified()
        {
            if (!hw_now_y)
            {
                c_save_y = false;
                c_savenew_y = false;
                ListView_Categories.SelectedItem = sel_cat;
                string c_name = H_Category_Name.Text;
                string c_inst = CB_H_Category_Inst.Text;
                bool sel_cinst_y = CB_H_Category_Inst.SelectedIndex > 0;
                bool sel_cname_y = c_name != "";
                TV_Cats cat_found = new();

                if (sel_cname_y) { cat_found = list_cats.Find(c => c.categoryName.ToUpper() == c_name.ToUpper()); }

                if (chk_cdata_y)
                {
                    if (c_name == sel_cat.categoryName)
                    {
                        if (c_inst != sel_cat.linked_inst)
                        {
                            c_save_y = true;
                        }
                    }
                    else if (cat_found != null)
                    {
                        H_Category_Name.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tb_LightRed));
                        ListView_Categories.SelectedItem = cat_found;
                    }
                    else
                    {
                        H_Category_Name.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tb_NotEmpty));
                        c_savenew_y = true;
                    }
                }
                else
                {
                    if (cat_found != null)
                    {
                        H_Category_Name.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tb_LightRed));
                    }
                    else
                    {
                        if (sel_cname_y)
                        {
                            H_Category_Name.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tb_NotEmpty));
                        }
                        else
                        {
                            H_Category_Name.Background = new SolidColorBrush(Colors.White);
                        }
                        c_savenew_y = sel_cinst_y && sel_cname_y;
                    }
                }
                H_Category_SaveColor();
            }
        }
        private void H_Category_SaveColor()
        {

            if (c_save_y)
            {
                H_Category_Save.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Green));
            }
            else { H_Category_Save.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray)); }

            if (c_savenew_y) { H_Category_SaveNew.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF9DD24")); }
            else { H_Category_SaveNew.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray)); }
        }
        private void SelUser_DataChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            Changed_UserData();
            if (tb.Text != "")
            {
                tb.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tb_NotEmpty));
                return;
            }
            tb.Background = new SolidColorBrush(Colors.White);
        }
        private void SelInst_DataChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = sender as TextBox;
            SelInst_CHK_Modified();
            if (tb.Text != "")
            {
                tb.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(tb_NotEmpty));
                //tb.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#6FB1A5"));
                return;
            }
            tb.Background = new SolidColorBrush(Colors.White);
        }

        private async void SelInst_CHK_Modified()
        {
            bool do_y = await Changed_InstData();
            if (do_y)
            {
                H_Inst_Save.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF6EF525"));
            }
            else
            {
                H_Inst_Save.Background = H_User_Search.Background;
            }
        }
        private void Categories_Clear(object sender, RoutedEventArgs e)
        {
            Categories_ClearData();
            ListView_Categories.Focus();
        }
        private void Categories_ClearData()
        {
            chk_cdata_y = false;
            hw_now_y = true;

            sel_cat = new TV_Cats();
            H_Category_Name.Text = "";
            H_Category_Name.Background = new SolidColorBrush(Colors.White);
            H_CategoryInst_Desc.Text = "";
            H_CategoryInst_Desc.Background = new SolidColorBrush(Colors.White);
            H_CategoryInst_Contact.Text = "";
            H_CategoryInst_Contact.Background = new SolidColorBrush(Colors.White);
            H_Category_Reports_N.Text = "";
            H_Category_Reports_N.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray));


            H_Category_Save.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray));
            H_Category_SaveNew.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray));
            H_Category_Delete.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray));

            CB_H_Category_Inst.SelectedIndex = -1;
            Change_ComboBoxBorderColor(CB_H_Category_Inst, "");

            hw_now_y = false;
        }

        private void Inst_Clear(object sender, RoutedEventArgs e)
        {
            Inst_ClearData();
        }
        private void Inst_ClearData()
        {
            //Új Intézmény felvételéhez minden mező törlése 
            chk_idata_y = false;
            H_Inst_Name.Text = "";
            H_Inst_Email.Text = "";
            H_Inst_Zip.Text = "";
            H_Inst_City.Text = "";
            H_Inst_Address.Text = "";
            H_Inst_Tel.Text = "";
            H_Inst_Desc.Text = "";
            H_Inst_Save.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray));
            H_Inst_SaveNew.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray));
            inst_logo_url = "";
            H_Inst_Logo.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray));
            H_Inst_Logo.Foreground = new SolidColorBrush(Colors.White);
            sel_inst = new TV_Inst();
            H_Inst_Delete.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray));
            Inst_LogoImg.Source = empty_logo;
        }
        private async void Inst_Save(object sender, RoutedEventArgs e)
        {
            bool do_y = await Changed_InstData();
            if (do_y)
            {
                Update_SelInst();
            }
        }
        private async void Update_SelInst()
        {
            Put_SelInst();
            bool upd_y = await connection.AdminUpdate_Inst(sel_inst);
            if (upd_y)
            {
                int a_index = list_inst.FindIndex(u => u.id == sel_inst.id);
                if (a_index > -1)
                {
                    list_inst[a_index] = sel_inst;
                    ListView_Inst.Items.Refresh();
                    Inst_ClearData();
                    MessageBox.Show("Elvileg Frissítve");
                }
                else
                {
                    MessageBox.Show("Update Hiba!");
                }
            }
            else { MessageBox.Show("Hiba"); }
        }
        private void Categories_ReLoad(object sender, RoutedEventArgs e)
        {
            Get_Categories_List();
        }
        private async void Get_Categories_List()
        {
            list_cats = await connection.Server_Get_Categories();
            if (!start_cats_y)
            {
                ListView_Categories.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bus_Yellow));
            }
            foreach (var item in list_cats)
            {
                int a_db = await connection.Server_Get_ReportCat_db(item.id);
                if (a_db == 0) { item.rep_db = ""; }
                else { item.rep_db = a_db.ToString(); }
            }
            ListView_Categories.ItemsSource = list_cats.OrderBy(c => c.categoryName).ToList();
            ListView_Categories.Focus();
            ListView_Categories.SelectedItem = ListView_Categories.Items[0];
            ListView_Categories.SelectedIndex = 0;

            if (!start_cats_y)
            {
                await Task.Delay(100);
                ListView_Categories.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
            }
            start_cats_y = false;
        }
        private async void Inst_ReLoad(object sender, RoutedEventArgs e)
        {
            Inst_ReLoad_Auto();
        }
        private async void Inst_ReLoad_Auto()
        {
            Get_Inst_List();
            if (!start_inst_y)
            {
                ListView_Inst.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bus_Yellow));
                await Task.Delay(100);
                ListView_Inst.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
            }
            Get_WorkerList_Auto();
            ListView_Inst.SelectedIndex = 0;
            ListView_Inst.Focus();
        }
        private void Put_SelInst()
        {
            sel_inst.name = H_Inst_Name.Text;
            sel_inst.email = H_Inst_Email.Text;
            sel_inst.description = H_Inst_Desc.Text;
            string full_addr = "Cím: " + H_Inst_Zip.Text + " " + H_Inst_City.Text + ", " + H_Inst_Address.Text;
            if (H_Inst_Tel.Text != "")
            {
                full_addr += " | Tel: " + H_Inst_Tel.Text;
            }
            sel_inst.contactInfo = full_addr;
            sel_inst.logoUrl = inst_logo_url;
        }
        private async void Inst_SaveAsNew(object sender, RoutedEventArgs e)
        {
            string e_inst = await connection.Check_ExistInst(H_Inst_Email.Text, H_Inst_Name.Text);
            if (e_inst == "00")
            {
                sel_inst = new();
                Put_SelInst();
                string new_instId = await connection.Admin_AddNewInst(sel_inst);
                if (new_instId == "22")
                {
                    MessageBox.Show("Hiba az új Intézmény felvételénél!");
                    return;
                }
                Inst_ClearData();
                Get_Inst_List();
            }
            else
            {
                if (e_inst == "01")
                {
                    MessageBox.Show("Email már létezik!");
                    return;
                }
                if (e_inst == "11")
                {
                    MessageBox.Show("Név és Email már létezik!");
                    return;
                }
                if (e_inst == "10")
                {
                    MessageBox.Show("Név már létezik!");
                    return;
                }
                MessageBox.Show("Egészen más Hiba!!");
            }
            //MessageBox.Show("Nincs kiválasztva Intézmény!");
        }
        private async void Inst_Logo(object sender, RoutedEventArgs e)
        {
            logoURLWindow = new TV_Logo_Input();
            logoURLWindow.Logo_IMG.Source = null;
            logoURLWindow.Inst_Name.Content = H_Inst_Name.Text;
            logoURLWindow.Inst_Logo_URL.Text = inst_logo_url;
            logoURLWindow.Closed += Bezarka;

            var hwnd = new WindowInteropHelper(this).Handle;
            IntPtr hMenu = GetSystemMenu(hwnd, false);
            EnableMenuItem(hMenu, SC_CLOSE, MF_GRAYED);

            this.IsEnabled = false;
            logoURLWindow.Top = this.Top + this.Height / 2 - logoURLWindow.Height / 2;
            logoURLWindow.Left = this.Left + this.Width / 2 - logoURLWindow.Width / 2;
            logoURLWindow.Show();
        }
        private async Task<bool> Changed_InstData()
        {
            //Változott e valami Hatósági adatokban??
            bool do_y = false;
            if (chk_idata_y)
            {
                string full_addr = "Cím: " + H_Inst_Zip.Text + " " + H_Inst_City.Text + ", " + H_Inst_Address.Text;
                if (H_Inst_Tel.Text != "")
                {
                    full_addr += " | Tel: " + H_Inst_Tel.Text;
                }
                do_y = do_y || H_Inst_Name.Text != sel_inst.name || H_Inst_Email.Text != sel_inst.email
                    || H_Inst_Desc.Text != sel_inst.description || full_addr != sel_inst.contactInfo || inst_logo_url != sel_inst.logoUrl;
                bool chk_y = true;
                if (sel_inst != null)
                {
                    chk_y = true;
                }
                else
                {
                    chk_y = H_Inst_Name.Text != sel_inst.name && H_Inst_Email.Text != sel_inst.email;
                }
                if (chk_y)
                {
                    string e_inst = await connection.Check_ExistInst(H_Inst_Email.Text, H_Inst_Name.Text);
                    if (e_inst == "00")
                    {
                        H_Inst_SaveNew.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF9DD24"));
                    }
                    else
                    {
                        H_Inst_SaveNew.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray));
                    }
                }
                else
                {
                    H_Inst_SaveNew.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray));
                }
            }
            return do_y;
        }

        private async void User_Delete(object sender, RoutedEventArgs e)
        {
            if (sel_user != null)
            {
                string del_name = H_User_Name.Text;
                string del_emil = H_User_Email.Text;
                if (del_name != "" && del_emil != "")
                {
                    if (MessageBox.Show("[ '" + del_name + "' ]\n\nMindenképpen törölni akarod ezt a felhasználót", "Felhasnáló Törlése:",
                        MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        string del_msg = await connection.Admin_DelUser(del_emil);
                        if (del_msg == "Nem vagyok Teszt üzemmódban.")
                        {
                            MessageBox.Show("A törlés most nem engedélyezett, csak teszt üzemmódban!");
                            return;
                        }
                        if (del_msg != "xx")
                        {
                            TV_User a_del = list_user.Find(u => u.email == del_emil);
                            if (a_del != null)
                            {
                                Get_User_List(S_User_Name.Text, S_User_Email.Text);
                            }
                            User_ClearData();
                            MessageBox.Show("User: '" + del_name + "'\n\n" + del_msg);
                            return;
                        }
                        MessageBox.Show("A törlés nem sikerült!");
                    }
                }
            }
        }
        public bool Valid_Email(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email && email.Last() != '.';
            }
            catch
            {
                return false;
            }
        }

        private async void Add_Admin_Picture(object sender, RoutedEventArgs e)
        {
            return;
            /*int a_id = 20;
            string a_uri = "R:\\56820-58e0dc105fbca.png";
            string rea_add = await connection.Server_AddNewPicture( a_id,  a_uri);*/
        }
        private async void Mod_Admin_Password(object sender, RoutedEventArgs e)
        {
            if (a_sel_menu == "Users")
            {
                if (H_User_Name.Text != "" && H_User_Email.Text != "")
                {
                    psw_input_using = "New_PSW";
                    Get_New_PSW();
                }
            }
        }
        private async void Category_SaveAsNew(object sender, RoutedEventArgs e)
        {
            if (c_savenew_y)
            {
                string new_name = H_Category_Name.Text;
                string new_cat_inst_id = list_inst.Find(u => u.name == CB_H_Category_Inst.SelectedItem).id;
                string new_cat_id = await connection.Server_AddNewCategory(new_name, new_cat_inst_id);
                if (new_cat_id == "22")
                {
                    MessageBox.Show("Nem sikerült az Új kategória hozzáadása!");
                    return;
                }
                Get_Categories_List();
                H_Category_Save.Background = H_User_Search.Background;
                H_Category_SaveNew.Background = H_User_Search.Background;
                Categories_ClearData();

            }
        }
        private async void Category_Delete(object sender, RoutedEventArgs e)
        {
            if (sel_cat != null)
            {
                if (H_Category_Reports_N.Text == "0")
                {
                    if (MessageBox.Show("[ '" + sel_cat.categoryName + "' ]\n\nMindenképpen törölni akarod ezt a kategóriát?",
                        "Kategória Törlése:", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        string del_res = await connection.Admin_Del_byID("/api/categories/delete/", sel_cat.id);
                        if (del_res == "xx") { MessageBox.Show("Nem sikerült a törlés!"); return; }
                        Get_Categories_List();
                        Categories_ClearData();
                    }
                }
                else
                {
                    MessageBox.Show("Ezt a kategóriát nem tudom törölni, mert már " + H_Category_Reports_N.Text +
                        " bejelentés érkezett rá!", "Kategória Törlése:");
                }
            }
        }
        private void Challenges_ReLoad(object sender, RoutedEventArgs e)
        {
            Get_Challenge_List();
        }
        private async void Get_Challenge_List()
        {
            Get_Challenges_List();
        }
        private async void Get_Challenges_List()
        {
            if (!start_challenge_y)
            {
                ListView_Challenges.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bus_Yellow));
            }
            string v_date = datePickerC_From.Text.Replace(". ", "-").Replace(".", "") + "T00:00:00.000Z";
            string k_date = datePickerC_Until.Text.Replace(". ", "-").Replace(".", "") + "T00:00:00.000Z";
            list_challanges = await connection.Server_Get_AllChallenges_FromDate(k_date, v_date);
            foreach (var item in list_challanges)
            {

                item.inst_name = list_inst.Where(inst => inst.id == item.institutionId).ToList()[0].name;
            }
            if (list_challanges.Count == 0)
            {
                ListView_Challenges.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                ListView_Challenges.ItemsSource = list_challanges;
                return;
            }
            if (!start_challenge_y)
            {
                await Task.Delay(100);
                ListView_Challenges.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
            }
            ListView_Challenges.ItemsSource = list_challanges;
            ListView_Challenges.SelectedIndex = 0;
            ListView_Challenges.Focus();
            Challenge_Filtering_Auto();

            Challenge_ListView_Click_Auto();
        }
        private void Report_ReLoad(object sender, RoutedEventArgs e)
        {
            Get_Report_List();
        }
        private async void Get_Report_List()
        {
            if (!start_report_y)
            {
                ListView_Reports.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bus_Yellow));
            }
            string v_date = datePicker_From.Text.Replace(". ", "-").Replace(".", "") + "T00:00:00.000Z";
            string k_date = datePicker_Until.Text.Replace(". ", "-").Replace(".", "") + "T00:00:00.000Z";
            list_reports = await connection.Server_Get_AllReports_FromDate(k_date, v_date);
            if (list_reports.Count == 0)
            {
                ListView_Reports.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
                ListView_Reports.ItemsSource = list_reports;
                return;
            }
            if (!start_report_y)
            {
                await Task.Delay(100);
                ListView_Reports.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFF"));
            }
            foreach (var item in list_reports)
            {
                if (item.reportImages.Count > 1) { item.pict_db = "+"; }
                else { item.pict_db = ""; }
                item.pict_db = item.reportImages.Count.ToString();
            }
            ListView_Reports.ItemsSource = list_reports;
            ListView_Reports.SelectedIndex = 0;
            ListView_Reports.Focus();
            Report_ListView_Click_Auto();
        }

        private void Report_Filtering(object sender, RoutedEventArgs e)
        {
            Report_Filtering_Auto();
        }
        private void Report_Filtering_Auto()
        {
            ListView_Reports.ItemsSource = list_reports.Where(r =>
                r.title.Contains(S_Report_PartialText.Text) || r.description.Contains(S_Report_PartialText.Text)).ToList();
            if (ListView_Reports.Items.Count == 0) { Panel.SetZIndex(Report_Detail, 0); }
            else { ListView_Reports.SelectedItem = ListView_Reports.Items[0]; Report_ListView_Click_Auto(); }
            if (ListView_Reports.Items.Count == 0)
            {
                S_Report_PartialText.Focus();
            }
            else
            {
                ListView_Reports.Focus();
            }
        }
        private async void Report_DelPicture_Click(object sender, RoutedEventArgs e)
        {
            string pict_id = sel_report.reportImages[view_rep_pict].id.ToString();
            if (MessageBox.Show("Mindenképpen törölni akarod ezt a képet", "Kép Törlése:", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                string del_msg = await connection.Admin_DelPict_byID(pict_id);
                MessageBox.Show(del_msg);
                int a_idx = ListView_Reports.SelectedIndex;
                sel_report_last = new();
                Get_Report_List();
                ListView_Reports.SelectedIndex = a_idx;
            }
        }
        private async void Report_AddPicture_Click(object sender, RoutedEventArgs e)
        {
            if (max_rep_pict >= 3)
            {
                MessageBox.Show("Már van 3 db kép!");
            }
            else
            {
                int a_id = sel_report.id;
                var ofd = new Microsoft.Win32.OpenFileDialog() { Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif" };
                var result = ofd.ShowDialog();
                if (result == false) return;
                MessageBox.Show(ofd.FileName);
                string a_uri = ofd.FileName;// "R:\\56820-58e0dc105fbca.png";
                string rea_add = await connection.Server_AddNewPicture(a_id, a_uri);
                if (rea_add == "00")
                {
                    MessageBox.Show("A kép sikeresen hozzáadva!");
                    int a_idx = ListView_Reports.SelectedIndex;
                    sel_report_last = new();
                    Get_Report_List();
                    ListView_Reports.SelectedIndex = a_idx;
                }
                else
                {

                }
            }
        }
        private void Challenge_Filtering_Clear(object sender, RoutedEventArgs e)
        {
            S_Challenge_PartialText.Text = "";
            Get_Challenge_List();
        }

        private void Report_Filtering_Clear(object sender, RoutedEventArgs e)
        {
            S_Report_PartialText.Text = "";
            Get_Report_List();
        }
        private void Challenge_PartialKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Challenge_Filtering_Auto();
            }
        }
        private void Challenge_Filtering(object sender, RoutedEventArgs e)
        {
            Challenge_Filtering_Auto();
        }
        private void Challenge_Filtering_Auto()
        {
            string keresett = S_Challenge_PartialText.Text.ToLower();
            if (keresett != "")
            {
                ListView_Challenges.ItemsSource = list_challanges.Where(r => r.title.ToLower().Contains(keresett) || r.description.Contains(keresett)).ToList();
                if (ListView_Challenges.Items.Count == 0) { Panel.SetZIndex(Challenge_Detail, 0); }
                else { ListView_Reports.SelectedItem = ListView_Challenges.Items[0]; Challenge_ListView_Click_Auto(); }
                if (ListView_Challenges.Items.Count == 0)
                {
                    S_Challenge_PartialText.Focus();
                }
                else
                {
                    ListView_Challenges.Focus();
                }
            }
        }
        private void Report_PartialKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Report_Filtering_Auto();
            }
        }
        private void Challenge_DLong_CBChanged(object sender, SelectionChangedEventArgs e)
        {
            int a_idx = CB_H_Challenge_DLong.SelectedIndex;
            int a_eltol = 0;
            if (a_idx == 6)
            {
                datePickerC_Until.SelectedDate = DateTime.Parse("2000.01.01");
            }
            else
            {
                a_eltol = last_ndays_n[CB_H_Challenge_DLong.SelectedIndex];
                datePickerC_Until.SelectedDate = DateTime.Now.AddDays(a_eltol);
            }
            if (!start_challenge_y)
            {
                Get_Challenge_List();
            }
        }
        private void Report_DLong_CBChanged(object sender, SelectionChangedEventArgs e)
        {
            int a_idx = CB_H_Report_DLong.SelectedIndex;
            int a_eltol = 0;
            if (a_idx == 6)
            {
                datePicker_Until.SelectedDate = DateTime.Parse("2000.01.01");
            }
            else
            {
                a_eltol = last_ndays_n[CB_H_Report_DLong.SelectedIndex];
                datePicker_Until.SelectedDate = DateTime.Now.AddDays(a_eltol);
            }
            if (!start_report_y)
            {
                Get_Report_List();
            }
        }

        private async void Inst_Delete(object sender, RoutedEventArgs e)
        {
            if (sel_inst != null)
            {
                List<TV_User> mod_workers = await connection.Get_Workers(sel_inst.id, "Inst_Delete");
                Del_Workers.ItemsSource = mod_workers;
                if (mod_workers.Count > 0)
                {
                    H_Inst_Delete.Background = new SolidColorBrush(Colors.Red);
                    MessageBox.Show("Az Intézménynek vannak Munkatársai, előbb a hozzárendeléseket törölni kell!");
                    return;
                }
                if (MessageBox.Show("[ '" + sel_inst.name + "' ]\n\nMindenképpen törölni akarod ezt az Intézményt?",
                    "Intézmény Törlése:", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    //await connection.Admin_DelInstitution(sel_inst.id);
                    string del_res = await connection.Admin_Del_byID("/api/institutions/delete/", sel_inst.id);
                    if (del_res == "xx") { MessageBox.Show("Nem sikerült a törlés!"); return; }
                    Get_Inst_List();
                    Inst_ClearData();
                }
            }
        }
        private void Detach_Worker(object sender, RoutedEventArgs e)
        {

        }
        private void SetColor_Label(int xmi_db, TextBox out_label)
        {
            out_label.Text = xmi_db.ToString();
            if (xmi_db == -1)
            {
                out_label.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray));
            }
            else if (xmi_db == 0)
            {
                out_label.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Green));
            }
            else
            {
                out_label.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bus_Yellow));
            }
        }
        private async void Bezarka(object sender, EventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            IntPtr hMenu = GetSystemMenu(hwnd, false);
            EnableMenuItem(hMenu, SC_CLOSE, MF_ENABLED);
            this.IsEnabled = true;
            if (sender is TV_Logo_Input)
            {
                Logo_Actual();
                SelInst_CHK_Modified();
            }
            else if (sender is TV_PSW_Input)
            {
                if (new_vmi_psw != "xx" && new_vmi_psw != "x")
                {
                    if (psw_input_using == "New_PSW")
                    {
                        string a_email = H_User_Email.Text;
                        if (a_email != "" && new_vmi_psw != "")
                        {
                            string mod_res = await connection.Admin_ModUser_Password(a_email, new_vmi_psw);
                        }
                    }
                    else if (psw_input_using == "SaveAsNew")
                    {
                        string new_log = "PSW for '" + H_User_Name.Text + " - " + H_User_Email.Text + "': '" + new_vmi_psw + "'\n";
                        App.postit += new_log;
                        File.AppendAllText(pict_path + "admin_added_users.txt", new_log);
                        string new_userId = await connection.Admin_AddNewUser(H_User_Email.Text, H_User_Name.Text, new_vmi_psw);
                        if (new_userId == "22")
                        {
                            MessageBox.Show("Hiba az új felhasználó felvételénél!");
                            return;
                        }
                        if (sel_user != null)
                        {
                            sel_user.id = new_userId;
                        }
                        User_Update();
                        //User_ClearData();
                        Get_User_List(S_User_Name.Text, S_User_Email.Text);
                        H_User_SaveNew.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray));
                    }
                }
            }
            else if (sender is TV_Show_Picture)
            {
                Rep_Control_Over_CTRL.Visibility = Visibility.Hidden;
            }
        }
        private void Logo_Actual()
        {
            if (inst_logo_url != null && inst_logo_url != "")
            {
                H_Inst_Logo.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_LightGreen));
                H_Inst_Logo.Foreground = new SolidColorBrush(Colors.White);
                Inst_LogoImg.Source = new BitmapImage(new Uri(inst_logo_url));
                if (Inst_LogoImg.Width == 0)
                {
                    inst_logo_url = "";
                    Inst_LogoImg.Source = empty_logo; // new BitmapImage(new Uri("https://smd.hu/Team/Empty_Logo.gif"));
                }
            }
            else
            {
                Inst_LogoImg.Source = empty_logo; // new BitmapImage(new Uri("https://smd.hu/Team/Empty_Logo.gif"));
                H_Inst_Logo.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom(bc_Gray));
                H_Inst_Logo.Foreground = new SolidColorBrush(Colors.White);
            }
        }
        private void Challenge_SortHeaderClick(object sender, RoutedEventArgs e)
        {
            string[] headText = ["Id", "Kép", "Kihívás elnevezése", "Kategória", "Pont költsége", "Pont jutalma", "Kihívás kezdete", "Kihívás vége"];
            try
            {
                string a_head = ((GridViewColumnHeader)e.OriginalSource).Column.Header.ToString();
                for (int i = 0; i < headText.Length; i++)
                {
                    if (a_head == headText[i])
                    {
                        switch (a_head)
                        {
                            case "Id":
                                if (order_challs_y[i])
                                {
                                    ListView_Challenges.ItemsSource = list_challanges.OrderBy(u => u.id).ToList();
                                }
                                else
                                {
                                    ListView_Challenges.ItemsSource = list_challanges.OrderByDescending(u => u.id).ToList();
                                }
                                break;
                            case "Kihívás elnevezése":
                                if (order_challs_y[i])
                                {
                                    ListView_Challenges.ItemsSource = list_challanges.OrderBy(u => u.id).ToList();
                                }
                                else
                                {
                                    ListView_Challenges.ItemsSource = list_challanges.OrderByDescending(u => u.id).ToList();
                                }
                                break;
                            case "Kategória":
                                if (order_challs_y[i])
                                {
                                    ListView_Challenges.ItemsSource = list_challanges.OrderBy(u => u.category).ToList();
                                }
                                else
                                {
                                    ListView_Challenges.ItemsSource = list_challanges.OrderByDescending(u => u.category).ToList();
                                }
                                break;
                            case "Pont költsége":
                                if (order_challs_y[i])
                                {
                                    ListView_Challenges.ItemsSource = list_challanges.OrderBy(u => u.costPoints).ToList();
                                }
                                else
                                {
                                    ListView_Challenges.ItemsSource = list_challanges.OrderByDescending(u => u.costPoints).ToList();
                                }
                                break;
                            case "Pont jutalma":
                                if (order_challs_y[i])
                                {
                                    ListView_Challenges.ItemsSource = list_challanges.OrderBy(u => u.rewardPoints).ToList();
                                }
                                else
                                {
                                    ListView_Challenges.ItemsSource = list_challanges.OrderByDescending(u => u.rewardPoints).ToList();
                                }
                                break;
                            case "Kihívás kezdete":
                                if (order_challs_y[i])
                                {
                                    ListView_Challenges.ItemsSource = list_challanges.OrderBy(u => u.startDate).ToList();
                                }
                                else
                                {
                                    ListView_Challenges.ItemsSource = list_challanges.OrderByDescending(u => u.startDate).ToList();
                                }
                                break;
                            case "Kihívás vége":
                                if (order_challs_y[i])
                                {
                                    ListView_Challenges.ItemsSource = list_challanges.OrderBy(u => u.endDate).ToList();
                                }
                                else
                                {
                                    ListView_Challenges.ItemsSource = list_challanges.OrderByDescending(u => u.endDate).ToList();
                                }
                                break;
                        }
                        order_challs_y[i] = !order_challs_y[i];
                    }
                }
            }
            catch { }
        }
        private void Report_SortHeaderClick(object sender, RoutedEventArgs e)
        {
            string[] headText = ["Id", "Kép", "Bejelentő", "Cím", "Kategória", "Város", "Dátum", "Intézmény"];
            try
            {
                string a_head = ((GridViewColumnHeader)e.OriginalSource).Column.Header.ToString();
                for (int i = 0; i < headText.Length; i++)
                {
                    if (a_head == headText[i])
                    {
                        switch (a_head)
                        {
                            case "Id":
                                if (order_reps_y[i])
                                {
                                    ListView_Reports.ItemsSource = list_reports.OrderBy(u => u.id).ToList();
                                }
                                else
                                {
                                    ListView_Reports.ItemsSource = list_reports.OrderByDescending(u => u.id).ToList();
                                }
                                break;

                            case "Kép":
                                ListView_Reports.ItemsSource = list_reports.OrderByDescending(u => u.createdAt).ToList();
                                break;
                            case "Bejelentő":
                                if (order_reps_y[i])
                                {
                                    ListView_Reports.ItemsSource = list_reports.OrderBy(u => u.user.username).ToList();
                                }
                                else
                                {
                                    ListView_Reports.ItemsSource = list_reports.OrderByDescending(u => u.user.username).ToList();
                                }
                                break;
                            case "Cím":
                                if (order_reps_y[i])
                                {
                                    ListView_Reports.ItemsSource = list_reports.OrderBy(u => u.title).ToList();
                                }
                                else
                                {
                                    ListView_Reports.ItemsSource = list_reports.OrderByDescending(u => u.title).ToList();
                                }
                                break;
                            case "Kategória":
                                if (order_reps_y[i])
                                {
                                    ListView_Reports.ItemsSource = list_reports.OrderBy(u => u.category.categoryName).ToList();
                                }
                                else
                                {
                                    ListView_Reports.ItemsSource = list_reports.OrderByDescending(u => u.category.categoryName).ToList();
                                }
                                break;
                            case "Város":
                                if (order_reps_y[i])
                                {
                                    ListView_Reports.ItemsSource = list_reports.OrderBy(u => u.city).ToList();
                                }
                                else
                                {
                                    ListView_Reports.ItemsSource = list_reports.OrderByDescending(u => u.city).ToList();
                                }
                                break;
                            case "Dátum":
                                if (order_reps_y[i])
                                {
                                    ListView_Reports.ItemsSource = list_reports.OrderBy(u => u.createdAt).ToList();
                                }
                                else
                                {
                                    ListView_Reports.ItemsSource = list_reports.OrderByDescending(u => u.createdAt).ToList();
                                }
                                break;
                            case "Intézmény":
                                if (order_reps_y[i])
                                {
                                    ListView_Reports.ItemsSource = list_reports.OrderBy(u => u.institution.name).ToList();
                                }
                                else
                                {
                                    ListView_Reports.ItemsSource = list_reports.OrderByDescending(u => u.institution.name).ToList();
                                }
                                break;
                            default:
                                break;
                        }
                        order_reps_y[i] = !order_reps_y[i];
                    }
                }
            }
            catch { }
        }
        private void Category_SortHeaderClick(object sender, RoutedEventArgs e)
        {
            string[] headText = ["Kategória Neve", "Report db", "Hozzárendelt Intézmény", "Létrehozás Dátuma"];
            try
            {
                string a_head = ((GridViewColumnHeader)e.OriginalSource).Column.Header.ToString();
                for (int i = 0; i < headText.Length; i++)
                {
                    if (a_head == headText[i])
                    {
                        switch (a_head)
                        {
                            case "Kategória Neve":
                                if (order_cats_y[i])
                                {
                                    ListView_Categories.ItemsSource = list_cats.OrderBy(u => u.categoryName).ToList();
                                }
                                else
                                {
                                    ListView_Categories.ItemsSource = list_cats.OrderByDescending(u => u.categoryName).ToList();
                                }
                                break;
                            case "Report db":
                                if (order_cats_y[i])
                                {
                                    ListView_Categories.ItemsSource = list_cats.OrderBy(u => u.rep_db).ToList();
                                }
                                else
                                {
                                    ListView_Categories.ItemsSource = list_cats.OrderByDescending(u => u.rep_db).ToList();
                                }
                                break;
                            case "Hozzárendelt Intézmény":
                                if (order_cats_y[i])
                                {
                                    ListView_Categories.ItemsSource = list_cats.OrderBy(u => u.institution.name).ToList();
                                }
                                else
                                {
                                    ListView_Categories.ItemsSource = list_cats.OrderByDescending(u => u.institution.name).ToList();
                                }
                                break;
                            case "Létrehozás Dátuma":
                                if (order_cats_y[i])
                                {
                                    ListView_Categories.ItemsSource = list_cats.OrderBy(u => u.createdAt).ToList();
                                }
                                else
                                {
                                    ListView_Categories.ItemsSource = list_cats.OrderByDescending(u => u.createdAt).ToList();
                                }
                                break;
                            default:
                                break;
                        }
                        order_cats_y[i] = !order_cats_y[i];
                    }
                }
            }
            catch { }
        }
        public class ImageConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                dynamic dataObject = value;
                if (dataObject != null)
                {
                    string path = dataObject.GetGuestImage();
                    if (System.IO.File.Exists(path))
                        return new Uri(dataObject.GetGuestImage(), UriKind.RelativeOrAbsolute);
                }

                return new Uri("https://smd.hu/Team/placeholderimage.png", UriKind.RelativeOrAbsolute);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
        public void Show_BigPicture(object sender, RoutedEventArgs e)
        {
            TV_Show_Picture view_big = new TV_Show_Picture();
            view_big.Closed += Bezarka;

            var hwnd = new WindowInteropHelper(this).Handle;
            IntPtr hMenu = GetSystemMenu(hwnd, false);
            EnableMenuItem(hMenu, SC_CLOSE, MF_GRAYED);
            double a_top = this.Top;
            double a_left = this.Left;

            double BarWidth = SystemParameters.VirtualScreenWidth - SystemParameters.WorkArea.Width;
            double BarHeight = SystemParameters.VirtualScreenHeight - SystemParameters.WorkArea.Height;
            view_big.Show();
            if (this.Left < 0)
            {
                view_big.Left = -(BarWidth + view_big.ActualWidth) / 2;
            }
            else
            {
                view_big.Left = (SystemParameters.VirtualScreenWidth - view_big.ActualWidth - BarWidth) / 2;
            }
            view_big.Top = (SystemParameters.VirtualScreenHeight - view_big.ActualHeight - BarHeight) / 2;
        }
        private void Report_Date_Changed(object sender, EventArgs e)
        {
            if (!start_report_y)
            {
                Get_Report_List();
            }
        }
        private void Challenge_Date_Changed(object sender, EventArgs e)
        {
            if (!start_challenge_y)
            {
                Get_Challenge_List();
            }
        }
        private void RepDetail_ListView_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Edit History");
        }
        public string Get_Pict_URL()
        {
            if (App.local_y) { return pict_path; }
            else { return "https://tisztavaros.hu"; }
        }
        public string Corr_URL(string inpo)
        {
            if (App.local_y)
            {
                return inpo.Replace("/", "\\");
            }
            return inpo;
        }
    }
}
