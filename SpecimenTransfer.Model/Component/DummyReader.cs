using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecimenTransfer.Model.Component
{
    public class DummyReader : IBarcodeReader
    {
        private string temp = " Dummy test";
        public string ReceiveData()
        {
            return temp;
        }
    }
}
