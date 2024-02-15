using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.SatkarTravel.SatkarTravelClass
{
    public class search
    {
        public string onwarddate { get; set; }
        public string returndate { get; set; }
        public string resultCategory { get; set; }
        public string prefclass { get; set; }
        public List<Segment> segments { get; set; }
        public int adultCount { get; set; }
        public int childCount { get; set; }
        public int infantCount { get; set; }
        public string endUserIp { get; set; }
        public string journeyType { get; set; }
        public List<string> sources { get; set; }
        public string domIntFlag { get; set; }
    }
    public class Segment
    {
        public string origin { get; set; }
        public string destination { get; set; }
        public string flightCabinClass { get; set; }
        public DateTime preferredDepartureTime { get; set; }
    }

    public class fare_quote
    {
        public List<string> resultSessionId { get; set; }
    }


    public class book_flight
    {
        public List<Passenger> passengers { get; set; }
        public string resultSessionId { get; set; }
        public bool lcc { get; set; }
        public string contactNo { get; set; }
        public string email { get; set; }
        public bool gSTDetailsRequired { get; set; }
    }

    public class Passenger
    {
        public int paxType { get; set; }
        public string title { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string addressLine1 { get; set; }
        public string city { get; set; }
        public string countryCode { get; set; }
        public string nationality { get; set; }
        public int postalCode { get; set; }
        public int gender { get; set; }
        public int paxId { get; set; }
        public bool isLeadPax { get; set; }
        public string dateOfBirth { get; set; }
        public string passportNo { get; set; }
        public string passportExpiry { get; set; }
        public string passportIssueDate { get; set; }
    }


    public class Token
    {
        public string username { get; set; }
        public string password { get; set; }
        public string auth_channel { get; set; }
    }



    public class booking_details
    {
     
    }


 
}
