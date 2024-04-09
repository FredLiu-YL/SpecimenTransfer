using SpecimenTransfer.Model.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SpecimenTransfer.Model
{
    public partial class Machine
    {
        
        private IDigitalSignalController digitaiOutput;

        

        //檢體盒卡匣 入料升降軸
        //private IAxis axisBoxCassetteElevator;
        // private IBarcodeReader barcodeReader;
        // private IElectricCylinder loadPushBoxCylinder;


        public MachineSetting MachineSet { get; set; }
        public LoadModule LoadModle { get; set; }
        public DumpModule DumpModle { get; set; }
        public OutputModule OutputModle { get; set; }
        public MainRecipe Recipe { get; set; }
        


        public void Initial(bool isSimulate)
        {
            //橫向搬送軸
            IAxis slideTableAxis = null;
            //濾紙升降軸
            IAxis filterPaperElevatorAxis = null;

            //瓶罐升降軸
            IAxis bottleElevatorAxis = null;
            //瓶蓋旋轉軸
            IAxis bottleScrewAxis = null;
            //瓶罐傾倒軸
            IAxis bottleDumpAxis = null;

            //放蓋及收納升降軸
            IAxis coverAndStorageElevatorAxis = null;

            //入料讀取器
            IBarcodeReader paperReader = null;
            //瓶罐讀取器
            IBarcodeReader bottleReader = null;

            //IO
            IDigitalSignalController digitalController1 = null;
            IDigitalSignalController digitalController2 = null;

            ////橫向搬送軸
            //IAxis axisTransfer = null;
            ////開蓋旋轉軸
            //IAxis axisTurnLid = null;
            ////傾倒藥罐軸
            //IAxis axisDump = null;
            ////入料 載體盒卡匣 升降軸
            //IAxis axisBoxCassetteElevator = null;

            //IElectricCylinder loadPushBoxCylinder = null;
            //IBarcodeReader boxReader = null;
            //IDigitalSignalController digitalController1 = null;
            //IDigitalSignalController digitalController2 = null;
            //IAxis carrierSlideTableAxis = null;
            //IAxis catchFilterPaperAxis = null;
            //IAxis medicineBottleAxis = null;
            //IAxis axisCoverElevator = null;
            //IAxis axisCarrierSlideTable = null;

            if (isSimulate)
            {
                slideTableAxis = new DummyAxis();
                filterPaperElevatorAxis = new DummyAxis();

                bottleElevatorAxis = new DummyAxis();
                bottleScrewAxis = new DummyAxis();
                bottleDumpAxis = new DummyAxis();

                coverAndStorageElevatorAxis = new DummyAxis();

                paperReader = new DummyReader();
                bottleReader = new DummyReader();

                digitalController1 = new DummyController(16, 16);// 0-15
                digitalController2 = new DummyController(16, 16);// 16-31
                //axisTransfer = new DummyAxis();
                //axisTurnLid = new DummyAxis();
                //axisDump = new DummyAxis();
                //axisBoxCassetteElevator = new DummyAxis();
                //loadPushBoxCylinder = new DummyElectricCylinder();
                //boxReader = new DummyReader();
                //digitalController1 = new DummyController(16, 16);// 0-15
                //digitalController2 = new DummyController(16, 16);// 16-31

                //catchFilterPaperAxis = new DummyAxis();
            }
            else
            {
                slideTableAxis = new ToyoAxis("COM3", 1);

                filterPaperElevatorAxis = new ToyoAxis("COM4", 1);

                bottleElevatorAxis = new ToyoAxis("COM5", 1);
                bottleScrewAxis = new OrientAxis("COM6", 1);
                bottleDumpAxis = new OrientAxis("COM7", 1);

                coverAndStorageElevatorAxis = new ToyoAxis("COM8", 1);

                paperReader = new BoxReader("192.168.100.80", 9005);
                bottleReader = new BoxReader("192.168.100.80", 9004);

                //IBarcodeReader medcineBottleReader = new MedcineBottleReader("192.168.100.81", 9005);

                digitalController1 = new ADTech_USB4750(1);// 0-15
                                                           // IDigitalSignalController digitalController2 = new ADTech_USB4750(2);// 0-15
                digitalController2 = new ADTech_USB4750(2);

                //axisTransfer = new ToyoAxis("COM4", 1);
                //axisTurnLid = new OrientAxis("COM3", 1);
                //axisDump = new ToyoAxis("COM5", 1);
                //axisBoxCassetteElevator = new ToyoAxis("COM11", 1);
                //loadPushBoxCylinder = new ToyoCylinder("COM13");

                //boxReader = new BoxReader("192.168.100.80", 9004);

                //axisBoxCassetteElevator = new ToyoAxis("COM11", 1);
                //loadPushBoxCylinder = new ToyoCylinder("COM13");


                ////IBarcodeReader medcineBottleReader = new MedcineBottleReader("192.168.100.81", 9005);

                //digitalController1 = new ADTech_USB4750(1);// 0-15
                //                                           // IDigitalSignalController digitalController2 = new ADTech_USB4750(2);// 0-15
            }
            //合併兩張控制卡的輸出輸入
            var outList = digitalController1.SignalOutput.ToList();
            outList.AddRange(digitalController2.SignalOutput);
            var inList = digitalController1.SignalInput.ToList();
            inList.AddRange(digitalController2.SignalInput);
            
            //module
            LoadModle = new LoadModule(outList.ToArray(), inList.ToArray(), slideTableAxis, filterPaperElevatorAxis, paperReader);
            DumpModle = new DumpModule(outList.ToArray(), inList.ToArray(), slideTableAxis, bottleElevatorAxis, bottleScrewAxis, bottleDumpAxis, bottleReader);
            OutputModle = new OutputModule(outList.ToArray(), inList.ToArray(), slideTableAxis, coverAndStorageElevatorAxis);

        }

        public async Task Home()
        {

            await LoadModle.Home();

            await DumpModle.Home();
        }



    }
}
