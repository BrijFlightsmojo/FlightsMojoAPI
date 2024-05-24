using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Travelopedia.TravelopediaClass.Ticket_Invoice
{
    public class TravelopediaTicketInvoiceResponse
    {
        public int Adult_Count { get; set; }
        public List<AirPNRDetail> AirPNRDetails { get; set; }
        public string AirRequeryResponse_Key { get; set; }
        public int Biller_Id { get; set; }
        public List<BookingPaymentDetail> BookingPaymentDetail { get; set; }
        public string Booking_DateTime { get; set; }
        public string Booking_RefNo { get; set; }
        public int Booking_Type { get; set; }
        public int Child_Count { get; set; }
        public int Class_of_Travel { get; set; }
        public CompanyDetail CompanyDetail { get; set; }
        public int CorporatePaymentMode { get; set; }
        public CustomerDetail CustomerDetail { get; set; }
        public bool GST { get; set; }
        public string GSTIN { get; set; }
        public int Infant_Count { get; set; }
        public string Invoice_Number { get; set; }
        public string PAX_EmailId { get; set; }
        public string PAX_Mobile { get; set; }
        public string Remark { get; set; }
        public ResponseHeader Response_Header { get; set; }
        public RetailerDetail RetailerDetail { get; set; }
        public int Travel_Type { get; set; }
    }

    public class AirportTaxis
    {
        public int Tax_Amount { get; set; }
        public string Tax_Code { get; set; }
        public string Tax_Desc { get; set; }
    }
    public class CancellationCharge
    {
        public int Applicablility { get; set; }
        public int DurationFrom { get; set; }
        public int DurationTo { get; set; }
        public int DurationTypeFrom { get; set; }
        public int DurationTypeTo { get; set; }
        public int OfflineServiceFee { get; set; }
        public int OnlineServiceFee { get; set; }
        public int PassengerType { get; set; }
        public string Remarks { get; set; }
        public bool Return_Flight { get; set; }
        public string Value { get; set; }
        public int ValueType { get; set; }
    }
    public class AirPNRDetail
    {
        public string Airline_Code { get; set; }
        public object Airline_Name { get; set; }
        public string Airline_PNR { get; set; }
        public List<object> BookingChangeRequests { get; set; }
        public string CRS_Code { get; set; }
        public string CRS_PNR { get; set; }
        public object FailureRemark { get; set; }
        public List<Flight> Flights { get; set; }
        public decimal Gross_Amount { get; set; }
        public List<PAXTicketDetail> PAXTicketDetails { get; set; }
        public int Post_Markup { get; set; }
        public string Record_Locator { get; set; }
        public int RetailerPostMarkup { get; set; }
        public string Supplier_RefNo { get; set; }
        public string Ticket_Status_Desc { get; set; }
        public string Ticket_Status_Id { get; set; }
        public string TicketingDate { get; set; }
    }
    public class PAXTicketDetail
    {
        public string Age { get; set; }
        public string DOB { get; set; }
        public List<Fare> Fares { get; set; }
        public string First_Name { get; set; }
        public object FrequentFlyerDetails { get; set; }
        public int Gender { get; set; }
        public string Last_Name { get; set; }
        public string Nationality { get; set; }
        public string Passport_Expiry { get; set; }
        public string Passport_Issuing_Country { get; set; }
        public string Passport_Number { get; set; }
        public int Pax_Id { get; set; }
        public int Pax_type { get; set; }
        public List<object> SSRDetails { get; set; }
        public List<TicketDetail> TicketDetails { get; set; }
        public string TicketStatus { get; set; }
        public string Title { get; set; }
    }
    public class BookingPaymentDetail
    {
        public string Currency_Code { get; set; }
        public int Gateway_Charges { get; set; }
        public string PaymentConfirmation_Number { get; set; }
        public int Payment_Amount { get; set; }
        public int Payment_Mode { get; set; }
    }
    public class CompanyDetail
    {
        public string Address { get; set; }
        public string City { get; set; }
        public string CompanyName { get; set; }
        public string ContactPerson { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string Fax { get; set; }
        public string GSTNo { get; set; }
        public string Logo { get; set; }
        public string MobileNo { get; set; }
        public string PANNo { get; set; }
        public string PhoneNo { get; set; }
        public string Pincode { get; set; }
        public string SACCode { get; set; }
        public string State { get; set; }
        public string Website { get; set; }
    }
    public class CustomerDetail
    {
        public string Customer_Email { get; set; }
        public string Customer_Mobile { get; set; }
        public string Customer_Name { get; set; }
        public string GSTAddress { get; set; }
        public string GSTHolderName { get; set; }
        public string GST_Number { get; set; }
    }
    public class RetailerDetail
    {
        public string BookedBy_Operator_Name { get; set; }
        public string Operator_Name { get; set; }
        public string Retailer_Address { get; set; }
        public string Retailer_Area { get; set; }
        public string Retailer_City { get; set; }
        public string Retailer_CompanyName { get; set; }
        public string Retailer_Email_Id { get; set; }
        public string Retailer_GSTno { get; set; }
        public string Retailer_Id { get; set; }
        public string Retailer_Landmark { get; set; }
        public string Retailer_Mobile_Number { get; set; }
        public string Retailer_Name { get; set; }
        public string Retailer_PANno { get; set; }
        public string Retailer_Pincode { get; set; }
        public string Retailer_State { get; set; }
        public string Sub_Retailer_CompanyName { get; set; }
        public string Sub_Retailer_Id { get; set; }
    }
    public class Fare
    {
        public List<FareDetail> FareDetails { get; set; }
        public int FareType { get; set; }
        public object Fare_Id { get; set; }
        public object Fare_Key { get; set; }
        public string Food_onboard { get; set; }
        public bool GSTMandatory { get; set; }
        public object LastFewSeats { get; set; }
        public string ProductClass { get; set; }
        public object PromptMessage { get; set; }
        public bool Refundable { get; set; }
        public object Seats_Available { get; set; }
        public string Warning { get; set; }
    }

    public class FareClass
    {
        public string Class_Code { get; set; }
        public string Class_Desc { get; set; }
        public string FareBasis { get; set; }
        public object Privileges { get; set; }
        public int Segment_Id { get; set; }
    }

    public class FareDetail
    {
        public int AirportTax_Amount { get; set; }
        public List<AirportTaxis> AirportTaxes { get; set; }
        public int Basic_Amount { get; set; }
        public List<CancellationCharge> CancellationCharges { get; set; }
        public string Currency_Code { get; set; }
        public List<FareClass> FareClasses { get; set; }
        public FreeBaggage Free_Baggage { get; set; }
        public int GST { get; set; }
        public int Gross_Commission { get; set; }
        public int Net_Commission { get; set; }
        public int PAX_Type { get; set; }
        public int Promo_Discount { get; set; }
        public List<RescheduleCharge> RescheduleCharges { get; set; }
        public int Service_Fee_Amount { get; set; }
        public int TDS { get; set; }
        public int Total_Amount { get; set; }
        public int Trade_Markup_Amount { get; set; }
        public int YQ_Amount { get; set; }
    }

    public class Flight
    {
        public string Airline_Code { get; set; }
        public bool Block_Ticket_Allowed { get; set; }
        public bool Cached { get; set; }
        public string Destination { get; set; }
        public List<Fare> Fares { get; set; }
        public string Flight_Id { get; set; }
        public string Flight_Key { get; set; }
        public object Flight_Numbers { get; set; }
        public bool GST_Entry_Allowed { get; set; }
        public bool HasMoreClass { get; set; }
        public int InventoryType { get; set; }
        public bool IsLCC { get; set; }
        public string Origin { get; set; }
        public bool Repriced { get; set; }
        public List<Segment> Segments { get; set; }
        public string TravelDate { get; set; }
    }

    public class FreeBaggage
    {
        public string Check_In_Baggage { get; set; }
        public string Hand_Baggage { get; set; }
    }

    public class RescheduleCharge
    {
        public int Applicablility { get; set; }
        public int DurationFrom { get; set; }
        public int DurationTo { get; set; }
        public int DurationTypeFrom { get; set; }
        public int DurationTypeTo { get; set; }
        public int OfflineServiceFee { get; set; }
        public int OnlineServiceFee { get; set; }
        public int PassengerType { get; set; }
        public string Remarks { get; set; }
        public bool Return_Flight { get; set; }
        public string Value { get; set; }
        public int ValueType { get; set; }
    }

    public class ResponseHeader
    {
        public string Error_Code { get; set; }
        public string Error_Desc { get; set; }
        public string Error_InnerException { get; set; }
        public string Request_Id { get; set; }
        public string Status_Id { get; set; }
    }
    public class Segment
    {
        public string Aircraft_Type { get; set; }
        public string Airline_Code { get; set; }
        public string Airline_Name { get; set; }
        public string Arrival_DateTime { get; set; }
        public string Departure_DateTime { get; set; }
        public string Destination { get; set; }
        public object Destination_City { get; set; }
        public string Destination_Terminal { get; set; }
        public string Duration { get; set; }
        public string Flight_Number { get; set; }
        public int Leg_Index { get; set; }
        public string Origin { get; set; }
        public object Origin_City { get; set; }
        public string Origin_Terminal { get; set; }
        public bool Return_Flight { get; set; }
        public int Segment_Id { get; set; }
        public object Stop_Over { get; set; }
    }

    public class TicketDetail
    {
        public string Flight_Id { get; set; }
        public string Ticket_Number { get; set; }
    }
}
