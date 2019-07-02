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

namespace ARPC_WPF.Template
{
    public partial class WindowBase : Window
    {
        public bool NoTab;

        private TabControl tabCtrl;
        private TabItem tabItem;

        public TabItem TabItem
        {
            get
            {
                return tabItem;
            }
            set
            {
                tabItem = value;
            }
        }

        public TabControl TabCtrl
        {
            set
            {
                tabCtrl = value;
            }
            get
            {
                return tabCtrl;
            }
        }
        public WindowBase()
        {
            
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            if (NoTab)
                return;
            if (tabCtrl != null)
            {
                tabCtrl.SelectedItem = tabItem;

                if (tabCtrl.Visibility == Visibility.Collapsed || tabCtrl.Visibility == Visibility.Hidden)
                {
                    tabCtrl.Visibility = Visibility.Visible;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (tabCtrl == null)
                return;
            this.tabCtrl.Items.Remove(tabItem);

            if (!tabCtrl.HasItems)
            {
                tabCtrl.Visibility = Visibility.Collapsed;
            }
        }
    }
}
