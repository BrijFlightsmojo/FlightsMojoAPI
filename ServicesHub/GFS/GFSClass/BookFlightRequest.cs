using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.GFS.GFS_BookRequest
{
   public class BookFlightRequest
    {
        public Query query { get; set; }
        public List<string> flight_keys { get; set; }
        public decimal total_price { get; set; }
        public string currency { get; set; }
        public List<Paxis> paxes { get; set; }
        public ClientDetails client_details { get; set; }
        public string agent_reference { get; set; }
    }

    public class ClientDetails
    {
        public string email { get; set; }
        public string phone { get; set; }
    }

    public class Leg
    {
        public string dst { get; set; }
        public string src { get; set; }
        public string dep { get; set; }
    }

    public class Paxis
    {
        public string title { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string type { get; set; }
        public string dob { get; set; }
        public string nationality { get; set; }
        public string passport_num { get; set; }
    }

    public class Query
    {
        public int nAdt { get; set; }
        public int nInf { get; set; }
        public int nChd { get; set; }
        public List<Leg> legs { get; set; }
    }
}
