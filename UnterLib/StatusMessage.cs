using System;
using System.Collections.Generic;
using System.Text;

namespace UnterLib
{
    public class StatusMessage
    {
        public Guid clientId { get; set; }
        public int status { get; set; }
        public DateTime timestamp { get; set; }
    }
}
