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
        private DigitalOutput carrierCylinder;

        //濾紙汽缸-推收
        private DigitalOutput filterPaperCylinder;

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

        //濾紙汽缸-推
        private DigitalIntput filterPaperCylinderPushSignal;
        //濾紙汽缸-收
        private DigitalIntput filterPaperCylinderPullSignal;

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

        //抓取濾紙升降滑台-升
        private IAxis catchFilterPaperUpAxis;
        //抓取濾紙升降滑台-降
        private IAxis catchFilterPaperDownAxis;
        //抓取濾紙升降滑台-ready
        private IAxis catchFilterPaperAxisReady;



        //藥罐toyo升降滑台-升
        private IAxis medicineBottleUpAxis;
        //藥罐toyo升降滑台-降
        private IAxis medicineBottleDownAxis;
        //藥罐toyo升降滑台-ready
        private IAxis medicineBottleAxisReady;

        //載體滑台回-Home
        private IAxis carrierSlideHomeAxis;
        //載體滑台-移動
        private IAxis carrierSlideMoveAxis;
        //載體滑台到-ready
        private IAxis carrierSlideAxisReady;
        
       

        //----條碼----
        //藥罐條碼
        private IBarcodeReader medcineBottle;
        //載體盒條碼
        private IBarcodeReader carrierBottle;

        

        public LoadModule(DigitalOutput[] signalOutput, DigitalIntput[] signalInput, IAxis carrierSlideTableAxis, IAxis catchFilterPaperAxis, IAxis medicineBottleAxis, IBarcodeReader barcodeReader)
        {
            //----Digital Output----
            shotCarrierBottleBarcode = signalOutput[0];//camera shot載體盒條碼

            shotMedcineBottleBarcode = signalOutput[1];//camera shot藥瓶條碼

            carrierCylinder = signalOutput[2];//載體氣缸

            filterPaperCylinder = signalOutput[3];//濾紙氣缸

            suctionFilterPaper = signalOutput[4];//吸濾紙

            upperClampMedicineCylinder = signalOutput[7];//藥罐瓶蓋氣缸-上夾爪

            lowerClampMedicineCylinder = signalOutput[8];//藥罐瓶蓋氣缸-下夾爪

            backLightCylinder = signalOutput[10];

            //----Digital Input----
            carrierCylinderPushSignal = signalInput[6];//載體盒 載體氣缸-推
            carrierCylinderPullSignal = signalInput[7];//載體盒 載體氣缸-收

            filterPaperCylinderPushSignal = signalInput[8];//濾紙氣缸-推
            filterPaperCylinderPullSignal = signalInput[9];//濾紙氣缸-收

            backLightCylinderPushSignal = signalInput[18];//背光氣缸-推
            backLightCylinderPushSignal = signalInput[19];//背光氣缸-收

            upperClampMedicineCylinderCloseSignal = signalInput[14];//藥罐瓶蓋上夾爪-關
            upperClampMedicineCylinderOpenSignal = signalInput[15];//藥罐瓶蓋上夾爪-開

            lowerClampMedicineCylinderCloseSignal = signalInput[16]; //藥罐瓶蓋下夾爪-關
            lowerClampMedicineCylinderOpenSignal = signalInput[17]; //藥罐瓶蓋下夾爪-開

            //----軸控----

            catchFilterPaperUpAxis = catchFilterPaperAxis;//抓取濾紙toyo升降滑台-升
            catchFilterPaperDownAxis = catchFilterPaperAxis; //抓取濾紙toyo升降滑台-降
            catchFilterPaperAxisReady = catchFilterPaperAxis; //抓取濾紙toyo升降滑台-ready

            medicineBottleUpAxis = medicineBottleAxis;//藥罐toyo升降滑台-升
            medicineBottleDownAxis = medicineBottleAxis;//藥罐toyo升降滑台-降
            medicineBottleAxisReady = medicineBottleAxis;//藥罐toyo升降滑台-ready

            carrierSlideHomeAxis = carrierSlideTableAxis;//載體滑台回Home
            carrierSlideMoveAxis = carrierSlideTableAxis;//載體滑台移動
            carrierSlideAxisReady = carrierSlideTableAxis;//載體滑台到位準備訊號
                                                         
            //----條碼----
            medcineBottle = barcodeReader;//藥罐條碼
            carrierBottle = barcodeReader;//載體盒條碼

        }

        



        //BarcodeComparison
        public async Task BarcodeComparison()
        {
            int count = 2;
            //拍照
            try
            {
                shotCarrierBottleBarcode.On(0, true);
                shotMedcineBottleBarcode.On(1, true);
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
                    shotCarrierBottleBarcode.On(0, true);
                    shotMedcineBottleBarcode.On(1, true);
                    count++;
                }

            }
        }

        //載體滑台移動至載體盒站
        public async Task MoveToCarrierBox()
        {
            carrierSlideMoveAxis.MoveAsync(1000);//載體滑台移動至載體盒站
            await Task.Delay(1000);

        }

        //載體滑台移動至濾紙站
        public async Task MoveToFilterPaper()
        {
            carrierSlideMoveAxis.MoveAsync(2000);//載體滑台移動至濾紙站
            await Task.Delay(1000);
        }



        //載體站/濾紙站動作流程
        public async Task PushCarrierBoxAndClampFilterPaper()
        {

            try
            {
                if (carrierCylinderPullSignal.Signal)//判斷載體盒氣缸是否在收位 && carrierSlideTableAxis到位訊號
                    carrierCylinder.On(2, true);//載體盒氣缸推

                else if (filterPaperCylinderPullSignal.Signal)//判斷濾紙氣缸是否在收位
                    filterPaperCylinder.On(3, true);//濾紙氣缸推

                else if (carrierCylinderPullSignal.Signal)//判斷載體盒氣缸是否在推位
                {
                    carrierCylinder.On(2, false);
                    carrierCylinder.Off(2, true);//載體盒氣缸收
                }

                else if (filterPaperCylinderPullSignal.Signal)//判斷濾紙氣缸是否在推位
                {
                    catchFilterPaperDownAxis.MoveAsync(10000);//抓取濾紙toyo升降滑台-降
                    await Task.Delay(1000);
                    suctionFilterPaper.On(4, true);//吸取濾紙
                    await Task.Delay(1000);
                    catchFilterPaperTableUpAxis.MoveAsync(1000);//抓取濾紙toyo升降滑台-升
                    filterPaperCylinder.On(3, false);//濾紙氣缸推
                    filterPaperCylinder.Off(3, true);//濾紙氣缸收
                    
                }

                else if (filterPaperCylinderPullSignal.Signal
                    && carrierSlideAxisReady.IsInposition)//判斷濾紙氣缸是否在收位 && 載體滑台到位訊號
                {
                    catchFilterPaperDownAxis.MoveAsync(1000);//抓取濾紙升降滑台-降
                    suctionFilterPaper.On(4, false);
                    suctionFilterPaper.Off(4, true);//放下濾紙
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


        //載體盒移動至注射站
        public async Task MoveToDump()
        {
            carrierSlideMoveAxis.MoveAsync(3000);//載體滑台移動至注射站

        }

    }
}
