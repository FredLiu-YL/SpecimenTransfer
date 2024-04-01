using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsFormsApp3.Component;

namespace WindowsFormsApp3.Initial_Model
{
    public class DumpModule
    {

        //----Digital Output----

        //camera shot載體盒條碼
        private DigitalOutput shotCarrierBottleBarcode;

        //camera shot藥罐條碼
        private DigitalOutput shotMedcineBottleBarcode;

        //camera拍照
        private DigitalOutput cameraShot;

        //上方夾藥罐氣缸-打開關閉
        private DigitalOutput upperClampMedicineCylinder;

        //下方夾藥罐氣缸-打開關閉
        private DigitalOutput lowerClampMedicineCylinder;

        //藥罐移載氣缸
        private DigitalOutput medicineBottleMoveCylinder;

        //背光氣缸
        private DigitalOutput backLightCylinder;

        //注射清洗氣缸
        private DigitalOutput injectionCleanCylinder;

        //注射清洗
        private DigitalOutput injectionCleanSwitch;

        //注射紅墨水
        private DigitalOutput injectRedInk;

        //紅墨水氣缸
        private DigitalOutput redInkCylinder;

        //----Digital Input----

        //上方夾藥罐氣缸-打開訊號
        private DigitalIntput upperClampMedicineCylinderOpenSignal;
        //上方夾藥罐氣缸-關閉訊號
        private DigitalIntput upperClampMedicineCylinderCloseSignal;

        //下方夾藥罐氣缸-打開訊號
        private DigitalIntput lowerClampMedicineCylinderOpenSignal;
        //下方夾藥罐氣缸-關閉訊號
        private DigitalIntput lowerClampMedicineCylinderCloseSignal;

        //藥罐移載氣缸-推
        private DigitalIntput medicineBottleMoveCylinderPushSignal;
        //藥罐移載氣缸-收
        private DigitalIntput medicineBottleMoveCylinderPullSignal;

        //背光氣缸-推
        private DigitalIntput backLightCylinderPushSignal;
        //背光氣缸-收
        private DigitalIntput backLightCylinderPullSignal;

        //注射氣缸-推
        private DigitalIntput injectionCylinderPushSignal;
        //注射氣缸-收
        private DigitalIntput injectionCylinderPullSignal;

        //紅墨水氣缸-推
        private DigitalIntput redInkCylinderPushSignal;
        //紅墨水氣缸-收
        private DigitalIntput redInkCylinderPullSignal;

        //----軸控----

        //載體滑台-Home
        private IAxis axisCarrierSlideHome;
        //載體滑台-位置
        private IAxis axisCarrierSlidePostion;
        //載體滑台到位-ready
        private IAxis axisCarrierSlideReady;

        //藥罐升降滑台-Home
        private IAxis axisMedicineBottleHome;
        //藥罐升降滑台-位置
        private IAxis axisMedicineBottleElevatorPostion;
        //藥罐升降滑台到位-ready
        private IAxis axisMedicineBottleElevatorReady;

        //旋藥蓋-Home
        private IAxis axisScrewMedicineJarHome;
        //旋藥蓋-位置
        private IAxis axisScrewMedicineJarPostion;
        //旋藥蓋到位-ready
        private IAxis axisScrewMedicineJarReady;

        //藥瓶傾倒-HOME
        private IAxis axisMedicineBottleDumpHome;
        //藥瓶傾倒-位置
        private IAxis axisMedicineBottleDumpPostion;
        //藥瓶傾倒到位-ready
        private IAxis axisMedicineBottleReady;

        //----條碼----
        //藥罐條碼
        private IBarcodeReader medcineBottleBarcode;
        //載體盒條碼
        private IBarcodeReader boxBottleBarcode;

        /// <summary>
        /// 入料模組參數
        /// </summary>
        public DumpModuleParamer DumpModuleParam { get; set; } = new DumpModuleParamer();

