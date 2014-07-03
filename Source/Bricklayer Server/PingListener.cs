#region Usings
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Bricklayer.Common.Data;
using Bricklayer.Common.Networking;
#endregion

namespace Bricklayer.Server
{
    /// <summary>
    /// Handles incoming ping requests for server data such as players online, motd, etc
    /// </summary>
    public class PingListener
    {
        #region Fields
        private Thread listenerThread;
        private int port;
        private TcpClient client;
        private TcpListener server;
        private NetworkStream stream;
        #endregion

        #region Methods
        /// <summary>
        /// Creates a new listener to listen for TCP messages from clients that want to check server status
        /// </summary>
        /// <param name="port">TThe port to listen on</param>
        public PingListener(int port)
        {
            this.port = port;
        }

        /// <summary>
        /// Starts listening on a new thread for ping requests
        /// </summary>
        public void Start()
        {
            listenerThread = new Thread(() => Listen()); //Start the TcpClient on a new thread
            listenerThread.Name = "PingListener";
            listenerThread.Start();
            Log.WriteLine(ConsoleColor.Green, "PingListener started, Listening for query requests.\n"); //Log message
        }

        /// <summary>
        /// Listens for ping requests, and sends back statistics
        /// </summary>
        private void Listen()
        {
            try
            {
                server = new TcpListener(IPAddress.Any, port); //Create a TcpListener to listen for requests
                server.Start(); //Start listening for client requests.

                byte[] bytes = new byte[1]; //Single byte buffer for reading data (should not exceed 1 byte)

                //Enter the listening loop.
                while (true)
                {
                    client = server.AcceptTcpClient(); //Wait and accept requests
                    stream = client.GetStream(); //Get a stream object for reading and writing
                    int i;

                    //Loop to receive all the data sent by the client
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        Debug.WriteLine(String.Format("PingListener: Data Received: {0}", bytes.ToString()));
                        //If ping request recieved, send back the data (0 is the ping packet id)
                        if ((byte)bytes[0] == 0x00)
                        {
                            Debug.WriteLine("PingListener: Data validated, query request recieved.");
                            //Get server stats to send back
                            ServerPingData pingData = Server.NetManager.GetQuery();
                            //Write the data to a steam and send
                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (StreamWriter writer = new StreamWriter(ms))
                                {
                                    writer.WriteLine((byte)pingData.Online);
                                    writer.WriteLine((byte)pingData.MaxOnline);
                                    writer.WriteLine(pingData.Description);
                                }
                                byte[] msg = ms.ToArray();
                                stream.Write(msg, 0, msg.Length);
                            }
                        }
                    }

                    // Shutdown and end connection
                    client.Close();
                }
            }
            catch (Exception e)
            {
                Log.WriteLine(LogType.Error, "PingListener Error: {0}", e);
            }
        }
        #endregion
    }
}