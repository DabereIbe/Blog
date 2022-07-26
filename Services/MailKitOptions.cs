using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Services
{
    public class MailKitOptions
    {
        public string smtpserver { get; set; }
        public int portnumber { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public bool trustedconnection { get; set; }
        public string email { get; set; }
        public string name { get; set; }
    }
}
