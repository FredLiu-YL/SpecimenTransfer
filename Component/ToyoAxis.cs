using Modbus.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3.Component
{
    public class ToyoAxis : IAxis
    {
        private SerialPort serialPort;
        private ModbusSerialMaster master;
        public ToyoAxis(string comport)
        {
            serialPort = new SerialPort
            {
                PortName = comport, // Adjust the COM port as necessary
                BaudRate = 19200,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One
            };


            //485通訊開啟
            serialPort.Open();
            //建立MODBUS主站通訊
            master = ModbusSerialMaster.CreateRtu(serialPort);

      

        }


        public double Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public double Acceleration => throw new NotImplementedException();

        public double Deceleration => throw new NotImplementedException();

        public double FinalVelocity => throw new NotImplementedException();

        public double NEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double PEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public AxisStatus Status => throw new NotImplementedException();

        public void Home()
        {
            master.WriteSingleRegister(1, 0x007D, 0x0010);
        }

        public void MoveAsync(double distance)
        {
            throw new NotImplementedException();
        }

        public void MoveToAsync(double pos)
        {
            try
            {
                // 命令和地址
                master.WriteSingleRegister(1, 0x201E, 0x0001);

               
            }
            catch (Exception ex )
            {

                throw ex;
            }
           

        }

        public void SetVelocity(double finalVelocity, double acceleration, double deceleration)
        {
            throw new NotImplementedException();
        }
    }



    public class ToyoEthnetAxis : IAxis
    {
        public ToyoEthnetAxis(string ip)
        {



        }

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
    }
}