        public DumpModule(DigitalOutput[] signalOutput, DigitalIntput[] signalInput,
            IAxis axisCarrierSlideTable, IAxis axisMedicineBottleElevator,
            IAxis axisScrewMedicineCap, IAxis axisMedicineBottleDump, IBarcodeReader boxReader)
        {
            //----Digital Output----
            injectionCleanCylinder = signalOutput[5];//注射清洗氣缸

            injectionCleanSwitch = signalOutput[6];//注射清洗

            upperClampMedicineCylinder = signalOutput[7];//藥罐瓶蓋氣缸-上夾爪

            lowerClampMedicineCylinder = signalOutput[8];//藥罐瓶蓋氣缸-下夾爪

            medicineBottleMoveCylinder = signalOutput[9];//藥罐移載氣缸

            cameraShot = signalOutput[10];//camera拍照

            backLightCylinder = signalOutput[11];//背光氣缸

            injectRedInk = signalOutput[12];//注射紅墨水

            redInkCylinder = signalOutput[13];//紅墨水氣缸



            //----Digital Input----
            injectionCylinderPushSignal = signalInput[10];//注射清洗氣缸-推
            injectionCylinderPullSignal = signalInput[11];//注射清洗氣缸-收

            medicineBottleMoveCylinderPushSignal = signalInput[12];//藥罐移載氣缸-推
            medicineBottleMoveCylinderPullSignal = signalInput[13];//藥罐移載氣缸-收

            upperClampMedicineCylinderCloseSignal = signalInput[14];//藥罐瓶蓋上夾爪-關
            upperClampMedicineCylinderOpenSignal = signalInput[15];//藥罐瓶蓋上夾爪-開

            lowerClampMedicineCylinderCloseSignal = signalInput[16]; //藥罐瓶蓋下夾爪-關
            lowerClampMedicineCylinderOpenSignal = signalInput[17]; //藥罐瓶蓋下夾爪-開

            backLightCylinderPushSignal = signalInput[18];//背光氣缸-推
            backLightCylinderPullSignal = signalInput[19];//背光氣缸-收

            redInkCylinderPushSignal = signalInput[20];//紅墨水氣缸-推
            redInkCylinderPullSignal = signalInput[21];//紅墨水氣缸-收



            //----軸控----
            axisMedicineBottleHome = axisMedicineBottleElevator;//藥罐升降滑台-home
            axisMedicineBottleElevatorPostion = axisMedicineBottleElevator;//藥罐升降滑台-位置
            axisMedicineBottleElevatorReady = axisMedicineBottleElevator;//藥罐升降滑台-ready

            axisScrewMedicineJarHome = axisScrewMedicineCap;//旋藥蓋-home
            axisScrewMedicineJarPostion = axisScrewMedicineCap;//旋藥蓋-位置
            axisScrewMedicineJarReady = axisScrewMedicineCap;//旋蓋到位訊號

            axisMedicineBottleDumpHome = axisMedicineBottleDump;//藥罐傾倒-home
            axisMedicineBottleDumpPostion = axisMedicineBottleDump;//藥罐傾倒-位置
            axisMedicineBottleReady = axisMedicineBottleDump; ;//藥罐到位訊號

            axisCarrierSlideHome = axisCarrierSlideTable;//載體滑台回Home
            axisCarrierSlidePostion = axisCarrierSlideTable;//載體滑台移動
            axisCarrierSlideReady = axisCarrierSlideTable;//載體滑台到位訊號
            

            //Barcode reader
            //藥罐條碼
            medcineBottleBarcode = boxReader;
            //載體盒條碼
            boxBottleBarcode = boxReader;

            
    }
        
       //旋開藥罐
       public async Task UnscrewMedicineJar()
       {
           try
           {
                //藥罐下降->藥罐下夾爪關閉->旋開藥罐->藥罐上升
                axisMedicineBottleElevatorPostion.MoveAsync(DumpModuleParam.BottleElevatorPos);
                lowerClampMedicineCylinder.Switch(true);
                WaitInputSignal(lowerClampMedicineCylinderCloseSignal);
                WaitInputSignal(upperClampMedicineCylinderOpenSignal);
                axisScrewMedicineJarPostion.MoveAsync(DumpModuleParam.BottleDumpPos);
                axisMedicineBottleElevatorPostion.MoveAsync(DumpModuleParam.BottleElevatorPos);

            }

           catch (Exception ex)
           {
               Console.Error.WriteLine(ex.Message);
               // MyErrorHandler.HandleError(ex); // 使用自訂的錯誤處理機制
           }

       }

