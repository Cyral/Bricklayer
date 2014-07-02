using System;
using System.Diagnostics;
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
            // Add timer to handle timeouts
            // @FER22F - DEPRECATED: Timer timeoutTimer = new Timer(3000);
            // Attemp to contact the server
            try
            {
                client = new TcpClient(host, port);
                //Send a single message notifying the server we would like it's stats (0 is the ping request ID)
                data = new byte[1] { 0x00 };

                //Get a client stream for reading and writing
                stream = client.GetStream();

                //Send the message to the connected TcpServer.
                stream.Write(data, 0, data.Length);

                Debug.WriteLine("ServerPinger Sent: " + data.ToString());

                // Gets the size of the message from the server. 65535 is the max
                int size = (stream.ReadByte() << 8) + stream.ReadByte();

                //Buffer to store the response bytes.
                data = new byte[size];

                //Read the first batch of the TcpServer response bytes
                int bytes = stream.Read(data, 0, data.Length);

                //TODO: This way of reading the recieved info is prone to errors
                //Someone needs to find a way to use methods like ReadString(), ReadByte(), etc for this section
                //This is a total hack and needs to be changed!
                pingData.Online = int.Parse(Encoding.ASCII.GetString(data).Substring(0,1));
                pingData.MaxOnline = int.Parse(Encoding.ASCII.GetString(data).Substring(1, 1));
                pingData.Description = Encoding.ASCII.GetString(data).Substring(2).Replace("\0","");
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
                        error = "Target is online, however is not accessible.\n(Not running a server on that port, or blocked through a firewall)";
                    else if (id == 10060) //A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond
                        error = "Connection timed out, could not connect.";
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
