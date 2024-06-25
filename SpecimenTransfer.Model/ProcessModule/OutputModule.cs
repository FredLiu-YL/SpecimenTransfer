using SpecimenTransfer.Model.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SpecimenTransfer;
using System.IO;
using SpecimenTransfer;


namespace SpecimenTransfer.Model
{
    public class OutputModule
    {

        DateTime date = DateTime.Now;
        public string TodayDateTime => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        public string Today => DateTime.Now.ToString("yyyy-MM-dd");

        public event Action<string> WriteLog;
        //Log委派
        public event Action<string> OnProcessCompleted;

        //----Digital Output----
        private DigitalOutput pressDownCoverCylinder;//壓蓋氣缸
        private DigitalOutput pushCoverCylinder;//推蓋氣缸
        private DigitalOutput storageCylinder;//收納氣缸

        //----Digital Iutput----
        private DigitalIntput pressDownCoverCylinderPushSignal;//壓蓋氣缸-推
        private DigitalIntput pressDownCoverCylinderPullSignal;//壓蓋氣缸-收
        private DigitalIntput pushCoverCylinderPushSignal;//推蓋氣缸-推
        private DigitalIntput pushCoverCylinderPullSignal;//推蓋氣缸-收
        private DigitalIntput storageCylinderCylinderPushSignal;//收納氣缸-推
        private DigitalIntput storageCylinderCylinderPullSignal;//收納氣缸-收

        //軸控
        //載體滑台
        public IAxis SlideTableAxis { get; set; }
        //蓋子及收納升降滑台
        public IAxis CoverAndStorageElevatorAxis { get; set; }

        public Action LoadCarrierBox;
        object monitorOBJ = new object();

        /// <summary>
        /// 輸出模組參數
        /// </summary>
        public OutputModuleParamer OutputModuleParam { get; set; } = new OutputModuleParamer();

        public OutputModule()
        {

        }

        public OutputModule(DigitalOutput[] signalOutput, DigitalIntput[] signalInput,
                            IAxis slideTableAxis, IAxis coverAndStorageElevatorAxis)
            
        {

            //----Digital Output----
            pressDownCoverCylinder = signalOutput[13];//壓蓋氣缸
            pushCoverCylinder = signalOutput[14]; ;//推蓋氣缸
            storageCylinder = signalOutput[15]; ;//收納氣缸

            //----Digital Input----
            pressDownCoverCylinderPushSignal = signalInput[22];//壓蓋氣缸-推
            pressDownCoverCylinderPullSignal = signalInput[23];//壓蓋氣缸-收
            pushCoverCylinderPushSignal = signalInput[24];//推蓋氣缸-推
            pushCoverCylinderPullSignal = signalInput[25];//推蓋氣缸-收
            storageCylinderCylinderPushSignal = signalInput[26];//收納氣缸-推
            storageCylinderCylinderPullSignal = signalInput[27]; ;//收納氣缸-收

            //----軸控----
            this.CoverAndStorageElevatorAxis = coverAndStorageElevatorAxis;//蓋子及收納升降滑台
            this.SlideTableAxis = slideTableAxis;//載體滑台
        }

        int loadCarrierCount;
        //人工放載體盒
        public async Task LoadCarrier()
        {
            WriteLog?.Invoke(TodayDateTime + "  " + "cassete上蓋料件計數");

            loadCarrierCount++;
            if(loadCarrierCount > 10)
            LoadCarrierBox.Invoke();//cassete料件已空，目前由人完成載體盒的放入先委派出去

            //WriteLog?.Invoke(TodayDateTime + "  " + "cassete上蓋料件已空，需要人工補料");

        }

