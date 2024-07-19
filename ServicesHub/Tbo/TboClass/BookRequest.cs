using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tbo.TboClass
{
    [DataContract]
    public class BookRequest
    {
        [DataMember]
        public string EndUserIp { get; set; }
        [DataMember]
        public string TokenId { get; set; }
        [DataMember]
        public string TraceId { get; set; }
        [DataMember]
        public string ResultIndex { get; set; }
        [DataMember]
        public List<PassengerBQ> Passengers { get; set; }
        //[DataMember]
        //public Payment Payment { get; set; }
        [DataMember]
        public string AgentReferenceNo { get; set; }
    }
    [DataContract]
    public class PassengerBQ
    {
        public PassengerBQ()
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
            Fare = new FareBQ();
            //Meal = new List<Meal>();
            //Seat = new Seat() { Code="",Description=""};
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
        public string Nationality { get; set; }
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
        public FareBQ Fare { get; set; }
        //[DataMember]
        //public List<Meal> Meal { get; set; }
        //[DataMember]
        //public Seat Seat { get; set; }

        [DataMember]
        public string CellCountryCode { get; set; }
        [DataMember]
        public string PassportIssueDate { get; set; }
        [DataMember]
        public string PassportIssueCountryCode { get; set; }
    }


    public class FareBQ
    {
        [DataMember]
        public string Currency { get; set; }
        [DataMember]
        public decimal BaseFare { get; set; }
        [DataMember]
        public decimal Tax { get; set; }
        [DataMember]
        public decimal YQTax { get; set; }
        [DataMember]
        public decimal AdditionalTxnFeePub { get; set; }
        [DataMember]
        public decimal AdditionalTxnFeeOfrd { get; set; }
        [DataMember]
        public decimal OtherCharges { get; set; }
        [DataMember]
        public decimal Discount { get; set; }
        [DataMember]
        public decimal PublishedFare { get; set; }
        [DataMember]
        public decimal OfferedFare { get; set; }
        [DataMember]
        public decimal TdsOnCommission { get; set; }
        [DataMember]
        public decimal TdsOnPLB { get; set; }
        [DataMember]
        public decimal TdsOnIncentive { get; set; }
        [DataMember]
        public decimal ServiceFee { get; set; }      
    }
    [DataContract]
    public class Meal
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Description { get; set; }
    }
    [DataContract]
    public class Seat
    {
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string Description { get; set; }
    }

}
