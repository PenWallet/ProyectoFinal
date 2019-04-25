﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CHAIRAPI_Entidades.Persistent
{
    public class IPBan
    {
        public string IP { get; set; }
        public string banReason { get; set; }
        public DateTime untilDate { get; set; }

        public IPBan(string IP, string banReason, DateTime untilDate)
        {
            this.IP = IP;
            this.banReason = banReason;
            this.untilDate = untilDate;
        }

        public IPBan()
        {
        }
    }
}
