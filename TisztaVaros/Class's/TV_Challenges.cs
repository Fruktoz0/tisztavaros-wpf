using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TisztaVaros.Class_s
{
    class TV_Challenges
    {
        public int id { get; set; }
        public string view_id
        {
            get { return id.ToString("0000"); }
            set { }
        }
        public string institutonId { get; set; }
        public string inst_name { get; set; }
        public string institutionId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public string viewPictURL { get {
                if (image == "") { return "https://smd.hu/Team/placeholderimage.png"; }
                else if (App.local_y) { return TV_Admin.pict_path + image; }
                else { return "https://tisztavaros.hu" + image; }
            } 
        }
        public string category { get; set; }
        public string costPoints { get; set; }
        public string rewardPoints { get; set; }
        public string status { get; set; }
        public string startDate { get; set; }
        public string startDateHH
        {
            get { return startDate.Substring(0, 16).Replace('T', ' '); }
            set { }
        }
        public string endDate { get; set; }
        public string endDateHH
        {
            get { return endDate.Substring(0, 16).Replace('T', ' '); }
            set { }
        }
        public string createdAt { get; set; }
        public string createdAtHH
        {
            get { return createdAt.Substring(0, 16).Replace('T', ' '); }
            set { }
        }
    }

}
