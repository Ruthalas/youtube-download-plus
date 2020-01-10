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

            // Begin construction of youtube-dl command with calling it's location (per settings)
            string strCommand = "W:\\VIDEO\\YOUTUBE\\youtube-dl.exe";

            // Add URL to youtube-dl command
            string strCommandParameters = tbURL.Text;
            // Add ignore errors command
            strCommandParameters = strCommandParameters + " -i";
            // Add output file-name formatting string
            strCommandParameters = strCommandParameters + " -o \"%(uploader)s [$(channel_id)s]/%(upload_date)s - %(title)s - (%(duration)ss) [%(id)s].%(ext)s\"";
            // Add quality parameters (for absolute best)
            strCommandParameters = strCommandParameters + " -f bestvideo+bestaudio --youtube-include-dash-manifest";
            // Add output container (MKV)
            strCommandParameters = strCommandParameters + " --merge-output mkv";
            // Add metdata to file to command
            strCommandParameters = strCommandParameters + " --add-metadata";
            // If user has checked the 'Download Description' checkbox, add that to the youtube-dl command
            if (cbDescription.IsChecked ?? false)
            {
                strCommandParameters = strCommandParameters + " --write-description";
            }
            if (cbThumbnail.IsChecked ?? false)
            {
                strCommandParameters = strCommandParameters + " --write-thumbnail";
            }
            // If user has checked the 'Download Subtitles' checkbox, add that to the command
                if (cbSubs.IsChecked ?? false)
            {
                strCommandParameters = strCommandParameters + " --all-subs --embed-subs";
            }

            //MessageBox.Show(strCommandParameters);

            // Let's get this working directory sorted out. First let's get a variable set up for it.
            string strWorkingDirectory = "";
            // Then let's check whether the path provided by the user in the textbox is valid
            if (System.IO.Directory.Exists(tbPath.Text))
            {
                // If valid, use it
                textBlock2.Text = textBlock2.Text + "Provided path is valid, using it for download.\n";
                strWorkingDirectory = tbPath.Text;
            }
            else
            {
                // If not valid, fetch the directory the program was run in, and use it instead
                textBlock2.Text = textBlock2.Text + "Provided path isn't valid, falling back to current directory.\n";
                strWorkingDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
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
