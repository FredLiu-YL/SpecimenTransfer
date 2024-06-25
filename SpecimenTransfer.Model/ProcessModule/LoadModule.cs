using SpecimenTransfer.Model.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;




namespace SpecimenTransfer.Model
{
    public class LoadModule
    {
        DateTime date = DateTime.Now;
        public string TodayDateTime => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        public string Today => DateTime.Now.ToString("yyyy-MM-dd");


        public event Action<string> WriteLog;
        /// <summary>
        /// 流程動作文字記錄
        /// </summary>

        //Log委派
        public event Action<string> OnProcessCompleted;


        //----Digital Output----

        //camera shot載體盒條碼
        private DigitalOutput shotCarrierBottleBarcode;

        //卡匣推送載體盒汽缸-推收
        private DigitalOutput carrierCassetteCylinder;

        //濾紙盒汽缸-推收
        private DigitalOutput filterPaperBoxCylinder;

        //吸濾紙
        private DigitalOutput suctionFilterPaper;

        //上方夾藥罐氣缸-打開關閉
        //    private DigitalOutput upperClampMedicineCylinder;

        //下方夾藥罐氣缸-打開關閉
        //    private DigitalOutput lowerClampMedicineCylinder;

        //背光氣缸-推收
        private DigitalOutput backLightCylinder;


        //----Digital Input----

        //載體盒 載體氣缸-推
        private DigitalIntput carrierCylinderPushSignal;
        //載體盒 載體氣缸-收
        private DigitalIntput carrierCylinderPullSignal;

        //濾紙盒 -推
        private DigitalIntput filterPaperBoxPushSignal;
        //濾紙盒 -收
        private DigitalIntput filterPaperBoxPullSignal;
       
        //濾紙在席
        private DigitalIntput filterPaperConfirm;


        //背光氣缸-推
        //   private DigitalIntput backLightCylinderPushSignal;
        //背光氣缸-收
        //   private DigitalIntput backLightCylinderPullSignal;


        /// <summary>
        /// 入料模組參數
        /// </summary>
        public IAxis FilterPaperElevatorAxis { get; set; }//濾紙升降軸
        public IAxis SlideTableAxis { get; set; }//載體滑台軸

        //----條碼----
        //載體盒條碼
        private IBarcodeReader carrierBottle;

        public LoadModuleParamer LoadModuleParam { get; set; } = new LoadModuleParamer();
        /// <summary>
        /// IO Input 濾紙盒 推訊號
        /// </summary>
        public DigitalIntput FilterPaperBoxPushSignal { get => filterPaperBoxPushSignal; }
        /// <summary>
        /// IO Input 濾紙盒 收訊號
        /// </summary>
        public DigitalIntput FilterPaperBoxPullSignal { get => filterPaperBoxPullSignal; }


        public LoadModule()
        {

        }

            public LoadModule(DigitalOutput[] signalOutput, DigitalIntput[] signalInput, IAxis slideTableAxis,
            IAxis filterPaperElevatorAxis, IBarcodeReader paperReader)
        {
            //----Digital Output----
            shotCarrierBottleBarcode = signalOutput[0];//camera shot載體盒條碼

            carrierCassetteCylinder = signalOutput[2];//載體盒卡匣

            filterPaperBoxCylinder = signalOutput[3];//濾紙氣缸

            suctionFilterPaper = signalOutput[4];//吸濾紙

            backLightCylinder = signalOutput[10];//背光氣缸

            //----Digital Input----
            carrierCylinderPushSignal = signalInput[6];//載體盒 載體氣缸-推
            carrierCylinderPullSignal = signalInput[7];//載體盒 載體氣缸-收

            filterPaperBoxPushSignal = signalInput[8];//濾紙氣缸-推
            filterPaperBoxPullSignal = signalInput[9];//濾紙氣缸-收
            filterPaperConfirm = signalInput[28];//濾紙在席

            //backLightCylinderPushSignal = signalInput[18];//背光氣缸-推
            //backLightCylinderPushSignal = signalInput[19];//背光氣缸-收

            //----軸控----
            this.FilterPaperElevatorAxis = filterPaperElevatorAxis;//濾紙升降滑台 
            this.SlideTableAxis = slideTableAxis;//載體滑台 

            //----條碼----
            //medcineBottle = paperReader;//藥罐條碼
            carrierBottle = paperReader;//載體盒條碼

        }

        object monitorOBJ = new object();

       

