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
using ARPC_WPF.Profesori;
using ARPC_WPF.Template;
using MaterialDesignThemes;

namespace ARPC_WPF
{
    /// <summary>
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainScreen : Window
    {
        int childCount = 0;
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

        private void CreateTabItem(WindowBase window)
        {
            //child window will now hold a reference value to the tab control

            window.TabCtrl = TabControlMain;

            //Add a Tabitem and enables it
            TabItem tp = new TabItem();
            tp.Header = window.Title;
            tp.Content = window.Content;
            TabControlMain.Items.Add(tp);

            //child Form will now hold a reference value to a tabitem
            window.TabItem = tp;

            //Activate the MDI child form

            childCount++;

            //Activate the newly created Tabitem
            TabControlMain.SelectedItem = tp;
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item!=null)
            {
                WindowProfesoriColectie wind = new WindowProfesoriColectie();
                CreateTabItem(wind);
            }
        }
    }
}
