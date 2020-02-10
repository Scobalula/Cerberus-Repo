using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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

namespace Cerberus.UI
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            VersionLabel.Content = string.Format("Version: {0}", Assembly.GetExecutingAssembly().GetName().Version);
        }

        private void DonateButtonClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.paypal.me/scobalula");
        }

        private void HomePageButtonClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://scobalula.github.io/Cerberus-Repo/");
        }
    }
}
