using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.AirIQ.AirIQClass
{
    public class TokenResponse
    {
        public string token { get; set; }
        public string token_type { get; set; }
        public User user { get; set; }
        public int expiration { get; set; }
    }


    public class User
    {
        public int agency_id { get; set; }
        public string agency_name { get; set; }
        public string contact_person { get; set; }
        public string city { get; set; }
        public object country { get; set; }
        public double balance { get; set; }
        public string email_id { get; set; }
        public string mobile_no { get; set; }
    }
}
