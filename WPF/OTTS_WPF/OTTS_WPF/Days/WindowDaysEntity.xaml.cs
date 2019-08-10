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

namespace OTTS_WPF.Days
{
    /// <summary>
    /// Interaction logic for WindowDaysEntity.xaml
    /// </summary>
    public partial class WindowDaysEntity : Window
    {
        public WindowDaysCollection WindowDaysCollection { get; set; }
        public MainScreen MainScreen { get; set; }
        public EnumWindowType WindowType { get; set; }
        public int ID_DAY { get; set; }
        public WindowDaysEntity()
        {
            InitializeComponent();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (WindowType == EnumWindowType.ADDMODE)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    DAYS day = new DAYS();
                    day.nvNAME = CTextName.CTextBox.Text;
                    day.iPRIORITY = Convert.ToInt32(CDecimalPriority.CNumericUpDown.Value);

                    day.bACTIVE = true;
                    day.dtCREATE_DATE = DateTime.UtcNow;
                    day.iCREATE_USER = PersistentData.LoggedUser.iID_USER;

                    db.DAYS.Add(day);
                    db.SaveChanges();
                    WindowDaysCollection.ReloadData();
                    CloseWindow();
                }
            }
            else if (WindowType == EnumWindowType.EDITMODE)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    var getDay = db.DAYS.FirstOrDefault(z => z.iID_DAY == ID_DAY && z.bACTIVE == true);
                    if (getDay != null)
                    {
                        getDay.nvNAME = CTextName.CTextBox.Text;
                        getDay.iPRIORITY = Convert.ToInt32(CDecimalPriority.CNumericUpDown.Value);

                        getDay.dtLASTMODIFIED_DATE = DateTime.UtcNow;
                        getDay.iLASTMODIFIED_USER = PersistentData.LoggedUser.iID_USER;

                        db.SaveChanges();
                        WindowDaysCollection.ReloadData();
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
                    var getDay = db.DAYS.FirstOrDefault(z => z.iID_DAY == ID_DAY && z.bACTIVE == true);
                    if (getDay != null)
                    {
                        CTextName.CTextBox.Text = getDay.nvNAME;
                        CDecimalPriority.CNumericUpDown.Value = getDay.iPRIORITY;
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
