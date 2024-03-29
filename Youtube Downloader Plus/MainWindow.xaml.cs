﻿using System;
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

// I apologize if this is a pile of hot garbage
// This is my first time using both Visual Studio and WPF

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
            // Begin construction of youtube-dlp command by selecting a variant (dlp by default, see settings)
            string strCommand = "";
            if (cbyoutubedl.IsChecked ?? true)
            {
                strCommand = "youtube-dl.exe";
            }
            else
            {
                strCommand = "youtube-dlp.exe";
            }

            string strCommandParameters = "";
            // If user has entered a batch file, use it, otherwise use URL from URL field
            if (batchFile.Text != string.Empty)
            {
                // Add batch file input to command string
                strCommandParameters = strCommandParameters + " --batch-file \"" + batchFile.Text + "\"";
            }
            else
            {
                // Add URL to youtube-dlp command
                strCommandParameters = strCommandParameters + tbURL.Text;
            }
            // Add ignore errors command
            strCommandParameters = strCommandParameters + " -i";
            // Begin output structure
            strCommandParameters = strCommandParameters + " -o";
            // Build output path, adding subfolder if selected
            if (cbSubFolder.IsChecked ?? false)
            {
                strCommandParameters = strCommandParameters + " \"%(uploader)s [%(channel_id)s]/%(upload_date)s - %(title)s - (%(duration)ss) [%(id)s].%(ext)s\"";
            }
            else
            {
                strCommandParameters = strCommandParameters + " \"%(upload_date)s - %(title)s - (%(duration)ss) [%(id)s].%(ext)s\"";
            }
            // Add quality parameters for absolute best, including the manifest allows youtube-dlp to find 4k footage, which is stored differently
            strCommandParameters = strCommandParameters + " --format \"(bestvideo[vcodec = av01][height >= 4320][fps > 30] / bestvideo[vcodec = vp9.2][height >= 4320][fps > 30] / bestvideo[vcodec = vp9][height >= 4320][fps > 30] / bestvideo[vcodec = av01][height >= 4320] / bestvideo[vcodec = vp9.2][height >= 4320] / bestvideo[vcodec = vp9][height >= 4320] / bestvideo[height >= 4320] / bestvideo[vcodec = av01][height >= 2880][fps > 30] / bestvideo[vcodec = vp9.2][height >= 2880][fps > 30] / bestvideo[vcodec = vp9][height >= 2880][fps > 30] / bestvideo[vcodec = av01][height >= 2880] / bestvideo[vcodec = vp9.2][height >= 2880] / bestvideo[vcodec = vp9][height >= 2880] / bestvideo[height >= 2880] / bestvideo[vcodec = av01][height >= 2160][fps > 30] / bestvideo[vcodec = vp9.2][height >= 2160][fps > 30] / bestvideo[vcodec = vp9][height >= 2160][fps > 30] / bestvideo[vcodec = av01][height >= 2160] / bestvideo[vcodec = vp9.2][height >= 2160] / bestvideo[vcodec = vp9][height >= 2160] / bestvideo[height >= 2160] / bestvideo[vcodec = av01][height >= 1440][fps > 30] / bestvideo[vcodec = vp9.2][height >= 1440][fps > 30] / bestvideo[vcodec = vp9][height >= 1440][fps > 30] / bestvideo[vcodec = av01][height >= 1440] / bestvideo[vcodec = vp9.2][height >= 1440] / bestvideo[vcodec = vp9][height >= 1440] / bestvideo[height >= 1440] / bestvideo[vcodec = av01][height >= 1080][fps > 30] / bestvideo[vcodec = vp9.2][height >= 1080][fps > 30] / bestvideo[vcodec = vp9][height >= 1080][fps > 30] / bestvideo[vcodec = av01][height >= 1080] / bestvideo[vcodec = vp9.2][height >= 1080] / bestvideo[vcodec = vp9][height >= 1080] / bestvideo[height >= 1080] / bestvideo[vcodec = av01][height >= 720][fps > 30] / bestvideo[vcodec = vp9.2][height >= 720][fps > 30] / bestvideo[vcodec = vp9][height >= 720][fps > 30] / bestvideo[vcodec = av01][height >= 720] / bestvideo[vcodec = vp9.2][height >= 720] / bestvideo[vcodec = vp9][height >= 720] / bestvideo[height >= 720] / bestvideo) + (bestaudio[acodec = opus] / bestaudio) / best\"";
            // Add output container (MKV)
            strCommandParameters = strCommandParameters + " --merge-output mkv";
            // Add metdata to file to command
            strCommandParameters = strCommandParameters + " --add-metadata";
            // If user has checked the 'Download Description' checkbox, add that to the youtube-dlp command
            if (cbDescription.IsChecked ?? false)
            {
                strCommandParameters = strCommandParameters + " --write-description";
            }
            // If the user has checked the 'Download Thumbnail' checkbox, add that to the command
            if (cbThumbnail.IsChecked ?? false)
            {
                strCommandParameters = strCommandParameters + " --write-thumbnail";
            }
            // If user has checked the 'Download Subtitles' checkbox, add that to the command
            if (cbSubs.IsChecked ?? false)
            {
                strCommandParameters = strCommandParameters + " --all-subs --embed-subs";
            }
            // If user has checked the 'Download JSON' checkbox, add that to the command
            if (cbJson.IsChecked ?? false)
            {
                strCommandParameters = strCommandParameters + " --write-info-json";
            }

            // If user has entered an archive file, use it
            if (archiveFile.Text != string.Empty)
            {
                strCommandParameters = strCommandParameters + " --download-archive \"" + archiveFile.Text + "\"";
            }

            // Uncomment the following line to display the final command. Useful for debugging.
            textBlock2.Text = textBlock2.Text + "Command: " + strCommand + " " + strCommandParameters + "\n";

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
            pProcess.StartInfo.RedirectStandardError = true;
            //Optional
            pProcess.StartInfo.WorkingDirectory = strWorkingDirectory;
            //Start the process
            pProcess.Start();

            //pProcess.MoveWindow(pProcess.MainWindowHandle, posX, 0, 50, 50, true);
            //Get program output
            string strOutput = pProcess.StandardOutput.ReadToEnd();
            textBlock2.Text = textBlock2.Text + strOutput;

            //Wait for process to finish
            pProcess.WaitForExit();
        }
    }
}
