using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.MetaResponse
{
    [DataContract]
    public class SkyscannerRankingResponse
    {
        [DataMember]
        public DateTime session_created_on { get; set; }
        [DataMember]
        public string currency { get; set; }
        [DataMember]
        public List<Place> places { get; set; }
        [DataMember]
        public string sorting_type { get; set; }
        [DataMember]
        public List<Leg> legs { get; set; }
        [DataMember]
        public List<Segment> segments { get; set; }
        [DataMember]
        public List<Itinerary> itineraries { get; set; }
        [DataMember]
        public SearchResultSummary search_result_summary { get; set; }
        [DataMember]
        public SearchInfo search_info { get; set; }
        [DataMember]
        public string uuid { get; set; }
    }
    [DataContract]
    public class Itinerary
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public List<string> leg_ids { get; set; }
        [DataMember]
        public Position position { get; set; }
        [DataMember]
        public int total_pricing_options { get; set; }
        [DataMember]
        public List<PricingOption> pricing_options { get; set; }
    }
    [DataContract]
    public class Leg
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public List<string> segment_ids { get; set; }
        [DataMember]
        public int origin_place_id { get; set; }
        [DataMember]
        public int destination_place_id { get; set; }
        [DataMember]
        public string date { get; set; }
    }
    [DataContract]
    public class Place
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string display_code { get; set; }
        [DataMember]
        public int place_id { get; set; }
    }
    [DataContract]
    public class Position
    {
        [DataMember]
        public int cheapest_sorting { get; set; }
    }
    [DataContract]
    public class PricingOption
    {
        [DataMember]
        public int rank { get; set; }
        [DataMember]
        public double price { get; set; }
        [DataMember]
        public string provider_type { get; set; }
    }
    [DataContract]
    public class SearchInfo
    {
        [DataMember]
        public List<Leg> legs { get; set; }
        [DataMember]
        public string market { get; set; }
        [DataMember]
        public int no_of_adults { get; set; }
        [DataMember]
        public int no_of_children { get; set; }
        [DataMember]
        public int no_of_infants { get; set; }
        [DataMember]
        public string locale { get; set; }
        [DataMember]
        public string cabin_class { get; set; }
        [DataMember]
        public string currency { get; set; }
        [DataMember]
        public string search_kind { get; set; }
    }
    [DataContract]
    public class SearchResultSummary
    {
        [DataMember]
        public int itinerary_count { get; set; }
    }
    [DataContract]
    public class Segment
    {
        [DataMember]
        public string id { get; set; }
        [DataMember]
        public int origin_place_id { get; set; }
        [DataMember]
        public int destination_place_id { get; set; }
        [DataMember]
        public DateTime departure { get; set; }
        [DataMember]
        public DateTime arrival { get; set; }
        [DataMember]
        public string marketing_flight_number { get; set; }
        [DataMember]
        public string marketing_carrier_id { get; set; }
    }
}
