using System;
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
using System.Threading.Tasks;
using WindowsFormsApp3.Component;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {

        //Modbus通訊
        private SerialPort serialPort;
        private IModbusSerialMaster master;
        private Machine machine;
        private MachineSetting machineSetting;
        private bool isSimulate = false;
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

            isSimulate = true;//本機電腦執行 設True

            machine = new Machine();
            machine.Initial(isSimulate);
            machine.Home();

            /*serialPort = new SerialPort
            {
                PortName = "COM4", // Adjust the COM port as necessary
                BaudRate = 19200,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One
            };*/

            try
            {
                /*
                //485通訊開啟
                serialPort.Open();
                //建立MODBUS主站通訊
                master = ModbusSerialMaster.CreateRtu(serialPort);

                //485通訊開啟判斷，開始設備狀態掃描
                if (serialPort.IsOpen != null)
                    timerCheckAxisStatus.Enabled = true;

                machineSetting = new MachineSetting();
                machine = new Machine();
                //  machine.Initial();
                // machine.Home();

                */
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


        }

        //TOYO位置設定
        private void WritePositionToTOYO()
        {



        }


        //絕對位置
        private void btnABScoordinate_Click(object sender, EventArgs e) =>

            // 命令和地址
            master.WriteSingleRegister(1, 0x201E, 0x0001);


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

        private void button3_Click(object sender, EventArgs e)
        {



        }

        //定位運轉
        private async void btnStart_Click(object sender, EventArgs e)
        {
            /*
            //位置轉換,單位step
            if (ushort.TryParse(txtSetPostion.Text, out ushort setPostion))
            {
                Console.WriteLine(setPostion);  
            }
            else
            {
                Console.WriteLine("轉換失敗。");
            }

            //速度轉換，單位Hz
            if (ushort.TryParse(txtSetVerlocity.Text, out ushort setVerlocity))
            {
                Console.WriteLine(setVerlocity);
            }
            else
            {
                Console.WriteLine("轉換失敗。");
            }
            */


            // 運轉方式
            master.WriteSingleRegister(1, 0x1800, 0x0000);
            master.WriteSingleRegister(1, 0x1801, 0x0001);

            // 運轉位置
            master.WriteSingleRegister(1, 0x005C, 0x0013);
            master.WriteSingleRegister(1, 0x005D, 0x0088);


            // 運轉速度
            master.WriteSingleRegister(1, 0x1804, 0x0000);
            master.WriteSingleRegister(1, 0x1805, 0x1388);

            //啟動on
            master.WriteSingleRegister(1, 0x007D, 0x0008);
            //啟動off
            master.WriteSingleRegister(1, 0x007D, 0x0000);

            //read現在位置
            ushort[] nowPostion = master.ReadHoldingRegisters(1, 0x00C6, 2);
            ushort positionValue = nowPostion[1];
            txtNowPostion.Text = positionValue.ToString();

            //read現在速度
            ushort[] nowVerlocity = master.ReadHoldingRegisters(1, 0x00C8, 2);
            ushort verlocityValue = nowVerlocity[1];
            txtNowVerlocity.Text = verlocityValue.ToString();


        }



        private void txtOriPostion_TextChanged(object sender, EventArgs e)
        {

        }

        private void medecineRotaAxisHome_Click(object sender, EventArgs e)
        {

        }

        private void lblOriPostion_Click(object sender, EventArgs e)
        {

        }

        private void Load_btn_Click(object sender, EventArgs e)
        {


        }

        private void button11_Click(object sender, EventArgs e)
        {
            machineSetting.TransferLoadPos = Convert.ToDouble(LoadPos_txb.Text);
            machineSetting.BoxCassetteElevatorStartPos = Convert.ToDouble(textBox10.Text);
            machine.MachineSet = machineSetting;
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void LoadFil_btn_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void DumpSpecimen_btn_Click(object sender, EventArgs e)
        {

        }

        private void LoadJar_btn_Click(object sender, EventArgs e)
        {

        }

        private void ConfirmExistence_btn_Click(object sender, EventArgs e)
        {

        }

        private void button14_Click(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void btnTCPConnect_Click_1(object sender, EventArgs e)
        {

        }
        
        private void button24_Click(object sender, EventArgs e)
        {

        }

        private void button23_Click(object sender, EventArgs e)
        {

        }

        private void button22_Click(object sender, EventArgs e)
        {

        }

        private void btnResetAlarm_Click(object sender, EventArgs e)
        {
            // 命令和地址       
            master.WriteSingleRegister(1, 0x007D, 0x0088);
        }

        private void btnORG_Click_1(object sender, EventArgs e)
        {

        }

        private void btnABScoordinate_Click_1(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
        ADTech_USB4750 i4750 = new ADTech_USB4750(1);
        private void MedicineFork_btn_Click(object sender, EventArgs e)
        {
            
            try
            {
                

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void button11_Click_1(object sender, EventArgs e)
        {



        }

        private void SaveParam_btn_Click(object sender, EventArgs e)
        {
            machineSetting.TransferLoadPos =Convert.ToDouble(LoadPos_txb.Text);
            machineSetting.TransferDumpPos = Convert.ToDouble(textBox10.Text);
            machineSetting.Save("D:\\CG.json");
        }

        private void LoadParam_btn_Click(object sender, EventArgs e)
        {
            machineSetting = AbstractRecipe.Load<MachineSetting>("D:\\CG.json");
            LoadPos_txb.Text = machineSetting.TransferLoadPos.ToString();
            textBox10.Text = machineSetting.TransferDumpPos.ToString()  ;

        }

        private void btnJogAddO_Click_1(object sender, EventArgs e)
        {

        }

        private void Initial_btn_Click(object sender, EventArgs e)
        {
            
        }

        private void Form_Click(object sender, EventArgs e)
        {

        }
        

        private void btnLoadCY_Click(object sender, EventArgs e)
        {
    
        }

        private void ReadBarcode_btn_Click(object sender, EventArgs e)
        {
         //   machine.LoadModle.ReadBarcode();

        }

        private void OpenMediAndFilCamChk_btn_Click(object sender, EventArgs e)
        {
            //machine.LoadModle.LoadAsync(0);
            machine.DumpModle.LoadAsync();
        }
    }



}
