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
        private DigitalOutput storageCylinder;//收納氣缸

        //----Digital Iutput----
        private DigitalIntput pressDownCoverCylinderPushSignal;//壓蓋氣缸-推
        private DigitalIntput pressDownCoverCylinderPullSignal;//壓蓋氣缸-收
        private DigitalIntput storageCylinderCylinderPushSignal;//收納氣缸-推
        private DigitalIntput storageCylinderCylinderPullSignal;//收納氣缸-收

        //軸控
        private IAxis axisCoverElevatorStandBy;//蓋子升降滑台-待命
        private IAxis axisCoverElevatorPostion;//蓋子升降滑台-位置
        private IAxis axisCoverElevatorReady;//蓋子升降滑台-ready




        public OutputModule(DigitalOutput[] signalOutput, DigitalIntput[] signalInput, 
                            IAxis axisCoverElevator, IAxis axisCarrierSlideTable)

        {

            //----Digital Output----
            pressDownCoverCylinder = signalOutput[14];//壓蓋氣缸
            storageCylinder = signalOutput[15]; ;//收納氣缸

            //----Digital Input----
            pressDownCoverCylinderPushSignal = signalInput[22];//壓蓋氣缸-推
            pressDownCoverCylinderPullSignal = signalInput[23];//壓蓋氣缸-收
            storageCylinderCylinderPushSignal = signalInput[24];//收納氣缸-推
            storageCylinderCylinderPullSignal = signalInput[25]; ;//收納氣缸-收

            //----軸控----
            axisCoverElevatorStandBy = axisCoverElevator;//蓋子升降滑台-待命
            axisCoverElevatorPostion = axisCoverElevator;//蓋子升降滑台-位置
            axisCoverElevatorReady = axisCoverElevator;//蓋子升降滑台-ready


        }

        //推蓋升降
        public async Task carrierMoveToFilterPaper()
        {
        }
        //收納升降



        public class OutputModuleParamer
        {
            
            public double axisCoverElevatorStandByPos { get; set; }//蓋子升降滑台-待命
            public double axisCoverElevatorPos { get; set; }//蓋子升降滑台-位置
            

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
