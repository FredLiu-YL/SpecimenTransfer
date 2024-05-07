using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SpecimenTransfer.Model.Component
{
    public class BoxReader : IBarcodeReader
    {

        //property
        private TcpClient tcpClient;
        //private Thread tcpThread;
        private NetworkStream networkStream;
        private string ip;
        private int port;
        string barcodeResult;
      


        public BoxReader(string ip, int port)
        {
            try
            {
                this.ip = ip;
                this.port = port;

                tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Parse(ip), port); // 連接server
                networkStream = tcpClient.GetStream();


                // 線程，監聽
                //tcpThread = new Thread(new ThreadStart(() => ReceiveData()));
                //tcpThread.IsBackground = true; // 後台線程
                //tcpThread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to barcode reader: {ex.Message}");
            }

        }


        public string ReceiveData()
        {
            if(tcpClient.Connected)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;

                    networkStream.ReadTimeout = 2000;

                    bytesRead = networkStream.Read(buffer, 0, buffer.Length);

                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    barcodeResult = dataReceived;



                }
                catch (Exception ex)
                {
                    if (ex.HResult == -2146232800)
                    {
                        //time out
                        barcodeResult = "not found!";
                    }
                    else
                        throw new Exception($"Error receiving data: {ex.Message}");
                }
            }
                

            return barcodeResult;

        }

        /*
        string IBarcodeReader.ReceiveData()
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                networkStream.ReadTimeout = 2000;

                while ((bytesRead = networkStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);



                    barcodeResult = dataReceived;

                }

            }
            catch (Exception ex)
            {
                if (ex.HResult == -2146232800)
                {
                    //time out
                    barcodeResult = "not found!";
                }
                else
                    throw new Exception($"Error receiving data: {ex.Message}");
            }

            return barcodeResult;
        }
        */
    }



}




