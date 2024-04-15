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
        
       
        //人工放藥罐
        public async Task Load()
        {
            //目前由人完成藥罐的載入 先委派出去
            SetupJar.Invoke();

        }
        
        //原點復歸
        public async Task Home()
        {
            
            try
            {
                //藥罐移載氣缸收->上夾爪開->下夾爪開->背光氣缸收->藥罐升降滑台home->旋藥蓋home->
                //藥罐傾倒home->藥罐位置在待命位->載台home
                medicineBottleMoveCylinder.Switch(false);
                upperClampMedicineCylinder.Switch(false);
                lowerClampMedicineCylinder.Switch(false);
                backLightCylinder.Switch(false);
                BottleElevatorAxis.Home();
                BottleScrewAxis.Home();
                BottleDumpAxis.Home();

                if (BottleElevatorAxis.Position == 0)
                    SlideTableAxis.Home();
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>
        /// 旋開藥罐
        /// </summary>
        /// <returns></returns>
        public async Task UnscrewMedicineJar()
        {
            try
            {
                //藥罐上夾爪夾->藥罐下夾爪夾->旋開藥蓋->藥蓋上升待命點
                upperClampMedicineCylinder.Switch(true);
                WaitInputSignal(upperClampMedicineCylinderCloseSignal);

                lowerClampMedicineCylinder.Switch(true);
                WaitInputSignal(lowerClampMedicineCylinderCloseSignal);

                BottleScrewAxis.MoveAsync(DumpModuleParam.BottleScrewStandbyPos);
                
                BottleElevatorAxis.MoveAsync(DumpModuleParam.BottleElevatorStandbyPos);

                WaitAxisSignal(BottleScrewAxis.IsInposition);
                WaitAxisSignal(BottleElevatorAxis.IsInposition);

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
                //藥罐上夾爪夾->藥罐下夾爪夾->旋緊藥蓋->藥罐下降目標位置
                upperClampMedicineCylinder.Switch(true);
                WaitInputSignal(upperClampMedicineCylinderCloseSignal);

                lowerClampMedicineCylinder.Switch(true);
                WaitInputSignal(lowerClampMedicineCylinderCloseSignal);

                BottleScrewAxis.MoveAsync(DumpModuleParam.BottleScrewTargetPos);
                BottleElevatorAxis.MoveAsync(DumpModuleParam.BottleElevatorScanPos);
                
                WaitAxisSignal(BottleScrewAxis.IsInposition);
                WaitAxisSignal(BottleElevatorAxis.IsInposition);
                

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
                // 藥罐移載氣缸收->載體盒到位->傾倒藥罐
                medicineBottleMoveCylinder.Switch(false);
                WaitInputSignal(medicineBottleMoveCylinderPullSignal);

                await CarrierMoveToDump();

                BottleDumpAxis.MoveAsync(DumpModuleParam.BottleDumpTargetPos);
                WaitAxisSignal(BottleDumpAxis.IsInposition);

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
            //洗藥罐->計時->停止清洗->藥罐待命位
            try
            {
                injectionCleanSwitch.Switch(true);
                await Task.Delay(3000);
                injectionCleanSwitch.Switch(false);

                BottleDumpAxis.MoveAsync(DumpModuleParam.BottleDumpStandbyPos);
                WaitAxisSignal(BottleDumpAxis.IsInposition);
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
                //藥罐移載氣缸推->背光氣缸推->拍照->藥罐移載氣缸收->背光氣缸收
                medicineBottleMoveCylinder.Switch(true);
                backLightCylinder.Switch(true);

                WaitInputSignal(medicineBottleMoveCylinderPushSignal);
                WaitInputSignal(backLightCylinderPushSignal);

                cameraShot.Switch(true);
                await Task.Delay(500);
                cameraShot.Switch(false);

                medicineBottleMoveCylinder.Switch(false);
                backLightCylinder.Switch(false);

                WaitInputSignal(medicineBottleMoveCylinderPullSignal);
                WaitInputSignal(backLightCylinderPullSignal);

                return true;
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

                redInkCylinder.Switch(false);
                WaitInputSignal(redInkCylinderPullSignal);

            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        //載體盒移動至清洗站
        public async Task CarrierMoveToDump()
        {
            try
            {
                //載體滑台移動至清洗站
                SlideTableAxis.MoveAsync(DumpModuleParam.SlideTableDumpPos);
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
                SlideTableAxis.MoveAsync(DumpModuleParam.SlideTableInkPos);
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

                private void WaitAxisSignal(bool isInposition)
                 {
                     try
                        {
                          SpinWait.SpinUntil(() => isInposition);
                        }
                     catch (Exception ex)
                        {

                          throw ex;
                        }
                }

    }

    public class DumpModuleParamer
    {
        /// <summary>
        /// 移載橫移軸 Jog移動量
        /// </summary>
        public double SlideTableJogDiatance { get; set; }
        /// <summary>
        /// 移載橫移軸 移動速度
        /// </summary>
        public double SlideTableSpeed { get; set; }

        /// <summary>
        /// 移載橫移軸 待命位
        /// </summary>
        public double SlideTableStandByPos { get; set; }

        /// <summary>
        /// 移載橫移軸 清洗位
        /// </summary>
        public double SlideTableCleanPos { get; set; }
        /// <summary>
        /// 移載橫移軸 傾倒位
        /// </summary>
        public double SlideTableDumpPos { get; set; }
        /// <summary>
        /// 移載橫移軸 墨水位
        /// </summary>
        public double SlideTableInkPos { get; set; }

        /// <summary>
        /// 瓶罐升降軸 Jog移動量
        /// </summary>
        public double BottleElevatorJogDiatance { get; set; }
        /// <summary>
        /// 瓶罐升降軸 移動速度
        /// </summary>
        public double BottleElevatorSpeed { get; set; }
        /// <summary>
        /// 瓶罐升降軸 旋轉中速度
        /// </summary>
        public double BottleElevatorScrewSpeed { get; set; }

        /// <summary>
        /// 瓶罐升降軸 待命位
        /// </summary>
        public double BottleElevatorStandbyPos { get; set; }

        /// <summary>
        /// 瓶罐升降軸 掃描位
        /// </summary>
        public double BottleElevatorScanPos { get; set; }
        /// <summary>
        /// 瓶罐升降軸 旋轉起點位
        /// </summary>
        public double BottleElevatorScrewStartPos { get; set; }
        /// <summary>
        /// 瓶罐升降軸 旋轉終點位
        /// </summary>
        public double BottleElevatorScrewTargetPos { get; set; }


        /// <summary>
        /// 瓶蓋旋轉軸 Jog移動量
        /// </summary>
        public double BottleScrewJogDiatance { get; set; }
        /// <summary>
        /// 瓶蓋旋轉軸 移動速度
        /// </summary>
        public double BottleScrewSpeed { get; set; }

        /// <summary>
        /// 瓶蓋旋轉軸 待命位
        /// </summary>
        public double BottleScrewStandbyPos { get; set; }

        /// <summary>
        /// 瓶蓋旋轉軸 目標位
        /// </summary>
        public double BottleScrewTargetPos { get; set; }


        /// <summary>
        /// 瓶蓋傾倒軸 Jog移動量
        /// </summary>
        public double BottleDumpJogDiatance { get; set; }
        /// <summary>
        /// 瓶蓋傾倒軸 移動速度
        /// </summary>
        public double BottleDumpSpeed { get; set; }

        /// <summary>
        /// 瓶蓋傾倒軸 待命位
        /// </summary>
        public double BottleDumpStandbyPos { get; set; }

        /// <summary>
        /// 瓶蓋傾倒軸 起點位
        /// </summary>
        public double BottleDumpStartPos { get; set; }
        /// <summary>
        /// 瓶蓋旋轉軸 目標位
        /// </summary>
        public double BottleDumpTargetPos { get; set; }




    }








}



