using System;
using System.Collections.Generic;
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
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TcpClient clientSocket;
        bool isConnected = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectButtonClick(object sender, RoutedEventArgs e)
        {
            clientSocket = new TcpClient();
            isConnected = true;

            clientSocket.BeginConnect(IPAddress.Parse(ipTextBox.Text), int.Parse(portTextBox.Text), ListenServer, clientSocket);
        }

        private void ListenServer(IAsyncResult ar)
        {
            ThreadPool.QueueUserWorkItem(Listen, ar.AsyncState);
        }

        private void Listen(object state)
        {
            TcpClient client = state as TcpClient;
            byte[] buf = new byte[4 * 1024];
            int recSize;

            while (true)
            {
                recSize = client.Client.Receive(buf);
                Dispatcher.Invoke(() => postCodesTextBlock.Text = Encoding.UTF8.GetString(buf, 0, recSize));
            }
        }

        private void PortTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if( (e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) )
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void IpTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.OemPeriod || e.Key == Key.Decimal)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            if(isConnected)
            {
                clientSocket.Client.Send(Encoding.UTF8.GetBytes(addressTextBox.Text));
            }
        }
    }
}
