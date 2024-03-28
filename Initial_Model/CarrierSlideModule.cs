using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsFormsApp3.Component;

namespace WindowsFormsApp3.Initial_Model
{
    class CarrierSlideModule
    {
        //----軸控----
        //載體滑台回-Home
        private IAxis carrierSlideHomeAxis;
        //載體滑台-移動
        private IAxis carrierSlideMoveAxis;
        //載體滑台到-ready
        private IAxis carrierSlideAxisReady;

            public CarrierSlideModule( IAxis carrierSlideTableAxis)
            {
                //----軸控----
                carrierSlideHomeAxis = carrierSlideTableAxis;//載體滑台回Home
                carrierSlideMoveAxis = carrierSlideTableAxis;//載體滑台移動
                carrierSlideAxisReady = carrierSlideTableAxis;//載體滑台到位準備訊號
            }

            //載體滑台移動至載體盒站
            public async Task MoveToCarrierBox()
            {
                carrierSlideMoveAxis.MoveAsync(1000);//載體滑台移動至載體盒站
                await Task.Delay(1000);

            }

            //載體滑台移動至濾紙站
            public async Task MoveToFilterPaper()
            {
                carrierSlideMoveAxis.MoveAsync(2000);//載體滑台移動至濾紙站
                await Task.Delay(1000);
            }

       
            //載體盒移動至注射站
            public async Task MoveToDump()
            {
                carrierSlideMoveAxis.MoveAsync(3000);//載體滑台移動至注射站

            }

        
    }

}
