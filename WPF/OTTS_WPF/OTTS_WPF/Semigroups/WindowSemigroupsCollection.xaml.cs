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

namespace OTTS_WPF.Semigroups
{
    /// <summary>
    /// Interaction logic for WindowSemigroupsCollection.xaml
    /// </summary>
    public partial class WindowSemigroupsCollection : WindowBase
    {
        public MainScreen MainScreen { get; set; }
        public WindowSemigroupsCollection()
        {
            InitializeComponent();
            BindComboGroup();
            BindComboPriority();
            ReloadData();
            CComboGroup.CComboBox.SelectionChanged += CComboBoxGroup_SelectionChanged;
            CComboPriority.CComboBox.SelectionChanged += CComboBoxPriority_SelectionChanged;
        }

        private void CComboBoxPriority_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReloadData();
        }

        private void CComboBoxGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReloadData();
        }

        private void BindComboGroup()
        {
            List<DTOGroup> list = new List<DTOGroup>();
            DTOGroup dto = new DTOGroup();
            dto.iID_GROUP = -1;
            dto.nvCOMBO_DISPLAY = "All";
            list.Add(dto);
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                var getGroup = (from u in db.GROUPS
                                     where u.bACTIVE == true
                                     select new DTOGroup
                                     {
                                         iID_GROUP = u.iID_GROUP,
                                         NAME = u.nvNAME,
                                         nvCOMBO_DISPLAY = u.nvNAME
                                     }).ToList();
                list.AddRange(getGroup);
            }
            CComboGroup.CComboBox.ItemsSource = list;
            CComboGroup.CComboBox.SelectedItem = list.FirstOrDefault();
        }

        private void BindComboPriority()
        {
            List<DTOPlaceholderCombo> list = new List<DTOPlaceholderCombo>();
            var enumlist = Enum.GetValues(typeof(EnumPriorityFilter)).Cast<EnumPriorityFilter>();
            foreach (var item in enumlist)
            {
                DTOPlaceholderCombo dto = new DTOPlaceholderCombo();
                dto.nvCOMBO_DISPLAY = item.ToString();
                list.Add(dto);
            }
            CComboPriority.CComboBox.ItemsSource = list;
            CComboPriority.CComboBox.SelectedItem = list.FirstOrDefault();
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
            List<DTOSemigroup> list = new List<DTOSemigroup>();
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                var FilterName = CTextName.CString.ToUpper();
                var FilterGroup = (DTOGroup)CComboGroup.CComboBox.SelectedItem;
                var FilterPriority = (DTOPlaceholderCombo)CComboPriority.CComboBox.SelectedItem;
                switch (FilterPriority.nvCOMBO_DISPLAY)
                {
                    case "None":
                        var getSemigroups = (from u in db.SEMIGROUPS
                                             where (
                                             (String.IsNullOrEmpty(FilterName) || u.nvNAME.Contains(FilterName))
                                             &&
                                             (FilterGroup.iID_GROUP == -1 || u.iID_GROUP == FilterGroup.iID_GROUP)
                                             &&
                                             u.bACTIVE == true)
                                             select new DTOSemigroup
                                             {
                                                 iID_GROUP = u.iID_GROUP,
                                                 iID_SEMIGROUP = u.iID_SEMIGROUP,
                                                 GROUP_NAME = u.GROUPS.nvNAME,
                                                 SEMIGROUP_NAME = u.nvNAME,
                                                 PRIORITY = u.iPRIORITY
                                             }).ToList();
                        list.AddRange(getSemigroups);
                        break;
                    case "Ascendant":
                        var getSemigroupsAscendant = (from u in db.SEMIGROUPS
                                             where (
                                             (String.IsNullOrEmpty(FilterName) || u.nvNAME.Contains(FilterName))
                                             &&
                                             (FilterGroup.iID_GROUP == -1 || u.iID_GROUP == FilterGroup.iID_GROUP)
                                             &&
                                             u.bACTIVE == true)
                                             select new DTOSemigroup
                                             {
                                                 iID_GROUP = u.iID_GROUP,
                                                 iID_SEMIGROUP = u.iID_SEMIGROUP,
                                                 GROUP_NAME = u.GROUPS.nvNAME,
                                                 SEMIGROUP_NAME = u.nvNAME,
                                                 PRIORITY = u.iPRIORITY
                                             }).OrderBy(z=>z.PRIORITY).ToList();
                        list.AddRange(getSemigroupsAscendant);
                        break;
                    case "Descendant":
                        var getSemigroupsDescendant = (from u in db.SEMIGROUPS
                                                      where (
                                                      (String.IsNullOrEmpty(FilterName) || u.nvNAME.Contains(FilterName))
                                                      &&
                                                      (FilterGroup.iID_GROUP == -1 || u.iID_GROUP == FilterGroup.iID_GROUP)
                                                      &&
                                                      u.bACTIVE == true)
                                                      select new DTOSemigroup
                                                      {
                                                          iID_GROUP = u.iID_GROUP,
                                                          iID_SEMIGROUP = u.iID_SEMIGROUP,
                                                          GROUP_NAME = u.GROUPS.nvNAME,
                                                          SEMIGROUP_NAME = u.nvNAME,
                                                          PRIORITY = u.iPRIORITY
                                                      }).OrderByDescending(z => z.PRIORITY).ToList();
                        list.AddRange(getSemigroupsDescendant);
                        break;
                    default:
                        break;
                }
                
            }
            DataGridSemigroups.ItemsSource = list;
            RenderColumns();
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var list = DataGridSemigroups.SelectedItems;
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
                            foreach (DTOSemigroup item in list)
                            {
                                var getSemigroup = db.SEMIGROUPS.FirstOrDefault(z => z.iID_SEMIGROUP == item.iID_SEMIGROUP && z.bACTIVE == true);
                                if (getSemigroup != null)
                                {
                                    getSemigroup.bACTIVE = false;
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

            var list = DataGridSemigroups.SelectedItems;
            if (list.Count == 0)
            {
                MessageBox.Show("Nici un rand nu este selectat");
                return;
            }
            else
            {
                if (list.Count > 1)
                {
                    MessageBox.Show("Modificarile asupra unei semigrupe se poate face doar la o singura semigrupa simultan");
                    return;
                }
                else
                {
                    WindowSemigroupsEntity semigroupsEntity = new WindowSemigroupsEntity();
                    semigroupsEntity.MainScreen = MainScreen;
                    semigroupsEntity.WindowType = Helpers.EnumWindowType.EDITMODE;
                    semigroupsEntity.ID_SEMIGROUP = ((DTOSemigroup)DataGridSemigroups.SelectedItem).iID_SEMIGROUP;
                    semigroupsEntity.WindowSemigroupsCollection = this;
                    semigroupsEntity.LoadData();
                    MainScreen.RaiseDownMenu(semigroupsEntity, Helpers.EnumWindowType.EDITMODE);
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            WindowSemigroupsEntity semigroupsEntity = new WindowSemigroupsEntity();
            semigroupsEntity.MainScreen = MainScreen;
            semigroupsEntity.WindowType = Helpers.EnumWindowType.ADDMODE;
            semigroupsEntity.WindowSemigroupsCollection = this;
            MainScreen.RaiseDownMenu(semigroupsEntity, Helpers.EnumWindowType.ADDMODE);
        }


        private void RenderColumns()
        {
            foreach (DataGridColumn c in DataGridSemigroups.Columns)
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

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            var HelpText = "\tIn acest meniu puteti defini semigrupele si sa le alocati prioritatea la planificare.\r\n" +
                "\tPrioritatile sunt in ordine crescatoare si astfel prioritatea 0 este cea mai mare.\r\n";
            HelpScreen help = new HelpScreen();
            help.TitleHelp.Text = "Semigroups Help";
            help.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            help.TextHelp.Text = HelpText;
            help.Show();
        }
    }
}
