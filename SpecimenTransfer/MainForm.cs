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
using System.IO;
using System.Reflection;

namespace WindowsFormsApp3
{
    public partial class MainForm : Form
    {

        //Modbus通訊


        private Machine machine;
        private MachineSetting machineSetting;
        private bool isSimulate = false;

        //// USB-4750 DI DO
        //private InstantDiCtrl instantDiCtrl = new InstantDiCtrl(); // 用於DI
        //private InstantDoCtrl instantDoCtrl = new InstantDoCtrl(); // 用於DO





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



        private async void MainForm_Load(object sender, EventArgs e)
        {


            //建立資料夾
            FolderInit();

            //顯示機械部位功能初始化
            ShowMechanicalPartInit();

            //讀取機械設定
            LoadMachineSetting();

            //IO 更新建構
            IoUiInit();


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


        private void UIAnchor()
        {
            View1_GB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));

            Log_GB.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom |
                                                                 System.Windows.Forms.AnchorStyles.Left |
                                                                 System.Windows.Forms.AnchorStyles.Right));

            Control_PN.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top |
                                                                     System.Windows.Forms.AnchorStyles.Right));

            Work_GB.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top |
                                                                  System.Windows.Forms.AnchorStyles.Right));

            View2_PN.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top |
                                                                   System.Windows.Forms.AnchorStyles.Bottom |
                                                                   System.Windows.Forms.AnchorStyles.Right));



            IDEL_PN.Anchor = ((System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top |
                                                                   System.Windows.Forms.AnchorStyles.Right));
        }

        private Label[] diLabel;
        private Label[] doLabel;
        private Thread updateIoThread;
        private bool isUpdateIoThreadStart;

        private void IoUiInit()
        {
            diLabel = new Label[32];
            doLabel = new Label[32];
            
            for (int i = 0; i < 32; i++)
            {
                string fieldname_di = string.Format("DI{0:d2}", i);
                diLabel[i] = (Label)this.GetType().GetField(fieldname_di, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).GetValue(this);

                string fieldname_do = string.Format("DO{0:d2}", i);
                doLabel[i] = (Label)this.GetType().GetField(fieldname_do, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).GetValue(this);
            }
            isUpdateIoThreadStart = false;

            // DOボタンのCLICKイベント追加
            foreach (Label lb in doLabel)
                lb.Click += new EventHandler(DO_Click);

        }
        private void DO_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < doLabel.Length; i++)
            {
                if (doLabel[i].Equals(sender))
                {
                    machine.IoOutList[i].Switch(!machine.IoOutList[i].IsSwitchOn);
                }
            }
        }

        private void MainTab_TC_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MainTab_TC.SelectedIndex == 2)
            {
                IoThreadStart();
            }
            else
            {
                IoThreadStop();
            }
        }
        private void IoThreadStart()
        {
            if (!isUpdateIoThreadStart)
            {
                // 創建一個執行緒來持續更新按鈕狀態
                updateIoThread = new Thread(UpdateIoLabel);
                updateIoThread.Start();
                isUpdateIoThreadStart = true;
            }
        }
        private void IoThreadStop()
        {
            if (isUpdateIoThreadStart)
            {
                updateIoThread.Abort();
                isUpdateIoThreadStart = false;
            }
        }


        private void UpdateIoLabel()
        {
            while (true)
            {
                for (int i = 0; i < 32; i++)
                {
                    bool signal = machine.IoInList[i].Signal;
                    //machine.IoOutList[i].Switch(true);
                    // 使用Control.Invoke將UI操作委派到UI執行緒上
                    diLabel[i].Invoke((MethodInvoker)(() =>
                    {
                        if (signal)
                        {
                            diLabel[i].BackColor = Color.Lime;
                            diLabel[i].Text = "ON";
                        }
                        else
                        {
                            diLabel[i].BackColor = Color.Red;
                            diLabel[i].Text = "OFF";
                        }

                        if(machine.IoOutList[i].IsSwitchOn)
                        {
                            doLabel[i].BackColor = Color.Lime;
                            doLabel[i].Text = "ON";

                        }
                        else
                        {
                            doLabel[i].BackColor = Color.Red;
                            doLabel[i].Text = "OFF";
                        }
                    }));
                }

                Thread.Sleep(1); // 等待一段時間再進行下一次更新
            }
            //while (true)
            //{
            //    // 隨機更新List<bool>中的元素值，模擬按鈕狀態的變化
            //    for (int i = 0; i < 32; i++)
            //    {
            //        if (machine.IoInList[i].Signal)
            //        {
            //            diLabel[i].BackColor = Color.Lime;
            //            diLabel[i].Text = "On";
            //        }
            //        else
            //        {
            //            diLabel[i].BackColor = Color.Red;
            //            diLabel[i].Text = "Off";

            //        }

            //        //if(machine.IoOutList[i])
            //        //{

            //        //}
            //    }

            //    // 等待一段時間再進行下一次更新，這裡設置為500毫秒
            //    Thread.Sleep(1);
            //}
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

        /// <summary>
        /// 專案文件資料夾路徑
        /// </summary>
        public string ProjectFolderPath
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\SpecimenTransfer"; }
        }






        private void ApplyMachinSetting_BTN_Click(object sender, EventArgs e)
        {
            //儲存機械設定
            SaveMachineSetting();
        }
        private void ReturnMachinSetting_BTN_Click(object sender, EventArgs e)
        {
            //儲存機械設定
            LoadMachineSetting();
        }
        private void SaveBackUpMachineSetting_BTN_Click(object sender, EventArgs e)
        {
            //儲存備份機械設定
            SaveBackupMachineSetting(BackUpMachineSetting_CBB.SelectedIndex + 1);
        }



        private void LoadBackUpMachineSetting_BTN_Click(object sender, EventArgs e)
        {
            //讀取備份機械設定
            LoadBackupMachineSetting(BackUpMachineSetting_CBB.SelectedIndex + 1);
        }

        /// <summary>
        /// 專案文件資料夾路徑
        /// </summary>
        public string MachineSettingFolderPath
        {
            get { return ProjectFolderPath + "\\MachineSetting"; }
        }

        private void FolderInit()
        {
            // 檢查資料夾是否存在，如果不存在，則建立資料夾

            //專案資料夾
            if (!Directory.Exists(ProjectFolderPath))
                Directory.CreateDirectory(ProjectFolderPath);

            //參數資料夾
            if (!Directory.Exists(MachineSettingFolderPath))
                Directory.CreateDirectory(MachineSettingFolderPath);
        }

        /// <summary>
        /// 儲存機械設定
        /// </summary>
        private void SaveMachineSetting()
        {
            machineSetting = UIToParam();

            machineSetting.Save(MachineSettingFolderPath + "\\MachineSetting.json");
        }
        /// <summary>
        /// 儲存備份機械設定
        /// </summary>
        private void SaveBackupMachineSetting(int number)
        {
            //備份只有1~3
            if (number <= 0)
                return;

            MachineSetting backUpMachineSetting = UIToParam();

            backUpMachineSetting.Save(MachineSettingFolderPath + "\\MachineSetting" + number.ToString() + ".json");
        }
        /// <summary>
        /// 讀取機械設定
        /// </summary>
        private void LoadMachineSetting()
        {
            string filepath = MachineSettingFolderPath + "\\MachineSetting.json";

            if (!File.Exists(filepath))
            {
                MachineSetting newMachineSetting = new MachineSetting();
                newMachineSetting.Save(filepath);
            }

            machineSetting = AbstractRecipe.Load<MachineSetting>(MachineSettingFolderPath + "\\MachineSetting.json");

            ParamToUI(machineSetting);

        }
        /// <summary>
        /// 儲存備份機械設定
        /// </summary>
        private void LoadBackupMachineSetting(int number)
        {
            //備份只有1~3
            if (number <= 0)
                return;

            MachineSetting backUpMachineSetting = AbstractRecipe.Load<MachineSetting>(MachineSettingFolderPath + "\\MachineSetting" + number.ToString() + ".json");

            ParamToUI(backUpMachineSetting);
        }
        /// <summary>
        /// 參數載入UI
        /// </summary>
        /// <param name="setting"></param>
        private void ParamToUI(MachineSetting setting)
        {
            //SlideTable 參數
            slideTable_JogDiatance_TB.Text = setting.OutputModuleParam.SlideTableJogDiatance.ToString();
            slideTable_Speed_TB.Text = setting.OutputModuleParam.SlideTableSpeed.ToString();

            slideTable_Load_TB.Text = setting.LoadModuleParam.SlideTableLoadPos.ToString();
            slideTable_Paper_TB.Text = setting.LoadModuleParam.SlideTablePaperPos.ToString();
            slideTable_Clean_TB.Text = setting.DumpModuleParam.SlideTableCleanPos.ToString();
            slideTable_Dump_TB.Text = setting.DumpModuleParam.SlideTableDumpPos.ToString();
            slideTable_Ink_TB.Text = setting.DumpModuleParam.SlideTableInkPos.ToString();
            slideTable_Gland_TB.Text = setting.OutputModuleParam.SlideTableGlandPos.ToString();
            slideTable_Cover_TB.Text = setting.OutputModuleParam.SlideTableCoverPos.ToString();
            slideTable_Output_TB.Text = setting.OutputModuleParam.SlideTableOutputPos.ToString();

            //filterPaperElevator 參數
            filterPaperElevator_JogDiatance_TB.Text = setting.LoadModuleParam.FilterPaperElevatorJogDiatance.ToString();
            filterPaperElevator_Speed_TB.Text = setting.LoadModuleParam.FilterPaperElevatorSpeed.ToString();
            filterPaperElevator_High_TB.Text = setting.LoadModuleParam.FilterPaperElevatorHighPos.ToString();
            filterPaperElevator_Low_TB.Text = setting.LoadModuleParam.FilterPaperElevatorLowPos.ToString();
            filterPaperElevator_Start_CBB.SelectedIndex = setting.LoadModuleParam.FilterPaperElevatorStartIndex;
            filterPaperElevator_Target_TB.Text = setting.LoadModuleParam.FilterPaperElevatorTargetPos.ToString();

            //bottleElevator 參數
            bottleElevator_JogDiatance_TB.Text = setting.DumpModuleParam.BottleElevatorJogDiatance.ToString();
            bottleElevator_Speed_TB.Text = setting.DumpModuleParam.BottleElevatorSpeed.ToString();
            bottleElevator_ScrewSpeed_TB.Text = setting.DumpModuleParam.BottleElevatorScrewSpeed.ToString();
            bottleElevator_Scan_TB.Text = setting.DumpModuleParam.BottleElevatorScanPos.ToString();
            bottleElevator_ScrewStart_TB.Text = setting.DumpModuleParam.BottleElevatorScrewStartPos.ToString();
            bottleElevator_ScrewTarget_TB.Text = setting.DumpModuleParam.BottleElevatorScrewTargetPos.ToString();

            //bottleScrew 參數
            bottleScrew_JogDiatance_TB.Text = setting.DumpModuleParam.BottleScrewJogDiatance.ToString();
            bottleScrew_Speed_TB.Text = setting.DumpModuleParam.BottleScrewSpeed.ToString();
            bottleScrew_Target_TB.Text = setting.DumpModuleParam.BottleScrewTargetPos.ToString();

            //bottleDump 參數
            bottleDump_JogDiatance_TB.Text = setting.DumpModuleParam.BottleDumpJogDiatance.ToString();
            bottleDump_Speed_TB.Text = setting.DumpModuleParam.BottleDumpSpeed.ToString();
            bottleDump_Start_TB.Text = setting.DumpModuleParam.BottleDumpStartPos.ToString();
            bottleDump_Target_TB.Text = setting.DumpModuleParam.BottleDumpTargetPos.ToString();

            //coverAndStorageElevator 參數
            coverAndStorageElevator_JogDiatance_TB.Text = setting.OutputModuleParam.CoverAndStorageElevatorJogDiatance.ToString();
            coverAndStorageElevator_Speed_TB.Text = setting.OutputModuleParam.CoverAndStorageElevatorSpeed.ToString();
            Storage_Start_TB.Text = setting.OutputModuleParam.StorageStartPos.ToString();
            Storage_Spacing_TB.Text = setting.OutputModuleParam.StorageSpacing.ToString();
            Storage_Target_CBB.SelectedIndex = setting.OutputModuleParam.StorageTargetIndex;
            Cover_Start_TB.Text = setting.OutputModuleParam.CoverStartPos.ToString();
            Cover_Spacing_TB.Text = setting.OutputModuleParam.CoverSpacing.ToString();
            Cover_Target_CBB.SelectedIndex = setting.OutputModuleParam.CoverTargetIndex;
        }
        /// <summary>
        /// UI轉換成參數
        /// </summary>
        private MachineSetting UIToParam()
        {
            MachineSetting setting = new MachineSetting();

            //SlideTable 參數
            setting.LoadModuleParam.SlideTableJogDiatance = Convert.ToDouble(slideTable_JogDiatance_TB.Text);
            setting.LoadModuleParam.SlideTableSpeed = Convert.ToDouble(slideTable_Speed_TB.Text);
            setting.DumpModuleParam.SlideTableJogDiatance = Convert.ToDouble(slideTable_JogDiatance_TB.Text);
            setting.DumpModuleParam.SlideTableSpeed = Convert.ToDouble(slideTable_Speed_TB.Text);
            setting.OutputModuleParam.SlideTableJogDiatance = Convert.ToDouble(slideTable_JogDiatance_TB.Text);
            setting.OutputModuleParam.SlideTableSpeed = Convert.ToDouble(slideTable_Speed_TB.Text);

            setting.LoadModuleParam.SlideTableLoadPos = Convert.ToDouble(slideTable_Load_TB.Text);
            setting.LoadModuleParam.SlideTablePaperPos = Convert.ToDouble(slideTable_Paper_TB.Text);
            setting.DumpModuleParam.SlideTableCleanPos = Convert.ToDouble(slideTable_Clean_TB.Text);
            setting.DumpModuleParam.SlideTableDumpPos = Convert.ToDouble(slideTable_Dump_TB.Text);
            setting.DumpModuleParam.SlideTableInkPos = Convert.ToDouble(slideTable_Ink_TB.Text);
            setting.OutputModuleParam.SlideTableGlandPos = Convert.ToDouble(slideTable_Gland_TB.Text);
            setting.OutputModuleParam.SlideTableCoverPos = Convert.ToDouble(slideTable_Cover_TB.Text);
            setting.OutputModuleParam.SlideTableOutputPos = Convert.ToDouble(slideTable_Output_TB.Text);

            //filterPaperElevator 參數
            setting.LoadModuleParam.FilterPaperElevatorJogDiatance = Convert.ToDouble(filterPaperElevator_JogDiatance_TB.Text);
            setting.LoadModuleParam.FilterPaperElevatorSpeed = Convert.ToDouble(filterPaperElevator_Speed_TB.Text);
            setting.LoadModuleParam.FilterPaperElevatorHighPos = Convert.ToDouble(filterPaperElevator_High_TB.Text);
            setting.LoadModuleParam.FilterPaperElevatorLowPos = Convert.ToDouble(filterPaperElevator_Low_TB.Text);
            setting.LoadModuleParam.FilterPaperElevatorStartIndex = filterPaperElevator_Start_CBB.SelectedIndex;
            setting.LoadModuleParam.FilterPaperElevatorTargetPos = Convert.ToDouble(filterPaperElevator_Target_TB.Text);

            //bottleElevator 參數
            setting.DumpModuleParam.BottleElevatorJogDiatance = Convert.ToDouble(bottleElevator_JogDiatance_TB.Text);
            setting.DumpModuleParam.BottleElevatorSpeed = Convert.ToDouble(bottleElevator_Speed_TB.Text);
            setting.DumpModuleParam.BottleElevatorScrewSpeed = Convert.ToDouble(bottleElevator_ScrewSpeed_TB.Text);
            setting.DumpModuleParam.BottleElevatorScanPos = Convert.ToDouble(bottleElevator_Scan_TB.Text);
            setting.DumpModuleParam.BottleElevatorScrewStartPos = Convert.ToDouble(bottleElevator_ScrewStart_TB.Text);
            setting.DumpModuleParam.BottleElevatorScrewTargetPos = Convert.ToDouble(bottleElevator_ScrewTarget_TB.Text);

            //bottleScrew 參數
            setting.DumpModuleParam.BottleScrewJogDiatance = Convert.ToDouble(bottleScrew_JogDiatance_TB.Text);
            setting.DumpModuleParam.BottleScrewSpeed = Convert.ToDouble(bottleScrew_Speed_TB.Text);
            setting.DumpModuleParam.BottleScrewTargetPos = Convert.ToDouble(bottleScrew_Target_TB.Text);

            //bottleDump 參數
            setting.DumpModuleParam.BottleDumpJogDiatance = Convert.ToDouble(bottleDump_JogDiatance_TB.Text);
            setting.DumpModuleParam.BottleDumpSpeed = Convert.ToDouble(bottleDump_Speed_TB.Text);
            setting.DumpModuleParam.BottleDumpStartPos = Convert.ToDouble(bottleDump_Start_TB.Text);
            setting.DumpModuleParam.BottleDumpTargetPos = Convert.ToDouble(bottleDump_Target_TB.Text);

            //coverAndStorageElevator 參數
            setting.OutputModuleParam.CoverAndStorageElevatorJogDiatance = Convert.ToDouble(coverAndStorageElevator_JogDiatance_TB.Text);
            setting.OutputModuleParam.CoverAndStorageElevatorSpeed = Convert.ToDouble(coverAndStorageElevator_Speed_TB.Text);
            setting.OutputModuleParam.StorageStartPos = Convert.ToDouble(Storage_Start_TB.Text);
            setting.OutputModuleParam.StorageSpacing = Convert.ToDouble(Storage_Spacing_TB.Text);
            setting.OutputModuleParam.StorageTargetIndex = Storage_Target_CBB.SelectedIndex;
            setting.OutputModuleParam.CoverStartPos = Convert.ToDouble(Cover_Start_TB.Text);
            setting.OutputModuleParam.CoverSpacing = Convert.ToDouble(Cover_Spacing_TB.Text);
            setting.OutputModuleParam.CoverTargetIndex = Cover_Target_CBB.SelectedIndex;

            return setting;

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
                case "slideTable_SetClean_BTN":
                    slideTable_Clean_TB.Text = machine.LoadModle.SlideTableAxis.Position.ToString();
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
                case "slideTable_GoLoad_BTN":
                    pos = double.Parse(slideTable_Load_TB.Text);
                    break;
                case "slideTable_GoPaper_BTN":
                    pos = double.Parse(slideTable_Paper_TB.Text);
                    break;
                case "slideTable_GoClean_BTN":
                    pos = double.Parse(slideTable_Clean_TB.Text);
                    break;
                case "slideTable_GoDump_BTN":
                    pos = double.Parse(slideTable_Dump_TB.Text);
                    break;
                case "slideTable_GoInk_BTN":
                    pos = double.Parse(slideTable_Ink_TB.Text);
                    break;
                case "slideTable_GoGland_BTN":
                    pos = double.Parse(slideTable_Gland_TB.Text);
                    break;
                case "slideTable_GoCover_BTN":
                    pos = double.Parse(slideTable_Cover_TB.Text);
                    break;
                case "slideTable_GoOutput_BTN":
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
            double pos;
            pos = 0;
            machine.DumpModle.BottleScrewAxis.MoveToAsync(pos);
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

        private void MachinePicture_PB_Paint(object sender, PaintEventArgs e)
        {
            //// 获取要连接的两个控件的位置
            //Point start = new Point(100, 100); // 第一个控件的位置
            //Point end = new Point(300, 200);   // 第二个控件的位置

            //// 计算箭头的各个点的位置
            //Point arrowPoint1 = new Point(end.X - 10, end.Y - 10);
            //Point arrowPoint2 = new Point(end.X - 10, end.Y + 10);

            //// 创建画笔和画刷
            //Pen pen = new Pen(Color.Black, 2);
            //SolidBrush brush = new SolidBrush(Color.Black);

            //// 绘制直线
            //e.Graphics.DrawLine(pen, start, end);

            //// 绘制箭头
            //Point[] arrowPoints = { end, arrowPoint1, arrowPoint2 };
            //e.Graphics.FillPolygon(brush, arrowPoints);
        }




        /// <summary>
        /// 顯示機械部位初始化
        /// </summary>
        private void ShowMechanicalPartInit()
        {
            //顯示元件加入
            ShowMechanicalPart_PB.Parent = MachinePicture_PB;

            //事件加入
            slideTable_GB.MouseEnter += Setting_MouseEnter;

            slideTable_Load_PN.MouseEnter += Setting_MouseEnter;
            slideTable_Paper_PN.MouseEnter += Setting_MouseEnter;
            slideTable_Dump_PN.MouseEnter += Setting_MouseEnter;
            slideTable_Ink_PN.MouseEnter += Setting_MouseEnter;
            slideTable_Gland_PN.MouseEnter += Setting_MouseEnter;
            slideTable_Cover_PN.MouseEnter += Setting_MouseEnter;
            slideTable_Output_PN.MouseEnter += Setting_MouseEnter;

            filterPaperElevator_GB.MouseEnter += Setting_MouseEnter;
            bottleElevator_GB.MouseEnter += Setting_MouseEnter;
            bottleScrew_GB.MouseEnter += Setting_MouseEnter;
            bottleDump_GB.MouseEnter += Setting_MouseEnter;
            coverAndStorageElevator_GB.MouseEnter += Setting_MouseEnter;
            Storage_GB.MouseEnter += Setting_MouseEnter;
            Cover_GB.MouseEnter += Setting_MouseEnter;

            paperReader_PN.MouseEnter += Setting_MouseEnter;
            bottleReader_PN.MouseEnter += Setting_MouseEnter;

            Back_PN.MouseEnter += Setting_MouseEnter;
            MachinePicture_PB.MouseEnter += Setting_MouseEnter;


        }
        private void Setting_MouseEnter(object sender, EventArgs e)
        {
            //顯示機械部位事件

            Point pos = new Point(0, 0);
            Size size = new Size(0, 0);

            if (sender is Panel)
            {
                switch (((Panel)sender).Name)
                {
                    case "slideTable_Load_PN":
                        pos = new Point(1206 - 188, 709 - 245);
                        size = new Size(72, 105);
                        break;
                    case "slideTable_Paper_PN":
                        pos = new Point(1104 - 188, 685 - 245);
                        size = new Size(72, 105);
                        break;
                    case "slideTable_Dump_PN":
                        pos = new Point(881 - 188, 638 - 245);
                        size = new Size(72, 105);
                        break;
                    case "slideTable_Ink_PN":
                        pos = new Point(733 - 188, 596 - 245);
                        size = new Size(72, 105);
                        break;
                    case "slideTable_Gland_PN":
                        pos = new Point(647 - 188, 581 - 245);
                        size = new Size(72, 105);
                        break;
                    case "slideTable_Cover_PN":
                        pos = new Point(589 - 188, 575 - 245);
                        size = new Size(72, 105);
                        break;
                    case "slideTable_Output_PN":
                        pos = new Point(424 - 188, 542 - 245);
                        size = new Size(72, 105);
                        break;
                    case "paperReader_PN":
                        pos = new Point(1256 - 188, 403 - 245);
                        size = new Size(94, 122);
                        break;
                    case "bottleReader_PN":
                        pos = new Point(845 - 188, 328 - 245);
                        size = new Size(81, 120);
                        break;
                    case "Back_PN":
                        pos = new Point(0, 0);
                        size = new Size(0, 0);
                        break;
                    default:
                        break;
                }

                ShowMechanicalPart_PB.BackColor = Color.FromArgb(85, ((Panel)sender).BackColor);

            }
            else if (sender is GroupBox)
            {
                switch (((GroupBox)sender).Name)
                {
                    case "slideTable_GB":
                        pos = new Point(209 - 188, 479 - 245);
                        size = new Size(205, 205);
                        break;
                    case "filterPaperElevator_GB":
                        pos = new Point(1110 - 188, 526 - 245);
                        size = new Size(84, 134);
                        break;
                    case "bottleElevator_GB":
                        pos = new Point(969 - 188, 296 - 245);
                        size = new Size(109, 312);
                        break;
                    case "bottleScrew_GB":
                        pos = new Point(878 - 188, 460 - 245);
                        size = new Size(75, 110);
                        break;
                    case "bottleDump_GB":
                        pos = new Point(878 - 188, 562 - 245);
                        size = new Size(80, 51);
                        break;
                    case "coverAndStorageElevator_GB":
                        pos = new Point(507 - 188, 331 - 245);
                        size = new Size(90, 268);
                        break;
                    case "Cover_GB":
                        pos = new Point(589 - 188, 448 - 245);
                        size = new Size(60, 152);
                        break;
                    case "Storage_GB":
                        pos = new Point(428 - 188, 374 - 245);
                        size = new Size(61, 182);
                        break;
                    default:
                        break;
                }

                ShowMechanicalPart_PB.BackColor = Color.FromArgb(85, ((GroupBox)sender).BackColor);

            }
            else
            {
                pos = new Point(0, 0);
                ShowMechanicalPart_PB.Size = new Size(0, 0);

            }


            ShowMechanicalPart_PB.Location = pos;
            ShowMechanicalPart_PB.Size = size;
        }

        private void coverAndStorageElevator_JogDiatance_LB_DoubleClick(object sender, EventArgs e)
        {
            Storage_Start_TB.ReadOnly = false;
            Storage_Spacing_TB.ReadOnly = false;
            Cover_Start_TB.ReadOnly = false;
            Cover_Spacing_TB.ReadOnly = false;
            bottleDump_Start_TB.ReadOnly = false;
            bottleDump_Target_TB.ReadOnly = false;
            bottleScrew_Target_TB.ReadOnly = false;
            bottleElevator_Scan_TB.ReadOnly = false;
            bottleElevator_ScrewStart_TB.ReadOnly = false;
            bottleElevator_ScrewTarget_TB.ReadOnly = false;
            filterPaperElevator_High_TB.ReadOnly = false;
            filterPaperElevator_Low_TB.ReadOnly = false;
            filterPaperElevator_Target_TB.ReadOnly = false;
            slideTable_Load_TB.ReadOnly = false;
            slideTable_Paper_TB.ReadOnly = false;
            slideTable_Clean_TB.ReadOnly = false;
            slideTable_Dump_TB.ReadOnly = false;
            slideTable_Ink_TB.ReadOnly = false;
            slideTable_Gland_TB.ReadOnly = false;
            slideTable_Cover_TB.ReadOnly = false;
            slideTable_Output_TB.ReadOnly = false;

        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            IoThreadStop();
        }

        private void Back_PN_Paint(object sender, PaintEventArgs e)
        {

        }
    }



}
