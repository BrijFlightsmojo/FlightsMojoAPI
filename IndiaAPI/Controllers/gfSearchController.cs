using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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
                device = Core.Device.Desktop,
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
                userIP = "",
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
    }
}
