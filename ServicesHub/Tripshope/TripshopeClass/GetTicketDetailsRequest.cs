using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tripshope.TicketDetailsRequest
{
   public class GetTicketDetailsRequest
    {
        public NextraGetItineraryRequest NextraGetItineraryRequest { get; set; }
    }

    public class Credentials
    {
        public string officeid { get; set; }
        public string password { get; set; }
        public string username { get; set; }
    }

    public class NextraGetItineraryRequest
    {
        public Credentials credentials { get; set; }
        public string txid { get; set; }
        public string wsmode { get; set; }
    }


}
