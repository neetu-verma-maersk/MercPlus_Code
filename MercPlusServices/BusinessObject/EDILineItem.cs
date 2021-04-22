﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace MercPlusServiceLibrary.BusinessObjects
{
    
    public class EDILineItem
    {
        
        public int EDIId { get; set; }
        
        public Nullable<int> LineNo { get; set; }
        
        public string LineDetail { get; set; }
    }
}