        /// <summary>
        /// 原點復歸
        /// </summary>
        /// <returns></returns>
        public async Task Home()
        {

         //string TodayDateTime = date.ToString("yyyy-MM-dd HH:mm:ss");
         //string Today = date.ToString("yyyy-MM-dd");

            try
            {
                
                WriteLog?.Invoke(TodayDateTime  + "  " + "LoadModule初始化中");

                //濾紙真空關->濾紙升降軸home->濾紙氣缸收->載體盒卡匣收
                double slideAxisPos = SlideTableAxis.GetPosition();

                suctionFilterPaper.Switch(false);
                backLightCylinder.Switch(false);
                carrierCassetteCylinder.Switch(false);

                SlideTableAxis.Home();

                bool slideTableAxisInHomeStatus = await Task.Run(async () =>
                {
                    while (!SlideTableAxis.IsHome)
                    {
                        // 短暫延遲，避免過度佔用 CPU
                        await Task.Delay(1000); // 10 毫秒 (可調整)
                    }
                    return true; // 回 Home 完成

                });
                WriteLog?.Invoke(TodayDateTime + "  " + "載體盒軸原點復歸狀態:" + slideTableAxisInHomeStatus);


                FilterPaperElevatorAxis.SetVelocity(100, 1, 1);
                FilterPaperElevatorAxis.Home();
  
                bool filterAxisInHomeStatus = await Task.Run(async() =>
               {
                   while (!FilterPaperElevatorAxis.IsHome)
                   {
                        // 短暫延遲，避免過度佔用 CPU
                     await Task.Delay(1000); // 10 毫秒 (可調整)
                    }
                   return true; // 回 Home 完成

                });
                WriteLog?.Invoke(TodayDateTime + "  " + "濾紙軸原點復歸狀態:" + filterAxisInHomeStatus);

                if (filterAxisInHomeStatus)
                     await FilterPaperElevatorAxis.MoveToAsync(73000);


                bool filterAxisInpStatus = await Task.Run(async () =>
                {
                    while (!FilterPaperElevatorAxis.IsInposition)
                    {
                        // 短暫延遲，避免過度佔用 CPU
                        await Task.Delay(1000); // 10 毫秒 (可調整)
                    }

                    filterPaperBoxCylinder.Switch(false);
                    return true; // 位置命令 完成

                });
                WriteLog?.Invoke(TodayDateTime + "  " + "濾紙軸絕對位置狀態:" + filterAxisInpStatus);
                WriteLog?.Invoke(TodayDateTime + "  " + "LoadModule初始化完成");


            }
  
            catch (Exception ex)
            {

                throw ex;
            }
          
        }

