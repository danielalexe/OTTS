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

namespace OTTS_WPF.Modules
{
    /// <summary>
    /// Interaction logic for WindowModulesCollection.xaml
    /// </summary>
    public partial class WindowModulesCollection : WindowBase
    {
        public MainScreen MainScreen { get; set; }
        public WindowModulesCollection()
        {
            InitializeComponent();
            ReloadData();
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
                var getModules = (from u in db.MODULES
                                where (
                                (String.IsNullOrEmpty(FilterName) || u.nvNAME.Contains(FilterName))
                                &&
                                u.bACTIVE == true)
                                select new DTOModule
                                {
                                    iID_MODULE = u.iID_MODULE,
                                    NAME = u.nvNAME
                                }).ToList();
                DataGridModules.ItemsSource = getModules;
                RenderColumns();
            }
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
                            foreach (DTOModule item in list)
                            {
                                var getModule = db.MODULES.FirstOrDefault(z => z.iID_MODULE == item.iID_MODULE && z.bACTIVE == true);
                                if (getModule != null)
                                {
                                    getModule.bACTIVE = false;
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

            var list = DataGridModules.SelectedItems;
            if (list.Count == 0)
            {
                MessageBox.Show("Nici un rand nu este selectat");
                return;
            }
            else
            {
                if (list.Count > 1)
                {
                    MessageBox.Show("Modificarile asupra unui modul se poate face doar la un singur modul simultan");
                    return;
                }
                else
                {
                    WindowModulesEntity modulesEntity = new WindowModulesEntity();
                    modulesEntity.MainScreen = MainScreen;
                    modulesEntity.WindowType = Helpers.EnumWindowType.EDITMODE;
                    modulesEntity.ID_MODULE = ((DTOModule)DataGridModules.SelectedItem).iID_MODULE;
                    modulesEntity.WindowModulesCollection = this;
                    modulesEntity.LoadData();
                    MainScreen.RaiseDownMenu(modulesEntity, Helpers.EnumWindowType.EDITMODE);
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            WindowModulesEntity modulesEntity = new WindowModulesEntity();
            modulesEntity.MainScreen = MainScreen;
            modulesEntity.WindowType = Helpers.EnumWindowType.ADDMODE;
            modulesEntity.WindowModulesCollection = this;
            MainScreen.RaiseDownMenu(modulesEntity, Helpers.EnumWindowType.ADDMODE);
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
            RenderColumns();
        }
    }
}
