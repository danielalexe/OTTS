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
    /// Interaction logic for WindowProfesoriSali.xaml
    /// </summary>
    public partial class WindowProfesoriSali : Window
    {
        public WindowProfesoriColectie WindowProfesoriColectie { get; set; }
        public MainScreen MainScreen { get; set; }
        public int ID_PROFESOR { get; set; }
        public WindowProfesoriSali()
        {
            InitializeComponent();
            LoadData();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            List<DTOProfessorPrefferedHall> list = (List<DTOProfessorPrefferedHall>)DataGridProfesori.ItemsSource;
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                foreach (var item in list)
                {
                    if (item.PRIORITY != 0)
                    {
                        var getPreferintaSala = db.TEACHER_PREFERRED_HALLS.FirstOrDefault(z => z.iID_TEACHER == ID_PROFESOR && z.iID_HALL == item.iID_HALL && z.bACTIVE==true);
                        if (getPreferintaSala != null)
                        {
                            getPreferintaSala.iPRIORITY = item.PRIORITY;

                            getPreferintaSala.dtLASTMODIFIED_DATE = DateTime.UtcNow;
                            getPreferintaSala.iLASTMODIFIED_USER = PersistentData.LoggedUser.iID_USER;

                            db.SaveChanges();
                        }
                        else
                        {
                            TEACHER_PREFERRED_HALLS tph = new TEACHER_PREFERRED_HALLS();
                            tph.iCREATE_USER = PersistentData.LoggedUser.iID_USER;
                            tph.dtCREATE_DATE = DateTime.UtcNow;
                            tph.bACTIVE = true;

                            tph.iID_HALL = item.iID_HALL;
                            tph.iID_TEACHER = ID_PROFESOR;
                            tph.iPRIORITY = item.PRIORITY;

                            db.TEACHER_PREFERRED_HALLS.Add(tph);
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
            List<DTOProfessorPrefferedHall> list = new List<DTOProfessorPrefferedHall>();
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {

                var getHalls = db.HALLS.Where(z=>z.bACTIVE==true).ToList();
                foreach (var item in getHalls)
                {
                    DTOProfessorPrefferedHall dto = new DTOProfessorPrefferedHall();
                    var getPrefferedHall = db.TEACHER_PREFERRED_HALLS.FirstOrDefault(z => z.iID_TEACHER == ID_PROFESOR && z.iID_HALL == item.iID_HALL && z.bACTIVE==true);
                    if (getPrefferedHall != null)
                    {
                        dto.HALL_NAME = item.nvNAME;
                        dto.iID_HALL = item.iID_HALL;
                        dto.PRIORITY = getPrefferedHall.iPRIORITY;
                    }
                    else
                    {
                        dto.HALL_NAME = item.nvNAME;
                        dto.iID_HALL = item.iID_HALL;
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
