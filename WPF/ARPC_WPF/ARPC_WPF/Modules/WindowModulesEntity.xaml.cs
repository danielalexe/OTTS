using DataLink;
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

namespace OTTS_WPF.Modules
{
    /// <summary>
    /// Interaction logic for WindowModulesEntity.xaml
    /// </summary>
    public partial class WindowModulesEntity : Window
    {
        public WindowModulesCollection WindowModulesCollection { get; set; }
        public MainScreen MainScreen { get; set; }
        public EnumWindowType WindowType { get; set; }
        public int ID_MODULE { get; set; }
        public WindowModulesEntity()
        {
            InitializeComponent();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (WindowType == EnumWindowType.ADDMODE)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    MODULES module = new MODULES();
                    module.nvNAME = CTextName.CTextBox.Text;

                    module.bACTIVE = true;
                    module.dtCREATE_DATE = DateTime.UtcNow;
                    module.iCREATE_USER = PersistentData.LoggedUser.iID_USER;

                    db.MODULES.Add(module);
                    db.SaveChanges();
                    WindowModulesCollection.ReloadData();
                    CloseWindow();
                }
            }
            else if (WindowType == EnumWindowType.EDITMODE)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    var getModule = db.MODULES.FirstOrDefault(z => z.iID_MODULE == ID_MODULE && z.bACTIVE == true);
                    if (getModule != null)
                    {
                        getModule.nvNAME = CTextName.CTextBox.Text;

                        getModule.dtLASTMODIFIED_DATE = DateTime.UtcNow;
                        getModule.iLASTMODIFIED_USER = PersistentData.LoggedUser.iID_USER;

                        db.SaveChanges();
                        WindowModulesCollection.ReloadData();
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
                    var getModule = db.MODULES.FirstOrDefault(z => z.iID_MODULE == ID_MODULE && z.bACTIVE == true);
                    if (getModule != null)
                    {
                        CTextName.CTextBox.Text = getModule.nvNAME;
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
