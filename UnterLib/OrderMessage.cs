using System;
using System.Collections.Generic;
using System.Text;

namespace UnterLib
{
    public class OrderMessage
    {
        public Guid clientId { get; set; }
        public GeoData location { get; set; }
        public DateTime timestamp { get; set; }
        public DateTime expiration { get; set; }
    }
}
