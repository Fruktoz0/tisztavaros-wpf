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
    /// Interaction logic for InputBox.xaml
    /// </summary>
    public partial class InputBox : Window
    {
        public InputBox()
        {
            InitializeComponent();
            Vmi_Password.Focus();
           }

        private void Get_PSW_OK(object sender, RoutedEventArgs e)
        {
            string a_psw = Vmi_Password.Text;
            if (a_psw.Length < 6)   
            {
                MessageBox.Show("A jelszónak legalább 6 karakter hosszúnak kell lennie!", "Hiba!");    
            }
            else
            {
                TV_Admin.new_vmi_psw = Vmi_Password.Text;
                this.Close();
            }
        }

        private void Get_PSW_Cancel(object sender, RoutedEventArgs e)
        {
            TV_Admin.new_vmi_psw = "xx";
            this.Close();
        }
    }
}
