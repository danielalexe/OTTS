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
    /// Interaction logic for CCombo.xaml
    /// </summary>
    public partial class CCombo : UserControl
    {
        public CCombo()
        {
            InitializeComponent();
        }

        public object CValue
        {
            get
            {
                return CComboValue.SelectedValue;
            }
            set
            {
                CComboValue.SelectedValue = value;
            }
        }

        public object CLabel
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

        public ComboBox CComboBox
        {
            get
            {
                return CComboValue;
            }
            set
            {
                CComboValue = value;
            }
        }
    }
}
