using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Modbus.Device;
using Newtonsoft.Json.Linq;

namespace WindowsFormsApp3.Component
{
    public class OrientAxis : IAxis
    {
        private SerialPort serialPort;
        private ModbusSerialMaster master;

        public bool IsInposition => Isinpos();

        public double Position => throw new NotImplementedException();

        public OrientAxis(string comport)
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
     

      
        public void motorAlarmReset(byte slaveAddress, ushort registerAddress, ushort value)
        {
            // 命令和地址       
            master.WriteSingleRegister(slaveAddress, registerAddress, value);
        }

        public void Home(byte slaveAddress, ushort registerAddress, ushort value)
        {
            // 命令和地址       
            master.WriteSingleRegister(slaveAddress, registerAddress, value);
        }

        public void Stop(byte slaveAddress, ushort registerAddress, ushort value)
        {
            // 命令和地址       
            master.WriteSingleRegister(slaveAddress, registerAddress, value);
        }

        public void SetVelocity(double finalVelocity, double acceleration, double deceleration)
        {
            throw new NotImplementedException();
        }

        public void MoveToAsync(double pos)
        {
            throw new NotImplementedException();
        }

        public void MoveAsync(double distance)
        {
            throw new NotImplementedException();
        }

        private bool Isinpos()
        {
            //Modbus Read INP

            ushort[] rotaRegisters2 = master.ReadHoldingRegisters(1, 0x007F, 0x0001);
            bool rotaMotorINP = (rotaRegisters2[0] & (1 << 14)) != 0; // 檢查bit14

            return rotaMotorINP;

        }

        void IAxis.Home()
        {
            throw new NotImplementedException();
        }

        void IAxis.Stop()
        {
            throw new NotImplementedException();
        }
        

        /*
        public double Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public double Acceleration => throw new NotImplementedException();

        public double Deceleration => throw new NotImplementedException();

        public double FinalVelocity => throw new NotImplementedException();

        public double NEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double PEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public AxisStatus Status => throw new NotImplementedException();

        public void Home()
        {
            throw new NotImplementedException();
        }

        public void MoveAsync(double distance)
        {
            throw new NotImplementedException();
        }

        public void MoveToAsync(double pos)
        {
            throw new NotImplementedException();
        }

        public void SetVelocity(double finalVelocity, double acceleration, double deceleration)
        {
            throw new NotImplementedException();
        }

        void IAxis.Postion()
        {
            throw new NotImplementedException();
        }
        */
    }
}
