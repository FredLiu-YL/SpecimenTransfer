using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3.Component
{
    public class KyenceBarcode : IBarcodeReader
    {
        //KEYENCE Barcode Reader 開始
        //TCPIP宣告
        private TcpClient tcpClient;
        private Thread tcpThread;
        private NetworkStream networkStream;
        private int deviceNumber;
        public KyenceBarcode()
        {
            // IP地址和端口號
            string ip = "192.168.100.80";
            int port = 9004;

            tcpClient = new TcpClient();
            tcpClient.Connect(IPAddress.Parse(ip), port); // 連接server

            networkStream = tcpClient.GetStream();

            // 線程，監聽
            tcpThread = new Thread(new ThreadStart(ReceiveData));
            tcpThread.IsBackground = true; // 後台線程
            tcpThread.Start();

        }

        private void ReceiveData()
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = networkStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    // 線程上更新內容
                    Invoke((MethodInvoker)delegate
                    {
                        txtReadBarcode.Text = dataReceived;
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error receiving data: {ex.Message}");
            }

        }


    }
}
