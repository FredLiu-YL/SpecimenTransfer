using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3.Component
{
    public class DummyAxis : IAxis
    {
        private double simulateTempPosition;//暫存紀錄 虛擬軸位置
        public DummyAxis()
        {


        }

        public bool IsInposition => true;

        public double Position => simulateTempPosition;

        public void Home()
        {
            simulateTempPosition = 0;
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
    }
}
