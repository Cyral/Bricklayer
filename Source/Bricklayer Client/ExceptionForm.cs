#region Usings
using System;
using System.Diagnostics;
using System.Management;
using System.Text;
using System.Threading;
using System.Windows.Forms;
#endregion

namespace BricklayerClient
{
    /// <summary>
    /// Error report dialog which gives the user options on sending in the report.
    /// A crashlog with relevant information is automaticly generated.
    /// </summary>
    /// <author>Cyral</author>
    public partial class ExceptionForm : Form
    {
        #region Constants
        public const string GithubLink = "https://github.com/Cyral/Bricklayer/issues/new";
        public const string ChatLink = "http://coldstorm.tk/#/login?channels=OpenEE";
        #endregion

        /// <summary>
        /// Create a new error report dialog from a runtime exception
        /// </summary>
        /// <param name="e">The exception to generate a crashlog from, using the message and stacktrace</param>
        public ExceptionForm(Exception exception)
        {
            Random r = new Random();
            InitializeComponent();
            errorBox.Text = "Collecting system information...";
            string[] reasons = new string[] { "Blame Cyral", "Sorry :(", "Darn Bugs...", "Report this!", "Ooops!", "Our bad...", "Please wait!", "We broke it.", "It crashed :|" };
            errorBox.Text += Environment.NewLine + reasons[r.Next(0, reasons.Length)];
            Thread CollectInfo = new Thread(delegate()
            {
                CollectSystemInfo(exception);
            });
            CollectInfo.Start();
        }
        private void CollectSystemInfo(Exception exception)
        {

            //Use a stringbuilder for more efficient string concatenation.
            StringBuilder sb = new StringBuilder();

            //Print basic infomation
            sb.AppendLine("---Automatically generated crashlog, please attach to post when reporting---");
            sb.AppendLine("Date: " + DateTime.Now.ToString("F"));
            sb.AppendLine("Version: " + AssemblyVersionName.GetVersion());
            sb.AppendLine("");

            //Print system info, catching any random exceptions that might happen
            try
            {
                sb.AppendLine("OS: " + GetOS());
                sb.AppendLine("Graphics: " + GetGraphics());
                sb.AppendLine("Processor: " + GetProcessor());
                sb.AppendLine("Installed RAM: " + Math.Round(new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory / 1024f / 1024f / 1024f, 2) + "GB");
            }
            catch
            {
                sb.AppendLine("Error while fetching system information");
            }

            //Append the entire exception (Info, Message, Stacktrace)
            sb.AppendLine("");
            sb.AppendLine(exception.ToString());

            errorBox.Invoke(new Action(() => errorBox.Text = sb.ToString()));
            //errorBox.Text = sb.ToString();
        }

        #region Get System Info
        private string GetProcessor()
        {
            string Processor = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (ManagementObject managementObject in searcher.Get())
            {
                if (managementObject["Name"] != null)
                    Processor = managementObject["Name"].ToString();

                Processor += " " + System.Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE") + " (";
                if (managementObject["NumberOfCores"] != null)
                    Processor += managementObject["NumberOfCores"].ToString() + " Core(s), ";
            }
            foreach (ManagementObject managementObject in new System.Management.ManagementObjectSearcher("Select * from Win32_ComputerSystem").Get())
            {
                if (managementObject["NumberOfProcessors"] != null)
                    Processor += managementObject["NumberOfProcessors"].ToString() + " Physical, ";
                if (managementObject["NumberOfLogicalProcessors"] != null)
                    Processor += managementObject["NumberOfLogicalProcessors"].ToString() + " Logical";
            }
            Processor += ")";
            return Processor;
        }
        private string GetGraphics()
        {

            string GraphicsCard = string.Empty;
            string Frequency = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DisplayConfiguration");
            foreach (ManagementObject managementObject in searcher.Get())
            {
                if (managementObject["Description"] != null)
                    GraphicsCard = managementObject["Description"].ToString();
                if (managementObject["DisplayFrequency"] != null)
                    Frequency = " (" + managementObject["DisplayFrequency"].ToString() + "Hz)";
            }
            return GraphicsCard + Frequency;
        }
        private string GetOS()
        {
            string OS = string.Empty;
            string Architecture = string.Empty;
            string Version = string.Empty;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            foreach (ManagementObject managementObject in searcher.Get())
            {
                if (managementObject["Caption"] != null)
                    OS = managementObject["Caption"].ToString();   //Display operating system caption
                if (managementObject["OSArchitecture"] != null)
                    Architecture = managementObject["OSArchitecture"].ToString();   //Display operating system architecture.
                if (managementObject["CSDVersion"] != null)
                    Version = managementObject["CSDVersion"].ToString(); //Display operating system version.
            }
            return OS.Trim() + " " + Architecture + ", " + Version;
        }
        #endregion

        #region Events
        private void btnGithub_Click(object sender, EventArgs e)
        {
            Process.Start(GithubLink);
        }
        private void btnChat_Click(object sender, EventArgs e)
        {
            Process.Start(ChatLink);
        }
        private void btnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(errorBox.Text);
            //Select all text just to give feeling and responsiveness of "copying"
            errorBox.SelectAll();
        }
        #endregion

        private void ExceptionForm_Load(object sender, EventArgs e)
        {

        }
    }
}
