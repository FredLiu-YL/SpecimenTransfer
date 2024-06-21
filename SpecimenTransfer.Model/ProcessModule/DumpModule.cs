using SpecimenTransfer.Model.Component;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;


namespace SpecimenTransfer.Model
{

    public class DumpModule
    {
        DateTime date = DateTime.Now;
        public string TodayDateTime => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        public string Today => DateTime.Now.ToString("yyyy-MM-dd");

        public event Action<string> WriteLog;

        //Log委派
        public event Action<string> OnProcessCompleted;

        #region Digital Output   

        //camera shot藥罐條碼
        private DigitalOutput shotMedcineBottleBarcode;

        //camera拍照
        private DigitalOutput cameraShot;

        //上方夾藥罐氣缸-打開關閉
        private DigitalOutput upperClampMedicineCylinder;

        //下方夾藥罐氣缸-打開關閉
        private DigitalOutput lowerClampMedicineCylinder;

        //藥罐移載氣缸
        private DigitalOutput medicineBottleMoveCylinder;

        //背光氣缸
        private DigitalOutput backLightCylinder;

        //注射清洗氣缸
        private DigitalOutput injectionCleanCylinder;

        //注射清洗
        private DigitalOutput injectionCleanSwitch;

        //注射紅墨水
        private DigitalOutput injectRedInk;

        //紅墨水氣缸
        private DigitalOutput redInkCylinder;
        #endregion

        #region DigitalInput    

        //上方夾藥罐氣缸-打開訊號
        private DigitalIntput upperClampMedicineCylinderOpenSignal;
        //上方夾藥罐氣缸-關閉訊號
        private DigitalIntput upperClampMedicineCylinderCloseSignal;

        //下方夾藥罐氣缸-打開訊號
        private DigitalIntput lowerClampMedicineCylinderOpenSignal;
        //下方夾藥罐氣缸-關閉訊號
        private DigitalIntput lowerClampMedicineCylinderCloseSignal;

        //藥罐移載氣缸-推
        private DigitalIntput medicineBottleMoveCylinderPushSignal;
        //藥罐移載氣缸-收
        private DigitalIntput medicineBottleMoveCylinderPullSignal;

        //背光氣缸-推
        private DigitalIntput backLightCylinderPushSignal;
        //背光氣缸-收
        private DigitalIntput backLightCylinderPullSignal;

        //注射氣缸-推
        private DigitalIntput injectionCylinderPushSignal;
        //注射氣缸-收
        private DigitalIntput injectionCylinderPullSignal;

        //紅墨水氣缸-推
        private DigitalIntput redInkCylinderPushSignal;
        //紅墨水氣缸-收
        private DigitalIntput redInkCylinderPullSignal;
        #endregion
        //載體滑台
        public IAxis SlideTableAxis { get; set; }
        //藥罐升降滑台-home
        public IAxis BottleElevatorAxis { get; set; }
        //旋藥蓋
        public IAxis BottleScrewAxis { get; set; }
        //藥瓶傾倒
        public IAxis BottleDumpAxis { get; set; }
        //----條碼----
        //藥罐條碼
        private IBarcodeReader medcineBottleBarcode;

        public Action SetupJar;

        /// <summary>
        /// 入料模組參數
        /// </summary>
        public DumpModuleParamer DumpModuleParam { get; set; } = new DumpModuleParamer();


        public DumpModule()
        {

        }

