using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using OTTS_WPF.Helpers;
using OTTS_WPF.Profesori;
using OTTS_WPF.Template;
using MaterialDesignThemes;

namespace OTTS_WPF
{
    /// <summary>
    /// Interaction logic for MainScreen.xaml
    /// </summary>
    public partial class MainScreen : Window
    {
        public LoginScreen SourceScreen { get; set; }
        int childCount = 0;
        public MainScreen()
        {
            InitializeComponent();
        }

        private void ButtonLogout_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            SourceScreen.Show();
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

        private void ButtonRaiseDownMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonRaiseDownMenu.Visibility = Visibility.Collapsed;
            ButtonLowerDownMenu.Visibility = Visibility.Visible;
        }

        private void ButtonLowerDownMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonRaiseDownMenu.Visibility = Visibility.Visible;
            ButtonLowerDownMenu.Visibility = Visibility.Collapsed;
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
                switch (item.Name)
                {
                    case "Planning":
                        break;
                    case "Teachers":
                        WindowProfesoriColectie wind = new WindowProfesoriColectie();
                        wind.MainScreen = this;
                        CreateTabItem(wind);
                        break;
                    case "Groups":
                        break;
                    case "Lectures":
                        break;
                    case "Modules":
                        break;
                    case "Days":
                        break;
                    case "Halls":
                        break;
                    case "Settings":
                        break;
                    default:
                        break;
                }                
            }
        }

        public void RaiseDownMenu(Window window,EnumWindowType WindowType)
        {
            if (WindowType == EnumWindowType.ADDMODE)
            {
                LabelDownMenu.Content = window.Title + " Add";
            }
            else if (WindowType == EnumWindowType.EDITMODE)
            {
                LabelDownMenu.Content = window.Title + " Edit";
            }
            //click the button
            ButtonRaiseDownMenu.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            //set the content
            GridDownMenuContent.Content = window.Content;
        }

        public void RaiseDownMenu(Window window)
        {
            LabelDownMenu.Content = window.Title;
            //click the button
            ButtonRaiseDownMenu.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            //set the content
            GridDownMenuContent.Content = window.Content;
        }

        public void LowerDownMenu()
        {
            Storyboard sb = this.FindResource("CloseDownMenu") as Storyboard;
            if (sb != null) { BeginStoryboard(sb); }
            LabelDownMenu.Content = "";
            GridDownMenuContent.Content = null;
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
