
using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tbo
{
    public class TboResponseMapping
    {
        public void getResults(Core.Flight.FlightSearchRequest request, ref TboClass.FlightResponse fsr, ref Core.Flight.FlightSearchResponseShort response)
        {
            int totPax = request.adults + request.child + request.infants;
            if (fsr.Response != null && fsr.Response.Results != null && fsr.Response.Results.Count > 0)
            {
                response.TraceId = fsr.Response.TraceId;
                int itinCtr = 0;
                int ctrError = 0;
                //List<TboClass.fareClassification> fc = new List<TboClass.fareClassification>();

                foreach (List<TboClass.Itinerary> listItin in fsr.Response.Results)
                {
                    List<Core.Flight.FlightResult> listFlightResult = new List<Core.Flight.FlightResult>();

                    foreach (TboClass.Itinerary Itin in listItin)
                    {
                        if (Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.Tbo) &&
                                   (o.SiteId == request.siteId) && (o.FareType.Count == 0) && o.airline.Contains(Itin.ValidatingAirline) &&
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
                            Core.Flight.FlightResult result = new Core.Flight.FlightResult()
                            {
                                AirlineRemark = Itin.AirlineRemark,
                                Fare = new Core.Flight.Fare(),
                                IsLCC = Itin.IsLCC,
                                IsRefundable = Itin.IsRefundable,
                                LastTicketDate = Itin.LastTicketDate,
                                ResultIndex = Itin.ResultIndex,
                                FlightSegments = new List<Core.Flight.FlightSegment>(),
                                Source = Itin.Source,
                                TicketAdvisory = Itin.TicketAdvisory,
                                //ValidatingAirline = Itin.ValidatingAirline,
                                cabinClass = request.cabinType,
                                gdsType = Core.GdsType.Tbo,
                                valCarrier = Itin.ValidatingAirline,
                                Color = Itin.FareClassification != null ? Itin.FareClassification.Color : "",
                                //fareType = getFareType(Itin.FareClassification.Type),
                                //groupID = "Tbo" + (itinCtr == 0 ? "OB" : "IB"),
                                ffFareType = getFareType(Itin.FareClassification != null ? Itin.FareClassification.Type : ""),
                                FareList = new List<Core.Flight.Fare>()
                            };
                            if (result.Color == "RosyBrown")
                            {

                            }
                            #region set flight segment
                            int segCtr = 0;
                            string Airline = string.Empty;
                            //string groupID = "Tbo" + (itinCtr == 0 ? "OB" : "IB");
                            int seatAvail = 9;
                            foreach (List<TboClass.FlightSegment> fseg in Itin.Segments)
                            {
                                Core.Flight.FlightSegment fs = new Core.Flight.FlightSegment() { Segments = new List<Core.Flight.Segment>(), Duration = 0, stop = 0, LayoverTime = 0, SegName = (segCtr == 0 && itinCtr == 0 ? "Depart" : "Return") };
                                int layCtr = 1;
                                foreach (TboClass.FlightSegment seg in fseg)
                                {
                                    Core.Flight.Segment segment = new Core.Flight.Segment()
                                    {
                                        Airline = seg.Airline.AirlineCode,
                                        ArrTime = seg.Destination.ArrTime,
                                        DepTime = seg.Origin.DepTime,
                                        Origin = seg.Origin.Airport.AirportCode,
                                        Destination = seg.Destination.Airport.AirportCode,
                                        Duration = seg.Duration,
                                        FareClass = seg.Airline.FareClass,
                                        FlightNumber = seg.Airline.FlightNumber,
                                        FromTerminal = seg.Origin.Airport.Terminal,
                                        ToTerminal = seg.Destination.Airport.Terminal,
                                        IsETicketEligible = seg.IsETicketEligible,
                                        OperatingCarrier =string.IsNullOrEmpty( seg.Airline.OperatingCarrier) ?seg.Airline.AirlineCode: seg.Airline.OperatingCarrier  ,
                                        SegmentIndicator = seg.SegmentIndicator,
                                        equipmentType = seg.Craft,
                                        CabinClass = request.cabinType,
                                    };
                                    seatAvail = seatAvail < seg.NoOfSeatAvailable ? seg.NoOfSeatAvailable : seatAvail;
                                    result.ResultCombination += (segment.Airline + segment.FlightNumber + segment.DepTime.ToString("ddMMHHmm"));
                                    #region LayOverTime
                                    if (fseg.Count > layCtr)
                                    {
                                        TimeSpan ts = fseg[layCtr].Origin.DepTime - segment.ArrTime;
                                        if (ts.Hours > 0 || ts.Minutes > 0)
                                        {
                                            if (ts.Hours > 0 && ts.Minutes > 0)
                                            {
                                                segment.layOverTime = (ts.Hours * 60) + ts.Minutes;
                                            }
                                            else if (ts.Hours > 0)
                                            {
                                                segment.layOverTime = ts.Hours * 60;
                                            }
                                            else if (ts.Minutes > 0)
                                            {
                                                segment.layOverTime = ts.Minutes;
                                            }
                                        }
                                        else
                                        {
                                            segment.layOverTime = 0;
                                        }
                                        fs.LayoverTime += segment.layOverTime;
                                    }
                                    layCtr++;
                                    #endregion

                                    //groupID += "" + segment.Airline + segment.FlightNumber + "_" + segment.DepTime.ToString("ddMM");
                                    fs.stop++;
                                    fs.Duration += segment.Duration;
                                    fs.Segments.Add(segment);
                                }
                                result.FlightSegments.Add(fs);
                                segCtr++;
                            }
                            #endregion
                            #region set flight fare

                            Core.Flight.Fare fare = new Core.Flight.Fare()
                            {
                                BaseFare = Itin.Fare.BaseFare,
                                Tax = Itin.Fare.Tax,
                                Currency = Itin.Fare.Currency,
                                Markup = 0,
                                PublishedFare = Itin.Fare.PublishedFare,
                                NetFare = Itin.Fare.PublishedFare,
                                AdditionalTxnFeeOfrd = Itin.Fare.AdditionalTxnFeeOfrd,
                                AdditionalTxnFeePub = Itin.Fare.AdditionalTxnFeePub,
                                Discount = Itin.Fare.Discount,
                                OfferedFare = Itin.Fare.OfferedFare,
                                OtherCharges = Itin.Fare.OtherCharges,
                                ServiceFee = Itin.Fare.ServiceFee,
                                TdsOnCommission = Itin.Fare.TdsOnCommission,
                                CommissionEarned = Itin.Fare.CommissionEarned,
                                TdsOnIncentive = Itin.Fare.TdsOnIncentive,
                                TdsOnPLB = Itin.Fare.TdsOnPLB,
                                YQTax = Itin.Fare.YQTax,
                                FareType = getFareType(Itin.FareClassification != null ? Itin.FareClassification.Type : ""),
                                mojoFareType = Core.FlightUtility.GetFmFareType(Itin.FareClassification != null ? Itin.FareClassification.Type : "", result.valCarrier, GdsType.Tbo),
                                // FareType = getFareType(Itin.FareClassification != null ? Itin.FareClassification.Type : ""),
                                //  Itin.FareClassification != null ? Itin.FareClassification.Type : ""
                                cabinType = result.cabinClass,
                                pLBEarned=Itin.Fare.PLBEarned,
                                gdsType = GdsType.Tbo,
                                tboResultIndex = Itin.ResultIndex,
                                SeatAvailable = seatAvail,
                                //tboGroupID= groupID
                            };
                            if (fare.mojoFareType == MojoFareType.None || fare.mojoFareType == MojoFareType.Unknown)
                            {
                                LogCreater.CreateLogFile(Itin.FareClassification != null ? Itin.FareClassification.Type : "" + "~" + result.valCarrier, "Log\\FareType", "tbo" + DateTime.Today.ToString("ddMMyyy") + ".txt");
                            }

                            fare.fareBreakdown = new List<Core.Flight.FareBreakdown>();
                            Core.Flight.FareBreakdown adtFare = new Core.Flight.FareBreakdown();
                            var adtBreakDown = Itin.FareBreakdown.Where(k => k.PassengerType == "1").ToList();
                            adtFare.BaseFare = adtBreakDown[0].BaseFare;
                            adtFare.Tax = adtBreakDown[0].Tax;
                            adtFare.AdditionalTxnFeeOfrd = adtBreakDown[0].AdditionalTxnFeeOfrd;
                            adtFare.AdditionalTxnFeePub = adtBreakDown[0].AdditionalTxnFeePub;
                            adtFare.PGCharge = adtBreakDown[0].PGCharge;
                            adtFare.YQTax = adtBreakDown[0].YQTax;
                            adtFare.PassengerType = Core.PassengerType.Adult;
                            fare.fareBreakdown.Add(adtFare);
                            if (request.child > 0)
                            {
                                Core.Flight.FareBreakdown chdFare = new Core.Flight.FareBreakdown();
                                var chdBreakDown = Itin.FareBreakdown.Where(k => k.PassengerType == "2").ToList();
                                chdFare.BaseFare = chdBreakDown[0].BaseFare;
                                chdFare.Tax = chdBreakDown[0].Tax;
                                chdFare.AdditionalTxnFeeOfrd = chdBreakDown[0].AdditionalTxnFeeOfrd;
                                chdFare.AdditionalTxnFeePub = chdBreakDown[0].AdditionalTxnFeePub;
                                chdFare.PGCharge = chdBreakDown[0].PGCharge;
                                chdFare.YQTax = chdBreakDown[0].YQTax;
                                chdFare.PassengerType = Core.PassengerType.Child;
                                fare.fareBreakdown.Add(chdFare);
                            }
                            if (request.infants > 0)
                            {
                                Core.Flight.FareBreakdown infFare = new Core.Flight.FareBreakdown();
                                var infBreakDown = Itin.FareBreakdown.Where(k => k.PassengerType == "3").ToList();
                                infFare.BaseFare = infBreakDown[0].BaseFare;
                                infFare.Tax = infBreakDown[0].Tax;
                                infFare.AdditionalTxnFeeOfrd = infBreakDown[0].AdditionalTxnFeeOfrd;
                                infFare.AdditionalTxnFeePub = infBreakDown[0].AdditionalTxnFeePub;
                                infFare.PGCharge = infBreakDown[0].PGCharge;
                                infFare.YQTax = infBreakDown[0].YQTax;
                                infFare.PassengerType = Core.PassengerType.Infant;
                                fare.fareBreakdown.Add(infFare);
                            }
                            if (fare.PublishedFare>(fare.BaseFare + fare.Tax + fare.OtherCharges + fare.ServiceFee))
                            {
                                fare.Tax += (fare.PublishedFare - (fare.BaseFare + fare.Tax + fare.OtherCharges + fare.ServiceFee));
                            }
                               
                            fare.grandTotal = fare.PublishedFare + fare.Markup - (fare.CommissionEarned+fare.pLBEarned);
                            if (request.cabinType == fare.cabinType)
                            {
                                #region BlockAirlines
                                if (Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.Tbo) &&
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
                                if (request.segment.Count > 1 && fare.mojoFareType == MojoFareType.Promo && (request.sourceMedia == "1015" || request.sourceMedia == "1013"))
                                {
                                    fare.isBlock = true;
                                }

                                if (request.cabinType==CabinType.Business && (fare.mojoFareType == MojoFareType.SeriesFareWithoutPNR|| fare.mojoFareType == MojoFareType.SeriesFareWithPNR))
                                {
                                    fare.isBlock = true;
                                }
                                //if (request.sourceMedia == "1037" && (fare.mojoFareType == MojoFareType.SeriesFareWithoutPNR || fare.mojoFareType == MojoFareType.SeriesFareWithPNR))
                                //{
                                //    fare.isBlock = true;
                                //}


                                result.FareList.Add(fare);

                                #endregion
                            }
                            #endregion
                            //if (!response.listGroupID.Exists(k => k == groupID))
                            //{
                            //    response.listGroupID.Add(groupID);
                            //}
                            listFlightResult.Add(result);
                        }
                    }
                    itinCtr++;
                    response.Results.Add(listFlightResult);
                }
                //string kk = Newtonsoft.Json.JsonConvert.SerializeObject(fc);
            }
           
        }

        //public void getResult(ref Core.Flight.FlightSearchRequest request, ref TboClass.FlightResponse fsr, ref Core.Flight.FlightSearchResponse response)
        //{
        //    if (fsr.Response != null && fsr.Response.Results != null && fsr.Response.Results.Count > 0)
        //    {
        //        response.TraceId = fsr.Response.TraceId;
        //        int itinCtr = 0;
        //        //List<TboClass.fareClassification> fc = new List<TboClass.fareClassification>();
        //        foreach (List<TboClass.Itinerary> listItin in fsr.Response.Results)
        //        {
        //            List<Core.Flight.FlightResult> listFlightResult = new List<Core.Flight.FlightResult>();
        //            foreach (TboClass.Itinerary Itin in listItin)
        //            {
        //                //if (!fc.Exists(k => k.Type == Itin.FareClassification.Type))
        //                //{
        //                //    fc.Add(new TboClass.fareClassification() { Type = Itin.FareClassification.Type, Color = Itin.FareClassification.Color });
        //                //}
        //                Core.Flight.FlightResult result = new Core.Flight.FlightResult()
        //                {
        //                    AirlineRemark = Itin.AirlineRemark,
        //                    Fare = new Core.Flight.Fare(),
        //                    IsLCC = Itin.IsLCC,
        //                    IsRefundable = Itin.IsRefundable,
        //                    LastTicketDate = Itin.LastTicketDate,
        //                    ResultIndex = Itin.ResultIndex,
        //                    FlightSegments = new List<Core.Flight.FlightSegment>(),
        //                    Source = Itin.Source,
        //                    TicketAdvisory = Itin.TicketAdvisory,
        //                    //ValidatingAirline = Itin.ValidatingAirline,
        //                    cabinClass = request.cabinType,
        //                    gdsType = Core.GdsType.Tbo,
        //                    valCarrier = Itin.ValidatingAirline,
        //                    Color = Itin.FareClassification.Color,
        //                    //fareType = getFareType(Itin.FareClassification.Type),
        //                    groupID = "Tbo" + (itinCtr == 0 ? "OB" : "IB"),
        //                    ffFareType = getFareType(Itin.FareClassification.Type),
        //                };
        //                if (result.Color == "RosyBrown")
        //                {
        //                }
        //                #region set flight segment
        //                int segCtr = 0;
        //                string Airline = string.Empty;
        //                foreach (List<TboClass.FlightSegment> fseg in Itin.Segments)
        //                {
        //                    Core.Flight.FlightSegment fs = new Core.Flight.FlightSegment() { Segments = new List<Core.Flight.Segment>(), Duration = 0, stop = 0, LayoverTime = 0, SegName = (segCtr == 0 && itinCtr == 0 ? "Depart" : "Return") };
        //                    int layCtr = 1;
        //                    foreach (TboClass.FlightSegment seg in fseg)
        //                    {
        //                        Core.Flight.Segment segment = new Core.Flight.Segment()
        //                        {
        //                            Airline = seg.Airline.AirlineCode,
        //                            ArrTime = seg.Destination.ArrTime,
        //                            DepTime = seg.Origin.DepTime,
        //                            Origin = seg.Origin.Airport.AirportCode,
        //                            Destination = seg.Destination.Airport.AirportCode,
        //                            Duration = seg.Duration,
        //                            FareClass = seg.Airline.FareClass,
        //                            FlightNumber = seg.Airline.FlightNumber,
        //                            FromTerminal = seg.Origin.Airport.Terminal,
        //                            ToTerminal = seg.Destination.Airport.Terminal,
        //                            IsETicketEligible = seg.IsETicketEligible,
        //                            OperatingCarrier = seg.Airline.OperatingCarrier,
        //                            SegmentIndicator = seg.SegmentIndicator,
        //                            equipmentType = seg.Craft,
        //                            CabinClass = request.cabinType,
        //                        };
        //                        #region LayOverTime
        //                        if (fseg.Count > layCtr)
        //                        {
        //                            TimeSpan ts = fseg[layCtr].Origin.DepTime - segment.ArrTime;
        //                            if (ts.Hours > 0 || ts.Minutes > 0)
        //                            {
        //                                if (ts.Hours > 0 && ts.Minutes > 0)
        //                                {
        //                                    segment.layOverTime = (ts.Hours * 60) + ts.Minutes;
        //                                }
        //                                else if (ts.Hours > 0)
        //                                {
        //                                    segment.layOverTime = ts.Hours * 60;
        //                                }
        //                                else if (ts.Minutes > 0)
        //                                {
        //                                    segment.layOverTime = ts.Minutes;
        //                                }
        //                            }
        //                            else
        //                            {
        //                                segment.layOverTime = 0;
        //                            }
        //                            fs.LayoverTime += segment.layOverTime;
        //                        }
        //                        layCtr++;
        //                        #endregion
        //                        result.groupID += "" + segment.Airline + segment.FlightNumber + "_" + segment.DepTime.ToString("ddMM");
        //                        fs.stop++;
        //                        fs.Duration += segment.Duration;
        //                        if (response.airline.Where(o => o.code.Equals(segment.Airline, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
        //                        {
        //                            response.airline.Add(Core.FlightUtility.GetAirline(segment.Airline));
        //                        }
        //                        if (Airline.IndexOf(segment.Airline, StringComparison.OrdinalIgnoreCase) == -1)
        //                        {
        //                            Airline = string.IsNullOrEmpty(Airline) ? segment.Airline : ("," + segment.Airline);
        //                        }
        //                        if (response.airline.Where(o => o.code.Equals(segment.OperatingCarrier, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
        //                        {
        //                            response.airline.Add(Core.FlightUtility.GetAirline(segment.OperatingCarrier));
        //                        }
        //                        if (response.airport.Where(o => o.airportCode.Equals(segment.Origin, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
        //                        {
        //                            response.airport.Add(Core.FlightUtility.GetAirport(segment.Origin));
        //                        }
        //                        if (response.airport.Where(o => o.airportCode.Equals(segment.Destination, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
        //                        {
        //                            response.airport.Add(Core.FlightUtility.GetAirport(segment.Destination));
        //                        }
        //                        if (response.aircraftDetail.Where(o => o.aircraftCode.Equals(segment.equipmentType, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
        //                        {
        //                            response.aircraftDetail.Add(Core.FlightUtility.GetAircraftDetail(segment.equipmentType));
        //                        }
        //                        fs.Segments.Add(segment);
        //                    }
        //                    result.FlightSegments.Add(fs);
        //                    segCtr++;
        //                }
        //                #endregion
        //                #region set flight fare
        //                result.Fare.BaseFare = Itin.Fare.BaseFare;
        //                result.Fare.Tax = Itin.Fare.Tax;
        //                result.Fare.Currency = Itin.Fare.Currency;
        //                result.Fare.Markup = 0;
        //                result.Fare.PublishedFare = Itin.Fare.PublishedFare;
        //                result.Fare.AdditionalTxnFeeOfrd = Itin.Fare.AdditionalTxnFeeOfrd;
        //                result.Fare.AdditionalTxnFeePub = Itin.Fare.AdditionalTxnFeePub;
        //                result.Fare.Discount = Itin.Fare.Discount;
        //                result.Fare.OfferedFare = Itin.Fare.OfferedFare;
        //                result.Fare.OtherCharges = Itin.Fare.OtherCharges;
        //                result.Fare.ServiceFee = Itin.Fare.ServiceFee;
        //                result.Fare.TdsOnCommission = Itin.Fare.TdsOnCommission;
        //                result.Fare.CommissionEarned = Itin.Fare.CommissionEarned;
        //                result.Fare.TdsOnIncentive = Itin.Fare.TdsOnIncentive;
        //                result.Fare.TdsOnPLB = Itin.Fare.TdsOnPLB;
        //                result.Fare.YQTax = Itin.Fare.YQTax;
        //                result.Fare.fareBreakdown = new List<Core.Flight.FareBreakdown>();
        //                Core.Flight.FareBreakdown adtFare = new Core.Flight.FareBreakdown();
        //                var adtBreakDown = Itin.FareBreakdown.Where(k => k.PassengerType == "1").ToList();
        //                adtFare.BaseFare = adtBreakDown[0].BaseFare;
        //                adtFare.Tax = adtBreakDown[0].Tax;
        //                adtFare.AdditionalTxnFeeOfrd = adtBreakDown[0].AdditionalTxnFeeOfrd;
        //                adtFare.AdditionalTxnFeePub = adtBreakDown[0].AdditionalTxnFeePub;
        //                adtFare.PGCharge = adtBreakDown[0].PGCharge;
        //                adtFare.YQTax = adtBreakDown[0].YQTax;
        //                adtFare.PassengerType = Core.PassengerType.Adult;
        //                result.Fare.fareBreakdown.Add(adtFare);
        //                if (request.child > 0)
        //                {
        //                    Core.Flight.FareBreakdown chdFare = new Core.Flight.FareBreakdown();
        //                    var chdBreakDown = Itin.FareBreakdown.Where(k => k.PassengerType == "2").ToList();
        //                    chdFare.BaseFare = chdBreakDown[0].BaseFare;
        //                    chdFare.Tax = chdBreakDown[0].Tax;
        //                    chdFare.AdditionalTxnFeeOfrd = chdBreakDown[0].AdditionalTxnFeeOfrd;
        //                    chdFare.AdditionalTxnFeePub = chdBreakDown[0].AdditionalTxnFeePub;
        //                    chdFare.PGCharge = chdBreakDown[0].PGCharge;
        //                    chdFare.YQTax = chdBreakDown[0].YQTax;
        //                    chdFare.PassengerType = Core.PassengerType.Child;
        //                    result.Fare.fareBreakdown.Add(chdFare);
        //                }
        //                if (request.infants > 0)
        //                {
        //                    Core.Flight.FareBreakdown infFare = new Core.Flight.FareBreakdown();
        //                    var infBreakDown = Itin.FareBreakdown.Where(k => k.PassengerType == "3").ToList();
        //                    infFare.BaseFare = infBreakDown[0].BaseFare;
        //                    infFare.Tax = infBreakDown[0].Tax;
        //                    infFare.AdditionalTxnFeeOfrd = infBreakDown[0].AdditionalTxnFeeOfrd;
        //                    infFare.AdditionalTxnFeePub = infBreakDown[0].AdditionalTxnFeePub;
        //                    infFare.PGCharge = infBreakDown[0].PGCharge;
        //                    infFare.YQTax = infBreakDown[0].YQTax;
        //                    infFare.PassengerType = Core.PassengerType.Infant;
        //                    result.Fare.fareBreakdown.Add(infFare);
        //                }
        //                result.Fare.grandTotal = result.Fare.PublishedFare + result.Fare.Markup - result.Fare.CommissionEarned;
        //                //result.Fare.showGrandTotal = result.Fare.grandTotal;
        //                #endregion
        //                if (!response.listGroupID.Exists(k => k == result.groupID))
        //                {
        //                    response.listGroupID.Add(result.groupID);
        //                }
        //                listFlightResult.Add(result);
        //            }
        //            itinCtr++;
        //            response.Results.Add(listFlightResult);
        //        }
        //        //string kk = Newtonsoft.Json.JsonConvert.SerializeObject(fc);
        //    }
        //    #region set CouponFareFaimly
        //    if (response.listGroupID.Count > 0 && response.Results != null && response.Results.Count > 0 && response.Results[0].Count > 0)
        //    {
        //        foreach (var groupID in response.listGroupID.Where(k => k.StartsWith("TboOB", StringComparison.OrdinalIgnoreCase)))
        //        {
        //            Core.Flight.FlightResult faimlyResult = response.Results[0].Where(k => k.groupID.Equals(groupID, StringComparison.OrdinalIgnoreCase) /*&& k.fareType != Core.FareType.Publish*/).ToList().OrderByDescending(k => k.Fare.grandTotal).ToList().LastOrDefault();
        //            if (faimlyResult != null)
        //            {
        //                foreach (var item in response.Results[0].Where(k => k.groupID.Equals(groupID, StringComparison.OrdinalIgnoreCase) /*&& k.fareType == Core.FareType.Publish*/).ToList())
        //                {
        //                    item.Fare.ffDiscount = item.Fare.grandTotal - faimlyResult.Fare.grandTotal;
        //                    if (item.Fare.ffDiscount > 0)
        //                    {
        //                        item.isPreCuponAvailable = true;
        //                        item.ffResultIndex = faimlyResult.ResultIndex;
        //                        //item.ffFareType = faimlyResult.fareType;
        //                        //item.Fare.showGrandTotal = item.Fare.grandTotal - item.Fare.ffDiscount;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    if (response.listGroupID.Count > 0 && response.Results != null && response.Results.Count > 1 && response.Results[1].Count > 0)
        //    {
        //        foreach (var groupID in response.listGroupID.Where(k => k.StartsWith("TboIB", StringComparison.OrdinalIgnoreCase)))
        //        {
        //            Core.Flight.FlightResult faimlyResult = response.Results[1].Where(k => k.groupID.Equals(groupID, StringComparison.OrdinalIgnoreCase) /*&& k.fareType != Core.FareType.Publish*/).ToList().OrderByDescending(k => k.Fare.grandTotal).ToList().LastOrDefault();
        //            if (faimlyResult != null)
        //            {
        //                foreach (var item in response.Results[1].Where(k => k.groupID.Equals(groupID, StringComparison.OrdinalIgnoreCase) /*&& k.fareType == Core.FareType.Publish*/).ToList())
        //                {
        //                    item.Fare.ffDiscount = item.Fare.grandTotal - faimlyResult.Fare.grandTotal;
        //                    if (item.Fare.ffDiscount > 0)
        //                    {
        //                        item.isPreCuponAvailable = true;
        //                        item.ffResultIndex = faimlyResult.ResultIndex;
        //                        // item.ffFareType = faimlyResult.fareType;
        //                        //item.Fare.showGrandTotal = item.Fare.grandTotal - item.Fare.ffDiscount;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    #endregion
        //}

        //public Core.Flight.FlightSearchResponse getResultDummy(ref Core.Flight.FlightSearchRequest request, string strResponse)
        //{
        //    DateTime depDate = Convert.ToDateTime("2020-06-14");
        //    DateTime retDate = Convert.ToDateTime("2020-06-20");
        //    int depDiff = (request.segment[0].travelDate - depDate).Days;
        //    int retDiff = 0;
        //    if (request.segment.Count > 1)
        //    {
        //        retDiff = (request.segment[1].travelDate - retDate).Days;
        //    }
        //    TboClass.FlightResponse fsr = Newtonsoft.Json.JsonConvert.DeserializeObject<TboClass.FlightResponse>(strResponse);
        //    Core.Flight.FlightSearchResponse _flightSearchResponse = new Core.Flight.FlightSearchResponse()
        //    {
        //        adults = request.adults,
        //        child = request.child,
        //        infants = request.infants,
        //        aircraftDetail = new List<Core.Flight.AircraftDetail>(),
        //        airline = new List<Core.Flight.Airline>(),
        //        airport = new List<Core.Flight.Airport>(),
        //        response = new Core.ResponseStatus(),
        //        Results = new List<List<Core.Flight.FlightResult>>()
        //    };
        //    if (fsr.Response != null && fsr.Response.Results != null && fsr.Response.Results.Count > 0)
        //    {
        //        _flightSearchResponse.TraceId = fsr.Response.TraceId;
        //        int itinCtr = 0;
        //        foreach (List<TboClass.Itinerary> listItin in fsr.Response.Results)
        //        {
        //            List<Core.Flight.FlightResult> listFlightResult = new List<Core.Flight.FlightResult>();

        //            foreach (TboClass.Itinerary Itin in listItin)
        //            {
        //                Core.Flight.FlightResult result = new Core.Flight.FlightResult()
        //                {
        //                    AirlineRemark = Itin.AirlineRemark,
        //                    Fare = new Core.Flight.Fare(),
        //                    IsLCC = Itin.IsLCC,
        //                    IsRefundable = Itin.IsRefundable,
        //                    LastTicketDate = Itin.LastTicketDate,
        //                    //ResultIndex = Itin.ResultIndex,
        //                    FlightSegments = new List<Core.Flight.FlightSegment>(),
        //                    Source = Itin.Source,
        //                    TicketAdvisory = Itin.TicketAdvisory,
        //                    //ValidatingAirline = Itin.ValidatingAirline,                           
        //                    cabinClass = request.cabinType,
        //                    gdsType = Core.GdsType.Tbo,
        //                    valCarrier = Itin.ValidatingAirline
        //                };
        //                #region set flight segment
        //                int segCtr = 0;
        //                foreach (List<TboClass.FlightSegment> fseg in Itin.Segments)
        //                {
        //                    Core.Flight.FlightSegment fs = new Core.Flight.FlightSegment() { Segments = new List<Core.Flight.Segment>(), Duration = 0, stop = 0, SegName = (segCtr == 0 && itinCtr == 0 ? "Depart" : "Return") };
        //                    foreach (TboClass.FlightSegment seg in fseg)
        //                    {
        //                        Core.Flight.Segment segment = new Core.Flight.Segment()
        //                        {
        //                            Airline = seg.Airline.AirlineCode,
        //                            ArrTime = segCtr == 0 ? seg.Destination.ArrTime.AddDays(depDiff) : seg.Destination.ArrTime.AddDays(retDiff),
        //                            DepTime = segCtr == 0 ? seg.Origin.DepTime.AddDays(depDiff) : seg.Origin.DepTime.AddDays(retDiff),
        //                            Origin = seg.Origin.Airport.AirportCode,
        //                            Destination = seg.Destination.Airport.AirportCode,
        //                            Duration = seg.Duration,
        //                            FareClass = seg.Airline.FareClass,
        //                            FlightNumber = seg.Airline.FlightNumber,
        //                            FromTerminal = seg.Origin.Airport.Terminal,
        //                            ToTerminal = seg.Destination.Airport.Terminal,
        //                            IsETicketEligible = seg.IsETicketEligible,
        //                            OperatingCarrier = seg.Airline.OperatingCarrier,
        //                            SegmentIndicator = seg.SegmentIndicator,
        //                            equipmentType = seg.Craft,
        //                            CabinClass = request.cabinType,
        //                        };
        //                        fs.stop++;
        //                        fs.Duration += segment.Duration;
        //                        if (_flightSearchResponse.airline.Where(o => o.code.Equals(segment.Airline, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
        //                        {
        //                            _flightSearchResponse.airline.Add(Core.FlightUtility.GetAirline(segment.Airline));
        //                        }
        //                        if (_flightSearchResponse.airline.Where(o => o.code.Equals(segment.OperatingCarrier, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
        //                        {
        //                            _flightSearchResponse.airline.Add(Core.FlightUtility.GetAirline(segment.OperatingCarrier));
        //                        }
        //                        if (_flightSearchResponse.airport.Where(o => o.airportCode.Equals(segment.Origin, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
        //                        {
        //                            _flightSearchResponse.airport.Add(Core.FlightUtility.GetAirport(segment.Origin));
        //                        }
        //                        if (_flightSearchResponse.airport.Where(o => o.airportCode.Equals(segment.Destination, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
        //                        {
        //                            _flightSearchResponse.airport.Add(Core.FlightUtility.GetAirport(segment.Destination));
        //                        }
        //                        if (_flightSearchResponse.aircraftDetail.Where(o => o.aircraftCode.Equals(segment.equipmentType, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
        //                        {
        //                            _flightSearchResponse.aircraftDetail.Add(Core.FlightUtility.GetAircraftDetail(segment.equipmentType));
        //                        }
        //                        fs.Segments.Add(segment);
        //                    }
        //                    result.FlightSegments.Add(fs);
        //                    segCtr++;
        //                }
        //                #endregion
        //                #region set flight fare
        //                result.Fare.BaseFare = Itin.Fare.BaseFare;
        //                result.Fare.Tax = Itin.Fare.Tax;
        //                result.Fare.Currency = Itin.Fare.Currency;
        //                result.Fare.Markup = 0;
        //                result.Fare.PublishedFare = Itin.Fare.PublishedFare;
        //                result.Fare.AdditionalTxnFeeOfrd = Itin.Fare.AdditionalTxnFeeOfrd;
        //                result.Fare.AdditionalTxnFeePub = Itin.Fare.AdditionalTxnFeePub;
        //                result.Fare.Discount = Itin.Fare.Discount;
        //                result.Fare.OfferedFare = Itin.Fare.OfferedFare;
        //                result.Fare.OtherCharges = Itin.Fare.OtherCharges;
        //                result.Fare.ServiceFee = Itin.Fare.ServiceFee;
        //                result.Fare.TdsOnCommission = Itin.Fare.TdsOnCommission;
        //                result.Fare.TdsOnIncentive = Itin.Fare.TdsOnIncentive;
        //                result.Fare.TdsOnPLB = Itin.Fare.TdsOnPLB;
        //                result.Fare.YQTax = Itin.Fare.YQTax;


        //                result.Fare.fareBreakdown = new List<Core.Flight.FareBreakdown>();
        //                Core.Flight.FareBreakdown adtFare = new Core.Flight.FareBreakdown();
        //                var adtBreakDown = Itin.FareBreakdown.Where(k => k.PassengerType == "1").ToList();
        //                adtFare.BaseFare = adtBreakDown[0].BaseFare;
        //                adtFare.Tax = adtBreakDown[0].Tax;
        //                adtFare.AdditionalTxnFeeOfrd = adtBreakDown[0].AdditionalTxnFeeOfrd;
        //                adtFare.AdditionalTxnFeePub = adtBreakDown[0].AdditionalTxnFeePub;
        //                adtFare.PGCharge = adtBreakDown[0].PGCharge;
        //                adtFare.YQTax = adtBreakDown[0].YQTax;


        //                adtFare.Markup = 0;
        //                adtFare.PassengerType = Core.PassengerType.Adult;
        //                result.Fare.fareBreakdown.Add(adtFare);
        //                if (request.child > 0)
        //                {
        //                    Core.Flight.FareBreakdown chdFare = new Core.Flight.FareBreakdown();
        //                    var chdBreakDown = Itin.FareBreakdown.Where(k => k.PassengerType == "2").ToList();
        //                    chdFare.BaseFare = chdBreakDown[0].BaseFare;
        //                    chdFare.Tax = chdBreakDown[0].Tax;
        //                    chdFare.AdditionalTxnFeeOfrd = chdBreakDown[0].AdditionalTxnFeeOfrd;
        //                    chdFare.AdditionalTxnFeePub = chdBreakDown[0].AdditionalTxnFeePub;
        //                    chdFare.PGCharge = chdBreakDown[0].PGCharge;
        //                    chdFare.YQTax = chdBreakDown[0].YQTax;
        //                    chdFare.Markup = 0;
        //                    chdFare.PassengerType = Core.PassengerType.Child;
        //                    result.Fare.fareBreakdown.Add(chdFare);
        //                }
        //                if (request.infants > 0)
        //                {
        //                    Core.Flight.FareBreakdown infFare = new Core.Flight.FareBreakdown();
        //                    var infBreakDown = Itin.FareBreakdown.Where(k => k.PassengerType == "3").ToList();
        //                    infFare.BaseFare = infBreakDown[0].BaseFare;
        //                    infFare.Tax = infBreakDown[0].Tax;
        //                    infFare.AdditionalTxnFeeOfrd = infBreakDown[0].AdditionalTxnFeeOfrd;
        //                    infFare.AdditionalTxnFeePub = infBreakDown[0].AdditionalTxnFeePub;
        //                    infFare.PGCharge = infBreakDown[0].PGCharge;
        //                    infFare.YQTax = infBreakDown[0].YQTax;
        //                    infFare.Markup = 0;
        //                    infFare.PassengerType = Core.PassengerType.Infant;
        //                    result.Fare.fareBreakdown.Add(infFare);
        //                }
        //                #endregion

        //                listFlightResult.Add(result);
        //            }
        //            itinCtr++;
        //            _flightSearchResponse.Results.Add(listFlightResult);
        //        }
        //    }
        //    return _flightSearchResponse;
        //}
        public void getFareQuoteResponse(ref Core.Flight.PriceVerificationRequest request, ref TboClass.FareQuoteResponse fqr, ref Core.Flight.FareQuoteResponse response, int ctr)
        {
            try
            {
                response.flightResult.Add(request.flightResult[ctr]);
                if (fqr.Response.IsPriceChanged)
                {
                    response.isFareChange = true;
                    response.VerifiedTotalPrice = fqr.Response.Results.Fare.PublishedFare;
                }
                if (fqr.Response.ResponseStatus == 1)
                {
                    response.VerifiedTotalPrice = fqr.Response.Results.Fare.PublishedFare;
                    response.IsGSTMandatory = fqr.Response.Results.IsGSTMandatory;
                    decimal diff = fqr.Response.Results.Fare.PublishedFare - request.flightResult[ctr].Fare.PublishedFare;

                    if (diff > 0)
                    {
                        response.isFareChange = true;
                        response.fareIncreaseAmount += diff;
                    }
                }

                else
                {
                    response.responseStatus.status = Core.TransactionStatus.Error;
                    response.responseStatus.message = fqr.Response.Error.ErrorCode + ":" + fqr.Response.Error.ErrorMessage;
                    response.ErrorCode = fqr.Response.Error.ErrorCode;
                    response.isRunFareQuoteFalse = true;
                }
            }
            catch
            {
                response.isRunFareQuoteFalse = true;
            }
        }
        public void getCalendarFare(ref Core.Flight.FlightSearchRequest request, ref TboClass.CalendarFareResponse cfr, ref Core.Flight.CalendarFareResponse response)
        {
            response.TraceId = cfr.Response.TraceId;

            foreach (var item in cfr.Response.SearchResults)
            {
                response.SearchResults.Add(new Core.Flight.SearchResult()
                {
                    AirlineCode = item.AirlineCode,
                    AirlineName = item.AirlineName,
                    BaseFare = item.BaseFare,
                    DepartureDate = item.DepartureDate,
                    Fare = item.Fare,
                    Tax = item.Tax,
                    FuelSurcharge = item.FuelSurcharge,
                    IsLowestFareOfMonth = item.IsLowestFareOfMonth,
                    OtherCharges = item.OtherCharges,
                });
            }


        }

        public void getFareRuleResponse(string strResponse, ref List<Core.Flight.FareRuleResponses> response)
        {
            try
            {
                Core.Flight.FareRuleResponses fareRuleResponses = new Core.Flight.FareRuleResponses() { FareRules = new List<Core.Flight.FareRule>() };
                TboClass.FareRuleResponse _Response = Newtonsoft.Json.JsonConvert.DeserializeObject<TboClass.FareRuleResponse>(strResponse);
                if (_Response.Response.ResponseStatus == 1 && _Response.Response.FareRules.Count > 0)
                {
                    foreach (var item in _Response.Response.FareRules)
                    {
                        Core.Flight.FareRule rule = new Core.Flight.FareRule();
                        rule.Airline = item.Airline;
                        rule.Origin = item.Origin;
                        rule.Destination = item.Destination;
                        rule.FareBasisCode = item.FareBasisCode;
                        rule.FareRestriction = item.FareRestriction;
                        rule.FareRuleDetail = item.FareRuleDetail;
                        fareRuleResponses.FareRules.Add(rule);
                    }
                }
                response.Add(fareRuleResponses);
            }
            catch
            {

            }
        }

        public Core.FareType getFareType(string type)
        {
            Core.FareType ft = Core.FareType.NONE;
            if (!string.IsNullOrEmpty(type))
            {
                switch (type.ToLower())
                {
                    case "publish": ft = Core.FareType.PUBLISH; break;
                    case "coupon": ft = Core.FareType.PROMO; break;
                    case "corporate": ft = Core.FareType.CORPORATE; break;
                    case "instantpur": ft = Core.FareType.INSTANTPUR; break;
                    case "sme.crpcon": ft = Core.FareType.CORPORATE; break;
                    case "inst_seriespur": ft = Core.FareType.INST_SERIESPUR; break;
                    case "saver": ft = Core.FareType.PUBLISH; break;
                    case "flexi": ft = Core.FareType.CORPORATE; break;
                    case "promo": ft = Core.FareType.PROMO; break;
                    case "tactical": ft = Core.FareType.TACTICAL; break;
                    case "inst_seriespurpf2": ft = Core.FareType.INSTANTPUR; break;
                    case "sme+gst": ft = Core.FareType.CORPORATE; break;
                    case "private": ft = Core.FareType.PRIVATE; break;
                    case "cluster": ft = Core.FareType.CLUSTER; break;
                    case "super6e": ft = Core.FareType.PUBLISH; break;
                    case "sme": ft = Core.FareType.CORPORATE; break;
                    case "sale": ft = Core.FareType.PUBLISH; break;
                    case "Cluster/TBF": ft = Core.FareType.CLUSTER; break;
                    case "ndc": ft = Core.FareType.NDC; break;
                    default: ft = Core.FareType.NONE; break;
                }
            }
            if (ft == Core.FareType.NONE)
            {
                LogCreater.CreateLogFile(type + Environment.NewLine, "Log\\TBO\\error", DateTime.Today.ToString("ddMMyyy") + ".txt");
            }
            return ft;
        }

        //private void saveFareType(int FMFareType, string Airline, string FareType, int Provider)
        //{
        //    var save = Task.Run(async () =>
        //    {
        //        await new DAL.Deal.FareType().SetFareType(FMFareType, Airline, FareType, Provider);
        //    });
        //}
    }
}
