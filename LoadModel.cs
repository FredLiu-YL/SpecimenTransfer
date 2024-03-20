using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsFormsApp3.Component;

namespace WindowsFormsApp3
{
  
    public class LoadModel
    {
        private IBarcodeReader barcodeReader;
        //檢體盒卡匣 入料升降軸
        private IAxis axisBoxCassetteElevator;
        
        private IElectricCylinder loadPushBoxCylinder;

        private DigitalOutput loadvaccum;
        private DigitalIntput loadvaccumSignal;

        public LoadModel( IBarcodeReader reader, IAxis axisBoxCassetteElevator, IElectricCylinder loadPushBoxCylinder)
        {
            barcodeReader = reader;
            this.axisBoxCassetteElevator = axisBoxCassetteElevator;
            this.loadPushBoxCylinder = loadPushBoxCylinder;

            
                  //loadvaccum = digitals[4];
            //loadvaccumSignal = digitalsIn[3];
            //  loadvaccumSignal = digitals[8];
        }


        public string Run(MachineSetting machineSet , int cassetteIndex)
        {
          


            //卡匣升降移動到準備推出的位置
            double pos = machineSet.BoxCassetteElevatorStartPos + (cassetteIndex - 1) * machineSet.BoxCassetteElevatorPitch;
            axisBoxCassetteElevator.MoveToAsync(pos);

            string barcode = barcodeReader.Read();


            loadPushBoxCylinder.On(); //電動缸推
            Thread.Sleep(200);
            loadPushBoxCylinder.Off(); //電動缸收




            loadvaccum.On();
            while (!loadvaccumSignal.Signal)
            {
                
                Thread.Sleep(100);
            }

            return barcode;
        }


        private void ASD()
        { }


    }
}
