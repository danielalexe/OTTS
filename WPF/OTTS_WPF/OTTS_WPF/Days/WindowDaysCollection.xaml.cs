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

namespace OTTS_WPF.Days
{
    /// <summary>
    /// Interaction logic for WindowDaysCollection.xaml
    /// </summary>
    public partial class WindowDaysCollection : WindowBase
    {
        public MainScreen MainScreen { get; set; }
        public WindowDaysCollection()
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
                var getDays = (from u in db.DAYS
                                  where (
                                  (String.IsNullOrEmpty(FilterName) || u.nvNAME.Contains(FilterName))
                                  &&
                                  u.bACTIVE == true)
                                  select new DTODay
                                  {
                                      iID_DAY = u.iID_DAY,
                                      NAME = u.nvNAME,
                                      PRIORITY = u.iPRIORITY
                                  }).ToList();
                DataGridDays.ItemsSource = getDays;
                RenderColumns();
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var list = DataGridDays.SelectedItems;
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
                            foreach (DTODay item in list)
                            {
                                var getDay = db.DAYS.FirstOrDefault(z => z.iID_DAY == item.iID_DAY && z.bACTIVE == true);
                                if (getDay != null)
                                {
                                    getDay.bACTIVE = false;
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

            var list = DataGridDays.SelectedItems;
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
                    WindowDaysEntity daysEntity = new WindowDaysEntity();
                    daysEntity.MainScreen = MainScreen;
                    daysEntity.WindowType = Helpers.EnumWindowType.EDITMODE;
                    daysEntity.ID_DAY = ((DTODay)DataGridDays.SelectedItem).iID_DAY;
                    daysEntity.WindowDaysCollection = this;
                    daysEntity.LoadData();
                    MainScreen.RaiseDownMenu(daysEntity, Helpers.EnumWindowType.EDITMODE);
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            WindowDaysEntity daysEntity = new WindowDaysEntity();
            daysEntity.MainScreen = MainScreen;
            daysEntity.WindowType = Helpers.EnumWindowType.ADDMODE;
            daysEntity.WindowDaysCollection = this;
            MainScreen.RaiseDownMenu(daysEntity, Helpers.EnumWindowType.ADDMODE);
        }


        private void RenderColumns()
        {
            foreach (DataGridColumn c in DataGridDays.Columns)
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
            var HelpText = "\tIn acest meniu definiti zilele in care se desfasoara prelegeri.\r\n" +
                "\tPrioritatile asociate sunt general valabile pentru toate grupele din aplicatie si de asemenea sunt in ordine crescatoare, prioritatea 0 este cea mai mare.\r\n";
            HelpScreen help = new HelpScreen();
            help.TitleHelp.Text = "Days Help";
            help.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            help.TextHelp.Text = HelpText;
            help.Show();
        }
    }
}
