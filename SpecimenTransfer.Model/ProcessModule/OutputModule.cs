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
            CoverAndStorageElevatorAxis = coverAndStorageElevatorAxis;//蓋子及收納升降滑台
            SlideTableAxis = slideTableAxis;//載體滑台
        }

        /// <summary>
        /// Home
        /// </summary>
        /// <returns></returns>
        public async Task Home()
        {
            //推蓋氣缸收->蓋子及收納升降滑台home->壓蓋氣缸收
            pushCoverCylinder.Switch(false);
            WaitInputSignal(pushCoverCylinderPullSignal);

            CoverAndStorageElevatorAxis.Home();
            WaitAxisSignal(CoverAndStorageElevatorAxis.IsInposition);
            
            pressDownCoverCylinder.Switch(false);
            WaitInputSignal(pressDownCoverCylinderPullSignal);

        }
        
        /// <summary>
        /// 推蓋子
        /// </summary>
        /// <returns></returns>
        public async Task LoadCoverAsync()
        {
            //推蓋階層運算
            double startPos;
            double Spacing;
            int layers;
            double targetPos;

            startPos = OutputModuleParam.CoverStartPos;
            Spacing = OutputModuleParam.CoverSpacing;
            layers = OutputModuleParam.CoverTargetIndex;

            //推蓋軸位置 = 起始點+間距*階層(第一層為0以此類推)
            targetPos = startPos + Spacing * layers;

            //等待載體滑台到位->蓋子及收納升降滑台->推蓋氣缸推
            await CarrierMoveToPushCover();

            CoverAndStorageElevatorAxis.MoveToAsync(targetPos);
            WaitAxisSignal(CoverAndStorageElevatorAxis.IsInposition);

            pushCoverCylinder.Switch(true);
            WaitInputSignal(pushCoverCylinderPushSignal);
            pushCoverCylinder.Switch(false);

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
                await CarrierMoveToPressDownCover();
                WaitInputSignal(SlideTableAxis.IsInposition);

                pressDownCoverCylinder.Switch(true);
                WaitInputSignal(pressDownCoverCylinderPushSignal);
                await Task.Delay(1000);
                pressDownCoverCylinder.Switch(false);
                WaitInputSignal(pressDownCoverCylinderPullSignal);
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// 載體盒做完收納
        /// </summary>
        /// <returns></returns>
        public async Task UnLoadBoxAsync(int cassetteIndex)
        {
            //蓋子及收納升降滑台->載體盒收納站到位->收納氣缸
            CoverAndStorageElevatorAxis.MoveAsync(OutputModuleParam.SlideTableCoverPos);
            await CarrierMoveToStorage();
            storageCylinder.Switch(true);
            await Task.Delay(1000);
            storageCylinder.Switch(false);

        }

        /// <summary>
        /// 蓋子升降
        /// </summary>
        /// <returns></returns>
        public async Task CarrierMoveToFilterPaper()
        {
            try
            {
                //載體盒蓋子站到位->蓋子升降->推出蓋子
                WaitInputSignal(CoverAndStorageElevatorAxis.IsInposition);
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
            //載體盒收納站到位->收納盒升降->收納氣缸
            WaitInputSignal(SlideTableAxis.IsInposition);
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
                SlideTableAxis.MoveAsync(OutputModuleParam.SlideTableCoverPos);
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
                SlideTableAxis.MoveAsync(OutputModuleParam.SlideTableGlandPos);
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
                SlideTableAxis.MoveAsync(OutputModuleParam.SlideTableOutputPos);
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


        private void WaitInputSignal(bool isInposition)
        {
            throw new NotImplementedException();
        }

    }

}
