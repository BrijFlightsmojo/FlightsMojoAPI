using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Travelopedia.TravelopediaClass.RePrice
{
   
    public class TravelopediaRePriceResponse
    {
        public ResponseHeader Response_Header { get; set; }
        public List<AirRepriceResponse> AirRepriceResponses { get; set; }       
    }
   
    public class AirportTaxis
    {
        public decimal Tax_Amount { get; set; }
        public string Tax_Code { get; set; }
        public string Tax_Desc { get; set; }
    }

    public class AirRepriceResponse
    {
        public Flight Flight { get; set; }
        public bool Frequent_Flyer_Accepted { get; set; }
        public List<RequiredPAXDetail> Required_PAX_Details { get; set; }
    }

    public class Fare
    {
        public List<FareDetail> FareDetails { get; set; }
        public int FareType { get; set; }
        public string Fare_Id { get; set; }
        public string Fare_Key { get; set; }
        public string Food_onboard { get; set; }
        public bool GSTMandatory { get; set; }
        public string LastFewSeats { get; set; }
        public string ProductClass { get; set; }
        public string PromptMessage { get; set; }
        public bool Refundable { get; set; }
        public string Seats_Available { get; set; }
        public string Warning { get; set; }
    }

    public class FareClass
    {
        public string Class_Code { get; set; }
        public string Class_Desc { get; set; }
        public string FareBasis { get; set; }
        public string Privileges { get; set; }
        public int Segment_Id { get; set; }
    }

    public class FareDetail
    {
        public decimal AirportTax_Amount { get; set; }
        public List<AirportTaxis> AirportTaxes { get; set; }
        public decimal Basic_Amount { get; set; }
        public string Currency_Code { get; set; }
        public List<FareClass> FareClasses { get; set; }
        public FreeBaggage Free_Baggage { get; set; }
        public decimal GST { get; set; }
        public decimal Gross_Commission { get; set; }
        public decimal Net_Commission { get; set; }
        public int PAX_Type { get; set; }
        public decimal Promo_Discount { get; set; }
        public decimal Service_Fee_Amount { get; set; }
        public decimal TDS { get; set; }
        public decimal Total_Amount { get; set; }
        public decimal Trade_Markup_Amount { get; set; }
        public decimal YQ_Amount { get; set; }
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
        public string Flight_Numbers { get; set; }
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

    public class RequiredPAXDetail
    {
        public bool Age { get; set; }
        public bool DOB { get; set; }
        public bool DefenceExpiryDate { get; set; }
        public bool DefenceIssueDate { get; set; }
        public bool DefenceServiceId { get; set; }
        public bool First_Name { get; set; }
        public bool Gender { get; set; }
        public bool IdProof_Number { get; set; }
        public bool Last_Name { get; set; }
        public string Mandatory_SSRs { get; set; }
        public bool Nationality { get; set; }
        public bool Passport_Expiry { get; set; }
        public bool Passport_Issuing_Country { get; set; }
        public bool Passport_Number { get; set; }
        public int Pax_type { get; set; }
        public bool Student_Id { get; set; }
        public bool Title { get; set; }
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
        public string Destination_City { get; set; }
        public string Destination_Terminal { get; set; }
        public string Duration { get; set; }
        public string Flight_Number { get; set; }
        public int Leg_Index { get; set; }
        public string Origin { get; set; }
        public string Origin_City { get; set; }
        public string Origin_Terminal { get; set; }
        public bool Return_Flight { get; set; }
        public int Segment_Id { get; set; }
        public string Stop_Over { get; set; }
    }
}
