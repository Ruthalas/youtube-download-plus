using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Youtube_Downloader_Plus
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

        private void Get_Version(object sender, RoutedEventArgs e)
        {
            string strCommand = "W:\\VIDEO\\OTHER\\YOUTUBE Staging\\youtube-dl.exe";
            string strCommandParameters = "--version";
            string strWorkingDirectory = "W:\\VIDEO\\OTHER\\YOUTUBE Staging";

            //Create process
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            //Set the window to run hidden
            pProcess.StartInfo.CreateNoWindow = false;
            //strCommand is path and file name of command to run
            pProcess.StartInfo.FileName = strCommand;
            //strCommandParameters are parameters to pass to program
            pProcess.StartInfo.Arguments = strCommandParameters;
            pProcess.StartInfo.UseShellExecute = false;
            //Set output of program to be written to process output stream
            pProcess.StartInfo.RedirectStandardOutput = true;
            //Optional
            pProcess.StartInfo.WorkingDirectory = strWorkingDirectory;
            //Start the process
            pProcess.Start();
            //Get program output
            string strOutput = pProcess.StandardOutput.ReadToEnd();
            textBlock2.Text = textBlock2.Text + strOutput;

            //Wait for process to finish
            pProcess.WaitForExit();
        }

        private void Browse_For_Folder(object sender, RoutedEventArgs e)
        {
            // Create a "Save As" dialog for selecting a directory (HACK)
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.InitialDirectory = ""; // Use current value for initial dir
            dialog.Title = "Select a Directory"; // instead of default "Save As"
            dialog.Filter = "Directory|*.a.directory"; // Prevents displaying files
            dialog.FileName = "select"; // Filename will then be "select.this.directory"
            if (dialog.ShowDialog() == true)
            {
                string path = dialog.FileName;
                // Remove fake filename from resulting path
                path = path.Replace("\\select.a.directory", "");
                path = path.Replace(".a.directory", "");
                // If user has changed the filename, create the new directory
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                // Our final value is in path
                tbPath.Text = path;
            }
        }

        private void Run_Download(object sender, RoutedEventArgs e)
        {
            string strCommand = "W:\\VIDEO\\OTHER\\YOUTUBE Staging\\youtube-dl.exe";
            string strCommandParameters = "https://youtu.be/YE7VzlLtp-4 -i -o \"%(uploader)s/%(upload_date)s - %(title)s - (%(duration)ss) [%(id)s].%(ext)s\" -f bestvideo+bestaudio --youtube-include-dash-manifest --merge-output mkv --write-description --add-metadata --all-subs --embed-subs";
            string strWorkingDirectory = "";

            if (System.IO.Directory.Exists(tbPath.Text))
            {
                strWorkingDirectory = tbPath.Text;
            }
            else
            {
                strWorkingDirectory = "W:\\VIDEO\\OTHER\\YOUTUBE Staging\\!TEMP - Copy";
            }

            //Create process
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();
            //Set the window to run hidden
            pProcess.StartInfo.CreateNoWindow = false;
            //strCommand is path and file name of command to run
            pProcess.StartInfo.FileName = strCommand;
            //strCommandParameters are parameters to pass to program
            pProcess.StartInfo.Arguments = strCommandParameters;
            pProcess.StartInfo.UseShellExecute = false;
            //Set output of program to be written to process output stream
            pProcess.StartInfo.RedirectStandardOutput = true;
            //Optional
            pProcess.StartInfo.WorkingDirectory = strWorkingDirectory;
            //Start the process
            pProcess.Start();
            //Get program output
            string strOutput = pProcess.StandardOutput.ReadToEnd();
            textBlock2.Text = textBlock2.Text + strOutput;

            //Wait for process to finish
            pProcess.WaitForExit();
        }

    }
}
