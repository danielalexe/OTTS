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

namespace OTTS_WPF.Teachers
{
    /// <summary>
    /// Interaction logic for WindowTeachersModules.xaml
    /// </summary>
    public partial class WindowTeachersModules : Window
    {
        public WindowTeachersCollection WindowTeachersCollection { get; set; }
        public MainScreen MainScreen { get; set; }
        public int ID_TEACHER { get; set; }
        public WindowTeachersModules()
        {
            InitializeComponent();
            LoadData();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            List<DTOTeacherPrefferedModule> list = (List<DTOTeacherPrefferedModule>)DataGridTeachers.ItemsSource;
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                foreach (var item in list)
                {
                    if (item.PRIORITY_BACHELOR != "N/A")
                    {
                        var getTeacherModule = db.TEACHER_PREFERRED_MODULES.FirstOrDefault(z => z.iID_TEACHER == ID_TEACHER && z.iID_MODULE == item.iID_MODULE && z.bACTIVE==true && z.iID_GROUP_TYPE==1);
                        if (getTeacherModule != null)
                        {
                            getTeacherModule.iPRIORITY = Convert.ToInt32(item.PRIORITY_BACHELOR);

                            getTeacherModule.dtLASTMODIFIED_DATE = DateTime.UtcNow;
                            getTeacherModule.iLASTMODIFIED_USER = PersistentData.LoggedUser.iID_USER;

                            db.SaveChanges();
                        }
                        else
                        {
                            TEACHER_PREFERRED_MODULES tpm = new TEACHER_PREFERRED_MODULES();
                            tpm.iCREATE_USER = PersistentData.LoggedUser.iID_USER;
                            tpm.dtCREATE_DATE = DateTime.UtcNow;
                            tpm.bACTIVE = true;

                            tpm.iID_MODULE = item.iID_MODULE;
                            tpm.iID_TEACHER = ID_TEACHER;
                            tpm.iPRIORITY = Convert.ToInt32(item.PRIORITY_BACHELOR);
                            tpm.iID_GROUP_TYPE = 1;

                            db.TEACHER_PREFERRED_MODULES.Add(tpm);
                            db.SaveChanges();
                        }
                    }
                    if (item.PRIORITY_MASTERS != "N/A")
                    {
                        var getTeacherModule = db.TEACHER_PREFERRED_MODULES.FirstOrDefault(z => z.iID_TEACHER == ID_TEACHER && z.iID_MODULE == item.iID_MODULE && z.bACTIVE==true && z.iID_GROUP_TYPE==2);
                        if (getTeacherModule != null)
                        {
                            getTeacherModule.iPRIORITY = Convert.ToInt32(item.PRIORITY_MASTERS);

                            getTeacherModule.dtLASTMODIFIED_DATE = DateTime.UtcNow;
                            getTeacherModule.iLASTMODIFIED_USER = PersistentData.LoggedUser.iID_USER;

                            db.SaveChanges();
                        }
                        else
                        {
                            TEACHER_PREFERRED_MODULES tpm = new TEACHER_PREFERRED_MODULES();
                            tpm.iCREATE_USER = PersistentData.LoggedUser.iID_USER;
                            tpm.dtCREATE_DATE = DateTime.UtcNow;
                            tpm.bACTIVE = true;

                            tpm.iID_MODULE = item.iID_MODULE;
                            tpm.iID_TEACHER = ID_TEACHER;
                            tpm.iPRIORITY = Convert.ToInt32(item.PRIORITY_MASTERS);
                            tpm.iID_GROUP_TYPE = 2;

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
            List<DTOTeacherPrefferedModule> list = new List<DTOTeacherPrefferedModule>();
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {

                var getModules = db.MODULES.Where(z=>z.bACTIVE==true).ToList();
                foreach (var item in getModules)
                {
                    DTOTeacherPrefferedModule dto = new DTOTeacherPrefferedModule();
                    dto.MODULE_NAME = item.nvNAME;
                    dto.iID_MODULE = item.iID_MODULE;
                    var getPreferredModuleBachelor = db.TEACHER_PREFERRED_MODULES.FirstOrDefault(z => z.iID_TEACHER == ID_TEACHER && z.iID_MODULE == item.iID_MODULE && z.bACTIVE==true && z.iID_GROUP_TYPE==1);
                    if (getPreferredModuleBachelor != null)
                    {
                        dto.PRIORITY_BACHELOR = getPreferredModuleBachelor.iPRIORITY.ToString();
                    }
                    else
                    {
                        dto.PRIORITY_BACHELOR = "N/A";
                    }
                    var getPreferredModuleMasters = db.TEACHER_PREFERRED_MODULES.FirstOrDefault(z => z.iID_TEACHER == ID_TEACHER && z.iID_MODULE == item.iID_MODULE && z.bACTIVE==true && z.iID_GROUP_TYPE==2);
                    if (getPreferredModuleMasters != null)
                    {
                        dto.PRIORITY_MASTERS = getPreferredModuleMasters.iPRIORITY.ToString();
                    }
                    else
                    {
                        dto.PRIORITY_MASTERS = "N/A";
                    }

                    list.Add(dto);
                }
                
            }
            DataGridTeachers.ItemsSource = list;
            RenderColumns();
        }

        private void RenderColumns()
        {
            foreach (DataGridColumn c in DataGridTeachers.Columns)
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

        private void DataGridTeachers_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (!e.Column.Header.ToString().StartsWith("PRIORITY"))
            {
                e.Column.IsReadOnly = true;
            }
        }

        private void DataGridTeachers_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }
    }
}
