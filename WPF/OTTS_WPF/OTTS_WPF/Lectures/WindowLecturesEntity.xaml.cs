using DataLink;
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

namespace OTTS_WPF.Lectures
{
    /// <summary>
    /// Interaction logic for WindowLecturesEntity.xaml
    /// </summary>
    public partial class WindowLecturesEntity : Window
    {
        public WindowLecturesCollection WindowLecturesCollection { get; set; }
        public MainScreen MainScreen { get; set; }
        public EnumWindowType WindowType { get; set; }
        public int ID_LECTURE { get; set; }
        public WindowLecturesEntity()
        {
            InitializeComponent();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (WindowType == EnumWindowType.ADDMODE)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    LECTURES lecture = new LECTURES();
                    lecture.nvNAME = CTextName.CTextBox.Text;

                    lecture.iID_SEMESTER = PersistentData.SelectedSemester;

                    lecture.bACTIVE = true;
                    lecture.dtCREATE_DATE = DateTime.UtcNow;
                    lecture.iCREATE_USER = PersistentData.LoggedUser.iID_USER;

                    db.LECTURES.Add(lecture);
                    db.SaveChanges();
                    WindowLecturesCollection.ReloadData();
                    CloseWindow();
                }
            }
            else if (WindowType == EnumWindowType.EDITMODE)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    var getLecture = db.LECTURES.FirstOrDefault(z => z.iID_LECTURE == ID_LECTURE && z.bACTIVE == true);
                    if (getLecture != null)
                    {
                        getLecture.nvNAME = CTextName.CTextBox.Text;

                        getLecture.iID_SEMESTER = PersistentData.SelectedSemester;

                        getLecture.dtLASTMODIFIED_DATE = DateTime.UtcNow;
                        getLecture.iLASTMODIFIED_USER = PersistentData.LoggedUser.iID_USER;

                        db.SaveChanges();
                        WindowLecturesCollection.ReloadData();
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
                    var getLecture = db.LECTURES.FirstOrDefault(z => z.iID_LECTURE == ID_LECTURE && z.bACTIVE == true);
                    if (getLecture != null)
                    {
                        CTextName.CTextBox.Text = getLecture.nvNAME;
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
