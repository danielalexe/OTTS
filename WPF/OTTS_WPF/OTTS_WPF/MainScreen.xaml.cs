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
using OTTS_WPF.Teachers;
using OTTS_WPF.Template;
using MaterialDesignThemes;
using OTTS_WPF.Halls;
using OTTS_WPF.Lectures;
using OTTS_WPF.Modules;
using OTTS_WPF.Days;
using OTTS_WPF.Groups;
using OTTS_WPF.Semigroups;
using OTTS_WPF.TeachersLectures;
using OTTS_WPF.Planning;
using DataLink;
using DataObjects;

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
            BindComboBoxSemester();
            ComboBoxSemester.DropDownClosed += ComboBoxSemester_DropDownClosed;
        }

        private void ComboBoxSemester_DropDownClosed(object sender, EventArgs e)
        {
            PersistentData.SelectedSemester = ((DTOSemester)ComboBoxSemester.SelectedItem).iID_SEMESTER;
            //Close entity ones
            LowerDownMenu();

            List<string> OpenedTabs = new List<string>();
            //Must redraw all tabbed screens
            if (TabControlMain.Items.Count > 0)
            {
                foreach (TabItem item in TabControlMain.Items)
                {
                    OpenedTabs.Add(item.Header.ToString().Replace(" ",""));
                }
            }
            var SelectedIndex = TabControlMain.SelectedIndex;
            TabControlMain.Items.Clear();
            foreach (var item in OpenedTabs)
            {
                OpenWindow(item);
            }
            TabControlMain.SelectedItem = TabControlMain.Items[SelectedIndex];
        }

        private void BindComboBoxSemester()
        {
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                var getSemesters = (from u in db.SEMESTERS
                                    where u.bACTIVE==true
                                    select new DTOSemester
                                    {
                                        iID_SEMESTER = u.iID_SEMESTER,
                                        nvNAME = u.nvNAME,
                                        nvCOMBO_DISPLAY = u.nvNAME
                                    }).ToList();
                ComboBoxSemester.ItemsSource = getSemesters;
                ComboBoxSemester.SelectedItem = getSemesters.FirstOrDefault();
                PersistentData.SelectedSemester = getSemesters.FirstOrDefault().iID_SEMESTER;
            }
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

        private void OpenWindow(string WindowName)
        {
            switch (WindowName)
            {
                case "Planning":
                    WindowPlanningCollection winPlanning = new WindowPlanningCollection();
                    winPlanning.MainScreen = this;
                    CreateTabItem(winPlanning);
                    break;
                case "Teachers":
                    WindowTeachersCollection winTeachers = new WindowTeachersCollection();
                    winTeachers.MainScreen = this;
                    CreateTabItem(winTeachers);
                    break;
                case "TeachersLectures":
                    WindowTeachersLecturesCollection winTeachersLectures = new WindowTeachersLecturesCollection();
                    winTeachersLectures.MainScreen = this;
                    CreateTabItem(winTeachersLectures);
                    break;
                case "Groups":
                    WindowGroupsCollection winGroups = new WindowGroupsCollection();
                    winGroups.MainScreen = this;
                    CreateTabItem(winGroups);
                    break;
                case "Semigroups":
                    WindowSemigroupsCollection winSemigroups = new WindowSemigroupsCollection();
                    winSemigroups.MainScreen = this;
                    CreateTabItem(winSemigroups);
                    break;
                case "Lectures":
                    WindowLecturesCollection winLectures = new WindowLecturesCollection();
                    winLectures.MainScreen = this;
                    CreateTabItem(winLectures);
                    break;
                case "Modules":
                    WindowModulesCollection winModules = new WindowModulesCollection();
                    winModules.MainScreen = this;
                    CreateTabItem(winModules);
                    break;
                case "Days":
                    WindowDaysCollection winDays = new WindowDaysCollection();
                    winDays.MainScreen = this;
                    CreateTabItem(winDays);
                    break;
                case "Halls":
                    WindowHallsCollection winHalls = new WindowHallsCollection();
                    winHalls.MainScreen = this;
                    CreateTabItem(winHalls);
                    break;
                default:
                    break;
            }
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            if (item!=null)
            {
                OpenWindow(item.Name);
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

        private void ButtonAboutUs_Click(object sender, RoutedEventArgs e)
        {
            AboutUs aboutUs = new AboutUs();
            aboutUs.MainScreen = this;
            CreateTabItem(aboutUs);
        }
    }
}
