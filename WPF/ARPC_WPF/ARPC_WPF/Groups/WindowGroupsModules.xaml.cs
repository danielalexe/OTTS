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
    /// Interaction logic for WindowGroupsModules.xaml
    /// </summary>
    public partial class WindowGroupsModules : Window
    {
        public WindowGroupsCollection WindowGroupsCollection { get; set; }
        public MainScreen MainScreen { get; set; }
        public int ID_GROUP { get; set; }
        public WindowGroupsModules()
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
            List<DTOGroupModule> list = new List<DTOGroupModule>();
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                var getGroupModules = (from u in db.GROUPS_MODULES_LINK
                                       where 
                                       u.iID_GROUP == ID_GROUP
                                       &&
                                       u.bACTIVE == true
                                       select new DTOGroupModule
                                       {
                                           iID_GROUP = u.iID_GROUP,
                                           iID_MODULE = u.iID_MODULE,
                                           MODULE_NAME = u.MODULES.nvNAME
                                       }).ToList();
                list.AddRange(getGroupModules);
            }
            DataGridModules.ItemsSource = list;
            RenderColumns();
        }

        private void RenderColumns()
        {
            foreach (DataGridColumn c in DataGridModules.Columns)
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

        private void DataGridModules_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() != "PRIORITY")
            {
                e.Column.IsReadOnly = true;
            }
        }

        private void DataGridModules_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var list = DataGridModules.SelectedItems;
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
                            foreach (DTOGroupModule item in list)
                            {
                                var getGroupModule = db.GROUPS_MODULES_LINK.FirstOrDefault(z => z.iID_GROUP == item.iID_GROUP && z.iID_MODULE == item.iID_MODULE && z.bACTIVE == true);
                                if (getGroupModule != null)
                                {
                                    getGroupModule.bACTIVE = false;
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
            WindowGroupsModulesEntity groupsModulesEntity = new WindowGroupsModulesEntity();
            groupsModulesEntity.MainScreen = MainScreen;
            groupsModulesEntity.ID_GROUP = ID_GROUP;
            groupsModulesEntity.WindowType = Helpers.EnumWindowType.ADDMODE;
            groupsModulesEntity.WindowGroupsModules = this;
            this.RaiseDownMenu(groupsModulesEntity, Helpers.EnumWindowType.ADDMODE);
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
