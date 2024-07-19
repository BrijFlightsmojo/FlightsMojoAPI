using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Flight
{
    public class FlightSupplier
    {
        public int Id { get; set; }
        public SiteId siteId { get; set; }
        public GdsType Provider { get; set; }
        public List<string> SourceMedia { get; set; }
        public List<string> SourceMedia_Not { get; set; }
        public List<string> FromCountry { get; set; }
        public List<string> FromCountry_Not { get; set; }
        public List<string> ToCountry { get; set; }
        public List<string> ToCountry_Not { get; set; }
        public FareType FareType { get; set; }
        //public string Currency { get; set; }
        public int FarePrioritySequence { get; set; }
        public int CustomerType { get; set; }
        public bool isMeta { get; set; }
        public FlightSupplier()
        {

        }
    }

    public class FlightSupplierNew
    {
        public SiteId siteId { get; set; }
        public GdsType Provider { get; set; }
        public List<string> FromAirport { get; set; }
        public List<string> ToAirport { get; set; }

        public List<string> FromAirportNot { get; set; }
        public List<string> ToAirportNot { get; set; }

        public List<string> FromCountry { get; set; }
        public List<string> ToCountry { get; set; }

        public List<string> FromCountryNot { get; set; }
        public List<string> ToCountryNot { get; set; }

        public List<string> SourceMedia { get; set; }
        public List<string> SourceMedia_Not { get; set; }
        public int FarePriority { get; set; }
        public bool isAirIQ { get; set; }
        public Device device { get; set; }
        public int PaxCountFrom { get; set; }
        public int PaxCountTo { get; set; }
        public FlightSupplierNew()
        {

        }
    }
    public class BookingManagements
    {
        public SiteId SiteId { get; set; }
        public List<string> AffiliateId { get; set; }
        public List<string> AffiliateId_Not { get; set; }
        public List<string> FromCountry { get; set; }
        public List<string> FromCountry_Not { get; set; }
        public List<string> ToCountry { get; set; }
        public List<string> ToCountry_Not { get; set; }
        public List<string> Airline { get; set; }
        public List<string> Airline_Not { get; set; }
        public GdsType Supplier { get; set; }
        public List<MojoFareType> FareType { get; set; }
        public Core.BookingAction BookingAction { get; set; }
        //public string TimeFrom { get; set; }
        //public string TimeTo { get; set; }
        public BookingManagements()
        {

        }
    }


    public class Affiliate
    {
        public string AffiliateId { get; set; }
        public string AffiliateName { get; set; }
        public string EmiConFee { get; set; }
        public string PayLaterConFee { get; set; }
        public string WalletConFee { get; set; }
        public string NetBankingConFee { get; set; }
        public string CardConFee { get; set; }
        public string UPIConFee { get; set; }
        public SiteId SiteID { get; set; }

    }

    public class SupplierFareType
    {
        public MojoFareType FMFareType { get; set; }
        public string Airline { get; set; }
        public string ProviderFareType { get; set; }
        public GdsType Provider { get; set; }
    }
    public class AirlineCommissionRule
    {
        public List<string> Airline { get; set; }
        public List<string> AirlineNot { get; set; }
        public List<string> SourceMedia { get; set; }
        public List<GdsType> Provider { get; set; }
    }
}
