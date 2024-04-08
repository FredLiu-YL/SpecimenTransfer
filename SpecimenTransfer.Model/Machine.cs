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
            IAxis carrierSlideTableAxis = null;
            //濾紙升降軸
            IAxis filterPaperElevatorAxis = null;

            //瓶子升降軸
            IAxis bottleElevatorAxis = null;
            //瓶子開蓋旋轉軸
            IAxis bottleScrewAxis = null;
            //瓶子傾倒軸
            IAxis bottleDumpAxis = null;

            //蓋子及收納升降滑台
            IAxis coverElevatorAxis = null;

            //入料讀取器
            IBarcodeReader paperReader = null;
            //瓶子讀取器
            IBarcodeReader bottleReader = null;

            //IO
            IDigitalSignalController digitalController1 = null;
            IDigitalSignalController digitalController2 = null;


            if (isSimulate)
            {
                carrierSlideTableAxis = new DummyAxis();
                filterPaperElevatorAxis = new DummyAxis();

                bottleElevatorAxis = new DummyAxis();
                bottleScrewAxis = new DummyAxis();
                bottleDumpAxis = new DummyAxis();

                coverElevatorAxis = new DummyAxis();

                paperReader = new DummyReader();
                bottleReader = new DummyReader();

                digitalController1 = new DummyController(16, 16);// 0-15
                digitalController2 = new DummyController(16, 16);// 16-31

            }
            else
            {

                carrierSlideTableAxis = new ToyoAxis("COM3", 1);
                filterPaperElevatorAxis = new OrientAxis("COM4", 1);

                bottleElevatorAxis=new ToyoAxis("COM5", 1);
                bottleScrewAxis = new ToyoAxis("COM6", 1);
                bottleDumpAxis = new ToyoAxis("COM7", 1);

                coverElevatorAxis = new ToyoAxis("COM8", 1);

                paperReader = new BoxReader("192.168.100.80", 9005);
                bottleReader = new BoxReader("192.168.100.80", 9004);

                //IBarcodeReader medcineBottleReader = new MedcineBottleReader("192.168.100.81", 9005);

                digitalController1 = new ADTech_USB4750(1);// 0-15
                                                           // IDigitalSignalController digitalController2 = new ADTech_USB4750(2);// 0-15
                digitalController2 = new ADTech_USB4750(2);
            }
            //合併兩張控制卡的輸出輸入
            var outList = digitalController1.SignalOutput.ToList();
            outList.AddRange(digitalController2.SignalOutput);
            var inList = digitalController1.SignalInput.ToList();
            inList.AddRange(digitalController2.SignalInput);
            
            ////module
            //LoadModle = new LoadModule(outList.ToArray(), inList.ToArray(), carrierSlideTableAxis, filterPaperElevatorAxis);
            //DumpModle = new DumpModule(outList.ToArray(), inList.ToArray(), carrierSlideTableAxis, bottleElevatorAxis, bottleDumpAxis, bottleScrewAxis);
            //OutputModle = new OutputModule(outList.ToArray(), inList.ToArray(),carrierSlideTableAxis, coverElevatorAxis );

        }

        public async Task Home()
        {

            await LoadModle.Home();

            await DumpModle.Home();
        }



    }
}
