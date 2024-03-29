using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsFormsApp3.Component;

namespace WindowsFormsApp3.Initial_Model
{
    public class LoadModule
    {

        //----Digital Output----

        //camera shot載體盒條碼
        private DigitalOutput shotCarrierBottleBarcode;

        //camera shot藥罐條碼
        private DigitalOutput shotMedcineBottleBarcode;

        //載體盒 載體氣缸-推收
        private DigitalOutput carrierCassetteCylinder;

        //濾紙盒汽缸-推收
        private DigitalOutput filterPaperBoxCylinder;

        //抓取濾紙toyo升降滑台-升
        private IAxis catchFilterPaperTableUpAxis;
        //抓取濾紙toyo升降滑台-降
        private IAxis catchFilterPaperTablerDownAxis;

        //吸濾紙
        private DigitalOutput suctionFilterPaper;

        //上方夾藥罐氣缸-打開關閉
        private DigitalOutput upperClampMedicineCylinder;

        //下方夾藥罐氣缸-打開關閉
        private DigitalOutput lowerClampMedicineCylinder;

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
        private DigitalIntput filterPaperVaccumSignal;


        //背光氣缸-推
        private DigitalIntput backLightCylinderPushSignal;
        //背光氣缸-收
        private DigitalIntput backLightCylinderPullSignal;

        //上方夾藥罐氣缸-打開訊號
        private DigitalIntput upperClampMedicineCylinderOpenSignal;
        //上方夾藥罐氣缸-關閉訊號
        private DigitalIntput upperClampMedicineCylinderCloseSignal;

        //下方夾藥罐氣缸-打開訊號
        private DigitalIntput lowerClampMedicineCylinderOpenSignal;
        //下方夾藥罐氣缸-關閉訊號
        private DigitalIntput lowerClampMedicineCylinderCloseSignal;


        //----軸控----

        //抓取濾紙軸
        private IAxis catchFilterPaperAxis;
        //載體滑台軸
        private IAxis carrierSlideTableAxis;



        //----條碼----
        //藥罐條碼
        private IBarcodeReader medcineBottle;
        //載體盒條碼
        private IBarcodeReader carrierBottle;



        public LoadModule(DigitalOutput[] signalOutput, DigitalIntput[] signalInput, IAxis carrierSlideTableAxis, IAxis catchFilterPaperAxis, IBarcodeReader barcodeReader, IElectricCylinder loadPushBoxCylinder)
        {
            //----Digital Output----
            shotCarrierBottleBarcode = signalOutput[0];//camera shot載體盒條碼

            shotMedcineBottleBarcode = signalOutput[1];//camera shot藥瓶條碼

            carrierCassetteCylinder = signalOutput[2];//載體氣缸

            filterPaperBoxCylinder = signalOutput[3];//濾紙氣缸

            suctionFilterPaper = signalOutput[4];//吸濾紙

            upperClampMedicineCylinder = signalOutput[7];//藥罐瓶蓋氣缸-上夾爪

            lowerClampMedicineCylinder = signalOutput[8];//藥罐瓶蓋氣缸-下夾爪

            backLightCylinder = signalOutput[10];

            //----Digital Input----
            carrierCylinderPushSignal = signalInput[6];//載體盒 載體氣缸-推
            carrierCylinderPullSignal = signalInput[7];//載體盒 載體氣缸-收

            filterPaperBoxPushSignal = signalInput[8];//濾紙氣缸-推
            filterPaperBoxPullSignal = signalInput[9];//濾紙氣缸-收

            backLightCylinderPushSignal = signalInput[18];//背光氣缸-推
            backLightCylinderPushSignal = signalInput[19];//背光氣缸-收

            upperClampMedicineCylinderCloseSignal = signalInput[14];//藥罐瓶蓋上夾爪-關
            upperClampMedicineCylinderOpenSignal = signalInput[15];//藥罐瓶蓋上夾爪-開

            lowerClampMedicineCylinderCloseSignal = signalInput[16]; //藥罐瓶蓋下夾爪-關
            lowerClampMedicineCylinderOpenSignal = signalInput[17]; //藥罐瓶蓋下夾爪-開

            //----軸控----

            this.catchFilterPaperAxis = catchFilterPaperAxis;//抓取濾紙 
            this.carrierSlideTableAxis = carrierSlideTableAxis;//載體滑台 


            //----條碼----
            medcineBottle = barcodeReader;//藥罐條碼
            carrierBottle = barcodeReader;//載體盒條碼

        }

        /// <summary>
        /// 入料模組參數
        /// </summary>
        public LoadModuleParamer LoadModuleParam { get; set; } = new LoadModuleParamer();



        public async Task Home()
        {


        }

        //BarcodeComparison
        /* public async Task BarcodeComparison()
         {
             int count = 2;
             //拍照
             try
             {
                 shotCarrierBottleBarcode.Switch(true);
                 shotMedcineBottleBarcode.Switch(true);
             }
             catch (Exception ex)
             {
                 Console.Error.WriteLine(ex.Message);
                 // MyErrorHandler.HandleError(ex); // 使用自訂的錯誤處理機制
             }

             //接收條碼
             string medcineDataReceived = medcineBottle.ReceiveData();
             string carrierDataReceived = carrierBottle.ReceiveData();
             await Task.Delay(500);

             //重複拍照檢測3次，比對條碼內容
             for (int i = 0; i <= count; i++)
             {
                 if (medcineDataReceived == carrierDataReceived && medcineBottle != null
                     && carrierBottle != null)//藥罐和載體盒條碼比對，藥換和載體盒條碼不為空
                 {
                     Console.WriteLine("條碼比對成功");
                     break;
                 }
                 else
                 {
                     shotCarrierBottleBarcode.Switch(true);
                     shotMedcineBottleBarcode.Switch(true);
                     count++;
                 }

             }
         }*/



