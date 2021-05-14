using System;

namespace UnterLib
{
    public class LocationMessage
    {
        public Guid clientId { get; set; }
        public GeoData location { get; set; }
        public DateTime timestamp { get; set; }
    }
}
