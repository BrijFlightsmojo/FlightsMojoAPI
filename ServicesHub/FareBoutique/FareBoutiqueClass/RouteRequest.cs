using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.FareBoutique.FareBoutiqueClass
{
 public   class RouteRequest
    {
       
            public string trip_type { get; set; }
            public string end_user_ip { get; set; }
            public string token { get; set; }
            public int with_aiq { get; set; }
        
    }
    public  class RouteDateRequest
    {
        public string trip_type { get; set; }
        public string departure_city_code { get; set; }
        public string arrival_city_code { get; set; }
        public string end_user_ip { get; set; }
        public string token { get; set; }
    }
}
