using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        private const string AUDACITY_FILE = @"<?xml version=|1.0| standalone=|no| ?>
<!DOCTYPE project PUBLIC |-//audacityproject-1.3.0//DTD//EN| |http://audacity.sourceforge.net/xml/audacityproject-1.3.0.dtd| >
<project xmlns=|http://audacity.sourceforge.net/xml/| projname=|empty_data| version=|1.3.0| audacityversion=|2.0.5| sel0=|0.0000000000| sel1=|0.0000000000| vpos=|0| h=|0.0000000000| zoom=|86.1328125000| rate=|44100.0|>
	<tags/>
</project>";


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var queryString = new NameValueCollection();

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                queryString = HttpUtility.ParseQueryString(ApplicationDeployment.CurrentDeployment.ActivationUri.Query);
            }
            else
            { 
                //for testing
                //Create Test
                queryString.Add("action", "Create");
                queryString.Add("path", @"C:\Users\SHAWN\temp\test\foo.aup");

                //Copy Test
                //queryString.Add("action", "Copy");
                //queryString.Add("src", @"C:\Users\SHAWN\temp\test\foo.aup");
                //queryString.Add("dest", @"C:\Users\SHAWN\temp\test2\foo.aup");
            }

            try
            {
                if (queryString["action"].Equals("Create", StringComparison.InvariantCultureIgnoreCase))
                {
                    CreateAudacityAndLaunch(queryString);
                }
                else if (queryString["action"].Equals("Copy", StringComparison.InvariantCultureIgnoreCase))
                {
                    CopyFileAndLaunch(queryString);
                }
                else
                {
                    MessageBox.Show("I dont know what you want me to do!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void CreateAudacityAndLaunch(NameValueCollection queryString)
        {
            //convert the file name into a file info object
            var fileInfo = new FileInfo(queryString["path"]);

            //Ensure that the directory structure is in place
            Directory.CreateDirectory(fileInfo.DirectoryName);

            if (!fileInfo.Exists)
            {
                //Create the audacity file
                File.WriteAllText(fileInfo.FullName, AUDACITY_FILE.Replace("|", "\"").Replace("empty", GetAudacityFileNameRoot(fileInfo)));

                //create the empty folder to match the destination
                Directory.CreateDirectory(GetAudacityDataFolderPath(fileInfo));
            }

            //Launch it
            System.Diagnostics.Process.Start(fileInfo.FullName);

            //Close
            Close();
        }

        private void CopyFileAndLaunch(NameValueCollection queryString)
        {
            //convert the file name into a file info object
            var srcFileInfo = new FileInfo(queryString["src"]);
            var destFileInfo = new FileInfo(queryString["dest"]);

            //Ensure that the directory structure is in place
            Directory.CreateDirectory(destFileInfo.DirectoryName);

            if (!destFileInfo.Exists)
            {
                File.Copy(srcFileInfo.FullName, destFileInfo.FullName);

                DirectoryCopy(GetAudacityDataFolderPath(srcFileInfo), GetAudacityDataFolderPath(destFileInfo), true);
            }

            //Launch it
            System.Diagnostics.Process.Start(destFileInfo.FullName);

            //Close
            Close();
        }

        private string GetAudacityFileNameRoot(FileInfo fi)
        {
            return System.IO.Path.GetFileNameWithoutExtension(fi.Name);
        }

        private string GetAudacityDataFolderPath(FileInfo fi)
        {
            return System.IO.Path.Combine(fi.DirectoryName, GetAudacityFileNameRoot(fi) + "_data");
        }


        private void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = System.IO.Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = System.IO.Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}