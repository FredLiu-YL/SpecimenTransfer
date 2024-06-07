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

        /// <summary>
        /// 輸出模組參數
        /// </summary>
        public OutputModuleParamer OutputModuleParam { get; set; } = new OutputModuleParamer();
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
            loadCarrierCount++;
            if(loadCarrierCount > 10)
            LoadCarrierBox.Invoke();//cassete料件已空，目前由人完成載體盒的放入先委派出去

        }

        /// <summary>
        /// Home
        /// </summary>
        /// <returns></returns>
        public async Task Home()
        {
            //推蓋氣缸收->蓋子及收納升降滑台home->壓蓋氣缸收

            CoverAndStorageElevatorAxis.Home();

            //double nowPos = CoverAndStorageElevatorAxis.GetPosition();
            //if (nowPos == 0)
            //CoverAndStorageElevatorAxis.MoveToAsync(89250);

            pushCoverCylinder.Switch(false);
   
            pressDownCoverCylinder.Switch(false);

            storageCylinder.Switch(false);

            //WaitInputSignal(pushCoverCylinderPullSignal);
            //WaitAxisSignal(CoverAndStorageElevatorAxis.IsInposition);
            //WaitInputSignal(pressDownCoverCylinderPullSignal);

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
            //推蓋階層運算
            if (layers >= 0 && layers <= 9 )
            {
                //推蓋軸位置 = 起始點+間距*階層(第一層為0以此類推)
                
                targetPos = startPos + Spacing * layers;
                CoverAndStorageElevatorAxis.MoveToAsync(targetPos);
                layers += 1;
                await Task.Delay(3000);
                //濾紙氣缸推
                double nowPos = CoverAndStorageElevatorAxis.GetPosition();
                //await Task.Delay(2000);

                if (nowPos == targetPos)
 
                {
                    await Task.Delay(1000); ;
                    pushCoverCylinder.Switch(true);
                    await Task.Delay(2000);
                    pushCoverCylinder.Switch(false);
                }
            }

            else if (layers >= 10)
            {
                CoverAndStorageElevatorAxis.Home();
                Thread.Sleep(3000);

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
            try
            {
                //待載體盒下壓站到位->壓蓋氣缸推->壓蓋氣缸收

                await Task.Delay(1000);
                double nowPos = SlideTableAxis.GetPosition();
                if (nowPos == 188970)
                {

                    pressDownCoverCylinder.Switch(true);
                    await Task.Delay(2000);
                    pressDownCoverCylinder.Switch(false);

                }

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


            //收納階層運算
            if (UnloadLayers >= 0 && UnloadLayers <= 9)
            {
                //收納軸位置 = 起始點+間距*階層(第一層為0以此類推)

                UnloadTargetPos = UnloadStartPos + UnloadSpacing * UnloadLayers;
                CoverAndStorageElevatorAxis.MoveToAsync(UnloadTargetPos);
                UnloadLayers += 1;
                await Task.Delay(2000);

                double nowPos = CoverAndStorageElevatorAxis.GetPosition();
                if(nowPos == UnloadTargetPos)
                {
                    await Task.Delay(1000);
                    storageCylinder.Switch(true);
                    await Task.Delay(3000); 
                    storageCylinder.Switch(false);
                }

            }
            
            else if (UnloadLayers >= 10)
            {
                CoverAndStorageElevatorAxis.Home();
                await Task.Delay(3000);

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
                SlideTableAxis.MoveToAsync(130880);
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
                SlideTableAxis.MoveToAsync(188970);
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
                SlideTableAxis.MoveToAsync(8460);
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
