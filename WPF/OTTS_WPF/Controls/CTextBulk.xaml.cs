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
    /// Interaction logic for CTextBulk.xaml
    /// </summary>
    public partial class CTextBulk : UserControl
    {
        public CTextBulk()
        {
            InitializeComponent();
        }
        public string CValue
        {
            get
            {
                return CTextBlockValue.Text;
            }
            set
            {
                CTextBlockValue.Text = value;
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

        public TextBlock CTextBlock
        {
            get
            {
                return CTextBlockValue;
            }
            set
            {
                CTextBlockValue = value;
            }
        }
    }
}
