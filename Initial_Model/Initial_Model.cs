using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsFormsApp3.Component;
using System.IO;



namespace WindowsFormsApp3.Initial_Model
{
    public class Initial_Model
    {
        
        ///TOYO AXIS
        //濾紙升降
        private IAxis axisFilterPaperLifting;
        //藥罐升降
        private IAxis axisMedicineTankLifting;
        //載台滑台
        private IAxis axisCarriageSlide;
        //蓋子滑台
        private IAxis axisCoverSlide;

        ///Origentalmotor Axis
        //藥罐旋轉
        private IAxis axisMedicineJarRotation;
        //藥罐傾倒
        private IAxis axisMedicineJarTippedOver;

        //Barcode reader
        private IBarcodeReader barcodeReader;


        ///USB-4750 16IN 16OUT
        //DI

        ///掃描站
        //載體盒Reader
        private DigitalIntput carrierReaderResultOK;
        private DigitalIntput carrierReaderResultNG;
        private DigitalIntput carrierReaderResultBusy;
        //藥罐Reader
        private DigitalIntput medcineReaderResultOK;
        private DigitalIntput medcineReaderResultNG;
        private DigitalIntput medcineReaderResultBusy;

        ///載體盒站
        //載體氣缸
        private DigitalIntput carrierCylinderFront;
        private DigitalIntput carrierCylinderBack;

        ///濾紙站
        //濾紙氣缸
        private DigitalIntput filterPaperCylinderFront;
        private DigitalIntput filterPaperCylinderBack;

        ///注入站
        //注入氣缸
        private DigitalIntput injectionCylinderFront;
        private DigitalIntput injectionCylinderBack;

        /// 藥罐站
        //藥罐氣缸
        private DigitalIntput medcineCylinderFront;
        private DigitalIntput medcineCylinderBack;
        //藥罐上夾爪
        private DigitalIntput clampAboveMedicineOpen;
        private DigitalIntput clampAboveMedicineClose;
        //藥罐下夾爪
        private DigitalIntput clampUnderMedicineOpen;
        private DigitalIntput clampUnderMedicineClose;

        /// 背光站
        //背光氣缸
        private DigitalIntput backlightCylinderFront;
        private DigitalIntput backlightCylinderBack;

        //紅墨水站
        //蓋子氣缸
        private DigitalIntput coverCylinderUP;
        private DigitalIntput coverCylinderDown;

        ///壓蓋子站
        //壓蓋氣缸
        private DigitalIntput pushGlandCylinderFront;
        private DigitalIntput pushGlandCylinderBack;

        //收納站
        //收納氣缸
        private DigitalIntput storageCylinderFront;
        private DigitalIntput storageCylinderBack;

        


        //DO
        ///掃描站
        //載體盒Reader
        //藥瓶Reader
        private DigitalOutput carrierReaderTrigger;
        private DigitalOutput medcineReaderTrigger;

        ///載體盒站
        //載體氣缸
        private DigitalOutput carrierCylinderOn;

        ///濾紙站
        //濾紙氣缸
        private DigitalOutput filterPaperCylinderOn;

        ///注入站
        //注入氣缸
        //注射
        private DigitalOutput injectionCylinderOn;
        private DigitalOutput injectionOn;

        //藥罐站
        //上夾爪
        //下夾爪
        private DigitalOutput clampAboveMedicineOn;
        private DigitalOutput clampUnderMedicineOn;

        //相機站
        //相機拍照
        private DigitalOutput cameraPhotographOn;

        /// 背光站
        //背光氣缸
        private DigitalOutput backlightCylinderOn;

        //紅墨水站
        //蓋子氣缸
        private DigitalOutput coverCylinderOn;

        ///壓蓋子站
        //壓蓋氣缸
        private DigitalOutput pushGlandCylinderOn;

        //收納站
        //收納氣缸
        private DigitalOutput storageCylinderOn;


        public Initial_Model(IAxis axisFilterPaperLifting, IAxis axisMedicineTankLifting, IAxis axisCarriageSlide, IAxis axisCoverSlide,
            IAxis axisMedicineJarRotation, IAxis axisMedicineJarTippedOver, IBarcodeReader barcodeReader, DigitalIntput[] digitalsIn, DigitalOutput[] digitalsOut)
        {
            this.axisFilterPaperLifting = axisFilterPaperLifting;
            this.axisMedicineTankLifting = axisMedicineTankLifting;
            this.axisCarriageSlide = axisCarriageSlide;
            this.axisCoverSlide = axisCoverSlide;
            this.axisMedicineJarRotation = axisMedicineJarRotation;
            this.axisMedicineJarTippedOver = axisMedicineJarTippedOver;
            this.barcodeReader = barcodeReader;


            //DI
            carrierReaderResultOK = digitalsIn[0];
            carrierReaderResultNG = digitalsIn[1];
            carrierReaderResultBusy = digitalsIn[2];
            medcineReaderResultOK = digitalsIn[3];
            medcineReaderResultNG = digitalsIn[4];
            medcineReaderResultBusy = digitalsIn[5];

            carrierCylinderFront = digitalsIn[6];
            carrierCylinderBack = digitalsIn[7];

            filterPaperCylinderFront = digitalsIn[8];
            filterPaperCylinderBack = digitalsIn[9];

            injectionCylinderFront = digitalsIn[10];
            injectionCylinderBack = digitalsIn[11];

            medcineCylinderFront = digitalsIn[12];
            medcineCylinderBack = digitalsIn[13];

            clampAboveMedicineOpen = digitalsIn[14];
            clampAboveMedicineClose = digitalsIn[15];

            clampUnderMedicineOpen = digitalsIn[16];
            clampUnderMedicineClose = digitalsIn[17];

            backlightCylinderFront = digitalsIn[18];
            backlightCylinderBack = digitalsIn[19];

            coverCylinderUP = digitalsIn[20];
            coverCylinderDown = digitalsIn[21];

            pushGlandCylinderFront = digitalsIn[22];
            pushGlandCylinderBack = digitalsIn[23];

            storageCylinderFront = digitalsIn[24];
            storageCylinderBack = digitalsIn[25];


            //DO
            carrierReaderTrigger = digitalsOut[0];
            medcineReaderTrigger = digitalsOut[1];
             
            carrierCylinderOn = digitalsOut[2];
            filterPaperCylinderOn = digitalsOut[3];

            injectionCylinderOn = digitalsOut[4];
            injectionOn = digitalsOut[5];

            clampAboveMedicineOn = digitalsOut[6];
            clampUnderMedicineOn = digitalsOut[7];
            cameraPhotographOn = digitalsOut[8];

            backlightCylinderOn = digitalsOut[9];

            coverCylinderOn = digitalsOut[10];

            pushGlandCylinderOn = digitalsOut[11];

            storageCylinderOn = digitalsOut[12];



    }

  
        



    }
}
