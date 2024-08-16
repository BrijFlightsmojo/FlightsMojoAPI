using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tripshope
{
    public class TripshopeRequestMappking
    {
        string officeid = ConfigurationManager.AppSettings["TS_OfcId"].ToString();
        string username = ConfigurationManager.AppSettings["TS_UserName"].ToString();
        string password = ConfigurationManager.AppSettings["TS_PWD"].ToString();
        string Mode = ConfigurationManager.AppSettings["Mode"].ToString();
        public string getFlightSearchRequest(Core.Flight.FlightSearchRequest fsr)
        {

            SearchRequest req = new SearchRequest()
            {
                flightsearchrequest = new Flightsearchrequest(),
            };

            req.flightsearchrequest = new Flightsearchrequest()
            {
                credentials = new Credentials() { officeid = officeid, username = username, password = password },
                destination = fsr.segment[0].destinationAirport,
                origin = fsr.segment[0].originAirport,
                journeytype = fsr.tripType.ToString(),
                numadults = fsr.adults,
                numchildren = fsr.child,
                numinfants = fsr.infants,
                numresults = 100,
                onwarddate = fsr.segment[0].travelDate.ToString("yyyy-MM-dd"),
                returndate = (fsr.segment.Count > 1 ? fsr.segment[1].travelDate.ToString("yyyy-MM-dd") : ""),
                prefcarrier = "All",
                prefclass = getCabinType(fsr.cabinType),
                requestformat = "JSON",
                resultformat = "JSON",
                searchmode = Mode,
                searchtype = getSearchType(fsr.tripType),
                sortkey = "1",
                promocode = "",
                actionname = "flightsearch"
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(req);
        }



        public string GetFareRuleRequest(Core.Flight.PriceVerificationRequest request, int ctr)
        {
            ServicesHub.Tripshope.TripshopeClass.FareRuleRequest FareRule = new TripshopeClass.FareRuleRequest()
            {
                farerulerequest = new TripshopeClass.Farerulerequest()
            };

            FareRule.farerulerequest = new TripshopeClass.Farerulerequest()
            {
                credentials = new TripshopeClass.Credentials { officeid = officeid, username = username, password = password },
                selectedflight = request.nextraflightkey,
                selectedflighttw = request.selectedflighttw,
                wsmode = Mode
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(FareRule);
        }

        public string getFareQuoteRequest(Core.Flight.PriceVerificationRequest request, int ctr)
        {
            TripshopeClass.FareQuoteRequest fareQuote = new TripshopeClass.FareQuoteRequest()
            {
                pricingrequest = new TripshopeClass.Pricingrequest()
            };

            fareQuote.pricingrequest = new TripshopeClass.Pricingrequest()
            {
                credentials = new TripshopeClass.Credentials { officeid = officeid, username = username, password = password },
                domint = request.TravelType,
                //domint = GetTravelType(request.TripType),
                selectedflight = request.nextraflightkey,
                selectedflight_return = request.selectedflighttw,
                wsmode = Mode
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(fareQuote);
        }


        public string getSSRRequest(Core.Flight.PriceVerificationRequest request, int ctr)
        {
            TripshopeClass.SSRRequest SSR = new TripshopeClass.SSRRequest()
            {
                ssrrequest = new TripshopeClass.Ssrrequest()
            };

            SSR.ssrrequest = new TripshopeClass.Ssrrequest()
            {
                credentials = new TripshopeClass.Credentials { officeid = officeid, username = username, password = password },
                triptype = request.triptype,
                selectedflight = request.nextraflightkey,
                wsmode = Mode
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(SSR);
        }

        public string getLccTicketingRequest(Core.Flight.FlightBookingRequest request, int ctr)
        {
            TripshopeClass.TicketingRequest TktRequest = new TripshopeClass.TicketingRequest()
            {
                bookingrequest = new TripshopeClass.Bookingrequest()
            };
            int i = 1;
            TktRequest.bookingrequest = new TripshopeClass.Bookingrequest()
            {
                credentials = new TripshopeClass.Credentials { officeid = officeid, username = username, password = password, agentid = officeid },
                taxInfoStr = request.BookingKey,
                leademail = "agency@flightsmojo.in",//request.emailID
                leadmobile = request.phoneNo,
                leadcountry = "IN",
                leadcity = "DELHI",
                leadstate = "DEL",
                paymentmode = "cashcard",
                domint = request.travelType == Core.TravelType.Domestic ? "domestic" : "international",
                numadults = request.adults,
                numchildren = request.child,
                numinfants = request.infants,
                //gstdetails = new TripshopeClass.Gstdetails { gstaddress = "", gstcompany = "", gstemail = "", gstnumber = "", gstphone = "" },
                wsmode = Mode,
                //ssrList = new TripshopeClass.SsrList()
                //{
                //    SSRRow = new List<TripshopeClass.SSRRow>()
                //    {
                //        new TripshopeClass.SSRRow()
                //        {
                //        carrier = request.flightResult[ctr].valCarrier,
                //        destination = request.flightResult[ctr].FlightSegments[ctr].Segments[ctr].Destination,
                //        jtype = request.flightResult.Count>1 ? "onward" : "return",
                //        origin = request.flightResult[ctr].FlightSegments[ctr].Segments[ctr].Origin,
                //        passenger_fname = "",
                //        passenger_lname = "",
                //        passenger_title = "",
                //        paxid = i,
                //        paxtype = "Adult",
                //        ssrcode = "PSEB",
                //        ssrname = "Sports equipment 20kg",
                //        ssrtype = "baggage",
                //        ssr_chargeableamount = ""
                //        }
                //    }
                //},
                passengerList = new List<TripshopeClass.PassengerList>()
            };

            foreach (var pax in request.passengerDetails)
            {
                TripshopeClass.PassengerList PaxList = new TripshopeClass.PassengerList();
                PaxList.title = pax.title;
                PaxList.first_name = pax.firstName + (string.IsNullOrEmpty(pax.middleName) ? "" : (" " + pax.middleName));
                PaxList.last_name = pax.lastName;
                PaxList.dob = pax.dateOfBirth.ToString("yyyy-MM-dd");
                PaxList.type = getPaxType(pax.passengerType).ToString();
                PaxList.tour_code = "";
                PaxList.deal_code = "";
                PaxList.frequent_flyer_number = "";
                PaxList.visa = "";

                if (!string.IsNullOrEmpty(pax.passportNumber))
                {
                    PaxList.passport = pax.passportNumber;
                    if (pax.expiryDate.HasValue)
                        PaxList.passport_dateofexpiry = pax.expiryDate.Value.ToString("yyyy-MM-dd");
                    if (pax.passportIssueDate.HasValue)
                        PaxList.passport_dateofissue = pax.passportIssueDate.Value.ToString("yyyy-MM-dd");
                }

                PaxList.passport_placeofissue = "";
                PaxList.agentid = officeid;
                PaxList.meal_preference = "";
                PaxList.seat_preference = "";
                PaxList.additional_segmentinfo = "";
                PaxList.profileid = i;
                //PaxList.paxssrinfo = new List<TripshopeClass.Paxssrinfo>()
                //{
                //    new TripshopeClass.Paxssrinfo()
                //    {
                        ////carrier = request.flightResult[ctr].valCarrier,
                        ////destination = request.flightResult[ctr].FlightSegments[ctr].Segments[ctr].Destination,
                        ////jtype = "onward",
                        ////origin = request.flightResult[ctr].FlightSegments[ctr].Segments[ctr].Origin,
                        ////passenger_fname = "",
                        ////passenger_lname = "",
                        ////passenger_title = "",
                        ////paxid = i,
                        ////paxtype = "Adult",
                        ////ssrcode = "PSEB",
                        ////ssrname = "Sports equipment 20kg",
                        ////ssrtype = "baggage",
                        ////ssr_chargeableamount = ""

                //        carrier = "",
                //        destination = "",
                //        jtype = "",
                //        origin = "",
                //        passenger_fname = "",
                //        passenger_lname = "",
                //        passenger_title = "",
                //        paxid = 0,
                //        paxtype = "",
                //        ssrcode = "",
                //        ssrname = "",
                //        ssrtype = "",
                //        ssr_chargeableamount = ""
                //    }
                //};



                TktRequest.bookingrequest.passengerList.Add(PaxList);
                i++;
            }

            //TripshopeClass.SsrList ssr = new TripshopeClass.SsrList();
            //ssr.carrier = request.flightResult[ctr].valCarrier;
            //ssr.destination = request.flightResult[ctr].FlightSegments[ctr].Segments[ctr].Destination;
            //ssr.jtype = "onward";
            //ssr.origin = request.flightResult[ctr].FlightSegments[ctr].Segments[ctr].Origin;
            //ssr.passenger_fname = "";
            //ssr.passenger_lname = "";
            //ssr.passenger_title = "";
            //ssr.paxid = i;
            //ssr.paxtype = "Adult";
            //ssr.ssrcode = "PSEB";
            //ssr.ssrname = "Sports equipment 20kg";
            //ssr.ssrtype = "baggage";
            //ssr.ssr_chargeableamount = "";
            //TktRequest.bookingrequest.ssrList.Add(ssr);

            //TripshopeClass.Ssrrequest ssrlst = new TripshopeClass.Ssrrequest();
            //ssrlst.

            return Newtonsoft.Json.JsonConvert.SerializeObject(TktRequest);
        }


        public string GetTicketingRequest(Core.Flight.FlightBookingRequest request, int ctr)
        {
            TicketDetailsRequest.GetTicketDetailsRequest TktRequest = new TicketDetailsRequest.GetTicketDetailsRequest()
            {
                NextraGetItineraryRequest =new TicketDetailsRequest.NextraGetItineraryRequest()
            };

            TktRequest.NextraGetItineraryRequest = new TicketDetailsRequest.NextraGetItineraryRequest()
            {
                credentials=new TicketDetailsRequest.Credentials { officeid = officeid, username = username, password = password },
                txid = request.txid,
                wsmode = Mode
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(TktRequest);
        }
        

        public string getCabinType(Core.CabinType ct)
        {
            string CabinName = string.Empty;
            if (ct == Core.CabinType.Economy)
            {
                //CabinName = "E-Economy";
                CabinName = "E";
            }
            else if (ct == Core.CabinType.First)
            {
                //CabinName = "F-First class";
                CabinName = "F";
            }
            else if (ct == Core.CabinType.Business)
            {
                // CabinName = "C-Business";
                CabinName = "C";
            }
            //else if (ct == Core.CabinType.First)
            //{
            //    CabinName = "FIRST";
            //}
            else
            {
                // CabinName = "E-Economy";
                CabinName = "E";
            }
            return CabinName;
        }

        public string getSearchType(Core.TripType tt)
        {
            string TripType = string.Empty;
            if (tt == Core.TripType.OneWay)
            {
                TripType = "Normal";
            }
            else if (tt == Core.TripType.RoundTrip)
            {
                TripType = "RoundTrip";
            }
            else if (tt == Core.TripType.MultiCity)
            {
                TripType = "SpecialRoundTrip";
            }
            else
            {
                TripType = "Normal";
            }
            return TripType;
        }


        public string GetTravelType(Core.TravelType GTT)
        {
            string TravelType = string.Empty;
            if (GTT == Core.TravelType.Domestic)
            {
                TravelType = "domestic";
            }
            else if (GTT == Core.TravelType.International)
            {
                TravelType = "international";
            }
            else
            {
                TravelType = "domestic";
            }
            return TravelType;
        }


        public string getPaxType(Core.PassengerType paxType)
        {
            string pType = string.Empty;
            if (paxType == Core.PassengerType.Adult)
            {
                pType = "Adult";
            }
            else if (paxType == Core.PassengerType.Child)
            {
                pType = "Child";
            }
            else if (paxType == Core.PassengerType.Infant)
            {
                pType = "Infant";
            }
            return pType;
        }
    }
}
