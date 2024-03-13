﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using Modbus.Device;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Automation.BDaq;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {

        //Modbus通訊
        private SerialPort serialPort;
        private IModbusSerialMaster master;

        /*
        // USB-4750 DI DO
        private InstantDiCtrl instantDiCtrl = new InstantDiCtrl(); // 用於DI
        private InstantDoCtrl instantDoCtrl = new InstantDoCtrl(); // 用於DO
        */

        public Form1()
        {
            InitializeComponent();

            timerCheckAxisStatus.Interval = 100;
            timerCheckAxisStatus.Tick += timerCheckAxisStatus_Tick;
            
            this.Load += Form1_Load; // Subscribe to the Load event
                                     
            /*
            // USB-4750 初始化設備
            instantDiCtrl.SelectedDevice = new DeviceInformation(0); // 假設設備編號為0
            instantDoCtrl.SelectedDevice = new DeviceInformation(0); // 同上
            */

        }


        private void Form1_Load(object sender, EventArgs e)
        {
            serialPort = new SerialPort
            {
                PortName = "COM4", // Adjust the COM port as necessary
                BaudRate = 19200,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One
            };

            try
            {
                //485通訊開啟
                serialPort.Open();
                //建立MODBUS主站通訊
                master = ModbusSerialMaster.CreateRtu(serialPort);
            
               //485通訊開啟判斷，開始設備狀態掃描
               if (serialPort.IsOpen != null)
               timerCheckAxisStatus.Enabled = true;

            }
            catch (Exception ex)
            {
               
            }

           
        }

        private void Form1_LoadClosing(object sender, FormClosingEventArgs e)
        {
            serialPort?.Close();
            timerCheckAxisStatus.Stop();
        }

        

        private void btnConnect_Click(object sender, EventArgs e) 
        {



        }


        //TOYO Modbus
        //TOYO原點復歸
        private void btnORG_Click(object sender, EventArgs e)
        {

           master.WriteSingleRegister(1, 0x007D, 0x0010);

        }
        

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        //TOYO速度設定
        private void WriteVelocityToTOYO()
        {
            byte slaveId = 1; // 站ID
            ushort velocityAddress = 0x2014; // 速度的起始地址


            if (ushort.TryParse(txtVelocity.Text, out ushort velocity))// 速度轉換
            {
                    master.WriteSingleRegister(slaveId, velocityAddress, velocity);
            }

        }

        //TOYO位置設定
        private void WritePositionToTOYO()
        {
            try
            {
                byte slaveId = 1; // 從站ID
                ushort positionAddress = 0x2002; // 座標位置的起始地址


                if (double.TryParse(txtABSPostion.Text, out double position))// 座標轉換
                {
                    // 将输入值转换为Modbus寄存器可以接受的格式
                    ushort positionHigh = (ushort)((int)(position * 100) >> 16); // 高位
                    ushort positionLow = (ushort)((int)(position * 100) & 0xFFFF); // 低位
                    master.WriteMultipleRegisters(slaveId, positionAddress, new ushort[] { positionHigh, positionLow });
                }

               
                MessageBox.Show("寫入成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"寫入失败：{ex.Message}");
            }
        }


        //絕對位置
        private void btnABScoordinate_Click(object sender, EventArgs e)
        {

           // 命令和地址
           master.WriteSingleRegister(1, 0x201E, 0x0001);

        }


        private void btnJogAdd_MouseUp(object sender, MouseEventArgs e)
        {
            // 命令和地址
            master.WriteSingleRegister(1, 0x201E, 0x0009);
         
        }

        private void btnJogAdd_MouseDown(object sender, MouseEventArgs e)
        {

            // 命令和地址
            master.WriteSingleRegister(1, 0x201E, 0x000B);

        }

        private void btnJogResuce_MouseDown(object sender, MouseEventArgs e)
        {

            // 命令和地址
            master.WriteSingleRegister(1, 0x201E, 0x000C);

        }

        private void btnJogResuce_MouseUp(object sender, MouseEventArgs e)
        {

            // 命令和地址
            master.WriteSingleRegister(1, 0x201E, 0x0009);

        }

        private void txtReadBarcode_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void btnJogAdd_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {

        }


        //Origentalmotor 軸控開始
        //Origentalmotor 開始連線

        private void btnORGO_Click(object sender, EventArgs e)//Origentalmotor 原點複歸
        {
            // 命令和地址       
            master.WriteSingleRegister(1, 0x007D, 0x0010);

        }

        private void btnABScoordinateO_Click(object sender, EventArgs e)//TOYO MOTOR 絕對位置
        {

                // 命令和地址
                master.WriteSingleRegister(1, 0x007D, 0x0008);

        }

        private void btnJogAddO_MouseDown(object sender, MouseEventArgs e)//Origentalmotor JOG+
        {
            // 命令和地址
            master.WriteSingleRegister(1, 0x007D, 0x4000);

        }

        private void btnJogAddO_MouseUp(object sender, MouseEventArgs e)//Origentalmotor JOG+
        {

            // 命令和地址
            master.WriteSingleRegister(1, 0x007D, 0x0020);
        }

        private void btnJogResuceO_MouseDown(object sender, MouseEventArgs e)//Origentalmotor JOG-
        {
            // 命令和地址
            master.WriteSingleRegister(1, 0x007D, 0x8000);
        }
        private void btnJogResuceO_MouseUp(object sender, MouseEventArgs e)//Origentalmotor JOG-
        {

            // 命令和地址
            master.WriteSingleRegister(1, 0x007D, 0x0020);


        }

        private void btnStop_Click(object sender, EventArgs e)
        {

                // 命令和地址
                byte slaveId = 1; // 從站地址
                ushort address = 0x007D; // 寄存器地址
                ushort value = 0x0000; // 要寫入的值
                master.WriteSingleRegister(1, 0x007D, 0x0000);
               
        
        }

        private void button21_Click(object sender, EventArgs e)
        {
          
           // 命令和地址
           master.WriteSingleRegister(1, 0x007D, 0x0009);


        }

        private void btnMedcRotaAxisP0_Click(object sender, EventArgs e)
        {
            // 命令和地址
            master.WriteSingleRegister(2, 0x007D, 0x0008);
        }

        private void btnJogAddO_Click(object sender, EventArgs e)
        {

        }

        private void btnMedcTipHome_Click(object sender, EventArgs e)
        {

            // 命令和地址
            master.WriteSingleRegister(2, 0x007D, 0x0010);

        }



        private void btnMedcRotaAxisP1_Click_1(object sender, EventArgs e)
        {
            // 命令和地址
            master.WriteSingleRegister(2, 0x007D, 0x0009);
        }

        private void btnMedTipJogAdd_MouseDown(object sender, MouseEventArgs e)
        {

            // 命令和地址
            master.WriteSingleRegister(2, 0x007D, 0x4000);
        }

        private void btnMedTipJogAdd_MouseUp(object sender, MouseEventArgs e)
        {

            // 命令和地址
            master.WriteSingleRegister(2, 0x007D, 0x0020);
        }

        private void btnMedTipJogReduce_MouseDown(object sender, MouseEventArgs e)
        {
            // 命令和地址
            master.WriteSingleRegister(2, 0x007D, 0x8000);
        }

        private void btnMedTipJogReduce_MouseUp(object sender, MouseEventArgs e)
        {

            // 命令和地址
            master.WriteSingleRegister(2, 0x007D, 0x0020);
        }

        private void btnMedcTipStop_Click(object sender, EventArgs e)
        {
            // 命令和地址
            master.WriteSingleRegister(2, 0x007D, 0x0000);
        }

        //Timer check flow

        private void timerCheckAxisStatus_Load(object sender, EventArgs e)
        {

           
        }

        //掃描馬達狀態
        private void timerCheckAxisStatus_Tick(object sender, EventArgs e)
        {
            /*
            //Origentalmotor_藥罐旋轉
            //Modbus Read MOVE
            try
            {
                    // 命令和地址
                    ushort[] rotaRegisters = master.ReadHoldingRegisters(1, 0x0179, 0x0001); 
                    bool rotaMotorMove= (rotaRegisters[0] & (1 << 6)) != 0; // 檢查bit6

                // 根據馬達是否運行來更新UI
                medecineRotaAxisMove.BackColor = rotaMotorMove ? Color.Red : Color.Lime;
                
                }
                catch (Exception ex)
                {
                    // 處理錯誤，例如記錄錯誤或顯示錯誤訊息
                    MessageBox.Show($"Error reading Modbus: {ex.Message}");
                }

            //Origentalmotor_藥罐旋轉
            //Modbus Read HOME
            try
            {
 
                ushort[] rotaRegisters1 = master.ReadHoldingRegisters(1, 0x0178, 0x0001);
                bool rotaMotorHome = (rotaRegisters1[0] & (1 << 0)) != 0; // // 檢查bit0
                // 根據馬達是否運行來更新UI
                medecineRotaAxisHome.BackColor = rotaMotorHome ? Color.Red : Color.Lime;
            }
            catch (Exception ex)
            {
                // 處理錯誤，例如記錄錯誤或顯示錯誤訊息
                MessageBox.Show($"Error reading Modbus: {ex.Message}");
            }

            //Origentalmotor_藥罐旋轉
            //Modbus Read INP
            try
            {

                ushort[] rotaRegisters2 = master.ReadHoldingRegisters(1, 0x007F, 0x0001);
                bool rotaMotorINP = (rotaRegisters2[0] & (1 << 14)) != 0; // 檢查bit14
                // 根據馬達是否運行來更新UI
                medecineRotaAxisINP.BackColor = rotaMotorINP ? Color.Red : Color.Lime;
            }
            catch (Exception ex)
            {
                // 處理錯誤，例如記錄錯誤或顯示錯誤訊息
                MessageBox.Show($"Error reading Modbus: {ex.Message}");
            }

            
            
            //Origentalmotor_藥罐傾倒
            //Modbus Read MOVE
            try
            {
                // 命令和地址
                ushort[] tipRegisters = master.ReadHoldingRegisters(2, 0x0179, 0x0001);
                bool tipMotorMove = (tipRegisters[0] & (1 << 6)) != 0; // 檢查bit6

                // 根據馬達是否運行來更新UI
                medecineTipAxisMove.BackColor = tipMotorMove ? Color.Red : Color.Lime;

            }
            catch (Exception ex)
            {
                // 處理錯誤，例如記錄錯誤或顯示錯誤訊息
                MessageBox.Show($"Error reading Modbus: {ex.Message}");
            }
            

            //Origentalmotor_藥罐傾倒
            //Modbus Read HOME
            try
            {
                ushort[] tipRegisters1 = master.ReadHoldingRegisters(2, 0x0178, 0x0001);
                bool tipMotorHome = (tipRegisters1[0] & (1 << 0)) != 0; // // 檢查bit0
                // 根據馬達是否運行來更新UI
                medecineTipAxisHome.BackColor = tipMotorHome ? Color.Red : Color.Lime;
            }
            catch (Exception ex)
            {
                // 處理錯誤，例如記錄錯誤或顯示錯誤訊息
                MessageBox.Show($"Error reading Modbus: {ex.Message}");
            }


            //Origentalmotor_藥罐傾倒
            //Modbus Read INP
            try
            {

                ushort[] tipRegisters2 =  master.ReadHoldingRegisters(2, 0x007F, 0x0001);
                bool tipMotorINP = (tipRegisters2[0] & (1 << 14)) != 0; // 檢查bit14
                // 根據馬達是否運行來更新UI
                medecineTipAxisINP.BackColor = tipMotorINP ? Color.Red : Color.Lime;
            }
            catch (Exception ex)
            {
                // 處理錯誤，例如記錄錯誤或顯示錯誤訊息
                MessageBox.Show($"Error reading Modbus: {ex.Message}");
            }
            
            */
        }


        private void isHomePositionOn_Click(object sender, EventArgs e)
        {

        }

        public void origentalmotorSignalStatus()
        {
      
        }

        private void changeOrigenPage_Click(object sender, EventArgs e)
        {


                      

        }

        private void changeTOYOPage_Click(object sender, EventArgs e)
        {
            
        }



        private void btnMedTipJogAdd_Click(object sender, EventArgs e)
        {

        }

        private void btnPageScan_Click(object sender, EventArgs e)
        {

        }

        private void btnPageCarrierBox_Click(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnPageFilterBox_Click(object sender, EventArgs e)
        {

        }



        private void panelFrom1Top_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void panelFrom1Top_MouseMove(object sender, MouseEventArgs e)
        {
          
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
          
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
           
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
           
        }

        private void panelFrom1Top_MouseUp(object sender, MouseEventArgs e)
        {
              
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnReadDI_Click(object sender, EventArgs e)
        {

        }

        private void btnReadDO_Click(object sender, EventArgs e)
        {

           

        }



        //KEYENCE Barcode Reader 開始
        //TCPIP宣告
        private TcpClient tcpClient;
        private Thread tcpThread;
        private NetworkStream networkStream;
        private int deviceNumber;

        private void btnTCPConnect_Click(object sender, EventArgs e)
        {

            try
            {
                // IP地址和端口號的设置
                string ip = "192.168.100.80";
                int port = 9004;

                tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Parse(ip), port); // 連接server

                networkStream = tcpClient.GetStream();

                // 線程，監聽
                tcpThread = new Thread(new ThreadStart(ReceiveData));
                tcpThread.IsBackground = true; // 後台線程
                tcpThread.Start();

                MessageBox.Show("Connected to barcode reader!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to barcode reader: {ex.Message}");
            }
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
        //KEYENCE Barcode Reader結束

        //FormClosing
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            tcpThread?.Abort();
            networkStream?.Close();
            tcpClient?.Close();
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }

        }
    }


   
}
