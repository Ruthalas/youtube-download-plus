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


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string strCommand = "W:\\VIDEO\\OTHER\\YOUTUBE Staging\\youtube-dl.exe";
            string strCommandParameters = "--version";
            string strWorkingDirectory = "W:\\VIDEO\\OTHER\\YOUTUBE Staging";

            //Indicate the start of run...
            textBlock2.Text = textBlock2.Text + "Running...&#10;";

            //Create process
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();

            //Set the window to run hidden?
            pProcess.StartInfo.CreateNoWindow = true;

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
