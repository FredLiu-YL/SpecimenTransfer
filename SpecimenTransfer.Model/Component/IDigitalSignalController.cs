using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automation.BDaq;

namespace SpecimenTransfer.Model.Component
{

    public interface IDigitalSignalController
    {
        /// <summary>
        /// Do
        /// </summary>
        DigitalOutput[] SignalOutput { get; }
        /// <summary>
        /// DI
        /// </summary>
        DigitalIntput[] SignalInput { get; }



        /// <summary>
        /// 實做輸出訊號
        /// </summary>
        /// <param name="number"></param>
        /// <param name="trigger"></param>
        void DigitalOutCommand(int number, bool trigger);
        /// <summary>
        /// 實作輸入訊號
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        bool DigitalInCommand(int number);


    }

    public class DigitalOutput
    {
        private IDigitalSignalController controller;
        private int number;
        private bool isSwitchOn;


        public DigitalOutput(int number, IDigitalSignalController controller)
        {
            this.number = number;
            this.controller = controller;
        }

        public bool IsSwitchOn { get => isSwitchOn; }


        public void Switch(bool trigger)
        {
            if (trigger)
            {
                On(number);
                isSwitchOn = true;

            }

            else
            {
                Off(number);
                isSwitchOn = false;
            }
             Task.Delay(100).Wait();
        }

        private void On(int number)
        {
            controller.DigitalOutCommand(number, true);
        }
        private void Off(int number)
        {
            controller.DigitalOutCommand(number, false);

        }


    }


    public class DigitalIntput
    {
        private IDigitalSignalController controller;
        private int number;
        public DigitalIntput(int number, IDigitalSignalController controller)
        {

            this.number = number;
            this.controller = controller;

        }

        public bool Signal => controller.DigitalInCommand(number);




    }
}
