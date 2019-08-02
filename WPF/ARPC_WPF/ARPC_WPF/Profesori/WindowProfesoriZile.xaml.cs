using OTTS_WPF.Helpers;
using DataLink;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DataObjects;

namespace OTTS_WPF.Profesori
{
    /// <summary>
    /// Interaction logic for WindowProfesoriZile.xaml
    /// </summary>
    public partial class WindowProfesoriZile : Window
    {
        public WindowProfesoriColectie WindowProfesoriColectie { get; set; }
        public MainScreen MainScreen { get; set; }
        public int ID_PROFESOR { get; set; }
        public WindowProfesoriZile()
        {
            InitializeComponent();
            LoadData();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            List<DTOProfessorPrefferedDay> list = (List<DTOProfessorPrefferedDay>)DataGridProfesori.ItemsSource;
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                foreach (var item in list)
                {
                    if (item.PRIORITY != 0)
                    {
                        var getPreferintaZi = db.TEACHER_PREFERRED_DAYS.FirstOrDefault(z => z.iID_TEACHER == ID_PROFESOR && z.iID_DAY == item.iID_DAY && z.bACTIVE == true);
                        if (getPreferintaZi != null)
                        {
                            getPreferintaZi.iPRIORITY = item.PRIORITY;

                            getPreferintaZi.dtLASTMODIFIED_DATE = DateTime.UtcNow;
                            getPreferintaZi.iLASTMODIFIED_USER = PersistentData.LoggedUser.iID_USER;

                            db.SaveChanges();
                        }
                        else
                        {
                            TEACHER_PREFERRED_DAYS tpd = new TEACHER_PREFERRED_DAYS();
                            tpd.iCREATE_USER = PersistentData.LoggedUser.iID_USER;
                            tpd.dtCREATE_DATE = DateTime.UtcNow;
                            tpd.bACTIVE = true;

                            tpd.iID_DAY = item.iID_DAY;
                            tpd.iID_TEACHER = ID_PROFESOR;
                            tpd.iPRIORITY = item.PRIORITY;

                            db.TEACHER_PREFERRED_DAYS.Add(tpd);
                            db.SaveChanges();
                        }
                    }
                }
                CloseWindow();
            }
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
            List<DTOProfessorPrefferedDay> list = new List<DTOProfessorPrefferedDay>();
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {

                var getZile = db.DAYS.Where(z => z.bACTIVE == true).ToList();
                foreach (var item in getZile)
                {
                    DTOProfessorPrefferedDay dto = new DTOProfessorPrefferedDay();
                    var getPreferintaZi = db.TEACHER_PREFERRED_DAYS.FirstOrDefault(z => z.iID_TEACHER == ID_PROFESOR && z.iID_DAY == item.iID_DAY && z.bACTIVE == true);
                    if (getPreferintaZi != null)
                    {
                        dto.DAY_NAME = item.nvNAME;
                        dto.iID_DAY = item.iID_DAY;
                        dto.PRIORITY = getPreferintaZi.iPRIORITY;
                    }
                    else
                    {
                        dto.DAY_NAME = item.nvNAME;
                        dto.iID_DAY = item.iID_DAY;
                        dto.PRIORITY = 0;
                    }
                    list.Add(dto);
                }

            }
            DataGridProfesori.ItemsSource = list;
            RenderColumns();
        }

        private void RenderColumns()
        {
            foreach (DataGridColumn c in DataGridProfesori.Columns)
            {
                if (c.Header.ToString().StartsWith("iID_") || c.Header.ToString().StartsWith("nvPASSWORD") || c.Header.ToString().StartsWith("nvPAROLA"))
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

        private void DataGridProfesori_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() != "PRIORITY")
            {
                e.Column.IsReadOnly = true;
            }
        }

        private void DataGridProfesori_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
