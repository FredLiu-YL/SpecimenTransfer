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
        //上方夾藥罐氣缸-打開訊號
        private DigitalIntput upperClampMedicineCylinderOpenSignal;
        //上方夾藥罐氣缸-關閉訊號
        private DigitalIntput upperClampMedicineCylinderCloseSignal;




    }
}
