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

namespace OTTS_WPF.Profesori
{
    /// <summary>
    /// Interaction logic for WindowProfesoriEntitate.xaml
    /// </summary>
    public partial class WindowProfesoriEntitate : Window
    {
        public WindowProfesoriColectie WindowProfesoriColectie { get; set; }
        public MainScreen MainScreen { get; set; }
        public EnumWindowType WindowType { get;set; }
        public int ID_PROFESOR { get; set; }
        public WindowProfesoriEntitate()
        {
            InitializeComponent();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (WindowType == EnumWindowType.ADDMODE)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    TEACHERS profesor = new TEACHERS();
                    profesor.nvNAME = CTextNume.CTextBox.Text;
                    profesor.nvSURNAME = CTextPrenume.CTextBox.Text;

                    profesor.bACTIVE = true;
                    profesor.dtCREATE_DATE = DateTime.UtcNow;
                    profesor.iCREATE_USER = PersistentData.LoggedUser.iID_USER;

                    db.TEACHERS.Add(profesor);
                    db.SaveChanges();
                    WindowProfesoriColectie.ReloadData();
                    CloseWindow();
                }
            }
            else if (WindowType == EnumWindowType.EDITMODE)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    var getProfesor = db.TEACHERS.FirstOrDefault(z => z.iID_TEACHER == ID_PROFESOR && z.bACTIVE==true);
                    if (getProfesor!=null)
                    {
                        getProfesor.nvNAME = CTextNume.CTextBox.Text;
                        getProfesor.nvSURNAME = CTextPrenume.CTextBox.Text;

                        getProfesor.dtLASTMODIFIED_DATE = DateTime.UtcNow;
                        getProfesor.iLASTMODIFIED_USER = PersistentData.LoggedUser.iID_USER;

                        db.SaveChanges();
                        WindowProfesoriColectie.ReloadData();
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
                    var getProfesor = db.TEACHERS.FirstOrDefault(z => z.iID_TEACHER == ID_PROFESOR && z.bACTIVE==true);
                    if (getProfesor != null)
                    {
                        CTextNume.CTextBox.Text = getProfesor.nvNAME;
                        CTextPrenume.CTextBox.Text = getProfesor.nvSURNAME;
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
