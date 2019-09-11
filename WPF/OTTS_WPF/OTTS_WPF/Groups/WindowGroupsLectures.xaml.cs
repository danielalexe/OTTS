using DataLink;
using DataObjects;
using OTTS_WPF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace OTTS_WPF.Groups
{
    /// <summary>
    /// Interaction logic for WindowGroupsLectures.xaml
    /// </summary>
    public partial class WindowGroupsLectures : Window
    {
        public WindowGroupsCollection WindowGroupsCollection { get; set; }
        public MainScreen MainScreen { get; set; }
        public int ID_GROUP { get; set; }
        public WindowGroupsLectures()
        {
            InitializeComponent();
            LoadData();
        }

        public void CloseWindow()
        {
            MainScreen.LowerDownMenu();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        public void LoadData()
        {
            List<DTOGroupLecture> list = new List<DTOGroupLecture>();
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                var getGroupLectures = (from u in db.GROUPS_LECTURES_LINK
                                       where
                                       u.iID_GROUP == ID_GROUP
                                       &&
                                       u.iID_SEMESTER == PersistentData.SelectedSemester
                                       &&
                                       u.bACTIVE == true
                                       select new DTOGroupLecture
                                       {
                                           iID_GROUP = u.iID_GROUP,
                                           iID_LECTURE = u.iID_LECTURE,
                                           LECTURE_NAME = u.LECTURES.nvNAME
                                       }).ToList();
                list.AddRange(getGroupLectures);
            }
            DataGridLectures.ItemsSource = list;
            RenderColumns();
        }

        private void RenderColumns()
        {
            foreach (DataGridColumn c in DataGridLectures.Columns)
            {
                if (c.Header.ToString().StartsWith("iID_") || c.Header.ToString().StartsWith("nvPASSWORD") || c.Header.ToString().StartsWith("nvCOMBO_DISPLAY"))
                    c.Visibility = Visibility.Collapsed;
                String aux = c.Header.ToString().ToLower();
                c.Header = ReplaceFirstCharacterFromString(aux);
            }
        }

        public string ReplaceFirstCharacterFromString(string text)
        {
            String aux = String.Format("{0}{1}", text.First().ToString().ToUpperInvariant(), text.Substring(1));
            return aux.Replace('_', ' ');
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void DataGridLectures_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() != "PRIORITY")
            {
                e.Column.IsReadOnly = true;
            }
        }

        private void DataGridLectures_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var list = DataGridLectures.SelectedItems;
            if (list.Count == 0)
            {
                MessageBox.Show("Nici un rand nu este selectat");
                return;
            }
            else
            {
                if (list.Count > 0)
                {
                    if (MessageBox.Show("Sunteti siguri ca vreti sa stergeti randurile selectate?", "Atentie", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        using (var db = new OTTSContext(PersistentData.ConnectionString))
                        {
                            foreach (DTOGroupLecture item in list)
                            {
                                var getSelectedGroup = db.GROUPS.FirstOrDefault(z => z.bACTIVE == true && z.iID_GROUP == item.iID_GROUP);
                                var getAllSimilarGroups = (from u in db.GROUPS
                                                           where u.bACTIVE == true
                                                           &&
                                                           u.iID_GROUP_TYPE == getSelectedGroup.iID_GROUP_TYPE
                                                           &&
                                                           u.iYEAR == getSelectedGroup.iYEAR
                                                           select u).ToList();

                                foreach (var group in getAllSimilarGroups)
                                {
                                    var getGroupLecture = db.GROUPS_LECTURES_LINK.FirstOrDefault(z => z.iID_GROUP == group.iID_GROUP && z.iID_LECTURE == item.iID_LECTURE && z.bACTIVE == true);
                                    if (getGroupLecture != null)
                                    {
                                        getGroupLecture.bACTIVE = false;
                                    }
                                }
                            }
                            db.SaveChanges();
                        }
                        LoadData();
                    }
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            WindowGroupsLecturesEntity groupsLecturesEntity = new WindowGroupsLecturesEntity();
            groupsLecturesEntity.MainScreen = MainScreen;
            groupsLecturesEntity.ID_GROUP = ID_GROUP;
            groupsLecturesEntity.WindowType = Helpers.EnumWindowType.ADDMODE;
            groupsLecturesEntity.WindowGroupsLectures = this;
            this.RaiseDownMenu(groupsLecturesEntity, Helpers.EnumWindowType.ADDMODE);
        }

        public void RaiseDownMenu(Window window)
        {
            LabelDownMenu.Content = window.Title;
            //click the button
            ButtonRaiseDownMenu.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
            //set the content
            GridDownMenuContent.Content = window.Content;
        }

        public void RaiseDownMenu(Window window, EnumWindowType WindowType)
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

        public void LowerDownMenu()
        {
            Storyboard sb = this.FindResource("CloseDownMenu") as Storyboard;
            if (sb != null) { BeginStoryboard(sb); }
            LabelDownMenu.Content = "";
            GridDownMenuContent.Content = null;
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
    }
}