        public DumpModule(DigitalOutput[] signalOutput, DigitalIntput[] signalInput,
            IAxis slideTableAxis, IAxis bottleElevatorAxis, IAxis bottleScrewAxis, IAxis bottleDumpAxis, IBarcodeReader bottleReader)
        {
            //----Digital Output----
            shotMedcineBottleBarcode = signalOutput[1];//注射清洗氣缸

            injectionCleanCylinder = signalOutput[5];//注射清洗氣缸

            injectionCleanSwitch = signalOutput[6];//注射清洗

            upperClampMedicineCylinder = signalOutput[7];//藥罐瓶蓋氣缸-上夾爪

            lowerClampMedicineCylinder = signalOutput[8];//藥罐瓶蓋氣缸-下夾爪

            medicineBottleMoveCylinder = signalOutput[9];//藥罐移載氣缸

            //cameraShot = signalOutput[10];//camera拍照

            backLightCylinder = signalOutput[10];//背光氣缸
            injectRedInk = signalOutput[11];//注射紅墨水
            redInkCylinder = signalOutput[12];//紅墨水氣缸

            //----Digital Input----
            injectionCylinderPushSignal = signalInput[10];//注射清洗氣缸-推
            injectionCylinderPullSignal = signalInput[11];//注射清洗氣缸-收

            medicineBottleMoveCylinderPushSignal = signalInput[12];//藥罐移載氣缸-推
            medicineBottleMoveCylinderPullSignal = signalInput[13];//藥罐移載氣缸-收

            upperClampMedicineCylinderCloseSignal = signalInput[14];//藥罐瓶蓋上夾爪-關
            upperClampMedicineCylinderOpenSignal = signalInput[15];//藥罐瓶蓋上夾爪-開

            lowerClampMedicineCylinderCloseSignal = signalInput[16]; //藥罐瓶蓋下夾爪-關
            lowerClampMedicineCylinderOpenSignal = signalInput[17]; //藥罐瓶蓋下夾爪-開

            backLightCylinderPushSignal = signalInput[18];//背光氣缸-推
            backLightCylinderPullSignal = signalInput[19];//背光氣缸-收

            redInkCylinderPushSignal = signalInput[20];//紅墨水氣缸-推
            redInkCylinderPullSignal = signalInput[21];//紅墨水氣缸-收

            //----軸控----
            this.SlideTableAxis = slideTableAxis;//載體滑台
            this.BottleElevatorAxis = bottleElevatorAxis;//藥罐升降滑台
            this.BottleScrewAxis = bottleScrewAxis;//旋藥蓋
            this.BottleDumpAxis = bottleDumpAxis;//藥罐傾倒

            //Barcode reader
            //藥罐條碼
            medcineBottleBarcode = bottleReader;

        }

        //人工放藥罐
        public async Task LoadBottle()
        {

            //目前由人完成藥罐的載入先委派出去
            SetupJar.Invoke();
            WriteLog?.Invoke(TodayDateTime + "  " + "人工裝載藥罐中");

        }

        object monitorOBJ = new object();

