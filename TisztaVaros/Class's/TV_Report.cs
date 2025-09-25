using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TisztaVaros
{
    class TV_Report
    {
        public TV_Report() { }

        public int id { get; set; }
        public string view_id
        {
            get { return id.ToString("0000"); }
            set { }
        }
        public string userId { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string categoryId { get; set; }
        public string address { get; set; }
        public string zipCode { get; set; }
        public string city { get; set; }
        public float locationLat { get; set; }
        public float locationLng { get; set; }
        public string status { get; set; }
        public string institutionId { get; set; }
        public string createdAt { get; set; }
        public string createdAtHH
        {
            get { return createdAt.Substring(0, 16).Replace('T', ' '); }
            set { }
        }
        public List<TVR_Images> reportImages { get; set; }
        public string firstPictURL
        {
            get
            {
                if (reportImages[0].imageUrl == "") { return "https://smd.hu/Team/placeholderimage.png"; }
                else if (App.local_y) { return TV_Admin.pict_path + reportImages[0].imageUrl; }
                else { return "https://tisztavaros.hu" + reportImages[0].imageUrl; }
            }
        }
        public string confirmed { get; set; }
        public string pict_db { get; set; }
        public TVR_User user { get; set; }
        public string username { get { return user.username; } }
        public TVR_Category category { get; set; }
        public string kategoria { get { return category.categoryName; } }
        public List<TVR_VotesTypes> reportVotes { get; set; } //= new List<TVR_VotesTypes>();
        public TRV_Inst institution { get; set; }
        public string intezmeny { get { return institution.name; } }
    }
    class TVR_Images
    {
        public TVR_Images() { }
        public string id { get; set; }
        public int reportId { get; set; }
        public string imageUrl { get; set; }
    }
    public class TVR_User
    {
        public string username { get; set; }
        public string avatarStyle { get; set; }
        public string avatarSeed { get; set; }
    }
    class TVR_Category
    {
        public string categoryName { get; set; }
    }
    class TVR_VotesTypes
    {
        public string voteType { get; set; }
        public string userId { get; set; }
    }
    class TRV_Inst
    {
        public string name { get; set; }
    }
}
