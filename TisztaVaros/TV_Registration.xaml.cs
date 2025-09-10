using System;
using System.Collections.Generic;
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
    /// Interaction logic for TV_Registration.xaml
    /// </summary>
    public partial class TV_Registration : Window
    {
        ServerConnection connection;
        List<TextBox> tb_list = new List<TextBox>();
        List<TextBox> psw_list = new List<TextBox>();
        List<Label> l_list = new List<Label>();
        List<Object> b_list = new List<Object>();
        string n_txt;
        string[] a_psw = { "", "" }, t_psw = { "", "" };
        bool[] hide_y = { true, true };
        public TV_Registration()
        {
            InitializeComponent();
            connection = new ServerConnection();
            LocationChanged += new EventHandler(Win_Mozog);
            Inp_Name.Focus();
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
        private async void Server_Registration(object sender, RoutedEventArgs e)
        {
            if (Inp_Name.Text == "" && Inp_Email.Text == "" && a_psw[0] == "" && a_psw[1] == "")
            {
                Inp_Name.Focus();
            }
            else if (a_psw[0] != a_psw[1])
            {
                MessageBox.Show("The passwords do not match!", "User Registration:");
                Inp_PSW.Focus();
            }
            else
            {
                Message regMessage = await connection.RegisterUser(Inp_Name.Text, Inp_Email.Text, a_psw);
                MessageBox.Show(regMessage.message, "User Registration:");
                if (ServerConnection.reg_ok)
                {
                    this.Close();
                }
            }
        }
        private void T_Changed(object sender, RoutedEventArgs e)
        {
            TextBox a_TB = sender as TextBox;
            int a = tb_list.IndexOf(a_TB), b;
            var margin = l_list[a].Margin;
            if (a_TB.Text == "")
            {
                margin.Left = 40;
                margin.Top = 15;
            }
            else
            {
                margin.Left = 22;
                margin.Top = 1;
                if (a > 1)
                {
                    b = a - 2;
                    t_psw[b] = psw_list[b].Text;
                    if (hide_y[b])
                    {
                        if (t_psw[b].Length > a_psw[b].Length)
                        {
                            string n_txt = t_psw[b].Replace("*", "");
                            a_psw[b] = a_psw[b] + n_txt;
                        }
                        else
                        {
                            a_psw[b] = a_psw[b].Substring(0, t_psw[b].Length);
                        }
                        psw_list[b].Text = "*****************".Substring(0, a_psw[b].Length);

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
        private void PswOnOff(object sender, RoutedEventArgs e)
        {
            int b = b_list.IndexOf(sender) % 2;
            hide_y[b] = !hide_y[b];
            if (hide_y[b])
            {
                psw_list[b].Text = "*****************".Substring(0, a_psw[b].Length);
            }
            else
            {
                psw_list[b].Text = a_psw[b];
            }
        }
        private void Win_Mozog(object sender, EventArgs e)
        {
            MainWindow.a_top = this.Top;
            MainWindow.a_left = this.Left;
        }
    }
}
