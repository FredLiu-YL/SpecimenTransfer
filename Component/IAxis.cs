using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3.Component
{
    public interface IAxis
    {

        /*    
            void motorHome(byte slaveAddress, ushort registerAddress, ushort value);
            void motorPostionNumber(byte slaveAddress, ushort registerAddress, ushort value);
            void motorJogForwardDirection(byte slaveAddress, ushort registerAddress, ushort value);
            void motorJogReverseDirection(byte slaveAddress, ushort registerAddress, ushort value);
            void motorStart(byte slaveAddress, ushort registerAddress, ushort value);
            void motorStop(byte slaveAddress, ushort registerAddress, ushort value);
            void motorAlarmReset(byte slaveAddress, ushort registerAddress, ushort value);
    */


        bool Isinpos { get; }
        void Home();
        void Stop();
        void SetVelocity(double finalVelocity, double acceleration, double deceleration);
      
        void MoveToAsync(double pos);

        void MoveAsync(double distance);

        
    }


    public enum AxisStatus
    {
        Origin,
        Alarm,

    }
}
