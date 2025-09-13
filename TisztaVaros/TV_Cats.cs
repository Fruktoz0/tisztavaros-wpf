using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TisztaVaros
{
    class TV_Cats
    {
        public string id { get; set; }
        public string categoryName { get; set; }
        public string defaultinstitutionId { get; set; }
        public string description { get; set; }
        public string createdAt { get; set; }
        public TV_Inst institution { get; set; }
        public string linked_inst
        {
            get { return institution.name; }
        }
        public string linked_inst_desc
        {
            get { return institution.description; }
        }
        public string linked_inst_contact
        {
            get { return institution.contactInfo; }
        }
        public string createdAtHH
        {
            get { return createdAt.Substring(0,16).Replace('T',' '); }
            //set { }
        }
    }
}
