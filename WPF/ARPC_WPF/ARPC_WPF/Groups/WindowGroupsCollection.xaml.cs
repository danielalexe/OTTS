using DataLink;
using DataObjects;
using OTTS_WPF.Helpers;
using OTTS_WPF.Template;
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

namespace OTTS_WPF.Groups
{
    /// <summary>
    /// Interaction logic for WindowGroupsCollection.xaml
    /// </summary>
    public partial class WindowGroupsCollection : WindowBase
    {
        public MainScreen MainScreen { get; set; }
        public WindowGroupsCollection()
        {
            InitializeComponent();
            BindComboGroupType();
            ReloadData();
        }
        private void BindComboGroupType()
        {
            List<DTOGroupType> list = new List<DTOGroupType>();
            DTOGroupType dto = new DTOGroupType();
            dto.iID_GROUP_TYPE = -1;
            dto.nvCOMBO_DISPLAY = "All";
            list.Add(dto);
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                var getGroupTypes = (from u in db.GROUP_TYPES
                                     where u.bACTIVE == true
                                     select new DTOGroupType
                                     {
                                         iID_GROUP_TYPE = u.iID_GROUP_TYPE,
                                         nvNAME = u.nvNAME,
                                         nvCOMBO_DISPLAY = u.nvNAME
                                     }).ToList();
                list.AddRange(getGroupTypes);
            }
            CComboGroupType.CComboBox.ItemsSource = list;
            CComboGroupType.CComboBox.SelectedItem = list.FirstOrDefault();
        }

        private void ButonClose_Click(object sender, RoutedEventArgs e)
        {
            this.TabCtrl.Items.Remove(this.TabItem);
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            ReloadData();
        }

        public void ReloadData()
        {
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                var FilterName = CTextName.CString.ToUpper();
                var FilterGroupType = (DTOGroupType)CComboGroupType.CComboBox.SelectedItem;
                var FilterNumberStudents = CDecimalNumberStudents.CValue.Value;
                var FilterYear = CDecimalYear.CValue.Value;
                var getGroups = (from u in db.GROUPS
                               where (
                               (String.IsNullOrEmpty(FilterName) || u.nvNAME.Contains(FilterName))
                               &&
                               (FilterGroupType.iID_GROUP_TYPE==-1 || u.iID_GROUP_TYPE == FilterGroupType.iID_GROUP_TYPE)
                               &&
                               (FilterNumberStudents==0 || u.iNUMBER_OF_STUDENTS>=FilterNumberStudents)
                               &&
                               (FilterYear==0 || u.iYEAR == FilterYear)
                               &&
                               u.bACTIVE == true)
                               select new DTOGroup
                               {
                                   iID_GROUP = u.iID_GROUP,
                                   NAME = u.nvNAME,
                                   NUMBER_OF_STUDENTS = u.iNUMBER_OF_STUDENTS,
                                   YEAR = u.iYEAR,
                                   iID_GROUP_TYPE = u.iID_GROUP_TYPE
                               }).ToList();
                foreach (var item in getGroups)
                {
                    var getGroupType = db.GROUP_TYPES.FirstOrDefault(z => z.iID_GROUP_TYPE == item.iID_GROUP_TYPE);
                    if (getGroupType!=null)
                    {
                        item.GROUP_TYPE = getGroupType.nvNAME;
                    }
                }
                DataGridGroups.ItemsSource = getGroups;
                RenderColumns();
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var list = DataGridGroups.SelectedItems;
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
                            foreach (DTOGroup item in list)
                            {
                                var getGroup = db.GROUPS.FirstOrDefault(z => z.iID_GROUP == item.iID_GROUP && z.bACTIVE == true);
                                if (getGroup != null)
                                {
                                    getGroup.bACTIVE = false;
                                }
                            }
                            db.SaveChanges();
                        }
                        ReloadData();
                    }
                }
            }
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {

            var list = DataGridGroups.SelectedItems;
            if (list.Count == 0)
            {
                MessageBox.Show("Nici un rand nu este selectat");
                return;
            }
            else
            {
                if (list.Count > 1)
                {
                    MessageBox.Show("Modificarile asupra unei grupe se poate face doar la o singura grupa simultan");
                    return;
                }
                else
                {
                    WindowGroupsEntity groupsEntity = new WindowGroupsEntity();
                    groupsEntity.MainScreen = MainScreen;
                    groupsEntity.WindowType = Helpers.EnumWindowType.EDITMODE;
                    groupsEntity.ID_GROUP = ((DTOGroup)DataGridGroups.SelectedItem).iID_GROUP;
                    groupsEntity.WindowGroupsCollection = this;
                    groupsEntity.LoadData();
                    MainScreen.RaiseDownMenu(groupsEntity, Helpers.EnumWindowType.EDITMODE);
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            WindowGroupsEntity groupsEntity = new WindowGroupsEntity();
            groupsEntity.MainScreen = MainScreen;
            groupsEntity.WindowType = Helpers.EnumWindowType.ADDMODE;
            groupsEntity.WindowGroupsCollection = this;
            MainScreen.RaiseDownMenu(groupsEntity, Helpers.EnumWindowType.ADDMODE);
        }


        private void RenderColumns()
        {
            foreach (DataGridColumn c in DataGridGroups.Columns)
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

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            RenderColumns();
        }

        private void ButtonModules_Click(object sender, RoutedEventArgs e)
        {
            var list = DataGridGroups.SelectedItems;
            if (list.Count == 0)
            {
                MessageBox.Show("Nici un rand nu este selectat");
                return;
            }
            else
            {
                if (list.Count > 1)
                {
                    MessageBox.Show("Modulele se pot aloca doar la o grupa simultan");
                    return;
                }
                else
                {
                    WindowGroupsModules groupsModules = new WindowGroupsModules();
                    groupsModules.MainScreen = MainScreen;
                    groupsModules.ID_GROUP = ((DTOGroup)DataGridGroups.SelectedItem).iID_GROUP;
                    groupsModules.WindowGroupsCollection = this;
                    MainScreen.RaiseDownMenu(groupsModules);
                }
            }
        }

        private void ButtonLectures_Click(object sender, RoutedEventArgs e)
        {
            var list = DataGridGroups.SelectedItems;
            if (list.Count == 0)
            {
                MessageBox.Show("Nici un rand nu este selectat");
                return;
            }
            else
            {
                if (list.Count > 1)
                {
                    MessageBox.Show("Prelegerile se pot aloca doar la o grupa simultan");
                    return;
                }
                else
                {
                    WindowGroupsLectures groupsLectures = new WindowGroupsLectures();
                    groupsLectures.MainScreen = MainScreen;
                    groupsLectures.ID_GROUP = ((DTOGroup)DataGridGroups.SelectedItem).iID_GROUP;
                    groupsLectures.WindowGroupsCollection = this;
                    MainScreen.RaiseDownMenu(groupsLectures);
                }
            }
        }
    }
}
