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
    /// Interaction logic for WindowGroupsLecturesEntity.xaml
    /// </summary>
    public partial class WindowGroupsLecturesEntity : Window
    {
        public WindowGroupsLectures WindowGroupsLectures { get; set; }
        public MainScreen MainScreen { get; set; }
        public EnumWindowType WindowType { get; set; }
        public int ID_GROUP { get; set; }
        public WindowGroupsLecturesEntity()
        {
            InitializeComponent();
            LoadData();
        }

        public void CloseWindow()
        {
            WindowGroupsLectures.LowerDownMenu();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        public void LoadData()
        {
            List<DTOLecture> list = new List<DTOLecture>();
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                var getGroupLecture = (from u in db.LECTURES
                                      where u.bACTIVE == true
                                      &&
                                      u.iID_SEMESTER == PersistentData.SelectedSemester
                                      &&
                                      !db.GROUPS_LECTURES_LINK.Any(z => z.iID_GROUP == ID_GROUP && z.iID_LECTURE == u.iID_LECTURE && z.bACTIVE == true)
                                      select new DTOLecture
                                      {
                                          iID_LECTURE = u.iID_LECTURE,
                                          NAME = u.nvNAME
                                      }).ToList();
                list.AddRange(getGroupLecture);
            }
            DataGridLectures.ItemsSource = list;
            RenderColumns();
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var list = DataGridLectures.SelectedItems;
            if (list.Count == 0)
            {
                MessageBox.Show("Nici un rand nu este selectat");
                return;
            }
            else
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    foreach (DTOLecture item in list)
                    {
                        GROUPS_LECTURES_LINK gll = new GROUPS_LECTURES_LINK();
                        gll.iID_GROUP = ID_GROUP;
                        gll.iID_LECTURE = item.iID_LECTURE;

                        gll.iID_SEMESTER = PersistentData.SelectedSemester;

                        gll.bACTIVE = true;
                        gll.iCREATE_USER = PersistentData.LoggedUser.iID_USER;
                        gll.dtCREATE_DATE = DateTime.UtcNow;

                        db.GROUPS_LECTURES_LINK.Add(gll);
                        db.SaveChanges();
                    }
                }
                WindowGroupsLectures.LoadData();
                CloseWindow();
            }

        }

        private void RenderColumns()
        {
            foreach (DataGridColumn c in DataGridLectures.Columns)
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
