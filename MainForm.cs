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
using SpecimenTransfer.Model;
using SpecimenTransfer.Model.Component;

namespace WindowsFormsApp3
{
    public partial class MainForm : Form
    {

        //Modbus通訊


        private Machine machine;
        private MachineSetting machineSetting;
        private bool isSimulate = false;
        /*
        // USB-4750 DI DO
        private InstantDiCtrl instantDiCtrl = new InstantDiCtrl(); // 用於DI
        private InstantDoCtrl instantDoCtrl = new InstantDoCtrl(); // 用於DO
        */

        public MainForm()
        {
            InitializeComponent();

            timerCheckAxisStatus.Interval = 100;
            timerCheckAxisStatus.Tick += timerCheckAxisStatus_Tick;

            /*
            // USB-4750 初始化設備
            instantDiCtrl.SelectedDevice = new DeviceInformation(0); // 假設設備編號為0
            instantDoCtrl.SelectedDevice = new DeviceInformation(0); // 同上
            */

        }



        private async void Form1_Load(object sender, EventArgs e)
        {



            try
            {
                isSimulate = true;//本機電腦執行 設True

                machine = new Machine();
                machine.Initial(isSimulate);
                await machine.Home();

                machine.DumpModle.SetupJar = SetupJar;


                machineSetting = AbstractRecipe.Load<MachineSetting>("D:\\CG.json");
                ParamToUI(machineSetting);
            }
            catch (Exception ex)
            {

            }


        }

        //private void UIAnchor()
        //{
        //    View1_GB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
        //    | System.Windows.Forms.AnchorStyles.Left)
        //    | System.Windows.Forms.AnchorStyles.Right)));

        //    Log_GB.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom |
        //                                                         System.Windows.Forms.AnchorStyles.Left|
        //                                                         System.Windows.Forms.AnchorStyles.Right));

        //    Control_PN.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top |
        //                                                             System.Windows.Forms.AnchorStyles.Right));

        //    Work_GB.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top |
        //                                                          System.Windows.Forms.AnchorStyles.Right));

        //    View2_PN.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top |
        //                                                           System.Windows.Forms.AnchorStyles.Bottom |
        //                                                           System.Windows.Forms.AnchorStyles.Right));



        //    IDEL_PN.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top |
        //                                                           System.Windows.Forms.AnchorStyles.Right));
        //}

        private void ParamToUI(MachineSetting setting)
        {
            slideTable_Load_TB.Text = setting.LoadModuleParam.CarrierTableBoxCassettePos.ToString();
            slideTable_Paper_TB.Text = setting.LoadModuleParam.CarrierTableFilterPaperPos.ToString();

            slideTable_Dump_TB.Text = setting.DumpModuleParam.CarrierTableBottleDumpPos.ToString();
            slideTable_Ink_TB.Text = setting.DumpModuleParam.CarrierTableRedInkPos.ToString(); ;


        }
        private MachineSetting UIToParam()
        {
            MachineSetting setting = new MachineSetting();
            setting.LoadModuleParam.CarrierTableBoxCassettePos = Convert.ToDouble(slideTable_Load_TB.Text);
            setting.LoadModuleParam.CarrierTableFilterPaperPos = Convert.ToDouble(slideTable_Paper_TB.Text);
            setting.LoadModuleParam.CarrierTableDumpPos = Convert.ToDouble(slideTable_Dump_TB.Text);
            setting.DumpModuleParam.CarrierTableBottleDumpPos = Convert.ToDouble(slideTable_Dump_TB.Text);
            setting.DumpModuleParam.CarrierTableRedInkPos = Convert.ToDouble(slideTable_Ink_TB.Text);


            return setting;

        }
        private void Form1_LoadClosing(object sender, FormClosingEventArgs e)
        {
            /* serialPort?.Close();
             timerCheckAxisStatus.Stop();
            */
        }



