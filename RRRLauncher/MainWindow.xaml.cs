using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RRRLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Get the path from the URL
            var fileName = @"C:\Users\SHAWN\temp\test\test.wav";

            if (ApplicationDeployment.IsNetworkDeployed)
                fileName = HttpUtility.UrlDecode(ApplicationDeployment.CurrentDeployment.ActivationUri.Query.Substring(6));
            
            //convert the file name into a file info object
            var fileInfo = new FileInfo(fileName);

            //Ensure that the directory structure is in place
            Directory.CreateDirectory(fileInfo.DirectoryName);

            if (!fileInfo.Exists)
            {
                //Rename the zero lenghth wav file to the spot that we need it at
                File.Copy("empty.wav", fileInfo.FullName, true);
            }
           
            //Launch it
            System.Diagnostics.Process.Start(fileInfo.FullName);

            //Close
            Close();

        }

    }
}
