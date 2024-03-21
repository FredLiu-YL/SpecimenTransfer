using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp3
{
    public partial class Machine
    {

        /*
        public string Load(int cassetteIndex)
        {
            try
            {
                //橫移軸移動到入料位置
                //axisTransfer.MoveToAsync(MachineSet.TransferLoadPos);
                //string barcode = loadModel.Run(MachineSet, cassetteIndex);

                /* //橫移軸移動到入料位置
                 axisTransfer.MoveToAsync(MachineSet.TransferLoadPos);

                 //卡匣升降移動到準備推出的位置
                 double pos = MachineSet.BoxCassetteElevatorStartPos + (CassetteIndex - 1) * MachineSet.BoxCassetteElevatorPitch;
                 axisBoxCassetteElevator.MoveToAsync(pos);

                 string barcode = barcodeReader.Read();


                 loadPushBoxCylinder.On(); //電動缸推
                 Thread.Sleep(200);
                 loadPushBoxCylinder.Off(); //電動缸收
                
                //return barcode;
            }
            catch (Exception ex)
            {

                throw ex;
            }
          
        }
        */
                  
        public void PutFilterPaper()
        {

        }
        public void DumpSpecimen()
        {
            /*
            //橫移軸移動到傾倒位置
            axisTransfer.MoveToAsync(MachineSet.TransferDumpPos);

            JarClampCylinder.On();

            //轉開蓋子
            axisTurnLid.MoveToAsync(MachineSet.DumpTurnOnPos);
            //倒出檢體
            axisDump.MoveToAsync(MachineSet.DumpPourOutPos);
            */
        }
        public void ConfirmClear()
        {

        }

        public void UnLoad()
        {


        }
    }
}
