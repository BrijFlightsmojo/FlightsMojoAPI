using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.FareBoutique.FareBoutiqueClass
{
    public class FBRoutesResponse
    {
        public string replyCode { get; set; }
        public string replyMsg { get; set; }
        public List<DataRoutes> data { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
    }

    public class DataRoutes
    {
        public string dep_airport_name { get; set; }
        public string dep_airport_code { get; set; }
        public string arr_airport_name { get; set; }
        public string arr_airport_code { get; set; }
        public string dep_city_name { get; set; }
        public string dep_city_code { get; set; }
        public string arr_city_name { get; set; }
        public string arr_city_code { get; set; }
    }
}
