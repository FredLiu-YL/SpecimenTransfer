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

        private IDigitalSignalController digitaiOutput;

        private OrientAxis orientalmotor;

        //檢體盒卡匣 入料升降軸
        //private IAxis axisBoxCassetteElevator;
        // private IBarcodeReader barcodeReader;
        // private IElectricCylinder loadPushBoxCylinder;

     
        public MachineSetting MachineSet { get; set; }
        public LoadModule LoadModle { get; set; }
        public DumpModule DumpModle { get; set; }
        public OutputModule OutputModle { get; set; }



        public void Initial(bool isSimulate)
        {
            //橫向搬送軸
            IAxis axisTransfer =null;
            //開蓋旋轉軸
            IAxis axisTurnLid = null;
            //傾倒藥罐軸
            IAxis axisDump = null;
            //入料 載體盒卡匣 升降軸
            IAxis axisBoxCassetteElevator = null;

            IElectricCylinder loadPushBoxCylinder = null;
            IBarcodeReader boxReader = null;
            IDigitalSignalController digitalController1 = null;

            if (isSimulate)
            {
                axisTransfer = new DummyAxis( );
                axisTurnLid = new DummyAxis( );
                axisDump = new DummyAxis( );
                axisBoxCassetteElevator = new DummyAxis();
                loadPushBoxCylinder = new DummyCylinder();
                boxReader = new DummyReader();
                digitalController1 = new DummyController(16,16);// 0-15
            }
            else
            {

                axisTransfer = new ToyoAxis("COM4");
                axisTurnLid = new OrientAxis("COM3");
                axisDump = new ToyoAxis("COM5");
                axisBoxCassetteElevator = new ToyoAxis("COM11");
                loadPushBoxCylinder = new ToyoCylinder("COM13");            
                boxReader = new BoxReader("192.168.100.80", 9004);
                //        IBarcodeReader medcineBottleReader = new MedcineBottleReader("192.168.100.81", 9005);

                digitalController1 = new ADTech_USB4750(1);// 0-15
                 // IDigitalSignalController digitalController2 = new ADTech_USB4750(2);// 0-15
            }

            // DigitalOutput[] outputarr = digitalController1.SignalOutput.Concat(digitalController2.SignalOutput).ToArray();//0-15 ,16-31  

            // LoadModle = new LoadModule(boxReader, axisBoxCassetteElevator, digitalController1.SignalOutput, digitalController1.SignalInput);

            //module
            loadModel = new LoadModel(boxReader, axisBoxCassetteElevator, loadPushBoxCylinder);
            DumpModle = new DumpModule(digitalController1.SignalOutput, digitalController1.SignalInput);
            OutputModle = new OutputModule();

        }



        public void Home()
        {
            DumpModle.PreHome();
             //LoadModle.Home();

            DumpModle.Home();
        }



    }
}
