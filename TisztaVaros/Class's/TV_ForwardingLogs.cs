using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TisztaVaros
{
    class TV_ForwardingLogs
    {
        public string id { get; set; }
        public string reportId { get; set; }
        public string forwardedToId { get; set; }
        public string forwardedFromId { get; set; }
        public string forwardedByUserId { get; set; }
        public string createdAt { get; set; }
        public string forwardedAt { get; set; }
        public string reason { get; set; }
        public TVR_FW_From forwardedFrom { get; set; }
        public TVR_FW_To forwardedTo { get; set; }
        public TVR_FW_User forwardedByUser { get; set; }    

        public class TVR_FW_From
        {
            public string id { get; set; }
            public string name { get; set; }
        }
        public class TVR_FW_To
        {
            public string id { get; set; }
            public string name { get; set; }
        }
        public class TVR_FW_User
        {
            public string id { get; set; }
            public string username { get; set; }
            public string email { get; set; }
        }
    }
}
