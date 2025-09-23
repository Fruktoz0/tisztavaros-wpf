using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TisztaVaros
{
    class TV_StatusHistory
    {
        public string status { get; set; }
        public string changedAt { get; set; }
        public TVR_User changedBy { get; set; }
        public string comment { get; set; }
        public string changedAtHH
        {
            get { return changedAt.Substring(0, 16).Replace('T', ' '); }
            set { }
        }
    }
}
