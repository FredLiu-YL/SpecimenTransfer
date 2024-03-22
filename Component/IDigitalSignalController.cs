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
        void DigitalOutCommand(int port, int bit, byte signalSwitch);
        /// <summary>
        /// 實作輸入訊號
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        void DigitalInCommand(int port, int bit, out byte portData);

    
    }

    public class DigitalIntput
    {
        
    

    }

    public class DigitalOutput
    {



    }
}
