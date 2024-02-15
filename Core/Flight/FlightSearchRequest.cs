﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.Flight
{

    [DataContract]
    public class FlightSearchRequest
    {
        [DataMember]
        public string SearchURL { get; set; }
        [DataMember]
        public string userSessionID { get; set; }
        [DataMember]
        public string userSearchID { get; set; }
        [DataMember]
        public string userLogID { get; set; }
        [DataMember]
        public ClientType client { get; set; }
        [DataMember]
        public List<SearchSegment> segment { get; set; }
        [DataMember]
        public bool searchDirectFlight { get; set; }
        [DataMember]
        public bool flexibleSearch { get; set; }
        [DataMember]
        public TripType tripType { get; set; }
        [DataMember]
        public int adults { get; set; }
        [DataMember]
        public int child { get; set; }
        [DataMember]
        public int infants { get; set; }       
        [DataMember]
        public CabinType cabinType { get; set; }
        [DataMember]
        public string airline { get; set; }
        [DataMember]
        public string currencyCode { get; set; }
        [DataMember]
        public SiteId siteId { get; set; }
        [DataMember]
        public string sourceMedia { get; set; }
        [DataMember]
        public string sID { get; set; }
        [DataMember]
        public string rID { get; set; }
        [DataMember]
        public string locale { get; set; }
        [DataMember]
        public bool isNearBy { get; set; }
        [DataMember]
        public string limit { get; set; }
        [DataMember]
        public string page { get; set; }
        [DataMember]
        public string pageValue { get; set; }
        [DataMember]
        public string userIP { get; set; }
        [DataMember]
        public string serverIP { get; set; }
        [DataMember]
        public string fareCachingKey { get; set; }
        [DataMember]
        public bool isMetaRequest { get; set; }
        [DataMember]
        public int fareType { get; set; }
        [DataMember]
        public Core.TravelType travelType { get; set; }
        [DataMember]
        public string deepLink{get;set; }
 		[DataMember]//Use for kayak S2S Pixiel
        public string kayakClickId { get; set; }
        [DataMember]//Use for wego S2S Pixiel
        public string wegoClickId { get; set; }
        [DataMember]
        public bool isGetLiveFare { get; set; }
        [DataMember]
        public string tgy_Request_id { get; set; }
    }
    [DataContract]
    public class SearchSegment
    {
        [DataMember]
        public string originAirport { get; set; }
        [DataMember]
        public Airport orgArp { get; set; }
        [DataMember]
        public string destinationAirport { get; set; }
        [DataMember]
        public Airport destArp { get; set; }
        [DataMember]
        public DateTime travelDate { get; set; }
    }
}