        //檢查藥罐
        public async Task CheckBottleAction()
        {
            //藥罐移載氣缸推->背光氣缸推->拍照檢查->藥罐移載氣缸收->背光氣缸收
            medicineBottleMoveCylinder.Switch(true);
            WaitInputSignal(medicineBottleMoveCylinderPushSignal);
            backLightCylinder.Switch(true);
            WaitInputSignal(backLightCylinderPushSignal);
            cameraShot.Switch(true);
            await Task.Delay(500);
            medicineBottleMoveCylinder.Switch(false);
            cameraShot.Switch(false);

        }

        //傾倒清洗藥罐
        public async Task DumpCleanBottle()
        {
            //傾倒藥罐->清洗藥罐->計時->停止清洗->藥罐回原點
            try
            {
                axisMedicineBottleDumpPostion.MoveAsync(DumpModuleParam.BottleDumpPos);
                WaitInputSignal(axisMedicineBottleReady.IsInposition);
                injectionCleanSwitch.Switch(true);
                await Task.Delay(2000);
                injectionCleanSwitch.Switch(false);
                axisMedicineBottleDumpPostion.MoveAsync(DumpModuleParam.BottleDumpPos);

            }

            catch (Exception ex)
            {
                throw ex;

            }

        }

        //旋緊藥罐
        public async Task ScrewMedicineJar()
        {
            try
            {
                //旋緊藥罐->藥罐下降->藥罐下夾爪開啟->藥罐上升
                axisScrewMedicineJarPostion.MoveAsync(DumpModuleParam.BottleElevatorPos);
                WaitInputSignal(axisScrewMedicineJarReady.IsInposition);
                axisMedicineBottleElevatorPostion.MoveAsync(DumpModuleParam.BottleElevatorPos);
                WaitInputSignal(axisMedicineBottleReady.IsInposition);
                lowerClampMedicineCylinder.Switch(false);
                axisMedicineBottleElevatorPostion.MoveAsync(DumpModuleParam.BottleElevatorPos);

            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        //注入紅墨水
        public async Task InjectRedInk()
        {
            try
            {
                //載體盒到位->紅墨水氣缸推->注射紅墨水->紅墨水氣缸收
                WaitInputSignal(axisCarrierSlideReady.IsInposition);
                redInkCylinder.Switch(true);
                WaitInputSignal(redInkCylinderPushSignal);
                injectRedInk.Switch(true);
                await Task.Delay(3000);
                injectRedInk.Switch(false);

            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        //載體盒移動至清洗站
        public async Task carrierMoveToClean()
        {
            axisCarrierSlidePostion.MoveAsync(6000);//載體滑台移動至注射站

        }

        //載體盒移動至清洗站
        public async Task carrierMoveToRedInk()
        {
            axisCarrierSlidePostion.MoveAsync(6000);//載體滑台移動至注射站

        }

        //載體滑台移動至濾紙站
        public async Task carrierMoveToFilterPaper()
        {
            axisCarrierSlidePostion.MoveAsync(5000);//載體滑台移動至濾紙站
            await Task.Delay(1000);
        }

        //讀藥罐條碼
        public async Task<string> ReadBarcode()
        {
            shotMedcineBottleBarcode.Switch(true);
            await Task.Delay(500);
            string carrierDataReceived = medcineBottleBarcode.ReceiveData();
            await Task.Delay(500);
            shotMedcineBottleBarcode.Switch(false);
            return carrierDataReceived;
        }

        public class DumpModuleParamer
        {
            //藥罐傾倒位置座標
            public double BottleDumpPos { get; set; }

            //藥罐升降位置座標
            public double BottleElevatorPos { get; set; }

            //橫移軸在傾倒載體位置
            public double CarrierTableBottleDumpPos { get; set; }

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
