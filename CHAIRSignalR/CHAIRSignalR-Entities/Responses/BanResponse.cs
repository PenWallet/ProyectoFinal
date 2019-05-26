using CHAIRSignalR_Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CHAIRSignalR_Entities.Responses
{
    /// <summary>
    /// A simple class made so I can send responses to the client
    /// </summary>
    public class BanResponse
    {
        public BannedByEnum bannedBy { get; set; } //IP or User
        public string banReason { get; set; }
        public DateTime bannedUntil { get; set; }

        public BanResponse(BannedByEnum bannedBy, string banReason, DateTime bannedUntil)
        {
            this.bannedBy = bannedBy;
            this.banReason = banReason;
            this.bannedUntil = bannedUntil;
        }

        public BanResponse()
        {
        }
    }
}
