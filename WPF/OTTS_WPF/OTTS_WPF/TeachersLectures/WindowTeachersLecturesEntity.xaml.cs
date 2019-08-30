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

namespace OTTS_WPF.TeachersLectures
{
    /// <summary>
    /// Interaction logic for WindowTeachersLecturesEntity.xaml
    /// </summary>
    public partial class WindowTeachersLecturesEntity : Window
    {
        public WindowTeachersLecturesCollection WindowTeachersLecturesCollection { get; set; }
        public MainScreen MainScreen { get; set; }
        public EnumWindowType WindowType { get; set; }
        public int ID_TEACHERS_LECTURES_LINK { get; set; }
        public WindowTeachersLecturesEntity()
        {
            InitializeComponent();
            BindComboTeachers();
            BindComboLectures();
            BindComboLectureType();
            LoadData();
        }

        private void BindComboTeachers()
        {
            List<DTOTeacher> list = new List<DTOTeacher>();
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                var getLectureTypess = (from u in db.TEACHERS
                                        where u.bACTIVE == true
                                        select new DTOTeacher
                                        {
                                            iID_TEACHER = u.iID_TEACHER,
                                            NAME = u.nvNAME,
                                            SURNAME = u.nvSURNAME,
                                            nvCOMBO_DISPLAY = u.nvSURNAME+" "+u.nvNAME
                                        }).ToList();
                list.AddRange(getLectureTypess);
            }
            CComboTeachers.CComboBox.ItemsSource = list;
            CComboTeachers.CComboBox.SelectedItem = list.FirstOrDefault();
        }

        private void BindComboLectures()
        {
            List<DTOLecture> list = new List<DTOLecture>();
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                var getLectures = (from u in db.LECTURES
                                        where u.bACTIVE == true
                                        &&
                                        u.iID_SEMESTER == PersistentData.SelectedSemester
                                        select new DTOLecture
                                        {
                                            iID_LECTURE = u.iID_LECTURE,
                                            NAME = u.nvNAME,
                                            nvCOMBO_DISPLAY = u.nvNAME
                                        }).ToList();
                list.AddRange(getLectures);
            }
            CComboLectures.CComboBox.ItemsSource = list;
            CComboLectures.CComboBox.SelectedItem = list.FirstOrDefault();
        }

        private void BindComboLectureType()
        {
            List<DTOLectureType> list = new List<DTOLectureType>();
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

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (WindowType == EnumWindowType.ADDMODE)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    TEACHERS_LECTURES_LINK tll = new TEACHERS_LECTURES_LINK();
                    tll.iHOURS = Convert.ToInt32(CDecimalHours.CValue);
                    tll.iID_LECTURE = ((DTOLecture)CComboLectures.CComboBox.SelectedItem).iID_LECTURE;
                    tll.iID_LECTURE_TYPE= ((DTOLectureType)CComboLectureType.CComboBox.SelectedItem).iID_LECTURE_TYPE;
                    tll.iID_TEACHER = ((DTOTeacher)CComboTeachers.CComboBox.SelectedItem).iID_TEACHER;

                    tll.iID_SEMESTER = PersistentData.SelectedSemester;

                    tll.bACTIVE = true;
                    tll.dtCREATE_DATE = DateTime.UtcNow;
                    tll.iCREATE_USER = PersistentData.LoggedUser.iID_USER;

                    var CheckTeachersLectures = db.TEACHERS_LECTURES_LINK.FirstOrDefault(z => z.bACTIVE == true
                    && z.iID_LECTURE == tll.iID_LECTURE
                    && z.iID_LECTURE_TYPE == tll.iID_LECTURE_TYPE
                    && z.iID_TEACHER == tll.iID_TEACHER
                    );
                    if (CheckTeachersLectures!=null)
                    {
                        MessageBox.Show("Exista deja o inregistrare cu profesorul, prelegerea si tipul de prelegere selectat","Eroare");
                    }
                    else
                    {
                        db.TEACHERS_LECTURES_LINK.Add(tll);
                        db.SaveChanges();
                        WindowTeachersLecturesCollection.ReloadData();
                        CloseWindow();
                    }
                }
            }
            else if (WindowType == EnumWindowType.EDITMODE)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    var getTeacherLecture = db.TEACHERS_LECTURES_LINK.FirstOrDefault(z => z.iID_TEACHERS_LECTURES_LINK == ID_TEACHERS_LECTURES_LINK && z.bACTIVE == true);
                    if (getTeacherLecture != null)
                    {
                        getTeacherLecture.iHOURS = Convert.ToInt32(CDecimalHours.CValue);
                        getTeacherLecture.iID_LECTURE = ((DTOLecture)CComboLectures.CComboBox.SelectedItem).iID_LECTURE;
                        getTeacherLecture.iID_LECTURE_TYPE = ((DTOLectureType)CComboLectureType.CComboBox.SelectedItem).iID_LECTURE_TYPE;
                        getTeacherLecture.iID_TEACHER = ((DTOTeacher)CComboTeachers.CComboBox.SelectedItem).iID_TEACHER;

                        getTeacherLecture.iID_SEMESTER = PersistentData.SelectedSemester;

                        getTeacherLecture.dtLASTMODIFIED_DATE = DateTime.UtcNow;
                        getTeacherLecture.iLASTMODIFIED_USER = PersistentData.LoggedUser.iID_USER;

                        db.SaveChanges();
                        WindowTeachersLecturesCollection.ReloadData();
                        CloseWindow();
                    }
                }
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
            if (WindowType == EnumWindowType.EDITMODE)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    var getTeacherLecture = db.TEACHERS_LECTURES_LINK.FirstOrDefault(z => z.iID_TEACHERS_LECTURES_LINK == ID_TEACHERS_LECTURES_LINK && z.bACTIVE == true);
                    if (getTeacherLecture != null)
                    {
                        CComboLectures.CComboBox.SelectedItem = ((List<DTOLecture>)CComboLectures.CComboBox.ItemsSource).FirstOrDefault(z=>z.iID_LECTURE==getTeacherLecture.iID_LECTURE);
                        CComboLectureType.CComboBox.SelectedItem = ((List<DTOLectureType>)CComboLectureType.CComboBox.ItemsSource).FirstOrDefault(z => z.iID_LECTURE_TYPE == getTeacherLecture.iID_LECTURE_TYPE);
                        CComboTeachers.CComboBox.SelectedItem = ((List<DTOTeacher>)CComboTeachers.CComboBox.ItemsSource).FirstOrDefault(z => z.iID_TEACHER == getTeacherLecture.iID_TEACHER);
                        CDecimalHours.CValue = getTeacherLecture.iHOURS;
                    }
                }
            }
        }

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