        /// <summary>
        /// Home
        /// </summary>
        /// <returns></returns>
        public async Task Home()
        {
                
                WriteLog?.Invoke(TodayDateTime + "  " + "OutputModule初始化中");

                //收納及推蓋軸原點復歸
                await CoverAndStorageElevatorAxis.Home();
                bool coverAndStorageElevatorAxisInHomeStatus = await Task.Run(async () =>
                {
                while (!CoverAndStorageElevatorAxis.IsHome)
                {
                    // 短暫延遲，避免過度佔用 CPU
                    await Task.Delay(1000); // 10 毫秒 (可調整)
                }
                return true; // 回 Home 完成

                 });
                 WriteLog?.Invoke(TodayDateTime + "  " + "收納及推蓋軸原點復歸狀態:" + coverAndStorageElevatorAxisInHomeStatus);

                 pushCoverCylinder.Switch(false);

                 pressDownCoverCylinder.Switch(false);

                 storageCylinder.Switch(false);

                //WaitInputSignal(pushCoverCylinderPullSignal);
                //WaitAxisSignal(CoverAndStorageElevatorAxis.IsInposition);
                //WaitInputSignal(pressDownCoverCylinderPullSignal);
                WriteLog?.Invoke(TodayDateTime + "  " + "OutputModule初始化完成");
 
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

        int count = 1;
        double startPos = 90550;
        double Spacing = -4420;
        int layers = 0;
        double targetPos;
        /// <summary>
        /// 推蓋子
        /// </summary>
        /// <returns></returns>
        public async Task LoadCoverAsync()
        {
            WriteLog?.Invoke(TodayDateTime + "  " + "上蓋Cassete移載，放蓋開始");
            //推蓋階層運算
            if (layers >= 0 && layers <= 9 )
            {
                //推蓋軸位置 = 起始點+間距*階層(第一層為0以此類推)
                
                targetPos = startPos + Spacing * layers;
                CoverAndStorageElevatorAxis.MoveToAsync(targetPos);

                bool CoverAndStorageElevatorAxisInPosStatus = await Task.Run(async () =>
                {
                    while (!CoverAndStorageElevatorAxis.IsInposition)
                    {
                        await Task.Delay(1000);
                    }
                    WriteLog?.Invoke(TodayDateTime + "  " + "推蓋階層移動完成");
                    return true;

                });

                layers += 1;
                
               if (CoverAndStorageElevatorAxis.GetPosition() == targetPos)
 
                {
                   
                    pushCoverCylinder.Switch(true);
                    await Task.Delay(1000);
                    pushCoverCylinder.Switch(false);
                }
                WriteLog?.Invoke(TodayDateTime + "  " + "上蓋Cassete移載，放蓋結束");
            }

            else if (layers >= 10)
            {
                CoverAndStorageElevatorAxis.Home();
                 bool CoverAndStorageElevatorAxisInPosStatus = await Task.Run(async () =>
                {
                    while (!CoverAndStorageElevatorAxis.IsHome)
                    {
                        await Task.Delay(1000);
                    }

                    return true;

                });
                WriteLog?.Invoke(TodayDateTime + "  " + "上蓋Cassete移載，回到階層1的位置");

            }

            //推蓋階層運算
            //double startPos;
            //double Spacing;
            //int layers;
            //double targetPos;

            //startPos = OutputModuleParam.CoverStartPos;
            //Spacing = OutputModuleParam.CoverSpacing;
            //layers = OutputModuleParam.CoverTargetIndex;

            //startPos = 82970;
            //Spacing = 600;
            //layers = 2;

        }
        /// <summary>
        /// 下壓蓋子
        /// </summary>
        /// <returns></returns>
        public async Task PressDownCoverAsync()
        {
            WriteLog?.Invoke(TodayDateTime + "  " + "壓蓋開始");
            try
            {
                
                if (SlideTableAxis.GetPosition() == 188970)
                {

                    pressDownCoverCylinder.Switch(true);
                    await Task.Delay(1000);
                    pressDownCoverCylinder.Switch(false);

                }
                WriteLog?.Invoke(TodayDateTime + "  " + "壓蓋結束");
                //WaitInputSignal(SlideTableAxis.IsInposition);
                //WaitInputSignal(pressDownCoverCylinderPushSignal);
                //WaitInputSignal(pressDownCoverCylinderPullSignal);
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        int UnloadCount = 1;
        double UnloadStartPos = 81990;
        double UnloadSpacing = -8670;
        int UnloadLayers = 0;       
        double UnloadTargetPos;
        /// <summary>
        /// 載體盒做完收納
        /// </summary>
        /// <returns></returns>
        public async Task UnLoadBoxAsync()
        {
            WriteLog?.Invoke(TodayDateTime + "  " + "載體盒cassete收納開始");
            //收納階層運算
            if (UnloadLayers >= 0 && UnloadLayers <= 9)
            {
                //收納軸位置 = 起始點+間距*階層(第一層為0以此類推)

                UnloadTargetPos = UnloadStartPos + UnloadSpacing * UnloadLayers;
                CoverAndStorageElevatorAxis.MoveToAsync(UnloadTargetPos);
                bool coverAndStorageElevatorAxisInpStatus = await Task.Run(async () =>
                {
                    while (!CoverAndStorageElevatorAxis.IsInposition)
                    {
                        // 短暫延遲，避免過度佔用 CPU
                        await Task.Delay(1000); // 10 毫秒 (可調整)
                    }
                    WriteLog?.Invoke(TodayDateTime + "  " + "載體盒收納階層移動完成");

                    return true; // 位置命令 完成

                });
                UnloadLayers += 1;

                if(CoverAndStorageElevatorAxis.GetPosition() == UnloadTargetPos)
                {
                    
                    storageCylinder.Switch(true);
                    await Task.Delay(1000); 
                    storageCylinder.Switch(false);
                }

                WriteLog?.Invoke(TodayDateTime + "  " + "載體盒cassete收納完成");

            }
            
            else if (UnloadLayers >= 10)
            {
                CoverAndStorageElevatorAxis.Home();
                await Task.Delay(3000);
                WriteLog?.Invoke(TodayDateTime + "  " + "收納cassete回到階層1的位置");

            }

            //CoverAndStorageElevatorAxis.MoveToAsync(OutputModuleParam.SlideTableCoverPos);
            //await CarrierMoveToStorage();
        }

        /// <summary>
        /// 蓋子升降
        /// </summary>
        /// <returns></returns>
        public async Task CarrierMoveToFilterPaper()
        {
            try
            {

                //CoverAndStorageElevatorAxis.MoveAsync(OutputModuleParam.axisCoverAndStorageElevatorPos); //參數改變
                pushCoverCylinder.Switch(true);
                await Task.Delay(2000);
                pushCoverCylinder.Switch(false);

            }

            catch (Exception ex)
            {
                throw ex;
            }

        }


        /// <summary>
        /// 收納盒升降
        /// </summary>
        /// <returns></returns>
        public async Task StorageBoxElevator()
        {

            //CoverAndStorageElevatorAxis.MoveAsync(OutputModuleParam.axisCoverAndStorageElevatorPos); //參數改變
            storageCylinder.Switch(true);
            WaitInputSignal(storageCylinderCylinderPushSignal);
            storageCylinder.Switch(false);
        }

        /// <summary>
        /// 收納及推蓋站原點復歸
        /// </summary>
        /// <returns></returns>
        public async Task CoverAndStorageElevatorHome()
        {
            try
            {
                //推蓋氣缸收->收納氣缸收->收納及推蓋站原點復歸
                pushCoverCylinder.Switch(false);
                storageCylinder.Switch(false);
                CoverAndStorageElevatorAxis.Home();
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 載體盒移動至推蓋站
        /// </summary>
        /// <returns></returns>
        public async Task CarrierMoveToPushCover()
        {
            try
            {
                //載體滑台移動至推蓋站
                //SlideTableAxis.MoveToAsync(OutputModuleParam.SlideTableCoverPos);
                SlideTableAxis.SetVelocity(90, 1, 1);
                SlideTableAxis.MoveToAsync(130880);
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
        //載體盒移動至壓蓋站
        public async Task CarrierMoveToPressDownCover()
        {
            try
            {
                //載體滑台移動至壓蓋站
                //SlideTableAxis.MoveToAsync(OutputModuleParam.SlideTableGlandPos);
                SlideTableAxis.SetVelocity(80, 1, 1);
                SlideTableAxis.MoveToAsync(188970);
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
        //載體盒移動至收納站
        public async Task CarrierMoveToStorage()
        {
            try
            {
                //載體滑台移動至收納站
                //SlideTableAxis.MoveToAsync(OutputModuleParam.SlideTableOutputPos);
                SlideTableAxis.SetVelocity(80, 1, 1);
                SlideTableAxis.MoveToAsync(8460);
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

        public class OutputModuleParamer
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
            /// 移載橫移軸 壓蓋位
            /// </summary>
            public double SlideTableGlandPos { get; set; }
            /// <summary>
            /// 移載橫移軸 放蓋位
            /// </summary>
            public double SlideTableCoverPos { get; set; }
            /// <summary>
            /// 移載橫移軸 出料位
            /// </summary>
            public double SlideTableOutputPos { get; set; }

            /// <summary>
            /// 放蓋及收納升降軸 Jog移動量
            /// </summary>
            public double CoverAndStorageElevatorJogDiatance { get; set; }
            /// <summary>
            /// 放蓋及收納升降軸 移動速度
            /// </summary>
            public double CoverAndStorageElevatorSpeed { get; set; }

            /// <summary>
            /// 放蓋及收納升降軸 待命位
            /// </summary>
            public double CoverAndStorageElevatorStandByPos { get; set; }

            /// <summary>
            /// 放蓋及收納升降軸 放蓋起點位
            /// </summary>
            public double CoverStartPos { get; set; }
            /// <summary>
            /// 放蓋及收納升降軸 放蓋間距
            /// </summary>
            public double CoverSpacing { get; set; }
            /// <summary>
            /// 放蓋及收納升降軸 放蓋目標格數
            /// </summary>
            public int CoverTargetIndex { get; set; }

            /// <summary>
            /// 放蓋及收納升降軸 收納起點位
            /// </summary>
            public double StorageStartPos { get; set; }
            /// <summary>
            /// 放蓋及收納升降軸 收納間距
            /// </summary>
            public double StorageSpacing { get; set; }
            /// <summary>
            /// 放蓋及收納升降軸 收納目標格數
            /// </summary>
            public int StorageTargetIndex { get; set; }

        }


    }

}
