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

namespace OTTS_WPF.Teachers
{
    /// <summary>
    /// Interaction logic for WindowTeachersHalls.xaml
    /// </summary>
    public partial class WindowTeachersHalls : Window
    {
        public WindowTeachersCollection WindowTeachersCollection { get; set; }
        public MainScreen MainScreen { get; set; }
        public int ID_TEACHER { get; set; }
        public WindowTeachersHalls()
        {
            InitializeComponent();
            LoadData();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            List<DTOTeacherPrefferedHall> list = (List<DTOTeacherPrefferedHall>)DataGridTeachers.ItemsSource;
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                foreach (var item in list)
                {
                    if (item.PRIORITY != "N/A")
                    {
                        var getPreferredHall = db.TEACHER_PREFERRED_HALLS.FirstOrDefault(z => z.iID_TEACHER == ID_TEACHER && z.iID_HALL == item.iID_HALL && z.bACTIVE==true);
                        if (getPreferredHall != null)
                        {
                            getPreferredHall.iPRIORITY = Convert.ToInt32(item.PRIORITY);

                            getPreferredHall.dtLASTMODIFIED_DATE = DateTime.UtcNow;
                            getPreferredHall.iLASTMODIFIED_USER = PersistentData.LoggedUser.iID_USER;

                            db.SaveChanges();
                        }
                        else
                        {
                            TEACHER_PREFERRED_HALLS tph = new TEACHER_PREFERRED_HALLS();
                            tph.iCREATE_USER = PersistentData.LoggedUser.iID_USER;
                            tph.dtCREATE_DATE = DateTime.UtcNow;
                            tph.bACTIVE = true;

                            tph.iID_HALL = item.iID_HALL;
                            tph.iID_TEACHER = ID_TEACHER;
                            tph.iPRIORITY = Convert.ToInt32(item.PRIORITY);

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
            List<DTOTeacherPrefferedHall> list = new List<DTOTeacherPrefferedHall>();
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {

                var getHalls = db.HALLS.Where(z=>z.bACTIVE==true).ToList();
                foreach (var item in getHalls)
                {
                    DTOTeacherPrefferedHall dto = new DTOTeacherPrefferedHall();
                    var getPrefferedHall = db.TEACHER_PREFERRED_HALLS.FirstOrDefault(z => z.iID_TEACHER == ID_TEACHER && z.iID_HALL == item.iID_HALL && z.bACTIVE==true);
                    if (getPrefferedHall != null)
                    {
                        dto.HALL_NAME = item.nvNAME;
                        dto.iID_HALL = item.iID_HALL;
                        dto.PRIORITY = getPrefferedHall.iPRIORITY.ToString();
                    }
                    else
                    {
                        dto.HALL_NAME = item.nvNAME;
                        dto.iID_HALL = item.iID_HALL;
                        dto.PRIORITY = "N/A";
                    }
                    list.Add(dto);
                }
                
            }
            DataGridTeachers.ItemsSource = list;
            RenderColumns();
        }

        private void RenderColumns()
        {
            foreach (DataGridColumn c in DataGridTeachers.Columns)
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

        private void DataGridTeachers_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() != "PRIORITY")
            {
                e.Column.IsReadOnly = true;
            }
        }

        private void DataGridTeachers_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
