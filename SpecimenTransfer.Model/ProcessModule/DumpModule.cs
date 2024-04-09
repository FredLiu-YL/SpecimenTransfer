using SpecimenTransfer.Model.Component;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SpecimenTransfer.Model
{
    public class DumpModule
    {


        #region Digital Output   

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
        #endregion

        #region DigitalInput    

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
        #endregion
        


        //----條碼----
        //藥罐條碼
        private IBarcodeReader medcineBottleBarcode;


        /// <summary>
        /// 入料模組參數
        /// </summary>
        public DumpModuleParamer DumpModuleParam { get; set; } = new DumpModuleParamer();

        public DumpModule(DigitalOutput[] signalOutput, DigitalIntput[] signalInput,
            IAxis slideTableAxis, IAxis bottleElevatorAxis, IAxis bottleScrewAxis, IAxis bottleDumpAxis, IBarcodeReader bottleReader)
        {
            //----Digital Output----
            injectionCleanCylinder = signalOutput[5];//注射清洗氣缸

            injectionCleanSwitch = signalOutput[6];//注射清洗

            upperClampMedicineCylinder = signalOutput[7];//藥罐瓶蓋氣缸-上夾爪

            lowerClampMedicineCylinder = signalOutput[8];//藥罐瓶蓋氣缸-下夾爪

            medicineBottleMoveCylinder = signalOutput[9];//藥罐移載氣缸

            //cameraShot = signalOutput[10];//camera拍照

            backLightCylinder = signalOutput[10];//背光氣缸
            injectRedInk = signalOutput[11];//注射紅墨水
            redInkCylinder = signalOutput[12];//紅墨水氣缸

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
            this.SlideTableAxis = slideTableAxis;//載體滑台
            this.BottleElevatorAxis = bottleElevatorAxis;//藥罐升降滑台
            this.BottleScrewAxis = bottleScrewAxis;//旋藥蓋
            this.BottleDumpAxis = bottleDumpAxis;//藥罐傾倒

            //Barcode reader
            //藥罐條碼
            medcineBottleBarcode = bottleReader;

        }

        public Action SetupJar;

        //載體滑台
        public IAxis SlideTableAxis { get; set; }
        //藥罐升降滑台-home
        public IAxis BottleElevatorAxis { get; set; }
        //旋藥蓋
        public IAxis BottleScrewAxis { get; set; }
        //藥瓶傾倒
        public IAxis BottleDumpAxis { get; set; }
        //藥瓶傾倒
       

        public async Task Load()
        {
            //目前由人完成藥罐的載入 先委派出去
            SetupJar.Invoke();

        }
        
        //原點復歸
        public async Task Home()
        {
            //藥罐移載氣缸收->上夾爪開->下夾爪開->背光氣缸收->藥罐升降滑台home->旋藥蓋home->藥罐傾倒home
            medicineBottleMoveCylinder.Switch(false);
            upperClampMedicineCylinder.Switch(false);
            lowerClampMedicineCylinder.Switch(false);
            backLightCylinder.Switch(false);
            BottleElevatorAxis.Home();
            BottleScrewAxis.Home();
            BottleDumpAxis.Home();
           
        }


        //旋開藥罐
        public async Task UnscrewMedicineJar()
        {
            try
            {
                //藥罐上夾爪關閉->旋開藥罐
                upperClampMedicineCylinder.Switch(true);
                WaitInputSignal(upperClampMedicineCylinderCloseSignal);
                BottleScrewAxis.MoveAsync(DumpModuleParam.BottleDumpUnScrewStandbyPos);
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 旋緊藥蓋
        /// </summary>
        /// <returns></returns>
        public async Task ScrewMedicineJar()
        {

            try
            {

                //藥罐上夾爪關閉->旋緊藥蓋位置
                upperClampMedicineCylinder.Switch(true);
                BottleScrewAxis.MoveAsync(DumpModuleParam.BottleDumpScrewPos);
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 檢查藥罐
        /// </summary>
        /// <returns></returns>
        public async Task<bool> CheckBottleAction()
        {
            try
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

                return true;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        //藥罐升降待命位
        public async Task BottleElevatorStandBy()
        {
            try
            {
                //藥罐上夾爪打開->藥罐回待命位置
                upperClampMedicineCylinder.Switch(false);
                WaitInputSignal(upperClampMedicineCylinderOpenSignal);
                BottleElevatorAxis.MoveAsync(DumpModuleParam.BottleElevatorStandbyPos);
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        //藥罐下降
        public async Task BottleElevatorDown()
        {
            try
            {
                //藥罐下夾爪關閉->藥罐到下降位置
                lowerClampMedicineCylinder.Switch(true);
                WaitInputSignal(lowerClampMedicineCylinderOpenSignal);
                BottleElevatorAxis.MoveAsync(DumpModuleParam.BottleElevatorPos);
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }


        /// <summary>
        /// 傾倒藥罐
        /// </summary>
        /// <returns></returns>
        public async Task DumpBottle()
        {

            try
            {
                //藥罐下夾爪關閉->傾倒藥罐位置
                lowerClampMedicineCylinder.Switch(true);
                WaitInputSignal(lowerClampMedicineCylinderOpenSignal);
                BottleDumpAxis.MoveAsync(DumpModuleParam.BottleDumpPos);

            }

            catch (Exception ex)
            {
                throw ex;

            }

        }

        /// <summary>
        /// 藥罐傾倒待命位
        /// </summary>
        /// <returns></returns>
        public async Task BottleDumpStandbyPos()
        {

            try
            {
                //藥罐下夾爪關閉->傾倒藥罐回待命位
                lowerClampMedicineCylinder.Switch(true);
                WaitInputSignal(lowerClampMedicineCylinderOpenSignal);
                BottleDumpAxis.MoveAsync(DumpModuleParam.BottleDumpStandbyPos);
            }

            catch (Exception ex)
            {
                throw ex;

            }

        }


        /// <summary>
        /// 清洗藥罐
        /// </summary>
        /// <returns></returns>
        public async Task CleanBottle()
        {
            //藥罐傾倒軸到位ready->清洗藥罐->計時->停止清洗
            try
            {
                await CarrierMoveToClean();
                injectionCleanSwitch.Switch(true);
                await Task.Delay(3000);
                injectionCleanSwitch.Switch(false);
            }

            catch (Exception ex)
            {
                throw ex;

            }

        }

        /// <summary>
        /// 注入紅墨水
        /// </summary>
        /// <returns></returns>
        public async Task InjectRedInk()
        {
            try
            {
                //載體盒到位->紅墨水氣缸推->注射紅墨水->紅墨水氣缸收
                await CarrierMoveToRedInk();
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
        public async Task CarrierMoveToClean()
        {
            try
            {
                //載體滑台移動至清洗站
                SlideTableAxis.MoveAsync(DumpModuleParam.CarrierTableBottleCleanPos);
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        //載體盒移動至紅墨水站
        public async Task CarrierMoveToRedInk()
        {
            try
            {
                //載體滑台移動至紅墨水站
                SlideTableAxis.MoveAsync(DumpModuleParam.CarrierTableRedInkPos);
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        
        //讀藥罐條碼
        public async Task<string> ReadBarcode()
        {

                //藥罐旋轉->camera trigger->延時->接收資料->延時->讀條碼關->回傳資料
                BottleScrewAxis.MoveToAsync(8000);
                shotMedcineBottleBarcode.Switch(true);
                await Task.Delay(3000);
                string carrierDataReceived = medcineBottleBarcode.ReceiveData();

                try
                {
                    if (carrierDataReceived != null)
                    {
                        shotMedcineBottleBarcode.Switch(false);
                    }
                    else
                    {
                        BottleScrewAxis.MoveToAsync(-8000);
                        shotMedcineBottleBarcode.Switch(true);
                    }
                }

                catch (Exception ex)
                {
                    throw ex;
                }

                return carrierDataReceived;

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


      
    }

    public class DumpModuleParamer
    {
        //藥罐傾倒位置座標
        public double BottleDumpPos { get; set; }

        //藥罐傾倒待命座標
        public double BottleDumpStandbyPos { get; set; }


        //藥蓋旋緊到位座標
        public double BottleDumpScrewPos { get; set; }

        //藥蓋旋開待命座標
        public double BottleDumpUnScrewStandbyPos { get; set; }


        //藥罐升降位置座標
        public double BottleElevatorPos { get; set; }

        //藥罐升降待命座標
        public double BottleElevatorStandbyPos { get; set; }


        //橫移軸在傾倒載體座標
        public double CarrierTableBottleDumpPos { get; set; }
        public double CarrierTableBottleCleanPos { get; set; }
        //橫移軸在紅墨水座標
        public double CarrierTableRedInkPos { get; set; }


    }








}



