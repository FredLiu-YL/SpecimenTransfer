using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nito.AsyncEx;
using SpecimenTransfer.Model.Component;

namespace SpecimenTransfer.Model
{

    public partial class Machine
    {

        private PauseTokenSource pts = new PauseTokenSource();
        private CancellationTokenSource cts = new CancellationTokenSource();

        public bool isRunning { get ; set; }


        public async Task ProcessRun()
        {
            //LoadModle.LoadModuleParam = MachineSet.LoadModuleParam;//載入LoadModulParam參數
            //DumpModle.DumpModuleParam = MachineSet.DumpModuleParam;//載入DumpModuleParam參數
            //OutputModle.OutputModuleParam = MachineSet.OutputModuleParam;//載入OutputModuleParam參數

            try
            {
                while (isRunning)
                {

                    //步驟1 先放好所有的卡匣與料，但因藥罐必須控制元件所以加入流程控制
                    await Task.Run(async () =>
                       {

                           //步驟1 物料就位
                           await LoadModle.LoadBoxAsync(1);
                           // ProcessPause();
                           cts.Token.ThrowIfCancellationRequested();
                           await pts.Token.WaitWhilePausedAsync(cts.Token);

                           await DumpModle.LoadBottle();//等待人將藥罐載入


                           //步驟2 讀取比對條碼是否吻合
                           string carrierbarcode = "";
                           string medcineDataReceived = "1";
                           int readCount = 0;
                           bool compareResult;

                           do
                           {
                               if (readCount > 2) throw new Exception("Barcode 驗證失敗");

                               Task<string> loadModleReadTask = LoadModle.ReadBarcode();//讀取濾紙載盤條碼
                               Task<string> dumpModleReadTask = DumpModle.ReadBarcode();//讀取藥罐條碼

                               await loadModleReadTask;//等待讀取條碼
                               await dumpModleReadTask;//等待讀取條碼

                               carrierbarcode = loadModleReadTask.Result;//讀取載盤條碼結果
                               medcineDataReceived = dumpModleReadTask.Result;//讀取藥罐條碼結果
                               readCount++;//累計讀取次數

                           }
                           while (BarcodeComparison(carrierbarcode, medcineDataReceived));//比對載盤及藥罐條碼結果
                           compareResult = BarcodeComparison(carrierbarcode, medcineDataReceived);

                           if (compareResult)
                               MessageBox.Show("條碼比對OK");
                           else
                               MessageBox.Show("條碼比對NG");

                           // ProcessPause();
                           cts.Token.ThrowIfCancellationRequested();
                           await pts.Token.WaitWhilePausedAsync(cts.Token);


                           //步驟3 放濾紙
                           await LoadModle.MoveToFilterPaper();//載體滑台移動至濾紙站
                           await LoadModle.PuttheFilterpaperInBox();//放濾紙
                           // ProcessPause();
                           cts.Token.ThrowIfCancellationRequested();
                           await pts.Token.WaitWhilePausedAsync(cts.Token);


                           //步驟4 旋開藥罐
                           await DumpModle.CarrierMoveToDump();//載體滑台移動至藥罐傾倒站
                           // ProcessPause();                                    
                           cts.Token.ThrowIfCancellationRequested();
                           await pts.Token.WaitWhilePausedAsync(cts.Token);

                           await DumpModle.UnscrewMedicineJar(); //先旋開藥罐 同步做其他事
                           // ProcessPause();                                    
                           cts.Token.ThrowIfCancellationRequested();
                           await pts.Token.WaitWhilePausedAsync(cts.Token);

                           //Task unscrewTask = DumpModle.UnscrewMedicineJar(); //先旋開藥罐 同步做其他事
                           //await unscrewTask;//等待旋開藥罐完成

                           await Task.Delay(2000);

                           //步驟5 傾倒藥罐
                           await DumpModle.DumpBottle();//傾倒藥罐
                           // ProcessPause();                                    
                           cts.Token.ThrowIfCancellationRequested();
                           await pts.Token.WaitWhilePausedAsync(cts.Token);

                           await DumpModle.CleanBottle();//清洗藥罐
                           // ProcessPause();                                    
                           cts.Token.ThrowIfCancellationRequested();
                           await pts.Token.WaitWhilePausedAsync(cts.Token);


                           /*
                           for (int i = 0; i < 3; i++)
                           {
                               await DumpModle.DumpBottle();//傾倒藥罐

                               await DumpModle.CleanBottle();//清洗藥罐

                               bool checkOK = await DumpModle.CheckBottleAction();//檢查藥罐

                               if (checkOK) break;//檢查成功離開迴圈
                               else
                                   if (i >= 2) throw new Exception("重作3次 失敗");//檢查失敗拋異常
                           }
                           */

                           //步驟6 旋緊藥罐
                           await DumpModle.ScrewMedicineJar();//旋緊藥罐
                           //await screwtask;
                           // ProcessPause();
                           cts.Token.ThrowIfCancellationRequested();
                           await pts.Token.WaitWhilePausedAsync(cts.Token);


                           //步驟7 注入紅墨水
                           await DumpModle.CarrierMoveToRedInk();//載體滑台移動至紅墨水站
                           // ProcessPause();
                           cts.Token.ThrowIfCancellationRequested();
                           await pts.Token.WaitWhilePausedAsync(cts.Token);

                           await DumpModle.InjectRedInk();//注入紅墨水
                           // ProcessPause();
                           cts.Token.ThrowIfCancellationRequested();
                           await pts.Token.WaitWhilePausedAsync(cts.Token);


                           //步驟8 放濾紙
                           await LoadModle.MoveToFilterPaper();//載體滑台移動至濾紙站
                           // ProcessPause();
                           cts.Token.ThrowIfCancellationRequested();
                           await pts.Token.WaitWhilePausedAsync(cts.Token);

                           await LoadModle.PuttheFilterpaperInBox();//放濾紙
                           // ProcessPause();
                           cts.Token.ThrowIfCancellationRequested();
                           await pts.Token.WaitWhilePausedAsync(cts.Token);


                           //步驟9 推蓋
                           await OutputModle.CarrierMoveToPushCover();//載體滑台移動至放蓋站
                           // ProcessPause();
                           cts.Token.ThrowIfCancellationRequested();
                           await pts.Token.WaitWhilePausedAsync(cts.Token);

                           await OutputModle.LoadCoverAsync();//推蓋
                           // ProcessPause();
                           cts.Token.ThrowIfCancellationRequested();
                           await pts.Token.WaitWhilePausedAsync(cts.Token);


                           await OutputModle.LoadCarrier();//放入載體盒
                           // ProcessPause();
                           cts.Token.ThrowIfCancellationRequested();
                           await pts.Token.WaitWhilePausedAsync(cts.Token);


                           //步驟10 壓蓋
                           await OutputModle.CarrierMoveToPressDownCover();//載體滑台移動至壓蓋站
                           // ProcessPause();
                           cts.Token.ThrowIfCancellationRequested();
                           await pts.Token.WaitWhilePausedAsync(cts.Token);

                           await OutputModle.PressDownCoverAsync();//壓蓋
                           // ProcessPause();
                           cts.Token.ThrowIfCancellationRequested();
                           await pts.Token.WaitWhilePausedAsync(cts.Token);


                           //步驟10 收納
                           await OutputModle.CarrierMoveToStorage();//載體滑台移動至收納站
                           // ProcessPause();
                           cts.Token.ThrowIfCancellationRequested();
                           await pts.Token.WaitWhilePausedAsync(cts.Token);

                           await OutputModle.UnLoadBoxAsync();//收納模組
                           // ProcessPause();
                           cts.Token.ThrowIfCancellationRequested();
                           await pts.Token.WaitWhilePausedAsync(cts.Token);


                       });

                }
            }

            catch (Exception ex)
            {

                Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                cts.Dispose();

            }
            

        }
        bool isPausedStatus;
        public async Task<bool> ProcessPause()
        {
            pts.IsPaused = true;
            bool isPausedStatus = pts.IsPaused;
            return isPausedStatus;
        }


        public async Task ProcessResume()
        {

            pts.IsPaused = false;


        }

        public async Task ProcessStop()
        {
            cts.Cancel();
      
        }

        /*
        public async Task ProcessStop()
        {
            LoadModle.SlideTableAxis.Stop();
            LoadModle.FilterPaperElevatorAxis.Stop();
            DumpModle.BottleElevatorAxis.Stop();
            DumpModle.BottleScrewAxis.Stop();
            OutputModle.CoverAndStorageElevatorAxis.Stop();

        }
        */


        //條碼比對
        public bool BarcodeComparison(string carrierDataReceived, string medcineDataReceived)
        {

            if (carrierDataReceived == medcineDataReceived)
                return true;
            else
                return false;

        }


    }
}
