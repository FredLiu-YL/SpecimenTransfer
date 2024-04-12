using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SpecimenTransfer.Model.Component;

namespace SpecimenTransfer.Model
{

    public partial class Machine
    {
        

        public async Task ProcessRun()
        {
            LoadModle.LoadModuleParam = MachineSet.LoadModuleParam;//載入LoadModulParam參數
            DumpModle.DumpModuleParam = MachineSet.DumpModuleParam;//載入DumpModuleParam參數
            OutputModle.OutputModuleParam = MachineSet.OutputModuleParam;//載入OutputModuleParam參數

            try
            {
                //步驟1 先放好所有的卡匣與料，但因藥罐必須控制元件所以加入流程控制

                await Task.Run(async () =>
                {
                    string carrierbarcode = "";
                    string medcineDataReceived = "1";
                    int readCount = 0;

                    await DumpModle.Load();//等待人將藥罐載入
   
                    //步驟2 比對條碼是否吻合
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

                    Task unscrewTask = DumpModle.UnscrewMedicineJar(); //先旋開藥罐 同步做其他事
                                 
                    await LoadModle.LoadAsync(0);//doubt??

                    await LoadModle.PuttheFilterpaperInBox();//載入一片載體盒(readbarcode時已經推出一片，??)

                    await DumpModle.CarrierMoveToClean();//載體滑台移動至藥罐傾倒站

                    await unscrewTask;//等待旋開藥罐完成

                  
                    
                    for (int i = 0; i < 3; i++)
                    {
                        await DumpModle.DumpBottle();//傾倒藥罐

                        await DumpModle.CleanBottle();//清洗藥罐

                        bool checkOK = await DumpModle.CheckBottleAction();//檢查藥罐

                        if (checkOK) break;//檢查成功離開迴圈
                        else
                            if (i >= 2) throw new Exception("重作3次 失敗");//檢查失敗拋異常
                    }

                    Task screwtask = DumpModle.ScrewMedicineJar();//旋緊藥罐

                    await DumpModle.CarrierMoveToRedInk();//載體滑台移動至紅墨水站

                    await DumpModle.InjectRedInk();//注入紅墨水

                    await LoadModle.MoveToFilterPaper();//載體滑台移動至濾紙站

                    await LoadModle.PuttheFilterpaperInBox();//放濾紙

                    await OutputModle.CarrierMoveToPushCover();//載體滑台移動至推蓋站

                    await OutputModle.LoadCoverAsync();//推蓋

                    await OutputModle.CarrierMoveToPressDownCover();//載體滑台移動至壓蓋站

                    await OutputModle.PressDownCoverAsync();//壓蓋

                    await OutputModle.CarrierMoveToStorage();//載體滑台移動至收納站

                    await OutputModle.UnLoadBoxAsync(0);//收納載體盒

                });
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {


            }

        }
        //條碼比對
        public bool BarcodeComparison(string carrierDataReceived, string medcineDataReceived)
        {

            return medcineDataReceived == carrierDataReceived;

        }


        public void DumpSpecimen()
        {

            /*
            //橫移軸移動到傾倒位置
            axisTransfer.MoveToAsync(MachineSet.TransferDumpPos);

            JarClampCylinder.On();

            //轉開蓋子
            axisTurnLid.MoveToAsync(MachineSet.DumpTurnOnPos);
            //倒出檢體
            axisDump.MoveToAsync(MachineSet.DumpPourOutPos);
            */
        }
        public void ConfirmClear()
        {

        }

        public void UnLoad()
        {


        }
    }
}
