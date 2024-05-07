using Modbus.Device;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpecimenTransfer.Model.Component
{
    public class ToyoAxis : IAxis
    {
        private SerialPort serialPort;
        private ModbusSerialMaster master;
        private byte slaveAddress;

        public ToyoAxis(SerialPort serialPort, int driverID)
        {
            /*serialPort = new SerialPort
            {
                PortName = comport, // Adjust the COM port as necessary
                BaudRate = 19200,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One
            };*/
            this.serialPort = serialPort;
            var id = BitConverter.GetBytes(driverID + 1);
            slaveAddress = id[0];

            try
            {
                //485通訊開啟
                serialPort.Open();

                //建立MODBUS主站通訊
                master = ModbusSerialMaster.CreateRtu(serialPort);
            }

            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }



        }
        public bool IsInposition => Isinpos();

        public double Position => throw new NotImplementedException();

        public double PEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double NEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsBusy => Move();


        public void Home()
        {

            try
            {

                master.WriteSingleRegister(slaveAddress, 0x201E, 0x0003);
            }
            catch (Exception error)
            {
                MessageBox.Show($"Error : {error.Message}");
            }

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
            try
            {
                ushort absAmount = (ushort)Convert.ToInt16(pos);//絕對移動量轉型
                master.WriteSingleRegister(slaveAddress, 0x201E, 0x0001);//選擇移動類型:ABS絕對移動
                master.WriteSingleRegister(slaveAddress, 0x2002, absAmount);//ABS絕對移動量輸入，單位0.01mm/1pulse
                SpinWait.SpinUntil(Isinpos, 2000);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void MoveAsync(double distance)
        {
            try
            {
                ushort incAmount = (ushort)Convert.ToInt16(distance);//相對移動量轉型
                master.WriteSingleRegister(slaveAddress, 0x201E, 0x0000);//選擇移動類型:INC相對移動
                master.WriteSingleRegister(slaveAddress, 0x2002, incAmount);//INC相對移動量輸入，單位0.01mm/1pulse

                SpinWait.SpinUntil(Isinpos, 2000);

            }

            catch (Exception error)
            {
                MessageBox.Show(error.Message);

            }

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

        public void JogAdd()
        {

            master.WriteSingleRegister(slaveAddress, 0x201E, 0x000B);
        }

        public void JogReduce()
        {
            master.WriteSingleRegister(slaveAddress, 0x201E, 0x000C);
        }



    }

}
