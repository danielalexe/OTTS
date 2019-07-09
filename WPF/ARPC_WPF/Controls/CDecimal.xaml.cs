using MahApps.Metro.Controls;
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
    /// Interaction logic for CDecimal.xaml
    /// </summary>
    public partial class CDecimal : UserControl
    {
        public CDecimal()
        {
            InitializeComponent();
        }

        public double? CValue
        {
            get
            {
                return CDecimalValue.Value;
            }
            set
            {
                CDecimalValue.Value = value;
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

        public NumericUpDown CNumericUpDown
        {
            get
            {
                return CDecimalValue;
            }
            set
            {
                CDecimalValue = value;
            }
        }
    }
}
