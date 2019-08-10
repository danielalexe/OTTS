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

namespace OTTS_WPF.Teachers
{
    /// <summary>
    /// Interaction logic for WindowTeachersEntity.xaml
    /// </summary>
    public partial class WindowTeachersEntity : Window
    {
        public WindowTeachersCollection WindowTeachersCollection { get; set; }
        public MainScreen MainScreen { get; set; }
        public EnumWindowType WindowType { get;set; }
        public int ID_TEACHER { get; set; }
        public WindowTeachersEntity()
        {
            InitializeComponent();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (WindowType == EnumWindowType.ADDMODE)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    TEACHERS teacher = new TEACHERS();
                    teacher.nvNAME = CTextName.CTextBox.Text;
                    teacher.nvSURNAME = CTextSurname.CTextBox.Text;

                    teacher.bACTIVE = true;
                    teacher.dtCREATE_DATE = DateTime.UtcNow;
                    teacher.iCREATE_USER = PersistentData.LoggedUser.iID_USER;

                    db.TEACHERS.Add(teacher);
                    db.SaveChanges();
                    WindowTeachersCollection.ReloadData();
                    CloseWindow();
                }
            }
            else if (WindowType == EnumWindowType.EDITMODE)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    var getTeacher = db.TEACHERS.FirstOrDefault(z => z.iID_TEACHER == ID_TEACHER && z.bACTIVE==true);
                    if (getTeacher!=null)
                    {
                        getTeacher.nvNAME = CTextName.CTextBox.Text;
                        getTeacher.nvSURNAME = CTextSurname.CTextBox.Text;

                        getTeacher.dtLASTMODIFIED_DATE = DateTime.UtcNow;
                        getTeacher.iLASTMODIFIED_USER = PersistentData.LoggedUser.iID_USER;

                        db.SaveChanges();
                        WindowTeachersCollection.ReloadData();
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
                    var getTeacher = db.TEACHERS.FirstOrDefault(z => z.iID_TEACHER == ID_TEACHER && z.bACTIVE==true);
                    if (getTeacher != null)
                    {
                        CTextName.CTextBox.Text = getTeacher.nvNAME;
                        CTextSurname.CTextBox.Text = getTeacher.nvSURNAME;
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
