using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tbo
{
    public class TboRequestMappking
    {
        public string getFlightSearchRequest(Core.Flight.FlightSearchRequest fsr, string TokenId)
        {
            ServicesHub.Tbo.TboClass.FlightSearchRequest flightSearchRequest = new ServicesHub.Tbo.TboClass.FlightSearchRequest()
            {
                AdultCount = fsr.adults,
                ChildCount = fsr.child,
                InfantCount = fsr.infants,
                DirectFlight = fsr.searchDirectFlight,
                EndUserIp = string.IsNullOrEmpty(fsr.userIP)&&false ? "54.214.158.214" : fsr.userIP,
                JourneyType = getJourneyType(fsr.tripType),
                OneStopFlight = false,
                PreferredAirlines = new List<string>(),
                Segments = new List<ServicesHub.Tbo.TboClass.Segment>(),
               
                TokenId = TokenId
            };
            if (!string.IsNullOrEmpty(fsr.airline) && !fsr.airline.Equals("all", StringComparison.OrdinalIgnoreCase) && !fsr.airline.Equals("any", StringComparison.OrdinalIgnoreCase))
            {
                flightSearchRequest.PreferredAirlines.Add(fsr.airline);
            }
            if (fsr.travelType == Core.TravelType.Domestic)
            {
                //flightSearchRequest.Sources.Add("GDS");
                //flightSearchRequest.Sources.Add("6E");
                //flightSearchRequest.Sources.Add("SG");
                //flightSearchRequest.Sources.Add("G8");
            }
            else
            {
                //flightSearchRequest.Sources = new List<string>();
                //flightSearchRequest.Sources.Add("GDS");
                //flightSearchRequest.Sources.Add("SG");// - SpiceJet 
                //flightSearchRequest.Sources.Add("6E");// - Indigo 
                //flightSearchRequest.Sources.Add("G8");// - Go Air 
                //flightSearchRequest.Sources.Add("G9");// -Air Arabia 
                //flightSearchRequest.Sources.Add("FZ");// -Fly Dubai 
                //flightSearchRequest.Sources.Add("IX");// -Air India Express 
                //flightSearchRequest.Sources.Add("AK");// - Air Asia
                //flightSearchRequest.Sources.Add("LB");// -Air Costa 
            }
            foreach (Core.Flight.SearchSegment ss in fsr.segment)
            {
                ServicesHub.Tbo.TboClass.Segment seg = new ServicesHub.Tbo.TboClass.Segment();
                seg.Origin = ss.originAirport;
                seg.Destination = ss.destinationAirport;
                seg.PreferredDepartureTime = ss.travelDate;
                seg.PreferredArrivalTime = ss.travelDate;
                seg.FlightCabinClass = getCabinType(fsr.cabinType);
                flightSearchRequest.Segments.Add(seg);
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(flightSearchRequest);
        }
        public string getCalendarFareUrlRequest(Core.Flight.FlightSearchRequest fsr, string TokenId)
        {
            ServicesHub.Tbo.TboClass.CalendarFareRequest flightSearchRequest = new ServicesHub.Tbo.TboClass.CalendarFareRequest()
            {
                EndUserIp = fsr.userIP,
                PreferredAirlines = new List<string>(),
                Segments = new List<TboClass.SegmentCFR>(),
                Sources = new List<string>(),
                TokenId = TokenId,
                JourneyType = getJourneyType(fsr.tripType)
            };
            if (!string.IsNullOrEmpty(fsr.airline) && !fsr.airline.Equals("all", StringComparison.OrdinalIgnoreCase) && !fsr.airline.Equals("any", StringComparison.OrdinalIgnoreCase))
            {
                flightSearchRequest.PreferredAirlines.Add(fsr.airline);
            }
            if (fsr.travelType == Core.TravelType.Domestic)
            {
                //flightSearchRequest.Sources.Add("GDS");
                flightSearchRequest.Sources.Add("6E");
                flightSearchRequest.Sources.Add("SG");
                flightSearchRequest.Sources.Add("G8");
            }
            else
            {
                flightSearchRequest.Sources.Add("GDS");
                flightSearchRequest.Sources.Add("SG");// - SpiceJet 
                flightSearchRequest.Sources.Add("6E");// - Indigo 
                flightSearchRequest.Sources.Add("G8");// - Go Air 
                flightSearchRequest.Sources.Add("G9");// -Air Arabia 
                flightSearchRequest.Sources.Add("FZ");// -Fly Dubai 
                flightSearchRequest.Sources.Add("IX");// -Air India Express 
                flightSearchRequest.Sources.Add("AK");// - Air Asia
                flightSearchRequest.Sources.Add("LB");// -Air Costa 
            }
            foreach (Core.Flight.SearchSegment ss in fsr.segment)
            {
                ServicesHub.Tbo.TboClass.SegmentCFR seg = new ServicesHub.Tbo.TboClass.SegmentCFR();
                seg.Origin = ss.originAirport;
                seg.Destination = ss.destinationAirport;
                seg.PreferredDepartureTime = ss.travelDate;
                seg.FlightCabinClass = getCabinType(fsr.cabinType);
                flightSearchRequest.Segments.Add(seg);
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(flightSearchRequest);
        }
        public string getCalendarFareUpdateRequest(Core.Flight.CalendarFareUpdateRequest fsr, string TokenId)
        {
            ServicesHub.Tbo.TboClass.CalendarFareUpdateRequest flightSearchRequest = new ServicesHub.Tbo.TboClass.CalendarFareUpdateRequest()
            {
                EndUserIp = fsr.userIP,
                PreferredAirlines = new List<string>(),
                Segments = new List<TboClass.SegmentCFR>(),
                Sources = new List<string>(),
                TokenId = TokenId,
                JourneyType = getJourneyType(fsr.tripType)
            };
            if (!string.IsNullOrEmpty(fsr.airline) && !fsr.airline.Equals("all", StringComparison.OrdinalIgnoreCase) && !fsr.airline.Equals("any", StringComparison.OrdinalIgnoreCase))
            {
                flightSearchRequest.PreferredAirlines.Add(fsr.airline);
            }
            if (fsr.travelType == Core.TravelType.Domestic)
            {
                //flightSearchRequest.Sources.Add("GDS");
                flightSearchRequest.Sources.Add("6E");
                flightSearchRequest.Sources.Add("SG");
                flightSearchRequest.Sources.Add("G8");
            }
            else
            {
                flightSearchRequest.Sources.Add("GDS");
            }
            foreach (Core.Flight.SearchSegment ss in fsr.segment)
            {
                ServicesHub.Tbo.TboClass.SegmentCFR seg = new ServicesHub.Tbo.TboClass.SegmentCFR();
                seg.Origin = ss.originAirport;
                seg.Destination = ss.destinationAirport;
                seg.PreferredDepartureTime = ss.travelDate;
                seg.FlightCabinClass = getCabinType(fsr.cabinType);
                flightSearchRequest.Segments.Add(seg);
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(flightSearchRequest);
        }
        public string getFareQuoteRequest(string resultIndex, string traceID, string userIp, string TokenId)
        {
            ServicesHub.Tbo.TboClass.FareQuoteRequest fareQuoteRequest = new ServicesHub.Tbo.TboClass.FareQuoteRequest()
            {
                TokenId = TokenId,
                EndUserIp = userIp,
                ResultIndex = resultIndex,
                TraceId = traceID
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(fareQuoteRequest);
        }

        public string getGdsBookingRequest(Core.Flight.FlightBookingRequest request, int ctr, string TokenId)
        {
            ServicesHub.Tbo.TboClass.BookRequest bookRequest = new ServicesHub.Tbo.TboClass.BookRequest()
            {
                EndUserIp = request.userIP,
                Passengers = new List<TboClass.PassengerBQ>(),
                ResultIndex = request.flightResult[ctr].Fare.tboResultIndex,
                TokenId = TokenId,
                TraceId = request.TvoTraceId
            };
            int i = 0;
            Core.Flight.FareBreakdown adtFare = null, chdFare = null, infFare = null;
            adtFare = request.flightResult[ctr].Fare.fareBreakdown.Where(k => k.PassengerType == Core.PassengerType.Adult).FirstOrDefault();
            if (request.child > 0)
                chdFare = request.flightResult[ctr].Fare.fareBreakdown.Where(k => k.PassengerType == Core.PassengerType.Child).FirstOrDefault();
            if (request.infants > 0)
                infFare = request.flightResult[ctr].Fare.fareBreakdown.Where(k => k.PassengerType == Core.PassengerType.Infant).FirstOrDefault();
            foreach (var item in request.passengerDetails)
            {
                TboClass.PassengerBQ pax = new TboClass.PassengerBQ();
                pax.Title = item.title;
                pax.FirstName = item.firstName + (string.IsNullOrEmpty(item.middleName) ? "" : (" " + item.middleName));
                pax.LastName = item.lastName;
                pax.DateOfBirth = item.dateOfBirth.ToString("yyyy-MM-ddTHH:mm:ss");
                pax.PaxType = getPaxType(item.passengerType).ToString();
                pax.Gender = getGenderType(item.gender).ToString();
                if (!string.IsNullOrEmpty(item.passportNumber))
                {
                    pax.PassportNo = item.passportNumber;
                    if (item.expiryDate.HasValue)
                        pax.PassportExpiry = item.expiryDate.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                }
                pax.AddressLine1 = "Plot No 83";// request.paymentDetails.address1;
                pax.AddressLine2 = "Sector 28";//request.paymentDetails.address2;
                pax.City = "Gurugram";// request.paymentDetails.city;
                pax.CountryCode = "IN";// request.paymentDetails.country;
                pax.CountryName = "India";// request.paymentDetails.countryName;
                pax.Nationality = "India";// string.IsNullOrEmpty(item.nationality) ? request.paymentDetails.country : item.nationality;
                pax.IsLeadPax = (i == 0 ? true : false);
                pax.ContactNo = request.phoneNo;
                pax.Email = request.emailID;
                if (item.passengerType == Core.PassengerType.Adult)
                {
                    pax.Fare = new TboClass.FareBQ()
                    {
                        AdditionalTxnFeeOfrd = adtFare.AdditionalTxnFeeOfrd / request.adults,
                        AdditionalTxnFeePub = adtFare.AdditionalTxnFeePub / request.adults,
                        BaseFare = adtFare.BaseFare / request.adults,
                        Currency = request.flightResult[ctr].Fare.Currency,
                        OtherCharges = request.flightResult[ctr].Fare.OtherCharges,
                        Tax = adtFare.Tax / request.adults,
                        YQTax = adtFare.YQTax / request.adults,
                        Discount = request.flightResult[ctr].Fare.Discount,
                        OfferedFare = request.flightResult[ctr].Fare.OfferedFare,
                        PublishedFare = request.flightResult[ctr].Fare.PublishedFare,
                        ServiceFee = request.flightResult[ctr].Fare.ServiceFee,
                        TdsOnCommission = request.flightResult[ctr].Fare.TdsOnCommission,
                        TdsOnIncentive = request.flightResult[ctr].Fare.TdsOnIncentive,
                        TdsOnPLB = request.flightResult[ctr].Fare.TdsOnPLB,
                    };
                }
                else if (item.passengerType == Core.PassengerType.Child)
                {
                    pax.Fare = new TboClass.FareBQ()
                    {
                        AdditionalTxnFeeOfrd = chdFare.AdditionalTxnFeeOfrd / request.child,
                        AdditionalTxnFeePub = chdFare.AdditionalTxnFeePub / request.child,
                        BaseFare = chdFare.BaseFare / request.child,
                        Currency = request.flightResult[ctr].Fare.Currency,
                        OtherCharges = request.flightResult[ctr].Fare.OtherCharges,
                        Tax = chdFare.Tax / request.child,
                        YQTax = chdFare.YQTax / request.child,
                        Discount = request.flightResult[ctr].Fare.Discount,
                        OfferedFare = request.flightResult[ctr].Fare.OfferedFare,
                        PublishedFare = request.flightResult[ctr].Fare.PublishedFare,
                        ServiceFee = request.flightResult[ctr].Fare.ServiceFee,
                        TdsOnCommission = request.flightResult[ctr].Fare.TdsOnCommission,
                        TdsOnIncentive = request.flightResult[ctr].Fare.TdsOnIncentive,
                        TdsOnPLB = request.flightResult[ctr].Fare.TdsOnPLB,
                    };
                }
                else if (item.passengerType == Core.PassengerType.Infant)
                {
                    pax.Fare = new TboClass.FareBQ()
                    {
                        AdditionalTxnFeeOfrd = infFare.AdditionalTxnFeeOfrd / request.infants,
                        AdditionalTxnFeePub = infFare.AdditionalTxnFeePub / request.infants,
                        BaseFare = infFare.BaseFare / request.infants,
                        Currency = request.flightResult[ctr].Fare.Currency,
                        OtherCharges = request.flightResult[ctr].Fare.OtherCharges,
                        Tax = infFare.Tax / request.infants,
                        YQTax = infFare.YQTax / request.infants,
                        Discount = request.flightResult[ctr].Fare.Discount,
                        OfferedFare = request.flightResult[ctr].Fare.OfferedFare,
                        PublishedFare = request.flightResult[ctr].Fare.PublishedFare,
                        ServiceFee = request.flightResult[ctr].Fare.ServiceFee,
                        TdsOnCommission = request.flightResult[ctr].Fare.TdsOnCommission,
                        TdsOnIncentive = request.flightResult[ctr].Fare.TdsOnIncentive,
                        TdsOnPLB = request.flightResult[ctr].Fare.TdsOnPLB,
                    };
                }
                bookRequest.Passengers.Add(pax);
                i++;
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(bookRequest);
        }
        public string getLccTicketingRequest(Core.Flight.FlightBookingRequest request, int ctr, string TokenId)
        {
            ServicesHub.Tbo.TboClass.LccTicketingRequest TktRequest = new ServicesHub.Tbo.TboClass.LccTicketingRequest()
            {
                EndUserIp = request.userIP,
                Passengers = new List<TboClass.PassengerLTR>(),
                //    ResultIndex = request.bookResult[ctr].ResultIndex,
                ResultIndex = request.flightResult[ctr].Fare.tboResultIndex,
                TokenId = TokenId,
                TraceId = request.TvoTraceId
            };
            int i = 0;
            Core.Flight.FareBreakdown adtFare = null, chdFare = null, infFare = null;
            adtFare = request.flightResult[ctr].Fare.fareBreakdown.Where(k => k.PassengerType == Core.PassengerType.Adult).FirstOrDefault();
            if (request.child > 0)
                chdFare = request.flightResult[ctr].Fare.fareBreakdown.Where(k => k.PassengerType == Core.PassengerType.Child).FirstOrDefault();
            if (request.infants > 0)
                infFare = request.flightResult[ctr].Fare.fareBreakdown.Where(k => k.PassengerType == Core.PassengerType.Infant).FirstOrDefault();
            foreach (var item in request.passengerDetails)
            {
                TboClass.PassengerLTR pax = new TboClass.PassengerLTR();
                pax.Title = item.title;
                pax.FirstName = item.firstName + (string.IsNullOrEmpty(item.middleName) ? "" : (" " + item.middleName));
                pax.LastName = item.lastName;
                pax.DateOfBirth = item.dateOfBirth.ToString("yyyy-MM-ddTHH:mm:ss");
                pax.PaxType = getPaxType(item.passengerType).ToString();
                pax.Gender = getGenderType(item.gender).ToString();
                if (!string.IsNullOrEmpty(item.passportNumber))
                {
                    pax.PassportNo = item.passportNumber;
                    if (item.expiryDate.HasValue)
                        pax.PassportExpiry = item.expiryDate.Value.ToString("yyyy-MM-ddTHH:mm:ss");
                }
                pax.AddressLine1 = "Plot No 83";// request.paymentDetails.address1;
                pax.AddressLine2 = "Sector 28";//request.paymentDetails.address2;
                pax.City = "Gurugram";// request.paymentDetails.city;
                pax.CountryCode = "IN";// request.paymentDetails.country;
                pax.CountryName = "India";// request.paymentDetails.countryName;
                pax.Nationality = "IN";// string.IsNullOrEmpty(item.nationality) ? request.paymentDetails.country : item.nationality;

                pax.IsLeadPax = (i == 0 ? true : false);
                pax.ContactNo = request.phoneNo;
                pax.Email = request.emailID;
                if (item.passengerType == Core.PassengerType.Adult)
                {
                    pax.Fare = new TboClass.FareLTR()
                    {
                        AdditionalTxnFeeOfrd = adtFare.AdditionalTxnFeeOfrd / request.adults,
                        AdditionalTxnFeePub = adtFare.AdditionalTxnFeePub / request.adults,
                        BaseFare = adtFare.BaseFare / request.adults,
                        Currency = request.flightResult[ctr].Fare.Currency,
                        OtherCharges = request.flightResult[ctr].Fare.OtherCharges,
                        Tax = adtFare.Tax / request.adults,
                        YQTax = adtFare.YQTax / request.adults,
                    };
                }
                else if (item.passengerType == Core.PassengerType.Child)
                {
                    pax.Fare = new TboClass.FareLTR()
                    {
                        AdditionalTxnFeeOfrd = chdFare.AdditionalTxnFeeOfrd / request.child,
                        AdditionalTxnFeePub = chdFare.AdditionalTxnFeePub / request.child,
                        BaseFare = chdFare.BaseFare / request.child,
                        Currency = request.flightResult[ctr].Fare.Currency,
                        OtherCharges = request.flightResult[ctr].Fare.OtherCharges,
                        Tax = chdFare.Tax / request.child,
                        YQTax = chdFare.YQTax / request.child,
                    };
                }
                else if (item.passengerType == Core.PassengerType.Infant)
                {
                    pax.Fare = new TboClass.FareLTR()
                    {
                        AdditionalTxnFeeOfrd = infFare.AdditionalTxnFeeOfrd / request.infants,
                        AdditionalTxnFeePub = infFare.AdditionalTxnFeePub / request.infants,
                        BaseFare = infFare.BaseFare / request.infants,
                        Currency = request.flightResult[ctr].Fare.Currency,
                        OtherCharges = request.flightResult[ctr].Fare.OtherCharges,
                        Tax = infFare.Tax / request.infants,
                        YQTax = infFare.YQTax / request.infants,
                    };
                }
                TktRequest.Passengers.Add(pax);
                i++;
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(TktRequest);
        }
        //public string getGdsTicketingRequest(Core.Flight.FlightBookingRequest request, int ctr, string TokenId)
        //{
        //    ServicesHub.Tbo.TboClass.GdsTicketingRequest TktRequest = new ServicesHub.Tbo.TboClass.GdsTicketingRequest()
        //    {
        //        EndUserIp = request.userIP,
        //        TokenId = TokenId,
        //        TraceId = request.TvoTraceId,
        //        PNR = ctr == 0 ? request.PNR : request.ReturnPNR,
        //        BookingId = ctr == 0 ? request.TvoBookingID : request.TvoReturnBookingID
        //    };
        //    return Newtonsoft.Json.JsonConvert.SerializeObject(TktRequest);
        //}
        public string getGdsTicketingRequest(Core.Flight.FlightBookingRequest request, string TokenId, string PNR, long TvoBookingID)
        {
            ServicesHub.Tbo.TboClass.GdsTicketingRequest TktRequest = new ServicesHub.Tbo.TboClass.GdsTicketingRequest()
            {
                EndUserIp = request.userIP,
                TokenId = TokenId,
                TraceId = request.TvoTraceId,
                PNR = PNR,
                BookingId = TvoBookingID
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(TktRequest);
        }
        //public string GetBookingDetailsRequestStr(Core.Flight.FlightBookingRequest request, int ctr, string TokenId)
        //{
        //    ServicesHub.Tbo.TboClass.GetBookingDetailsRequest BdetailsRequest = new ServicesHub.Tbo.TboClass.GetBookingDetailsRequest()
        //    {
        //        EndUserIp = request.userIP,
        //        TokenId = TokenId,
        //        PNR = ctr == 0 ? request.PNR : request.ReturnPNR,
        //        BookingId = ctr == 0 ? request.TvoBookingID : request.TvoReturnBookingID
        //    };

        //    return Newtonsoft.Json.JsonConvert.SerializeObject(BdetailsRequest);
        //}

        public string GetFareRuleRequest(Core.Flight.PriceVerificationRequest request, int ctr, string TokenId)
        {
            ServicesHub.Tbo.TboClass.FareRuleRequest fareRuleRequest = new ServicesHub.Tbo.TboClass.FareRuleRequest()
            {
                EndUserIp = request.userIP,
                TokenId = TokenId,
                ResultIndex = request.flightResult[ctr].Fare.tboResultIndex,
                TraceId = request.TvoTraceId
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(fareRuleRequest);
        }
        public string GetSsrRequest(Core.Flight.PriceVerificationRequest request, int ctr, string TokenId)
        {
            ServicesHub.Tbo.TboClass.SSRRequest fareRuleRequest = new ServicesHub.Tbo.TboClass.SSRRequest()
            {
                EndUserIp = request.userIP,
                TokenId = TokenId,
                ResultIndex = request.flightResult[ctr].Fare.tboResultIndex,
                TraceId = request.TvoTraceId
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(fareRuleRequest);
        }
        public string GetAgencyBalanceRequest()
        {
            DataSet ds = new DAL.Tbo.Tbo_DataSetGet().getToken();

            //id = Convert.ToInt32(dr["id"]),
            //tokenID = dr["tokenID"].ToString(),
            //status = dr["status"].ToString(),
            //creationDateTime = Convert.ToDateTime(dr["creationDateTime"]),
            //MemberId = Convert.ToInt32(dr["MemberId"]),
            //AgencyId = Convert.ToInt32(dr["AgencyId"])

            ServicesHub.Tbo.TboClass.AgencyBalanceRequest agencyBalanceRequest = new ServicesHub.Tbo.TboClass.AgencyBalanceRequest()
            {
                ClientId = "tboprod",
                EndUserIp = "182.72.103.98",
                TokenAgencyId = ds.Tables[0].Rows[0]["AgencyId"].ToString(),
                TokenId = ds.Tables[0].Rows[0]["tokenID"].ToString(),
                TokenMemberId = ds.Tables[0].Rows[0]["MemberId"].ToString()
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(agencyBalanceRequest);
        }
        public ServicesHub.Tbo.TboClass.JourneyType getJourneyType(Core.TripType tt)
        {
            ServicesHub.Tbo.TboClass.JourneyType jt = ServicesHub.Tbo.TboClass.JourneyType.Return;
            if (tt == Core.TripType.OneWay)
                jt = ServicesHub.Tbo.TboClass.JourneyType.OneWay;
            else if (tt == Core.TripType.MultiCity)
                jt = ServicesHub.Tbo.TboClass.JourneyType.MultiStop;

            return jt;
        }
        public ServicesHub.Tbo.TboClass.FlightCabinClass getCabinType(Core.CabinType ct)
        {
            ServicesHub.Tbo.TboClass.FlightCabinClass cc = ServicesHub.Tbo.TboClass.FlightCabinClass.All;
            if (ct == Core.CabinType.Economy)
            {
                cc = ServicesHub.Tbo.TboClass.FlightCabinClass.Economy;
            }
            else if (ct == Core.CabinType.PremiumEconomy)
            {
                cc = ServicesHub.Tbo.TboClass.FlightCabinClass.PremiumEconomy;
            }
            else if (ct == Core.CabinType.Business)
            {
                cc = ServicesHub.Tbo.TboClass.FlightCabinClass.Business;
            }
            else if (ct == Core.CabinType.First)
            {
                cc = ServicesHub.Tbo.TboClass.FlightCabinClass.First;
            }
            return cc;
        }
        public int getPaxType(Core.PassengerType paxType)
        {
            int pType = 0;
            if (paxType == Core.PassengerType.Adult)
            {
                pType = 1;
            }
            else if (paxType == Core.PassengerType.Child)
            {
                pType = 2;
            }
            else if (paxType == Core.PassengerType.Infant)
            {
                pType = 3;
            }
            return pType;
        }
        public int getGenderType(Core.Gender genderType)
        {
            int gType = 0;
            if (genderType == Core.Gender.Male)
            {
                gType = 1;
            }
            else if (genderType == Core.Gender.Female)
            {
                gType = 2;
            }
            return gType;
        }
    }
}
