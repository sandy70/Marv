﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPipeline
{
    public interface IPoint
    {
        double X { get; set; }
        double Y { get; set; }
        double Value { get; set; }
    }
}
