using SpecimenTransfer.Model.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecimenTransfer.Model.ProcessModule
{
    public class TranferModule
    {
        IAxis transAxis;
        bool isBusy;
        object lockobj;

        public TranferModule(IAxis transAxis)
        {

            this.transAxis = transAxis;
        }



        public async Task MoveTo(double pos)
        {
            if (isBusy) throw new Exception();

            transAxis.MoveToAsync(pos);




        }
    }
}
