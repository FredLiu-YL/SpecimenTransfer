using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
 
using Modbus.Device;
using Newtonsoft.Json.Linq;

namespace SpecimenTransfer.Model.Component
{
    public class OrientAxis : IAxis
    {
        private SerialPort serialPort;
        private ModbusSerialMaster master;
        private byte slaveAddress;  
        public bool IsInposition => Isinpos();

        public double Position => GetPosition();

        public double PEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double NEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsBusy => throw new NotImplementedException();

        public OrientAxis(string comport, int driverID)
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

        public bool Isinpos()
        {
            ushort[] rotaRegisters2 = master.ReadHoldingRegisters(1, 0x007F, 0x0001);
            bool rotaMotorINP = (rotaRegisters2[0] & (1 << 14)) != 0; // 檢查bit14
            return rotaMotorINP;                                                      

        }


        public void SetVelocity(double finalVelocity, double accelerationTime, double decelerationTime)
        {
            SetFinalSpeed(finalVelocity);
            
            //要在幾秒內到達最高速度  例0.1秒到達10000的速度  Max= 10000  加速就是10萬  = 10000 /0.1   
            var accSpeed = finalVelocity / accelerationTime;
            var decSpeed = finalVelocity / decelerationTime;

            SetACCSpeed((int)accSpeed, (int)decSpeed);
        }

        public void MoveToAsync(double position )
        {
            try
            {
                SetOperationMode(OperationMode.Absolute);

                byte[] posBytes = BitConverter.GetBytes((int)position);
                ushort posValueH = BitConverter.ToUInt16(posBytes, 2);
                ushort posValueL = BitConverter.ToUInt16(posBytes, 0);


                byte[] up = new byte[] { 0x82, 0x18 };//按照陣列排列 下位 需要放前面 ， 上位在後面
                byte[] add = new byte[] { 0x83, 0x18 };

                ushort upAddress = BitConverter.ToUInt16(up, 0);
                master.WriteSingleRegister(slaveAddress, upAddress, posValueH);

                ushort Address = BitConverter.ToUInt16(add, 0);

                master.WriteSingleRegister(slaveAddress, Address, (ushort)(posValueL - 1));

                master.WriteSingleRegister(slaveAddress, 0x7D, 0x0A);
                CommandReset();

                SpinWait.SpinUntil(Isinpos, 2000);
            }
            catch (Exception)
            {

                throw;
            }
           
            
        }

        public void MoveAsync(double distance)
        {
            SetOperationMode(OperationMode.Relative);
            //定義位置移動需要的 基準座標  地址
            byte[] up = new byte[] { 0x82, 0x18 };//按照陣列排列 下位 需要放前面 ， 上位在後面
            ushort upAddress = BitConverter.ToUInt16(up, 0);

            //定義位置移動需要的 目標座標  地址
            byte[] add = new byte[] { 0x83, 0x18 };//按照陣列排列 下位 需要放前面 ， 上位在後面 P.363頁
            ushort Address = BitConverter.ToUInt16(add, 0);


            if (distance >= 0)
            {

                byte[] distanceBytes = BitConverter.GetBytes((int)distance);
                ushort distValueH = BitConverter.ToUInt16(distanceBytes, 2);
                ushort distValueL = BitConverter.ToUInt16(distanceBytes, 0);


                //正向
                master.WriteSingleRegister(slaveAddress, upAddress, distValueH);
                master.WriteSingleRegister(slaveAddress, Address, (ushort)(distValueL - 1));

            }
            else
            {

                var dis = -distance; //東方沒有 方向移動不能吃負號 ，所以反向必須調整成 正值 
                byte[] distanceBytes = BitConverter.GetBytes((int)dis);
                ushort distValueH = BitConverter.ToUInt16(distanceBytes, 2);
                ushort distValueL = BitConverter.ToUInt16(distanceBytes, 0);

                //負向
                master.WriteSingleRegister(slaveAddress, upAddress, (ushort)(65535 - distValueH));
                master.WriteSingleRegister(slaveAddress, Address, (ushort)(65535 - distValueL));
            }


            // SetSpeed((ushort)AxisSpeed);
            //馬達運行  啟動第2組設定
            master.WriteSingleRegister(slaveAddress, 0x7D, 0x0A);
          
            CommandReset();

            SpinWait.SpinUntil(Isinpos, 2000);
        }



