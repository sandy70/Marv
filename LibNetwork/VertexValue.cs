﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace LibNetwork
{
    public class VertexValue : Dictionary<string, double> 
    {
        public bool IsEvidenceEntered { get; set; }
    }
}