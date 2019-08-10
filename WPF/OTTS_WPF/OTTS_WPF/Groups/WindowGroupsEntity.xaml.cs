using DataLink;
using DataObjects;
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

namespace OTTS_WPF.Groups
{
    /// <summary>
    /// Interaction logic for WindowGroupsEntity.xaml
    /// </summary>
    public partial class WindowGroupsEntity : Window
    {
        public WindowGroupsCollection WindowGroupsCollection { get; set; }
        public MainScreen MainScreen { get; set; }
        public EnumWindowType WindowType { get; set; }
        public int ID_GROUP { get; set; }
        public WindowGroupsEntity()
        {
            InitializeComponent();
            BindComboGroupType();
        }
        private void BindComboGroupType()
        {
            List<DTOGroupType> list = new List<DTOGroupType>();
            DTOGroupType dto = new DTOGroupType();
            dto.iID_GROUP_TYPE = -1;
            dto.nvCOMBO_DISPLAY = "All";
            list.Add(dto);
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                var getGroupTypes = (from u in db.GROUP_TYPES
                                     where u.bACTIVE == true
                                     select new DTOGroupType
                                     {
                                         iID_GROUP_TYPE = u.iID_GROUP_TYPE,
                                         nvNAME = u.nvNAME,
                                         nvCOMBO_DISPLAY = u.nvNAME
                                     }).ToList();
                list.AddRange(getGroupTypes);
            }
            CComboGroupType.CComboBox.ItemsSource = list;
            CComboGroupType.CComboBox.SelectedItem = list.FirstOrDefault();
        }


        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (WindowType == EnumWindowType.ADDMODE)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    GROUPS group = new GROUPS();
                    group.nvNAME = CTextName.CTextBox.Text;
                    group.iID_GROUP_TYPE = ((DTOGroupType)CComboGroupType.CComboBox.SelectedItem).iID_GROUP_TYPE;
                    group.iYEAR = Convert.ToInt32(CDecimalYear.CValue.Value);
                    group.iNUMBER_OF_STUDENTS = Convert.ToInt32(CDecimalNumberStudents.CValue.Value);

                    group.bACTIVE = true;
                    group.dtCREATE_DATE = DateTime.UtcNow;
                    group.iCREATE_USER = PersistentData.LoggedUser.iID_USER;

                    db.GROUPS.Add(group);
                    db.SaveChanges();
                    WindowGroupsCollection.ReloadData();
                    CloseWindow();
                }
            }
            else if (WindowType == EnumWindowType.EDITMODE)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    var getGroup = db.GROUPS.FirstOrDefault(z => z.iID_GROUP == ID_GROUP && z.bACTIVE == true);
                    if (getGroup != null)
                    {
                        getGroup.nvNAME = CTextName.CTextBox.Text;
                        getGroup.iID_GROUP_TYPE = ((DTOGroupType)CComboGroupType.CComboBox.SelectedItem).iID_GROUP_TYPE;
                        getGroup.iYEAR = Convert.ToInt32(CDecimalYear.CValue.Value);
                        getGroup.iNUMBER_OF_STUDENTS = Convert.ToInt32(CDecimalNumberStudents.CValue.Value);

                        getGroup.dtLASTMODIFIED_DATE = DateTime.UtcNow;
                        getGroup.iLASTMODIFIED_USER = PersistentData.LoggedUser.iID_USER;

                        db.SaveChanges();
                        WindowGroupsCollection.ReloadData();
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
                    var getGroup = db.GROUPS.FirstOrDefault(z => z.iID_GROUP == ID_GROUP && z.bACTIVE == true);
                    if (getGroup != null)
                    {
                        CTextName.CTextBox.Text = getGroup.nvNAME;
                        CComboGroupType.CComboBox.SelectedItem = ((List<DTOGroupType>) CComboGroupType.CComboBox.ItemsSource).FirstOrDefault(z=>z.iID_GROUP_TYPE==getGroup.iID_GROUP_TYPE);
                        CDecimalNumberStudents.CValue = getGroup.iNUMBER_OF_STUDENTS;
                        CDecimalYear.CValue = getGroup.iYEAR;
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
