using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3.Component
{
    public interface IDigitalSignalController
    {

         void SignalOutputOn();
         void SignalOutputOff();

         bool SignalInput();

    }



    public class DigitalOutput
    {
        IDigitalSignalController controller;
        public DigitalOutput(IDigitalSignalController controller)
        {

            this.controller = controller;
        }

        public void On()
        {

            controller.SignalOutputOn();
        }
        public void Off()
        {
            controller.SignalOutputOff();

        }
    }

    public class DigitalIntput
    {

        IDigitalSignalController controller;
        public DigitalIntput(IDigitalSignalController controller)
        {

            this.controller = controller;
        }

        public bool Signal => controller.SignalInput();

    }
}
