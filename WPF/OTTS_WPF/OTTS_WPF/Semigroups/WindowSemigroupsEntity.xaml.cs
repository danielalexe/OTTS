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

namespace OTTS_WPF.Semigroups
{
    /// <summary>
    /// Interaction logic for WindowSemigroupsEntity.xaml
    /// </summary>
    public partial class WindowSemigroupsEntity : Window
    {
        public WindowSemigroupsCollection WindowSemigroupsCollection { get; set; }
        public MainScreen MainScreen { get; set; }
        public EnumWindowType WindowType { get; set; }
        public int ID_SEMIGROUP { get; set; }
        public WindowSemigroupsEntity()
        {
            InitializeComponent();
            BindComboGroup();
            LoadData();
        }

        private void BindComboGroup()
        {
            List<DTOGroup> list = new List<DTOGroup>();
            DTOGroup dto = new DTOGroup();
            dto.iID_GROUP = -1;
            dto.nvCOMBO_DISPLAY = "All";
            list.Add(dto);
            using (var db = new OTTSContext(PersistentData.ConnectionString))
            {
                var getGroup = (from u in db.GROUPS
                                where u.bACTIVE == true
                                select new DTOGroup
                                {
                                    iID_GROUP = u.iID_GROUP,
                                    NAME = u.nvNAME,
                                    nvCOMBO_DISPLAY = u.nvNAME
                                }).ToList();
                list.AddRange(getGroup);
            }
            CComboGroup.CComboBox.ItemsSource = list;
            CComboGroup.CComboBox.SelectedItem = list.FirstOrDefault();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (WindowType == EnumWindowType.ADDMODE)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    SEMIGROUPS semigroup = new SEMIGROUPS();
                    semigroup.nvNAME = CTextName.CTextBox.Text;
                    semigroup.iID_GROUP = ((DTOGroup)CComboGroup.CComboBox.SelectedItem).iID_GROUP;
                    semigroup.iPRIORITY = Convert.ToInt32(CDecimalPriority.CValue.Value);

                    semigroup.bACTIVE = true;
                    semigroup.dtCREATE_DATE = DateTime.UtcNow;
                    semigroup.iCREATE_USER = PersistentData.LoggedUser.iID_USER;

                    db.SEMIGROUPS.Add(semigroup);
                    db.SaveChanges();
                    WindowSemigroupsCollection.ReloadData();
                    CloseWindow();
                }
            }
            else if (WindowType == EnumWindowType.EDITMODE)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    var getSemigroup = db.SEMIGROUPS.FirstOrDefault(z => z.iID_SEMIGROUP == ID_SEMIGROUP && z.bACTIVE == true);
                    if (getSemigroup != null)
                    {
                        getSemigroup.nvNAME = CTextName.CTextBox.Text;
                        getSemigroup.iID_GROUP = ((DTOGroup)CComboGroup.CComboBox.SelectedItem).iID_GROUP;
                        getSemigroup.iPRIORITY = Convert.ToInt32(CDecimalPriority.CValue.Value);

                        getSemigroup.dtLASTMODIFIED_DATE = DateTime.UtcNow;
                        getSemigroup.iLASTMODIFIED_USER = PersistentData.LoggedUser.iID_USER;

                        db.SaveChanges();
                        WindowSemigroupsCollection.ReloadData();
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
                    var getSemigroup = db.SEMIGROUPS.FirstOrDefault(z => z.iID_SEMIGROUP == ID_SEMIGROUP && z.bACTIVE == true);
                    if (getSemigroup != null)
                    {
                        CTextName.CTextBox.Text = getSemigroup.nvNAME;
                        CComboGroup.CComboBox.SelectedItem = ((List<DTOGroup>)CComboGroup.CComboBox.ItemsSource).FirstOrDefault(z => z.iID_GROUP == getSemigroup.iID_GROUP);
                        CDecimalPriority.CValue = getSemigroup.iPRIORITY;
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
