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

        //藥罐升降滑台-Home
        private IAxis medicineBottleHomeAxis;
        //藥罐升降滑台-位置
        private IAxis medicineBottlePostionAxis;
        //藥罐升降滑台到位-ready
        private IAxis medicineBottleUpDownAxisReady;

        //藥蓋-Home
        private IAxis MedicineJarHomeAxis;
        //藥蓋-位置
        private IAxis MedicineJarPostionAxis;
        //旋轉藥蓋到位-ready
        private IAxis screwMedicineJarAxisReady;

        //藥瓶傾倒-HOME
        private IAxis medicineBottleDumpHomeAxis;
        //藥瓶傾倒-位置
        private IAxis medicineBottleDumpPostionAxis;
        //藥瓶軸到位-ready
        private IAxis medicineBottleAxisReady;

        //載體滑台-Home
        private IAxis carrierSlideHomeAxis;
        //載體滑台-位置
        private IAxis carrierSlidePostionAxis;
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

            /*
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
            */
        }
        /*
       //傾倒前置動作
       public async Task DumpPreAction()
       {
           try
           {
               if (upperClampMedicineCylinderCloseSignal.Signal && lowerClampMedicineCylinderOpenSignal.Signal
                   && medicineBottleMoveCylinderPullSignal.Signal)//判斷藥罐上夾爪在夾的位置且下夾爪在開的位置且藥罐移載氣缸在收的位置
               {
                   medicineBottleDownAxis.SetVelocity(3000, 10, 10);//藥罐下降速度
                   medicineBottleDownAxis.MoveAsync(1000);//藥罐下降位置
                   await Task.Delay(1000);
               }
               else if (medicineBottleAxisReady.IsInposition)//藥罐下降到位
               {
                   lowerClampMedicineCylinder.On(8, true);//藥罐下夾爪夾
               }
               else if (upperClampMedicineCylinderCloseSignal.Signal && lowerClampMedicineCylinderCloseSignal.Signal)//判斷藥罐上夾爪及下夾爪皆在夾的位置
               {
                   unscrewMedicineJarAxis.SetVelocity(1000, 10, 10);//旋開藥罐速度
                   unscrewMedicineJarAxis.MoveAsync(0);//旋開藥罐位置
                   medicineBottleUpAxis.SetVelocity(3000, 10, 10);//藥罐上升位置
                   medicineBottleUpAxis.MoveAsync(3000);//藥罐上升速度
               }

               else if (unscrewMedicineJarAxis.IsInposition && medicineBottleUpAxis.IsInposition)//判斷藥罐旋開到位且藥罐上升到位
               {
                   medicineBottleMoveCylinder.On(9, true);//藥罐移載氣缸推
                   backLightCylinder.On(11, true);//背光氣缸推
                   await Task.Delay(1000);
               }
               else if (medicineBottleMoveCylinderPushSignal.Signal && backLightCylinderPushSignal.Signal)//判斷藥罐氣缸和背光氣缸是否在推位
               {
                   cameraShot.On(10, true);//拍照檢查
                   await Task.Delay(1000);
                   medicineBottleMoveCylinder.Off(9, true);//藥罐移載氣缸收
                   backLightCylinder.Off(11, true);//背光氣缸收
                   injectionCleanCylinder.On(5, true);//清洗氣缸推
                   await Task.Delay(1000);

               }
           }

           catch (Exception ex)
           {
               Console.Error.WriteLine(ex.Message);
               // MyErrorHandler.HandleError(ex); // 使用自訂的錯誤處理機制
           }

       }
        
        bool cleanStatus;
        //傾倒載體並檢查藥瓶
        public async Task DumpCarrierCheckMeddicneBottle()
        {

            try
            {
                if (medicineBottleMoveCylinderPullSignal.Signal && backLightCylinderPullSignal.Signal
               && injectionCylinderPushSignal.Signal)//判斷藥罐、背光氣缸是否在收位、注射氣缸是否在推位
                {
                    cleanStatus = true;
                    for (int i = 0; i <= 2; i++)
                    {
                        medicineBottleDumpTissueAxis.MoveAsync(2000);//藥罐傾倒
                        await Task.Delay(500);
                        injectionCleanSwitch.On(6, true);//注射清洗啟動
                        await Task.Delay(5000);
                    }
                    cleanStatus = false;
                }
                else if (cleanStatus = false)
                {
                    medicineBottleDumpTissueAxis.MoveAsync(0);//藥罐傾倒回原點
                    injectionCleanSwitch.Off(6, true);//注射清洗關閉
                    injectionCleanCylinder.Off(5, true);//清洗氣缸收
                    medicineBottleMoveCylinder.On(9, true);//藥罐移載氣缸推
                    backLightCylinder.On(11, true);//背光氣缸推
                    await Task.Delay(500);
                }
                else if (backLightCylinderPushSignal.Signal)
                {
                    cameraShot.On(10, true);//拍照檢查
                }
            }

            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                // MyErrorHandler.HandleError(ex); // 使用自訂的錯誤處理機制
            }

        }

        //傾倒後續動作
        public async Task DumpFollowUp()
        {
            medicineBottleMoveCylinder.Off(9, true);//藥罐移載氣缸收
            backLightCylinder.Off(11, true);//背光氣缸收

            try
            {
                if(medicineBottleMoveCylinderPullSignal.Signal && upperClampMedicineCylinderCloseSignal.Signal 
                    && lowerClampMedicineCylinderCloseSignal.Signal)//判斷藥罐汽缸在收的位置且上夾爪及下夾爪皆在夾的位置
                {
                    medicineBottleDownAxis.SetVelocity(3000, 10, 10);//藥罐下降速度
                    medicineBottleDownAxis.MoveAsync(1000);//藥罐下降位置
                    await Task.Delay(1000);
                }
               

            }

            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                // MyErrorHandler.HandleError(ex); // 使用自訂的錯誤處理機制
            }

        }
        */






    }
}
