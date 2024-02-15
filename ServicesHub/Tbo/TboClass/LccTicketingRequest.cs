using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tbo.TboClass
{
    [DataContract]
    public class LccTicketingRequest
    {
        [DataMember]
        public string PreferredCurrency { get; set; }
        [DataMember]
        public string IsBaseCurrencyRequired { get; set; }
        //[DataMember]
        //public string AgentReferenceNo { get; set; }
        [DataMember]
        public string EndUserIp { get; set; }
        [DataMember]
        public string TokenId { get; set; }
        [DataMember]
        public string TraceId { get; set; }
        [DataMember]
        public string ResultIndex { get; set; }
        [DataMember]
        public List<PassengerLTR> Passengers { get; set; }
    }

    [DataContract]
    public class FareLTR
    {
        [DataMember]
        public string Currency { get; set; }
        [DataMember]
        public decimal BaseFare { get; set; }
        [DataMember]
        public decimal Tax { get; set; }
        //[DataMember]
        //public decimal TransactionFee { get; set; }
        [DataMember]
        public decimal YQTax { get; set; }
        [DataMember]
        public decimal AdditionalTxnFeeOfrd { get; set; }
        [DataMember]
        public decimal AdditionalTxnFeePub { get; set; }
        //[DataMember]
        //public decimal AirTransFee { get; set; }
        //[DataMember]
        //public decimal Discount { get; set; }
        //[DataMember]
        //public decimal PublishedFare { get; set; }
        //[DataMember]
        //public decimal OfferedFare { get; set; }
        [DataMember]
        public decimal OtherCharges { get; set; }
        //[DataMember]
        //public decimal TdsOnCommission { get; set; }
        //[DataMember]
        //public decimal TdsOnPLB { get; set; }
        //[DataMember]
        //public decimal TdsOnIncentive { get; set; }
        //[DataMember]
        //public decimal ServiceFee { get; set; }
     
    }
    [DataContract]
    public class PassengerLTR
    {
        public PassengerLTR()
        {
            Title = "";
            FirstName = "";
            LastName = "";
            PaxType = "";
            DateOfBirth = "";
            Gender = "";
            PassportNo = "";
            PassportExpiry = "";
            Nationality = "";
            AddressLine1 = "";
            AddressLine2 = "";
            City = "";
            CountryCode = "";
            CountryName = "";
            ContactNo = "";
            Email = "";
            IsLeadPax = false;
            FFAirline = "";
            FFNumber = "";
            Fare = new FareLTR();          
        }
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string PaxType { get; set; }
        [DataMember]
        public string DateOfBirth { get; set; }
        [DataMember]
        public string Gender { get; set; }
        [DataMember]
        public string PassportNo { get; set; }
        [DataMember]
        public string PassportExpiry { get; set; }
        [DataMember]
        public string Nationality { get; set; }
        [DataMember]
        public string AddressLine1 { get; set; }
        [DataMember]
        public string AddressLine2 { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string CountryCode { get; set; }
        [DataMember]
        public string CountryName { get; set; }
        [DataMember]
        public string ContactNo { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public bool IsLeadPax { get; set; }
        [DataMember]
        public string FFAirline { get; set; }
        [DataMember]
        public string FFNumber { get; set; }
        [DataMember]
        public FareLTR Fare { get; set; }
        [DataMember(EmitDefaultValue = false)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<MealDynamicLTR> MealDynamic { get; set; }
        [DataMember(EmitDefaultValue = false)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<BaggageLTR> Baggage { get; set; }
    }
    [DataContract]
    public class MealDynamicLTR
    {
        [DataMember]
        public int WayType { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public int Description { get; set; }
        [DataMember]
        public string AirlineDescription { get; set; }
        [DataMember]
        public int Quantity { get; set; }
        [DataMember]
        public int Price { get; set; }
        [DataMember]
        public string Currency { get; set; }
        [DataMember]
        public string Origin { get; set; }
        [DataMember]
        public string Destination { get; set; }
    }

    [DataContract]
    public class BaggageLTR
    {
        [DataMember]
        public int WayType { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public int Description { get; set; }
        [DataMember]
        public int Weight { get; set; }
        [DataMember]
        public string Currency { get; set; }
        [DataMember]
        public int Price { get; set; }
        [DataMember]
        public string Origin { get; set; }
        [DataMember]
        public string Destination { get; set; }
    }
}
