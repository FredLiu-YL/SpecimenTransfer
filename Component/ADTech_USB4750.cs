using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3.Component
{
    public class ADTech_USB4750 : IDigitalSignalController
    {

        public ADTech_USB4750(string portName)
        {


            Initial(portName);

        }

        private void Initial(string portName)
        {
            SerialPort serialPort = new SerialPort();


        }


        /// <summary>
        /// DO 
        /// </summary>
        public DigitalOutput[] SignalOutput => GetDigitalOut();
        /// <summary>
        /// DI
        /// </summary>
        public DigitalIntput[] SignalInput => GetDigitalIn();

        public bool DigitalInCommand(int number)
        {
            //要補上實作
            throw new NotImplementedException();
        }

        public void DigitalOutCommand(int number, bool trigger)
        {
            //要補上實作
        }

        private DigitalOutput[] GetDigitalOut()
        {
            List<DigitalOutput> switches = new List<DigitalOutput>();
            for (int i = 0; i < 16; i++)
            {

                switches.Add(new DigitalOutput(i,this));
            }
            return switches.ToArray();
        }
        private DigitalIntput[] GetDigitalIn()
        {
            List<DigitalIntput> dis = new List<DigitalIntput>();
            for (int i = 0; i < 16; i++)
            {

                dis.Add(new DigitalIntput(i, this));
            }
            return dis.ToArray();
        }
    }
}
