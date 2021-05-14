using System;
using System.Collections.Generic;
using System.Text;

namespace UnterLib
{
    public class OrderAcceptMessage
    {
        public Guid clientId { get; set; }
        public GeoData location { get; set; }
        public DateTime timestamp { get; set; }
        public OrderMessage order { get; set; }
    }
}
