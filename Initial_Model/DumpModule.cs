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

        //上方夾藥罐氣缸
        private DigitalOutput upperClampMedicineCylinder;

        //下方夾藥罐氣缸
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
        private DigitalIntput redInkCylinderCylinderPushSignal;
        //紅墨水氣缸-收
        private DigitalIntput redInkCylinderCylinderPullSignal;

        //----軸控----

        //藥罐升降滑台-Home
        private IAxis axisMedicineBottleEvelatorHome;
        //藥罐升降滑台-位置
        private IAxis axisMedicineBottleEvelatorPostion;
        //藥罐升降滑台到位-ready
        private IAxis axisMedicineBottleEvelatorReady;

        //藥蓋-Home
        private IAxis axisMedicineJarHome;
        //藥蓋-位置
        private IAxis axisMedicineJarPostion;
        //旋蓋到位-ready
        private IAxis axisScrewMedicineJarReady;

        //藥瓶傾倒-HOME
        private IAxis axisMedicineBottleDumpHome;
        //藥瓶傾倒-位置
        private IAxis axisMedicineBottleDumpPostion;
        //藥瓶軸到位-ready
        private IAxis axisMedicineBottleReady;

        //載體滑台-Home
        private IAxis axisCarrierSlideHome;
        //載體滑台-位置
        private IAxis axisCarrierSlidePostion;
        //載體滑台到位-ready
        private IAxis axisCarrierSlideReady;



        //----條碼----
        //藥罐條碼
        private IBarcodeReader medcineBottle;
        //載體盒條碼
        private IBarcodeReader carrierBottle;

        public DumpModule(DigitalOutput[] signalOutput, DigitalIntput[] signalInput,
            IAxis carrierSlideTableAxis, IAxis medicineBottleAxis,IAxis axisTurnLid, IAxis axisDump)
        {
            
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

            redInkCylinderCylinderPushSignal = signalInput[20];//紅墨水氣缸-推
            redInkCylinderCylinderPullSignal = signalInput[21];//紅墨水氣缸-收

            //----軸控----
            axisMedicineBottleEvelatorHome = medicineBottleAxis;//藥罐升降滑台-home
            axisMedicineBottleEvelatorPostion = medicineBottleAxis;//藥罐升降滑台-位置
            axisMedicineBottleEvelatorReady = medicineBottleAxis; //藥罐升降滑台-ready

            axisMedicineJarHome = axisTurnLid;//旋藥蓋-home
            axisMedicineJarPostion = axisTurnLid; ;//旋藥蓋-位置
            axisScrewMedicineJarReady = axisTurnLid;//旋蓋到位訊號

            axisMedicineBottleDumpHome = axisDump;//藥瓶傾倒-home
            axisMedicineBottleDumpPostion = axisDump;//藥瓶傾倒
            axisMedicineBottleReady = axisDump; ;//藥瓶軸到位訊號

            axisCarrierSlideHome = carrierSlideTableAxis;//載體滑台-home
            axisCarrierSlidePostion = carrierSlideTableAxis;//載體滑台-位置
            axisCarrierSlideReady = carrierSlideTableAxis;//載體滑台-ready

        }
        
       //傾倒前置動作
       public async Task DumpPreAction()
       {
           try
           {
               if (upperClampMedicineCylinderCloseSignal.Signal && lowerClampMedicineCylinderOpenSignal.Signal
                   && medicineBottleMoveCylinderPullSignal.Signal)//判斷藥罐上夾爪在夾的位置且下夾爪在開的位置且藥罐移載氣缸在收的位置
               {
                    axisMedicineBottleEvelatorPostion.SetVelocity(3000, 10, 10);//藥罐下降速度
                    axisMedicineBottleEvelatorPostion.MoveAsync(1000);//藥罐下降位置
                   await Task.Delay(1000);
               }
               else if (axisMedicineBottleEvelatorReady.IsInposition)//藥罐下降到位
               {
                   lowerClampMedicineCylinder.Switch(true);//藥罐下夾爪關
               }
               else if (upperClampMedicineCylinderCloseSignal.Signal && lowerClampMedicineCylinderCloseSignal.Signal)//判斷藥罐上夾爪及下夾爪皆在夾的位置
               {
                    axisMedicineJarPostion.SetVelocity(1000, 10, 10);//旋開藥罐速度
                    axisMedicineJarPostion.MoveAsync(0);//旋開藥罐位置
                    axisMedicineBottleEvelatorPostion.SetVelocity(3000, 10, 10);//藥罐上升位置
                    axisMedicineBottleEvelatorPostion.MoveAsync(3000);//藥罐上升速度
               }

               else if (axisScrewMedicineJarReady.IsInposition && axisMedicineBottleEvelatorReady.IsInposition)//判斷藥罐旋開到位且藥罐上升到位
               {
                   medicineBottleMoveCylinder.Switch(true);//藥罐移載氣缸推
                   backLightCylinder.Switch(true);//背光氣缸推
                   await Task.Delay(1000);
               }
               else if (medicineBottleMoveCylinderPushSignal.Signal && backLightCylinderPushSignal.Signal)//判斷藥罐氣缸和背光氣缸是否在推位
               {
                   cameraShot.Switch(true);//拍照檢查
                   await Task.Delay(1000);
                   medicineBottleMoveCylinder.Switch(false);//藥罐移載氣缸收
                   backLightCylinder.Switch(false);//背光氣缸收
                   injectionCleanCylinder.Switch(true);//清洗氣缸推
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
                        axisMedicineBottleDumpPostion.MoveAsync(2000);//藥罐傾倒
                        await Task.Delay(500);
                        injectionCleanSwitch.Switch(true);//注射清洗啟動
                        await Task.Delay(5000);
                    }
                    cleanStatus = false;
                }
                else if (cleanStatus = false)
                {
                    axisMedicineBottleDumpPostion.MoveAsync(0);//藥罐傾倒回原點
                    injectionCleanSwitch.Switch(true);//注射清洗關閉
                    injectionCleanCylinder.Switch(false);//清洗氣缸收
                    medicineBottleMoveCylinder.Switch(true);//藥罐移載氣缸推
                    backLightCylinder.Switch(true);//背光氣缸推
                    await Task.Delay(500);
                }
                else if (backLightCylinderPushSignal.Signal)
                {
                    cameraShot.Switch(true);//拍照檢查
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
            medicineBottleMoveCylinder.Switch(false);//藥罐移載氣缸收
            backLightCylinder.Switch(false);//背光氣缸收

            try
            {
                if(medicineBottleMoveCylinderPullSignal.Signal && upperClampMedicineCylinderCloseSignal.Signal 
                    && lowerClampMedicineCylinderCloseSignal.Signal)//判斷藥罐汽缸在收的位置且上夾爪及下夾爪皆在夾的位置
                {
                    axisMedicineJarPostion.SetVelocity(1000, 1, 1);//旋緊藥罐速度
                    axisMedicineJarPostion.MoveAsync(5000);//旋緊藥罐位置
                    axisMedicineBottleEvelatorPostion.SetVelocity(3000, 10, 10);//藥罐下降速度
                    axisMedicineBottleEvelatorPostion.MoveAsync(1000);//藥罐下降位置
                }
                else if(axisScrewMedicineJarReady.IsInposition)//藥罐旋緊判斷
                    lowerClampMedicineCylinder.Switch(true);//藥罐下夾爪開

                else if(lowerClampMedicineCylinderOpenSignal.Signal)//藥罐下夾爪開訊號
                {
                    axisMedicineBottleEvelatorPostion.SetVelocity(3000, 10, 10);//藥罐上升速度
                    axisMedicineBottleEvelatorPostion.MoveAsync(0);//藥罐上升位置
                }

                else if(axisCarrierSlidePostion.IsInposition && redInkCylinderCylinderPullSignal.Signal)//判斷載台移動到位且紅墨水氣缸在拉位
                {
                    redInkCylinder.Switch(true);
                    await Task.Delay(3000);
                    redInkCylinder.Switch(false);

                }
            }

            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                // MyErrorHandler.HandleError(ex); // 使用自訂的錯誤處理機制
            }

        }

        //載體滑台移動至載體盒站
        public async Task MoveToCarrierBox()
        {
            axisCarrierSlidePostion.SetVelocity(6000, 1, 1);//載體滑台移動速度
            axisCarrierSlidePostion.MoveAsync(6000);//載體滑台移動至載體盒站
            await Task.Delay(1000);

        }

        //載體滑台移動至濾紙站
        public async Task MoveToFilterPaper()
        {
            axisCarrierSlidePostion.SetVelocity(8000, 1, 1);//載體滑台移動速度
            axisCarrierSlidePostion.MoveAsync(8000);//載體滑台移動至濾紙站
            await Task.Delay(1000);
        }

        //載體滑台移動至紅墨水站
        public async Task MoveToRedInk()
        {
            axisCarrierSlidePostion.SetVelocity(5000, 1, 1);//載體滑台移動速度
            axisCarrierSlidePostion.MoveAsync(5000);//載體滑台移動至濾紙站
            
        }



    }
}
