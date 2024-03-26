using Modbus.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3.Component
{
    public interface IElectricCylinder
    {

        void Home();
        void On();
        void Off();

    }


    public class ToyoCylinder : IElectricCylinder
    {

        private SerialPort serialPort;
        private ModbusSerialMaster master;
        public ToyoCylinder(string comport)
        {
            serialPort = new SerialPort
            {
                PortName = comport, // Adjust the COM port as necessary
                BaudRate = 19200,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One
            };

            /*
            //485通訊開啟
            serialPort.Open();
            //建立MODBUS主站通訊
            master = ModbusSerialMaster.CreateRtu(serialPort);
            */


        }

        public void Home()
        {
            master.WriteSingleRegister(1, 0x007D, 0x0010);
        }

        public void Off()
        {
            // 命令和地址
            master.WriteSingleRegister(1, 0x201E, 0x0001);
        }

        public void On()
        {
            // 命令和地址
            master.WriteSingleRegister(1, 0x201E, 0x0002);

        }
    }
}
