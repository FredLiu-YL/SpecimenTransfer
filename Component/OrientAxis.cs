using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3.Component
{
    public class OrientAxis : IAxis
    {

        public OrientAxis(string comport)
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
    }
}
