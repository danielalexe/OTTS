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

namespace OTTS_WPF.TeachersLectures
{
    /// <summary>
    /// Interaction logic for WindowTeachersLecturesCollection.xaml
    /// </summary>
    public partial class WindowTeachersLecturesCollection : WindowBase
    {
        public MainScreen MainScreen { get; set; }
        public WindowTeachersLecturesCollection()
        {
            InitializeComponent();
            BindComboLectureType();
            ReloadData();
        }

        private void BindComboLectureType()
        {
            List<DTOLectureType> list = new List<DTOLectureType>();
            DTOLectureType dto = new DTOLectureType();
            dto.iID_LECTURE_TYPE = -1;
            dto.nvCOMBO_DISPLAY = "All";
            list.Add(dto);
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                var getLectureTypess = (from u in db.LECTURE_TYPE
                                where u.bACTIVE == true
                                select new DTOLectureType
                                {
                                    iID_LECTURE_TYPE = u.iID_LECTURE_TYPE,
                                    NAME = u.nvNAME,
                                    nvCOMBO_DISPLAY = u.nvNAME
                                }).ToList();
                list.AddRange(getLectureTypess);
            }
            CComboLectureType.CComboBox.ItemsSource = list;
            CComboLectureType.CComboBox.SelectedItem = list.FirstOrDefault();
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
                var FilterTeacherName = CTextTeacherName.CString.ToUpper();
                var FilterLectureName = CTextLectureName.CString.ToUpper();
                var FilterLectureType = ((DTOLectureType)CComboLectureType.CComboBox.SelectedItem).iID_LECTURE_TYPE;
                var getTeachersLectures = (from u in db.TEACHERS_LECTURES_LINK
                                   where (
                                   (String.IsNullOrEmpty(FilterTeacherName) || u.TEACHERS.nvNAME.Contains(FilterTeacherName))
                                   &&
                                   (String.IsNullOrEmpty(FilterLectureName) || u.LECTURES.nvNAME.Contains(FilterLectureName))
                                   &&
                                   (FilterLectureType == -1||u.iID_LECTURE_TYPE == FilterLectureType)
                                   &&
                                   (u.iID_SEMESTER == PersistentData.SelectedSemester)
                                   &&
                                   u.bACTIVE == true)
                                   select new DTOTeachersLectures
                                   {
                                       iID_TEACHERS_LECTURES_LINK = u.iID_TEACHERS_LECTURES_LINK,
                                       iID_LECTURE = u.iID_LECTURE,
                                       LECTURE_NAME = u.LECTURES.nvNAME,
                                       iID_TEACHER = u.iID_TEACHER,
                                       TEACHER_NAME = u.TEACHERS.nvSURNAME+" "+u.TEACHERS.nvNAME,
                                       iID_LECTURE_TYPE = u.iID_LECTURE_TYPE,
                                       LECTURE_TYPE = u.LECTURE_TYPE.nvNAME,
                                       HOURS = u.iHOURS
                                   }).ToList();
                DataGridTeachersLectures.ItemsSource = getTeachersLectures;
                RenderColumns();
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var list = DataGridTeachersLectures.SelectedItems;
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
                            foreach (DTOTeachersLectures item in list)
                            {
                                var getTeacherLecture = db.TEACHERS_LECTURES_LINK.FirstOrDefault(z => z.iID_TEACHERS_LECTURES_LINK == item.iID_TEACHERS_LECTURES_LINK && z.bACTIVE == true);
                                if (getTeacherLecture != null)
                                {
                                    getTeacherLecture.bACTIVE = false;
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

            var list = DataGridTeachersLectures.SelectedItems;
            if (list.Count == 0)
            {
                MessageBox.Show("Nici un rand nu este selectat");
                return;
            }
            else
            {
                if (list.Count > 1)
                {
                    MessageBox.Show("Modificarile asupra unei legaturi profesor-prelegere se poate face doar la o singura inregistrare simultan");
                    return;
                }
                else
                {
                    WindowTeachersLecturesEntity teacherslecturesEntity = new WindowTeachersLecturesEntity();
                    teacherslecturesEntity.MainScreen = MainScreen;
                    teacherslecturesEntity.WindowType = Helpers.EnumWindowType.EDITMODE;
                    teacherslecturesEntity.ID_TEACHERS_LECTURES_LINK = ((DTOTeachersLectures)DataGridTeachersLectures.SelectedItem).iID_TEACHERS_LECTURES_LINK;
                    teacherslecturesEntity.WindowTeachersLecturesCollection = this;
                    teacherslecturesEntity.LoadData();
                    MainScreen.RaiseDownMenu(teacherslecturesEntity, Helpers.EnumWindowType.EDITMODE);
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            WindowTeachersLecturesEntity teacherslecturesEntity = new WindowTeachersLecturesEntity();
            teacherslecturesEntity.MainScreen = MainScreen;
            teacherslecturesEntity.WindowType = Helpers.EnumWindowType.ADDMODE;
            teacherslecturesEntity.WindowTeachersLecturesCollection = this;
            MainScreen.RaiseDownMenu(teacherslecturesEntity, Helpers.EnumWindowType.ADDMODE);
        }


        private void RenderColumns()
        {
            foreach (DataGridColumn c in DataGridTeachersLectures.Columns)
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