        /// <summary>
        /// 從卡匣進一片載體盒到移載平台
        /// </summary>
        /// <param name="cassetteIndex"></param>
        /// <returns></returns>
        public async Task LoadAsync(int cassetteIndex)
        {

            await MoveToCBoxCassette();

            //電動缸推出載體盒再收回
            carrierCassetteCylinder.Switch(true);
            await Task.Delay(1000);


            carrierCassetteCylinder.Switch(false);



        }
        public async Task<string> ReadBarcode()
        {

            shotCarrierBottleBarcode.Switch(true);
            await Task.Delay(500);
            string carrierDataReceived = carrierBottle.ReceiveData();
            await Task.Delay(500);
            shotCarrierBottleBarcode.Switch(false);
            return carrierDataReceived;
        }
        //濾紙放到載體盒
        public async Task PuttheFilterpaperInBox()
        {

            try
            {
                //濾紙盒推->濾紙吸嘴下->吸濾紙->濾紙吸嘴上->濾紙盒收->移載平台移到下方承接->濾紙吸嘴下->放濾紙-濾紙吸嘴上

                filterPaperBoxCylinder.Switch(true);//濾紙盒氣缸 推
                WaitInputSignal(filterPaperBoxPushSignal);
                catchFilterPaperAxis.MoveToAsync(LoadModuleParam.FilterPaperCatchPos);
                suctionFilterPaper.Switch(true);//吸濾紙
                WaitInputSignal(filterPaperVaccumSignal);
                catchFilterPaperAxis.MoveToAsync(LoadModuleParam.FilterPaperBackPos);
                filterPaperBoxCylinder.Switch(false);//濾紙盒氣缸 -收
                WaitInputSignal(filterPaperBoxPullSignal);

                //放到移載平台
                catchFilterPaperAxis.MoveToAsync(LoadModuleParam.FilterPaperCatchPos);//移動到濾紙下方
                suctionFilterPaper.Switch(false);//放濾紙
                WaitInputSignal(filterPaperVaccumSignal);
                catchFilterPaperAxis.MoveToAsync(LoadModuleParam.FilterPaperBackPos);

            }
            catch (Exception)
            {

                throw;
            }

        }

        //載體站/濾紙站動作流程
        public async Task PushCarrierBoxAndClampFilterPaper()
        {

            try
            {
                if (carrierCylinderPullSignal.Signal)//判斷載體盒氣缸是否在收位 && carrierSlideTableAxis到位訊號
                    carrierCassetteCylinder.Switch(true);//載體盒氣缸推

                else if (filterPaperBoxPullSignal.Signal)//判斷濾紙氣缸是否在收位
                    filterPaperBoxCylinder.Switch(true);//濾紙氣缸推

                else if (carrierCylinderPullSignal.Signal)//判斷載體盒氣缸是否在推位
                {
                    carrierCassetteCylinder.Switch(true);
                    carrierCassetteCylinder.Switch(false);//載體盒氣缸收
                }

                else if (filterPaperBoxPullSignal.Signal)//判斷濾紙氣缸是否在推位
                {
                    catchFilterPaperAxis.MoveAsync(10000);//抓取濾紙toyo升降滑台-降
                    await Task.Delay(1000);
                    suctionFilterPaper.Switch(true);//吸取濾紙
                    await Task.Delay(1000);
                    catchFilterPaperTableUpAxis.MoveAsync(1000);//抓取濾紙toyo升降滑台-升
                    filterPaperBoxCylinder.Switch(false);//濾紙氣缸收

                }

                else if (filterPaperBoxPullSignal.Signal
                    && catchFilterPaperAxis.IsInposition)//判斷濾紙氣缸是否在收位 && 載體滑台到位訊號
                {
                    catchFilterPaperAxis.MoveAsync(1000);//抓取濾紙升降滑台-降
                    suctionFilterPaper.Switch(false);//放下濾紙
                    await Task.Delay(1000);
                    catchFilterPaperTableUpAxis.MoveAsync(2000);//抓取濾紙升降滑台-升
                }

            }

            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                // MyErrorHandler.HandleError(ex); // 使用自訂的錯誤處理機制
            }

        }



        //載體滑台移動至載體盒站
        public async Task MoveToCBoxCassette()
        {
            carrierSlideTableAxis.MoveAsync(LoadModuleParam.CarrierTableBoxCassettePos);//載體滑台移動至載體盒站
            await Task.Delay(1000);

        }

        //載體滑台移動至濾紙站
        public async Task MoveToFilterPaper()
        {
            carrierSlideTableAxis.MoveAsync(LoadModuleParam.CarrierTableFilterPaperPos);//載體滑台移動至濾紙站
            await Task.Delay(1000);
        }
        //載體盒移動至注射站
        public async Task MoveToDump()
        {
            carrierSlideTableAxis.MoveAsync(3000);//載體滑台移動至注射站

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
    {     /// <summary>
          ///   吸取濾紙軸-抓取位置座標 
          /// </summary>
        public double FilterPaperCatchPos { get; set; }
        /// <summary>
        /// 吸取濾紙軸-軸收回座標 
        /// </summary>
        public double FilterPaperBackPos { get; set; }

        /// <summary>
        /// 橫移軸 在放濾紙工作位置
        /// </summary>
        public double Pos { get; set; }


        /// <summary>
        /// 橫移軸 在載體盒卡匣工作位置
        /// </summary>
        public double CarrierTableBoxCassettePos { get; set; }

        /// <summary>
        /// 橫移軸 在放濾紙工作位置
        /// </summary>
        public double CarrierTableFilterPaperPos { get; set; }

    }
}
