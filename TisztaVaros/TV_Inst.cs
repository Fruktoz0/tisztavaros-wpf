using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TisztaVaros
{
    class TV_Inst
    {
        //public TV_Inst() { }

        public string id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string description { get; set; }
        public string contactInfo { get; set; }
        public string logo { get; set; }
        public string logoUrl { get; set; }
        public string createdAt { get; set; }
        public string createdAtHH
        {
            get { return createdAt.Substring(0, 16).Replace('T', ' '); }
        }
    }
}
