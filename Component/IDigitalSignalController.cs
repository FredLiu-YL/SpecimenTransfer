using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Automation.BDaq;

namespace WindowsFormsApp3.Component
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
        void DigitalOutCommand(int number,bool trigger);
        /// <summary>
        /// 實作輸入訊號
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        bool DigitalInCommand(int number);
        

    }

    public class DigitalOutput
    {
        public DigitalOutput(int i, ADTech_USB4750 aDTech_USB4750)
        {
        }

        internal void On()
        {
            throw new NotImplementedException();
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
