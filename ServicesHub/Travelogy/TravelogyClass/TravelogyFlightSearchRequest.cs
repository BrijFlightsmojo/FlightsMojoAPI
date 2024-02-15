using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Travelogy.TravelogyClass
{
    public  class TravelogyFlightSearchRequest
    {
        public AuthHeader Auth_Header { get; set; }
        public int Travel_Type { get; set; }
        public int Booking_Type { get; set; }
        public List<TripInfo> TripInfo { get; set; }
        public string Adult_Count { get; set; }
        public string Child_Count { get; set; }
        public string Infant_Count { get; set; }
        public string Class_Of_Travel { get; set; }
        public int InventoryType { get; set; }
        public List<FilteredAirline> Filtered_Airline { get; set; }
    }
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class AuthHeader
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string IP_Address { get; set; }
        public string Request_Id { get; set; }
        public string IMEI_Number { get; set; }
    }

    public class FilteredAirline
    {
        public string Airline_Code { get; set; }
    }
    public class TripInfo
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string TravelDate { get; set; }
        public int Trip_Id { get; set; }
    }


}
