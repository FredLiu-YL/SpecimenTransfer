using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp3.Component;

namespace WindowsFormsApp3.Initial_Model
{
    public class LoadModule
    {
        //抓取濾紙汽缸
        private DigitalOutput catchFilterPaperCylinder;

        //抓取罐子夾爪汽缸
        private DigitalOutput jarClampCylinder;

        //檢體盒 壓蓋汽缸
        private DigitalOutput closeBoxCoverCylinder;

        //抓取濾紙汽缸到位訊號-推
        private DigitalIntput catchFilterPaperInPush;
        //抓取濾紙汽缸到位訊號-收
        private DigitalIntput catchFilterPaperInPull;

        private IBarcodeReader barcodeReader;
        private IAxis cassetteElevatorAxis;

        public LoadModule(IBarcodeReader barcodeReader ,IAxis cassetteElevatorAxis, DigitalOutput[] signalOutput , DigitalIntput[] signalInput )
        {
            this.barcodeReader = barcodeReader;
            this.cassetteElevatorAxis = cassetteElevatorAxis;

            catchFilterPaperCylinder = signalOutput[0];
            jarClampCylinder = signalOutput[1];
            closeBoxCoverCylinder = signalOutput[2];

            catchFilterPaperInPush = signalInput[20];
            catchFilterPaperInPull = signalInput[21];
            catchFilterPaperInPush = signalInput[20];
            catchFilterPaperInPull = signalInput[21];


         
        }

        public void Home()
        {


        }



        public void LoadAsync(int cassetteId)
        {

            cassetteElevatorAxis.MoveToAsync(0);
        }


        public string ReadBarcode()
        {
           string data = barcodeReader.ReceiveData();
            return data;
        }
    }
}
