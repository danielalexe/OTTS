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
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OTTS_WPF.Groups
{
    /// <summary>
    /// Interaction logic for WindowGroupsModulesEntity.xaml
    /// </summary>
    public partial class WindowGroupsModulesEntity : Window
    {
        public WindowGroupsModules WindowGroupsModules { get; set; }
        public MainScreen MainScreen { get; set; }
        public EnumWindowType WindowType { get; set; }
        public int ID_GROUP { get; set; }
        public WindowGroupsModulesEntity()
        {
            InitializeComponent();
            LoadData();
        }

        public void CloseWindow()
        {
            WindowGroupsModules.LowerDownMenu();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        public void LoadData()
        {
            List<DTOModule> list = new List<DTOModule>();
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                var getGroupModule = (from u in db.MODULES
                                      where u.bACTIVE == true
                                      &&
                                      !db.GROUPS_MODULES_LINK.Any(z => z.iID_GROUP == ID_GROUP && z.iID_MODULE == u.iID_MODULE && z.bACTIVE == true)
                                      select new DTOModule
                                      {
                                          iID_MODULE=u.iID_MODULE,
                                          NAME = u.nvNAME
                                      }).ToList();
                list.AddRange(getGroupModule);
            }
            DataGridModules.ItemsSource = list;
            RenderColumns();
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var list = DataGridModules.SelectedItems;
            if (list.Count == 0)
            {
                MessageBox.Show("Nici un rand nu este selectat");
                return;
            }
            else
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    foreach (DTOModule item in list)
                    {
                        GROUPS_MODULES_LINK gml = new GROUPS_MODULES_LINK();
                        gml.iID_GROUP = ID_GROUP;
                        gml.iID_MODULE = item.iID_MODULE;

                        gml.bACTIVE = true;
                        gml.iCREATE_USER = PersistentData.LoggedUser.iID_USER;
                        gml.dtCREATE_DATE = DateTime.UtcNow;

                        db.GROUPS_MODULES_LINK.Add(gml);
                        db.SaveChanges();
                    }
                }
                WindowGroupsModules.LoadData();
                CloseWindow();
            }

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

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
