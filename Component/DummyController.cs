using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp3.Component
{
    public class DummyController : IDigitalSignalController
    {
        private bool[] tempInputSignal;//模擬 Input訊號 ，紀錄現在訊號

        private int outputCount; //暫時沒想到output模擬需要怎樣呈現 ，資訊暫時留著
        public DummyController(int inputCount, int outputCount)
        {
            List<bool> inputs = new List<bool>();
            for (int i = 0; i < inputCount; i++)
                inputs.Add(false);


            tempInputSignal = inputs.ToArray();
            this.outputCount = outputCount;
        }

        public DigitalOutput[] SignalOutput => CreateOutputs();

        public DigitalIntput[] SignalInput => CreateInputs();

        public bool DigitalInCommand(int number)
        {

            return tempInputSignal[number];
        }

        public void DigitalOutCommand(int number, bool trigger)
        {

        }


        private DigitalIntput[] CreateInputs()
        {
            return tempInputSignal.Select((b, i) => new DigitalIntput(i, this)).ToArray();

        }
        private DigitalOutput[] CreateOutputs()
        {
            List<DigitalOutput> outputs = new List<DigitalOutput>();
            for (int i = 0; i < outputCount; i++)
            {
                outputs.Add(new DigitalOutput(i, this));
            }
            return outputs.ToArray();
        }
    }
}
