using ARPC_WPF.Helpers;
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

namespace ARPC_WPF.Profesori
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
                using (var db = new ARPCContext())
                {
                    PROFESORI profesor = new PROFESORI();
                    profesor.NUME = CTextNume.CTextBox.Text;
                    profesor.PRENUME = CTextPrenume.CTextBox.Text;
                    db.PROFESORI.Add(profesor);
                    db.SaveChanges();
                    WindowProfesoriColectie.ReloadData();
                    CloseWindow();
                }
            }
            else if (WindowType == EnumWindowType.EDITMODE)
            {
                using (var db = new ARPCContext())
                {
                    var getProfesor = db.PROFESORI.FirstOrDefault(z => z.ID_PROFESOR == ID_PROFESOR);
                    if (getProfesor!=null)
                    {
                        getProfesor.NUME = CTextNume.CTextBox.Text;
                        getProfesor.PRENUME = CTextPrenume.CTextBox.Text;
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
                using (var db = new ARPCContext())
                {
                    var getProfesor = db.PROFESORI.FirstOrDefault(z => z.ID_PROFESOR == ID_PROFESOR);
                    if (getProfesor != null)
                    {
                        CTextNume.CTextBox.Text = getProfesor.NUME;
                        CTextPrenume.CTextBox.Text = getProfesor.PRENUME;
                    }
                }
            }
        }
    }
}
