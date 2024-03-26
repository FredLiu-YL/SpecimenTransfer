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

        /// <summary>
        /// 到位訊號
        /// </summary>
        bool IsInposition { get; }

        /// <summary>
        /// 當前位置
        /// </summary>
        double Position { get; }

        /// <summary>
        /// 原點復歸
        /// </summary>
        void Home();

        /// <summary>
        /// 停止
        /// </summary>
        void Stop();

        void SetVelocity(double finalVelocity, double acceleration, double deceleration);


        /// <summary>
        /// 絕對移動
        /// </summary>
        /// <param name="pos"></param>
        void MoveToAsync(double pos);


        /// <summary>
        /// 相對移動
        /// </summary>
        /// <param name="distance"></param>
        void MoveAsync(double distance);


    }


    public enum AxisStatus
    {
        Origin,
        Alarm,

    }
}
