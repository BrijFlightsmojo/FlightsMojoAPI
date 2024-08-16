using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tripshope.TripshopeClass
{
    public class FareQuoteRequest
    {
        public Pricingrequest pricingrequest { get; set; }
    }

    public class Pricingrequest
    {
        public Credentials credentials { get; set; }
        public string domint { get; set; }
        public string selectedflight { get; set; }
        public string selectedflight_return { get; set; }
        public string wsmode { get; set; }
    }

    //public class Credentials
    //{
    //    public string officeid { get; set; }
    //    public string username { get; set; }
    //    public string password { get; set; }
    //}

    public class SSRRequest
    {
        public Ssrrequest ssrrequest { get; set; }
    }

    public class Ssrrequest
    {
        public Credentials credentials { get; set; }
        public string selectedflight { get; set; }
        public string triptype { get; set; }
        public string wsmode { get; set; }
    }

}
