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

namespace Controls
{
    /// <summary>
    /// Interaction logic for CText.xaml
    /// </summary>
    public partial class CText : UserControl
    {
        public CText()
        {
            InitializeComponent();
        }

        public string CString
        {
            get
            {
                return CStringValue.Text;
            }
            set
            {
                CStringValue.Text = value;
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

        public TextBox CTextBox
        {
            get
            {
                return CStringValue;
            }
            set
            {
                CStringValue = value;
            }
        }
    }
}
