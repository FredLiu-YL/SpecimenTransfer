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
        InstantDiCtrl instantDiCtrl1 = new InstantDiCtrl();
        InstantDoCtrl InstantDoCtrl1 = new InstantDoCtrl();


        public ADTech_USB4750()
        {
            // 初始化
            instantDiCtrl1.SelectedDevice = new DeviceInformation(1);
            InstantDoCtrl1.SelectedDevice = new DeviceInformation(1);
        }

       
        public string DigitalInCommand(int port, int bit, out byte portData)
        {
            //4750讀取數據
            instantDiCtrl1.ReadBit(port, bit, out portData);
            string data = portData.ToString();

            //byte 11111111反向為00000000
            string invertedBinaryString = new string(data.Select(c => c == '1' ? '0' : '1').ToArray());
            return invertedBinaryString;
        }

        public void DigitalOutCommand(int port, int bit, byte signalSwitch)
        {
            //port有2個，0和1
            //bit是0~7，0和1各1組
            //signalSwitch，控制開關0或1
            InstantDoCtrl1.WriteBit(port, bit, signalSwitch);

        }


        DigitalOutput[] IDigitalSignalController.SignalOutput => throw new NotImplementedException();

        DigitalIntput[] IDigitalSignalController.SignalInput => throw new NotImplementedException();


    }
}