        public void Home()
        {
            try
            {
                if (master == null) return;
                ushort[] m_zhome = { 0x01, 0x06, 0x00, 0x7D, 0x00, 0x10, 0x18, 0x1E }; //ZHOME


                byte[] byte1 = BitConverter.GetBytes(0x00);
                byte[] byte2 = BitConverter.GetBytes(0x7D);
                byte[] byteAddress = new byte[] { byte1[0], byte1[1], byte2[0], byte2[2] };
                ushort Address = BitConverter.ToUInt16(byteAddress, 0);

                master.WriteSingleRegister(slaveAddress, 0x7D, 0x10);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Stop()
        {
            try
            {
                if (master == null) throw new Exception("Device is null");


                master.WriteSingleRegister(slaveAddress, 0x7D, 0x20);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AlarmReset()
        {
            ResetAlarmCode();
        }
      
        private void CommandReset()
        {
            master.WriteSingleRegister(slaveAddress, 0x7D, 0x00);
        }

        private void ResetAlarmCode()
        {

            master.WriteSingleRegister(slaveAddress, 384, 12480);
            master.WriteSingleRegister(slaveAddress, 385, 12480);

        }
        private int GetAlarmCode()
        {
            //回傳  01 03 02 00 31 79 90   CODE在陣列4的位置 例30 過負載 ， 31超速   P425頁
            var temp = master.ReadHoldingRegisters(slaveAddress, 0x81, 0x01);



            return temp[4];
        }
        private int GetPosition()
        {
            var datas = master.ReadHoldingRegisters(slaveAddress, 0xCC, 0x02);

            var pos = datas[0] * 65536 + datas[1];

            return pos;
        }
        private void SetOperationMode(OperationMode mode)
        {
            byte[] add = new byte[] { 0x81, 0x18 };//設定運轉模式 位置
            ushort address1 = BitConverter.ToUInt16(add, 0);

            switch (mode)
            {

                case OperationMode.Absolute:
                    master.WriteSingleRegister(slaveAddress, address1, 0x01);//絕對位置
                    break;

                case OperationMode.Relative:
                    master.WriteSingleRegister(slaveAddress, address1, 0x03);//相對位置
                    break;


            }


        }

        private void SetFinalSpeed(double speed)
        {

            var uspeedBytes = BitConverter.GetBytes((int)speed);


            byte[] addH = new byte[] { 0x84, 0x18 };  //速度由 32位元組成  [84,85] 但1次只能寫16位元 所以要分兩次下參數
            byte[] addL = new byte[] { 0x85, 0x18 };

            ushort addressH = BitConverter.ToUInt16(addH, 0);
            ushort addressL = BitConverter.ToUInt16(addL, 0);

            ushort valueH = BitConverter.ToUInt16(uspeedBytes, 2);
            ushort valueL = BitConverter.ToUInt16(uspeedBytes, 0);


            master.WriteSingleRegister(slaveAddress, addressH, valueH);
            master.WriteSingleRegister(slaveAddress, addressL, valueL);
        }
        private void SetACCSpeed(int accSpeed, int decSpeed)
        {
            var accspeedByte = BitConverter.GetBytes(accSpeed);
            var decspeedByte = BitConverter.GetBytes(decSpeed);

            //加速度由 32位元組成  [86,87] 但1次只能寫16位元 所以要分兩次下參數
            byte[] accSpeedAddressH = new byte[] { 0x86, 0x18 }; //高位元
            byte[] accSpeedAddressL = new byte[] { 0x87, 0x18 };  //低位元
            ushort accaddressH = BitConverter.ToUInt16(accSpeedAddressH, 0);
            ushort accaddressL = BitConverter.ToUInt16(accSpeedAddressL, 0);
            ushort accValueH = BitConverter.ToUInt16(accspeedByte, 2);
            ushort accValueL = BitConverter.ToUInt16(accspeedByte, 0);

            master.WriteSingleRegister(slaveAddress, accaddressH, Convert.ToUInt16(accValueH));
            master.WriteSingleRegister(slaveAddress, accaddressL, Convert.ToUInt16(accValueL));



            //減速度由 32位元組成  [88,89] 但1次只能寫16位元 所以要分兩次下參數
            byte[] decSpeedAddressH = new byte[] { 0x88, 0x18 };//高位元
            byte[] decSpeedAddressL = new byte[] { 0x89, 0x18 };//低位元
            ushort decaddressH = BitConverter.ToUInt16(decSpeedAddressH, 0);
            ushort decaddressL = BitConverter.ToUInt16(decSpeedAddressL, 0);
            ushort decValueH = BitConverter.ToUInt16(decspeedByte, 2);
            ushort decValueL = BitConverter.ToUInt16(decspeedByte, 0);
            master.WriteSingleRegister(slaveAddress, decaddressH, Convert.ToUInt16(decValueH));
            master.WriteSingleRegister(slaveAddress, decaddressL, Convert.ToUInt16(decValueL));
        }

        public void Home(double axisCoverAndStorageElevatorHomePos)
        {
            throw new NotImplementedException();
        }
    }

    public enum OperationMode
    {
        Relative,
        Absolute


    }
}
