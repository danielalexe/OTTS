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

namespace OTTS_WPF.Halls
{
    /// <summary>
    /// Interaction logic for WindowHallsCollection.xaml
    /// </summary>
    public partial class WindowHallsCollection : WindowBase
    {
        public MainScreen MainScreen { get; set; }
        public WindowHallsCollection()
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
                var FilterMinimumCapacity = CDecimalMinimumCapacity.CNumericUpDown.Value;
                var getHalls = (from u in db.HALLS
                                    where (
                                    (String.IsNullOrEmpty(FilterName) || u.nvNAME.Contains(FilterName))
                                    &&
                                    (FilterMinimumCapacity==0 || u.iCAPACITY > FilterMinimumCapacity)
                                    &&
                                    u.bACTIVE == true)
                                    select new DTOHall
                                    {
                                        iID_HALL = u.iID_HALL,
                                        NAME = u.nvNAME,
                                        CAPACITY = u.iCAPACITY
                                    }).ToList();
                DataGridHalls.ItemsSource = getHalls;
                RenderColumns();
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var list = DataGridHalls.SelectedItems;
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
                            foreach (DTOHall item in list)
                            {
                                var getHall = db.HALLS.FirstOrDefault(z => z.iID_HALL == item.iID_HALL && z.bACTIVE == true);
                                if (getHall != null)
                                {
                                    getHall.bACTIVE = false;
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

            var list = DataGridHalls.SelectedItems;
            if (list.Count == 0)
            {
                MessageBox.Show("Nici un rand nu este selectat");
                return;
            }
            else
            {
                if (list.Count > 1)
                {
                    MessageBox.Show("Modificarile asupra unei sali se poate face doar la o singura sala simultan");
                    return;
                }
                else
                {
                    WindowHallsEntity hallsEntity = new WindowHallsEntity();
                    hallsEntity.MainScreen = MainScreen;
                    hallsEntity.WindowType = Helpers.EnumWindowType.EDITMODE;
                    hallsEntity.ID_HALL = ((DTOHall)DataGridHalls.SelectedItem).iID_HALL;
                    hallsEntity.WindowHallsCollection = this;
                    hallsEntity.LoadData();
                    MainScreen.RaiseDownMenu(hallsEntity, Helpers.EnumWindowType.EDITMODE);
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            WindowHallsEntity hallsEntity = new WindowHallsEntity();
            hallsEntity.MainScreen = MainScreen;
            hallsEntity.WindowType = Helpers.EnumWindowType.ADDMODE;
            hallsEntity.WindowHallsCollection = this;
            MainScreen.RaiseDownMenu(hallsEntity, Helpers.EnumWindowType.ADDMODE);
        }


        private void RenderColumns()
        {
            foreach (DataGridColumn c in DataGridHalls.Columns)
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
            var HelpText = "\tIn acest meniu definiti salile disponibile ale departamentului. Momentan acestea nu sunt folosite la planificare.\r\n";
            HelpScreen help = new HelpScreen();
            help.TitleHelp.Text = "Halls Help";
            help.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            help.TextHelp.Text = HelpText;
            help.Show();
        }
    }
}
