﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
 

namespace SpecimenTransfer.Model.Component
{
    public interface IBarcodeReader
    {
        string ReceiveData();

    }

   
 
}
