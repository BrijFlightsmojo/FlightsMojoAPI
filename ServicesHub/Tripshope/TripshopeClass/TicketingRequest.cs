using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tripshope.TripshopeClass
{
    public class TicketingRequest
    {
        //public Bookingrequest bookingrequest { get; set; }
        public Bookingrequest bookingrequest { get; set; }
    }

    public class Bookingrequest
    {
        public string taxInfoStr { get; set; }
        public string leademail { get; set; }
        public string leadmobile { get; set; }
        public string leadcountry { get; set; }
        public string leadcity { get; set; }
        public string leadstate { get; set; }
        public string paymentmode { get; set; }
        public string domint { get; set; }
        public Credentials credentials { get; set; }
        public int numadults { get; set; }
        public int numchildren { get; set; }
        public int numinfants { get; set; }
        public List<PassengerList> passengerList { get; set; }
        public string wsmode { get; set; }
    }


    public class PassengerList
    {
        public string title { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string dob { get; set; }
        public string type { get; set; }
        public string tour_code { get; set; }
        public string deal_code { get; set; }
        public string frequent_flyer_number { get; set; }
        public string visa { get; set; }
        public string passport { get; set; }
        public string passport_dateofexpiry { get; set; }
        public string passport_dateofissue { get; set; }
        public string passport_placeofissue { get; set; }
        public string agentid { get; set; }
        public string meal_preference { get; set; }
        public string seat_preference { get; set; }
        public string additional_segmentinfo { get; set; }
        public int profileid { get; set; }
    }

   

    //public class Bookingrequest
    //{
    //    public string taxInfoStr { get; set; }
    //    public string leademail { get; set; }
    //    public string leadmobile { get; set; }
    //    public string leadcountry { get; set; }
    //    public string leadcity { get; set; }
    //    public string leadstate { get; set; }
    //    public string paymentmode { get; set; }
    //    public string domint { get; set; }
    //    public Credentials credentials { get; set; }
    //    public Gstdetails gstdetails { get; set; }
    //    public int numadults { get; set; }
    //    public int numchildren { get; set; }
    //    public int numinfants { get; set; }
    //    public List<PassengerList> passengerList { get; set; }
    //    public SsrList ssrList { get; set; }
    //    public string wsmode { get; set; }
    //}

    //public class Credentials
    //{
    //    public string agentid { get; set; }
    //    public string officeid { get; set; }
    //    public string password { get; set; }
    //    public string username { get; set; }
    //}

    //public class Gstdetails
    //{
    //    public string gstnumber { get; set; }
    //    public string gstemail { get; set; }
    //    public string gstcompany { get; set; }
    //    public string gstaddress { get; set; }
    //    public string gstphone { get; set; }
    //}

    //public class PassengerList
    //{
    //    public string title { get; set; }
    //    public string first_name { get; set; }
    //    public string last_name { get; set; }
    //    public string dob { get; set; }
    //    public string type { get; set; }
    //    public string tour_code { get; set; }
    //    public string deal_code { get; set; }
    //    public string frequent_flyer_number { get; set; }
    //    public string visa { get; set; }
    //    public string passport { get; set; }
    //    public string passport_dateofexpiry { get; set; }
    //    public string passport_dateofissue { get; set; }
    //    public string passport_placeofissue { get; set; }
    //    public string agentid { get; set; }
    //    public string meal_preference { get; set; }
    //    public string seat_preference { get; set; }
    //    public string additional_segmentinfo { get; set; }
    //    public List<Paxssrinfo> paxssrinfo { get; set; }
    //    public int profileid { get; set; }
    //}


    //public class SsrList
    //{
    //    public List<SSRRow> SSRRow { get; set; }
    //}

    //public class SSRRow
    //{
    //    public string carrier { get; set; }
    //    public string destination { get; set; }
    //    public string jtype { get; set; }
    //    public string origin { get; set; }
    //    public string passenger_fname { get; set; }
    //    public string passenger_lname { get; set; }
    //    public string passenger_title { get; set; }
    //    public int paxid { get; set; }
    //    public string paxtype { get; set; }
    //    public string ssr_chargeableamount { get; set; }
    //    public string ssrcode { get; set; }
    //    public string ssrname { get; set; }
    //    public string ssrtype { get; set; }
    //}

    //public class Paxssrinfo
    //{
    //    public string carrier { get; set; }
    //    public string destination { get; set; }
    //    public string jtype { get; set; }
    //    public string origin { get; set; }
    //    public string passenger_fname { get; set; }
    //    public string passenger_lname { get; set; }
    //    public string passenger_title { get; set; }
    //    public int paxid { get; set; }
    //    public string paxtype { get; set; }
    //    public string ssr_chargeableamount { get; set; }
    //    public string ssrcode { get; set; }
    //    public string ssrname { get; set; }
    //    public string ssrtype { get; set; }
    //}

}
