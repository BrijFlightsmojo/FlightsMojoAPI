using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Travelopedia.TravelopediaClass.TempBookingRequest
{
    public class Travelopedia_TempBookingRequest
    {
        public AuthHeader Auth_Header { get; set; }
        public string Customer_Mobile { get; set; }
        public string Passenger_Mobile { get; set; }
        public string WhatsAPP_Mobile { get; set; }
        public string Passenger_Email { get; set; }
        public List<PAXDetail> PAX_Details { get; set; }
        public bool GST { get; set; }
        public string GST_Number { get; set; }
        public string GST_HolderName { get; set; }
        public string GST_Address { get; set; }
        public List<BookingFlightDetail> BookingFlightDetails { get; set; }
        public int CostCenterId { get; set; }
        public int ProjectId { get; set; }
        public string BookingRemark { get; set; }
        public int CorporateStatus { get; set; }
        public int CorporatePaymentMode { get; set; }
        public string MissedSavingReason { get; set; }
        public string CorpTripType { get; set; }
        public string CorpTripSubType { get; set; }
        public string TripRequestId { get; set; }
        public string BookingAlertIds { get; set; }
    }
    public class AuthHeader
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string IP_Address { get; set; }
        public string Request_Id { get; set; }
        public string IMEI_Number { get; set; }
    }
    public class BookingFlightDetail
    {
        public string Search_Key { get; set; }
        public string Flight_Key { get; set; }
        public List<string> BookingSSRDetails { get; set; }
    }
    public class PAXDetail
    {
        public int Pax_Id { get; set; }
        public int Pax_type { get; set; }
        public string Title { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public int Gender { get; set; }
        public string Age { get; set; }
        public string DOB { get; set; }
        public string Passport_Number { get; set; }
        public string Passport_Issuing_Country { get; set; }
        public string Passport_Expiry { get; set; }
        public string Nationality { get; set; }
        public string FrequentFlyerDetails { get; set; }
    }
}
