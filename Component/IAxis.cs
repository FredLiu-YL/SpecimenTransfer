using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3.Component
{
    public interface IAxis
    {



        double Position { get; set; }

        double Acceleration { get; }

        double Deceleration { get; }

        double FinalVelocity { get; }

        double NEL { get; set; }
        double PEL { get; set; }

        AxisStatus Status { get; }


        void Home();
        void Postion();



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
