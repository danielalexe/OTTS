using OTTS_WPF.Helpers;
using OTTS_WPF.Template;
using DataLink;
using DataObjects;
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

namespace OTTS_WPF.Profesori
{
    /// <summary>
    /// Interaction logic for WindowProfesoriColectie.xaml
    /// </summary>
    public partial class WindowProfesoriColectie : WindowBase
    {
        public MainScreen MainScreen { get; set; }
        public WindowProfesoriColectie()
        {
            InitializeComponent();
            ReloadData();
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
                var FiltruNume = CTextNume.CString.ToUpper();
                var FiltruPrenume = CTextPrenume.CString.ToUpper();
                var getProfesori = (from u in db.TEACHERS
                                    where ((String.IsNullOrEmpty(FiltruNume) || u.nvNAME.Contains(FiltruNume))
                                    &&
                                    (String.IsNullOrEmpty(FiltruPrenume) || u.nvSURNAME.Contains(FiltruPrenume)))
                                    select new DTOProfessor
                                    {
                                        ID_PROFESOR = u.iID_TEACHER,
                                        NUME = u.nvNAME,
                                        PRENUME = u.nvSURNAME
                                    }).ToList();
                DataGridProfesori.ItemsSource = getProfesori;
                RenderColumns();
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var list = DataGridProfesori.SelectedItems;
            if (list.Count>0)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    foreach (DTOProfessor item in list)
                    {
                        var getProfesor = db.TEACHERS.FirstOrDefault(z => z.iID_TEACHER == item.ID_PROFESOR);
                        if (getProfesor!=null)
                        {
                            db.TEACHERS.Remove(getProfesor);
                        }
                    }
                    db.SaveChanges();
                }
            }
            ReloadData();
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            WindowProfesoriEntitate profesoriEntitate = new WindowProfesoriEntitate();
            profesoriEntitate.MainScreen = MainScreen;
            profesoriEntitate.WindowType = Helpers.EnumWindowType.EDITMODE;
            profesoriEntitate.ID_PROFESOR = ((DTOProfessor)DataGridProfesori.SelectedItem).ID_PROFESOR;
            profesoriEntitate.WindowProfesoriColectie = this;
            profesoriEntitate.LoadData();
            MainScreen.RaiseDownMenu(profesoriEntitate, Helpers.EnumWindowType.EDITMODE);
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            WindowProfesoriEntitate profesoriEntitate = new WindowProfesoriEntitate();
            profesoriEntitate.MainScreen = MainScreen;
            profesoriEntitate.WindowType = Helpers.EnumWindowType.ADDMODE;
            profesoriEntitate.WindowProfesoriColectie = this;
            MainScreen.RaiseDownMenu(profesoriEntitate, Helpers.EnumWindowType.ADDMODE);
        }


        private void RenderColumns()
        {
            foreach (DataGridColumn c in DataGridProfesori.Columns)
            {
                if (c.Header.ToString().StartsWith("ID_") || c.Header.ToString().StartsWith("PASSWORD") || c.Header.ToString().StartsWith("PAROLA"))
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
