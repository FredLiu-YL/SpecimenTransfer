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
        private IAxis axisCoverAndStorageElevatorStandBy;//蓋子及收納升降滑台-待命
        private IAxis axisCoverAndStorageElevatorPostion;//蓋子及收納升降滑台-位置
        private IAxis axisCoverAndStorageElevatorReady;//蓋子及收納升降滑台-ready

        private IAxis axisCarrierSlideStandBy;//載體滑台-待命
        private IAxis axisCarrierSlidePostion; //載體滑台-位置
        private IAxis axisCarrierSlideReady;//載體滑台到位-ready


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
            axisCoverAndStorageElevatorStandBy = axisCoverElevator;//蓋子及收納升降滑台-待命
            axisCoverAndStorageElevatorPostion = axisCoverElevator;//蓋子及收納升降滑台-位置
            axisCoverAndStorageElevatorReady = axisCoverElevator;//蓋子及收納升降滑台-ready

            axisCarrierSlideStandBy = axisCarrierSlideTable;//載體滑台-待命
            axisCarrierSlidePostion = axisCarrierSlideTable;//載體滑台-位置
            axisCarrierSlideReady = axisCarrierSlideTable;//載體滑台到位-ready

        }

        //蓋子升降
        public async Task CarrierMoveToFilterPaper()
        {
            try
            {
                //載體盒蓋子站到位->蓋子升降->推出蓋子
                WaitInputSignal(axisCarrierSlideReady.IsInposition);
                axisCoverAndStorageElevatorPostion.MoveAsync(OutputModuleParam.axisCoverAndStorageElevatorPos);
                pushCoverCylinder.Switch(true);
                await Task.Delay(2000);
                pushCoverCylinder.Switch(false);

            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        //下壓蓋子
        public async Task PressDownCover()
        {
            try
            {
                //載體盒下壓站到位->壓蓋氣缸
                WaitInputSignal(axisCarrierSlideReady.IsInposition);
                pressDownCoverCylinder.Switch(true);
                WaitInputSignal(pressDownCoverCylinderPushSignal);
                pressDownCoverCylinder.Switch(false);

            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        //收納盒升降
        public async Task StorageBoxElevator()
        {
            //載體盒收納站到位->收納盒升降->收納氣缸
            WaitInputSignal(axisCarrierSlideReady.IsInposition);
            axisCoverAndStorageElevatorPostion.MoveAsync(OutputModuleParam.axisCoverAndStorageElevatorPos);
            storageCylinder.Switch(true);
            WaitInputSignal(storageCylinderCylinderPushSignal);
            storageCylinder.Switch(false);
        }
        //載體盒移動至推蓋站
        public async Task CarrierMoveToPushCover()
        {
            try
            {
                //載體滑台移動至推蓋站
                axisCarrierSlidePostion.MoveAsync(6000);
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
                axisCarrierSlidePostion.MoveAsync(6000);
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
                axisCarrierSlidePostion.MoveAsync(6000);
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        //收納及推蓋站原點復歸
        public async Task Home()
        {


        }

        public class OutputModuleParamer
        {
            
            public double axisCoverAndStorageElevatorStandByPos { get; set; }//蓋子及收納升降滑台-待命
            public double axisCoverAndStorageElevatorPos { get; set; }//蓋子及收納升降滑台-位置

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
