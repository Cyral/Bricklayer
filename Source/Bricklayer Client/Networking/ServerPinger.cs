using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using Bricklayer.Common.Data;

namespace Bricklayer.Client.Networking
{
    /// <summary>
    /// Queries a TCP server for data regarding players online, message of the day, etc
    /// </summary>
    public class ServerPinger
    {
        private TcpClient client;
        private byte[] data;
        NetworkStream stream;

        public ServerPingData PingServer(string host, int port, out string error)
        {
            ServerPingData pingData = new ServerPingData();
            error = string.Empty;
            //Attempt to contact the server
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    IAsyncResult ar = client.BeginConnect(host, port, null, null);
                    System.Threading.WaitHandle wh = ar.AsyncWaitHandle;
                    try
                    {
                        if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(Common.GlobalSettings.ConnectTimeout), false))
                        {
                            client.Close();
                            error = "Offline (Connection Timeout)";
                            pingData.Error = true;
                            return pingData;
                        }

                        client.EndConnect(ar);
                    }
                    finally
                    {
                        wh.Close();
                    }
                    //Send a single message notifying the server we would like it's stats (0 is the ping request ID)
                    data = new byte[1] { 0x00 };

                    //Get a client stream for reading and writing
                    stream = client.GetStream();

                    //Send the message to the connected TcpServer.
                    stream.Write(data, 0, data.Length);

                    Debug.WriteLine("ServerPinger Sent: " + data.ToString());

                    //Buffer to store the response bytes.
                    data = new byte[256];

                    //Read the first batch of the TcpServer response bytes
                    int bytes = stream.Read(data, 0, data.Length);

                    using (MemoryStream ms = new MemoryStream(data))
                    {
                        using (StreamReader reader = new StreamReader(ms))
                        {
                            pingData.Online = (int)byte.Parse(reader.ReadLine());
                            pingData.MaxOnline = (int)byte.Parse(reader.ReadLine());
                            pingData.Description = reader.ReadLine();
                        }
                    }
                    client.Close();
                } 
            }
            catch (Exception e)
            {
                error = e.Message;
                pingData.Error = true;
                Debug.WriteLine(e.ToString());
                if (e is SocketException)
                {
                    //Provide some better error messages
                    int id = (e as SocketException).ErrorCode;
                    if (id == 10061) //No connection could be made because the target machine actively refused it
                        error = "Offline (Target Online, Server Not Accessible)";
                    else if (id == 10060) //A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond
                        error = "Offline (Connection Timeout)";
                }
            }
            finally
            {
                if (client != null)
                    client.Close();
            }
            return pingData;
        }
    }
}
