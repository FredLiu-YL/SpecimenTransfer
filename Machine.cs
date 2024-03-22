using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp3.Component;
using WindowsFormsApp3.Initial_Model;

namespace WindowsFormsApp3
{
    public partial class Machine
    {
   
   
        //開蓋旋轉軸
        private IAxis axisTurnLid;
        //傾倒藥罐軸
        private IAxis axisDump;

        private IDigitalSignalController digitaiOutput;

        private OrientAxis orientalmotor;

        //檢體盒卡匣 入料升降軸
        //private IAxis axisBoxCassetteElevator;
        // private IBarcodeReader barcodeReader;
        // private IElectricCylinder loadPushBoxCylinder;
       
        private LoadModel loadModel;

      

        //Barcode reader

 

        public MachineSetting MachineSet { get; set; }
        public LoadModule LoadModle { get; set; }
        public DumpModule DumpModle { get; set; }
        public OutputModule OutputModle { get; set; }



        public  void Initial()
        {

            //軸控
            IAxis axisTransfer = new ToyoAxis("COM4");
            axisTurnLid = new OrientAxis("COM3");
            axisDump = new ToyoAxis("COM5");
            IAxis axisBoxCassetteElevator = new ToyoAxis("COM11");
            IElectricCylinder loadPushBoxCylinder = new ToyoCylinder("COM13");

            //reader
            IBarcodeReader boxReader = new BoxReader("192.168.100.80", 9004);
            IBarcodeReader medcineBottleReader = new MedcineBottleReader("192.168.100.81", 9005);

            //module
            loadModel = new LoadModel(boxReader, axisBoxCassetteElevator, loadPushBoxCylinder);


            IDigitalSignalController digitalController1 = new ADTech_USB4750(1);// 0-15
           // IDigitalSignalController digitalController2 = new ADTech_USB4750(2);// 0-15


           // DigitalOutput[] outputarr = digitalController1.SignalOutput.Concat(digitalController2.SignalOutput).ToArray();//0-15 ,16-31  

            LoadModle = new LoadModule(boxReader, axisBoxCassetteElevator, digitalController1.SignalOutput, digitalController1.SignalInput);
            DumpModle = new DumpModule(digitalController1.SignalOutput, digitalController1.SignalInput);
            OutputModle = new OutputModule();

        }

        

        public void Home()
        {
            DumpModle.PreHome();
            LoadModle.Home();

            DumpModle.Home();
        }


      
    }
}
