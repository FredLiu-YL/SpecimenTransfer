using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp3.Component;

namespace WindowsFormsApp3
{
    public partial class Machine
    {

        //大橫移軸
        private IAxis axisTransfer;
        //開蓋旋轉軸
        private IAxis axisTurnLid;
        //傾倒藥罐軸
        private IAxis axisDump;

        //檢體盒卡匣 入料升降軸
        //private IAxis axisBoxCassetteElevator;
        // private IBarcodeReader barcodeReader;
        // private IElectricCylinder loadPushBoxCylinder;
       
        private LoadModel loadModel;

        //抓取濾紙汽缸
        private DigitalOutput CatchFilterPaperCylinder;

        //抓取罐子夾爪汽缸
        private DigitalOutput JarClampCylinder;

        //檢體盒 壓蓋汽缸
        private DigitalOutput CloseBoxCoverCylinder;



        public Machine()
        {
       
        }



        public MachineSetting MachineSet { get; set; }




        public  void Initial()
        {
            axisTransfer = new ToyoAxis("COM4");

            axisTurnLid = new OrientAxis("COM3");
            axisDump = new ToyoAxis("COM5");

            IAxis axisBoxCassetteElevator = new ToyoAxis("COM11");
            IBarcodeReader barcode = new KyenceBarcode();
            IElectricCylinder loadPushBoxCylinder = new ToyoCylinder("COM13");
            ADTech_USB4750 aDTech_USB4750 = new ADTech_USB4750("");

            loadModel = new LoadModel(barcode, axisBoxCassetteElevator, loadPushBoxCylinder, aDTech_USB4750.SignalOutput,aDTech_USB4750.SignalInput);


        }

        public void Home()
        {


        }
    }
}
