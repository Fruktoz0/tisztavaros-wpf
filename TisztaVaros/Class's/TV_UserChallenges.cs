using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TisztaVaros
{
    class TV_UserChallenges
    {
        public string id { get; set; }
        public string userId { get; set; }
        public int challengeId { get; set; }
        public string status { get; set; }
        public int pointsEarned { get; set; }
        public string description { get; set; }
        public string image1 { get; set; }
        public string image2 { get; set; }
        public string image3 { get; set; }
        public string createdAt { get; set; }
        public string createdAtHH
        {
            get { return createdAt.Substring(0, 16).Replace('T', ' '); }
            set { }
        }
        public TVR_User user { get; set; }

    }
}
