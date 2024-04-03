

using Modbus.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecimenTransfer.Model.Component
{
    public class ToyoAxis : IAxis
    {
        private SerialPort serialPort;
        private ModbusSerialMaster master;
        private byte slaveAddress;


        public ToyoAxis(string comport, int driverID)
        {
            serialPort = new SerialPort
            {
                PortName = comport, // Adjust the COM port as necessary
                BaudRate = 19200,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One
            };
            var id = BitConverter.GetBytes(driverID);
            slaveAddress = id[0];
            /*
            //485通訊開啟
            serialPort.Open();
            //建立MODBUS主站通訊
            master = ModbusSerialMaster.CreateRtu(serialPort);
            */

        }
        public bool IsInposition => Isinpos();

        public double Position => throw new NotImplementedException();

        public double PEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double NEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsBusy => throw new NotImplementedException();

        public void Home()
        {
            master.WriteSingleRegister(slaveAddress, 0x007D, 0x0010);
        }

        public void Stop()
        {
            master.WriteSingleRegister(slaveAddress, 0x201E, 0x0009);
        }

        public void SetVelocity(double finalVelocity, double acceleration, double deceleration)
        {
            throw new NotImplementedException();
        }

        public void MoveToAsync(double pos)
        {
            master.WriteSingleRegister(slaveAddress, 0x201E, 0x0001);
        }

        public void MoveAsync(double distance)
        {
            throw new NotImplementedException();
        }


        private bool Isinpos()
        {
            //Modbus Read INP

            ushort[] rotaRegisters2 = master.ReadHoldingRegisters(1, 0x007F, 0x0001);
            bool caririerSlideTableINP = (rotaRegisters2[0] & (1 << 14)) != 0; // 檢查bit14

            return caririerSlideTableINP;

        }

        public void AlarmReset()
        {
            master.WriteSingleRegister(1, 0x007D, 0x0088);
        }

        public void Home(double axisCoverAndStorageElevatorHomePos)
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

void IAxis.Postion()
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

void IAxis.Postion()
{
throw new NotImplementedException();
}
*/
    }

}