        //軸原點復歸
        public async Task Home()
        {

                try
                {

                    WriteLog?.Invoke(TodayDateTime + "  " + "DumpModule初始化中");
                    upperClampMedicineCylinder.Switch(false);
                    lowerClampMedicineCylinder.Switch(false);
                    injectionCleanSwitch.Switch(false);
                    injectRedInk.Switch(false);
                    medicineBottleMoveCylinder.Switch(false);

                  //藥罐旋轉軸原點復歸
                    double bottleScrewNowPos = BottleScrewAxis.GetPosition();
                    if(bottleScrewNowPos != 0)
                    await BottleScrewAxis.Home();

                    bool bottleScrewAxisInHomeStatus = await Task.Run(async () =>
                    {
                    while (!BottleScrewAxis.IsHome)
                    {
                        
                        await Task.Delay(1000); 
                    }
                    return true; // 回 Home 完成

                    });
                     WriteLog?.Invoke(TodayDateTime + "  " + "藥罐旋轉軸原點復歸狀態:" + bottleScrewAxisInHomeStatus);

                    Task.Delay(2000);


                    //藥罐傾倒軸原點復歸
                    double bottleDumpNowPos = BottleDumpAxis.GetPosition();
                    if (bottleDumpNowPos != 0)
                    await BottleDumpAxis.Home();

                    bool bottleDumpAxisInHomeStatus = await Task.Run(async () =>
                    {
                    while (!BottleDumpAxis.IsHome)
                    {
                        
                        await Task.Delay(1000); 
                    }
                    return true; // 回 Home 完成

                    });
                     WriteLog?.Invoke(TodayDateTime + "  " + "藥罐傾倒軸原點復歸狀態:" + bottleDumpAxisInHomeStatus);


                   //藥罐升降軸原點復歸
                    double bottleElevatorNowPos = BottleElevatorAxis.GetPosition();
                    if (bottleElevatorNowPos != 0)
                    await BottleElevatorAxis.Home();

                    bool bottleElevatorAxisInHomeStatus = await Task.Run(async () =>
                     {
                        while (!BottleElevatorAxis.IsHome)
                        {
                            
                             await Task.Delay(1000); 
                         }
                         return true; // 回 Home 完成

                     });
                      WriteLog?.Invoke(TodayDateTime + "  " + "藥罐升降軸原點復歸狀態:" + bottleElevatorAxisInHomeStatus);

                if (bottleElevatorAxisInHomeStatus)
                    await BottleElevatorAxis.MoveToAsync(23000);

                    WriteLog?.Invoke(TodayDateTime + "  " + "藥罐升降軸絕對位置狀態:" + bottleElevatorAxisInHomeStatus);
                    WriteLog?.Invoke(TodayDateTime + "  " + "DumpModule初始化完成");
                }

                catch (Exception error)
                {
                    error.ToString();
                }

        }
        /// <summary>
        /// 旋開藥罐
        /// </summary>
        /// <returns></returns>
        public async Task UnscrewMedicineJar()
        {

            WriteLog?.Invoke(TodayDateTime + "  " + "旋開藥罐開始");
            try
            {
                //藥罐升降軸夾取瓶身位
                BottleElevatorAxis.SetVelocity(80, 1, 1);
                Thread.Sleep(1000);
                BottleElevatorAxis.MoveToAsync(11620);
                Thread.Sleep(2000);

                //藥罐旋轉軸開蓋位
                double nowPos = BottleElevatorAxis.GetPosition();
                if (nowPos == 11620 )
                {
                    lowerClampMedicineCylinder.Switch(true);
                    Thread.Sleep(1000);
                    //藥罐旋轉軸開蓋位
                    BottleScrewAxis.SetVelocity(1000, 2, 2);
                    Thread.Sleep(2000);
                    BottleScrewAxis.MoveToAsync(9500);

                    //藥罐升降軸開蓋加速位
                    Thread.Sleep(8000);

                    BottleElevatorAxis.SetVelocity(1, 1, 1);
                    Thread.Sleep(2000);
                    BottleElevatorAxis.MoveToAsync(22000);

                    //BottleScrewAxis.MoveAsync(DumpModuleParam.BottleScrewStandbyPos);
                    //BottleElevatorAxis.MoveAsync(DumpModuleParam.BottleElevatorStandbyPos);
                }
                //藥罐升降軸開蓋停止位
                Thread.Sleep(13000);
                double nowPos2 = BottleElevatorAxis.GetPosition();
                if (nowPos2 == 22000)
                {
                    Thread.Sleep(1000);
                    BottleElevatorAxis.SetVelocity(50, 1, 1);
                    Thread.Sleep(1000);
                    BottleElevatorAxis.MoveToAsync(46000);
                  

                }
                WriteLog?.Invoke(TodayDateTime + "  " + "旋開藥罐完成");

            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 旋緊藥蓋
        /// </summary>
        /// <returns></returns>
        public async Task ScrewMedicineJar()
        {
            WriteLog?.Invoke(TodayDateTime + "  " + "旋緊藥罐開始");
            try
            {
                //藥罐上夾爪夾->藥罐下夾爪夾->旋緊藥蓋->藥罐下降目標位置
                await Task.Delay(2000);
                double nowPos = BottleElevatorAxis.GetPosition();
                if (nowPos == 46000)
                {

                    //藥罐升降軸鎖瓶蓋預鎖位
                    await Task.Delay(1000);
                    BottleElevatorAxis.SetVelocity(50, 1, 1);
                    await Task.Delay(1000);
                    BottleElevatorAxis.MoveToAsync(22000);

                    //BottleScrewAxis.MoveAsync(DumpModuleParam.BottleScrewTargetPos);
                    //BottleElevatorAxis.MoveAsync(DumpModuleParam.BottleElevatorScanPos);

                }

                await Task.Delay(2000);

                double nowPos1 = BottleElevatorAxis.GetPosition();
                if (nowPos1 == 22000)
                {
                    await Task.Delay(1000);
                    //藥罐升降軸至旋緊位
                    BottleElevatorAxis.SetVelocity(10, 1, 1);
                    await Task.Delay(1000);
                    BottleElevatorAxis.MoveToAsync(12500);
                    await Task.Delay(3000);

                    //藥罐旋轉軸旋緊位
                    BottleScrewAxis.SetVelocity(300, 1, 1);
                    await Task.Delay(3000);
                    BottleScrewAxis.MoveToAsync(0);

                }

                //藥罐升降軸旋緊停止上升位
                await Task.Delay(36000);
                double nowPos2 = BottleElevatorAxis.GetPosition();
                if (nowPos2 == 12500)
                {
                    upperClampMedicineCylinder.Switch(false);
                    await Task.Delay(1000);
                    BottleElevatorAxis.MoveToAsync(46000);
                    await Task.Delay(1000);

                }

                WriteLog?.Invoke(TodayDateTime + "  " + "旋緊藥罐完成");
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 傾倒藥罐
        /// </summary>
        /// <returns></returns>
        public async Task DumpBottle()
        {
            WriteLog?.Invoke(TodayDateTime + "  " + "傾倒藥罐開始");
            try
            {
                // 藥罐移載氣缸收->載體盒到位->傾倒藥罐
                medicineBottleMoveCylinder.Switch(true);
                Thread.Sleep(1000);
                backLightCylinder.Switch(true);
                Thread.Sleep(1000);
                injectionCleanCylinder.Switch(true);
                Thread.Sleep(2000);
                medicineBottleMoveCylinder.Switch(false);
                //Thread.Sleep(1000);
                backLightCylinder.Switch(false);

                Thread.Sleep(1000);
                double nowPos = BottleDumpAxis.GetPosition();
                
                if (nowPos >= 0 || nowPos <= 100)
                {
                    Thread.Sleep(1000);
                    BottleDumpAxis.MoveToAsync(2700);
                }

                Thread.Sleep(1000);
                WriteLog?.Invoke(TodayDateTime + "  " + "傾倒藥罐結束");

                //BottleDumpAxis.MoveToAsync(DumpModuleParam.BottleDumpTargetPos);
                //WaitInputSignal(medicineBottleMoveCylinderPullSignal);        
                //WaitAxisSignal(BottleDumpAxis.IsInposition);

            }

            catch (Exception ex)
            {
                throw ex;

            }

        }

        /// <summary>
        /// 清洗藥罐
        /// </summary>
        /// <returns></returns>
        public async Task CleanBottle()
        {

            WriteLog?.Invoke(TodayDateTime + "  " + "清洗藥罐開始");
            try
            {
                injectionCleanSwitch.Switch(true);
                Thread.Sleep(500);
                injectionCleanSwitch.Switch(false);

                Thread.Sleep(2000);
                double nowPos = BottleDumpAxis.GetPosition();
                if (nowPos >= 2600 || nowPos  <= 2700)
                    BottleDumpAxis.Home();

                Thread.Sleep(3000);
                double nowPos1 = BottleDumpAxis.GetPosition();
                if (nowPos1 == 0)
                {
                    medicineBottleMoveCylinder.Switch(true);
                    Thread.Sleep(1000);
                    backLightCylinder.Switch(true);
                    Thread.Sleep(1000);
                    medicineBottleMoveCylinder.Switch(false);
                    Thread.Sleep(1000);
                    backLightCylinder.Switch(false);
                    injectionCleanCylinder.Switch(false);

                }
                WriteLog?.Invoke(TodayDateTime + "  " + "清洗藥罐完成");
                //BottleDumpAxis.MoveAsync(DumpModuleParam.BottleDumpStandbyPos);
                //WaitAxisSignal(BottleDumpAxis.IsInposition);
            }

            catch (Exception ex)
            {
                throw ex;

            }

        }

        /// <summary>
        /// 檢查藥罐
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckBottleAction()
        {
            try
            {
                //藥罐移載氣缸推->背光氣缸推->拍照->藥罐移載氣缸收->背光氣缸收
                medicineBottleMoveCylinder.Switch(true);
                Thread.Sleep(1000);
                backLightCylinder.Switch(true);

                //WaitInputSignal(medicineBottleMoveCylinderPushSignal);
                //WaitInputSignal(backLightCylinderPushSignal);

                //cameraShot.Switch(true);
                //await Task.Delay(500);
                //cameraShot.Switch(false);

                Thread.Sleep(1000);
                medicineBottleMoveCylinder.Switch(false);
                backLightCylinder.Switch(false);

                //WaitInputSignal(medicineBottleMoveCylinderPullSignal);
                //WaitInputSignal(backLightCylinderPullSignal);

                return true;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 注入紅墨水
        /// </summary>
        /// <returns></returns>
        public async Task InjectRedInk()
        {
            WriteLog?.Invoke(TodayDateTime + "  " + "注入紅墨水開始");
            try
            {

                await Task.Delay(3000);
                double nowPos = SlideTableAxis.GetPosition();
                if(nowPos == 234280)
                {
                    redInkCylinder.Switch(true);
                    await Task.Delay(2000);
                    injectRedInk.Switch(true);
                    await Task.Delay(300);
                    injectRedInk.Switch(false);
                    await Task.Delay(1000);
                    redInkCylinder.Switch(false);
                }

                WriteLog?.Invoke(TodayDateTime + "  " + "注入紅墨水完成");
                //WaitInputSignal(redInkCylinderPushSignal);
                //WaitInputSignal(redInkCylinderPullSignal);

            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        //table移動至清洗站
        public async Task CarrierMoveToDump()
        {
            try
            {
                //載體滑台移動至清洗站
                //SlideTableAxis.MoveAsync(DumpModuleParam.SlideTableDumpPos);
                //SlideTableAxis.SetVelocity();
                SlideTableAxis.MoveToAsync(382670);
                await Task.Delay(500);
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }


        //table移動至紅墨水站
        public async Task CarrierMoveToRedInk()
        {
            try
            {
                
                //載體滑台移動至紅墨水站
                 SlideTableAxis.MoveToAsync(234280);
                //SlideTableAxis.MoveToAsync(DumpModuleParam.SlideTableInkPos);
                await Task.Delay(1000);
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        //讀藥罐條碼
        public async Task<string> ReadBarcode()
        {

            string carrierDataReceived;

            try
            {
                WriteLog?.Invoke(TodayDateTime + "  " + "讀取藥罐條碼");
                //BottleScrewAxis.MoveToAsync(8000);
                shotMedcineBottleBarcode.Switch(true);
                shotMedcineBottleBarcode.Switch(false);
                //await Task.Delay(3000);
                carrierDataReceived = medcineBottleBarcode.ReceiveData();


            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                shotMedcineBottleBarcode.Switch(false);
            }

            return carrierDataReceived;

        }

        private void WaitInputSignal(DigitalIntput intput, int timeout = 1000)
        {

            try
            {
                SpinWait.SpinUntil(() => intput.Signal, timeout);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private void WaitAxisSignal(bool isInposition)
        {
            try
            {
                SpinWait.SpinUntil(() => isInposition);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }

    public class DumpModuleParamer
    {
        /// <summary>
        /// 移載橫移軸 Jog移動量
        /// </summary>
        public double SlideTableJogDiatance { get; set; }
        /// <summary>
        /// 移載橫移軸 移動速度
        /// </summary>
        public double SlideTableSpeed { get; set; }

        /// <summary>
        /// 移載橫移軸 待命位
        /// </summary>
        public double SlideTableStandByPos { get; set; }

        /// <summary>
        /// 移載橫移軸 清洗位
        /// </summary>
        public double SlideTableCleanPos { get; set; }
        /// <summary>
        /// 移載橫移軸 傾倒位
        /// </summary>
        public double SlideTableDumpPos { get; set; }
        /// <summary>
        /// 移載橫移軸 墨水位
        /// </summary>
        public double SlideTableInkPos { get; set; }

        /// <summary>
        /// 瓶罐升降軸 Jog移動量
        /// </summary>
        public double BottleElevatorJogDiatance { get; set; }
        /// <summary>
        /// 瓶罐升降軸 移動速度
        /// </summary>
        public double BottleElevatorSpeed { get; set; }
        /// <summary>
        /// 瓶罐升降軸 旋轉中速度
        /// </summary>
        public double BottleElevatorScrewSpeed { get; set; }

        /// <summary>
        /// 瓶罐升降軸 待命位
        /// </summary>
        public double BottleElevatorStandbyPos { get; set; }

        /// <summary>
        /// 瓶罐升降軸 掃描位
        /// </summary>
        public double BottleElevatorScanPos { get; set; }
        /// <summary>
        /// 瓶罐升降軸 旋轉起點位
        /// </summary>
        public double BottleElevatorScrewStartPos { get; set; }
        /// <summary>
        /// 瓶罐升降軸 旋轉終點位
        /// </summary>
        public double BottleElevatorScrewTargetPos { get; set; }


        /// <summary>
        /// 瓶蓋旋轉軸 Jog移動量
        /// </summary>
        public double BottleScrewJogDiatance { get; set; }
        /// <summary>
        /// 瓶蓋旋轉軸 移動速度
        /// </summary>
        public double BottleScrewSpeed { get; set; }

        /// <summary>
        /// 瓶蓋旋轉軸 待命位
        /// </summary>
        public double BottleScrewStandbyPos { get; set; }

        /// <summary>
        /// 瓶蓋旋轉軸 目標位
        /// </summary>
        public double BottleScrewTargetPos { get; set; }


        /// <summary>
        /// 瓶蓋傾倒軸 Jog移動量
        /// </summary>
        public double BottleDumpJogDiatance { get; set; }
        /// <summary>
        /// 瓶蓋傾倒軸 移動速度
        /// </summary>
        public double BottleDumpSpeed { get; set; }

        /// <summary>
        /// 瓶蓋傾倒軸 待命位
        /// </summary>
        public double BottleDumpStandbyPos { get; set; }

        /// <summary>
        /// 瓶蓋傾倒軸 起點位
        /// </summary>
        public double BottleDumpStartPos { get; set; }
        /// <summary>
        /// 瓶蓋旋轉軸 目標位
        /// </summary>
        public double BottleDumpTargetPos { get; set; }




    }








}



