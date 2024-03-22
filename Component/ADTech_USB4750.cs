using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automation.BDaq;

namespace WindowsFormsApp3.Component
{
    public class ADTech_USB4750 : IDigitalSignalController
    {
        private InstantDiCtrl instantDiCtrl1 = new InstantDiCtrl();
        private InstantDoCtrl InstantDoCtrl1 = new InstantDoCtrl();
        //暫存給輸出紀錄用的 8Bit狀態
        private char[] bitOutputPort0 = new char[8] { '0', '0', '0', '0', '0', '0', '0', '0' };
        private char[] bitOutputPort1 = new char[8] { '0', '0', '0', '0', '0', '0', '0', '0' };
        public ADTech_USB4750(int deviceNum)
        {
            // 初始化
            instantDiCtrl1.SelectedDevice = new DeviceInformation(deviceNum);
            InstantDoCtrl1.SelectedDevice = new DeviceInformation(deviceNum);
        }



        public DigitalOutput[] SignalOutput => ReturnOutput();

        public DigitalIntput[] SignalInput => ReturnIntput();


        public void DigitalOutCommand(int number, bool trigger)
        {

            //port有2個，0和1
            //bit是0~7，0和1各1組
            //signalSwitch，控制開關0或1

            if (number < 8)
            {
                string byteStr = ArrayToString(bitOutputPort0, number, trigger);//觸發號碼轉換2進制bit字串
                byte byteValue = Convert.ToByte(byteStr, 2); //將2進制bit字串 轉換成byte
                InstantDoCtrl1.WriteBit(0, number, byteValue);
            }
            else if (number >= 8)
            {
                int bitnumber = number - 8; //256僅能表示8bit ，所以16 Out 的設計上是拆成 0~7，8-15 。超過8就是另外一組
                string byteStr = ArrayToString(bitOutputPort1, bitnumber, trigger);//觸發號碼轉換2進制bit字串
                byte byteValue = Convert.ToByte(byteStr, 2);//將2進制bit字串 轉換成byte
                InstantDoCtrl1.WriteBit(1, bitnumber, byteValue);//8到15 port號就是1 後面一樣8bit表示
            }

        }

        public bool DigitalInCommand(int number)
        {
            char tempChar = '0';

            if (number < 8)
            {
                //4750讀取數據
                instantDiCtrl1.Read(0, out byte portData);
                var bit = 255 - portData; //用255-數值可以得到相反的bit數值 ，8bit預設數值"11111111"，所以數值255 表示全空 ， 例:2號觸發 就是"11111011" 數值253
                char[] carArr = AnalyzeBitData((byte)bit).Reverse().ToArray(); //將字元陣列相反 ， 陣列位置才會跟號碼一致
                tempChar = carArr[number];//讀取對應(號碼)陣列位置的字元 

            }
            else if (number >= 8)
            {
                int bitnumber = number - 8; //256僅能表示8bit ，所以16 Out 的設計上是拆成 0~7，8-15 。超過8就是另外一組
                //4750讀取數據
                instantDiCtrl1.Read(1,  out byte portData);
                var bit = 255 - portData; //用255-數值可以得到相反的bit數值 ，8bit預設數值"11111111"，所以數值255 表示全空 ， 例:2號觸發 就是"11111011" 數值251
                char[] carArr = AnalyzeBitData((byte)bit).Reverse().ToArray(); //將字元陣列相反 ， 陣列位置才會跟號碼一致
                tempChar = carArr[bitnumber]; //讀取對應(號碼)陣列位置的字元 


            }

            if (tempChar == '1')//因為已經用255相減 相反了， 所以1 表示 TRUE
                return true;
            else
                return false;
        }



        private DigitalOutput[] ReturnOutput()
        {
            List<DigitalOutput> outputList = new List<DigitalOutput>();
            for (int i = 0; i < 16; i++)
            {
                outputList.Add(new DigitalOutput(i, this));
            }
            return outputList.ToArray();
        }
        private DigitalIntput[] ReturnIntput()
        {
            List<DigitalIntput> list = new List<DigitalIntput>();
            for (int i = 0; i < 16; i++)
            {
                list.Add(new DigitalIntput(i, this));
            }
            return list.ToArray();
        }

        //計算根據觸發號碼不同傳出對應的Bit 變化 例: 第2個號碼被觸發 回傳 "00000010"
        private string ArrayToString(char[] bitOutput, int number, bool trigger)
        {
            if (trigger)
                bitOutput[number] = '1';  
            else
                bitOutput[number] = '0';   

            var reBit = bitOutput.Reverse();
            var a = new string(reBit.ToArray());
            return a;
        }
        private char[] AnalyzeBitData(byte data)
        {
            string binaryString = Convert.ToString(data, 2);//轉二進制字串
            return binaryString.ToCharArray();//轉字元陣列
        }

    }
}
