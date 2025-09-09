using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel.DataAnnotations;

namespace TisztaVaros
{
    internal static class ValidEmail
    {
        public static bool IsValid(string email)
        {
            if (email.EndsWith(".")) { return false; }
            string[] e_parts = email.Split('@');
            if (e_parts.Length == 1) { return false; }
            string domain_part = e_parts[e_parts.Length - 1];
            string local_part = email.Substring(0, email.Length - domain_part.Length - 1);
            string[] d_parts = domain_part.Split('.');

            if (d_parts.Length != 2) { return false; }
            //if (e_parts.Length > 2) { return false; }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
