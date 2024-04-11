using SpecimenTransfer.Model.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace SpecimenTransfer.Model
{
    public class LoadModule
    {

        //----Digital Output----

        //camera shot載體盒條碼
        private DigitalOutput shotCarrierBottleBarcode;

        //camera shot藥罐條碼
        private DigitalOutput shotMedcineBottleBarcode;

        //卡匣推送載體盒汽缸-推收
        private DigitalOutput carrierCassetteCylinder;

        //濾紙盒汽缸-推收
        private DigitalOutput filterPaperBoxCylinder;

        //吸濾紙
        private DigitalOutput suctionFilterPaper;

        //上方夾藥罐氣缸-打開關閉
        //    private DigitalOutput upperClampMedicineCylinder;

        //下方夾藥罐氣缸-打開關閉
        //    private DigitalOutput lowerClampMedicineCylinder;

        //背光氣缸-推收
        //    private DigitalOutput backLightCylinder;


        //----Digital Input----

        //載體盒 載體氣缸-推
        private DigitalIntput carrierCylinderPushSignal;
        //載體盒 載體氣缸-收
        private DigitalIntput carrierCylinderPullSignal;

        //濾紙盒 -推
        private DigitalIntput filterPaperBoxPushSignal;
        //濾紙盒 -收
        private DigitalIntput filterPaperBoxPullSignal;
        private DigitalIntput filterPaperVaccumSignal;

        internal Task LoadAsync(int v)
        {
            throw new NotImplementedException();
        }


        //背光氣缸-推
        //   private DigitalIntput backLightCylinderPushSignal;
        //背光氣缸-收
        //   private DigitalIntput backLightCylinderPullSignal;



        //----軸控----



        //----條碼----
        //藥罐條碼
        private IBarcodeReader medcineBottle;
        //載體盒條碼
        private IBarcodeReader carrierBottle;

        public LoadModuleParamer LoadModuleParam { get; set; } = new LoadModuleParamer();

        public LoadModule(DigitalOutput[] signalOutput, DigitalIntput[] signalInput, IAxis slideTableAxis,
            IAxis filterPaperElevatorAxis, IBarcodeReader paperReader)
        {
            //----Digital Output----
            shotCarrierBottleBarcode = signalOutput[0];//camera shot載體盒條碼

            shotMedcineBottleBarcode = signalOutput[1];//camera shot藥瓶條碼

            carrierCassetteCylinder = signalOutput[2];//載體盒卡匣

            filterPaperBoxCylinder = signalOutput[3];//濾紙氣缸

            suctionFilterPaper = signalOutput[4];//吸濾紙



            //       backLightCylinder = signalOutput[10];

            //----Digital Input----
            carrierCylinderPushSignal = signalInput[6];//載體盒 載體氣缸-推
            carrierCylinderPullSignal = signalInput[7];//載體盒 載體氣缸-收

            filterPaperBoxPushSignal = signalInput[8];//濾紙氣缸-推
            filterPaperBoxPullSignal = signalInput[9];//濾紙氣缸-收

            //        backLightCylinderPushSignal = signalInput[18];//背光氣缸-推
            //        backLightCylinderPushSignal = signalInput[19];//背光氣缸-收


            //----軸控----

            this.FilterPaperElevatorAxis = filterPaperElevatorAxis;//抓取濾紙 
            this.SlideTableAxis = slideTableAxis;//載體滑台 


            //----條碼----
            medcineBottle = paperReader;//藥罐條碼
            carrierBottle = paperReader;//載體盒條碼

        }

        /// <summary>
        /// 入料模組參數
        /// </summary>


        //抓取濾紙toyo升降滑台
        public IAxis FilterPaperElevatorAxis { get; set; }
        //載體滑台軸
        public IAxis SlideTableAxis { get; set; }


        //原點復歸
        public async Task Home()
        {
            //卡匣推送載體盒汽缸收->濾紙真空關->抓取濾紙升降軸home->濾紙盒汽缸收
            carrierCassetteCylinder.Switch(false);
            suctionFilterPaper.Switch(false);
            FilterPaperElevatorAxis.Home();
            filterPaperBoxCylinder.Switch(false);

        }


        /// <summary>
        /// 從卡匣進一片載體盒到移載平台
        /// </summary>
        /// <param name="cassetteIndex"></param>
        /// <returns></returns>
        public async Task LoadBoxAsync(int cassetteIndex)
        {

            await MoveToCBoxCassette();

            //電動缸推出載體盒再收回
            carrierCassetteCylinder.Switch(true);
            await Task.Delay(1000);

            carrierCassetteCylinder.Switch(false);

        }
        public async Task<string> ReadBarcode()
        {
            //載體盒氣缸推->camera trigger->延時->接收資料->延時->讀條碼關->回傳資料
            carrierCassetteCylinder.Switch(true);
            WaitInputSignal(carrierCylinderPushSignal);
            shotCarrierBottleBarcode.Switch(true);
            await Task.Delay(3000);
            string carrierDataReceived = carrierBottle.ReceiveData();
            await Task.Delay(500);

            try
            {
                if (carrierDataReceived != null)
                    shotCarrierBottleBarcode.Switch(false);

            }

            catch (Exception ex)
            {
                throw ex;
            }

            return carrierDataReceived;
        }

        private void WaitInputSignal(bool signal)
        {
            throw new NotImplementedException();

        }

        //濾紙放到載體盒
        public async Task PuttheFilterpaperInBox()
        {

            try
            {
                //濾紙盒推->濾紙吸嘴下->吸濾紙->濾紙吸嘴上->濾紙盒收->移載平台移到下方承接->濾紙吸嘴下->放濾紙-濾紙吸嘴上

                filterPaperBoxCylinder.Switch(true);//濾紙盒氣缸 推
                WaitInputSignal(filterPaperBoxPushSignal);
                FilterPaperElevatorAxis.MoveToAsync(LoadModuleParam.FilterPaperElevatorHighPos);
                suctionFilterPaper.Switch(true);//吸濾紙
                WaitInputSignal(filterPaperVaccumSignal);
                FilterPaperElevatorAxis.MoveToAsync(LoadModuleParam.FilterPaperElevatorTargetPos);
                filterPaperBoxCylinder.Switch(false);//濾紙盒氣缸 -收
                WaitInputSignal(filterPaperBoxPullSignal);

                //放到移載平台
                FilterPaperElevatorAxis.MoveToAsync(LoadModuleParam.FilterPaperElevatorHighPos);//移動到濾紙下方
                suctionFilterPaper.Switch(false);//放濾紙
                WaitInputSignal(filterPaperVaccumSignal);
                FilterPaperElevatorAxis.MoveToAsync(LoadModuleParam.FilterPaperElevatorTargetPos);

            }
            catch (Exception)
            {

                throw;
            }

        }


        //載體滑台移動至載體盒站
        public async Task MoveToCBoxCassette()
        {
            SlideTableAxis.MoveAsync(LoadModuleParam.SlideTableLoadPos);//載體滑台移動至載體盒站
            await Task.Delay(500);

        }

        //載體滑台移動至濾紙站
        public async Task MoveToFilterPaper()
        {
            SlideTableAxis.MoveAsync(LoadModuleParam.SlideTablePaperPos);//載體滑台移動至濾紙站
            await Task.Delay(500);
        }
        //載體盒移動至傾倒站
        public async Task MoveToDump()
        {
            //   SlideTableAxis.MoveAsync(LoadModuleParam.SlideTableDumpPos);//載體滑台移動至注射站

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

    public class LoadModuleParamer
    {
        /// <summary>
        /// 移載橫移軸 入料位
        /// </summary>
        public double SlideTableLoadPos { get; set; }
        /// <summary>
        /// 移載橫移軸 濾紙位
        /// </summary>
        public double SlideTablePaperPos { get; set; }

        /// <summary>
        /// 濾紙升降軸 Jog移動量
        /// </summary>
        public double FilterPaperElevatorJogDiatance { get; set; }
        /// <summary>
        /// 濾紙升降軸 速度
        /// </summary>
        public double FilterPaperElevatorSpeed { get; set; }
        /// <summary>
        /// 濾紙升降軸 最高位
        /// </summary>
        public double FilterPaperElevatorHighPos { get; set; }
        /// <summary>
        /// 濾紙升降軸 最低位
        /// </summary>
        public double FilterPaperElevatorLowPos { get; set; }
        /// <summary>
        /// 濾紙升降軸 目標位
        /// </summary>
        public double FilterPaperElevatorTargetPos { get; set; }





    }
}
