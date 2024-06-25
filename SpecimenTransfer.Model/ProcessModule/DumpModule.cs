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
                    await BottleScrewAxis.Home();
                    bool bottleScrewAxisInHomeStatus = await Task.Run(async () =>
                    {
                    while (!BottleScrewAxis.IsHome)
                    {
                        // 短暫延遲，避免過度佔用 CPU
                        await Task.Delay(1000); // 10 毫秒 (可調整)
                    }
                    return true; // 回 Home 完成

                    });
                     WriteLog?.Invoke(TodayDateTime + "  " + "藥罐旋轉軸原點復歸狀態:" + bottleScrewAxisInHomeStatus);


                    //藥罐傾倒軸原點復歸

                    await Task.Delay(500);
                    await BottleDumpAxis.Home();
                    bool bottleDumpAxisInHomeStatus = await Task.Run(async () =>
                    {
                    while (!BottleDumpAxis.IsHome)
                    {
                        // 短暫延遲，避免過度佔用 CPU
                        await Task.Delay(1000); // 10 毫秒 (可調整)
                    }
                    return true; // 回 Home 完成

                    });
                     WriteLog?.Invoke(TodayDateTime + "  " + "藥罐傾倒軸原點復歸狀態:" + bottleDumpAxisInHomeStatus);


                    //藥罐升降軸原點復歸
                    await Task.Delay(500);
                    await BottleElevatorAxis.Home();
                    bool bottleElevatorAxisInHomeStatus = await Task.Run(async () =>
                     {
                        while (!BottleElevatorAxis.IsHome)
                        {
                            // 短暫延遲，避免過度佔用 CPU
                             await Task.Delay(1000); // 10 毫秒 (可調整)
                         }
                         return true; // 回 Home 完成

                     });
                      WriteLog?.Invoke(TodayDateTime + "  " + "藥罐升降軸原點復歸狀態:" + bottleElevatorAxisInHomeStatus);


                await Task.Delay(500);
                await BottleElevatorAxis.SetVelocity(90, 1, 1);   
                await BottleElevatorAxis.MoveToAsync(23000);
                bool bottleElevatorAxisInHomeStatus2 = await Task.Run(async () =>
                {
                    while (!BottleElevatorAxis.IsInposition)
                    {
                        // 短暫延遲，避免過度佔用 CPU
                        await Task.Delay(1000); // 10 毫秒 (可調整)
                    }
                    return true; // 回 Home 完成

                });

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
                await BottleElevatorAxis .MoveToAsync(11620);

                //藥罐升降軸下降夾取瓶身位置
                bool bottleElevatorAxisInPosStatus = await Task.Run(async () =>
                {
                    while (!BottleElevatorAxis.IsInposition)
                    {        
                        await Task.Delay(1000); 
                    }
                    lowerClampMedicineCylinder.Switch(true);
                    await Task.Delay(500);
                    return true; 

                });

                BottleScrewAxis.SetVelocity(1000, 2, 2);
                //await Task.Delay(1000);
                await BottleScrewAxis .MoveToAsync(9500);
                //await Task.Delay(1000);

                //藥罐旋轉軸開蓋位置
                bool bottleScrewAxisInPosStatus = await Task.Run(async () =>
                {
                    while (!BottleScrewAxis.IsInposition)
                    {
                        await Task.Delay(1000);
                    }
                    
                    return true;

                });

                //await Task.Delay(1000);
                //藥罐升降軸開蓋位置
                BottleElevatorAxis.SetVelocity(50, 1, 1);
                await BottleElevatorAxis .MoveToAsync(22000);

                bool bottleElevatorAxisInPosStatus1 = await Task.Run(async () =>
                {
                    while (!BottleElevatorAxis.IsInposition)
                    {
                        await Task.Delay(1000);
                    }

                    return true;

                });

                
                //藥罐升降軸開蓋停止位
                if(BottleElevatorAxis.GetPosition() == 22000)
                {
                    BottleElevatorAxis.SetVelocity(90, 1, 1);
                    await BottleElevatorAxis .MoveToAsync(46000);
                }
                bool bottleElevatorAxisInPosStatus2 = await Task.Run(async () =>
                {
                    while (!BottleElevatorAxis.IsInposition)
                    {
                        await Task.Delay(1000);
                    }

                    return true;

                });
                WriteLog?.Invoke(TodayDateTime + "  " + "旋開藥罐完成");
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
     
                // 氣缸作業流程
                medicineBottleMoveCylinder.Switch(true);
                await Task.Delay(1000);
                backLightCylinder.Switch(true);
                await Task.Delay(1000);
                injectionCleanCylinder.Switch(true);
                await Task.Delay(1000);
                medicineBottleMoveCylinder.Switch(false);
                backLightCylinder.Switch(false);

                await Task.Delay(500);

                //藥罐傾倒作業
                
                try
                {
                //BottleDumpAxis.SetVelocity(3000, 1, 1);
                await BottleDumpAxis.MoveToAsync(2700);
                //await Task.Delay(1000);
                bool bottleDumpAxisInPosStatus = await Task.Run(async () =>
                    {
                        while (!BottleDumpAxis.IsInposition)
                        {
                            await Task.Delay(1000);
                        }

                        return true;

                    });
                }

                 catch (Exception error)
                {
                    throw error;

                 }


            WriteLog?.Invoke(TodayDateTime + "  " + "傾倒藥罐作業中");

                //BottleDumpAxis.MoveToAsync(DumpModuleParam.BottleDumpTargetPos);
                //WaitInputSignal(medicineBottleMoveCylinderPullSignal);        
                //WaitAxisSignal(BottleDumpAxis.IsInposition);

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
                await Task.Delay(1000);
                injectionCleanSwitch.Switch(false);

                //藥罐清洗作業
                
                await BottleDumpAxis.MoveToAsync(0);
                //await Task.Delay(1000);
                bool bottleDumpAxisInPosStatus = await Task.Run(async () =>
                {
                    while (!BottleDumpAxis.IsInposition)
                    {
                        await Task.Delay(1000);
                    }

                    return true;

                });

                await Task.Delay(1000);
                if (BottleDumpAxis.GetPosition() == 0)
                {
                    medicineBottleMoveCylinder.Switch(true);
                    await Task.Delay(1000);
                    backLightCylinder.Switch(true);
                    await Task.Delay(1000);
                    medicineBottleMoveCylinder.Switch(false);
                    await Task.Delay(1000);
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
        /// 旋緊藥蓋
        /// </summary>
        /// <returns></returns>
        public async Task ScrewMedicineJar()
        {
            WriteLog?.Invoke(TodayDateTime + "  " + "旋緊藥蓋開始");
            try
            {
                //藥罐升降軸鎖瓶蓋預鎖位
                if (BottleElevatorAxis.GetPosition() == 46000)
                {
                    
                    BottleElevatorAxis .SetVelocity(50, 1, 1);
                    await BottleElevatorAxis .MoveToAsync(22000);
                }
                bool bottleElevatorAxisInPosStatus = await Task.Run(async () =>
                {
                    while (!BottleElevatorAxis.IsInposition)
                    {
                        await Task.Delay(1000);
                    }

                    return true;

                });
                WriteLog?.Invoke(TodayDateTime + "  " + "藥罐軸到達預鎖位置");

                //await Task.Delay(500);
                //藥罐升降軸鎖瓶蓋鎖緊位
                if (BottleElevatorAxis.GetPosition() <= 22000)
                {
                    
                    //藥罐升降軸至旋緊位
                    BottleElevatorAxis.SetVelocity(10, 1, 1);
                    await BottleElevatorAxis .MoveToAsync(12500);
                }
                bool bottleElevatorAxisInPosStatus1 = await Task.Run(async () =>
                {
                    while (!BottleElevatorAxis.IsInposition)
                    {
                        await Task.Delay(1000);
                    }

                    return true;

                });

                await Task.Delay(500);
                //藥罐旋轉軸旋緊到位
                await BottleScrewAxis.SetVelocity(300, 1, 1);
                //await Task.Delay(1000);
                await BottleScrewAxis.MoveToAsync(0);
                //await Task.Delay(1000);
                bool bottleScrewInPosStatus = await Task.Run(async () =>
                {
                    while (!BottleScrewAxis.IsInposition)
                    {
                        await Task.Delay(1000);
                    }

                    return true;

                });
                WriteLog?.Invoke(TodayDateTime + "  " + "藥罐旋緊完成");

                //藥罐升降軸待命位
                if (BottleElevatorAxis.GetPosition() == 12500)
                {
                    upperClampMedicineCylinder.Switch(false);
                    await Task.Delay(500);
                    BottleElevatorAxis.SetVelocity(70, 1, 1);
                    await BottleElevatorAxis.MoveToAsync(46000);
                   
                }
                bool bottleElevatorAxisInPosStatus2 = await Task.Run(async () =>
                {
                    while (!BottleElevatorAxis.IsInposition)
                    {
                        await Task.Delay(1000);
                    }

                    return true;

                });
                WriteLog?.Invoke(TodayDateTime + "  " + "藥罐升降軸待命位到位");

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
                await Task.Delay(1000);
                backLightCylinder.Switch(true);

                //WaitInputSignal(medicineBottleMoveCylinderPushSignal);
                //WaitInputSignal(backLightCylinderPushSignal);

                //cameraShot.Switch(true);
                //await Task.Delay(500);
                //cameraShot.Switch(false);

                await Task.Delay(1000);
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
                if (SlideTableAxis.GetPosition() == 234280)//載體盒在紅墨水站
                {
                redInkCylinder.Switch(true);
                await Task.Delay(1000);
                injectRedInk.Switch(true);
                await Task.Delay(300);
                injectRedInk.Switch(false);
                await Task.Delay(1000);
                redInkCylinder.Switch(false);
                 }
          
                 WriteLog?.Invoke(TodayDateTime + "  " + "注入紅墨水完成");

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
                SlideTableAxis.SetVelocity(80, 1, 1);
                SlideTableAxis.MoveToAsync(382670);

                bool slideTableAxisInPosStatus = await Task.Run(async () =>
                {
                    while (!SlideTableAxis.IsInposition)
                    {
                        await Task.Delay(1000);
                    }

                    return true;

                });
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
                 SlideTableAxis.SetVelocity(80, 1, 1);
                 SlideTableAxis.MoveToAsync(234280);
                //SlideTableAxis.MoveToAsync(DumpModuleParam.SlideTableInkPos);
                

            bool slideTableAxisInPosStatus = await Task.Run(async () =>
            {
                while (!SlideTableAxis.IsInposition)
                {
                    await Task.Delay(1000);
                }

                return true;

            });

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



