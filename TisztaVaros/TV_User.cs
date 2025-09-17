using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

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
        public string email { get; set; }
        public string psw { get; set; }
        public string c_psw { get; set; }
        public int points { get; set; }
        public string role { get; set; }
        public string c_role { get 
            {
                string out_color = "";
                if (role == "user") out_color = "White";
                else if (role == "admin") out_color = "MediumSpringGreen";
                else if (role == "inspector") out_color = "Yellow";
                else if (role == "institution") out_color = "Orange";
                return out_color;
            }
        }
        public string isActive { get; set; }

        public string c_isActive {
            get 
            {
                string out_color = "";
                if (isActive == "archived") out_color = "LightGray";
                else if (isActive =="active") out_color = "White";
                else out_color = "LightGray";
                return out_color;
            }
        }
        public string c_row { 
            get
            {
                string out_color = "";
                if (isActive == "archived") out_color = "LightGray";
                else if (role == "user") out_color = "White";
                else if (role == "admin") out_color = "MediumSpringGreen";
                else if (role == "inspector") out_color = "Yellow";
                else if (role == "institution") out_color = "Orange";
                return out_color;
            }
        }

        public string createdAt { get; set; }
        public string createdAtHH
        {
            get { return createdAt.Substring(0, 16).Replace('T', ' ').Replace('-','.'); }
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
