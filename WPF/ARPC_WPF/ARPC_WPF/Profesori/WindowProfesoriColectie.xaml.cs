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
                                    where (
                                    (String.IsNullOrEmpty(FiltruNume) || u.nvNAME.Contains(FiltruNume))
                                    &&
                                    (String.IsNullOrEmpty(FiltruPrenume) || u.nvSURNAME.Contains(FiltruPrenume))
                                    &&
                                    u.bACTIVE==true)
                                    select new DTOProfessor
                                    {
                                        iID_PROFESOR = u.iID_TEACHER,
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
            if (list.Count==0)
            {
                MessageBox.Show("Nici un rand nu este selectat");
                return;
            }
            else
            {
                if (list.Count > 0)
                {
                    if (MessageBox.Show("Sunteti siguri ca vreti sa stergeti randurile selectate?","Atentie",MessageBoxButton.YesNo)==MessageBoxResult.Yes)
                    {
                        using (var db = new OTTSContext(PersistentData.ConnectionString))
                        {
                            foreach (DTOProfessor item in list)
                            {
                                var getProfesor = db.TEACHERS.FirstOrDefault(z => z.iID_TEACHER == item.iID_PROFESOR && z.bACTIVE==true);
                                if (getProfesor != null)
                                {
                                    getProfesor.bACTIVE = false;
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

            var list = DataGridProfesori.SelectedItems;
            if (list.Count == 0)
            {
                MessageBox.Show("Nici un rand nu este selectat");
                return;
            }
            else
            {
                if (list.Count > 1)
                {
                    MessageBox.Show("Modificarile asupra unui profesor se pot face doar la un singur profesor simultan");
                    return;
                }
                else
                {
                    WindowProfesoriEntitate profesoriEntitate = new WindowProfesoriEntitate();
                    profesoriEntitate.MainScreen = MainScreen;
                    profesoriEntitate.WindowType = Helpers.EnumWindowType.EDITMODE;
                    profesoriEntitate.ID_PROFESOR = ((DTOProfessor)DataGridProfesori.SelectedItem).iID_PROFESOR;
                    profesoriEntitate.WindowProfesoriColectie = this;
                    profesoriEntitate.LoadData();
                    MainScreen.RaiseDownMenu(profesoriEntitate, Helpers.EnumWindowType.EDITMODE);
                }
            }
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

        private void ButtonSali_Click(object sender, RoutedEventArgs e)
        {
            var list = DataGridProfesori.SelectedItems;
            if (list.Count == 0)
            {
                MessageBox.Show("Nici un rand nu este selectat");
                return;
            }
            else
            {
                if (list.Count > 1)
                {
                    MessageBox.Show("Prioritatea pe sali se poate aloca doar la o singura persoana simultan");
                    return;
                }
                else
                {
                    WindowProfesoriSali profesoriSali = new WindowProfesoriSali();
                    profesoriSali.MainScreen = MainScreen;
                    profesoriSali.ID_PROFESOR = ((DTOProfessor)DataGridProfesori.SelectedItem).iID_PROFESOR;
                    profesoriSali.WindowProfesoriColectie = this;
                    MainScreen.RaiseDownMenu(profesoriSali);
                }
            }
        }

        private void ButtonModules_Click(object sender, RoutedEventArgs e)
        {
            var list = DataGridProfesori.SelectedItems;
            if (list.Count == 0)
            {
                MessageBox.Show("Nici un rand nu este selectat");
                return;
            }
            else
            {
                if (list.Count > 1)
                {
                    MessageBox.Show("Prioritatea pe module se poate aloca doar la o singura persoana simultan");
                    return;
                }
                else
                {
                    WindowProfesoriModule profesoriModule = new WindowProfesoriModule();
                    profesoriModule.MainScreen = MainScreen;
                    profesoriModule.ID_PROFESOR = ((DTOProfessor)DataGridProfesori.SelectedItem).iID_PROFESOR;
                    profesoriModule.WindowProfesoriColectie = this;
                    MainScreen.RaiseDownMenu(profesoriModule);
                }
            }
        }

        private void ButtonZile_Click(object sender, RoutedEventArgs e)
        {
            var list = DataGridProfesori.SelectedItems;
            if (list.Count == 0)
            {
                MessageBox.Show("Nici un rand nu este selectat");
                return;
            }
            else
            {
                if (list.Count > 1)
                {
                    MessageBox.Show("Prioritatea pe zile se poate aloca doar la o singura persoana simultan");
                    return;
                }
                else
                {
                    WindowProfesoriZile profesoriZile = new WindowProfesoriZile();
                    profesoriZile.MainScreen = MainScreen;
                    profesoriZile.ID_PROFESOR = ((DTOProfessor)DataGridProfesori.SelectedItem).iID_PROFESOR;
                    profesoriZile.WindowProfesoriColectie = this;
                    MainScreen.RaiseDownMenu(profesoriZile);
                }
            }
        }
    }
}
