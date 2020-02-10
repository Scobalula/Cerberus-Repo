using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Cerberus.Logic;

namespace Cerberus.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Exports selected assets on double click
        /// </summary>
        private void ListViewItemMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)Current.MainWindow;

            mainWindow.ListViewDoubleClick(sender, e);
        }
    }
}
