using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Travelogy.TravelogyClass
{
    public class TravelogyFlightSearchResponse
    {
        public ResponseHeader Response_Header { get; set; }
        public string Search_Key { get; set; }
        public List<TripDetail> TripDetails { get; set; }
    }
    public class TripDetail
    {
        public List<Flight> Flights { get; set; }
        public int Trip_Id { get; set; }
    }
    public class AirportTaxis
    {
        public decimal Tax_Amount { get; set; }
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
        public int Seats_Available { get; set; }
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
        public List<CancellationCharge> CancellationCharges { get; set; }
        public List<RescheduleCharge> RescheduleCharges { get; set; }
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
        public DateTime Arrival_DateTime { get; set; }
        public DateTime Departure_DateTime { get; set; }
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
        public List<string> Stop_Over { get; set; }
    }  
}
