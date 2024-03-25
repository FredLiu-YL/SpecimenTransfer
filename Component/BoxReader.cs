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
    class BoxReader : IBarcodeReader
    {

        
        //property
        private TcpClient tcpClient;
        private Thread tcpThread;
        private NetworkStream networkStream;
        private int deviceNumber;
        private string ip;
        private int port;
        string barcodeResult;

        public BoxReader(string ip, int port)
        {
            this.ip = ip;
            this.port = port;

            tcpClient = new TcpClient();
            tcpClient.Connect(IPAddress.Parse(ip), port); // 連接server

            networkStream = tcpClient.GetStream();
            // 線程，監聽
            tcpThread = new Thread(new ThreadStart(() => ReceiveData()));
            tcpThread.IsBackground = true; // 後台線程
            tcpThread.Start();

        }
        
        public string ReceiveData()
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while ((bytesRead = networkStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    
                    // 線程上更新內容
                    //Invoker((MethodInvoker)delegate
                    {
                        barcodeResult = dataReceived;
                    };
                    
                    return dataReceived;
                    
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error receiving data: {ex.Message}");
            }
            return barcodeResult;
        }

    


        string IBarcodeReader.ReceiveData()
        {
            throw new NotImplementedException();
        }
    }

}

