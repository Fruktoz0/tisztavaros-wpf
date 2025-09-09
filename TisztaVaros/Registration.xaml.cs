using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        List<TextBox> tb_list = new List<TextBox>();
        List<TextBox> psw_list = new List<TextBox>();
        List<Label> l_list = new List<Label>();
        List<Object> b_list = new List<Object>();
        string max_star = "*****************";
        string[] a_psw = { "", "" }, t_psw = { "", "" };
        bool[] hide_y = { true, true };
        public Registration()
        {
            InitializeComponent();

            tb_list.Add(Inp_Name);
            tb_list.Add(Inp_Email);
            tb_list.Add(Inp_PSW);
            tb_list.Add(Inp_PSW2);

            psw_list.Add(Inp_PSW);
            psw_list.Add(Inp_PSW2);

            b_list.Add(PictView1);
            b_list.Add(PictView2);

            l_list.Add(txtUserPlaceholder);
            l_list.Add(txtEmailPlaceholder);
            l_list.Add(txtPswPlaceholder);
            l_list.Add(txtPsw2Placeholder);
        }
        void Server_Registration(object sender, RoutedEventArgs e)
        {
            if (PreCheck_Reg())
            {
                MessageBox.Show("Na mehet a regisztráció!");
            }
        }
        // Előellenőrzés
        private bool PreCheck_Reg()
        {
            Inp_Name.Text = Inp_Name.Text.Trim();
            if (Inp_Name.Text.Length < 6)
            {
                MessageBox.Show("Túl rövid a felhasználónév!");
                return false;
            }

            Inp_Email.Text = Inp_Email.Text.Trim();
            if (!ValidEmail.IsValid(Inp_Email.Text))
            {
                MessageBox.Show("Érvénytelen email cím!");
                return false;
            }

            if (a_psw[0] != a_psw[1])
            {
                MessageBox.Show("Nem egyezik a két jelszó!");
                return false;
            }
            if (a_psw[0] == "" || a_psw[1] == "")
            {
                MessageBox.Show("Üres jelszó nem lehet!");
                return false;
            }
            if (a_psw[0].Length < 6 || a_psw[1].Length < 6)
            {
                MessageBox.Show("Túl rövid jelszó!");
                return false;
            }
            return true;
        }
        // Placeholder animáció
        private void T_Changed(object sender, RoutedEventArgs e)
        {
            TextBox a_tb= sender as TextBox;
            int a = tb_list.IndexOf(a_tb), b;
            var margin = l_list[a].Margin;
            if (a_tb.Text == "")
            {
                margin.Left = 40;
                margin.Top = 15;
                l_list[a].FontSize = 12;
            }
            else
            {
                //margin.Left = 40;
                //margin.Top = -26;
                margin.Left = 20;
                margin.Top = -6;
                l_list[a].FontSize = 10;

                // var bc = new BrushConverter();
                // l_list[a].Background = (Brush)bc.ConvertFrom("#FFFDF5");
                // l_list[a].Opacity = 1;
                // l_list[a].Background = Brushes.Blue;
                if (a > 1)
                {
                    b = a - 2;
                    t_psw[b] = psw_list[b].Text;
                    if (hide_y[b])
                    {
                        if (t_psw[b].Length > a_psw[b].Length)
                        {
                            a_psw[b] = a_psw[b] + t_psw[b].Replace("*", "");
                        }
                        else
                        {
                            a_psw[b] = a_psw[b].Substring(0, t_psw[b].Length);
                        }
                        psw_list[b].Text = "****************".Substring(0, a_psw[b].Length);

                    }
                    else
                    {
                        a_psw[b] = t_psw[b];
                        psw_list[b].Text = a_psw[b];
                    }
                    psw_list[b].Select(a_psw[b].Length, 0);
                }
            }
            l_list[a].Margin = margin;
        }
        // Jelszó mutatása/elrejtése
        private void PswOnOff(object sender, RoutedEventArgs e)
        {
            int b = b_list.IndexOf(sender) % 2;
            hide_y[b] = !hide_y[b];
            if (hide_y[b])
            {
                psw_list[b].Text = "****************".Substring(0, a_psw[b].Length);
            }
            else
            {
                psw_list[b].Text = a_psw[b];
            }
        }
    }
}
