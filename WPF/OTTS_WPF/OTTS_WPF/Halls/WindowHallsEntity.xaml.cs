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

namespace OTTS_WPF.Halls
{
    /// <summary>
    /// Interaction logic for WindowHallsEntity.xaml
    /// </summary>
    public partial class WindowHallsEntity : Window
    {
        public WindowHallsCollection WindowHallsCollection { get; set; }
        public MainScreen MainScreen { get; set; }
        public EnumWindowType WindowType { get; set; }
        public int ID_HALL { get; set; }
        public WindowHallsEntity()
        {
            InitializeComponent();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (WindowType == EnumWindowType.ADDMODE)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    HALLS hall = new HALLS();
                    hall.nvNAME = CTextName.CTextBox.Text;
                    hall.iCAPACITY = Convert.ToInt32(CDecimalMinimumCapacity.CNumericUpDown.Value);

                    hall.bACTIVE = true;
                    hall.dtCREATE_DATE = DateTime.UtcNow;
                    hall.iCREATE_USER = PersistentData.LoggedUser.iID_USER;

                    db.HALLS.Add(hall);
                    db.SaveChanges();
                    WindowHallsCollection.ReloadData();
                    CloseWindow();
                }
            }
            else if (WindowType == EnumWindowType.EDITMODE)
            {
                using (var db = new OTTSContext(PersistentData.ConnectionString))
                {
                    var getHall = db.HALLS.FirstOrDefault(z => z.iID_HALL == ID_HALL && z.bACTIVE == true);
                    if (getHall != null)
                    {
                        getHall.nvNAME = CTextName.CTextBox.Text;
                        getHall.iCAPACITY = Convert.ToInt32(CDecimalMinimumCapacity.CNumericUpDown.Value);

                        getHall.dtLASTMODIFIED_DATE = DateTime.UtcNow;
                        getHall.iLASTMODIFIED_USER = PersistentData.LoggedUser.iID_USER;

                        db.SaveChanges();
                        WindowHallsCollection.ReloadData();
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
                    var getHall = db.HALLS.FirstOrDefault(z => z.iID_HALL == ID_HALL && z.bACTIVE == true);
                    if (getHall != null)
                    {
                        CTextName.CTextBox.Text = getHall.nvNAME;
                        CDecimalMinimumCapacity.CNumericUpDown.Value = getHall.iCAPACITY;
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
