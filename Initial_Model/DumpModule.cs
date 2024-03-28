using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp3.Component;

namespace WindowsFormsApp3.Initial_Model
{
    public class DumpModule
    {

        //----Digital Output----

        //camera shot取像
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

        //----軸控----

        //藥罐升降滑台-升
        private IAxis medicineBottleUpAxis;
        //藥罐升降滑台-降
        private IAxis medicineBottleDownAxis;
        //藥罐升降滑台到位-ready
        private IAxis medicineBottleUpDownAxisReady;

        //旋開藥蓋
        private IAxis unscrewMedicineJarAxis;
        //旋緊藥蓋
        private IAxis screwMedicineJarAxis;
        //旋轉藥蓋到位-ready
        private IAxis screwMedicineJarAxisReady;

        //藥瓶傾倒回HOME
        private IAxis medicineBottleDumpHomeAxis;
        //藥瓶傾倒
        private IAxis medicineBottleDumpTissueAxis;
        //藥瓶軸到位-ready
        private IAxis medicineBottleAxisReady;

        //載體滑台-Home
        private IAxis carrierSlideHomeAxis;
        //載體滑台-移動
        private IAxis carrierSlideMoveAxis;
        //載體滑台到位-ready
        private IAxis carrierSlideAxisReady;

        //----條碼----
        //藥罐條碼
        private IBarcodeReader medcineBottle;
        //載體盒條碼
        private IBarcodeReader carrierBottle;

        public DumpModule(DigitalOutput[] signalOutput, DigitalIntput[] signalInput, 
            IAxis carrierSlideTableAxis, IAxis catchFilterPaperAxis, IAxis medicineBottleUpDownAxis, 
            IAxis screwMedicineCapAxis, IAxis medicineBottleDumpAxis)
        {
            //----Digital Output----
            injectionCleanCylinder = signalOutput[5];//注射清洗氣缸

            injectionCleanSwitch = signalOutput[6];//注射清洗

            upperClampMedicineCylinder = signalOutput[7];//藥罐瓶蓋氣缸-上夾爪

            lowerClampMedicineCylinder = signalOutput[8];//藥罐瓶蓋氣缸-下夾爪

            medicineBottleMoveCylinder = signalOutput[9];//藥罐移載氣缸

            cameraShot = signalOutput[10];//camera shot取像

            backLightCylinder = signalOutput[11];//背光氣缸

            injectRedInk = signalOutput[12];//注射紅墨水

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

            //----軸控----
            medicineBottleUpAxis = medicineBottleUpDownAxis;//藥罐升降滑台-升
            medicineBottleDownAxis = medicineBottleUpDownAxis;//藥罐升降滑台-降
            medicineBottleAxisReady = medicineBottleUpDownAxis;//藥罐升降滑台-ready

            unscrewMedicineJarAxis = screwMedicineCapAxis;//旋開藥蓋
            screwMedicineJarAxis = screwMedicineCapAxis; ;//旋緊藥蓋
            screwMedicineJarAxisReady = screwMedicineCapAxis;//旋蓋到位訊號

             medicineBottleDumpHomeAxis = medicineBottleDumpAxis;//藥瓶傾倒回HOME
             medicineBottleDumpTissueAxis = medicineBottleDumpAxis;//藥瓶傾倒
             medicineBottleAxisReady = medicineBottleDumpAxis; ;//藥瓶軸到位訊號

            carrierSlideHomeAxis = carrierSlideTableAxis;//載體滑台回Home
            carrierSlideMoveAxis = carrierSlideTableAxis;//載體滑台移動
            carrierSlideAxisReady = carrierSlideTableAxis;//載體滑台到位訊號

        }

        //傾倒前置動作
        public async Task DumpPreAction()
        {
            try
            {
                if (upperClampMedicineCylinderCloseSignal.Signal && lowerClampMedicineCylinderOpenSignal.Signal
                    && medicineBottleMoveCylinderPullSignal.Signal)//判斷藥罐上夾爪在夾的位置且下夾爪在開的位置且藥罐移載氣缸在收的位置
                {
                    medicineBottleDownAxis.SetVelocity(3000, 10, 10);
                    medicineBottleDownAxis.MoveAsync(1000);
                    await Task.Delay(1000);
                }
                else if(medicineBottleAxisReady.IsInposition)
                {
                    lowerClampMedicineCylinder.On(8,true);
                }
                else if(upperClampMedicineCylinderCloseSignal.Signal && lowerClampMedicineCylinderCloseSignal.Signal)
                {
                    unscrewMedicineJarAxis.SetVelocity(1000, 10, 10);//旋開藥罐速度
                    unscrewMedicineJarAxis.MoveAsync(0);//旋開藥罐位置
                    medicineBottleUpAxis.SetVelocity(3000, 10, 10);//藥罐上升位置
                    medicineBottleUpAxis.MoveAsync(3000);//藥罐上升速度
                }
                    
            }
         
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                // MyErrorHandler.HandleError(ex); // 使用自訂的錯誤處理機制
            }

        }

        //旋緊藥罐
        public async Task ScrewMedicineCover()
        {

            unscrewMedicineJarAxis.SetVelocity(1000, 10, 10);//旋緊藥罐速度
            unscrewMedicineJarAxis.MoveAsync(3000);//旋緊藥罐位置

        }

        //藥罐下降
        public async Task MedicineBottleDown()
        {
           

        }

        //拍照檢查
        public async Task CameraShot()
        {

          cameraShot.On(10, true);//拍照

        }

        //傾倒載體
        public async Task DumpCarrier()
        {
      
          medicineBottleDumpTissueAxis.SetVelocity(1000,10,10);//藥罐傾倒速度
          medicineBottleDumpTissueAxis.MoveAsync(1000);//藥罐傾倒位置
         
        }


        //載體回home
        public async Task CarrierReturnHome()
        {

            medicineBottleDumpHomeAxis.Home();

        }

        //清洗藥罐
        public async Task CleanMedicineBottle()
        {

            injectionCleanSwitch.On(6, true);//注射清洗開
            await Task.Delay(3000);
            injectionCleanSwitch.Off(6, true);//注射清洗關;

        }


        //注射紅墨水
        public async Task InjectRedInk()
        {

            injectRedInk.On(12, true);
            await Task.Delay(2000);

        }
        
        
    }

     

}
