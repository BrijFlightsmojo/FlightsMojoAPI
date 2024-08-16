using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.AirIQ
{
    public class AirIQResponseMapping
    {
        public void getResults(Core.Flight.FlightSearchRequest request, ref AirIQClass.FlightResponse fsr, ref Core.Flight.FlightSearchResponseShort response)
        {
            int totPax = request.adults + request.child + request.infants;
            if ((fsr.code == 200 ||(!string.IsNullOrEmpty(fsr.status)&& fsr.status.Equals("success", StringComparison.OrdinalIgnoreCase))) && fsr.data != null)
            {
                int itinCtr = 0;
                List<Core.Flight.FlightResult> listFlightResult = new List<Core.Flight.FlightResult>();

                foreach (AirIQClass.Datum Itin in fsr.data)
                {
                    if (Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.AirIQ) &&
                       (o.SiteId == request.siteId) && (o.FareType.Count == 0) && o.airline.Contains(Itin.flight_number.Substring(0, 2)) &&
                       ((o.CountryFrom.Any() && o.CountryFrom.Contains(request.segment[0].orgArp.countryCode)) || o.CountryFrom.Any() == false) &&
                       ((o.CountryTo.Any() && o.CountryTo.Contains(request.segment[0].destArp.countryCode)) || o.CountryTo.Any() == false) &&
                       (o.CountryFrom_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                       (o.CountryTo_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                       ((o.WeekOfDays.Any() && o.WeekOfDays.Contains((WeekDays)Enum.Parse(typeof(WeekDays), Convert.ToString(DateTime.Today.DayOfWeek)))) || o.WeekOfDays.Any() == false) &&
                       ((o.AffiliateId.Any() && o.AffiliateId.Contains(request.sourceMedia)) || o.AffiliateId.Any() == false) &&
                       ((o.NoOfPaxFrom <= totPax && o.NoOfPaxTo >= totPax)) &&
                       (o.AffiliateId_Not.Contains(request.sourceMedia) == false)&&
                       (o.device == Device.None || o.device == request.device)).ToList().Count == 0)
                    {
                        //if (Itin.total_available_seats >= request.adults + request.child)
                        //{
                        Core.Flight.FlightResult result = new Core.Flight.FlightResult()
                        {
                            AirlineRemark = "",
                            Fare = new Core.Flight.Fare(),
                            IsLCC = true,
                            IsRefundable = false,
                            LastTicketDate = DateTime.Today.AddDays(1),
                            ResultIndex = "AirIQ" + itinCtr,
                            FlightSegments = new List<Core.Flight.FlightSegment>(),
                            Source = 0,
                            TicketAdvisory = "",
                            cabinClass = request.cabinType,
                            gdsType = Core.GdsType.FareBoutique,
                            valCarrier = Itin.flight_number.Substring(0, 2),
                            Color = "",
                            ffFareType = FareType.OFFERFARE,
                            FareList = new List<Core.Flight.Fare>()
                        };

                        #region set flight segment

                        string Airline = string.Empty;

                        Core.Flight.FlightSegment fs = new Core.Flight.FlightSegment() { Segments = new List<Core.Flight.Segment>(), Duration = 0, stop = 0, LayoverTime = 0, SegName = "Depart" };

                        Core.Flight.Segment segment = new Core.Flight.Segment()
                        {
                            Airline = Itin.flight_number.Substring(0, 2),
                            ArrTime = DateTime.ParseExact(Itin.arival_date + " " + Itin.arival_time, "yyyy/MM/dd HH:mm", new System.Globalization.CultureInfo("en-US")),
                            DepTime = DateTime.ParseExact(Itin.departure_date + " " + Itin.departure_time, "yyyy/MM/dd HH:mm", new System.Globalization.CultureInfo("en-US")),
                            Origin = Itin.origin,
                            Destination = Itin.destination,
                            Duration = 0,
                            FareClass = "",
                            FlightNumber = System.Text.RegularExpressions.Regex.Replace(Itin.flight_number, "[A-Za-z ]", ""),// Itin.flight_number.Remove(0, 3),
                            FromTerminal = "",
                            ToTerminal = "",
                            IsETicketEligible = true,
                            OperatingCarrier = Itin.flight_number.Substring(0, 2),
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

                        decimal totPric = (Itin.price * request.adults) + (Itin.price * request.child) + (Itin.infant_price * request.infants);
                        Core.Flight.Fare fare = new Core.Flight.Fare()
                        {
                            AQ_ticket_id = Itin.ticket_id,
                            BaseFare = totPric * 0.75m,
                            Tax = totPric * 0.25m,
                            Currency = request.currencyCode,
                            Markup = 0,
                            PublishedFare = totPric,
                            NetFare = totPric,
                            FareType = FareType.OFFER_FARE_WITH_PNR,
                            cabinType = result.cabinClass,
                            gdsType = GdsType.AirIQ
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
                        adtFare.BaseFare = Itin.price * 0.75m;
                        adtFare.Tax = Itin.price * 0.25m;
                        adtFare.PassengerType = Core.PassengerType.Adult;
                        fare.fareBreakdown.Add(adtFare);
                        if (request.child > 0)
                        {
                            Core.Flight.FareBreakdown chdFare = new Core.Flight.FareBreakdown();
                            chdFare.BaseFare = Itin.price * 0.75m;
                            chdFare.Tax = Itin.price * 0.25m;
                            chdFare.PassengerType = Core.PassengerType.Child;
                            fare.fareBreakdown.Add(chdFare);
                        }
                        #endregion
                        fare.NetFare = fare.grandTotal = fare.PublishedFare + fare.Markup - fare.CommissionEarned;
                        if (request.cabinType == fare.cabinType)
                        {
                            #region BlockAirlines
                            if (Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.AirIQ) &&
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
                        if (result.FlightSegments[0].Segments.Count == 1)
                            listFlightResult.Add(result);
                        //}
                    }
                    itinCtr++;
                }
                response.Results.Add(listFlightResult);
            }
        }
    }
}
