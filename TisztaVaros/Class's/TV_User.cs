using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;

namespace TisztaVaros
{
    internal class TV_User
    {
        public TV_User() { }
        public TV_User(string nev, string email, string psw, string c_psw)
        {
            this.username = nev;
            this.email = email;
            this.psw = psw;
            this.c_psw = c_psw;
        }

        // 'id', 'username', 'email', 'points', 'role', 'isActive', 'createdAt', 'updatedAt', "institutionId", 'zipCode', 'city', 'address',
        // 'avatarSeed', 'avatarStyle', 'avatarChangesToday', 'lastAvatarChangeDate'
        public string id { get; set; }
        public string token { get; set; }
        public string username { get; set; }
        public string v_username
        {
            get { return "  " + username + "  "; }
        }
        public string email { get; set; }
        public string psw { get; set; }
        public string c_psw { get; set; }
        public int points { get; set; }
        public string role { get; set; }
        public string isActive { get; set; }
        public string v_isActive
        {
            get { return "  " + isActive + "  "; }
        }
        public string c_row
        {
            get
            {
                if (isActive == "archived") return "LightGray";
                else if (isActive == "inactive") return "#FFFFACAC";
                else return "White";
            }
        }

        public string createdAt { get; set; }
        public string createdAtHH
        {
            get
            {
                if (createdAt == null) { return ""; }
                else { return createdAt.Substring(0, 16).Replace('T', ' ').Replace('-', '.'); }
            }
            set { }
        }
        public string updatedAt { get; set; }
        public string institutionId { get; set; }
        public string institution { get; set; }
        public string zipCode { get; set; }
        public string city { get; set; }
        public string address { get; set; }
        public string avatarSeed { get; set; }
        public string avatarStyle { get; set; }
        public int avatarChangesToday { get; set; }
        public string lastAvatarChangeDate { get; set; }
        public string message { get; set; }

        public static List<TV_User> List_User = new List<TV_User>();
        public static List<Grid> allGrid = new List<Grid>();
    }
}
