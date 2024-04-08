using SpecimenTransfer.Model.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


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
        public Action SetupJar;
        //載體滑台
        public IAxis AxisCarrier { get; set; }
        //蓋子及收納升降滑台
        public IAxis AxisCoverAndStorageElevator { get; set; }

        /// <summary>
        /// 輸出模組參數
        /// </summary>
        public OutputModuleParamer OutputModuleParam { get; set; } = new OutputModuleParamer();
        public OutputModule(DigitalOutput[] signalOutput, DigitalIntput[] signalInput,
                            IAxis axisCoverElevator, IAxis axisCarrierSlideTable)

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
            AxisCoverAndStorageElevator = axisCoverElevator;//蓋子及收納升降滑台
            AxisCarrier = axisCarrierSlideTable;//載體滑台
        }

        public async Task HomeAsync()
        {
            //推蓋氣缸收->蓋子及收納升降滑台home->壓蓋氣缸收
            pushCoverCylinder.Switch(false);
            AxisCoverAndStorageElevator.Home();
            pressDownCoverCylinder.Switch(false);

        }
        /// <summary>
        /// 取蓋子
        /// </summary>
        /// <returns></returns>
        public async Task LoadCoverAsync()
        {
            //蓋子及收納升降滑台->載體盒推蓋站到位->推蓋氣缸推->
            AxisCoverAndStorageElevator.MoveAsync(OutputModuleParam.axisCarrierMoveToPushCoverPos);
            await CarrierMoveToPushCover();
            WaitInputSignal(AxisCarrier.IsInposition);
            pushCoverCylinder.Switch(true);
            await Task.Delay(1000);
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
                //載體盒下壓站到位->壓蓋氣缸
                await CarrierMoveToPressDownCover();
                WaitInputSignal(AxisCarrier.IsInposition);
                pressDownCoverCylinder.Switch(true);
                WaitInputSignal(pressDownCoverCylinderPushSignal);
                pressDownCoverCylinder.Switch(false);

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
            AxisCoverAndStorageElevator.MoveAsync(OutputModuleParam.axisCarrierMoveToPushCoverPos);
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
                WaitInputSignal(AxisCoverAndStorageElevator.IsInposition);
                AxisCoverAndStorageElevator.MoveAsync(OutputModuleParam.axisCoverAndStorageElevatorPos);
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
            WaitInputSignal(AxisCarrier.IsInposition);
            AxisCoverAndStorageElevator.MoveAsync(OutputModuleParam.axisCoverAndStorageElevatorPos);
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
                AxisCoverAndStorageElevator.Home();
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
                AxisCarrier.MoveAsync(OutputModuleParam.axisCarrierMoveToPushCoverPos);
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
                AxisCarrier.MoveAsync(OutputModuleParam.axisCarrierMoveToPressDownCoverPos);
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
                AxisCarrier.MoveAsync(OutputModuleParam.axisCarrierMoveToStoragePos);
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public class OutputModuleParamer
        {
            //蓋子及收納升降滑台-home
            public double axisCoverAndStorageElevatorHomePos { get; set; }
            //蓋子及收納升降滑台-待命
            public double axisCoverAndStorageElevatorStandByPos { get; set; }
            //蓋子及收納升降滑台-位置
            public double axisCoverAndStorageElevatorPos { get; set; }

            //載體滑台-home
            public double axisCarrierTableHomePos { get; set; }
            //載體滑台移動至壓蓋站-位置
            public double axisCarrierMoveToPressDownCoverPos { get; set; }
            //載體滑台移動至推蓋站-位置
            public double axisCarrierMoveToPushCoverPos { get; set; }
            //載體滑台移動至收納站-位置
            public double axisCarrierMoveToStoragePos { get; set; }

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