        /// <summary>
        /// 讀條碼
        /// </summary>
        /// <returns></returns>
        public async Task<string> ReadBarcode()
        {
            string carrierDataReceived;

            try
            {
                WriteLog?.Invoke(TodayDateTime + "  " + "讀取載體盒條碼");
                //await Task.Delay(1000);
                shotCarrierBottleBarcode.Switch(true);
                shotCarrierBottleBarcode.Switch(false);
                carrierDataReceived = carrierBottle.ReceiveData();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally 
            {
                shotCarrierBottleBarcode.Switch(false);
            }

            return carrierDataReceived;
        }

        /// <summary>
        /// 從卡匣進一片載體盒到移載平台
        /// </summary>
        /// <param name="cassetteIndex"></param>
        /// <returns></returns>
        public async Task<int> LoadBoxAsync(int cassetteIndex)
        {
            WriteLog?.Invoke(TodayDateTime + "  " + "載體滑台移動到卡匣站");
            int countCassette = 0;
            //載體滑台移動到卡匣站->載體盒推->載體盒收
            await MoveToCBoxCassette();

            bool slideTableAxisInpStatus = await Task.Run(async () =>
            {
                while (!SlideTableAxis.IsInposition)
                {
                    // 短暫延遲，避免過度佔用 CPU
                    await Task.Delay(1000); // 10 毫秒 (可調整)
                }

                carrierCassetteCylinder.Switch(true);
                await Task.Delay(2000);
                carrierCassetteCylinder.Switch(false);
                WriteLog?.Invoke(TodayDateTime + "  " + "載體盒推入完成");
                await Task.Delay(1000);
                await SlideTableAxis.MoveToAsync(563920);
                countCassette++;

                return true; // 位置命令 完成

            });
            return countCassette;
            WriteLog?.Invoke(TodayDateTime + "  " + "載體滑台移動到讀條碼站");

        }


        //濾紙放到載體盒
        public async Task PuttheFilterpaperInBox()
        {

            WriteLog?.Invoke(TodayDateTime + "  " + "放濾紙開始");

            try
            {
                //濾紙盒推->濾紙升降滑台目標位->吸濾紙->濾紙升降滑台待命位->濾紙盒收 

                bool slideTableAxisInpStatus = await Task.Run(async () =>
                {
                    while (!SlideTableAxis.IsInposition)
                    {
                        // 短暫延遲，避免過度佔用 CPU
                        await Task.Delay(1000); // 10 毫秒 (可調整)
                    }

                    filterPaperBoxCylinder.Switch(true);
                    return true; // 位置命令 完成

                });
                await Task.Delay(500);

                FilterPaperElevatorAxis.MoveToAsync(20000);
                bool filterPaperElevatorAxisInpStatus = await Task.Run(async () =>
                {
                    while (!FilterPaperElevatorAxis.IsInposition)
                    {
                        // 短暫延遲，避免過度佔用 CPU
                        await Task.Delay(1000); // 10 毫秒 (可調整)
                    }
                    suctionFilterPaper.Switch(true);
                    await Task.Delay(1000);

                    return true; // 位置命令 完成

                });

                FilterPaperElevatorAxis.MoveToAsync(73000);
                bool filterPaperElevatorAxisInpStatus1 = await Task.Run(async () =>
                {
                    while (!FilterPaperElevatorAxis.IsInposition)
                    {
                        // 短暫延遲，避免過度佔用 CPU
                        await Task.Delay(1000); // 10 毫秒 (可調整)
                    }
                    filterPaperBoxCylinder.Switch(false);
                    await Task.Delay(500);

                    return true; // 位置命令 完成

                });

                FilterPaperElevatorAxis.MoveToAsync(0);
                bool filterPaperElevatorAxisInpStatus2 = await Task.Run(async () =>
                {
                    while (!FilterPaperElevatorAxis.IsInposition)
                    {
                        // 短暫延遲，避免過度佔用 CPU
                        await Task.Delay(1000); // 10 毫秒 (可調整)
                    }
                    suctionFilterPaper.Switch(false);
                    await Task.Delay(500);

                    return true; // 位置命令 完成

                });

                FilterPaperElevatorAxis.MoveToAsync(73000);
                bool filterPaperElevatorAxisInpStatus3 = await Task.Run(async () =>
                {
                    while (!FilterPaperElevatorAxis.IsInposition)
                    {
                        // 短暫延遲，避免過度佔用 CPU
                        await Task.Delay(1000); // 10 毫秒 (可調整)
                    }
                    WriteLog?.Invoke(TodayDateTime + "  " + "放濾紙完成");

                    return true; // 位置命令 完成

                });

            }
            catch (Exception error)
            {

                error.ToString();
            }

        }

        //載體滑台移動至載體盒站
        public async Task MoveToCBoxCassette()
        {
            
            SlideTableAxis.SetVelocity(80,1,1);
            SlideTableAxis.MoveToAsync(592350);

            bool slideTableAxisInPosStatus = await Task.Run(async () =>
            {
                while (!SlideTableAxis.IsInposition)
                {
                    await Task.Delay(1000);
                }

                return true;

            });


        }

        //載體滑台移動至濾紙站
        public async Task MoveToFilterPaper()
        {
            //SlideTableAxis.MoveAsync(LoadModuleParam.SlideTablePaperPos);
            SlideTableAxis.SetVelocity(90, 1, 1);
            SlideTableAxis.MoveToAsync(521680);

            bool slideTableAxisInPosStatus = await Task.Run(async () =>
            {
                while (!SlideTableAxis.IsInposition)
                {
                    await Task.Delay(1000);
                }

                return true;

            });
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




        internal Task LoadAsync(int v)
        {
            throw new NotImplementedException();
        }

    }

    public class LoadModuleParamer
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
        /// 移載橫移軸 入料位
        /// </summary>
        public double SlideTableLoadPos { get; set; }
        /// <summary>
        /// 移載橫移軸 濾紙位
        /// </summary>
        public double SlideTablePaperPos { get; set; }

        /// <summary>
        /// 濾紙升降軸 Jog移動量
        /// </summary>
        public double FilterPaperElevatorJogDiatance { get; set; }
        /// <summary>
        /// 濾紙升降軸 速度
        /// </summary>
        public double FilterPaperElevatorSpeed { get; set; }

        /// <summary>
        /// 濾紙升降軸 待命位
        /// </summary>
        public double FilterPaperElevatorStandByPos { get; set; }

        /// <summary>
        /// 濾紙升降軸 最高位
        /// </summary>
        public double FilterPaperElevatorHighPos { get; set; }
        /// <summary>
        /// 濾紙升降軸 最低位
        /// </summary>
        public double FilterPaperElevatorLowPos { get; set; }

        /// <summary>
        /// 濾紙升降軸 起始格數
        /// </summary>
        public int FilterPaperElevatorStartIndex { get; set; }

        /// <summary>
        /// 濾紙升降軸 目標位
        /// </summary>
        public double FilterPaperElevatorTargetPos { get; set; }


    }


}
