using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tripshope.TicketingDetailsResponse
{
    public class GetTicketingDetailsResponse
    {
        public ApiStatus ApiStatus { get; set; }
    }

    public class ApiStatus
    {
        public string Status { get; set; }
        public string StatusCode { get; set; }
        public string Message { get; set; }
        public string Operation { get; set; }
        public string ResultFormat { get; set; }
        public string TransactionStatus { get; set; }
        public Result Result { get; set; }
    }

    public class Result
    {
        public string txid { get; set; }
        public FlightRecord flightRecord { get; set; }
        public List<PassengerRecord> passengerRecords { get; set; }
        public List<PassengerFareCollection> passengerFareCollection { get; set; }
        public List<SectorRecord> sectorRecords { get; set; }
        public List<FlightTicket> flightTickets { get; set; }
        public List<FlightCommissionCollection> flightCommissionCollection { get; set; }
        public string supplierinfo { get; set; }
        public string userinfo { get; set; }
        public int additional_markup { get; set; }
        public string accountingXml { get; set; }
        public string taxbreakupxml { get; set; }
        public string supplierapiname { get; set; }
        public string narration1 { get; set; }
        public string narration2 { get; set; }
        public string narration3 { get; set; }
        public string bookingstrlog { get; set; }
        public string paxid { get; set; }
        public string booking_class { get; set; }
        public string flight_number { get; set; }
        public string invoicedate { get; set; }
        public List<Bookingparam> bookingparams { get; set; }
        public List<object> travelinsurance { get; set; }
        public string owfaredetails { get; set; }
        public string rtfaredetails { get; set; }
        public string officeid { get; set; }
        public List<object> passengerssrrecords { get; set; }
        public List<Pricinglog> pricinglogs { get; set; }
        public List<object> webcheckins { get; set; }
        public string rescmap { get; set; }
    }
    public class FlightRecord
    {
        public string txid { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public decimal total_amount { get; set; }
        public decimal fare_quote_amount { get; set; }
        public decimal total_base_tax { get; set; }
        public string payment_status { get; set; }
        public string booking_status { get; set; }
        public string payment_mode { get; set; }
        public string bank_id { get; set; }
        public string bookingdate { get; set; }
        public int num_sectors { get; set; }
        public int num_adults { get; set; }
        public int num_children { get; set; }
        public int num_infants { get; set; }
        public int num_pax { get; set; }
        public string gds_pnr { get; set; }
        public string airline_pnr { get; set; }
        public string fare_breakup { get; set; }
        public string booking_type { get; set; }
        public string booking_provider { get; set; }
        public string bookedbyname { get; set; }
        public string bookedbyroleid { get; set; }
        public string process_status { get; set; }
        public string erptxid { get; set; }
        public string ispassthrough { get; set; }
        public string passthroughtype { get; set; }
        public decimal ssramount { get; set; }
        public string issbill { get; set; }
        public string tripnumber { get; set; }
        public string iscloned { get; set; }
        public string travelinsurance_txid { get; set; }
        public string approval_status { get; set; }
        public string approvaltxid { get; set; }
        public string projectcode { get; set; }
        public string ticketing_date { get; set; }
        public string passthroughjtype { get; set; }
        public string passthroughxml { get; set; }
        public decimal passthroughamount { get; set; }
        public decimal educess { get; set; }
        public decimal highereducess { get; set; }
        public string corporatecode { get; set; }
        public string isaccountingpushed { get; set; }
        public decimal jn { get; set; }
        public decimal hiddenmarkup { get; set; }
        public string jtype { get; set; }
        public string agentid { get; set; }
        public string passengername { get; set; }
        public string passengerlastname { get; set; }
        public string passengertitle { get; set; }
        public string passengeremail { get; set; }
        public string passengercontact { get; set; }
        public string passengeraddress { get; set; }
        public string domint { get; set; }
        public string jdate { get; set; }
        public string rdate { get; set; }
        public string agent_company { get; set; }
        public decimal markup { get; set; }
        public decimal servicetax { get; set; }
        public decimal tds { get; set; }
        public decimal commission { get; set; }
        public decimal handlingcharges { get; set; }
        public decimal tfeefq { get; set; }
        public decimal tfeeinv { get; set; }
        public decimal basefare { get; set; }
        public decimal tax { get; set; }
        public decimal yq { get; set; }
        public string channelinfo { get; set; }
        public string bookingchannel { get; set; }
        public string corporatename { get; set; }
        public string travel_status { get; set; }
        public string cabinclass { get; set; }
        public string baggageinfo { get; set; }
        public string splremarks { get; set; }
        public string costcenter { get; set; }
        public string bookedbyid { get; set; }
        public string issplrt { get; set; }
        public string travelinsurance_included { get; set; }
        public string travelinsurance_amount { get; set; }
        public string inpolicy { get; set; }
        public string policy_reason { get; set; }
        public decimal additional_basehc { get; set; }
        public decimal additional_taxhc { get; set; }
        public decimal additional_feehc { get; set; }
        public string employeeid { get; set; }
        public string apitxid { get; set; }
        public string apidbname { get; set; }
        public string promocode { get; set; }
        public string promocode_used { get; set; }
        public decimal promovalue { get; set; }
        public string corporate_department { get; set; }
        public string apiremarks { get; set; }
        public string requestedby { get; set; }
        public string carrier { get; set; }
        public string invoicenumber { get; set; }
        public string adminremarks { get; set; }
        public string pgcharges { get; set; }
        public string additional { get; set; }
        public string flightticketnums { get; set; }
        public int additional_markup { get; set; }
        public string hasssr { get; set; }
        public string hasseatssr { get; set; }
        public string phoneIntlCode { get; set; }
        public string phoneAreaCode { get; set; }
        public string skipadditionalhc { get; set; }
        public string endUserIP { get; set; }
        public string endUserBrowser { get; set; }
        public string ismultiticketnumber { get; set; }
    }

    public class PassengerRecord
    {
        public string txid { get; set; }
        public string paxid { get; set; }
        public string trip_number { get; set; }
        public string title { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string dob { get; set; }
        public string type { get; set; }
        public string email { get; set; }
        public string contact_number { get; set; }
        public decimal base_fare { get; set; }
        public decimal tax { get; set; }
        public decimal fuel_surcharge { get; set; }
        public string gds_pnr { get; set; }
        public string airline_pnr { get; set; }
        public string flight_ticket_number { get; set; }
        public string fare_breakup { get; set; }
        public string status { get; set; }
        public string tour_code { get; set; }
        public string deal_code { get; set; }
        public string frequent_flyer_number { get; set; }
        public string passport { get; set; }
        public string visa { get; set; }
        public string passport_dateofissue { get; set; }
        public string passport_placeofissue { get; set; }
        public string passport_dateofexpiry { get; set; }
        public string agentid { get; set; }
        public string meal_preference { get; set; }
        public string seat_preference { get; set; }
        public string additional_segmentinfo { get; set; }
        public string paxssrinfo { get; set; }
        public string bookingpageparams { get; set; }
        public string endorsement_code { get; set; }
        public string paxseatinfo { get; set; }
        public string paxssrseatinfo { get; set; }
        public string seatnumber { get; set; }
        public string seattypes { get; set; }
        public string idproof { get; set; }
        public string cost_center { get; set; }
        public string grade { get; set; }
        public string department { get; set; }
        public string designation { get; set; }
        public string namechange_allowed { get; set; }
        public string liftstatus { get; set; }
        public string isnameupdated { get; set; }
        public string tour_code_status { get; set; }
        public string deal_code_status { get; set; }
        public string freqfly_status { get; set; }
        public string passport_status { get; set; }
        public string ssr_status { get; set; }
        public string ssr_error { get; set; }
        public string error_details { get; set; }
        public string seat_status { get; set; }
        public string meal_status { get; set; }
        public string seat_error { get; set; }
        public string meal_error { get; set; }
        public string specialssr { get; set; }
        public decimal commission { get; set; }
        public decimal tds { get; set; }
        public decimal servicetax { get; set; }
        public decimal handlingcharges { get; set; }
        public decimal tfeeinv { get; set; }
        public decimal tfeefq { get; set; }
        public decimal promo_discount { get; set; }
        public decimal discomm { get; set; }
        public decimal totalinvoice { get; set; }
        public decimal totalfarequote { get; set; }
        public decimal markup { get; set; }
        public string canxtxid { get; set; }
        public string resctxid { get; set; }
        public decimal refundamount { get; set; }
        public string profileid { get; set; }
        public decimal additional_basehc { get; set; }
        public decimal additional_taxhc { get; set; }
        public decimal additional_feehc { get; set; }
        public string beneficiaryname { get; set; }
        public List<object> ssrs { get; set; }
    }

    public class PassengerFareCollection
    {
        public string txid { get; set; }
        public string paxid { get; set; }
        public string paxtype { get; set; }
        public string fare_componentid { get; set; }
        public decimal amount { get; set; }
    }

    public class SectorRecord
    {
        public string txid { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
        public string sector_number { get; set; }
        public string jdate { get; set; }
        public string departure_time { get; set; }
        public string arrival_time { get; set; }
        public string arrdate { get; set; }
        public string booking_class { get; set; }
        public string fare_basis { get; set; }
        public string carrier { get; set; }
        public string flight_number { get; set; }
        public string flight_type { get; set; }
        public string insert_time { get; set; }
        public string stop_over { get; set; }
        public string airline_pnr { get; set; }
        public string baggageinfo { get; set; }
        public string flight_model { get; set; }
        public bool refundable { get; set; }
        public string refundable_info { get; set; }
        public string airport_terminal { get; set; }
        public string agentid { get; set; }
        public string sector_duration { get; set; }
        public string journey_duration { get; set; }
        public string origin_fullname { get; set; }
        public string destination_fullname { get; set; }
        public string tripnumber { get; set; }
        public string Supplier_id { get; set; }
        public string origin_airportfullname { get; set; }
        public string destination_airportfullname { get; set; }
        public decimal base_fare { get; set; }
        public decimal tax { get; set; }
        public decimal total_amount { get; set; }
        public string faretype { get; set; }
        public string carrier_name { get; set; }
        public string arrival_terminal { get; set; }
        public string depart_terminal { get; set; }
        public string operatedby { get; set; }
    }
    public class FlightTicket
    {
        public string txid { get; set; }
        public string paxid { get; set; }
        public string sector_number { get; set; }
        public string trip_number { get; set; }
        public string ticket_number { get; set; }
        public string airline_pnr { get; set; }
        public string ffnnumber { get; set; }
    }

    public class FlightCommissionCollection
    {
        public string txid { get; set; }
        public string agentid { get; set; }
        public string type { get; set; }
        public string carrier { get; set; }
        public string fare_componentid { get; set; }
        public string unit { get; set; }
        public string value { get; set; }
        public string fare_componentid_computed { get; set; }
        public string sign { get; set; }
    }

    public class Bookingparam
    {
        public string txid { get; set; }
        public string paramname { get; set; }
        public string paramvalue { get; set; }
        public string txdate { get; set; }
        public string sendtogds { get; set; }
        public string domint { get; set; }
    }

    public class Pricinglog
    {
        public string txid { get; set; }
        public string pricingtype { get; set; }
        public string apiname { get; set; }
        public string requestxml { get; set; }
        public string responsexml { get; set; }
        public string inserttime { get; set; }
    }


}
