using DataLink;
using DataObjects;
using OTTS_WPF.Helpers;
using OTTS_WPF.Template;
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
    /// Interaction logic for WindowLecturesCollection.xaml
    /// </summary>
    public partial class WindowLecturesCollection : WindowBase
    {
        public MainScreen MainScreen { get; set; }
        public WindowLecturesCollection()
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
            if (SelectedGroup.iID_GROUP==-1)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    var FilterName = CTextName.CString.ToUpper();
                    var getLectures = (from u in db.LECTURES
                                       where (
                                       (String.IsNullOrEmpty(FilterName) || u.nvNAME.Contains(FilterName))
                                       &&
                                       (u.iID_SEMESTER == PersistentData.SelectedSemester)
                                       &&
                                       u.bACTIVE == true)
                                       select new DTOLecture
                                       {
                                           iID_LECTURE = u.iID_LECTURE,
                                           NAME = u.nvNAME
                                       }).ToList();
                    DataGridLectures.ItemsSource = getLectures;
                    RenderColumns();
                }
            }
            else
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    var FilterName = CTextName.CString.ToUpper();
                    var getLectures = (from u in db.LECTURES
                                       from z in db.GROUPS_LECTURES_LINK
                                       where (
                                       (String.IsNullOrEmpty(FilterName) || u.nvNAME.Contains(FilterName))
                                       &&
                                       (u.iID_SEMESTER == PersistentData.SelectedSemester)
                                       &&
                                       u.bACTIVE == true
                                       &&
                                       z.bACTIVE==true
                                       &&
                                       z.iID_GROUP == SelectedGroup.iID_GROUP
                                       && 
                                       z.iID_SEMESTER == PersistentData.SelectedSemester
                                       &&
                                       z.iID_LECTURE == u.iID_LECTURE
                                       )
                                       select new DTOLecture
                                       {
                                           iID_LECTURE = u.iID_LECTURE,
                                           NAME = u.nvNAME
                                       }).ToList();
                    DataGridLectures.ItemsSource = getLectures;
                    RenderColumns();
                }
            }
            
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            var list = DataGridLectures.SelectedItems;
            if (list.Count == 0)
            {
                MessageBox.Show("Nici un rand nu este selectat");
                return;
            }
            else
            {
                if (list.Count > 0)
                {
                    if (MessageBox.Show("Sunteti siguri ca vreti sa stergeti randurile selectate?", "Atentie", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        using (var db = new OTTSContext(PersistentData.ConnectionString))
                        {
                            foreach (DTOLecture item in list)
                            {
                                var getLecture = db.LECTURES.FirstOrDefault(z => z.iID_LECTURE == item.iID_LECTURE && z.bACTIVE == true);
                                if (getLecture != null)
                                {
                                    getLecture.bACTIVE = false;
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

            var list = DataGridLectures.SelectedItems;
            if (list.Count == 0)
            {
                MessageBox.Show("Nici un rand nu este selectat");
                return;
            }
            else
            {
                if (list.Count > 1)
                {
                    MessageBox.Show("Modificarile asupra unei prelegeri se poate face doar la o singura prelegere simultan");
                    return;
                }
                else
                {
                    WindowLecturesEntity lecturesEntity = new WindowLecturesEntity();
                    lecturesEntity.MainScreen = MainScreen;
                    lecturesEntity.WindowType = Helpers.EnumWindowType.EDITMODE;
                    lecturesEntity.ID_LECTURE = ((DTOLecture)DataGridLectures.SelectedItem).iID_LECTURE;
                    lecturesEntity.WindowLecturesCollection = this;
                    lecturesEntity.LoadData();
                    MainScreen.RaiseDownMenu(lecturesEntity, Helpers.EnumWindowType.EDITMODE);
                }
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            WindowLecturesEntity lecturesEntity = new WindowLecturesEntity();
            lecturesEntity.MainScreen = MainScreen;
            lecturesEntity.WindowType = Helpers.EnumWindowType.ADDMODE;
            lecturesEntity.WindowLecturesCollection = this;
            MainScreen.RaiseDownMenu(lecturesEntity, Helpers.EnumWindowType.ADDMODE);
        }


        private void RenderColumns()
        {
            foreach (DataGridColumn c in DataGridLectures.Columns)
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
    }
}
