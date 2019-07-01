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
using MaterialDesignThemes;

namespace ARPC_WPF
{
    /// <summary>
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainScreen : Window
    {
        public MainScreen()
        {
            InitializeComponent();
        }

        private void ButtonLogout_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ButtonOpenDrawer_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenDrawer.Visibility = Visibility.Collapsed;
            ButtonCloseDrawer.Visibility = Visibility.Visible;
        }

        private void ButtonCloseDrawer_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenDrawer.Visibility = Visibility.Visible;
            ButtonCloseDrawer.Visibility = Visibility.Collapsed;
        }
    }
}
