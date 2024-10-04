
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.FareBoutique
{
    public class FareBoutiqueResponseMapping
    {
        public void getResults(Core.Flight.FlightSearchRequest request, ref FareBoutiqueClass.FlightResponse fsr, ref Core.Flight.FlightSearchResponseShort response)
        {
            int totPax = request.adults + request.child + request.infants;
            if (fsr != null && fsr.data != null && ((fsr.errorCode != null && fsr.errorCode == 0) || (!string.IsNullOrEmpty(fsr.replyCode) && fsr.replyCode.Equals("success", StringComparison.OrdinalIgnoreCase))))
            {
                response.FB_booking_token_id = fsr.booking_token_id;
                int itinCtr = 0;

                List<Core.Flight.FlightResult> listFlightResult = new List<Core.Flight.FlightResult>();
                foreach (FareBoutiqueClass.Datum Itin in fsr.data)
                {
                    if (Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.FareBoutique) &&
                       (o.SiteId == request.siteId) && (o.FareType.Count == 0) && o.airline.Contains(Itin.airline_code) &&
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
                        if (Itin.total_available_seats >= request.adults + request.child)
                        {
                            Core.Flight.FlightResult result = new Core.Flight.FlightResult()
                            {
                                AirlineRemark = "",
                                Fare = new Core.Flight.Fare(),
                                IsLCC = true,
                                IsRefundable = false,
                                LastTicketDate = DateTime.Today.AddDays(1),
                                ResultIndex = "FB" + itinCtr++,
                                FlightSegments = new List<Core.Flight.FlightSegment>(),
                                Source = 0,
                                TicketAdvisory = "",
                                cabinClass = request.cabinType,
                                gdsType = Core.GdsType.FareBoutique,
                                valCarrier = Itin.airline_code,
                                Color = "",
                                ffFareType = FareType.OFFERFARE,
                                FareList = new List<Core.Flight.Fare>()
                            };
                            bool isSetCabinType = true;
                            #region set flight segment

                            string Airline = string.Empty;

                            Core.Flight.FlightSegment fs = new Core.Flight.FlightSegment() { Segments = new List<Core.Flight.Segment>(), Duration = 0, stop = 0, LayoverTime = 0, SegName = "Depart" };
                            Core.Flight.Segment segment = new Core.Flight.Segment()
                            {
                                Airline = Itin.airline_code,
                                ArrTime = DateTime.ParseExact(Itin.arrival_date + " " + Itin.arrival_time, "yyyy-MM-dd H:mm", new System.Globalization.CultureInfo("en-US")),
                                DepTime = DateTime.ParseExact(Itin.departure_date + " " + Itin.departure_time, "yyyy-MM-dd H:mm", new System.Globalization.CultureInfo("en-US")),
                                Origin = Itin.departure_airport_code,
                                Destination = Itin.arrival_airport_code,
                                Duration = 0,
                                FareClass = "",
                                FlightNumber = Itin.flight_number.Length > 4 ? "" : Itin.flight_number,
                                FromTerminal = Itin.departure_terminal_no,
                                ToTerminal = Itin.arrival_terminal_no,
                                IsETicketEligible = true,
                                OperatingCarrier = Itin.airline_code,
                                SegmentIndicator = 0,
                                equipmentType = "",
                                CabinClass = request.cabinType,

                            };

                            if (segment.CabinClass == CabinType.None)
                            {
                                isSetCabinType = false;
                            }
                            string retBaggage = string.Empty, retCabinBaggage = string.Empty;
                            GetBaggege(request.cabinType, request.travelType, segment.Baggage, segment.CabinBaggage, ref retBaggage, ref retCabinBaggage);
                            segment.Baggage = retBaggage;
                            segment.CabinBaggage = retCabinBaggage;

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

                            Core.Flight.Fare fare = new Core.Flight.Fare()
                            {
                                FB_flight_id = Itin.flight_id,
                                FB_static = Itin.@static,
                                BaseFare = Itin.total_payable_price * 0.75m,
                                Tax = Itin.total_payable_price * 0.25m,
                                Currency = request.currencyCode,
                                Markup = 0,
                                PublishedFare = Itin.total_payable_price,
                                NetFare = Itin.total_payable_price,
                                FareType = FareType.OFFER_FARE_WITH_PNR,
                                cabinType = result.cabinClass,
                                gdsType = GdsType.FareBoutique,
                                SeatAvailable = Itin.total_available_seats,
                                subProvider = SMCommanMethod.getSubProvider(Itin.inventory_from != null ? Itin.inventory_from.company_name : ""),
                                refundType = Core.RefundType.NonRefundable
                            };
                            fare.mojoFareType = MojoFareType.SeriesFareWithPNR;

                            fare.fareBreakdown = new List<Core.Flight.FareBreakdown>();
                            #region set fare Breakup
                            if (request.infants > 0)
                            {
                                Core.Flight.FareBreakdown infFare = new Core.Flight.FareBreakdown();
                                infFare.BaseFare = 1500 * 0.75m;
                                infFare.Tax = 1500 * 0.25m;
                                infFare.PassengerType = Core.PassengerType.Infant;
                                fare.fareBreakdown.Add(infFare);
                            }

                            decimal PaxTotPrice = (Itin.total_payable_price - (1500 * request.infants)) / (request.adults + request.child);
                            Core.Flight.FareBreakdown adtFare = new Core.Flight.FareBreakdown();
                            adtFare.BaseFare = PaxTotPrice * 0.75m;
                            adtFare.Tax = PaxTotPrice * 0.25m;
                            adtFare.PassengerType = Core.PassengerType.Adult;
                            fare.fareBreakdown.Add(adtFare);
                            if (request.child > 0)
                            {
                                Core.Flight.FareBreakdown chdFare = new Core.Flight.FareBreakdown();
                                chdFare.BaseFare = PaxTotPrice * 0.75m;
                                chdFare.Tax = PaxTotPrice * 0.25m;
                                chdFare.PassengerType = Core.PassengerType.Child;
                                fare.fareBreakdown.Add(chdFare);
                            }
                            #endregion
                            fare.NetFare = fare.grandTotal = fare.PublishedFare + fare.Markup - fare.CommissionEarned;

                            if (result.valCarrier == "6E")
                            {

                            }

                            if (request.cabinType == fare.cabinType)
                            {
                                #region BlockAirlines
                                if (Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.FareBoutique) &&
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
                                //if (result.valCarrier == "SG" && request.segment[0].travelDate > DateTime.Today.AddDays(15) && (fare.mojoFareType == MojoFareType.SeriesFareWithoutPNR || fare.mojoFareType == MojoFareType.SeriesFareWithPNR))
                                //{
                                //    fare.isBlock = true;
                                //}
                                result.FareList.Add(fare);

                                #endregion
                            }
                            #endregion
                            if (result.FlightSegments[0].Segments.Count == 1)
                                listFlightResult.Add(result);
                        }
                    }
                }

                response.Results.Add(listFlightResult);
            }
            else
            {
                response.Results.Add(new List<Core.Flight.FlightResult>()); 
            }
        }

        public void GetBaggege(CabinType ct, TravelType tt, string Baggage, string CabinBaggage, ref string retBaggage, ref string retCabinBaggage)
        {
            if (tt == TravelType.Domestic && ct == CabinType.Economy)
            {
                if (string.IsNullOrEmpty(Baggage))
                {
                    retBaggage = "15KG";
                }
                else
                {
                    retBaggage = Baggage;
                }
                if (string.IsNullOrEmpty(CabinBaggage))
                {
                    retCabinBaggage = "7KG";
                }
                else
                {
                    retCabinBaggage = CabinBaggage;
                }
            }
            else
            {
                retBaggage = Baggage;
                retCabinBaggage = CabinBaggage;
            }
        }

        public void getFareQuoteResponse(ref Core.Flight.PriceVerificationRequest request,
            ref FB_FareQuote.FareQuoteResponse fqr, ref Core.Flight.FareQuoteResponse response, int ctr)
        {
            if (fqr.data != null && (fqr.data.total_amount > request.flightResult[ctr].Fare.PublishedFare))
            {
                response.flightResult.Add(request.flightResult[ctr]);
                response.VerifiedTotalPrice = fqr.data.total_amount;
                response.fareIncreaseAmount += fqr.data.total_amount - request.flightResult[ctr].Fare.PublishedFare;
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
