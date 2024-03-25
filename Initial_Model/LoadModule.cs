using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
        private IAxis catchFilterPaperCylinderUpAxis;
        //抓取濾紙toyo升降滑台-降
        private IAxis catchFilterPaperCylinderDownAxis;

        //吸濾紙
        private DigitalOutput suctionFilterPaper;

        //上方夾藥罐氣缸-打開關閉
        private DigitalOutput upperClampMedicineCylinder;

        //下方夾藥罐氣缸-打開關閉
        private DigitalOutput lowerClampMedicineCylinder;

        
        //----Digital Input----

        //載體盒 載體氣缸-推
        private DigitalIntput carrierCylinderPushSignal;
        //載體盒 載體氣缸-收
        private DigitalIntput carrierCylinderPullSignal;

        //濾紙汽缸-推
        private DigitalIntput filterPaperCylinderPushSignal;
        //濾紙汽缸-收
        private DigitalIntput filterPaperCylinderPullSignal;

        //上方夾藥罐氣缸-打開訊號
        private DigitalIntput upperClampMedicineCylinderOpenSignal;
        //上方夾藥罐氣缸-關閉訊號
        private DigitalIntput upperClampMedicineCylinderCloseSignal;

        //下方夾藥罐氣缸-打開訊號
        private DigitalIntput lowerClampMedicineCylinderOpenSignal;
        //下方夾藥罐氣缸-關閉訊號
        private DigitalIntput lowerClampMedicineCylinderCloseSignal;

        //----軸控----
      
        //抓取濾紙toyo升降滑台-升
        private IAxis catchFilterPaperUpAxis;
        //抓取濾紙toyo升降滑台-降
        private IAxis catchFilterPaperDownAxis;
        //抓取濾紙toyo升降滑台-ready
        private IAxis catchFilterPaperCylinderAxisReady;

        //藥罐toyo升降滑台-升
        private IAxis medicineBottleUpAxis;
        //藥罐toyo升降滑台-降
        private IAxis medicineBottleDownAxis;
        //藥罐toyo升降滑台-ready
        private IAxis medicineBottleAxisReady;


        //載體滑台在載體位置
        private IAxis caririerSlideTableCarrierPostion;
        //載體滑台到位
        private IAxis caririerSlideTableReady;

        //----條碼----
        //藥罐條碼
        private IBarcodeReader medcineBottle;
        //載體盒條碼
        private IBarcodeReader carrierBottle;


        public LoadModule( DigitalOutput[] signalOutput , DigitalIntput[] signalInput, IAxis carrierSlideTableAxis, IAxis catchFilterPaperAxis,IAxis medicineBottleAxis ,IBarcodeReader barcodeReader)
        {
            //----Digital Output----
            shotCarrierBottleBarcode = signalOutput[0];//camera shot載體盒條碼

            shotMedcineBottleBarcode = signalOutput[1];//camera shot藥瓶條碼

            carrierCylinder = signalOutput[2];//載體氣缸

            filterPaperCylinder = signalOutput[3];//濾紙氣缸

            suctionFilterPaper = signalOutput[4];//吸濾紙

            upperClampMedicineCylinder = signalOutput[7];//藥罐瓶蓋氣缸-上夾爪

            lowerClampMedicineCylinder = signalOutput[8];//藥罐瓶蓋氣缸-下夾爪

            //----Digital Input----
            carrierCylinderPushSignal = signalInput[6];//載體盒 載體氣缸-推
            carrierCylinderPullSignal = signalInput[7];//載體盒 載體氣缸-收

            filterPaperCylinderPushSignal = signalInput[8];//濾紙氣缸-推
            filterPaperCylinderPullSignal = signalInput[9];//濾紙氣缸-收

            upperClampMedicineCylinderCloseSignal = signalInput[14];//藥罐瓶蓋上夾爪-關
            upperClampMedicineCylinderOpenSignal = signalInput[15];//藥罐瓶蓋上夾爪-開

            lowerClampMedicineCylinderCloseSignal = signalInput[16]; //藥罐瓶蓋下夾爪-關
            lowerClampMedicineCylinderOpenSignal = signalInput[17]; //藥罐瓶蓋下夾爪-開

            //----軸控----
          
            catchFilterPaperUpAxis = catchFilterPaperAxis;//抓取濾紙toyo升降滑台-升
            catchFilterPaperDownAxis = catchFilterPaperAxis; //抓取濾紙toyo升降滑台-降
            catchFilterPaperCylinderAxisReady = catchFilterPaperAxis; //抓取濾紙toyo升降滑台-ready

            caririerSlideTableCarrierPostion = carrierSlideTableAxis;//載體滑台在載體位置
            caririerSlideTableReady = carrierSlideTableAxis;//載體滑台載體位置到位

            medicineBottleUpAxis = medicineBottleAxis;//藥罐toyo升降滑台-升
            medicineBottleDownAxis = medicineBottleAxis;//藥罐toyo升降滑台-降
            medicineBottleAxisReady = medicineBottleAxis;//藥罐toyo升降滑台-ready


            //----條碼----
            medcineBottle = barcodeReader;//藥罐條碼
            carrierBottle = barcodeReader;//載體盒條碼

            
        }

        //camera
        public async void BarcodeComparison() 
        {
            int count = 2;
            //拍照
            shotCarrierBottleBarcode.On(0,true);
            shotMedcineBottleBarcode.On(1,true);
            
            //條碼比對
            string medcineDataReceived = medcineBottle.ReceiveData();
            string carrierDataReceived = carrierBottle.ReceiveData();
            await Task.Delay(500);

            //確認條碼內容，條碼發生問題，重複拍照檢測3次
            for (int i = 0; i <= count; i++)
            {
                if (medcineDataReceived == carrierDataReceived && medcineBottle != null &&carrierBottle != null)//藥罐和載體盒條碼比對，藥換和載體盒條碼不為空
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
        public async void PushCarrierBoxAndClampFilterPaper()
        {
            caririerSlideTableCarrierPostion.Home();//載體滑台移動至載體盒位置
            await Task.Delay(500);
            if (carrierCylinderPullSignal.Signal)//判斷載體盒氣缸是否在收位 && carrierSlideTableAxis到位訊號
                carrierCylinder.On(2, true);//載體盒氣缸推
  
            else if (filterPaperCylinderPullSignal.Signal)//判斷濾紙氣缸是否在收位
                filterPaperCylinder.On(3, true);//濾紙氣缸推

            else if (carrierCylinderPullSignal.Signal)//判斷載體盒氣缸是否在推位
                carrierCylinder.Off(2, false);//載體盒氣缸收

            else if (filterPaperCylinderPullSignal.Signal)//判斷濾紙氣缸是否在推位
            {
                catchFilterPaperDownAxis.MoveAsync(10000);//抓取濾紙toyo升降滑台-降
                await Task.Delay(1000);
                suctionFilterPaper.On(4, true);
                await Task.Delay(1000);
                catchFilterPaperCylinderUpAxis.Home();

            }

        }

        public async void ClampMedicineBottle()
        {
            if (upperClampMedicineCylinderCloseSignal.Signal && 
                lowerClampMedicineCylinderOpenSignal.Signal )//上藥罐夾爪關且下藥罐夾爪開且藥罐軸在home點
                medicineBottleDownAxis.MoveAsync(1000);

            /* 待IAxis軸加入ready的判斷功能
            else if(medicineBottleAxisReady.)//判斷藥罐軸下降是否到位
            {
                lowerClampMedicineCylinder.On(8, true);
            }
            */
        }




        public void LoadAsync(int cassetteId)
        {


        }

    }
}
