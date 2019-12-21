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

namespace OTTS_WPF
{
    /// <summary>
    /// Interaction logic for LoadingScreen.xaml
    /// </summary>
    public partial class LoadingScreen : Window
    {
        public LoadingScreen()
        {
            InitializeComponent();
            ElapsedTime = 0;
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }
        System.Windows.Threading.DispatcherTimer dispatcherTimer;
        int ElapsedTime;
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            ElapsedTime++;
            TimeoutText.Text="Timeout in "+(60-ElapsedTime).ToString()+" seconds";
            if (ElapsedTime>=60)
            {
                dispatcherTimer.Stop();
                TimeoutText.Text = "Aplicatia cel mai probabil s-a blocat. Apasati butonul de mai jos pentru a inchide aplicatia.";
            }
        }

        private void ButtonAbort_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
