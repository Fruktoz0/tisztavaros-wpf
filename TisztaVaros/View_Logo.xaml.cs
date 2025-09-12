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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TisztaVaros
{
    /// <summary>
    /// Interaction logic for View_Logo.xaml
    /// </summary>
    public partial class View_Logo : Window
    {
        string LogoUrlBackup;
        public View_Logo()
        {
            InitializeComponent();
            LogoUrlBackup = TV_Admin.inst_logo_url;
        }

        private void LogoURL_DataChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Logo_IMG.Source = new BitmapImage(new Uri(Inst_Logo_URL.Text));
            }
            catch (Exception)
            {
                Logo_IMG.Source = null;
            }
        }

        private void Try_View_Logo(object sender, RoutedEventArgs e)
        {

        }

        private void InstLogo_Save(object sender, RoutedEventArgs e)
        {
            if (Logo_IMG.Source == null)
            {
                TV_Admin.inst_logo_url = "";
            }
            else
            {
                TV_Admin.inst_logo_url = Logo_IMG.Source.ToString();
            }
            this.Close();
        }

        private void InstLogo_Cancel(object sender, RoutedEventArgs e)
        {
            TV_Admin.inst_logo_url = LogoUrlBackup;
            this.Close();
        }
    }
}
