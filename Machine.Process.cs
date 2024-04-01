using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsFormsApp3
{
    public partial class Machine
    {

        /*
        public string Load(int cassetteIndex)
        {
            try
            {
                //橫移軸移動到入料位置
                //axisTransfer.MoveToAsync(MachineSet.TransferLoadPos);
                //string barcode = loadModel.Run(MachineSet, cassetteIndex);

                /* //橫移軸移動到入料位置
                 axisTransfer.MoveToAsync(MachineSet.TransferLoadPos);

                 //卡匣升降移動到準備推出的位置
                 double pos = MachineSet.BoxCassetteElevatorStartPos + (CassetteIndex - 1) * MachineSet.BoxCassetteElevatorPitch;
                 axisBoxCassetteElevator.MoveToAsync(pos);

                 string barcode = barcodeReader.Read();


                 loadPushBoxCylinder.On(); //電動缸推
                 Thread.Sleep(200);
                 loadPushBoxCylinder.Off(); //電動缸收
                
                //return barcode;
            }
            catch (Exception ex)
            {

                throw ex;
            }
          
        }
        */
        public async Task ProcessRun()
        {

            LoadModle.LoadModuleParam = Recipe.LoadModuleParam;
            try
            {
                await Task.Run(async () =>
                {
                    string carrierbarcode = "";
                    string medcineDataReceived = "1";
                    int readCount = 0;
                    //比對條碼是否吻合
                    do
                    {
                        if (readCount > 2) throw new Exception("Barcode 驗證失敗");
                        carrierbarcode = await LoadModle.ReadBarcode();
                        medcineDataReceived = await DumpModle.ReadBarcode();
                        readCount++;

                    } while (BarcodeComparison(carrierbarcode, medcineDataReceived));


                    //載入一片載體盒
                    await LoadModle.LoadAsync(0);
                    await LoadModle.PuttheFilterpaperInBox();

                   //await  DumpModle.ClampMedicineBottle();

                    await LoadModle.MoveToDump();


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
