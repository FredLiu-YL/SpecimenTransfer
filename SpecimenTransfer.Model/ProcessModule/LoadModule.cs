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
        private DigitalOutput backLightCylinder;


        //----Digital Input----

        //載體盒 載體氣缸-推
        private DigitalIntput carrierCylinderPushSignal;
        //載體盒 載體氣缸-收
        private DigitalIntput carrierCylinderPullSignal;

        //濾紙盒 -推
        private DigitalIntput filterPaperBoxPushSignal;
        //濾紙盒 -收
        private DigitalIntput filterPaperBoxPullSignal;
       
        //濾紙在席
        private DigitalIntput filterPaperConfirm;


        //背光氣缸-推
        //   private DigitalIntput backLightCylinderPushSignal;
        //背光氣缸-收
        //   private DigitalIntput backLightCylinderPullSignal;


        /// <summary>
        /// 入料模組參數
        /// </summary>
        public IAxis FilterPaperElevatorAxis { get; set; }//濾紙升降軸
        public IAxis SlideTableAxis { get; set; }//載體滑台軸

        //----條碼----
        //載體盒條碼
        private IBarcodeReader carrierBottle;

        public LoadModuleParamer LoadModuleParam { get; set; } = new LoadModuleParamer();
        /// <summary>
        /// IO Input 濾紙盒 推訊號
        /// </summary>
        public DigitalIntput FilterPaperBoxPushSignal { get => filterPaperBoxPushSignal; }
        /// <summary>
        /// IO Input 濾紙盒 收訊號
        /// </summary>
        public DigitalIntput FilterPaperBoxPullSignal { get => filterPaperBoxPullSignal; }

            public LoadModule(DigitalOutput[] signalOutput, DigitalIntput[] signalInput, IAxis slideTableAxis,
            IAxis filterPaperElevatorAxis, IBarcodeReader paperReader)
        {
            //----Digital Output----
            shotCarrierBottleBarcode = signalOutput[0];//camera shot載體盒條碼

            carrierCassetteCylinder = signalOutput[2];//載體盒卡匣

            filterPaperBoxCylinder = signalOutput[3];//濾紙氣缸

            suctionFilterPaper = signalOutput[4];//吸濾紙

            backLightCylinder = signalOutput[10];//背光氣缸

            //----Digital Input----
            carrierCylinderPushSignal = signalInput[6];//載體盒 載體氣缸-推
            carrierCylinderPullSignal = signalInput[7];//載體盒 載體氣缸-收

            filterPaperBoxPushSignal = signalInput[8];//濾紙氣缸-推
            filterPaperBoxPullSignal = signalInput[9];//濾紙氣缸-收
            filterPaperConfirm = signalInput[28];//濾紙在席

            //backLightCylinderPushSignal = signalInput[18];//背光氣缸-推
            //backLightCylinderPushSignal = signalInput[19];//背光氣缸-收

            //----軸控----

            this.FilterPaperElevatorAxis = filterPaperElevatorAxis;//濾紙升降滑台 
            this.SlideTableAxis = slideTableAxis;//載體滑台 

            //----條碼----
            //medcineBottle = paperReader;//藥罐條碼
            carrierBottle = paperReader;//載體盒條碼

        }

        object monitorOBJ = new object();

        public async Task StartHome()
        {
            bool homeStatus = false;
            Task t1 = Task.Run(Home);
            t1.Wait();
            
        }

        /// <summary>
        /// 原點復歸
        /// </summary>
        /// <returns></returns>
        public async Task Home()
        {

            lock(monitorOBJ)
            {
                //濾紙真空關->濾紙升降軸home->濾紙氣缸收->載體盒卡匣收
                double slideAxisPos = SlideTableAxis.GetPosition();

                suctionFilterPaper.Switch(false);
                backLightCylinder.Switch(false);
                carrierCassetteCylinder.Switch(false);

                SlideTableAxis.Home();
                FilterPaperElevatorAxis.SetVelocity(100, 1, 1);
                FilterPaperElevatorAxis.Home();
                Thread.Sleep(25000);

                double filterPaperAxisPos = FilterPaperElevatorAxis.GetPosition();
                if (filterPaperAxisPos == 0)
                    FilterPaperElevatorAxis.MoveToAsync(73000);

                Thread.Sleep(5000);
                double filterPaperAxisStandPos = FilterPaperElevatorAxis.GetPosition();
                if (filterPaperAxisStandPos == 73000)
                    filterPaperBoxCylinder.Switch(false);
            }

        }

        /// <summary>
        /// 讀條碼
        /// </summary>
        /// <returns></returns>
        public async Task<string> ReadBarcode()
        {
            string carrierDataReceived;

            try
            {
                //await Task.Delay(1000);
                shotCarrierBottleBarcode.Switch(true);
                shotCarrierBottleBarcode.Switch(false);
                carrierDataReceived = carrierBottle.ReceiveData();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally 
            {
                shotCarrierBottleBarcode.Switch(false);
            }

            return carrierDataReceived;
        }

        /// <summary>
        /// 從卡匣進一片載體盒到移載平台
        /// </summary>
        /// <param name="cassetteIndex"></param>
        /// <returns></returns>
        public async Task<int> LoadBoxAsync(int cassetteIndex)
        {
            int countCassette = 0;

            //載體滑台移動到卡匣站->載體盒推->載體盒收
            await MoveToCBoxCassette();

            Thread.Sleep(3000);
            double nowPos = SlideTableAxis.GetPosition();
            if(nowPos == 592350)
            {
                
                carrierCassetteCylinder.Switch(true);
                Thread.Sleep(2000);
                carrierCassetteCylinder.Switch(false);
            }
            Thread.Sleep(1000);

            SlideTableAxis.MoveToAsync(563920);

            countCassette++;

            return countCassette;
            //WaitInputSignal(carrierCylinderPushSignal);

        }


        //濾紙放到載體盒
        public async Task PuttheFilterpaperInBox()
        {
           
            try
            {
                //濾紙盒推->濾紙升降滑台目標位->吸濾紙->濾紙升降滑台待命位->濾紙盒收 

                //濾紙氣缸推
                double nowPos = FilterPaperElevatorAxis.GetPosition();
                if (nowPos >= 68000)
                    filterPaperBoxCylinder.Switch(true);

                //濾紙軸下降
                Thread.Sleep(1000);
                //FilterPaperElevatorAxis.MoveToAsync(LoadModuleParam.FilterPaperElevatorTargetPos);
                FilterPaperElevatorAxis.MoveToAsync(20000);

                //if(FilterPaperElevatorAxis.IsInposition)
                //吸濾紙開
                Thread.Sleep(2000);
                double nowPos1 = FilterPaperElevatorAxis.GetPosition();
                if (nowPos1 == 20000)
                    suctionFilterPaper.Switch(true);


                //FilterPaperElevatorAxis.MoveToAsync(LoadModuleParam.FilterPaperElevatorStandByPos);
                //濾紙軸上升待命位
                Thread.Sleep(1000);
                double nowPos2 = FilterPaperElevatorAxis.GetPosition();
                if (nowPos2 == 20000)
                    FilterPaperElevatorAxis.MoveToAsync(73000);

                //濾紙氣缸收
                Thread.Sleep(2000);
                double nowPos3 = FilterPaperElevatorAxis.GetPosition();
                if (nowPos3 == 73000)
                filterPaperBoxCylinder.Switch(false);

                //濾紙軸下降放濾紙
                Thread.Sleep(2000);
                double nowPos4 = FilterPaperElevatorAxis.GetPosition();
                if (nowPos4 == 73000)
                    FilterPaperElevatorAxis.MoveToAsync(0);

                //吸濾紙關
                Thread.Sleep(2000);
                double nowPos5 = FilterPaperElevatorAxis.GetPosition();
                if (nowPos5 == 0)
                    suctionFilterPaper.Switch(false);

                //濾紙軸上升待命位
                Thread.Sleep(1000);
                double nowPos6 = FilterPaperElevatorAxis.GetPosition();
                if (nowPos6 == 0)
                    FilterPaperElevatorAxis.MoveToAsync(73000);


                //WaitInputSignal(filterPaperBoxPushSignal);
                //WaitAxisSignal(FilterPaperElevatorAxis.IsInposition);
                //WaitInputSignal(filterPaperConfirm);
                //WaitInputSignal(filterPaperBoxPullSignal);
                //WaitAxisSignal(FilterPaperElevatorAxis.IsInposition);
            }
            catch (Exception error)
            {

              error.ToString();
            }

        }


        //載體滑台移動至載體盒站
        public async Task MoveToCBoxCassette()
        {
            SlideTableAxis.MoveToAsync(592350);
            await Task.Delay(500);

        }

        //載體滑台移動至濾紙站
        public async Task MoveToFilterPaper()
        {
            //SlideTableAxis.MoveAsync(LoadModuleParam.SlideTablePaperPos);
            SlideTableAxis.MoveToAsync(521680);
            await Task.Delay(500);
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

        internal Task LoadAsync(int v)
        {
            throw new NotImplementedException();
        }

    }

    public class LoadModuleParamer
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
        /// 濾紙升降軸 待命位
        /// </summary>
        public double FilterPaperElevatorStandByPos { get; set; }

        /// <summary>
        /// 濾紙升降軸 最高位
        /// </summary>
        public double FilterPaperElevatorHighPos { get; set; }
        /// <summary>
        /// 濾紙升降軸 最低位
        /// </summary>
        public double FilterPaperElevatorLowPos { get; set; }

        /// <summary>
        /// 濾紙升降軸 起始格數
        /// </summary>
        public int FilterPaperElevatorStartIndex { get; set; }

        /// <summary>
        /// 濾紙升降軸 目標位
        /// </summary>
        public double FilterPaperElevatorTargetPos { get; set; }


    }


}
