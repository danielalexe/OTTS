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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DataControls
{
    /// <summary>
    /// Interaction logic for CUploadFile.xaml
    /// </summary>
    public partial class CUploadFile : UserControl
    {
        public CUploadFile()
        {
            InitializeComponent();
        }
        public object CValue
        {
            get
            {
                return CLabelValue.Content;
            }
            set
            {
                CLabelValue.Content = value;
            }
        }

        public Label CLabel
        {
            get
            {
                return CLabelValue;
            }
            set
            {
                CLabelValue = value;
            }
        }

        public Button CButton
        {
            get
            {
                return CButtonValue;
            }
            set
            {
                CButtonValue = value;
            }
        }
    }
}
