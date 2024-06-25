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


        public bool IsInposition => true;

        public double Position => simulateTempPosition;

        public double PEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double NEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsBusy => throw new NotImplementedException();

        bool IAxis.IsBusy => throw new NotImplementedException();

        bool IAxis.IsInposition => throw new NotImplementedException();


        double IAxis.Position { get => throw new NotImplementedException(); }
        double IAxis.PEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        double IAxis.NEL { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        bool IAxis.IsHome => throw new NotImplementedException();

        public void AlarmReset()
        {
            throw new NotImplementedException();
        }

        public double GetPosition()
        {
            throw new NotImplementedException();
        }

        public async Task Home()
        {
            simulateTempPosition = 0;

        }

        public async Task Home(double axisCoverAndStorageElevatorHomePos)
        {
            throw new NotImplementedException();
        }

        public void JogPlusMosueDown()
        {
            throw new NotImplementedException();
        }

        public void JogPlusMouseUp()
        {
            throw new NotImplementedException();
        }

        public void JogReduceMosueDown()
        {
            throw new NotImplementedException();
        }

        public void JogReduceMouseUp()
        {
            throw new NotImplementedException();
        }

        public void MoveAsync(double distance)
        {
            simulateTempPosition += distance;
        }

        public async Task MoveToAsync(double pos)
        {
            simulateTempPosition = pos;
        }

        public async Task SetVelocity(double finalVelocity, double acceleration, double deceleration)
        {
            
        }

        public void Stop()
        {
            
        }

        double IAxis.GetVelocity()
        {
            throw new NotImplementedException();
        }
    }
}
