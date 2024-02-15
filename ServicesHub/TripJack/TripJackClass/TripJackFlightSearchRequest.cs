using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.TripJack.TripJackClass
{
    public  class TripJackFlightSearchRequest
    {
        public SearchQuery searchQuery { get; set; }
    }
    public class SearchQuery
    {
        public string cabinClass { get; set; }
        public PaxInfo paxInfo { get; set; }
        public List<RouteInfo> routeInfos { get; set; }
        public SearchModifiers searchModifiers { get; set; }
    }
    public class PaxInfo
    {
        public string ADULT { get; set; }
        public string CHILD { get; set; }
        public string INFANT { get; set; }
    }
    public class RouteInfo
    {
        public CityOrAirport fromCityOrAirport { get; set; }
        public CityOrAirport toCityOrAirport { get; set; }
        public string travelDate { get; set; }
    }
    public class CityOrAirport
    {
        public string code { get; set; }
    }
    public class SearchModifiers
    {
        public bool isDirectFlight { get; set; }
        public bool isConnectingFlight { get; set; }
    }
}