        //TOYO Modbus
        //TOYO原點復歸
        private void btnORG_Click(object sender, EventArgs e)
        {

            //   master.WriteSingleRegister(1, 0x007D, 0x0010);

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
        private void btnABScoordinate_Click(object sender, EventArgs e)
        {

            // 命令和地址
            //    master.WriteSingleRegister(1, 0x201E, 0x0001);

        }
        private void btnJogAdd_MouseUp(object sender, MouseEventArgs e)
        {
            // 命令和地址
            //    master.WriteSingleRegister(1, 0x201E, 0x0009);

        }

        private void btnJogAdd_MouseDown(object sender, MouseEventArgs e)
        {

            // 命令和地址
            //    master.WriteSingleRegister(1, 0x201E, 0x000B);

        }

        private void btnJogResuce_MouseDown(object sender, MouseEventArgs e)
        {

            // 命令和地址
            //    master.WriteSingleRegister(1, 0x201E, 0x000C);

        }

        private void btnJogResuce_MouseUp(object sender, MouseEventArgs e)
        {

            // 命令和地址
            //    master.WriteSingleRegister(1, 0x201E, 0x0009);

        }




        //Origentalmotor 軸控開始
        //Origentalmotor 開始連線
        private void btnORGO_Click(object sender, EventArgs e)//Origentalmotor 原點複歸
        {
            // 命令和地址       
            //    master.WriteSingleRegister(1, 0x007D, 0x0010);

        }

        private void btnABScoordinateO_Click(object sender, EventArgs e)//TOYO MOTOR 絕對位置
        {

            // 命令和地址
            //    master.WriteSingleRegister(1, 0x007D, 0x0008);

        }

        private void btnJogAddO_MouseDown(object sender, MouseEventArgs e)//Origentalmotor JOG+
        {
            // 命令和地址
            //    master.WriteSingleRegister(1, 0x007D, 0x4000);

        }

        private void btnJogAddO_MouseUp(object sender, MouseEventArgs e)//Origentalmotor JOG+
        {

            // 命令和地址
            //    master.WriteSingleRegister(1, 0x007D, 0x0020);
        }

        private void btnJogResuceO_MouseDown(object sender, MouseEventArgs e)//Origentalmotor JOG-
        {
            // 命令和地址
            //   master.WriteSingleRegister(1, 0x007D, 0x8000);
        }
        private void btnJogResuceO_MouseUp(object sender, MouseEventArgs e)//Origentalmotor JOG-
        {

            // 命令和地址
            //    master.WriteSingleRegister(1, 0x007D, 0x0020);


        }

        private void btnStop_Click(object sender, EventArgs e)
        {


            //     master.WriteSingleRegister(1, 0x007D, 0x0000);


        }

        private void button21_Click(object sender, EventArgs e)
        {

            // 命令和地址
            //    master.WriteSingleRegister(1, 0x007D, 0x0009);


        }

        private void btnMedcRotaAxisP0_Click(object sender, EventArgs e)
        {
            // 命令和地址
            //   master.WriteSingleRegister(2, 0x007D, 0x0008);
        }



        private void btnMedcTipHome_Click(object sender, EventArgs e)
        {

            // 命令和地址
            //    master.WriteSingleRegister(2, 0x007D, 0x0010);

        }



        private void btnMedcRotaAxisP1_Click_1(object sender, EventArgs e)
        {
            // 命令和地址
            //    master.WriteSingleRegister(2, 0x007D, 0x0009);
        }

        private void btnMedTipJogAdd_MouseDown(object sender, MouseEventArgs e)
        {

            // 命令和地址
            //    master.WriteSingleRegister(2, 0x007D, 0x4000);
        }

        private void btnMedTipJogAdd_MouseUp(object sender, MouseEventArgs e)
        {

            // 命令和地址
            //   master.WriteSingleRegister(2, 0x007D, 0x0020);
        }

        private void btnMedTipJogReduce_MouseDown(object sender, MouseEventArgs e)
        {
            // 命令和地址
            //    master.WriteSingleRegister(2, 0x007D, 0x8000);
        }

        private void btnMedTipJogReduce_MouseUp(object sender, MouseEventArgs e)
        {

            // 命令和地址
            //    master.WriteSingleRegister(2, 0x007D, 0x0020);
        }

        private void btnMedcTipStop_Click(object sender, EventArgs e)
        {
            // 命令和地址
            //   master.WriteSingleRegister(2, 0x007D, 0x0000);
        }

        //Timer check flow



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
            /* tcpThread?.Abort();
             networkStream?.Close();
             tcpClient?.Close();
             if (serialPort != null && serialPort.IsOpen)
             {
                 serialPort.Close();
             }*/

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

            /*
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
            */

        }





        private void btnResetAlarm_Click(object sender, EventArgs e)
        {
            // 命令和地址       
            //    master.WriteSingleRegister(1, 0x007D, 0x0088);
        }



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


        private void SaveParam_btn_Click(object sender, EventArgs e)
        {

            machineSetting = UIToParam();

            machineSetting.Save("D:\\CG.json");
        }



        private void LoadParam_btn_Click(object sender, EventArgs e)
        {
            //  machineSetting = AbstractRecipe.Load<MachineSetting>("D:\\CG.json");


        }


