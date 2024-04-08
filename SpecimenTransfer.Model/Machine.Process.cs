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


            LoadModle.LoadModuleParam = MachineSet.LoadModuleParam;
            DumpModle.DumpModuleParam = MachineSet.DumpModuleParam;
            try
            {
                //步驟1 先放好所有的卡匣與料  ，但因藥罐必須控制元件所以加入流程控制

                await Task.Run(async () =>
                {
                    string carrierbarcode = "";
                    string medcineDataReceived = "1";
                    int readCount = 0;

                    await DumpModle.Load();//藥罐載入

                    //步驟2 比對條碼是否吻合
                    do
                    {
                        if (readCount > 2) throw new Exception("Barcode 驗證失敗");

                        Task<string> loadModleReadTask = LoadModle.ReadBarcode();
                        Task<string> dumpModleReadTask = DumpModle.ReadBarcode();

                        await loadModleReadTask;
                        await dumpModleReadTask;


                        carrierbarcode = loadModleReadTask.Result;
                        medcineDataReceived = dumpModleReadTask.Result;
                        readCount++;

                    } while (BarcodeComparison(carrierbarcode, medcineDataReceived));
                    Task unscrewTask = DumpModle.UnscrewMedicineJar(); //先旋開藥罐 同步做其他事

                    //載入一片載體盒
                    await LoadModle.LoadAsync(0);
                    await LoadModle.PuttheFilterpaperInBox();

                    //await  DumpModle.ClampMedicineBottle();

                    await LoadModle.MoveToDump();

                    await unscrewTask;//等待藥罐完成

                    for (int i = 0; i < 3; i++)
                    {
                        await DumpModle.DumpBottle();

                        await DumpModle.CleanBottle();

                        bool checkOK = await DumpModle.CheckBottleAction();
                        if (checkOK) break;//成功 離開迴圈
                        else
                            if (i >= 2) throw new Exception("重作3次 失敗");
                    }


                    Task screwtask = DumpModle.ScrewMedicineJar();
                    await DumpModle.InjectRedInk();
                    await OutputModle.LoadCoverAsync();
                    await OutputModle.PressDownCoverAsync();
                    await OutputModle.UnLoadBoxAsync(0);

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
