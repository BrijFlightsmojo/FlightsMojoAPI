using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Travelopedia
{
    public class TravelopediaResponseMapping
    {
        //public void getResult(ref Core.Flight.FlightSearchRequest request, ref TravelogyClass.TravelogyFlightSearchResponse fsr, ref Core.Flight.FlightSearchResponse response)
        //{
        //    int ctrError = 0;
        //    try
        //    {
        //        if (request.segment.Count >= 1)
        //        {
        //            if (fsr.TripDetails != null && fsr.TripDetails.Count > 0 && fsr.TripDetails[0] != null && fsr.TripDetails[0].Flights != null && fsr.TripDetails[0].Flights.Count > 0)
        //            {
        //                response.tgy_Search_Key = fsr.Search_Key;
        //                foreach (var totTrip in fsr.TripDetails)
        //                {

        //                    int Ctr = 0;
        //                    List<Core.Flight.FlightResult> listFlightResult = new List<Core.Flight.FlightResult>();
        //                    foreach (ServicesHub.Travelogy.TravelogyClass.Flight flight in totTrip.Flights)
        //                    {
        //                        if (ctrError++ == 12)
        //                        {

        //                        }
        //                        Core.Flight.FlightResult result = new Core.Flight.FlightResult()
        //                        {
        //                            FareList = new List<Core.Flight.Fare>(),
        //                            IsLCC = flight.IsLCC,
        //                            ResultIndex = "TGY" + Ctr++,
        //                            FlightSegments = new List<Core.Flight.FlightSegment>(),
        //                            cabinClass = request.cabinType,
        //                            gdsType = Core.GdsType.Travelogy,
        //                            valCarrier = flight.Airline_Code,
        //                            ResultCombination = "TGY",
        //                            Tgy_Flight_Id = flight.Flight_Id,
        //                            tgy_Flight_Key = flight.Flight_Key,
        //                            Tgy_Flight_No = flight.Flight_Numbers,
        //                        };
        //                        #region set Segment

        //                        Core.Flight.FlightSegment fs = new Core.Flight.FlightSegment()
        //                        {
        //                            Segments = new List<Core.Flight.Segment>(),
        //                            SegName = result.FlightSegments.Count == 0 ? "Depart" : "Return",
        //                            Duration = 0,
        //                            LayoverTime = 0
        //                        };
        //                        int segCtr = 0;
        //                        foreach (var tgySeg in flight.Segments)
        //                        {
        //                            Core.Flight.Segment seg = new Core.Flight.Segment()
        //                            {
        //                                Airline = tgySeg.Airline_Code,
        //                                FlightNumber = tgySeg.Flight_Number,
        //                                Origin = tgySeg.Origin,
        //                                Destination = tgySeg.Destination,
        //                                DepTime = tgySeg.Departure_DateTime,
        //                                ArrTime = tgySeg.Arrival_DateTime,
        //                                CabinClass = request.cabinType,
        //                                Duration = getDuration(tgySeg.Duration),
        //                                //FareClass = Itin.totalPriceList[0].fd.ADULT.cB,
        //                                FromTerminal = tgySeg.Origin_Terminal,
        //                                ToTerminal = tgySeg.Destination_Terminal,
        //                                IsETicketEligible = true,
        //                                layOverTime = 0,
        //                                OperatingCarrier = tgySeg.Airline_Code,
        //                                id = tgySeg.Segment_Id.ToString()
        //                            };
        //                            result.ResultCombination += (seg.Airline + seg.FlightNumber);
        //                            #region LayOverTime
        //                            if (segCtr > 0)
        //                            {
        //                                TimeSpan ts = seg.DepTime - fs.Segments[segCtr - 1].ArrTime;
        //                                fs.Segments[segCtr - 1].layOverTime = Convert.ToInt32(ts.TotalMinutes);
        //                                fs.LayoverTime += fs.Segments[segCtr - 1].layOverTime;
        //                            }
        //                            #endregion
        //                            fs.Duration += seg.Duration;
        //                            fs.stop++;
        //                            fs.Segments.Add(seg);
        //                            segCtr++;
        //                        }
        //                        result.FlightSegments.Add(fs);
        //                        #endregion
        //                        #region setAmount
        //                        foreach (var tpl in flight.Fares)
        //                        {
        //                            Core.Flight.Fare fare = new Core.Flight.Fare()
        //                            {
        //                                SeatAvailable = tpl.Seats_Available,
        //                                //   FareType = getFareType(tpl.FareType),
        //                                FareType = getFareTypestr(tpl.FareDetails[0].FareClasses[0].Class_Desc),
        //                                Tgy_FareID = tpl.Fare_Id,
        //                                fareBreakdown = new List<Core.Flight.FareBreakdown>(),
        //                                Currency = "INR",
        //                                cabinType = request.cabinType,
        //                                refundType = tpl.Refundable ? Core.RefundType.Refundable : Core.RefundType.NonRefundable,
        //                                bookingClass = tpl.ProductClass,
        //                            };


        //                            if (tpl.FareDetails != null && tpl.FareDetails.Count > 0 && tpl.FareDetails[0].Free_Baggage != null)
        //                            {
        //                                fare.baggageInfo = new Core.Flight.BaggageInfo()
        //                                {
        //                                    checkInBaggage = tpl.FareDetails[0].Free_Baggage.Check_In_Baggage,
        //                                    cabinBaggage = tpl.FareDetails[0].Free_Baggage.Hand_Baggage
        //                                };
        //                            }
        //                            if (request.adults > 0)
        //                            {
        //                                ServicesHub.Travelogy.TravelogyClass.FareDetail fd = tpl.FareDetails.Where(k => k.PAX_Type == 0).FirstOrDefault();
        //                                Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
        //                                {
        //                                    BaseFare = fd.Basic_Amount,
        //                                    Tax = fd.AirportTax_Amount,
        //                                    CommissionEarned = fd.Gross_Commission,
        //                                    YQTax = fd.YQ_Amount,
        //                                    PassengerType = Core.PassengerType.Adult,
        //                                    OtherCharges = fd.Trade_Markup_Amount,
        //                                    ServiceFee = fd.Service_Fee_Amount,
        //                                    AdditionalTxnFeeOfrd = 0,
        //                                    AdditionalTxnFeePub = 0,
        //                                    GST = fd.GST,
        //                                    Markup = 0,
        //                                    PGCharge = 0,
        //                                    TdsOnCommission = fd.TDS,
        //                                    Discount = fd.Promo_Discount,
        //                                };
        //                                fare.BaseFare += (paxFare.BaseFare * request.adults);
        //                                fare.Tax += (paxFare.Tax * request.adults);
        //                                fare.CommissionEarned += (paxFare.CommissionEarned * request.adults);
        //                                fare.YQTax += (paxFare.CommissionEarned * request.adults);
        //                                fare.OtherCharges += (paxFare.OtherCharges * request.adults);
        //                                fare.ServiceFee += (paxFare.ServiceFee * request.adults);
        //                                fare.AdditionalTxnFeeOfrd += (paxFare.AdditionalTxnFeeOfrd * request.adults);
        //                                fare.AdditionalTxnFeePub += (paxFare.AdditionalTxnFeePub * request.adults);
        //                                fare.GST += (paxFare.GST * request.adults);
        //                                fare.Markup += (paxFare.Markup * request.adults);
        //                                fare.PGCharge += (paxFare.PGCharge * request.adults);
        //                                fare.TdsOnCommission += (paxFare.TdsOnCommission * request.adults);
        //                                fare.Discount += (paxFare.Discount * request.adults);
        //                                fare.fareBreakdown.Add(paxFare);
        //                            }
        //                            if (request.child > 0)
        //                            {
        //                                ServicesHub.Travelogy.TravelogyClass.FareDetail fd = tpl.FareDetails.Where(k => k.PAX_Type == 1).FirstOrDefault();
        //                                Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
        //                                {
        //                                    BaseFare = fd.Basic_Amount,
        //                                    Tax = fd.AirportTax_Amount,
        //                                    CommissionEarned = fd.Gross_Commission,
        //                                    YQTax = fd.YQ_Amount,
        //                                    PassengerType = Core.PassengerType.Child,
        //                                    OtherCharges = fd.Trade_Markup_Amount,
        //                                    ServiceFee = fd.Service_Fee_Amount,
        //                                    AdditionalTxnFeeOfrd = 0,
        //                                    AdditionalTxnFeePub = 0,
        //                                    GST = fd.GST,
        //                                    Markup = 0,
        //                                    PGCharge = 0,
        //                                    TdsOnCommission = fd.TDS,
        //                                    Discount = fd.Promo_Discount,
        //                                };
        //                                fare.BaseFare += (paxFare.BaseFare * request.child);
        //                                fare.Tax += (paxFare.Tax * request.child);
        //                                fare.CommissionEarned += (paxFare.CommissionEarned * request.child);
        //                                fare.YQTax += (paxFare.CommissionEarned * request.child);
        //                                fare.OtherCharges += (paxFare.OtherCharges * request.child);
        //                                fare.ServiceFee += (paxFare.ServiceFee * request.child);
        //                                fare.AdditionalTxnFeeOfrd += (paxFare.AdditionalTxnFeeOfrd * request.child);
        //                                fare.AdditionalTxnFeePub += (paxFare.AdditionalTxnFeePub * request.child);
        //                                fare.GST += (paxFare.GST * request.child);
        //                                fare.Markup += (paxFare.Markup * request.child);
        //                                fare.PGCharge += (paxFare.PGCharge * request.child);
        //                                fare.TdsOnCommission += (paxFare.TdsOnCommission * request.child);
        //                                fare.Discount += (paxFare.Discount * request.child);
        //                                fare.fareBreakdown.Add(paxFare);
        //                            }
        //                            if (request.infants > 0)
        //                            {
        //                                ServicesHub.Travelogy.TravelogyClass.FareDetail fd = tpl.FareDetails.Where(k => k.PAX_Type == 2).FirstOrDefault();
        //                                Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
        //                                {
        //                                    BaseFare = fd.Basic_Amount,
        //                                    Tax = fd.AirportTax_Amount,
        //                                    CommissionEarned = fd.Gross_Commission,
        //                                    YQTax = fd.YQ_Amount,
        //                                    PassengerType = Core.PassengerType.Infant,
        //                                    OtherCharges = fd.Trade_Markup_Amount,
        //                                    ServiceFee = fd.Service_Fee_Amount,
        //                                    AdditionalTxnFeeOfrd = 0,
        //                                    AdditionalTxnFeePub = 0,
        //                                    GST = fd.GST,
        //                                    Markup = 0,
        //                                    PGCharge = 0,
        //                                    TdsOnCommission = fd.TDS,
        //                                    Discount = fd.Promo_Discount,
        //                                };
        //                                fare.BaseFare += (paxFare.BaseFare * request.infants);
        //                                fare.Tax += (paxFare.Tax * request.infants);
        //                                fare.CommissionEarned += (paxFare.CommissionEarned * request.infants);
        //                                fare.YQTax += (paxFare.CommissionEarned * request.infants);
        //                                fare.OtherCharges += (paxFare.OtherCharges * request.infants);
        //                                fare.ServiceFee += (paxFare.ServiceFee * request.infants);
        //                                fare.AdditionalTxnFeeOfrd += (paxFare.AdditionalTxnFeeOfrd * request.infants);
        //                                fare.AdditionalTxnFeePub += (paxFare.AdditionalTxnFeePub * request.infants);
        //                                fare.GST += (paxFare.GST * request.infants);
        //                                fare.Markup += (paxFare.Markup * request.infants);
        //                                fare.PGCharge += (paxFare.PGCharge * request.infants);
        //                                fare.TdsOnCommission += (paxFare.TdsOnCommission * request.infants);
        //                                fare.Discount += (paxFare.Discount * request.infants);
        //                                fare.fareBreakdown.Add(paxFare);
        //                            }
        //                            fare.NetFare = fare.grandTotal = fare.BaseFare + fare.Tax + fare.ServiceFee + fare.OtherCharges;
        //                            //fare.grandTotal = fare.grandTotal - fare.CommissionEarned;
        //                            //if (fare.FareType != Core.FareType.OFFER_FARE_WITHOUT_PNR && request.cabinType == fare.cabinType)
        //                            if (request.cabinType == fare.cabinType)
        //                            {
        //                                #region BlockAirlines

        //                                if (result.FlightSegments[0].Segments[0].Airline != "6E" && result.FlightSegments[0].Segments[0].Airline != "G8")/**/
        //                                {
        //                                    //if (fare.FareType != Core.FareType.OFFER_RETURN_FARE_WITHOUT_PNR)
        //                                    //{
        //                                        result.FareList.Add(fare);
        //                                    //}

        //                                    //if ( request.sourceMedia != "1015")/*result.FlightSegments[0].Segments[0].Airline != "SG" &&*/
        //                                    //{
        //                                    //    if (fare.FareType != Core.FareType.OFFER_RETURN_FARE_WITHOUT_PNR)
        //                                    //    {
        //                                    //        result.FareList.Add(fare);
        //                                    //    }
        //                                    //}
        //                                    //else
        //                                    //{
        //                                    //    if (fare.FareType != Core.FareType.OFFER_RETURN_FARE_WITHOUT_PNR && fare.FareType != Core.FareType.OFFER_FARE_WITH_PNR)
        //                                    //    {
        //                                    //        result.FareList.Add(fare);
        //                                    //    }

        //                                    //}
        //                                }

        //                                #endregion
        //                            }
        //                        }
        //                        if (result.FareList.Count > 0 && result.FareList.Exists(k => k.FareType == Core.FareType.PUBLISH))
        //                        {
        //                            result.Fare = result.FareList.OrderBy(k => k.grandTotal).FirstOrDefault();
        //                            listFlightResult.Add(result);
        //                        }
        //                        else
        //                        {
        //                            result.Fare = result.FareList.OrderBy(k => k.grandTotal).FirstOrDefault();
        //                            if (result.Fare != null)
        //                                listFlightResult.Add(result);
        //                        }

        //                        #endregion

        //                        //listFlightResult.Add(result);

        //                    }
        //                    response.Results.Add(listFlightResult);
        //                }
        //            }
        //        }
        //        else
        //        {

        //        }


        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public void getResults(Core.Flight.FlightSearchRequest request, ref TravelopediaClass.TravelopediaFlightSearchResponse fsr, ref Core.Flight.FlightSearchResponseShort response)
        {
            int ctrError = 0;
            try
            {

                if (request.segment.Count >= 1)
                {
                    if (fsr.TripDetails != null && fsr.TripDetails.Count > 0 && fsr.TripDetails[0] != null && fsr.TripDetails[0].Flights != null && fsr.TripDetails[0].Flights.Count > 0)
                    {
                        response.tgy_Search_Key = fsr.Search_Key;
                        int itinCtr = 0;
                        foreach (var totTrip in fsr.TripDetails)
                        {

                            int Ctr = 0;
                            List<Core.Flight.FlightResult> listFlightResult = new List<Core.Flight.FlightResult>();
                            foreach (ServicesHub.Travelopedia.TravelopediaClass.Flight flight in totTrip.Flights)
                            {
                                if (Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.Travelogy) &&
                                    (o.SiteId == request.siteId) && (o.FareType.Count == 0) && o.airline.Contains(flight.Airline_Code) &&
                                    ((o.CountryFrom.Any() && o.CountryFrom.Contains(request.segment[0].orgArp.countryCode)) || o.CountryFrom.Any() == false) &&
                                    ((o.CountryTo.Any() && o.CountryTo.Contains(request.segment[0].destArp.countryCode)) || o.CountryTo.Any() == false) &&
                                    (o.CountryFrom_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                                    (o.CountryTo_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                                    ((o.WeekOfDays.Any() && o.WeekOfDays.Contains((WeekDays)Enum.Parse(typeof(WeekDays), Convert.ToString(DateTime.Today.DayOfWeek)))) || o.WeekOfDays.Any() == false) &&
                                     ((o.AffiliateId.Any() && o.AffiliateId.Contains(request.sourceMedia)) || o.AffiliateId.Any() == false) &&
                                    (o.AffiliateId_Not.Contains(request.sourceMedia) == false)&& (o.device == Device.None || o.device == request.device)).ToList().Count == 0)
                                {
                                    Core.Flight.FlightResult result = new Core.Flight.FlightResult()
                                    {
                                        FareList = new List<Core.Flight.Fare>(),
                                        IsLCC = flight.IsLCC,
                                        ResultIndex =(itinCtr==0?"TGYO": "TGYI") + Ctr++,
                                        FlightSegments = new List<Core.Flight.FlightSegment>(),
                                        cabinClass = request.cabinType,
                                        gdsType = Core.GdsType.Travelopedia,
                                        valCarrier = flight.Airline_Code,
                                        ResultCombination = "",
                                        Tgy_Flight_Id = flight.Flight_Id,
                                        tgy_Flight_Key = flight.Flight_Key,
                                        Tgy_Flight_No = flight.Flight_Numbers,
                                    };
                                    #region set Segment

                                    Core.Flight.FlightSegment fs = new Core.Flight.FlightSegment()
                                    {
                                        Segments = new List<Core.Flight.Segment>(),
                                        SegName = result.FlightSegments.Count == 0 ? "Depart" : "Return",
                                        Duration = 0,
                                        LayoverTime = 0
                                    };
                                    int segCtr = 0;
                                    foreach (var tgySeg in flight.Segments)
                                    {
                                        Core.Flight.Segment seg = new Core.Flight.Segment()
                                        {
                                            Airline = tgySeg.Airline_Code,
                                            FlightNumber = tgySeg.Flight_Number,
                                            Origin = tgySeg.Origin,
                                            Destination = tgySeg.Destination,
                                            DepTime = tgySeg.Departure_DateTime,
                                            ArrTime = tgySeg.Arrival_DateTime,
                                            CabinClass = request.cabinType,
                                            Duration = getDuration(tgySeg.Duration),
                                            //FareClass = Itin.totalPriceList[0].fd.ADULT.cB,
                                            FromTerminal = tgySeg.Origin_Terminal,
                                            ToTerminal = tgySeg.Destination_Terminal,
                                            IsETicketEligible = true,
                                            layOverTime = 0,
                                            OperatingCarrier = tgySeg.Airline_Code,
                                            id = tgySeg.Segment_Id.ToString()
                                        };
                                        result.ResultCombination += (seg.Airline + seg.FlightNumber + seg.DepTime.ToString("ddMMHHmm"));
                                        #region LayOverTime
                                        if (segCtr > 0)
                                        {
                                            TimeSpan ts = seg.DepTime - fs.Segments[segCtr - 1].ArrTime;
                                            fs.Segments[segCtr - 1].layOverTime = Convert.ToInt32(ts.TotalMinutes);
                                            fs.LayoverTime += fs.Segments[segCtr - 1].layOverTime;
                                        }
                                        #endregion
                                        fs.Duration += seg.Duration;
                                        fs.stop++;
                                        fs.Segments.Add(seg);
                                        segCtr++;
                                    }
                                    result.FlightSegments.Add(fs);
                                    #endregion
                                    #region setAmount
                                    foreach (var tpl in flight.Fares)
                                    {

                                        Core.Flight.Fare fare = new Core.Flight.Fare()
                                        {
                                            SeatAvailable = tpl.Seats_Available,
                                            FareType = getFareTypestr(tpl.FareDetails[0].FareClasses[0].Class_Desc),
                                            Tgy_FareID = tpl.Fare_Id,
                                            fareBreakdown = new List<Core.Flight.FareBreakdown>(),
                                            Currency = "INR",
                                            cabinType = request.cabinType,
                                            refundType = tpl.Refundable ? Core.RefundType.Refundable : Core.RefundType.NonRefundable,
                                            bookingClass = tpl.ProductClass,
                                            gdsType = GdsType.Travelopedia,
                                        };
                                        fare.mojoFareType = SMCommanMethod.getMojoFareType(fare.FareType);

                                        if (tpl.FareDetails != null && tpl.FareDetails.Count > 0 && tpl.FareDetails[0].Free_Baggage != null)
                                        {
                                            fare.baggageInfo = new Core.Flight.BaggageInfo()
                                            {
                                                checkInBaggage = tpl.FareDetails[0].Free_Baggage.Check_In_Baggage,
                                                cabinBaggage = tpl.FareDetails[0].Free_Baggage.Hand_Baggage
                                            };
                                        }
                                        if (request.adults > 0)
                                        {
                                            ServicesHub.Travelopedia.TravelopediaClass.FareDetail fd = tpl.FareDetails.Where(k => k.PAX_Type == 0).FirstOrDefault();
                                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
                                            {
                                                BaseFare = fd.Basic_Amount,
                                                Tax = fd.AirportTax_Amount,
                                                CommissionEarned = fd.Gross_Commission,
                                                YQTax = fd.YQ_Amount,
                                                PassengerType = Core.PassengerType.Adult,
                                                OtherCharges = fd.Trade_Markup_Amount,
                                                ServiceFee = fd.Service_Fee_Amount,
                                                AdditionalTxnFeeOfrd = 0,
                                                AdditionalTxnFeePub = 0,
                                                GST = fd.GST,
                                                Markup = 0,
                                                PGCharge = 0,
                                                TdsOnCommission = fd.TDS,
                                                Discount = fd.Promo_Discount,
                                            };

                                            fare.BaseFare += (paxFare.BaseFare * request.adults);
                                            fare.Tax += (paxFare.Tax * request.adults);
                                            fare.CommissionEarned += (paxFare.CommissionEarned * request.adults);
                                            fare.YQTax += (paxFare.CommissionEarned * request.adults);
                                            fare.OtherCharges += (paxFare.OtherCharges * request.adults);
                                            fare.ServiceFee += (paxFare.ServiceFee * request.adults);
                                            fare.AdditionalTxnFeeOfrd += (paxFare.AdditionalTxnFeeOfrd * request.adults);
                                            fare.AdditionalTxnFeePub += (paxFare.AdditionalTxnFeePub * request.adults);
                                            fare.GST += (paxFare.GST * request.adults);
                                            fare.Markup += (paxFare.Markup * request.adults);
                                            fare.PGCharge += (paxFare.PGCharge * request.adults);
                                            fare.TdsOnCommission += (paxFare.TdsOnCommission * request.adults);
                                            fare.Discount += (paxFare.Discount * request.adults);
                                            fare.fareBreakdown.Add(paxFare);
                                        }
                                        if (request.child > 0)
                                        {
                                            ServicesHub.Travelopedia.TravelopediaClass.FareDetail fd = tpl.FareDetails.Where(k => k.PAX_Type == 1).FirstOrDefault();
                                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
                                            {
                                                BaseFare = fd.Basic_Amount,
                                                Tax = fd.AirportTax_Amount,
                                                CommissionEarned = fd.Gross_Commission,
                                                YQTax = fd.YQ_Amount,
                                                PassengerType = Core.PassengerType.Child,
                                                OtherCharges = fd.Trade_Markup_Amount,
                                                ServiceFee = fd.Service_Fee_Amount,
                                                AdditionalTxnFeeOfrd = 0,
                                                AdditionalTxnFeePub = 0,
                                                GST = fd.GST,
                                                Markup = 0,
                                                PGCharge = 0,
                                                TdsOnCommission = fd.TDS,
                                                Discount = fd.Promo_Discount,
                                            };
                                            fare.BaseFare += (paxFare.BaseFare * request.child);
                                            fare.Tax += (paxFare.Tax * request.child);
                                            fare.CommissionEarned += (paxFare.CommissionEarned * request.child);
                                            fare.YQTax += (paxFare.CommissionEarned * request.child);
                                            fare.OtherCharges += (paxFare.OtherCharges * request.child);
                                            fare.ServiceFee += (paxFare.ServiceFee * request.child);
                                            fare.AdditionalTxnFeeOfrd += (paxFare.AdditionalTxnFeeOfrd * request.child);
                                            fare.AdditionalTxnFeePub += (paxFare.AdditionalTxnFeePub * request.child);
                                            fare.GST += (paxFare.GST * request.child);
                                            fare.Markup += (paxFare.Markup * request.child);
                                            fare.PGCharge += (paxFare.PGCharge * request.child);
                                            fare.TdsOnCommission += (paxFare.TdsOnCommission * request.child);
                                            fare.Discount += (paxFare.Discount * request.child);
                                            fare.fareBreakdown.Add(paxFare);
                                        }
                                        if (request.infants > 0)
                                        {
                                            ServicesHub.Travelopedia.TravelopediaClass.FareDetail fd = tpl.FareDetails.Where(k => k.PAX_Type == 2).FirstOrDefault();
                                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
                                            {
                                                BaseFare = fd.Basic_Amount,
                                                Tax = fd.AirportTax_Amount,
                                                CommissionEarned = fd.Gross_Commission,
                                                YQTax = fd.YQ_Amount,
                                                PassengerType = Core.PassengerType.Infant,
                                                OtherCharges = fd.Trade_Markup_Amount,
                                                ServiceFee = fd.Service_Fee_Amount,
                                                AdditionalTxnFeeOfrd = 0,
                                                AdditionalTxnFeePub = 0,
                                                GST = fd.GST,
                                                Markup = 0,
                                                PGCharge = 0,
                                                TdsOnCommission = fd.TDS,
                                                Discount = fd.Promo_Discount,
                                            };
                                            fare.BaseFare += (paxFare.BaseFare * request.infants);
                                            fare.Tax += (paxFare.Tax * request.infants);
                                            fare.CommissionEarned += (paxFare.CommissionEarned * request.infants);
                                            fare.YQTax += (paxFare.CommissionEarned * request.infants);
                                            fare.OtherCharges += (paxFare.OtherCharges * request.infants);
                                            fare.ServiceFee += (paxFare.ServiceFee * request.infants);
                                            fare.AdditionalTxnFeeOfrd += (paxFare.AdditionalTxnFeeOfrd * request.infants);
                                            fare.AdditionalTxnFeePub += (paxFare.AdditionalTxnFeePub * request.infants);
                                            fare.GST += (paxFare.GST * request.infants);
                                            fare.Markup += (paxFare.Markup * request.infants);
                                            fare.PGCharge += (paxFare.PGCharge * request.infants);
                                            fare.TdsOnCommission += (paxFare.TdsOnCommission * request.infants);
                                            fare.Discount += (paxFare.Discount * request.infants);
                                            fare.fareBreakdown.Add(paxFare);
                                        }
                                        fare.NetFare = fare.grandTotal = fare.BaseFare + fare.Tax + fare.ServiceFee + fare.OtherCharges;

                                        if (request.cabinType == fare.cabinType)
                                        {
                                            #region BlockAirlines
                                            if (Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.Travelopedia) &&
                                             (o.SiteId == request.siteId) && (o.FareType.Any() && o.FareType.Contains(fare.mojoFareType)) &&
                                             ((o.airline.Any() && o.airline.Contains(result.valCarrier)) || o.airline.Any() == false) &&
                                             ((o.CountryFrom.Any() && o.CountryFrom.Contains(request.segment[0].orgArp.countryCode)) || o.CountryFrom.Any() == false) &&
                                             ((o.CountryTo.Any() && o.CountryTo.Contains(request.segment[0].destArp.countryCode)) || o.CountryTo.Any() == false) &&
                                             (o.CountryFrom_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                                             (o.CountryTo_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                                             ((o.WeekOfDays.Any() && o.WeekOfDays.Contains((WeekDays)Enum.Parse(typeof(WeekDays), Convert.ToString(DateTime.Today.DayOfWeek)))) || o.WeekOfDays.Any() == false) &&
                                              ((o.AffiliateId.Any() && o.AffiliateId.Contains(request.sourceMedia)) || o.AffiliateId.Any() == false) &&
                                             (o.AffiliateId_Not.Contains(request.sourceMedia) == false)).ToList().Count > 0)
                                            {
                                                fare.isBlock = true;
                                            }
                                            result.FareList.Add(fare);

                                            #endregion
                                        }
                                    }
                                    //if (result.FareList.Count > 0 && result.FareList.Exists(k => k.FareType == Core.FareType.PUBLISH))
                                    //{
                                    //    result.Fare = result.FareList.OrderBy(k => k.grandTotal).FirstOrDefault();
                                    //    listFlightResult.Add(result);
                                    //}
                                    //else
                                    //{
                                    //    result.Fare = result.FareList.OrderBy(k => k.grandTotal).FirstOrDefault();
                                    //    if (result.Fare != null)
                                    //        listFlightResult.Add(result);
                                    //}

                                    #endregion
                                    listFlightResult.Add(result);
                                }
                            }
                            response.Results.Add(listFlightResult);
                            itinCtr++;
                        }
                    }
                }
                else
                {

                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public Core.FareType getFareType(int fType)
        {
            Core.FareType fareType = Core.FareType.NONE;
            switch (fType)
            {//0 - RetailFare / 1 - CouponFare / 2 - CorporateFare / 3 - SMEFare / 4 - RTSPLFare / 5 - DealFare

                case 0: fareType = Core.FareType.PUBLISH; break;
                case 1: fareType = Core.FareType.COUPON; break;
                case 2: fareType = Core.FareType.CORPORATE; break;
                case 3: fareType = Core.FareType.SMECRPCON; break;
                case 4: fareType = Core.FareType.RTSPLFARE; break;
                case 5: fareType = Core.FareType.DEALFARE; break;
            }
            if (fareType == Core.FareType.NONE)
            {
                LogCreater.CreateLogFile(fType + Environment.NewLine, "Log\\Travelogy\\error", DateTime.Today.ToString("ddMMyyy") + ".txt");
            }
            return fareType;
        }


        public Core.FareType getFareTypestr(string strfType)
        {
            if (!string.IsNullOrEmpty(strfType.ToUpper()))
            {
                Core.FareType ftype;
                bool kk = Enum.TryParse(strfType.ToUpper(), out ftype);
                if (kk)
                {
                    return ftype;
                }
                else
                {
                    if (strfType == "Offline Booking (30 Minutes STA)" || strfType == "Offline Request" || strfType == "DEAL FARE (OFFLINE BOOKING)")
                    {
                        return Core.FareType.OFFER_RETURN_FARE_WITHOUT_PNR;
                    }
                    else if (strfType == "Non Refundable (LIVE Booking)	" || strfType == "Non Refundable (Instant)" || strfType == "Special Deal (Instant)"
                        || strfType == "REFUNDABLE AUTO FIXED FARE" || strfType == "Deal Fare (Instant PNR)" || strfType == "DEAL FARE (LIVE PNR)" || strfType == "Non Refundable (Insta PNR)"
                        || strfType == "Series Fare (Instant PNR)" || strfType == "Non Refundable (Instant Booking)" || strfType == "Automatic Series Fare"
                        || strfType == "Non Refundable (Live PNR)" || strfType == "Special Fare" || strfType == "Offer Fare (Live PNR)" || strfType == "SPECIAL"
                        || strfType == "Series Fare"
                        || strfType == "Deal Fare (Instant)" || strfType == "Non Refundable (Live PNR) "
                        || strfType == "H" || strfType == "Y" || strfType == "Z" || strfType == "V" || strfType == "E" || strfType == "Q"
                        || strfType == "T" || strfType == "B" || strfType == "L" || strfType == "M" || strfType == "K"  || strfType == "G"
                        || strfType == "U" || strfType == "W" || strfType == "X" || strfType == "I" || strfType == "D" || strfType == "C" || strfType == "P" || strfType == "O"
                          || strfType == "F" || strfType == "On Request" || strfType == "Non Refundable (LIVE Booking)/t" || strfType == "Super6E"
                        || strfType == "Non Refundable (LIVE PNR)" || strfType == "Non Refundable (LIVE BOOKING)" || strfType == "NON REFUNDABLE (INSTANT PNR)" || strfType == "EE" || strfType == "AUTO FIXED FARE")
                    {
                        return Core.FareType.OFFER_FARE_WITH_PNR;
                    }
                    else if (strfType == "Flexi" || strfType == "FlexiFare" || strfType == "FlexiPlus" || strfType == "PremiumFlex" || strfType == "Premium Flex"
                        || strfType == "EXPRESS_FLEXI" || strfType == "FLX" || strfType == "FLEX" || strfType == "FLY" || strfType == "Express FLEXI 20 Kg"
                        || strfType == "Express FLEXI 30 Kg")
                    {
                        return Core.FareType.FLEXI;
                    }

                    else if (strfType == "PUBLISHED" || strfType == "REGULAR" || strfType == "PUBLISH" || strfType == "Family" || strfType == "Family Fare"
                         || strfType == "Flexible" || strfType == "Super Saver" || strfType == "Super 6E" || strfType == "Go More" || strfType == "RoundTrip"
                         || strfType == "CLUSTER" || strfType == "BASIC" || strfType == "S" || strfType == "R"
                         || strfType == "J" || strfType == "A" || strfType == "N")
                    {
                        return Core.FareType.PUBLISH;
                    }

                    else if (strfType == "Corp Connect" || strfType == "Corporate Select" || strfType == "Economy"
                        || strfType == "Coupon Fare" || strfType == "Corporate Value" || strfType == "Economy Promo")
                    {
                        return Core.FareType.CORPORATE;
                    }
                    else if (strfType == "Promo" || strfType == "CN2 Fare" || strfType == "Plus" || strfType == "SME Fare"
                        || strfType == "SME" || strfType == "Express VALUE 20 Kg" || strfType == "Express VALUE 30 Kg")
                    {
                        return Core.FareType.PROMO;
                    }

                    //LogCreater.CreateLogFile(strfType + Environment.NewLine, "Log\\Travelogy\\error", DateTime.Today.ToString("ddMMyyy") + ".txt");
                    return Core.FareType.OFFER_RETURN_FARE_WITH_PNR;
                }
            }
            else
            {
                LogCreater.CreateLogFile(strfType + Environment.NewLine, "Log\\Travelogy\\error", DateTime.Today.ToString("ddMMyyy") + ".txt");
                return Core.FareType.OFFER_RETURN_FARE_WITH_PNR;
            }
            // return (Core.FareType)Enum.Parse(typeof(Core.FareType), fType, true);

        }


        //public Core.MojoFareType getFareTypestr(string strfType)
        //{
        //    if (!string.IsNullOrEmpty(strfType.ToUpper()))
        //    {
        //        Core.MojoFareType ftype;
        //        bool kk = Enum.TryParse(strfType.ToUpper(), out ftype);
        //        if (kk)
        //        {
        //            return ftype;
        //        }
        //        else
        //        {
        //            if (strfType == "Offline Booking (30 Minutes STA)" || strfType == "Offline Request" || strfType == "DEAL FARE (OFFLINE BOOKING)")
        //            {
        //                return Core.MojoFareType.Series_Fare_without_PNR;
        //            }
        //            else if (strfType == "Non Refundable (LIVE Booking)	" || strfType == "Non Refundable (Instant)" || strfType == "Special Deal (Instant)"
        //                || strfType == "REFUNDABLE AUTO FIXED FARE" || strfType == "Deal Fare (Instant PNR)" || strfType == "DEAL FARE (LIVE PNR)" || strfType == "Non Refundable (Insta PNR)"
        //                || strfType == "Series Fare (Instant PNR)" || strfType == "Non Refundable (Instant Booking)" || strfType == "Automatic Series Fare"
        //                || strfType == "Non Refundable (Live PNR)" || strfType == "Special Fare" || strfType == "Offer Fare (Live PNR)" || strfType == "SPECIAL"
        //                || strfType == "Series Fare" || strfType == "SME Fare"
        //                || strfType == "SME"
        //                || strfType == "Express VALUE 20 Kg" || strfType == "Express VALUE 30 Kg" || strfType == "Deal Fare (Instant)" || strfType == "Non Refundable (Live PNR) "
        //                || strfType == "H" || strfType == "Y" || strfType == "Z" || strfType == "V" || strfType == "E" || strfType == "Q"
        //                || strfType == "T" || strfType == "B" || strfType == "L" || strfType == "M" || strfType == "K" || strfType == "S" || strfType == "G"
        //                || strfType == "N" || strfType == "U" || strfType == "W" || strfType == "X" || strfType == "I" || strfType == "D" || strfType == "C" || strfType == "P" || strfType == "O"
        //                || strfType == "R" || strfType == "A" || strfType == "F" || strfType == "On Request" || strfType == "Non Refundable (LIVE Booking)/t" || strfType == "Super6E"
        //                || strfType == "Non Refundable (LIVE PNR)" || strfType == "Non Refundable (LIVE BOOKING)" || strfType == "NON REFUNDABLE (INSTANT PNR)" || strfType == "EE")
        //            {
        //                return Core.MojoFareType.Series_Fare_with_PNR;
        //            }
        //            else if (strfType == "Flexi" || strfType == "FlexiFare" || strfType == "FlexiPlus" || strfType == "PremiumFlex" || strfType == "Premium Flex"
        //                || strfType == "EXPRESS_FLEXI" || strfType == "FLX" || strfType == "FLEX" || strfType == "FLY" || strfType == "J" || strfType == "Express FLEXI 20 Kg"
        //                || strfType == "Express FLEXI 30 Kg")
        //            {
        //                return Core.MojoFareType.Flexi;
        //            }

        //            else if (strfType == "PUBLISHED" || strfType == "REGULAR" || strfType == "PUBLISH" || strfType == "Family" || strfType == "Family Fare"
        //                 || strfType == "Flexible" || strfType == "Super Saver" || strfType == "AUTO FIXED FARE" || strfType == "Super 6E" || strfType == "Go More"
        //                 || strfType == "RoundTrip" || strfType == "CLUSTER" || strfType == "BASIC")
        //            {
        //                return Core.MojoFareType.Publish;
        //            }

        //            else if (strfType == "Corp Connect" || strfType == "Corporate Select" || strfType == "Economy"
        //                || strfType == "Coupon Fare" || strfType == "Corporate Value" || strfType == "Economy Promo")
        //            {
        //                return Core.MojoFareType.Corporate;
        //            }

        //            else if (strfType == "Plus")
        //            {
        //                return Core.MojoFareType.Tactical;
        //            }

        //            else if (strfType == "Promo" || strfType == "CN2 Fare")
        //            {
        //                return Core.MojoFareType.Promo;
        //            }

        //            LogCreater.CreateLogFile(strfType + Environment.NewLine, "Log\\Travelogy\\error", DateTime.Today.ToString("ddMMyyy") + ".txt");
        //            return Core.MojoFareType.Tactical;
        //        }
        //    }
        //    else
        //    {
        //        LogCreater.CreateLogFile(strfType + Environment.NewLine, "Log\\Travelogy\\error", DateTime.Today.ToString("ddMMyyy") + ".txt");
        //        return Core.MojoFareType.Tactical;
        //    }
        //    return (Core.FareType)Enum.Parse(typeof(Core.FareType), fType, true);

        //}



        //public Core.FareType getMojoFareType(int fType)
        //{
        //    Core.FareType fareType = Core.FareType.None;
        //    switch (fType)
        //    {
        //        case 0: fareType = Core.FareType.Publish; break;
        //        case 1: fareType = Core.FareType.Coupon; break;
        //        case 2: fareType = Core.FareType.Corporate; break;
        //        case 3: fareType = Core.FareType.SMECrpCon; break;
        //        case 4: fareType = Core.FareType.RTSPLFare; break;
        //        case 5: fareType = Core.FareType.DEALFARE; break;
        //        case 6: fareType = Core.FareType.Publish; break;
        //        case 7: fareType = Core.FareType.Coupon; break;
        //        case 8: fareType = Core.FareType.Corporate; break;
        //        case 9: fareType = Core.FareType.SMECrpCon; break;
        //    }
        //    if (fareType == Core.FareType.None)
        //    {
        //        LogCreater.CreateLogFile(fType + Environment.NewLine, "Log\\Travelogy\\error", DateTime.Today.ToString("ddMMyyy") + ".txt");
        //    }
        //    return fareType;
        //}


        public Core.CabinType getCabinType(string fType)
        {
            Core.CabinType fareType = Core.CabinType.None;
            switch (fType.ToUpper())
            {
                case "ECONOMY": fareType = Core.CabinType.Economy; break;
                case "PREMIUM_ECONOMY": fareType = Core.CabinType.PremiumEconomy; break;
                case "BUSINESS": fareType = Core.CabinType.Business; break;
                case "FIRST": fareType = Core.CabinType.First; break;

            }
            return fareType;
        }
        public int getDuration(string dur)
        {
            int flightdur = 0;
            if (dur != null)
            {
                string[] ssArr = dur.Split(':');
                if (ssArr.Length == 2)
                {
                    if (!string.IsNullOrEmpty(ssArr[0]))
                        flightdur += (Convert.ToInt32(ssArr[0].ToLower().Replace("h", "")) * 60);
                    if (ssArr.Length > 1 && !string.IsNullOrEmpty(ssArr[1]))
                        flightdur += (Convert.ToInt32(ssArr[1].ToLower().Replace("m", "")));
                }
            }
            return flightdur;
        }

       
    }
}
