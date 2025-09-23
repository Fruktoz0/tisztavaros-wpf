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
    /// Interaction logic for TV_Show_Picture.xaml
    /// </summary>
    public partial class TV_Show_Picture : Window
    {
        int a_idx, m_idx;
        public TV_Show_Picture()
        {
            InitializeComponent();

            this.KeyDown += Report_BigPictKeyDown;
            this.KeyUp += Report_BigPictKeyUp;
            a_idx = TV_Admin.view_rep_pict;
            m_idx = TV_Admin.a_img_urls.Count;
            Report_BigPict.Source = new BitmapImage(new Uri(TV_Admin.rep_pict_url + TV_Admin.a_img_urls[a_idx]));
            Button_PrevNext_OnOff();
        }
        private void BigPict_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void View_ReportBigPict_Prev(object sender, RoutedEventArgs e)
        {
            if (a_idx > 0)
            {
                a_idx--;
                Report_BigPict.Source = new BitmapImage(new Uri(TV_Admin.rep_pict_url + TV_Admin.a_img_urls[a_idx]));
                Button_PrevNext_OnOff();
            }
        }

        private void View_ReportBigPict_Next(object sender, RoutedEventArgs e)
        {
            if (a_idx < m_idx - 1)
            {
                a_idx++;
                Report_BigPict.Source = new BitmapImage(new Uri(TV_Admin.rep_pict_url + TV_Admin.a_img_urls[a_idx]));
                Button_PrevNext_OnOff();
            }
        }
        private void Button_PrevNext_OnOff()
        {
            if (m_idx == 1)
            {
                B_Prev_Pict.Visibility = Visibility.Hidden;
                B_Next_Pict.Visibility = Visibility.Hidden;
            }
            else if (a_idx == 0)
            {
                B_Prev_Pict.Visibility = Visibility.Hidden;
                B_Next_Pict.Visibility = Visibility.Visible;
            }
            else if (0 <= a_idx && a_idx < m_idx - 1)
            {
                B_Prev_Pict.Visibility = Visibility.Visible;
                B_Next_Pict.Visibility = Visibility.Visible;
            }
            else
            {
                B_Prev_Pict.Visibility = Visibility.Visible;
                B_Next_Pict.Visibility = Visibility.Hidden;
            }
        }
        private async void Report_BigPictKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left) { View_ReportBigPict_Prev(sender, e); Rep_Cursor_OFF(); TV_Login.aWindow.Rep_Control_Over_Left.Visibility = Visibility.Visible; return; }
            if (e.Key == Key.Right) { View_ReportBigPict_Next(sender, e); Rep_Cursor_OFF(); TV_Login.aWindow.Rep_Control_Over_Right.Visibility = Visibility.Visible; return; }
        }
        private void Report_BigPictKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.RightCtrl) || e.Key.Equals(Key.Escape)) { this.Close(); }
        }
        private void Rep_Cursor_OFF()
        {
            TV_Login.aWindow.Rep_Control_Over_Left.Visibility = Visibility.Hidden;
            TV_Login.aWindow.Rep_Control_Over_Right.Visibility = Visibility.Hidden;
            TV_Login.aWindow.Rep_Control_Over_Up.Visibility = Visibility.Hidden;
            TV_Login.aWindow.Rep_Control_Over_Down.Visibility = Visibility.Hidden;
        }
    }
}
