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
    /// Interaction logic for WindowTeachersDays.xaml
    /// </summary>
    public partial class WindowTeachersDays : Window
    {
        public WindowTeachersCollection WindowTeachersCollection { get; set; }
        public MainScreen MainScreen { get; set; }
        public int ID_TEACHER { get; set; }
        public WindowTeachersDays()
        {
            InitializeComponent();
            LoadData();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            List<DTOTeacherPrefferedDay> list = (List<DTOTeacherPrefferedDay>)DataGridTeachers.ItemsSource;
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                foreach (var item in list)
                {
                    if (item.PRIORITY != "N/A")
                    {
                        var getPreferredDay = db.TEACHER_PREFERRED_DAYS.FirstOrDefault(z => z.iID_TEACHER == ID_TEACHER && z.iID_DAY == item.iID_DAY && z.bACTIVE == true);
                        if (getPreferredDay != null)
                        {
                            getPreferredDay.iPRIORITY = Convert.ToInt32(item.PRIORITY);

                            getPreferredDay.dtLASTMODIFIED_DATE = DateTime.UtcNow;
                            getPreferredDay.iLASTMODIFIED_USER = PersistentData.LoggedUser.iID_USER;

                            db.SaveChanges();
                        }
                        else
                        {
                            TEACHER_PREFERRED_DAYS tpd = new TEACHER_PREFERRED_DAYS();
                            tpd.iCREATE_USER = PersistentData.LoggedUser.iID_USER;
                            tpd.dtCREATE_DATE = DateTime.UtcNow;
                            tpd.bACTIVE = true;

                            tpd.iID_DAY = item.iID_DAY;
                            tpd.iID_TEACHER = ID_TEACHER;
                            tpd.iPRIORITY = Convert.ToInt32(item.PRIORITY);

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
            List<DTOTeacherPrefferedDay> list = new List<DTOTeacherPrefferedDay>();
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {

                var getDays = db.DAYS.Where(z => z.bACTIVE == true).ToList();
                foreach (var item in getDays)
                {
                    DTOTeacherPrefferedDay dto = new DTOTeacherPrefferedDay();
                    var getPreferredDay = db.TEACHER_PREFERRED_DAYS.FirstOrDefault(z => z.iID_TEACHER == ID_TEACHER && z.iID_DAY == item.iID_DAY && z.bACTIVE == true);
                    if (getPreferredDay != null)
                    {
                        dto.DAY_NAME = item.nvNAME;
                        dto.iID_DAY = item.iID_DAY;
                        dto.PRIORITY = getPreferredDay.iPRIORITY.ToString();
                    }
                    else
                    {
                        dto.DAY_NAME = item.nvNAME;
                        dto.iID_DAY = item.iID_DAY;
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

        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            var HelpText = "\tPrioritatile sunt in ordine crescatoare si astfel prioritatea 0 este cea mai mare.\r\n";
            HelpScreen help = new HelpScreen();
            help.TitleHelp.Text = "Teacher Days Priority Help";
            help.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            help.TextHelp.Text = HelpText;
            help.Show();
        }
    }
}
