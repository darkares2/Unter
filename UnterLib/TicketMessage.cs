using System;
using System.Collections.Generic;
using System.Text;

namespace UnterLib
{
    public class TicketMessage
    {
        public Guid clientId { get; set; }
        public UInt64 sequence { get; set; }
    }
}