        private void ReadBarcode_btn_Click(object sender, EventArgs e)
        {
            //   machine.LoadModle.ReadBarcode();

        }

        private async void OpenMediAndFilCamChk_btn_Click(object sender, EventArgs e)
        {
            //machine.LoadModle.LoadAsync(0);
            //await  machine.DumpModle.LoadAsync();
        }



        private void SetupJar()
        {
            MessageBox.Show("請安裝藥罐 ，裝完後按下確定使流程繼續");
        }

        private async void Home_btn_Click(object sender, EventArgs e)
        {
            await machine.Home();
        }

        #region slideTable 事件

        private void slideTable_Jog_BTN_MouseUp(object sender, MouseEventArgs e)
        {

            var dis = Convert.ToDouble(slideTable_JogDiatance_TB.Text);
            switch (((Button)sender).Name)
            {
                case "slideTable_JogPlus_BTN":
                    machine.LoadModle.SlideTableAxis.MoveAsync(dis);
                    break;
                case "slideTable_JogMinus_BTN":
                    machine.LoadModle.SlideTableAxis.MoveAsync(-dis);
                    break;
                default:
                    return;

            }

        }

        private void slideTable_Set_BTN_Click(object sender, EventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "slideTable_SetLoad_BTN":
                    slideTable_Load_TB.Text = machine.LoadModle.SlideTableAxis.Position.ToString();
                    break;
                case "slideTable_SetPaper_BTN":
                    slideTable_Paper_TB.Text = machine.LoadModle.SlideTableAxis.Position.ToString();
                    break;
                case "slideTable_SetDump_BTN":
                    slideTable_Dump_TB.Text = machine.LoadModle.SlideTableAxis.Position.ToString();
                    break;
                case "slideTable_SetInk_BTN":
                    slideTable_Ink_TB.Text = machine.LoadModle.SlideTableAxis.Position.ToString();
                    break;
                case "slideTable_SetGland_BTN":
                    slideTable_Gland_TB.Text = machine.LoadModle.SlideTableAxis.Position.ToString();
                    break;
                case "slideTable_SetCover_BTN":
                    slideTable_Cover_TB.Text = machine.LoadModle.SlideTableAxis.Position.ToString();
                    break;
                case "slideTable_SetOutput_BTN":
                    slideTable_Output_TB.Text = machine.LoadModle.SlideTableAxis.Position.ToString();
                    break;

                default:
                    return;

            }

        }

        private void slideTable_Go_BTN_Click(object sender, EventArgs e)
        {
            double pos;

            switch (((Button)sender).Name)
            {
                case "slideTable_SetLoad_BTN":
                    pos = double.Parse(slideTable_Load_TB.Text);
                    break;
                case "slideTable_SetPaper_BTN":
                    pos = double.Parse(slideTable_Paper_TB.Text);
                    break;
                case "slideTable_SetDump_BTN":
                    pos = double.Parse(slideTable_Dump_TB.Text);
                    break;
                case "slideTable_SetInk_BTN":
                    pos = double.Parse(slideTable_Ink_TB.Text);
                    break;
                case "slideTable_SetGland_BTN":
                    pos = double.Parse(slideTable_Gland_TB.Text);
                    break;
                case "slideTable_SetCover_BTN":
                    pos = double.Parse(slideTable_Cover_TB.Text);
                    break;
                case "slideTable_SetOutput_BTN":
                    pos = double.Parse(slideTable_Output_TB.Text);
                    break;
                default:
                    return;


            }

            machine.LoadModle.SlideTableAxis.MoveToAsync(pos);
        }

        #endregion slideTable 事件

        #region filterPaperElevator 事件

        private void filterPaperElevator_Jog_BTN_MouseUp(object sender, MouseEventArgs e)
        {
            var dis = Convert.ToDouble(filterPaperElevator_JogDiatance_TB.Text);

            switch (((Button)sender).Name)
            {
                case "filterPaperElevator_JogPlus_BTN":
                    machine.LoadModle.FilterPaperElevatorAxis.MoveAsync(dis);
                    break;
                case "filterPaperElevator_JogMinus_BTN":
                    machine.LoadModle.FilterPaperElevatorAxis.MoveAsync(-dis);
                    break;
                default:
                    return;

            }
        }

