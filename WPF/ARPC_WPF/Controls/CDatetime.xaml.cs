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
    /// Interaction logic for CDatetime.xaml
    /// </summary>
    public partial class CDatetime : UserControl
    {
        public CDatetime()
        {
            InitializeComponent();
        }

        public DateTime? CValue
        {
            get
            {
                return CDateValue.SelectedDate;
            }
            set
            {
                CDateValue.SelectedDate = value;
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

        public DatePicker CDatePicker
        {
            get
            {
                return CDateValue;
            }
            set
            {
                CDateValue = value;
            }
        }
    }
}
