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

namespace OTTS_WPF.Profesori
{
    /// <summary>
    /// Interaction logic for WindowProfesoriModule.xaml
    /// </summary>
    public partial class WindowProfesoriModule : Window
    {
        public WindowProfesoriColectie WindowProfesoriColectie { get; set; }
        public MainScreen MainScreen { get; set; }
        public int ID_PROFESOR { get; set; }
        public WindowProfesoriModule()
        {
            InitializeComponent();
            LoadData();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            List<DTOProfessorPrefferedModule> list = (List<DTOProfessorPrefferedModule>)DataGridProfesori.ItemsSource;
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                foreach (var item in list)
                {
                    if (item.PRIORITY != 0)
                    {
                        var getPreferintaModul = db.TEACHER_PREFERRED_MODULES.FirstOrDefault(z => z.iID_TEACHER == ID_PROFESOR && z.iID_MODULE == item.iID_MODULE && z.bACTIVE==true);
                        if (getPreferintaModul != null)
                        {
                            getPreferintaModul.iPRIORITY = item.PRIORITY;

                            getPreferintaModul.dtLASTMODIFIED_DATE = DateTime.UtcNow;
                            getPreferintaModul.iLASTMODIFIED_USER = PersistentData.LoggedUser.iID_USER;

                            db.SaveChanges();
                        }
                        else
                        {
                            TEACHER_PREFERRED_MODULES tpm = new TEACHER_PREFERRED_MODULES();
                            tpm.iCREATE_USER = PersistentData.LoggedUser.iID_USER;
                            tpm.dtCREATE_DATE = DateTime.UtcNow;
                            tpm.bACTIVE = true;

                            tpm.iID_MODULE = item.iID_MODULE;
                            tpm.iID_TEACHER = ID_PROFESOR;
                            tpm.iPRIORITY = item.PRIORITY;

                            db.TEACHER_PREFERRED_MODULES.Add(tpm);
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
            List<DTOProfessorPrefferedModule> list = new List<DTOProfessorPrefferedModule>();
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {

                var getModules = db.MODULES.Where(z=>z.bACTIVE==true).ToList();
                foreach (var item in getModules)
                {
                    DTOProfessorPrefferedModule dto = new DTOProfessorPrefferedModule();
                    var getPreferredModule = db.TEACHER_PREFERRED_MODULES.FirstOrDefault(z => z.iID_TEACHER == ID_PROFESOR && z.iID_MODULE == item.iID_MODULE && z.bACTIVE==true);
                    if (getPreferredModule != null)
                    {
                        dto.MODULE_NAME = item.nvNAME;
                        dto.iID_MODULE = item.iID_MODULE;
                        dto.PRIORITY = getPreferredModule.iPRIORITY;
                    }
                    else
                    {
                        dto.MODULE_NAME = item.nvNAME;
                        dto.iID_MODULE = item.iID_MODULE;
                        dto.PRIORITY = 0;
                    }
                    list.Add(dto);
                }
                
            }
            DataGridProfesori.ItemsSource = list;
            RenderColumns();
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

        private void ButtonRefresh_Click(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void DataGridProfesori_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.Column.Header.ToString() != "PRIORITY")
            {
                e.Column.IsReadOnly = true;
            }
        }

        private void DataGridProfesori_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
