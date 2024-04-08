

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

        public bool IsBusy => Move();

        public void Home()
        {
            master.WriteSingleRegister(slaveAddress, 0x201E, 0x0003);
        }

        public void Stop()
        {
            master.WriteSingleRegister(slaveAddress, 0x201E, 0x0008);
        }

        public void SetVelocity(double finalVelocity, double acceleration, double deceleration)
        {
            ushort moveSpeedSet = (ushort)Convert.ToInt16(finalVelocity);//移動速度轉型
            master.WriteSingleRegister(slaveAddress, 0x2014, moveSpeedSet);//移動速度設定，單位0~100%
        }

        public void MoveToAsync(double pos)
        {
            ushort absAmount = (ushort)Convert.ToInt16(pos);//絕對移動量轉型
            master.WriteSingleRegister(slaveAddress, 0x201E, 0x0001);//選擇移動類型:ABS絕對移動
            master.WriteSingleRegister(slaveAddress, 0x2002, absAmount);//ABS絕對移動量輸入，單位0.01mm/1pulse

        }

        public void MoveAsync(double distance)
        {
            ushort incAmount = (ushort)Convert.ToInt16(distance);//相對移動量轉型
            master.WriteSingleRegister(slaveAddress, 0x201E, 0x0000);//選擇移動類型:INC相對移動
            master.WriteSingleRegister(slaveAddress, 0x2002, incAmount);//INC相對移動量輸入，單位0.01mm/1pulse
        }


        public bool Isinpos()
        {
          
            var response = master.ReadHoldingRegisters(1, 0x0700, 0x0001);
            bool inPosition = Convert.ToBoolean(response);
            return inPosition;
        }

        public bool Move() 
        {
                  
            var response = master.ReadHoldingRegisters(1, 0x0703, 0x0001);
            bool isBusy = Convert.ToBoolean(response);
            return isBusy;
            
        }

        public void AlarmReset()
        {
            master.WriteSingleRegister(1, 0x007D, 0x0088);
        }

        public void AccelTime(int time)
        {
            ushort accelTime = (ushort)Convert.ToInt16(time);//加速時間設定
            master.WriteSingleRegister(slaveAddress, 0x0804, accelTime);
        }

        public void DecelTime(int time)
        {
            ushort decelTime = (ushort)Convert.ToInt16(time);//減速時間設定
            master.WriteSingleRegister(slaveAddress, 0x0805, decelTime);
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
