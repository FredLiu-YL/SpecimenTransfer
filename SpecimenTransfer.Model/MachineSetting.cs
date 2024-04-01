using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SpecimenTransfer.Model
{
    public class MachineSetting: AbstractRecipe
    {

        /// <summary>
        /// 檢體盒卡匣升降軸起點位置(第一個盒子的位置)
        /// </summary>
        public double BoxCassetteElevatorStartPos { get; set; }



        /// <summary>
        /// 檢體盒卡匣內間隔 (mm)
        /// </summary>
        public double BoxCassetteElevatorPitch { get; set; }

        /// <summary>
        /// 推送軸將檢體盒推出卡匣的座標
        /// </summary>
        public double BoxCassettePushPos { get; set; }
        /// <summary>
        /// 大橫移軸入料位置
        /// </summary>
        public double TransferLoadPos { get; set; }
        /// <summary>
        /// 大橫移軸 傾倒站位置
        /// </summary>
        public double TransferDumpPos { get; set; }
        /// <summary>
        /// 大橫移軸出料位置
        /// </summary>
        public double TransferUnLoadPos { get; set; }


        /// <summary>
        /// 傾倒站 轉開蓋子位置
        /// </summary>
        public double DumpTurnOnPos { get; set; }
        /// <summary>
        /// 傾倒站 傾倒軸 倒出檢體位置
        /// </summary>
        public double DumpPourOutPos { get; set; }
    }
}
