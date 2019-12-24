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

namespace OTTS_WPF.Teachers
{
    /// <summary>
    /// Interaction logic for WindowTeachersCollection.xaml
    /// </summary>
    public partial class WindowTeachersCollection : WindowBase
    {
        public MainScreen MainScreen { get; set; }
        public WindowTeachersCollection()
        {
            InitializeComponent();
            BindComboGroup();
            ReloadData();
            CComboGroup.CComboBox.SelectionChanged += CComboBoxGroup_SelectionChanged;
        }

        private void CComboBoxGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ReloadData();
        }

        private void BindComboGroup()
        {
            List<DTOGroup> list = new List<DTOGroup>();
            DTOGroup dTO = new DTOGroup();
            dTO.iID_GROUP = -1;
            dTO.NAME = "Neselectat";
            dTO.nvCOMBO_DISPLAY = "Neselectat";
            list.Add(dTO);

            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                var getGroups = (from u in db.GROUPS
                                 where u.bACTIVE == true
                                 select new DTOGroup
                                 {
                                     iID_GROUP = u.iID_GROUP,
                                     NAME = u.nvNAME,
                                     nvCOMBO_DISPLAY = u.nvNAME
                                 }).ToList();
                list.AddRange(getGroups);
            }

            CComboGroup.CComboBox.ItemsSource = list;
            CComboGroup.CComboBox.SelectedItem = list.FirstOrDefault();
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
            DTOGroup SelectedGroup = (DTOGroup)CComboGroup.CComboBox.SelectedItem;
            if (SelectedGroup.iID_GROUP == -1)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    var FilterName = CTextName.CString.ToUpper();
                    var FilterSurname = CTextSurname.CString.ToUpper();
                    var getTeachers = (from u in db.TEACHERS
                                       where (
                                       (String.IsNullOrEmpty(FilterName) || u.nvNAME.Contains(FilterName))
                                       &&
                                       (String.IsNullOrEmpty(FilterSurname) || u.nvSURNAME.Contains(FilterSurname))
                                       &&
                                       u.bACTIVE == true)
                                       select new DTOTeacher
                                       {
                                           iID_TEACHER = u.iID_TEACHER,
                                           NAME = u.nvNAME,
                                           SURNAME = u.nvSURNAME
                                       }).ToList();
                    foreach (var item in getTeachers)
                    {
                        var getBachelorPriority = db.TEACHERS_GROUP_TYPES_PRIORITY.FirstOrDefault(z => z.bACTIVE == true && z.iID_TEACHER == item.iID_TEACHER && z.iID_GROUP_TYPE == 1);
                        if (getBachelorPriority!=null)
                        {
                            item.BACHELOR_PRIORITY = getBachelorPriority.iPRIORITY.ToString();
                        }
                        else
                        {
                            item.BACHELOR_PRIORITY = "N/A";
                        }
                        var getMastersPriority = db.TEACHERS_GROUP_TYPES_PRIORITY.FirstOrDefault(z => z.bACTIVE == true && z.iID_TEACHER == item.iID_TEACHER && z.iID_GROUP_TYPE == 2);
                        if (getMastersPriority!=null)
                        {
                            item.MASTERS_PRIORITY = getMastersPriority.iPRIORITY.ToString();
                        }
                        else
                        {
                            item.MASTERS_PRIORITY = "N/A";
                        }
                    }
                    DataGridTeachers.ItemsSource = getTeachers;
                    RenderColumns();
                }
            }
            else
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    var FilterName = CTextName.CString.ToUpper();
                    var FilterSurname = CTextSurname.CString.ToUpper();
                    var getTeachers = (from u in db.TEACHERS
                                       join z in db.TEACHERS_LECTURES_LINK on u.iID_TEACHER equals z.iID_TEACHER
                                       join y in db.GROUPS_LECTURES_LINK on z.iID_LECTURE equals y.iID_LECTURE
                                       where (
                                       (String.IsNullOrEmpty(FilterName) || u.nvNAME.Contains(FilterName))
                                       &&
                                       (String.IsNullOrEmpty(FilterSurname) || u.nvSURNAME.Contains(FilterSurname))
                                       &&
                                       u.bACTIVE == true
                                       &&
                                       z.bACTIVE == true
                                       &&
                                       z.iID_TEACHER==u.iID_TEACHER
                                       &&
                                       y.bACTIVE==true
                                       &&
                                       y.iID_LECTURE==z.iID_LECTURE
                                       &&
                                       y.iID_GROUP == SelectedGroup.iID_GROUP
                                       )
                                       select new DTOTeacher
                                       {
                                           iID_TEACHER = u.iID_TEACHER,
                                           NAME = u.nvNAME,
                                           SURNAME = u.nvSURNAME
                                       }).Distinct().ToList();
                    foreach (var item in getTeachers)
                    {
                        var getBachelorPriority = db.TEACHERS_GROUP_TYPES_PRIORITY.FirstOrDefault(z => z.bACTIVE == true && z.iID_TEACHER == item.iID_TEACHER && z.iID_GROUP_TYPE == 1);
                        if (getBachelorPriority != null)
                        {
                            item.BACHELOR_PRIORITY = getBachelorPriority.iPRIORITY.ToString();
                        }
                        else
                        {
                            item.BACHELOR_PRIORITY = "N/A";
                        }
                        var getMastersPriority = db.TEACHERS_GROUP_TYPES_PRIORITY.FirstOrDefault(z => z.bACTIVE == true && z.iID_TEACHER == item.iID_TEACHER && z.iID_GROUP_TYPE == 2);
                        if (getMastersPriority != null)
                        {
                            item.MASTERS_PRIORITY = getMastersPriority.iPRIORITY.ToString();
                        }
                        else
                        {
                            item.MASTERS_PRIORITY = "N/A";
                        }
                    }
                    DataGridTeachers.ItemsSource = getTeachers;
                    RenderColumns();
                }
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var list = DataGridTeachers.SelectedItems;
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
                            foreach (DTOTeacher item in list)
                            {
                                var getTeacher = db.TEACHERS.FirstOrDefault(z => z.iID_TEACHER == item.iID_TEACHER && z.bACTIVE==true);
                                if (getTeacher != null)
                                {
                                    getTeacher.bACTIVE = false;
                                    var getBachelorPriority = db.TEACHERS_GROUP_TYPES_PRIORITY.Where(z => z.bACTIVE == true && z.iID_TEACHER == getTeacher.iID_TEACHER && z.iID_GROUP_TYPE == 1).ToList();
                                    foreach (var prop in getBachelorPriority)
                                    {
                                        prop.bACTIVE = false;
                                    }
                                    var getMastersPriority = db.TEACHERS_GROUP_TYPES_PRIORITY.Where(z => z.bACTIVE == true && z.iID_TEACHER == getTeacher.iID_TEACHER && z.iID_GROUP_TYPE == 2).ToList();
                                    foreach (var prop in getMastersPriority)
                                    {
                                        prop.bACTIVE = false;
                                    }
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

            var list = DataGridTeachers.SelectedItems;
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
                    WindowTeachersEntity teachersEntity = new WindowTeachersEntity();
                    teachersEntity.MainScreen = MainScreen;
                    teachersEntity.WindowType = Helpers.EnumWindowType.EDITMODE;
                    teachersEntity.ID_TEACHER = ((DTOTeacher)DataGridTeachers.SelectedItem).iID_TEACHER;
                    teachersEntity.WindowTeachersCollection = this;
                    teachersEntity.LoadData();
                    MainScreen.RaiseDownMenu(teachersEntity, Helpers.EnumWindowType.EDITMODE);
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            WindowTeachersEntity teachersEntity = new WindowTeachersEntity();
            teachersEntity.MainScreen = MainScreen;
            teachersEntity.WindowType = Helpers.EnumWindowType.ADDMODE;
            teachersEntity.WindowTeachersCollection = this;
            MainScreen.RaiseDownMenu(teachersEntity, Helpers.EnumWindowType.ADDMODE);
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

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            RenderColumns();
        }

        private void ButtonHalls_Click(object sender, RoutedEventArgs e)
        {
            var list = DataGridTeachers.SelectedItems;
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
                    WindowTeachersHalls teachersHalls = new WindowTeachersHalls();
                    teachersHalls.MainScreen = MainScreen;
                    teachersHalls.ID_TEACHER = ((DTOTeacher)DataGridTeachers.SelectedItem).iID_TEACHER;
                    teachersHalls.WindowTeachersCollection = this;
                    MainScreen.RaiseDownMenu(teachersHalls);
                }
            }
        }

        private void ButtonModules_Click(object sender, RoutedEventArgs e)
        {
            var list = DataGridTeachers.SelectedItems;
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
                    WindowTeachersModules teachersModules = new WindowTeachersModules();
                    teachersModules.MainScreen = MainScreen;
                    teachersModules.ID_TEACHER = ((DTOTeacher)DataGridTeachers.SelectedItem).iID_TEACHER;
                    teachersModules.WindowTeachersCollection = this;
                    MainScreen.RaiseDownMenu(teachersModules);
                }
            }
        }

        private void ButtonDays_Click(object sender, RoutedEventArgs e)
        {
            var list = DataGridTeachers.SelectedItems;
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
                    WindowTeachersDays teachersDays = new WindowTeachersDays();
                    teachersDays.MainScreen = MainScreen;
                    teachersDays.ID_TEACHER = ((DTOTeacher)DataGridTeachers.SelectedItem).iID_TEACHER;
                    teachersDays.WindowTeachersCollection = this;
                    MainScreen.RaiseDownMenu(teachersDays);
                }
            }
        }
        private void ButtonHelp_Click(object sender, RoutedEventArgs e)
        {
            var HelpText = "\tIn acest meniu se pot efectua urmatoarele actiuni: Asociere Prioritati Sali Profesor, Asociere Prioritati Module Profesor, Asociere Prioritati Zile Profesor, Adaugare/Editare/Stergere Profesor.\r\n" +
                "\tPrioritatile sunt in ordine crescatoare si astfel prioritatea 0 este cea mai mare.\r\n";
            HelpScreen help = new HelpScreen();
            help.TitleHelp.Text = "Teachers Collection Help";
            help.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            help.TextHelp.Text = HelpText;
            help.Show();
        }
    }
}
