using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TisztaVaros
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        List<TextBox> tb_list = new List<TextBox>();
        List<Label> l_list = new List<Label>();
        bool hide_y = true;
        string a_psw = "", t_psw, max_star = "*****************";
        public Login()
        {
            InitializeComponent();
            tb_list.Add(Inp_Email);
            tb_list.Add(Inp_PSW);
            l_list.Add(txtEmailPlaceholder);
            l_list.Add(txtPswPlaceholder);
        }
        void Server_Login(object sender, RoutedEventArgs e)
        {
            if (PreCheck_Login())
            {
                MessageBox.Show("Na mehet a Login!");
            }
        }
        // Előellenőrzés
        private bool PreCheck_Login()
        {
            Inp_Email.Text = Inp_Email.Text.Trim();
            if (!ValidEmail.IsValid(Inp_Email.Text))
            {
                MessageBox.Show("Érvénytelen email cím!");
                return false;
            }
            if (a_psw.Length < 6)
            {
                MessageBox.Show("Túl rövid jelszó!");
                return false;
            }
            return true;
        }
        // Placeholder animáció
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
                        a_psw = a_psw + t_psw.Replace("*", "");
                    }
                    else
                    {
                        a_psw = a_psw.Substring(0, t_psw.Length);
                    }
                    Inp_PSW.Text = max_star.Substring(0, a_psw.Length);
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
        // Jelszó mutatása/elrejtése
        private void PswOnOff(object sender, RoutedEventArgs e)
        {
            hide_y = !hide_y;
            if (hide_y)
            {
                Inp_PSW.Text = max_star.Substring(0, a_psw.Length);
            }
            else
            {
                Inp_PSW.Text = a_psw;
            }
        }
    }
}
