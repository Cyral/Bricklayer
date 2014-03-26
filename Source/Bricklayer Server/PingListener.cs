using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Bricklayer.Client.Networking;

namespace Bricklayer.Server
{
    /// <summary>
    /// Handles incoming ping requests for server data such as players online, motd, etc
    /// </summary>
    public class PingListener
    {
        private Thread ListenerThread;
        private int port;

        /// <summary>
        /// Creates a new listener to listen for TCP messages from clients that want to check server status
        /// </summary>
        /// <param name="port">TThe port to listen on</param>
        public PingListener(int port)
        {
            this.port = port;
        }
        public void Start()
        {
            //Start the UdpClient on a new thread
            ListenerThread = new Thread(() => Listen());
            ListenerThread.Start();
            Console.WriteLine("PingListener started, Listening for query requests.");
        }
        /// <summary>
        /// Listens for ping requests, and sends back statistics
        /// </summary>
        private void Listen()
        {
            try
            {
                //Create a TcpListener to listen for requests
                TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                //Start listening for client requests.
                server.Start();

                // Buffer for reading data
                byte[] bytes = new byte[1];
                //Use a client to send a response back
                TcpClient client;
                NetworkStream stream;

                // Enter the listening loop.
                while (true)
                {
                    //Perform a blocking call to accept requests.
                    //You could also user server.AcceptSocket() here.
                    client = server.AcceptTcpClient();

                    //Get a stream object for reading and writing
                    stream = client.GetStream();
                    int i;
                    //Loop to receive all the data sent by the client
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        Debug.WriteLine(String.Format("PingListener: Data Received: {0}", bytes.ToString()));
                        //If ping request recieved, send back the data (0 is the ping packet id)
                        if ((byte)bytes[0] == 0x00)
                        {
                            Debug.WriteLine("Query request recieved.");
                            //Get server stats to send back
                            ServerPingData pingData = Program.NetManager.GetQuery();
                            //Write the data to a steam and send
                            using (MemoryStream ms = new MemoryStream())
                            {
                                using (StreamWriter writer = new StreamWriter(ms))
                                {
                                    writer.Write((byte)pingData.Online);
                                    writer.Write((byte)pingData.MaxOnline);
                                    writer.Write(pingData.MOTD);
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
                Console.WriteLine("PingListener Error: {0}", e);
            }
        }
    }
}
