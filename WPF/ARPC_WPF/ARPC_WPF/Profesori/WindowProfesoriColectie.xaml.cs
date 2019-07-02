using ARPC_WPF.Template;
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

namespace ARPC_WPF.Profesori
{
    /// <summary>
    /// Interaction logic for WindowProfesoriColectie.xaml
    /// </summary>
    public partial class WindowProfesoriColectie : WindowBase
    {
        public WindowProfesoriColectie()
        {
            InitializeComponent();
        }

        private void ButonClose_Click(object sender, RoutedEventArgs e)
        {
            this.TabCtrl.Items.Remove(this.TabItem);
        }
    }
}