        private void filterPaperElevator_Set_BTN_Click(object sender, EventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "filterPaperElevator_SetHigh_BTN":
                    filterPaperElevator_High_TB.Text = machine.LoadModle.FilterPaperElevatorAxis.Position.ToString();
                    break;
                case "filterPaperElevator_SetLow_BTN":
                    filterPaperElevator_Low_TB.Text = machine.LoadModle.FilterPaperElevatorAxis.Position.ToString();
                    break;
                case "filterPaperElevator_SetTarget_BTN":
                    filterPaperElevator_Target_TB.Text = machine.LoadModle.FilterPaperElevatorAxis.Position.ToString();
                    break;
                default:
                    return;

            }
        }

        private void filterPaperElevator_Go_BTN_Click(object sender, EventArgs e)
        {
            double pos;

            switch (((Button)sender).Name)
            {
                case "filterPaperElevator_GoStart_BTN":
                    double gap = double.Parse(filterPaperElevator_High_TB.Text) - double.Parse(filterPaperElevator_Low_TB.Text) / 10;
                    //最高點-所選階層(第一階等同於最高點以此類推)
                    pos = double.Parse(filterPaperElevator_High_TB.Text) - gap * filterPaperElevator_Start_CBB.SelectedIndex;
                    break;
                case "filterPaperElevator_GoTarget_BTN":
                    pos = double.Parse(filterPaperElevator_Target_TB.Text);
                    break;

                default:
                    return;


            }

            machine.LoadModle.FilterPaperElevatorAxis.MoveToAsync(pos);
        }

        #endregion filterPaperElevator 事件

        #region bottleElevator 事件

        private void bottleElevator_Jog_BTN_MouseUp(object sender, MouseEventArgs e)
        {
            var dis = Convert.ToDouble(bottleElevator_JogDiatance_TB.Text);

            switch (((Button)sender).Name)
            {
                case "bottleElevator_JogPlus_BTN":
                    machine.DumpModle.BottleElevatorAxis.MoveAsync(dis);
                    break;
                case "bottleElevator_JogMinus_BTN":
                    machine.DumpModle.BottleElevatorAxis.MoveAsync(-dis);
                    break;
                default:
                    return;

            }
        }

        private void bottleElevator_Set_BTN_Click(object sender, EventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "bottleElevator_SetScan_BTN":
                    bottleElevator_Scan_TB.Text = machine.DumpModle.BottleElevatorAxis.Position.ToString();
                    break;
                case "bottleElevator_SetScrewStart_BTN":
                    bottleElevator_ScrewStart_TB.Text = machine.DumpModle.BottleElevatorAxis.Position.ToString();
                    break;
                case "bottleElevator_SetScrewTarget_BTN":
                    bottleElevator_ScrewTarget_TB.Text = machine.DumpModle.BottleElevatorAxis.Position.ToString();
                    break;
                default:
                    return;

            }
        }

        private void bottleElevator_Go_BTN_Click(object sender, EventArgs e)
        {
            double pos;

            switch (((Button)sender).Name)
            {
                case "bottleElevator_GoScan_BTN":
                    pos = double.Parse(bottleElevator_Scan_TB.Text);
                    break;
                case "bottleElevator_GoScrewStart_BTN":
                    pos = double.Parse(bottleElevator_ScrewStart_TB.Text);
                    break;
                case "bottleElevator_GoScrewTarget_BTN":
                    pos = double.Parse(bottleElevator_ScrewTarget_TB.Text);
                    break;
                default:
                    return;


            }

            machine.DumpModle.BottleElevatorAxis.MoveToAsync(pos);
        }

        #endregion bottleElevator 事件

        #region bottleScrew 事件

        private void bottleScrew_Jog_BTN_MouseUp(object sender, MouseEventArgs e)
        {
            var dis = Convert.ToDouble(bottleScrew_JogDiatance_TB.Text);

            switch (((Button)sender).Name)
            {
                case "bottleScrew_JogPlus_BTN":
                    machine.DumpModle.BottleScrewAxis.MoveAsync(dis);
                    break;
                case "bottleScrew_JogMinus_BTN":
                    machine.DumpModle.BottleScrewAxis.MoveAsync(-dis);
                    break;
                default:
                    return;

            }
        }

        private void bottleScrew_Org_BTN_Click(object sender, EventArgs e)
        {

        }

        private void bottleScrew_Set_BTN_Click(object sender, EventArgs e)
        {
            bottleScrew_Target_TB.Text = machine.DumpModle.BottleScrewAxis.Position.ToString();
        }

        private void bottleScrew_Go_BTN_Click(object sender, EventArgs e)
        {
            double pos;
            pos = double.Parse(bottleScrew_Target_TB.Text);
            machine.DumpModle.BottleScrewAxis.MoveToAsync(pos);
        }

        #endregion bottleScrew 事件

        #region bottleDump 事件

        private void bottleDump_Jog_BTN_MouseUp(object sender, MouseEventArgs e)
        {
            var dis = Convert.ToDouble(bottleDump_JogDiatance_TB.Text);

            switch (((Button)sender).Name)
            {
                case "bottleDump_JogPlus_BTN":
                    machine.DumpModle.BottleDumpAxis.MoveAsync(dis);
                    break;
                case "bottleDump_JogMinus_BTN":
                    machine.DumpModle.BottleDumpAxis.MoveAsync(-dis);
                    break;
                default:
                    return;

            }
        }

        private void bottleDump_Set_BTN_Click(object sender, EventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "bottleDump_SetStart_BTN":
                    bottleDump_Start_TB.Text = machine.DumpModle.BottleDumpAxis.Position.ToString();
                    break;
                case "bottleDump_SetTarget_BTN":
                    bottleDump_Target_TB.Text = machine.DumpModle.BottleDumpAxis.Position.ToString();
                    break;
                default:
                    return;

            }
        }

        private void bottleDump_Go_BTN_Click(object sender, EventArgs e)
        {
            double pos;

            switch (((Button)sender).Name)
            {
                case "bottleDump_GoStart_BTN":
                    pos = double.Parse(bottleDump_Start_TB.Text);
                    break;
                case "bottleDump_GoTarget_BTN":
                    pos = double.Parse(bottleDump_Target_TB.Text);
                    break;
                default:
                    return;


            }

            machine.DumpModle.BottleDumpAxis.MoveToAsync(pos);
        }

        #endregion bottleDump 事件

        #region coverAndStorageElevator 事件

        private void coverAndStorageElevator_Jog_BTN_MouseUp(object sender, MouseEventArgs e)
        {
            var dis = Convert.ToDouble(coverAndStorageElevator_JogDiatance_TB.Text);

            switch (((Button)sender).Name)
            {
                case "coverAndStorageElevator_JogPlus_BTN":
                    machine.OutputModle.CoverAndStorageElevatorAxis.MoveAsync(dis);
                    break;
                case "coverAndStorageElevator_JogMinus_BTN":
                    machine.OutputModle.CoverAndStorageElevatorAxis.MoveAsync(-dis);
                    break;
                default:
                    return;

            }
        }

        private void Storage_Set_BTN_Click(object sender, EventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "Storage_SetStart_BTN":
                    Storage_Start_TB.Text = machine.OutputModle.CoverAndStorageElevatorAxis.Position.ToString();
                    break;
                case "Storage_SetSpacing_BTN":
                    //第二階減掉第一階得到間距
                    double gap = machine.OutputModle.CoverAndStorageElevatorAxis.Position - double.Parse(Storage_Start_TB.Text);
                    Storage_Spacing_TB.Text = gap.ToString();
                    break;
                default:
                    return;

            }
        }

        private void Cover_Set_BTN_Click(object sender, EventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "Cover_SetStart_BTN":
                    Cover_Start_TB.Text = machine.OutputModle.CoverAndStorageElevatorAxis.Position.ToString();
                    break;
                case "Cover_SetSpacing_BTN":
                    //第二階減掉第一階得到間距
                    double gap = machine.OutputModle.CoverAndStorageElevatorAxis.Position - double.Parse(Cover_Start_TB.Text);
                    Cover_Spacing_TB.Text = gap.ToString();
                    break;
                    default:
                    return;

            }
        }

        private void Storage_GoTarget_BTN_Click(object sender, EventArgs e)
        {
            double pos;

            //起始點+間距*階層(第一層為0以此類推)
            pos = double.Parse(Storage_Start_TB.Text) + double.Parse(Storage_Spacing_TB.Text) * Storage_Target_CBB.SelectedIndex;
            machine.OutputModle.CoverAndStorageElevatorAxis.MoveToAsync(pos);
        }

        private void Cover_GoTarget_BTN_Click(object sender, EventArgs e)
        {
            double pos;

            //起始點+間距*階層(第一層為0以此類推)
            pos = double.Parse(Cover_Start_TB.Text) + double.Parse(Cover_Spacing_TB.Text) * Cover_Target_CBB.SelectedIndex;
            machine.OutputModle.CoverAndStorageElevatorAxis.MoveToAsync(pos);

        }

        #endregion coverAndStorageElevator 事件

        private void slideTable_JogMinus_BTN_Click(object sender, EventArgs e)
        {

        }

        private void VacPaperAndForkMedci_btn_Click(object sender, EventArgs e)
        {

        }

        private void bottleReader_BTN_Click(object sender, EventArgs e)
        {

        }
    }



}
