using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.GFS.GFSFareQuoteRequestClass
{
   public class FareQuoteRequest
    {
        public Query query { get; set; }
        public List<string> flight_keys { get; set; }
        public decimal total_price { get; set; }
        public string currency { get; set; }
    }

    public class Leg
    {
        public string dst { get; set; }
        public string src { get; set; }
        public string dep { get; set; }
    }

    public class Query
    {
        public int nAdt { get; set; }
        public int nInf { get; set; }
        public int nChd { get; set; }
        public List<Leg> legs { get; set; }
    }
}
