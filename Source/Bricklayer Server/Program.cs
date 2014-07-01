#region Usings
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
#endregion

namespace Bricklayer.Server
{
    public static class Program
    {
        #region Properties
        /// <summary>
        /// The application's server instance
        /// </summary>
        public static Server Server { get; set; }
        #endregion

        #region Fields
        private static readonly string title = Console.Title = "Bricklayer Server - " + AssemblyVersionName.GetVersion();
        #endregion

        static void Main(string[] args)
        {
            //Setup console events
            consoleHandler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(consoleHandler, true);

            //Setup forms stuff, to possibly be used if an error occurs and a dialog needs to be opened
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            Console.Title = title;

            //Only show error screen in release builds
            #if DEBUG
                Server = new Server();
                Server.Run();
            #else
                try
                {
                    Server = new Server();
                    Server.Run();
                }
                catch (Exception e)
                {
                    //Open all exceptions in an error dialog
                   System.Windows.Forms.Application.Run(new Bricklayer.Common.ExceptionForm(e));
            }
            #endif
        }

        #region Console Events
        //Console Events
        private static ConsoleEventDelegate consoleHandler;
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
        /// <summary>
        /// Used to handle events from the console
        /// </summary>
        private static bool ConsoleEventCallback(int eventType)
        {
            if (eventType == 2)
            {
                Server.NetManager.Shutdown("Server Closed by Operator"); //Console closing
            }
            return false;
        }
        #endregion
    }
}
