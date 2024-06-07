using Nito.AsyncEx;
using SpecimenTransfer.Model.Component;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
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

        
        /// <summary>
        /// IO Output 資料
        /// </summary>
        public List<DigitalOutput> IoOutList { get; private set; }
        /// <summary>
        /// IO Input 資料
        /// </summary>
        public List<DigitalIntput> IoInList { get; private set; }

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

            //4750 IO
            IDigitalSignalController digitalController1 = null;
            IDigitalSignalController digitalController2 = null;

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

            }
            else
            {

                var serialPortToyo = new SerialPort
                {
                    PortName = "COM6", // Adjust the COM port as necessary
                    BaudRate = 19200,
                    DataBits = 8,
                    Parity = Parity.None,
                    StopBits = StopBits.One
                };

             var   serialPortOrient = new SerialPort
                {
                    PortName = "COM4", // Adjust the COM port as necessary
                    BaudRate = 19200,
                    DataBits = 8,
                    Parity = Parity.None,
                    StopBits = StopBits.One
                };
                //485通訊開啟
                serialPortToyo.Open();
                serialPortOrient.Open();

                //TOYO
                slideTableAxis = new ToyoAxis(serialPortToyo, 1);
                coverAndStorageElevatorAxis = new ToyoAxis(serialPortToyo, 2);
                bottleElevatorAxis = new ToyoAxis(serialPortToyo, 3);
                filterPaperElevatorAxis = new ToyoAxis(serialPortToyo, 4);

                //Orientalmotor
                bottleDumpAxis = new OrientAxis(serialPortOrient, 1);
                bottleScrewAxis = new OrientAxis(serialPortOrient, 2);
                
                //Barcode reader
                paperReader = new BoxReader("192.168.100.100", 9004);
                bottleReader = new BoxReader("192.168.100.80", 9004);

                digitalController1 = new ADTech_USB4750(1);// 0-15
                                                           
                digitalController2 = new ADTech_USB4750(2);// IDigitalSignalController digitalController2 = new ADTech_USB4750(2);// 0-15

            }
            //合併兩張控制卡的輸出輸入
            IoOutList = digitalController1.SignalOutput.ToList();
            IoOutList.AddRange(digitalController2.SignalOutput);
            IoInList = digitalController1.SignalInput.ToList();
            IoInList.AddRange(digitalController2.SignalInput);

            //module
            LoadModle = new LoadModule(IoOutList.ToArray(), IoInList.ToArray(), slideTableAxis, filterPaperElevatorAxis, paperReader);
            DumpModle = new DumpModule(IoOutList.ToArray(), IoInList.ToArray(), slideTableAxis, bottleElevatorAxis, bottleScrewAxis, bottleDumpAxis, bottleReader);
            OutputModle = new OutputModule(IoOutList.ToArray(), IoInList.ToArray(), slideTableAxis, coverAndStorageElevatorAxis);

            
        }



    }
}
