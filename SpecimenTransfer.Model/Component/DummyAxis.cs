using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpecimenTransfer.Model.Component
{
    public class DummyAxis : IAxis
    {
        private double simulateTempPosition;//暫存紀錄 虛擬軸位置
        public DummyAxis()
        {


        }

        public bool IsInposition => true;

        public double Position => simulateTempPosition;

        public double PEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double NEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsBusy => throw new NotImplementedException();

        bool IAxis.IsBusy => throw new NotImplementedException();

        bool IAxis.IsInposition => throw new NotImplementedException();

        double IAxis.Position => throw new NotImplementedException();

        double IAxis.PEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        double IAxis.NEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void AlarmReset()
        {
            throw new NotImplementedException();
        }

        public void Home()
        {
            simulateTempPosition = 0;

        }

        public void Home(double axisCoverAndStorageElevatorHomePos)
        {
            throw new NotImplementedException();
        }

        public void MoveAsync(double distance)
        {
            simulateTempPosition += distance;
        }

        public void MoveToAsync(double pos)
        {
            simulateTempPosition = pos;
        }

        public void SetVelocity(double finalVelocity, double acceleration, double deceleration)
        {
            
        }

        public void Stop()
        {
            
        }

        void IAxis.AlarmReset()
        {
            throw new NotImplementedException();
        }

        void IAxis.Home()
        {
            throw new NotImplementedException();
        }

        void IAxis.JogAdd()
        {
            throw new NotImplementedException();
        }



        void IAxis.JogReduce()
        {
            throw new NotImplementedException();
        }

        void IAxis.MoveAsync(double distance)
        {
            throw new NotImplementedException();
        }

        void IAxis.MoveToAsync(double pos)
        {
            throw new NotImplementedException();
        }

        void IAxis.SetVelocity(double finalVelocity, double accelerationTime, double decelerationTime)
        {
            throw new NotImplementedException();
        }

        void IAxis.Stop()
        {
            throw new NotImplementedException();
        }
    }
}
