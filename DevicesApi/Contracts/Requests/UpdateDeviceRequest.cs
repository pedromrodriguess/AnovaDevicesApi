﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevicesApi.Contracts.Requests
{
    public class UpdateDeviceRequest
    {
        public string Name { get; set; }
        public string Location { get; set; }
    }
}