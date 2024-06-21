using Modbus.Device;
using Modbus.Extensions.Enron;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        private object lockobj = new object();

        private bool _toyoStatus;

        public double Position => GetPosition();

        public bool IsHome => Isinhome();

        public bool IsInposition => Isinpos();

        public double PEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double NEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsBusy => Move();


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

                //建立MODBUS主站通訊
                master = ModbusSerialMaster.CreateRtu(serialPort);
            }

            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }



        private bool Isinpos()
        {

            //Read the data from the PLC
            var status = master.ReadHoldingRegisters(slaveAddress, 0x1001, 0x0001);

            //Check the first byte of data
            if (status[0] == 0)
            {
                _toyoStatus = false;
            }
            else
            {
                _toyoStatus = true;
            }

            return _toyoStatus;
        }


        public bool Isinhome()
        {

            ushort[] rotaRegisters = master.ReadHoldingRegisters(slaveAddress, 0x1020, 0x0001);
            bool rotaMotorHome = (rotaRegisters[0] & (1 << 1)) != 0; // // 檢查bit1
            return rotaMotorHome;
        }

        public async Task Home()
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

        public async Task SetVelocity(double finalVelocity, double acceleration, double deceleration)
        {
        

                ushort moveSpeedSet = (ushort)Convert.ToInt16(finalVelocity);//移動速度轉型
                master.WriteSingleRegister(slaveAddress, 0x2014, moveSpeedSet);//移動速度設定，單位0~100%
          
        }

        public async Task MoveToAsync(double pos)
        {
        
                try
                {
                    //  ushort absAmount = (ushort)Convert.ToInt32(pos);//絕對移動量轉型
                    var uspeedBytes = BitConverter.GetBytes((int)pos / 10);

                    ushort valueH = BitConverter.ToUInt16(uspeedBytes, 2);
                    ushort valueL = BitConverter.ToUInt16(uspeedBytes, 0);

                    //    master.WriteSingleRegister(slaveAddress, 0x2002, absAmount);//ABS絕對移動量輸入，單位0.01mm/1pulse
                    ushort[] vs = { valueH, valueL };
                    //   ushort[] vs = new ushort[] {  0x0000 , 0x03E8 };
                    master.WriteMultipleRegisters(slaveAddress, 0x2002, vs);
                    //        master.WriteMultipleRegisters(slaveAddress, 0x2000, vs);
                    master.WriteSingleRegister(slaveAddress, 0x201E, 0x0001);//選擇移動類型:ABS絕對移動
                                                                             //SpinWait.SpinUntil(Isinpos, 2000);

                }
                catch (Exception error)
                {

                    MessageBox.Show(error.Message);
                }
          

        }

        public void MoveAsync(double distance)
        {
       
                try
                {
                    ushort incAmount = (ushort)Convert.ToInt16(distance);//相對移動量轉型
                    master.WriteSingleRegister(slaveAddress, 0x201E, 0x0000);//選擇移動類型:INC相對移動
                    master.WriteSingleRegister(slaveAddress, 0x2002, incAmount);//INC相對移動量輸入，單位0.01mm/1pulse

                    var pos = GetPosition();
                    //await Isinpos(pos+ distance);
                    // SpinWait.SpinUntil(Isinpos , 2000);

                }

                catch (Exception error)
                {
                    MessageBox.Show(error.Message);

                }
            

        }


        public async Task<bool> Isinpos( double targetpPos)
        {
       

                // 命令和地址
                //ushort[] slideInpos = master.ReadHoldingRegisters(1, 0x0179, 0x0001);
                //bool statusLamp = (slideInpos[0] & (1 << 8)) != 0; // 檢查bit6
                //return statusLamp;
                // 根據馬達是否運行來更新UI
                //lampSlideBusy.BackColor = statusLamp ? Color.Red : Color.Lime;

                /*
                var response = master.ReadHoldingRegisters(1, 0x1020, 0x0001);
               // var response = master.ReadInputRegisters(1, 0x1020, 0x0001);
                string binaryString = Convert.ToString(response[0], 2);//轉二進制字串

                bool inPosition = Convert.ToBoolean(response);
                return inPosition;
                */
                int timeOut = 0;
                var pos = GetPosition();
                while (Math.Abs(targetpPos - pos) >= 5)
                {

                    pos = GetPosition();
                    Thread.Sleep(30);
                    timeOut++;
                    if (timeOut > 200)
                        throw new Exception("Axis Timeout");

                }

                return true;
          

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
        
        //jog+
        public void JogPlusMosueDown()
        {
          
                master.WriteSingleRegister(slaveAddress, 0x2014, 0x0014);
                master.WriteSingleRegister(slaveAddress, 0x201E, 0x000B);
         
        }

        public void JogPlusMouseUp()
        {
         
                master.WriteSingleRegister(slaveAddress, 0x201E, 0x0009);
           
        }

        //jog-
        public void JogReduceMosueDown()
        {
         
                master.WriteSingleRegister(slaveAddress, 0x2014, 0x0014);
                master.WriteSingleRegister(slaveAddress, 0x201E, 0x000C);
          
            
        }

        public void JogReduceMouseUp()
        {
         
                master.WriteSingleRegister(slaveAddress, 0x201E, 0x0009);
           
        }


        public double GetPosition()
        {
            var datas = master.ReadHoldingRegisters(slaveAddress, 0x1008, 0x0002);

             int pos =(datas[0] * 65536 + datas[1])*10;

            return pos;

        }

        public double GetVelocity()
        {
            var datas = master.ReadHoldingRegisters(slaveAddress, 0x1006, 0x01);

            //var velicoty = datas[0] * 65536 + datas[1];
            var velicoty = datas[0];

            return velicoty;
        }

    }

}
