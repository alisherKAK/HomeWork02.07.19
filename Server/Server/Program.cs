using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Server
{
    class Program
    {
        static List<string> postCodes;

        static void Main(string[] args)
        {
            TcpListener serverSocket = new TcpListener(IPAddress.Parse("0.0.0.0"), 12345);
            serverSocket.Start(100);

            Thread serverThread = new Thread(ServerThreadRoutine);
            serverThread.Start(serverSocket);
        }

        private static void ServerThreadRoutine(object state)
        {
            TcpListener serverSocket = state as TcpListener;

            IAsyncResult asyncResult = serverSocket.BeginAcceptTcpClient(ClientThreadRoutine, serverSocket);

            while (true)
            {
                
            }
        }

        private static void ClientThreadRoutine(IAsyncResult result)
        {
            TcpListener server = result.AsyncState as TcpListener;
            TcpClient client = server.EndAcceptTcpClient(result);

            ThreadPool.QueueUserWorkItem(ClientThreadRoutine2, client);
        }

        private static void ClientThreadRoutine2(object state)
        {
            TcpClient client = state as TcpClient;
            byte[] buf = new byte[4 * 1024];
            int recSize;
            string allPostCodes;

            while (true)
            {
                recSize = client.Client.Receive(buf);
                FindPostCodesByAddress(Encoding.UTF8.GetString(buf, 0, recSize));
                allPostCodes = string.Empty;

                foreach (string postCode in postCodes)
                {
                    allPostCodes += postCode + "\n";
                }
                client.Client.Send(Encoding.UTF8.GetBytes(allPostCodes));
            }
        }

        private static void FindPostCodesByAddress(string address)
        {
            using(var client = new WebClient())
            {
                //client.DownloadStringCompleted += ClientDownloadStringCompleted;
                string result = client.DownloadString(new Uri($"https://api.post.kz/api/byAddress/{address}?from=0"));
                postCodes = JsonConvert.DeserializeObject<Address>(result).Data.Select(post => post.Postcode).ToList();
            }
        }

        private static void ClientDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            postCodes = JsonConvert.DeserializeObject<Address>(e.Result).Data.Select(address => address.Postcode).ToList();
        }
    }
}
