using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.GFS
{
    public class GFSResponseMapping
    {
        public void getResults(Core.Flight.FlightSearchRequest request, ref GFSClass.FlightResponse fsr, ref Core.Flight.FlightSearchResponseShort response)
        {
            int totPax = request.adults + request.child + request.infants;
            if ((fsr.success = true || fsr._data.flights.Count > 0) && fsr._data.flights != null)
            {
                int itinCtr = 0;
                List<Core.Flight.FlightResult> listFlightResult = new List<Core.Flight.FlightResult>();


                foreach (GFSClass.Flight Itin in fsr._data.flights)
                {
                    if (Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.GFS) &&
                       (o.SiteId == request.siteId) && (o.FareType.Count == 0) && o.airline.Contains(Itin.segments[0].legs[0].airline) &&
                       ((o.CountryFrom.Any() && o.CountryFrom.Contains(request.segment[0].orgArp.countryCode)) || o.CountryFrom.Any() == false) &&
                       ((o.CountryTo.Any() && o.CountryTo.Contains(request.segment[0].destArp.countryCode)) || o.CountryTo.Any() == false) &&
                       (o.CountryFrom_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                       (o.CountryTo_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                       ((o.WeekOfDays.Any() && o.WeekOfDays.Contains((WeekDays)Enum.Parse(typeof(WeekDays), Convert.ToString(DateTime.Today.DayOfWeek)))) || o.WeekOfDays.Any() == false) &&
                       ((o.AffiliateId.Any() && o.AffiliateId.Contains(request.sourceMedia)) || o.AffiliateId.Any() == false) &&
                       (o.AffiliateId_Not.Contains(request.sourceMedia) == false) &&
                        ((o.NoOfPaxFrom <= totPax && o.NoOfPaxTo >= totPax)) && (o.device == Device.None || o.device == request.device)).ToList().Count == 0)
                    {
                        if (Itin.seats_available >= request.adults + request.child)
                        {
                            Core.Flight.FlightResult result = new Core.Flight.FlightResult()
                            {
                                AirlineRemark = "",
                                Fare = new Core.Flight.Fare(),
                                IsLCC = true,
                                IsRefundable = false,
                                LastTicketDate = DateTime.Today.AddDays(1),
                                ResultIndex = "GFS" + itinCtr,
                                FlightSegments = new List<Core.Flight.FlightSegment>(),
                                Source = 0,
                                TicketAdvisory = "",
                                cabinClass = request.cabinType,
                                gdsType = Core.GdsType.GFS,
                                valCarrier = Itin.segments[0].legs[0].airline,
                                Color = "",
                                ffFareType = FareType.OFFERFARE,
                                FareList = new List<Core.Flight.Fare>()
                            };

                            #region set flight segment

                            string Airline = string.Empty;

                            Core.Flight.FlightSegment fs = new Core.Flight.FlightSegment() { Segments = new List<Core.Flight.Segment>(), Duration = 0, stop = 0, LayoverTime = 0, SegName = "Depart" };

                            Core.Flight.Segment segment = new Core.Flight.Segment()
                            {
                                Airline = Itin.segments[0].legs[0].airline,
                                ArrTime = DateTime.ParseExact(Itin.segments[0].legs[0].arrival_time, "yyyy-MM-dd HH:mm:ss", new System.Globalization.CultureInfo("en-US")),
                                DepTime = DateTime.ParseExact(Itin.segments[0].legs[0].departure_time, "yyyy-MM-dd HH:mm:ss", new System.Globalization.CultureInfo("en-US")),
                                Origin = Itin.segments[0].legs[0].origin,
                                Destination = Itin.segments[0].legs[0].destination,
                                Duration = Itin.segments[0].duration,
                                FareClass = "",
                                FlightNumber = Itin.segments[0].legs[0].flight_number,
                                FromTerminal = "",
                                ToTerminal = "",
                                IsETicketEligible = true,
                                OperatingCarrier = Itin.segments[0].legs[0].airline,
                                SegmentIndicator = 0,
                                equipmentType = "",
                                CabinClass = request.cabinType,

                            };
                            if (FlightUtility.GetAirport(segment.Origin).countryCode.Equals("IN", StringComparison.OrdinalIgnoreCase) && FlightUtility.GetAirport(segment.Destination).countryCode.Equals("IN", StringComparison.OrdinalIgnoreCase))
                            {
                                segment.Duration = (int)(segment.ArrTime - segment.DepTime).TotalMinutes;
                            }

                            result.ResultCombination += (segment.Airline + segment.FlightNumber + segment.DepTime.ToString("ddMMHHmm"));

                            fs.stop++;
                            fs.Duration += segment.Duration;
                            fs.Segments.Add(segment);
                            result.FlightSegments.Add(fs);
                            #endregion

                            #region set flight fare

                            // decimal totPric = (Itin.price * request.adults) + (Itin.price * request.child) + (Itin.infant_price * request.infants);
                            decimal totPric = Itin.total_price;

                            Core.Flight.Fare fare = new Core.Flight.Fare()
                            {
                                GFS_FlightKey = Itin.key,
                                BaseFare = totPric * 0.75m,
                                Tax = totPric * 0.25m,
                                Currency = request.currencyCode,
                                Markup = 0,
                                PublishedFare = totPric,
                                NetFare = totPric,
                                FareType = FareType.OFFER_FARE_WITH_PNR,
                                cabinType = result.cabinClass,
                                SeatAvailable = Itin.seats_available,
                                gdsType = GdsType.GFS
                            };
                            fare.mojoFareType = MojoFareType.SeriesFareWithPNR;

                            fare.fareBreakdown = new List<Core.Flight.FareBreakdown>();
                            #region set fare Breakup
                            if (request.infants > 0)
                            {
                                Core.Flight.FareBreakdown infFare = new Core.Flight.FareBreakdown();
                                infFare.BaseFare = Itin.infant_price * 0.75m;
                                infFare.Tax = Itin.infant_price * 0.25m;
                                infFare.PassengerType = Core.PassengerType.Infant;
                                fare.fareBreakdown.Add(infFare);
                            }

                            Core.Flight.FareBreakdown adtFare = new Core.Flight.FareBreakdown();
                            adtFare.BaseFare = (Itin.adult_price) * 0.75m;
                            adtFare.Tax = (Itin.adult_price) * 0.25m;
                            adtFare.PassengerType = Core.PassengerType.Adult;
                            fare.fareBreakdown.Add(adtFare);
                            if (request.child > 0)
                            {
                                Core.Flight.FareBreakdown chdFare = new Core.Flight.FareBreakdown();
                                chdFare.BaseFare = (Itin.child_price) * 0.75m;
                                chdFare.Tax = (Itin.child_price) * 0.25m;
                                chdFare.PassengerType = Core.PassengerType.Child;
                                fare.fareBreakdown.Add(chdFare);
                            }
                            #endregion
                            fare.NetFare = fare.grandTotal = fare.PublishedFare + fare.Markup - fare.CommissionEarned;
                            if (request.cabinType == fare.cabinType)
                            {
                                #region BlockAirlines
                                if (Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.GFS) &&
                                             (o.SiteId == request.siteId) && (o.FareType.Any() && o.FareType.Contains(fare.mojoFareType)) &&
                                             ((o.airline.Any() && o.airline.Contains(result.valCarrier)) || o.airline.Any() == false) &&
                                             ((o.CountryFrom.Any() && o.CountryFrom.Contains(request.segment[0].orgArp.countryCode)) || o.CountryFrom.Any() == false) &&
                                             ((o.CountryTo.Any() && o.CountryTo.Contains(request.segment[0].destArp.countryCode)) || o.CountryTo.Any() == false) &&
                                             (o.CountryFrom_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                                             (o.CountryTo_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                                             ((o.WeekOfDays.Any() && o.WeekOfDays.Contains((WeekDays)Enum.Parse(typeof(WeekDays), Convert.ToString(DateTime.Today.DayOfWeek)))) || o.WeekOfDays.Any() == false) &&
                                             ((o.AffiliateId.Any() && o.AffiliateId.Contains(request.sourceMedia)) || o.AffiliateId.Any() == false) &&
                                              ((o.NoOfPaxFrom <= totPax && o.NoOfPaxTo >= totPax)) &&
                                               (o.device == Device.None || o.device == request.device) &&
                                             (o.AffiliateId_Not.Contains(request.sourceMedia) == false)).ToList().Count > 0)
                                {
                                    fare.isBlock = true;
                                }
                                result.FareList.Add(fare);

                                #endregion
                            }
                            #endregion

                            listFlightResult.Add(result);
                        }
                    }
                    itinCtr++;
                }
                response.Results.Add(listFlightResult);
            }
            else
            {
                response.Results.Add(new List<Core.Flight.FlightResult>());
            }
        }

        public void getFareQuoteResponse(ref Core.Flight.PriceVerificationRequest request,
            ref GFSFareQuoteResponse.FareQuoteResponse fqr, ref Core.Flight.FareQuoteResponse response, int ctr)
        {
            if (fqr._data != null && (fqr._data.flight.total_price > request.flightResult[ctr].Fare.PublishedFare))
            {
                response.flightResult.Add(request.flightResult[ctr]);
                response.VerifiedTotalPrice = fqr._data.flight.total_price;
                response.fareIncreaseAmount += fqr._data.flight.total_price - request.flightResult[ctr].Fare.PublishedFare;
                if (response.fareIncreaseAmount > 0)
                {
                    response.isFareChange = true;
                }
            }
            else
            {
                response.fareIncreaseAmount = 0;
                response.VerifiedTotalPrice = request.flightResult[ctr].Fare.PublishedFare;
            }
        }
    }
}
