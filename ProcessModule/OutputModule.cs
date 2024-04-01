using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp3.Component;

namespace WindowsFormsApp3.Initial_Model
{
    public class OutputModule
    {

        //----Digital Output----

        //壓蓋氣缸
        private DigitalOutput pressDownCoverCylinder;
        //收納氣缸
        private DigitalOutput storageCylinder;

        //----Digital Iutput----

        //壓蓋氣缸-推
        private DigitalIntput pressDownCoverCylinderPushSignal;
        //壓蓋氣缸-收
        private DigitalIntput pressDownCoverCylinderPullSignal;
        //收納氣缸-推
        private DigitalIntput storageCylinderCylinderPushSignal;
        //收納氣缸-收
        private DigitalIntput storageCylinderCylinderPullSignal;


        //軸控





        public OutputModule(DigitalOutput[] signalOutput, DigitalIntput[] signalInput,
           IAxis axisCarrierSlideTable, IAxis axisMedicineBottleElevator,
           IAxis axisScrewMedicineCap, IAxis axisMedicineBottleDump, IBarcodeReader boxReader)
        {







        }
    }
}
