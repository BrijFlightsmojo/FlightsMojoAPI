using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace IndiaAPI.Controllers
{
    [RoutePrefix("gfSearch")]
    public class gfSearchController : ApiController
    {
        public static string SecurityCode = "fl1asdfghasdftmoasdfjado2o";
        public static bool authorizeRequest(string securityCodeGet)
        {
            return SecurityCode == securityCodeGet;
        }
        [BasicAuthentication]
        [HttpPost]
        [Route("Flight")]
        public HttpResponseMessage Flight(Core.GoogleFlight.FlightSearchRequest searchRequest)
        {

            #region Make SearchRequest
            Core.Flight.FlightSearchRequest fsr = new Core.Flight.FlightSearchRequest()
            {
                adults = searchRequest.adults,
                child = searchRequest.children,
                infants = searchRequest.infants,
                airline = "",
                cabinType = searchRequest.intendedCabin,
                client = Core.ClientType.Meta,
                currencyCode = "INR",
                deepLink = "",
                isMetaRequest = true,
                device = GetDevice(),
                fareCachingKey = "",
                fareType = 0,
                flexibleSearch = false,
                isGetLiveFare = true,
                isNearBy = false,
                kayakClickId = "",
                limit = "",
                locale = "",
                page = "",
                pageValue = "",
                siteId = Core.SiteId.FlightsMojoIN,
                sourceMedia = "1037",
                rID = "",
                searchDirectFlight = false,
                SearchURL = "",
                segment = new List<Core.Flight.SearchSegment>(),
                serverIP = "",
                sID = "",
                tgy_Request_id = "",
                tripType = string.IsNullOrEmpty(searchRequest.tripSpec.returnDate) ? Core.TripType.OneWay : Core.TripType.RoundTrip,
                userIP = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"],
                userSearchID = getSearchID(),

            };
            fsr.userSessionID = fsr.userSearchID;
            fsr.userLogID = fsr.userSearchID;
            fsr.segment.Add(new Core.Flight.SearchSegment()
            {
                originAirport = searchRequest.tripSpec.departureAirports[0],
                orgArp = Core.FlightUtility.GetAirport(searchRequest.tripSpec.departureAirports[0]),
                destinationAirport = searchRequest.tripSpec.arrivalAirports[0],
                destArp = Core.FlightUtility.GetAirport(searchRequest.tripSpec.arrivalAirports[0]),
                travelDate = DateTime.ParseExact(searchRequest.tripSpec.departureDate, "yyyy-MM-dd", new System.Globalization.CultureInfo("en-US"))
            });

            fsr.travelType = new Core.FlightUtility().getTravelType(fsr.segment[0].orgArp.countryCode, fsr.segment[0].destArp.countryCode);

            if (fsr.tripType == Core.TripType.RoundTrip)
            {
                fsr.segment.Add(new Core.Flight.SearchSegment()
                {
                    originAirport = searchRequest.tripSpec.arrivalAirports[0],
                    orgArp = Core.FlightUtility.GetAirport(searchRequest.tripSpec.arrivalAirports[0]),
                    destinationAirport = searchRequest.tripSpec.departureAirports[0],
                    destArp = Core.FlightUtility.GetAirport(searchRequest.tripSpec.departureAirports[0]),
                    travelDate = DateTime.ParseExact(searchRequest.tripSpec.returnDate, "yyyy-MM-dd", new System.Globalization.CultureInfo("en-US"))
                });
            }
            #endregion
            Core.Flight.FlightSearchResponse result = new FlightMapper().GetFlightResultMultiGDSGF(fsr);
            saveSearchListGoogleApi(fsr,result.Results.First().Count,"");
            var gfResponse = GetGfResponse(result, fsr);
            return Request.CreateResponse(HttpStatusCode.OK, gfResponse);

        }
        private string getSearchID()
        {
            //    var nonceString = new StringBuilder();
            //    for (int i = 0; i < 15; i++)
            //    {
            //        nonceString.Append(validChars[random.Next(0, validChars.Length - 1)]);
            //    }
            //Guid guidValue = Guid.NewGuid();

            //Guid hashed = new Guid(md5.ComputeHash(guidValue.ToByteArray()));
            return (DateTime.Now.ToString("ddMMyyHHmmss") + Guid.NewGuid().ToString("N"));
        }
        private string getUserSessionID()
        {
            string UserSessionID = System.Text.RegularExpressions.Regex.Replace(Guid.NewGuid().ToString(), "[^a-zA-Z0-9]+", "");
            UserSessionID = UserSessionID.Length > 20 ? UserSessionID.Substring(0, 20) : UserSessionID;
            return UserSessionID;
        }
        public string getCabinName(Core.CabinType ct)
        {
            if (ct == Core.CabinType.PremiumEconomy)
            {
                return "PREMIUM_ECONOMY";
            }
            else if (ct == Core.CabinType.First)
            {
                return "FIRST";
            }
            else if (ct == Core.CabinType.Business)
            {
                return "BUSINESS";
            }
            else
            {
                return "ECONOMY";
            }
        }
        private Core.GoogleFlight.FlightResponse GetGfResponse(Core.Flight.FlightSearchResponse SearchRes, Core.Flight.FlightSearchRequest fsr)
        {

            Core.GoogleFlight.FlightResponse response = new Core.GoogleFlight.FlightResponse() { itineraries = new List<Core.GoogleFlight.Itinerary>()};
            if (SearchRes != null && SearchRes.Results != null && SearchRes.Results.Count() > 0 && SearchRes.Results[0].Count > 0 && SearchRes.Results.LastOrDefault().Count > 0)
            {
                if (SearchRes.Results.Count == 1)
                {
                    #region for one way

                    foreach (Core.Flight.FlightResult result in SearchRes.Results[0])
                    {
                        Core.GoogleFlight.Itinerary itin = new Core.GoogleFlight.Itinerary()
                        {
                            booking_url = "https://www.flightsmojo.in/flight/itinerarygf?org=" + fsr.segment[0].originAirport + "&dest=" + fsr.segment[0].destinationAirport + "&depdate=" + fsr.segment[0].travelDate.ToString("dd-MM-yyyy") + "&retdate=" + (fsr.segment.Count > 1 ? fsr.segment[1].travelDate.ToString("dd-MM-yyyy") : "") + "&tripType=" + (fsr.segment.Count > 1 ? "R" : "O") + "&adults=" + fsr.adults + "&child=" + fsr.child + "&infants=" + fsr.infants + "&cabin=" + ((int)fsr.cabinType) + "&utm_source=" + fsr.sourceMedia + "&currency=inr",
                            outbound = new Core.GoogleFlight.Outbound(),
                            price = new Core.GoogleFlight.Price(),
                            validity_seconds = "10800",
                            virtual_interline_type = Convert.ToString(Core.VirtualInterlineType.DEFAULT_TYPE)
                        };
                        itin.price.currency_code = "INR";
                        itin.price.total_decimal = result.Fare.grandTotal.ToString("f2");
                        itin.booking_url += ("&price=" + result.Fare.grandTotal.ToString("f2"));

                        string resultID = string.Empty;
                        itin.outbound = new Core.GoogleFlight.Outbound() { segments = new List<Core.GoogleFlight.Segment>() };
                        int ctrFseg = 0;
                        foreach (var fs in result.FlightSegments)
                        {
                            if (ctrFseg == 1 && itin.inbound == null)
                            {
                                itin.inbound = new Core.GoogleFlight.Inbound() { segments = new List<Core.GoogleFlight.Segment>() };
                            }
                            foreach (var seg in fs.Segments)
                            {
                                Core.GoogleFlight.Segment gSeg = new Core.GoogleFlight.Segment()
                                {
                                    legs = new List<Core.GoogleFlight.Leg>(),
                                    cabin_code = getCabinName(seg.CabinClass),
                                    baggage_allowance = new Core.GoogleFlight.BaggageAllowance()
                                };
                                Core.GoogleFlight.Leg leg = new Core.GoogleFlight.Leg()
                                {
                                    carrier = seg.Airline,
                                    flight_number = seg.FlightNumber,
                                    departure_airport = seg.Origin,
                                    arrival_airport = seg.Destination,
                                    departure_date_time = seg.DepTime,
                                    arrival_date_time = seg.ArrTime
                                };
                                resultID += (leg.carrier + "_" + leg.flight_number + "_" + leg.departure_date_time.ToString("yyMMdd|HHmm") + "_" + leg.arrival_date_time.ToString("yyMMdd|HHmm"));
                                gSeg.legs.Add(leg);
                                if (ctrFseg == 0)
                                {
                                    itin.outbound.segments.Add(gSeg);
                                }
                                else
                                {
                                    itin.inbound.segments.Add(gSeg);
                                }
                            }
                            ctrFseg++;
                        }
                        itin.booking_url += ("&rdtl=" + resultID);
                        response.itineraries.Add(itin);
                    }
                    #endregion
                }
                else
                {
                    #region Make Combination Special Return
                    List<Core.Flight.FlightResult> depSpecialReturn = SearchRes.Results[0].Where(k => k.Fare.FareType == Core.FareType.SPECIALRETURN).ToList();
                    List<Core.Flight.FlightResult> retSpecialReturn = SearchRes.Results[1].Where(k => k.Fare.FareType == Core.FareType.SPECIALRETURN).ToList();
                    if (depSpecialReturn != null && depSpecialReturn.Count > 0 && retSpecialReturn != null && retSpecialReturn.Count > 0)
                    {
                        for (int i = 0; i < depSpecialReturn.Count; i++)
                        {
                            List<Core.Flight.FlightResult> retSpecialReturnMatch = retSpecialReturn.Where(k => k.Fare.msri.Contains(depSpecialReturn[i].Fare.sri)).ToList();
                            for (int j = 0; j < retSpecialReturnMatch.Count; j++)
                            {
                                TimeSpan ts = retSpecialReturnMatch[j].FlightSegments[0].Segments[0].DepTime - depSpecialReturn[i].FlightSegments[0].Segments.Last().ArrTime;
                                if (ts.TotalMinutes > 240)
                                {

                                    Core.GoogleFlight.Itinerary itin = new Core.GoogleFlight.Itinerary()
                                    {
                                        booking_url = "https://www.flightsmojo.in/flight/itinerarygf?org=" + fsr.segment[0].originAirport + "&dest=" + fsr.segment[0].destinationAirport + "&depdate=" + fsr.segment[0].travelDate.ToString("dd-MM-yyyy") + "&retdate=" + (fsr.segment.Count > 1 ? fsr.segment[1].travelDate.ToString("dd-MM-yyyy") : "") + "&tripType=" + (fsr.segment.Count > 1 ? "R" : "O") + "&adults=" + fsr.adults + "&child=" + fsr.child + "&infants=" + fsr.infants + "&cabin=" + ((int)fsr.cabinType) + "&utm_source=" + fsr.sourceMedia + "&currency=inr",
                                        outbound = new Core.GoogleFlight.Outbound(),
                                        price = new Core.GoogleFlight.Price(),
                                        validity_seconds = "10800",
                                        virtual_interline_type = Convert.ToString(Core.VirtualInterlineType.DEFAULT_TYPE)
                                    };
                                    itin.price.currency_code = "INR";
                                    itin.price.total_decimal = (depSpecialReturn[i].Fare.grandTotal + retSpecialReturn[j].Fare.grandTotal).ToString("f2");
                                    itin.booking_url += ("&price=" + itin.price.total_decimal);

                                    string resultID = string.Empty;
                                    itin.outbound = new Core.GoogleFlight.Outbound() { segments = new List<Core.GoogleFlight.Segment>() };
                                    foreach (var fs in depSpecialReturn[i].FlightSegments)
                                    {
                                        foreach (var seg in fs.Segments)
                                        {
                                            Core.GoogleFlight.Segment gSeg = new Core.GoogleFlight.Segment()
                                            {
                                                legs = new List<Core.GoogleFlight.Leg>(),
                                                cabin_code = getCabinName(seg.CabinClass),
                                                baggage_allowance = new Core.GoogleFlight.BaggageAllowance()
                                            };
                                            Core.GoogleFlight.Leg leg = new Core.GoogleFlight.Leg()
                                            {
                                                carrier = seg.Airline,
                                                flight_number = seg.FlightNumber,
                                                departure_airport = seg.Origin,
                                                arrival_airport = seg.Destination,
                                                departure_date_time = seg.DepTime,
                                                arrival_date_time = seg.ArrTime
                                            };
                                            resultID += (leg.carrier + "_" + leg.flight_number + "_" + leg.departure_date_time.ToString("yyMMdd|HHmm") + "_" + leg.arrival_date_time.ToString("yyMMdd|HHmm"));
                                            gSeg.legs.Add(leg);
                                            itin.outbound.segments.Add(gSeg);
                                        }
                                    }
                                    itin.booking_url += ("&rdtl=" + resultID);

                                    string resultID2 = string.Empty;
                                    itin.inbound = new Core.GoogleFlight.Inbound() { segments = new List<Core.GoogleFlight.Segment>() };
                                    foreach (var fs in depSpecialReturn[i].FlightSegments)
                                    {
                                        foreach (var seg in fs.Segments)
                                        {
                                            Core.GoogleFlight.Segment gSeg = new Core.GoogleFlight.Segment()
                                            {
                                                legs = new List<Core.GoogleFlight.Leg>(),
                                                cabin_code = getCabinName(seg.CabinClass),
                                                baggage_allowance = new Core.GoogleFlight.BaggageAllowance()
                                            };
                                            Core.GoogleFlight.Leg leg = new Core.GoogleFlight.Leg()
                                            {
                                                carrier = seg.Airline,
                                                flight_number = seg.FlightNumber,
                                                departure_airport = seg.Origin,
                                                arrival_airport = seg.Destination,
                                                departure_date_time = seg.DepTime,
                                                arrival_date_time = seg.ArrTime
                                            };
                                            resultID2 += (leg.carrier + "_" + leg.flight_number + "_" + leg.departure_date_time.ToString("yyMMdd|HHmm") + "_" + leg.arrival_date_time.ToString("yyMMdd|HHmm"));
                                            gSeg.legs.Add(leg);
                                            itin.inbound.segments.Add(gSeg);
                                        }
                                    }
                                    itin.booking_url += ("&r2dtl=" + resultID2);
                                    response.itineraries.Add(itin);

                                }
                            }
                        }
                    }
                    #endregion
                    #region Make Combination top 10
                    List<Core.Flight.FlightResult> dep = SearchRes.Results[0].Where(k => k.Fare.FareType != Core.FareType.SPECIALRETURN).ToList();
                    List<Core.Flight.FlightResult> ret = SearchRes.Results[1].Where(k => k.Fare.FareType != Core.FareType.SPECIALRETURN).ToList();
                    for (int i = 0; i < dep.Count && i < 10; i++)
                    {
                        for (int j = 0; j < ret.Count && j < 10; j++)
                        {
                            TimeSpan ts = ret[j].FlightSegments[0].Segments[0].DepTime - dep[i].FlightSegments[0].Segments.Last().ArrTime;
                            if (ts.TotalMinutes > 240)
                            {
                                Core.GoogleFlight.Itinerary itin = new Core.GoogleFlight.Itinerary()
                                {
                                    booking_url = "https://www.flightsmojo.in/flight/itinerarygf?org=" + fsr.segment[0].originAirport + "&dest=" + fsr.segment[0].destinationAirport + "&depdate=" + fsr.segment[0].travelDate.ToString("dd-MM-yyyy") + "&retdate=" + (fsr.segment.Count > 1 ? fsr.segment[1].travelDate.ToString("dd-MM-yyyy") : "") + "&tripType=" + (fsr.segment.Count > 1 ? "R" : "O") + "&adults=" + fsr.adults + "&child=" + fsr.child + "&infants=" + fsr.infants + "&cabin=" + ((int)fsr.cabinType) + "&utm_source=" + fsr.sourceMedia + "&currency=inr",
                                    outbound = new Core.GoogleFlight.Outbound(),
                                    price = new Core.GoogleFlight.Price(),
                                    validity_seconds = "10800",
                                    virtual_interline_type = Convert.ToString(Core.VirtualInterlineType.DEFAULT_TYPE)
                                };
                                itin.price.currency_code = "INR";
                                itin.price.total_decimal = (dep[i].Fare.grandTotal + ret[j].Fare.grandTotal).ToString("f2");
                                itin.booking_url += ("&price=" + itin.price.total_decimal);

                                string resultID = string.Empty;
                                itin.outbound = new Core.GoogleFlight.Outbound() { segments = new List<Core.GoogleFlight.Segment>() };
                                foreach (var fs in dep[i].FlightSegments)
                                {
                                    foreach (var seg in fs.Segments)
                                    {
                                        Core.GoogleFlight.Segment gSeg = new Core.GoogleFlight.Segment()
                                        {
                                            legs = new List<Core.GoogleFlight.Leg>(),
                                            cabin_code = getCabinName(seg.CabinClass),
                                            baggage_allowance = new Core.GoogleFlight.BaggageAllowance()
                                        };
                                        Core.GoogleFlight.Leg leg = new Core.GoogleFlight.Leg()
                                        {
                                            carrier = seg.Airline,
                                            flight_number = seg.FlightNumber,
                                            departure_airport = seg.Origin,
                                            arrival_airport = seg.Destination,
                                            departure_date_time = seg.DepTime,
                                            arrival_date_time = seg.ArrTime
                                        };
                                        resultID += (leg.carrier + "_" + leg.flight_number + "_" + leg.departure_date_time.ToString("yyMMdd|HHmm") + "_" + leg.arrival_date_time.ToString("yyMMdd|HHmm"));
                                        gSeg.legs.Add(leg);
                                        itin.outbound.segments.Add(gSeg);
                                    }
                                }
                                itin.booking_url += ("&rdtl=" + resultID);

                                string resultID2 = string.Empty;
                                itin.inbound = new Core.GoogleFlight.Inbound() { segments = new List<Core.GoogleFlight.Segment>() };
                                foreach (var fs in ret[i].FlightSegments)
                                {
                                    foreach (var seg in fs.Segments)
                                    {
                                        Core.GoogleFlight.Segment gSeg = new Core.GoogleFlight.Segment()
                                        {
                                            legs = new List<Core.GoogleFlight.Leg>(),
                                            cabin_code = getCabinName(seg.CabinClass),
                                            baggage_allowance = new Core.GoogleFlight.BaggageAllowance()
                                        };
                                        Core.GoogleFlight.Leg leg = new Core.GoogleFlight.Leg()
                                        {
                                            carrier = seg.Airline,
                                            flight_number = seg.FlightNumber,
                                            departure_airport = seg.Origin,
                                            arrival_airport = seg.Destination,
                                            departure_date_time = seg.DepTime,
                                            arrival_date_time = seg.ArrTime
                                        };
                                        resultID2 += (leg.carrier + "_" + leg.flight_number + "_" + leg.departure_date_time.ToString("yyMMdd|HHmm") + "_" + leg.arrival_date_time.ToString("yyMMdd|HHmm"));
                                        gSeg.legs.Add(leg);
                                        itin.inbound.segments.Add(gSeg);
                                    }
                                }
                                itin.booking_url += ("&r2dtl=" + resultID2);
                                response.itineraries.Add(itin);
                            }
                        }
                    }
                    #endregion
                    #region Make Combination after 10
                    for (int i = 10; i < dep.Count && i < 30; i++)
                    {
                        for (int j = 10; j < ret.Count && j < 30; j++)
                        {
                            TimeSpan ts = ret[j].FlightSegments[0].Segments[0].DepTime - dep[i].FlightSegments[0].Segments.Last().ArrTime;
                            if (ts.TotalMinutes > 240)
                            {
                                Core.GoogleFlight.Itinerary itin = new Core.GoogleFlight.Itinerary()
                                {
                                    booking_url = "https://www.flightsmojo.in/flight/itinerarygf?org=" + fsr.segment[0].originAirport + "&dest=" + fsr.segment[0].destinationAirport + "&depdate=" + fsr.segment[0].travelDate.ToString("dd-MM-yyyy") + "&retdate=" + (fsr.segment.Count > 1 ? fsr.segment[1].travelDate.ToString("dd-MM-yyyy") : "") + "&tripType=" + (fsr.segment.Count > 1 ? "R" : "O") + "&adults=" + fsr.adults + "&child=" + fsr.child + "&infants=" + fsr.infants + "&cabin=" + ((int)fsr.cabinType) + "&utm_source=" + fsr.sourceMedia + "&currency=inr",
                                    outbound = new Core.GoogleFlight.Outbound(),
                                    price = new Core.GoogleFlight.Price(),
                                    validity_seconds = "10800",
                                    virtual_interline_type = Convert.ToString(Core.VirtualInterlineType.DEFAULT_TYPE)
                                };
                                itin.price.currency_code = "INR";
                                itin.price.total_decimal = (dep[i].Fare.grandTotal + ret[j].Fare.grandTotal).ToString("f2");
                                itin.booking_url += ("&price=" + itin.price.total_decimal);

                                string resultID = string.Empty;
                                itin.outbound = new Core.GoogleFlight.Outbound() { segments = new List<Core.GoogleFlight.Segment>() };
                                foreach (var fs in dep[i].FlightSegments)
                                {
                                    foreach (var seg in fs.Segments)
                                    {
                                        Core.GoogleFlight.Segment gSeg = new Core.GoogleFlight.Segment()
                                        {
                                            legs = new List<Core.GoogleFlight.Leg>(),
                                            cabin_code = getCabinName(seg.CabinClass),
                                            baggage_allowance = new Core.GoogleFlight.BaggageAllowance()
                                        };
                                        Core.GoogleFlight.Leg leg = new Core.GoogleFlight.Leg()
                                        {
                                            carrier = seg.Airline,
                                            flight_number = seg.FlightNumber,
                                            departure_airport = seg.Origin,
                                            arrival_airport = seg.Destination,
                                            departure_date_time = seg.DepTime,
                                            arrival_date_time = seg.ArrTime
                                        };
                                        resultID += (leg.carrier + "_" + leg.flight_number + "_" + leg.departure_date_time.ToString("yyMMdd|HHmm") + "_" + leg.arrival_date_time.ToString("yyMMdd|HHmm"));
                                        gSeg.legs.Add(leg);
                                        itin.outbound.segments.Add(gSeg);
                                    }
                                }
                                itin.booking_url += ("&rdtl=" + resultID);

                                string resultID2 = string.Empty;
                                itin.inbound = new Core.GoogleFlight.Inbound() { segments = new List<Core.GoogleFlight.Segment>() };
                                foreach (var fs in ret[i].FlightSegments)
                                {
                                    foreach (var seg in fs.Segments)
                                    {
                                        Core.GoogleFlight.Segment gSeg = new Core.GoogleFlight.Segment()
                                        {
                                            legs = new List<Core.GoogleFlight.Leg>(),
                                            cabin_code = getCabinName(seg.CabinClass),
                                            baggage_allowance = new Core.GoogleFlight.BaggageAllowance()
                                        };
                                        Core.GoogleFlight.Leg leg = new Core.GoogleFlight.Leg()
                                        {
                                            carrier = seg.Airline,
                                            flight_number = seg.FlightNumber,
                                            departure_airport = seg.Origin,
                                            arrival_airport = seg.Destination,
                                            departure_date_time = seg.DepTime,
                                            arrival_date_time = seg.ArrTime
                                        };
                                        resultID2 += (leg.carrier + "_" + leg.flight_number + "_" + leg.departure_date_time.ToString("yyMMdd|HHmm") + "_" + leg.arrival_date_time.ToString("yyMMdd|HHmm"));
                                        gSeg.legs.Add(leg);
                                        itin.inbound.segments.Add(gSeg);
                                    }
                                }
                                itin.booking_url += ("&r2dtl=" + resultID2);
                                response.itineraries.Add(itin);
                            }
                        }
                    }
                    #endregion
                }
            }
            return response;
        }


        public Core.Device GetDevice()
        {
            string u = System.Web.HttpContext.Current.Request.Headers["User-Agent"];
            System.Text.RegularExpressions.Regex b = new System.Text.RegularExpressions.Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);
            System.Text.RegularExpressions.Regex v = new System.Text.RegularExpressions.Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);
            if ((b.IsMatch(u) || v.IsMatch(u.Substring(0, 4))))
            {
                return Core.Device.Mobile;
            }
            return Core.Device.Desktop;
        }

        private void saveSearchListGoogleApi(Core.Flight.FlightSearchRequest flightSearchRequest, int totalResult, string Provider)
        {
            var save = Task.Run(async () =>
            {
                await new DAL.Deal.UserSearchHistory().SaveUserSearchHistoryGoogleApi(flightSearchRequest, totalResult, Provider, GlobalData.ServerID);
            });
        }
    }
}
