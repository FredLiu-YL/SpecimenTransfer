﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpecimenTransfer.Model.Component
{


    public interface IAxis
    {
        bool IsHome { get; }

        bool IsBusy { get; }

        /// <summary>
        /// 到位訊號
        /// </summary>
        bool IsInposition { get; }

        /// <summary>
        /// 當前位置
        /// </summary>
        double Position { get; }
        /// <summary>
        /// 軟體正極限
        /// </summary>
        double PEL { get; set; }
        /// <summary>
        /// 軟體負極限
        /// </summary>
        double NEL { get; set; }


        /// <summary>
        /// 監控位置
        /// </summary>
        /// <returns></returns>
        double GetPosition();

        /// <summary>
        /// 監控速度
        /// </summary>
        /// <returns></returns>
        double GetVelocity();

        /// <summary>
        /// 原點復歸
        /// </summary>
        Task Home();

        /// <summary>
        /// 停止
        /// </summary>
        void Stop();

        /// <summary>
        /// 設定速度
        /// </summary>
        /// <param name="finalVelocity">最高速度</param>
        /// <param name="accelerationTime">到達最高速要多久(S) 建議0.1-0.2秒</param>
        /// <param name="decelerationTime">到停止要多久(S) 建議0.1-0.2秒</param>
        Task SetVelocity(double finalVelocity, double accelerationTime, double decelerationTime);


        /// <summary>
        /// 絕對移動
        /// </summary>
        /// <param name="pos"></param>
        Task MoveToAsync(double pos);


        /// <summary>
        /// 相對移動
        /// </summary>
        /// <param name="distance"></param>
        void  MoveAsync(double distance);

        /// <summary>
        /// 異常清除
        /// </summary>
        void AlarmReset();


        /// <summary>
        /// Jog+
        /// </summary>
        void JogPlusMosueDown();


        void JogPlusMouseUp();

        /// <summary>
        /// Jog-
        /// </summary>
        void JogReduceMosueDown();

        void JogReduceMouseUp();



    }


    public enum AxisStatus
    {
        Origin,
        Alarm,

    }
}
