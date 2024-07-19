using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.TripJack
{
    public class TripJackResponseMapping
    {
        //public void getResult(ref Core.Flight.FlightSearchRequest request, ref TripJackClass.TripJackFlightSearchResponse fsr, ref Core.Flight.FlightSearchResponse response)
        //{
        //    if (request.segment.Count >= 1)
        //    {
        //        if (fsr.searchResult != null && fsr.searchResult.tripInfos != null)
        //        {
        //            #region OnWard fare
        //            if (fsr.searchResult.tripInfos.ONWARD != null && fsr.searchResult.tripInfos.ONWARD.Count > 0)
        //            {
        //                List<Core.Flight.FlightResult> listFlightResult = new List<Core.Flight.FlightResult>();
        //                int Ctr = 0;
        //                foreach (TripJackClass.ONWARD Itin in fsr.searchResult.tripInfos.ONWARD)
        //                {
        //                    Core.Flight.FlightResult result = new Core.Flight.FlightResult()
        //                    {
        //                        //Fare = new Core.Flight.Fare(),
        //                        FareList = new List<Core.Flight.Fare>(),
        //                        IsLCC = Itin.sI.FirstOrDefault().fD.aI.isLcc,
        //                        //LastTicketDate = Itin.LastTicketDate,
        //                        ResultIndex = "tjo" + Ctr++,
        //                        FlightSegments = new List<Core.Flight.FlightSegment>(),
        //                        //Source = Itin.Source,
        //                        //TicketAdvisory = Itin.TicketAdvisory,
        //                        //ValidatingAirline = Itin.sI.FirstOrDefault().fD.aI.code,
        //                        cabinClass = request.cabinType,
        //                        gdsType = Core.GdsType.TripJack,
        //                        valCarrier = Itin.sI.FirstOrDefault().fD.aI.code,
        //                        //Color = Itin.FareClassification.Color,
        //                        //fareType = getFareType(Itin.FareClassification.Type),
        //                        //groupID = "Tbo" + (itinCtr == 0 ? "OB" : "IB"),
        //                        //ffFareType = getFareType(Itin.FareClassification.Type),
        //                        ResultCombination = "TJ"
        //                    };

        //                    #region set Segment
        //                    result.FlightSegments = new List<Core.Flight.FlightSegment>();
        //                    Core.Flight.FlightSegment fs = new Core.Flight.FlightSegment()
        //                    {
        //                        Segments = new List<Core.Flight.Segment>(),
        //                        SegName = "Depart",
        //                        Duration = 0,
        //                        LayoverTime = 0
        //                    };
        //                    int segCtr = 0;
        //                    foreach (var tjSeg in Itin.sI)
        //                    {
        //                        Core.Flight.Segment seg = new Core.Flight.Segment()
        //                        {
        //                            Airline = tjSeg.fD.aI.code,
        //                            FlightNumber = tjSeg.fD.fN,
        //                            equipmentType = tjSeg.fD.eT,
        //                            Origin = tjSeg.da.code,
        //                            Destination = tjSeg.aa.code,
        //                            DepTime = tjSeg.dt,
        //                            ArrTime = tjSeg.at,
        //                            CabinClass = request.cabinType,
        //                            Duration = tjSeg.duration,
        //                            FareClass = Itin.totalPriceList[0].fd.ADULT.cB,
        //                            FromTerminal = tjSeg.da.terminal,
        //                            ToTerminal = tjSeg.aa.terminal,
        //                            IsETicketEligible = true,
        //                            layOverTime = 0,
        //                            OperatingCarrier = (tjSeg.oB != null && !string.IsNullOrEmpty(tjSeg.oB.code) && (tjSeg.fD.aI.code != tjSeg.oB.code)) ? tjSeg.oB.code : tjSeg.fD.aI.code,
        //                            id = tjSeg.id.ToString()
        //                        };
        //                        result.ResultCombination += (seg.Airline + seg.FlightNumber);
        //                        #region LayOverTime
        //                        if (segCtr > 0)
        //                        {
        //                            TimeSpan ts = seg.DepTime - fs.Segments[segCtr - 1].ArrTime;
        //                            fs.Segments[segCtr - 1].layOverTime = Convert.ToInt32(ts.TotalMinutes);
        //                            fs.LayoverTime += fs.Segments[segCtr - 1].layOverTime;
        //                        }
        //                        #endregion

        //                        fs.Duration += tjSeg.duration;
        //                        fs.stop++;
        //                        fs.Segments.Add(seg);
        //                        segCtr++;
        //                    }
        //                    result.FlightSegments.Add(fs);
        //                    #endregion

        //                    #region setAmount
        //                    foreach (var tpl in Itin.totalPriceList)
        //                    {
        //                        Core.Flight.Fare fare = new Core.Flight.Fare()
        //                        {
        //                            FareType = getFareType(tpl.fareIdentifier),
        //                            tjID = tpl.id,
        //                            fareBreakdown = new List<Core.Flight.FareBreakdown>(),
        //                            Currency = "INR",
        //                            cabinType = getCabinType(tpl.fd.ADULT.cc),
        //                            refundType = (Core.RefundType)tpl.fd.ADULT.rT,
        //                            baggageInfo = new Core.Flight.BaggageInfo() { checkInBaggage = tpl.fd.ADULT.bI.iB, cabinBaggage = tpl.fd.ADULT.bI.cB },
        //                            bookingClass = tpl.fd.ADULT.bI.cB,
        //                            sri = tpl.sri,
        //                            msri = tpl.msri
        //                        };
        //                        if (request.adults > 0)
        //                        {
        //                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
        //                            {
        //                                BaseFare = tpl.fd.ADULT.fC.BF,
        //                                Tax = tpl.fd.ADULT.fC.TAF,
        //                                CommissionEarned = tpl.fd.ADULT.fC.NCM,
        //                                YQTax = tpl.fd.ADULT.afC.TAF.YQ,
        //                                PassengerType = Core.PassengerType.Adult,
        //                                OtherCharges = tpl.fd.ADULT.afC.TAF.OT,
        //                            };
        //                            fare.BaseFare += (paxFare.BaseFare * request.adults);
        //                            fare.Tax += (paxFare.Tax * request.adults);
        //                            fare.CommissionEarned += (paxFare.CommissionEarned * request.adults);
        //                            fare.fareBreakdown.Add(paxFare);
        //                        }
        //                        if (request.child > 0)
        //                        {
        //                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
        //                            {
        //                                BaseFare = tpl.fd.CHILD.fC.BF,
        //                                Tax = tpl.fd.CHILD.fC.TAF,
        //                                CommissionEarned = tpl.fd.CHILD.fC.NCM,
        //                                YQTax = tpl.fd.CHILD.afC.TAF.YQ,
        //                                PassengerType = Core.PassengerType.Child,
        //                                OtherCharges = tpl.fd.CHILD.afC.TAF.OT,
        //                            };
        //                            fare.BaseFare += (paxFare.BaseFare * request.child);
        //                            fare.Tax += (paxFare.Tax * request.child);
        //                            fare.CommissionEarned += (paxFare.CommissionEarned * request.child);
        //                            fare.fareBreakdown.Add(paxFare);
        //                        }
        //                        if (request.infants > 0)
        //                        {
        //                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
        //                            {
        //                                BaseFare = tpl.fd.INFANT.fC.BF,
        //                                Tax = tpl.fd.INFANT.fC.TAF,
        //                                CommissionEarned = tpl.fd.INFANT.fC.NCM,
        //                                YQTax = tpl.fd.INFANT.afC.TAF.YQ,
        //                                PassengerType = Core.PassengerType.Infant,
        //                                OtherCharges = tpl.fd.INFANT.afC.TAF.OT,
        //                            };
        //                            fare.BaseFare += (paxFare.BaseFare * request.infants);
        //                            fare.Tax += (paxFare.Tax * request.infants);
        //                            fare.CommissionEarned += (paxFare.CommissionEarned * request.infants);
        //                            fare.fareBreakdown.Add(paxFare);
        //                        }
        //                        fare.NetFare = fare.grandTotal = (fare.BaseFare + fare.Tax);/* - fare.CommissionEarned */

        //                        //if (fare.FareType != Core.FareType.OFFER_FARE_WITHOUT_PNR && request.cabinType == fare.cabinType)
        //                        if (request.cabinType == fare.cabinType)
        //                        {
        //                            //   if (result.FlightSegments[0].Segments[0].Airline != "6E" && result.FlightSegments[0].Segments[0].Airline != "G8")
        //                            if (result.FlightSegments[0].Segments[0].Airline != "G8" && result.FlightSegments[0].Segments[0].Airline != "6E")/* */
        //                            {
        //                                //if (fare.FareType != Core.FareType.OFFER_RETURN_FARE_WITHOUT_PNR)
        //                                //{
        //                                result.FareList.Add(fare);
        //                                //}
        //                            }

        //                            ////if (false && (request.sourceMedia == "1000" || request.sourceMedia == "1015") &&
        //                            ////    (result.FlightSegments[0].Segments[0].Airline == "6E" || result.FlightSegments[0].Segments[0].Airline == "G8" ||
        //                            ////    result.FlightSegments[0].Segments[0].Airline == "SG" || result.FlightSegments[0].Segments[0].Airline == "I5"))
        //                            ////{
        //                            ////    //if(fare.FareType != Core.FareType.Tactical && fare.FareType != Core.FareType.OFFER_FARE_WITHOUT_PNR
        //                            ////    //&& fare.FareType != Core.FareType.OFFER_FARE_WITH_PNR && fare.FareType != Core.FareType.OFFER_RETURN_FARE_WITHOUT_PNR
        //                            ////    //&& fare.FareType != Core.FareType.OFFER_RETURN_FARE_WITH_PNR)
        //                            ////    if (fare.FareType == Core.FareType.PUBLISH)
        //                            ////    {
        //                            ////        result.FareList.Add(fare);
        //                            ////    }
        //                            ////}
        //                            ////else
        //                            ////{
        //                            ////    result.FareList.Add(fare);
        //                            ////}
        //                            //if (request.sourceMedia != "1000"&& request.sourceMedia != "1015" && result.FlightSegments[0].Segments[0].Airline != "6E"
        //                            //    && fare.FareType != Core.FareType.Tactical && fare.FareType != Core.FareType.OFFER_FARE_WITHOUT_PNR
        //                            //    && fare.FareType != Core.FareType.OFFER_FARE_WITH_PNR && fare.FareType != Core.FareType.OFFER_RETURN_FARE_WITHOUT_PNR
        //                            //    && fare.FareType != Core.FareType.OFFER_RETURN_FARE_WITH_PNR)
        //                            //{
        //                            //    result.FareList.Add(fare);
        //                            //}
        //                        }
        //                    }
        //                    if (result.FareList.Count > 0)/*&& result.FareList.Exists(k => k.FareType == Core.FareType.PUBLISH)*/
        //                    {
        //                        result.Fare = result.FareList.OrderBy(k => k.grandTotal).FirstOrDefault();

        //                        listFlightResult.Add(result);
        //                    }
        //                    #endregion
        //                }
        //                response.Results.Add(listFlightResult);
        //            }
        //            #endregion

        //            #region Return fare
        //            if (fsr.searchResult.tripInfos.RETURN != null && fsr.searchResult.tripInfos.RETURN.Count > 0)
        //            {
        //                int Ctr = 0;
        //                List<Core.Flight.FlightResult> listFlightResult = new List<Core.Flight.FlightResult>();
        //                foreach (TripJackClass.ONWARD Itin in fsr.searchResult.tripInfos.RETURN)
        //                {
        //                    Core.Flight.FlightResult result = new Core.Flight.FlightResult()
        //                    {
        //                        //Fare = new Core.Flight.Fare(),
        //                        FareList = new List<Core.Flight.Fare>(),
        //                        IsLCC = Itin.sI.FirstOrDefault().fD.aI.isLcc,
        //                        //LastTicketDate = Itin.LastTicketDate,
        //                        ResultIndex = "tji" + Ctr++,
        //                        FlightSegments = new List<Core.Flight.FlightSegment>(),
        //                        //Source = Itin.Source,
        //                        //TicketAdvisory = Itin.TicketAdvisory,
        //                        //ValidatingAirline = Itin.sI.FirstOrDefault().fD.aI.code,
        //                        cabinClass = request.cabinType,
        //                        gdsType = Core.GdsType.TripJack,
        //                        valCarrier = Itin.sI.FirstOrDefault().fD.aI.code,
        //                        //Color = Itin.FareClassification.Color,
        //                        //fareType = getFareType(Itin.FareClassification.Type),
        //                        //groupID = "Tbo" + (itinCtr == 0 ? "OB" : "IB"),
        //                        //ffFareType = getFareType(Itin.FareClassification.Type),
        //                        ResultCombination = "TJ"
        //                    };

        //                    #region set Segment
        //                    result.FlightSegments = new List<Core.Flight.FlightSegment>();
        //                    Core.Flight.FlightSegment fs = new Core.Flight.FlightSegment()
        //                    {
        //                        Segments = new List<Core.Flight.Segment>(),
        //                        SegName = "Return",
        //                        Duration = 0,
        //                        LayoverTime = 0
        //                    };
        //                    int segCtr = 0;
        //                    foreach (var tjSeg in Itin.sI)
        //                    {
        //                        Core.Flight.Segment seg = new Core.Flight.Segment()
        //                        {
        //                            Airline = tjSeg.fD.aI.code,
        //                            FlightNumber = tjSeg.fD.fN,
        //                            equipmentType = tjSeg.fD.eT,
        //                            Origin = tjSeg.da.code,
        //                            Destination = tjSeg.aa.code,
        //                            DepTime = tjSeg.dt,
        //                            ArrTime = tjSeg.at,
        //                            CabinClass = request.cabinType,
        //                            Duration = tjSeg.duration,
        //                            FareClass = Itin.totalPriceList[0].fd.ADULT.cB,
        //                            FromTerminal = tjSeg.da.terminal,
        //                            ToTerminal = tjSeg.aa.terminal,
        //                            IsETicketEligible = true,
        //                            layOverTime = 0,
        //                            OperatingCarrier = (tjSeg.oB != null && !string.IsNullOrEmpty(tjSeg.oB.code) && (tjSeg.fD.aI.code != tjSeg.oB.code)) ? tjSeg.oB.code : tjSeg.fD.aI.code,
        //                            id = tjSeg.id.ToString()
        //                        };
        //                        result.ResultCombination += (seg.Airline + seg.FlightNumber);
        //                        #region LayOverTime
        //                        if (segCtr > 0)
        //                        {
        //                            TimeSpan ts = seg.DepTime - fs.Segments[segCtr - 1].ArrTime;
        //                            fs.Segments[segCtr - 1].layOverTime = Convert.ToInt32(ts.TotalMinutes);
        //                            fs.LayoverTime += fs.Segments[segCtr - 1].layOverTime;
        //                        }
        //                        #endregion


        //                        fs.Duration += tjSeg.duration;
        //                        fs.Segments.Add(seg);
        //                        fs.stop++;
        //                        segCtr++;
        //                    }
        //                    result.FlightSegments.Add(fs);
        //                    #endregion

        //                    #region setAmount
        //                    foreach (var tpl in Itin.totalPriceList)
        //                    {
        //                        Core.Flight.Fare fare = new Core.Flight.Fare()
        //                        {
        //                            FareType = getFareType(tpl.fareIdentifier),
        //                            tjID = tpl.id,
        //                            fareBreakdown = new List<Core.Flight.FareBreakdown>(),
        //                            Currency = "INR",
        //                            cabinType = getCabinType(tpl.fd.ADULT.cc),
        //                            refundType = (Core.RefundType)tpl.fd.ADULT.rT,
        //                            baggageInfo = new Core.Flight.BaggageInfo() { checkInBaggage = tpl.fd.ADULT.bI.iB, cabinBaggage = tpl.fd.ADULT.bI.cB },
        //                            bookingClass = tpl.fd.ADULT.bI.cB,
        //                            sri = tpl.sri,
        //                            msri = tpl.msri
        //                        };
        //                        if (request.adults > 0)
        //                        {
        //                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
        //                            {
        //                                BaseFare = tpl.fd.ADULT.fC.BF,
        //                                Tax = tpl.fd.ADULT.fC.TAF,
        //                                CommissionEarned = tpl.fd.ADULT.fC.NCM,
        //                                YQTax = tpl.fd.ADULT.afC.TAF.YQ,
        //                                PassengerType = Core.PassengerType.Adult,
        //                                OtherCharges = tpl.fd.ADULT.afC.TAF.OT,
        //                            };
        //                            fare.BaseFare += (paxFare.BaseFare * request.adults);
        //                            fare.Tax += (paxFare.Tax * request.adults);
        //                            fare.CommissionEarned += (paxFare.CommissionEarned * request.adults);
        //                            fare.fareBreakdown.Add(paxFare);
        //                        }
        //                        if (request.child > 0)
        //                        {
        //                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
        //                            {
        //                                BaseFare = tpl.fd.CHILD.fC.BF,
        //                                Tax = tpl.fd.CHILD.fC.TAF,
        //                                CommissionEarned = tpl.fd.CHILD.fC.NCM,
        //                                YQTax = tpl.fd.CHILD.afC.TAF.YQ,
        //                                PassengerType = Core.PassengerType.Child,
        //                                OtherCharges = tpl.fd.CHILD.afC.TAF.OT,
        //                            };
        //                            fare.BaseFare += (paxFare.BaseFare * request.child);
        //                            fare.Tax += (paxFare.Tax * request.child);
        //                            fare.CommissionEarned += (paxFare.CommissionEarned * request.child);
        //                            fare.fareBreakdown.Add(paxFare);
        //                        }
        //                        if (request.infants > 0)
        //                        {
        //                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
        //                            {
        //                                BaseFare = tpl.fd.INFANT.fC.BF,
        //                                Tax = tpl.fd.INFANT.fC.TAF,
        //                                CommissionEarned = tpl.fd.INFANT.fC.NCM,
        //                                YQTax = tpl.fd.INFANT.afC.TAF.YQ,
        //                                PassengerType = Core.PassengerType.Infant,
        //                                OtherCharges = tpl.fd.INFANT.afC.TAF.OT,
        //                            };
        //                            fare.BaseFare += (paxFare.BaseFare * request.infants);
        //                            fare.Tax += (paxFare.Tax * request.infants);
        //                            fare.CommissionEarned += (paxFare.CommissionEarned * request.infants);
        //                            fare.fareBreakdown.Add(paxFare);
        //                        }
        //                        fare.NetFare = fare.grandTotal = (fare.BaseFare + fare.Tax);/* - fare.CommissionEarned*/
        //                        //if (fare.FareType != Core.FareType.OFFER_FARE_WITHOUT_PNR && request.cabinType == fare.cabinType)  
        //                        if (request.cabinType == fare.cabinType)
        //                        {
        //                            //  if (result.FlightSegments[0].Segments[0].Airline != "6E" && result.FlightSegments[0].Segments[0].Airline != "G8")
        //                            if (result.FlightSegments[0].Segments[0].Airline != "G8")
        //                            {
        //                                //if (fare.FareType != Core.FareType.OFFER_RETURN_FARE_WITHOUT_PNR)
        //                                //{
        //                                result.FareList.Add(fare);
        //                                //}
        //                            }
        //                            //if (false && (request.sourceMedia == "1000" || request.sourceMedia == "1015") &&
        //                            //    (result.FlightSegments[0].Segments[0].Airline == "6E" || result.FlightSegments[0].Segments[0].Airline == "G8" ||
        //                            //    result.FlightSegments[0].Segments[0].Airline == "SG" || result.FlightSegments[0].Segments[0].Airline == "I5"))
        //                            //{
        //                            //    if (fare.FareType != Core.FareType.TACTICAL && fare.FareType != Core.FareType.OFFER_FARE_WITHOUT_PNR
        //                            //    && fare.FareType != Core.FareType.OFFER_FARE_WITH_PNR && fare.FareType != Core.FareType.OFFER_RETURN_FARE_WITHOUT_PNR
        //                            //    && fare.FareType != Core.FareType.OFFER_RETURN_FARE_WITH_PNR)
        //                            //    {
        //                            //        result.FareList.Add(fare);
        //                            //    }
        //                            //}
        //                            //else
        //                            //{
        //                            //    result.FareList.Add(fare);
        //                            //}
        //                        }
        //                    }
        //                    if (result.FareList.Count > 0)/* && result.FareList.Exists(k => k.FareType == Core.FareType.PUBLISH)*/
        //                    {
        //                        result.Fare = result.FareList.OrderBy(k => k.grandTotal).FirstOrDefault();
        //                        listFlightResult.Add(result);
        //                    }
        //                    #endregion
        //                }
        //                response.Results.Add(listFlightResult);
        //            }
        //            #endregion

        //            #region Combo fare
        //            if (fsr.searchResult.tripInfos.COMBO != null && fsr.searchResult.tripInfos.COMBO.Count > 0)
        //            {
        //                int Ctr = 0;
        //                List<Core.Flight.FlightResult> listFlightResult = new List<Core.Flight.FlightResult>();
        //                foreach (TripJackClass.ONWARD Itin in fsr.searchResult.tripInfos.COMBO)
        //                {
        //                    Core.Flight.FlightResult result = new Core.Flight.FlightResult()
        //                    {
        //                        //Fare = new Core.Flight.Fare(),
        //                        FareList = new List<Core.Flight.Fare>(),
        //                        IsLCC = Itin.sI.FirstOrDefault().fD.aI.isLcc,
        //                        //LastTicketDate = Itin.LastTicketDate,
        //                        ResultIndex = "tjc" + Ctr++,
        //                        FlightSegments = new List<Core.Flight.FlightSegment>(),
        //                        //Source = Itin.Source,
        //                        //TicketAdvisory = Itin.TicketAdvisory,
        //                        //ValidatingAirline = Itin.sI.FirstOrDefault().fD.aI.code,
        //                        cabinClass = request.cabinType,
        //                        gdsType = Core.GdsType.TripJack,
        //                        valCarrier = Itin.sI.FirstOrDefault().fD.aI.code,
        //                        //Color = Itin.FareClassification.Color,
        //                        //fareType = getFareType(Itin.FareClassification.Type),
        //                        //groupID = "Tbo" + (itinCtr == 0 ? "OB" : "IB"),
        //                        //ffFareType = getFareType(Itin.FareClassification.Type),
        //                        ResultCombination = "TJ"
        //                    };
        //                    result.FlightSegments = new List<Core.Flight.FlightSegment>();
        //                    #region set Segment outBound

        //                    if (Itin.sI.Where(k => k.isRs == false).ToList().Count > 0)
        //                    {
        //                        Core.Flight.FlightSegment fs = new Core.Flight.FlightSegment()
        //                        {
        //                            Segments = new List<Core.Flight.Segment>(),
        //                            SegName = "Depart",
        //                            Duration = 0,
        //                            LayoverTime = 0
        //                        };
        //                        int segCtr = 0;
        //                        foreach (var tjSeg in Itin.sI.Where(k => k.isRs == false))
        //                        {
        //                            Core.Flight.Segment seg = new Core.Flight.Segment()
        //                            {
        //                                Airline = tjSeg.fD.aI.code,
        //                                FlightNumber = tjSeg.fD.fN,
        //                                equipmentType = tjSeg.fD.eT,
        //                                Origin = tjSeg.da.code,
        //                                Destination = tjSeg.aa.code,
        //                                DepTime = tjSeg.dt,
        //                                ArrTime = tjSeg.at,
        //                                CabinClass = request.cabinType,
        //                                Duration = tjSeg.duration,
        //                                FareClass = Itin.totalPriceList[0].fd.ADULT.cB,
        //                                FromTerminal = tjSeg.da.terminal,
        //                                ToTerminal = tjSeg.aa.terminal,
        //                                IsETicketEligible = true,
        //                                layOverTime = 0,
        //                                OperatingCarrier = (tjSeg.oB != null && !string.IsNullOrEmpty(tjSeg.oB.code) && (tjSeg.fD.aI.code != tjSeg.oB.code)) ? tjSeg.oB.code : tjSeg.fD.aI.code,
        //                                id = tjSeg.id.ToString()
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


        //                            fs.Duration += tjSeg.duration;
        //                            fs.stop++;
        //                            fs.Segments.Add(seg);
        //                            segCtr++;
        //                        }
        //                        result.FlightSegments.Add(fs);
        //                    }
        //                    #endregion

        //                    #region set Segment inBound

        //                    if (Itin.sI.Where(k => k.isRs == true).ToList().Count > 0)
        //                    {
        //                        Core.Flight.FlightSegment fs = new Core.Flight.FlightSegment()
        //                        {
        //                            Segments = new List<Core.Flight.Segment>(),
        //                            SegName = "Return",

        //                            Duration = 0,
        //                            LayoverTime = 0
        //                        };
        //                        int segCtr = 0;
        //                        foreach (var tjSeg in Itin.sI.Where(k => k.isRs))
        //                        {
        //                            Core.Flight.Segment seg = new Core.Flight.Segment()
        //                            {
        //                                Airline = tjSeg.fD.aI.code,
        //                                FlightNumber = tjSeg.fD.fN,
        //                                equipmentType = tjSeg.fD.eT,
        //                                Origin = tjSeg.da.code,
        //                                Destination = tjSeg.aa.code,
        //                                DepTime = tjSeg.dt,
        //                                ArrTime = tjSeg.at,
        //                                CabinClass = request.cabinType,
        //                                Duration = tjSeg.duration,
        //                                FareClass = Itin.totalPriceList[0].fd.ADULT.cB,
        //                                FromTerminal = tjSeg.da.terminal,
        //                                ToTerminal = tjSeg.aa.terminal,
        //                                IsETicketEligible = true,
        //                                layOverTime = 0,
        //                                OperatingCarrier = (tjSeg.oB != null && !string.IsNullOrEmpty(tjSeg.oB.code) && (tjSeg.fD.aI.code != tjSeg.oB.code)) ? tjSeg.oB.code : tjSeg.fD.aI.code,
        //                                id = tjSeg.id.ToString()
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


        //                            fs.Duration += tjSeg.duration;
        //                            fs.stop++;
        //                            fs.Segments.Add(seg);
        //                            segCtr++;
        //                        }
        //                        result.FlightSegments.Add(fs);
        //                    }
        //                    #endregion

        //                    #region setAmount
        //                    foreach (var tpl in Itin.totalPriceList)
        //                    {
        //                        Core.Flight.Fare fare = new Core.Flight.Fare()
        //                        {
        //                            FareType = getFareType(tpl.fareIdentifier),
        //                            tjID = tpl.id,
        //                            fareBreakdown = new List<Core.Flight.FareBreakdown>(),
        //                            Currency = "INR",
        //                            cabinType = getCabinType(tpl.fd.ADULT.cc),
        //                            refundType = (Core.RefundType)tpl.fd.ADULT.rT,
        //                            baggageInfo = new Core.Flight.BaggageInfo() { checkInBaggage = tpl.fd.ADULT.bI.iB, cabinBaggage = tpl.fd.ADULT.bI.cB },
        //                            bookingClass = tpl.fd.ADULT.bI.cB
        //                        };
        //                        if (request.adults > 0)
        //                        {
        //                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
        //                            {
        //                                BaseFare = tpl.fd.ADULT.fC.BF,
        //                                Tax = tpl.fd.ADULT.fC.TAF,
        //                                CommissionEarned = tpl.fd.ADULT.fC.NCM,
        //                                YQTax = tpl.fd.ADULT.afC.TAF.YQ,
        //                                PassengerType = Core.PassengerType.Adult,
        //                                OtherCharges = tpl.fd.ADULT.afC.TAF.OT,
        //                            };
        //                            fare.BaseFare += (paxFare.BaseFare * request.adults);
        //                            fare.Tax += (paxFare.Tax * request.adults);
        //                            fare.CommissionEarned += (paxFare.CommissionEarned * request.adults);
        //                            fare.fareBreakdown.Add(paxFare);
        //                        }
        //                        if (request.child > 0)
        //                        {
        //                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
        //                            {
        //                                BaseFare = tpl.fd.CHILD.fC.BF,
        //                                Tax = tpl.fd.CHILD.fC.TAF,
        //                                CommissionEarned = tpl.fd.CHILD.fC.NCM,
        //                                YQTax = tpl.fd.CHILD.afC.TAF.YQ,
        //                                PassengerType = Core.PassengerType.Child,
        //                                OtherCharges = tpl.fd.CHILD.afC.TAF.OT,
        //                            };
        //                            fare.BaseFare += (paxFare.BaseFare * request.child);
        //                            fare.Tax += (paxFare.Tax * request.child);
        //                            fare.CommissionEarned += (paxFare.CommissionEarned * request.child);
        //                            fare.fareBreakdown.Add(paxFare);
        //                        }
        //                        if (request.infants > 0)
        //                        {
        //                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
        //                            {
        //                                BaseFare = tpl.fd.INFANT.fC.BF,
        //                                Tax = tpl.fd.INFANT.fC.TAF,
        //                                CommissionEarned = tpl.fd.INFANT.fC.NCM,
        //                                YQTax = tpl.fd.INFANT.afC.TAF.YQ,
        //                                PassengerType = Core.PassengerType.Infant,

        //                                OtherCharges = tpl.fd.INFANT.afC.TAF.OT,
        //                            };
        //                            fare.BaseFare += (paxFare.BaseFare * request.infants);
        //                            fare.Tax += (paxFare.Tax * request.infants);
        //                            fare.CommissionEarned += (paxFare.CommissionEarned * request.infants);
        //                            fare.fareBreakdown.Add(paxFare);
        //                        }
        //                        fare.NetFare = fare.grandTotal = (fare.BaseFare + fare.Tax);/* - fare.CommissionEarned*/
        //                        //if (fare.FareType != Core.FareType.OFFER_FARE_WITHOUT_PNR && request.cabinType == fare.cabinType)  
        //                        if (request.cabinType == fare.cabinType)
        //                        {
        //                            if (result.FlightSegments[0].Segments[0].Airline != "G8")
        //                            {
        //                                //if (fare.FareType != Core.FareType.OFFER_RETURN_FARE_WITHOUT_PNR)
        //                                //{
        //                                result.FareList.Add(fare);
        //                                //}
        //                            }

        //                            //if (result.FlightSegments[0].Segments[0].Airline == "6E")
        //                            //{

        //                            //}
        //                            //else
        //                            //{

        //                            //}
        //                            //if (false && (request.sourceMedia == "1000" || request.sourceMedia == "1015") &&
        //                            //    (result.FlightSegments[0].Segments[0].Airline == "6E" || result.FlightSegments[0].Segments[0].Airline == "G8" ||
        //                            //    result.FlightSegments[0].Segments[0].Airline == "SG" || result.FlightSegments[0].Segments[0].Airline == "I5"))
        //                            //{
        //                            //    if (fare.FareType != Core.FareType.TACTICAL && fare.FareType != Core.FareType.OFFER_FARE_WITHOUT_PNR
        //                            //    && fare.FareType != Core.FareType.OFFER_FARE_WITH_PNR && fare.FareType != Core.FareType.OFFER_RETURN_FARE_WITHOUT_PNR
        //                            //    && fare.FareType != Core.FareType.OFFER_RETURN_FARE_WITH_PNR)
        //                            //    {
        //                            //        result.FareList.Add(fare);
        //                            //    }
        //                            //}
        //                            //else
        //                            //{
        //                            //    result.FareList.Add(fare);
        //                            //}
        //                        }
        //                    }
        //                    if (result.FareList.Count > 0)/*&& result.FareList.Exists(k => k.FareType == Core.FareType.PUBLISH)*/
        //                    {
        //                        result.Fare = result.FareList.OrderBy(k => k.grandTotal).FirstOrDefault();
        //                        listFlightResult.Add(result);
        //                    }
        //                    #endregion
        //                }
        //                response.Results.Add(listFlightResult);
        //            }
        //            #endregion
        //        }
        //    }
        //    else
        //    {

        //    }
        //}
        public void getResults(Core.Flight.FlightSearchRequest request, ref TripJackClass.TripJackFlightSearchResponse fsr, ref Core.Flight.FlightSearchResponseShort response)
        {
            int totPax = request.adults + request.child + request.infants;
            try
            {
                int ctrError = 0;
                if (request.segment.Count >= 1)
                {
                    if (fsr.searchResult != null && fsr.searchResult.tripInfos != null)
                    {
                        #region OnWard fare
                        if (fsr.searchResult.tripInfos.ONWARD != null && fsr.searchResult.tripInfos.ONWARD.Count > 0)
                        {
                            List<Core.Flight.FlightResult> listFlightResult = new List<Core.Flight.FlightResult>();
                            int Ctr = 0;
                            //try
                            //{
                            foreach (TripJackClass.ONWARD Itin in fsr.searchResult.tripInfos.ONWARD)
                            {
                                if (Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.TripJack) &&
                                       (o.SiteId == request.siteId) && (o.FareType.Count == 0) && o.airline.Contains(Itin.sI.FirstOrDefault().fD.aI.code) &&
                                       ((o.CountryFrom.Any() && o.CountryFrom.Contains(request.segment[0].orgArp.countryCode)) || o.CountryFrom.Any() == false) &&
                                       ((o.CountryTo.Any() && o.CountryTo.Contains(request.segment[0].destArp.countryCode)) || o.CountryTo.Any() == false) &&
                                       (o.CountryFrom_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                                       (o.CountryTo_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                                       ((o.WeekOfDays.Any() && o.WeekOfDays.Contains((WeekDays)Enum.Parse(typeof(WeekDays), Convert.ToString(DateTime.Today.DayOfWeek)))) || o.WeekOfDays.Any() == false) &&
                                        ((o.AffiliateId.Any() && o.AffiliateId.Contains(request.sourceMedia)) || o.AffiliateId.Any() == false) &&
                                       (o.AffiliateId_Not.Contains(request.sourceMedia) == false) &&
                                        ((o.NoOfPaxFrom <= totPax && o.NoOfPaxTo >= totPax)) &&
                                       (o.device == Device.None || o.device == request.device)).ToList().Count == 0)
                                {

                                    Core.Flight.FlightResult result = new Core.Flight.FlightResult()
                                    {
                                        //Fare = new Core.Flight.Fare(),
                                        FareList = new List<Core.Flight.Fare>(),
                                        IsLCC = Itin.sI.FirstOrDefault().fD.aI.isLcc,
                                        //LastTicketDate = Itin.LastTicketDate,
                                        ResultIndex = "tjo" + Ctr++,
                                        FlightSegments = new List<Core.Flight.FlightSegment>(),
                                        //Source = Itin.Source,
                                        //TicketAdvisory = Itin.TicketAdvisory,
                                        //ValidatingAirline = Itin.sI.FirstOrDefault().fD.aI.code,
                                        cabinClass = request.cabinType,
                                        gdsType = Core.GdsType.TripJack,
                                        valCarrier = Itin.sI.FirstOrDefault().fD.aI.code,
                                        //Color = Itin.FareClassification.Color,
                                        //fareType = getFareType(Itin.FareClassification.Type),
                                        //groupID = "Tbo" + (itinCtr == 0 ? "OB" : "IB"),
                                        //ffFareType = getFareType(Itin.FareClassification.Type),
                                        ResultCombination = ""
                                    };

                                    #region set Segment
                                    result.FlightSegments = new List<Core.Flight.FlightSegment>();
                                    Core.Flight.FlightSegment fs = new Core.Flight.FlightSegment()
                                    {
                                        Segments = new List<Core.Flight.Segment>(),
                                        SegName = "Depart",
                                        Duration = 0,
                                        LayoverTime = 0
                                    };
                                    int segCtr = 0;
                                    foreach (var tjSeg in Itin.sI)
                                    {
                                        Core.Flight.Segment seg = new Core.Flight.Segment()
                                        {
                                            Airline = tjSeg.fD.aI.code,
                                            FlightNumber = tjSeg.fD.fN,
                                            equipmentType = tjSeg.fD.eT,
                                            Origin = tjSeg.da.code,
                                            Destination = tjSeg.aa.code,
                                            DepTime = tjSeg.dt,
                                            ArrTime = tjSeg.at,
                                            CabinClass = request.cabinType,
                                            Duration = tjSeg.duration,
                                            FareClass = Itin.totalPriceList[0].fd.ADULT.cB,
                                            FromTerminal = tjSeg.da.terminal,
                                            ToTerminal = tjSeg.aa.terminal,
                                            IsETicketEligible = true,
                                            layOverTime = 0,
                                            OperatingCarrier = (tjSeg.oB != null && !string.IsNullOrEmpty(tjSeg.oB.code) && (tjSeg.fD.aI.code != tjSeg.oB.code)) ? tjSeg.oB.code : tjSeg.fD.aI.code,
                                            id = tjSeg.id.ToString()
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

                                        fs.Duration += tjSeg.duration;
                                        fs.stop++;
                                        fs.Segments.Add(seg);
                                        segCtr++;
                                    }
                                    result.FlightSegments.Add(fs);
                                    #endregion

                                    #region setAmount
                                    foreach (var tpl in Itin.totalPriceList)
                                    {
                                        Core.Flight.Fare fare = new Core.Flight.Fare()
                                        {
                                            SeatAvailable = tpl.fd.ADULT.sR,
                                            FareType = getFareType(tpl.fareIdentifier, result.valCarrier),
                                            mojoFareType = Core.FlightUtility.GetFmFareType(tpl.fareIdentifier, result.valCarrier, GdsType.TripJack),
                                            tjID = tpl.id,
                                            fareBreakdown = new List<Core.Flight.FareBreakdown>(),
                                            Currency = "INR",
                                            cabinType = getCabinType(tpl.fd.ADULT.cc),
                                            refundType = (Core.RefundType)tpl.fd.ADULT.rT,
                                            baggageInfo = new Core.Flight.BaggageInfo() { checkInBaggage = tpl.fd.ADULT.bI.iB, cabinBaggage = tpl.fd.ADULT.bI.cB },
                                            bookingClass = tpl.fd.ADULT.bI.cB,
                                            sri = tpl.sri,
                                            msri = tpl.msri,
                                            gdsType = GdsType.TripJack,
                                        };
                                        if (fare.mojoFareType == MojoFareType.None || fare.mojoFareType == MojoFareType.Unknown)
                                        {
                                            LogCreater.CreateLogFile(tpl.fareIdentifier + "~" + result.valCarrier, "Log\\FareType", "TripJack" + DateTime.Today.ToString("ddMMyyy") + ".txt");
                                        }


                                        if (request.adults > 0)
                                        {
                                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
                                            {
                                                BaseFare = tpl.fd.ADULT.fC.BF,
                                                Tax = tpl.fd.ADULT.fC.TAF,
                                                CommissionEarned = tpl.fd.ADULT.fC.NCM,
                                                YQTax = tpl.fd.ADULT.afC.TAF.YQ,
                                                PassengerType = Core.PassengerType.Adult,
                                                OtherCharges = tpl.fd.ADULT.afC.TAF.OT,
                                            };
                                            fare.BaseFare += (paxFare.BaseFare * request.adults);
                                            fare.Tax += (paxFare.Tax * request.adults);
                                            fare.CommissionEarned += (paxFare.CommissionEarned * request.adults);
                                            fare.fareBreakdown.Add(paxFare);
                                        }
                                        if (request.child > 0)
                                        {
                                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
                                            {
                                                BaseFare = tpl.fd.CHILD.fC.BF,
                                                Tax = tpl.fd.CHILD.fC.TAF,
                                                CommissionEarned = tpl.fd.CHILD.fC.NCM,
                                                YQTax = tpl.fd.CHILD.afC.TAF.YQ,
                                                PassengerType = Core.PassengerType.Child,
                                                OtherCharges = tpl.fd.CHILD.afC.TAF.OT,
                                            };
                                            fare.BaseFare += (paxFare.BaseFare * request.child);
                                            fare.Tax += (paxFare.Tax * request.child);
                                            fare.CommissionEarned += (paxFare.CommissionEarned * request.child);
                                            fare.fareBreakdown.Add(paxFare);
                                        }
                                        if (request.infants > 0)
                                        {
                                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
                                            {
                                                BaseFare = tpl.fd.INFANT.fC.BF,
                                                Tax = tpl.fd.INFANT.fC.TAF,
                                                CommissionEarned = tpl.fd.INFANT.fC.NCM,
                                                YQTax = tpl.fd.INFANT.afC.TAF.YQ,
                                                PassengerType = Core.PassengerType.Infant,
                                                OtherCharges = tpl.fd.INFANT.afC.TAF.OT,
                                            };
                                            fare.BaseFare += (paxFare.BaseFare * request.infants);
                                            fare.Tax += (paxFare.Tax * request.infants);
                                            fare.CommissionEarned += (paxFare.CommissionEarned * request.infants);
                                            fare.fareBreakdown.Add(paxFare);
                                        }
                                        if (result.FlightSegments[0].Segments[0].Airline == "UK")
                                        {
                                            fare.NetFare = fare.grandTotal = (fare.BaseFare + fare.Tax) - fare.OtherCharges;
                                        }
                                        else
                                        {
                                            fare.NetFare = fare.grandTotal = (fare.BaseFare + fare.Tax);/* - fare.CommissionEarned */
                                        }
                                        if (request.cabinType == fare.cabinType)
                                        {
                                            if (fare.mojoFareType != MojoFareType.Publish && result.valCarrier == "AI")
                                            {

                                            }
                                            #region BlockAirlines
                                            if (fare.mojoFareType == MojoFareType.SeriesFareWithoutPNR)
                                            {

                                            }
                                            if (Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.TripJack) &&
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
                                            if (request.segment.Count > 1 && (fare.mojoFareType == MojoFareType.SeriesFareWithPNR || fare.mojoFareType == MojoFareType.SeriesFareWithoutPNR))
                                            {
                                                fare.isBlock = true;
                                            }
                                            result.FareList.Add(fare);

                                            #endregion
                                        }
                                    }

                                    #endregion
                                    listFlightResult.Add(result);
                                }
                            }
                            response.Results.Add(listFlightResult);
                        }
                        #endregion

                        #region Return fare
                        if (fsr.searchResult.tripInfos.RETURN != null && fsr.searchResult.tripInfos.RETURN.Count > 0)
                        {
                            int Ctr = 0;
                            List<Core.Flight.FlightResult> listFlightResult = new List<Core.Flight.FlightResult>();
                            foreach (TripJackClass.ONWARD Itin in fsr.searchResult.tripInfos.RETURN)
                            {
                                if (Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.TripJack) &&
                                         (o.SiteId == request.siteId) && (o.FareType.Count == 0) && o.airline.Contains(Itin.sI.FirstOrDefault().fD.aI.code) &&
                                         ((o.CountryFrom.Any() && o.CountryFrom.Contains(request.segment[0].orgArp.countryCode)) || o.CountryFrom.Any() == false) &&
                                         ((o.CountryTo.Any() && o.CountryTo.Contains(request.segment[0].destArp.countryCode)) || o.CountryTo.Any() == false) &&
                                         (o.CountryFrom_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                                         (o.CountryTo_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                                         ((o.WeekOfDays.Any() && o.WeekOfDays.Contains((WeekDays)Enum.Parse(typeof(WeekDays), Convert.ToString(DateTime.Today.DayOfWeek)))) || o.WeekOfDays.Any() == false) &&
                                          ((o.AffiliateId.Any() && o.AffiliateId.Contains(request.sourceMedia)) || o.AffiliateId.Any() == false) &&
                                           ((o.NoOfPaxFrom <= totPax && o.NoOfPaxTo >= totPax)) &&
                                             (o.device == Device.None || o.device == request.device) &&
                                         (o.AffiliateId_Not.Contains(request.sourceMedia) == false) && (o.device == Device.None || o.device == request.device)).ToList().Count == 0)
                                {
                                    Core.Flight.FlightResult result = new Core.Flight.FlightResult()
                                    {
                                        //Fare = new Core.Flight.Fare(),
                                        FareList = new List<Core.Flight.Fare>(),
                                        IsLCC = Itin.sI.FirstOrDefault().fD.aI.isLcc,
                                        //LastTicketDate = Itin.LastTicketDate,
                                        ResultIndex = "tji" + Ctr++,
                                        FlightSegments = new List<Core.Flight.FlightSegment>(),
                                        //Source = Itin.Source,
                                        //TicketAdvisory = Itin.TicketAdvisory,
                                        //ValidatingAirline = Itin.sI.FirstOrDefault().fD.aI.code,
                                        cabinClass = request.cabinType,
                                        gdsType = Core.GdsType.TripJack,
                                        valCarrier = Itin.sI.FirstOrDefault().fD.aI.code,
                                        //Color = Itin.FareClassification.Color,
                                        //fareType = getFareType(Itin.FareClassification.Type),
                                        //groupID = "Tbo" + (itinCtr == 0 ? "OB" : "IB"),
                                        //ffFareType = getFareType(Itin.FareClassification.Type),
                                        ResultCombination = ""
                                    };

                                    #region set Segment
                                    result.FlightSegments = new List<Core.Flight.FlightSegment>();
                                    Core.Flight.FlightSegment fs = new Core.Flight.FlightSegment()
                                    {
                                        Segments = new List<Core.Flight.Segment>(),
                                        SegName = "Return",
                                        Duration = 0,
                                        LayoverTime = 0
                                    };
                                    int segCtr = 0;
                                    foreach (var tjSeg in Itin.sI)
                                    {
                                        Core.Flight.Segment seg = new Core.Flight.Segment()
                                        {
                                            Airline = tjSeg.fD.aI.code,
                                            FlightNumber = tjSeg.fD.fN,
                                            equipmentType = tjSeg.fD.eT,
                                            Origin = tjSeg.da.code,
                                            Destination = tjSeg.aa.code,
                                            DepTime = tjSeg.dt,
                                            ArrTime = tjSeg.at,
                                            CabinClass = request.cabinType,
                                            Duration = tjSeg.duration,
                                            FareClass = Itin.totalPriceList[0].fd.ADULT.cB,
                                            FromTerminal = tjSeg.da.terminal,
                                            ToTerminal = tjSeg.aa.terminal,
                                            IsETicketEligible = true,
                                            layOverTime = 0,
                                            OperatingCarrier = (tjSeg.oB != null && !string.IsNullOrEmpty(tjSeg.oB.code) && (tjSeg.fD.aI.code != tjSeg.oB.code)) ? tjSeg.oB.code : tjSeg.fD.aI.code,
                                            id = tjSeg.id.ToString()
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


                                        fs.Duration += tjSeg.duration;
                                        fs.Segments.Add(seg);
                                        fs.stop++;
                                        segCtr++;
                                    }
                                    result.FlightSegments.Add(fs);
                                    #endregion

                                    #region setAmount
                                    foreach (var tpl in Itin.totalPriceList)
                                    {
                                        Core.Flight.Fare fare = new Core.Flight.Fare()
                                        {
                                            SeatAvailable = tpl.fd.ADULT.sR,
                                            FareType = getFareType(tpl.fareIdentifier, result.valCarrier),
                                            mojoFareType = Core.FlightUtility.GetFmFareType(tpl.fareIdentifier, result.valCarrier, GdsType.TripJack),
                                            tjID = tpl.id,
                                            fareBreakdown = new List<Core.Flight.FareBreakdown>(),
                                            Currency = "INR",
                                            cabinType = getCabinType(tpl.fd.ADULT.cc),
                                            refundType = (Core.RefundType)tpl.fd.ADULT.rT,
                                            baggageInfo = new Core.Flight.BaggageInfo() { checkInBaggage = tpl.fd.ADULT.bI.iB, cabinBaggage = tpl.fd.ADULT.bI.cB },
                                            bookingClass = tpl.fd.ADULT.bI.cB,
                                            sri = tpl.sri,
                                            msri = tpl.msri,
                                            gdsType = GdsType.TripJack,
                                        };
                                        if (fare.mojoFareType == MojoFareType.None || fare.mojoFareType == MojoFareType.Unknown)
                                        {
                                            LogCreater.CreateLogFile(tpl.fareIdentifier + "~" + result.valCarrier, "Log\\FareType", "TripJack" + DateTime.Today.ToString("ddMMyyy") + ".txt");
                                        }
                                        if (request.adults > 0)
                                        {
                                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
                                            {
                                                BaseFare = tpl.fd.ADULT.fC.BF,
                                                Tax = tpl.fd.ADULT.fC.TAF,
                                                CommissionEarned = tpl.fd.ADULT.fC.NCM,
                                                YQTax = tpl.fd.ADULT.afC.TAF.YQ,
                                                PassengerType = Core.PassengerType.Adult,
                                                OtherCharges = tpl.fd.ADULT.afC.TAF.OT,
                                            };
                                            fare.BaseFare += (paxFare.BaseFare * request.adults);
                                            fare.Tax += (paxFare.Tax * request.adults);
                                            fare.CommissionEarned += (paxFare.CommissionEarned * request.adults);
                                            fare.fareBreakdown.Add(paxFare);
                                        }
                                        if (request.child > 0)
                                        {
                                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
                                            {
                                                BaseFare = tpl.fd.CHILD.fC.BF,
                                                Tax = tpl.fd.CHILD.fC.TAF,
                                                CommissionEarned = tpl.fd.CHILD.fC.NCM,
                                                YQTax = tpl.fd.CHILD.afC.TAF.YQ,
                                                PassengerType = Core.PassengerType.Child,
                                                OtherCharges = tpl.fd.CHILD.afC.TAF.OT,
                                            };
                                            fare.BaseFare += (paxFare.BaseFare * request.child);
                                            fare.Tax += (paxFare.Tax * request.child);
                                            fare.CommissionEarned += (paxFare.CommissionEarned * request.child);
                                            fare.fareBreakdown.Add(paxFare);
                                        }
                                        if (request.infants > 0)
                                        {
                                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
                                            {
                                                BaseFare = tpl.fd.INFANT.fC.BF,
                                                Tax = tpl.fd.INFANT.fC.TAF,
                                                CommissionEarned = tpl.fd.INFANT.fC.NCM,
                                                YQTax = tpl.fd.INFANT.afC.TAF.YQ,
                                                PassengerType = Core.PassengerType.Infant,
                                                OtherCharges = tpl.fd.INFANT.afC.TAF.OT,
                                            };
                                            fare.BaseFare += (paxFare.BaseFare * request.infants);
                                            fare.Tax += (paxFare.Tax * request.infants);
                                            fare.CommissionEarned += (paxFare.CommissionEarned * request.infants);
                                            fare.fareBreakdown.Add(paxFare);
                                        }
                                        if (result.FlightSegments[0].Segments[0].Airline == "UK")
                                        {
                                            fare.NetFare = fare.grandTotal = (fare.BaseFare + fare.Tax) - fare.CommissionEarned;
                                        }
                                        else
                                        {
                                            fare.NetFare = fare.grandTotal = (fare.BaseFare + fare.Tax);
                                        }
                                        if (request.cabinType == fare.cabinType)
                                        {
                                            #region BlockAirlines
                                            if (Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.TripJack) &&
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
                                            if (request.segment.Count > 1 && (fare.mojoFareType == MojoFareType.SeriesFareWithPNR || fare.mojoFareType == MojoFareType.SeriesFareWithoutPNR))
                                            {
                                                fare.isBlock = true;
                                            }
                                            result.FareList.Add(fare);

                                            #endregion
                                        }
                                    }
                                    #endregion
                                    listFlightResult.Add(result);
                                }
                            }
                            response.Results.Add(listFlightResult);
                        }
                        #endregion

                        #region Combo fare
                        if (fsr.searchResult.tripInfos.COMBO != null && fsr.searchResult.tripInfos.COMBO.Count > 0)
                        {
                            int Ctr = 0;
                            List<Core.Flight.FlightResult> listFlightResult = new List<Core.Flight.FlightResult>();
                            foreach (TripJackClass.ONWARD Itin in fsr.searchResult.tripInfos.COMBO)
                            {
                                if (Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.TripJack) &&
                                         (o.SiteId == request.siteId) && (o.FareType.Count == 0) && o.airline.Contains(Itin.sI.FirstOrDefault().fD.aI.code) &&
                                         ((o.CountryFrom.Any() && o.CountryFrom.Contains(request.segment[0].orgArp.countryCode)) || o.CountryFrom.Any() == false) &&
                                         ((o.CountryTo.Any() && o.CountryTo.Contains(request.segment[0].destArp.countryCode)) || o.CountryTo.Any() == false) &&
                                         (o.CountryFrom_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                                         (o.CountryTo_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                                         ((o.WeekOfDays.Any() && o.WeekOfDays.Contains((WeekDays)Enum.Parse(typeof(WeekDays), Convert.ToString(DateTime.Today.DayOfWeek)))) || o.WeekOfDays.Any() == false) &&
                                          ((o.AffiliateId.Any() && o.AffiliateId.Contains(request.sourceMedia)) || o.AffiliateId.Any() == false) &&
                                           ((o.NoOfPaxFrom <= totPax && o.NoOfPaxTo >= totPax)) &&
                                             (o.device == Device.None || o.device == request.device) &&
                                         (o.AffiliateId_Not.Contains(request.sourceMedia) == false) && (o.device == Device.None || o.device == request.device)).ToList().Count == 0)
                                {
                                    Core.Flight.FlightResult result = new Core.Flight.FlightResult()
                                    {
                                        //Fare = new Core.Flight.Fare(),
                                        FareList = new List<Core.Flight.Fare>(),
                                        IsLCC = Itin.sI.FirstOrDefault().fD.aI.isLcc,
                                        //LastTicketDate = Itin.LastTicketDate,
                                        ResultIndex = "tjc" + Ctr++,
                                        FlightSegments = new List<Core.Flight.FlightSegment>(),
                                        //Source = Itin.Source,
                                        //TicketAdvisory = Itin.TicketAdvisory,
                                        //ValidatingAirline = Itin.sI.FirstOrDefault().fD.aI.code,
                                        cabinClass = request.cabinType,
                                        gdsType = Core.GdsType.TripJack,
                                        valCarrier = Itin.sI.FirstOrDefault().fD.aI.code,
                                        //Color = Itin.FareClassification.Color,
                                        //fareType = getFareType(Itin.FareClassification.Type),
                                        //groupID = "Tbo" + (itinCtr == 0 ? "OB" : "IB"),
                                        //ffFareType = getFareType(Itin.FareClassification.Type),
                                        ResultCombination = ""
                                    };
                                    result.FlightSegments = new List<Core.Flight.FlightSegment>();
                                    #region set Segment outBound

                                    if (Itin.sI.Where(k => k.isRs == false).ToList().Count > 0)
                                    {
                                        Core.Flight.FlightSegment fs = new Core.Flight.FlightSegment()
                                        {
                                            Segments = new List<Core.Flight.Segment>(),
                                            SegName = "Depart",
                                            Duration = 0,
                                            LayoverTime = 0
                                        };
                                        int segCtr = 0;
                                        foreach (var tjSeg in Itin.sI.Where(k => k.isRs == false))
                                        {
                                            Core.Flight.Segment seg = new Core.Flight.Segment()
                                            {
                                                Airline = tjSeg.fD.aI.code,
                                                FlightNumber = tjSeg.fD.fN,
                                                equipmentType = tjSeg.fD.eT,
                                                Origin = tjSeg.da.code,
                                                Destination = tjSeg.aa.code,
                                                DepTime = tjSeg.dt,
                                                ArrTime = tjSeg.at,
                                                CabinClass = request.cabinType,
                                                Duration = tjSeg.duration,
                                                FareClass = Itin.totalPriceList[0].fd.ADULT.cB,
                                                FromTerminal = tjSeg.da.terminal,
                                                ToTerminal = tjSeg.aa.terminal,
                                                IsETicketEligible = true,
                                                layOverTime = 0,
                                                OperatingCarrier = (tjSeg.oB != null && !string.IsNullOrEmpty(tjSeg.oB.code) && (tjSeg.fD.aI.code != tjSeg.oB.code)) ? tjSeg.oB.code : tjSeg.fD.aI.code,
                                                id = tjSeg.id.ToString()
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


                                            fs.Duration += tjSeg.duration;
                                            fs.stop++;
                                            fs.Segments.Add(seg);
                                            segCtr++;
                                        }
                                        result.FlightSegments.Add(fs);
                                    }
                                    #endregion

                                    #region set Segment inBound

                                    if (Itin.sI.Where(k => k.isRs == true).ToList().Count > 0)
                                    {
                                        Core.Flight.FlightSegment fs = new Core.Flight.FlightSegment()
                                        {
                                            Segments = new List<Core.Flight.Segment>(),
                                            SegName = "Return",

                                            Duration = 0,
                                            LayoverTime = 0
                                        };
                                        int segCtr = 0;
                                        foreach (var tjSeg in Itin.sI.Where(k => k.isRs))
                                        {
                                            Core.Flight.Segment seg = new Core.Flight.Segment()
                                            {
                                                Airline = tjSeg.fD.aI.code,
                                                FlightNumber = tjSeg.fD.fN,
                                                equipmentType = tjSeg.fD.eT,
                                                Origin = tjSeg.da.code,
                                                Destination = tjSeg.aa.code,
                                                DepTime = tjSeg.dt,
                                                ArrTime = tjSeg.at,
                                                CabinClass = request.cabinType,
                                                Duration = tjSeg.duration,
                                                FareClass = Itin.totalPriceList[0].fd.ADULT.cB,
                                                FromTerminal = tjSeg.da.terminal,
                                                ToTerminal = tjSeg.aa.terminal,
                                                IsETicketEligible = true,
                                                layOverTime = 0,
                                                OperatingCarrier = (tjSeg.oB != null && !string.IsNullOrEmpty(tjSeg.oB.code) && (tjSeg.fD.aI.code != tjSeg.oB.code)) ? tjSeg.oB.code : tjSeg.fD.aI.code,
                                                id = tjSeg.id.ToString()
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


                                            fs.Duration += tjSeg.duration;
                                            fs.stop++;
                                            fs.Segments.Add(seg);
                                            segCtr++;
                                        }
                                        result.FlightSegments.Add(fs);
                                    }
                                    #endregion

                                    #region setAmount
                                    foreach (var tpl in Itin.totalPriceList)
                                    {
                                        Core.Flight.Fare fare = new Core.Flight.Fare()
                                        {
                                            SeatAvailable = tpl.fd.ADULT.sR,
                                            FareType = getFareType(tpl.fareIdentifier, result.valCarrier),
                                            mojoFareType = Core.FlightUtility.GetFmFareType(tpl.fareIdentifier, result.valCarrier, GdsType.TripJack),
                                            tjID = tpl.id,
                                            fareBreakdown = new List<Core.Flight.FareBreakdown>(),
                                            Currency = "INR",
                                            cabinType = getCabinType(tpl.fd.ADULT.cc),
                                            refundType = (Core.RefundType)tpl.fd.ADULT.rT,
                                            baggageInfo = new Core.Flight.BaggageInfo() { checkInBaggage = tpl.fd.ADULT.bI.iB, cabinBaggage = tpl.fd.ADULT.bI.cB },
                                            bookingClass = tpl.fd.ADULT.bI.cB,
                                            gdsType = GdsType.TripJack,
                                        };
                                        if (fare.mojoFareType == MojoFareType.None || fare.mojoFareType == MojoFareType.Unknown)
                                        {
                                            LogCreater.CreateLogFile(tpl.fareIdentifier + "~" + result.valCarrier, "Log\\FareType", "TripJack" + DateTime.Today.ToString("ddMMyyy") + ".txt");
                                        }
                                        if (request.adults > 0)
                                        {
                                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
                                            {
                                                BaseFare = tpl.fd.ADULT.fC.BF,
                                                Tax = tpl.fd.ADULT.fC.TAF,
                                                CommissionEarned = tpl.fd.ADULT.fC.NCM,
                                                YQTax = tpl.fd.ADULT.afC.TAF.YQ,
                                                PassengerType = Core.PassengerType.Adult,
                                                OtherCharges = tpl.fd.ADULT.afC.TAF.OT,
                                            };
                                            fare.BaseFare += (paxFare.BaseFare * request.adults);
                                            fare.Tax += (paxFare.Tax * request.adults);
                                            fare.CommissionEarned += (paxFare.CommissionEarned * request.adults);
                                            fare.fareBreakdown.Add(paxFare);
                                        }
                                        if (request.child > 0)
                                        {
                                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
                                            {
                                                BaseFare = tpl.fd.CHILD.fC.BF,
                                                Tax = tpl.fd.CHILD.fC.TAF,
                                                CommissionEarned = tpl.fd.CHILD.fC.NCM,
                                                YQTax = tpl.fd.CHILD.afC.TAF.YQ,
                                                PassengerType = Core.PassengerType.Child,
                                                OtherCharges = tpl.fd.CHILD.afC.TAF.OT,
                                            };
                                            fare.BaseFare += (paxFare.BaseFare * request.child);
                                            fare.Tax += (paxFare.Tax * request.child);
                                            fare.CommissionEarned += (paxFare.CommissionEarned * request.child);
                                            fare.fareBreakdown.Add(paxFare);
                                        }
                                        if (request.infants > 0)
                                        {
                                            Core.Flight.FareBreakdown paxFare = new Core.Flight.FareBreakdown()
                                            {
                                                BaseFare = tpl.fd.INFANT.fC.BF,
                                                Tax = tpl.fd.INFANT.fC.TAF,
                                                CommissionEarned = tpl.fd.INFANT.fC.NCM,
                                                YQTax = tpl.fd.INFANT.afC.TAF.YQ,
                                                PassengerType = Core.PassengerType.Infant,

                                                OtherCharges = tpl.fd.INFANT.afC.TAF.OT,
                                            };
                                            fare.BaseFare += (paxFare.BaseFare * request.infants);
                                            fare.Tax += (paxFare.Tax * request.infants);
                                            fare.CommissionEarned += (paxFare.CommissionEarned * request.infants);
                                            fare.fareBreakdown.Add(paxFare);
                                        }
                                        if (result.FlightSegments[0].Segments[0].Airline == "UK")
                                        {
                                            fare.NetFare = fare.grandTotal = (fare.BaseFare + fare.Tax) - fare.CommissionEarned;
                                        }
                                        else
                                        {
                                            fare.NetFare = fare.grandTotal = (fare.BaseFare + fare.Tax);
                                        }
                                        if (request.cabinType == fare.cabinType)
                                        {
                                            #region BlockAirlines
                                            if (Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.TripJack) &&
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
                                            if (request.segment.Count > 1 && (fare.mojoFareType == MojoFareType.SeriesFareWithPNR || fare.mojoFareType == MojoFareType.SeriesFareWithoutPNR))
                                            {
                                                fare.isBlock = true;
                                            }
                                            result.FareList.Add(fare);
                                            #endregion
                                        }
                                    }
                                    #endregion

                                    listFlightResult.Add(result);
                                }
                            }
                            response.Results.Add(listFlightResult);
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            { }
        }
        public Core.FareType getFareType(string fType)
        {
            Core.FareType fareType = Core.FareType.NONE;
            switch (fType.ToUpper())
            {
                case "PUBLISHEDFARE": fareType = Core.FareType.PUBLISH; break;
                case "PUBLISHED": fareType = Core.FareType.PUBLISH; break;
                case "SME": fareType = Core.FareType.PROMO; break;
                case "CORPORATE": fareType = Core.FareType.CORPORATE; break;
                case "FLEXIFARE": fareType = Core.FareType.CORPORATE; break;
                case "COUPON": fareType = Core.FareType.PROMO; break;
                case "FAMILY": fareType = Core.FareType.FAMILYFARE; break;
                case "TACTICAL": fareType = Core.FareType.TACTICAL; break;
                case "PREMIUM": fareType = Core.FareType.PREMIUM; break;
                case "GOMORE": fareType = Core.FareType.GOMORE; break;
                case "CORPORATEFLEX": fareType = Core.FareType.CORPORATEFLEX; break;
                case "FLEXI_PLUS": fareType = Core.FareType.FLEXIPLUS; break;
                case "PREMIUM_FLEX": fareType = Core.FareType.CORPORATE; break;
                case "GOMOREFARE": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "OFFERFARE": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "INSTANTOFFERFARE": fareType = Core.FareType.INSTANTPUR; break;
                case "STANDARD": fareType = Core.FareType.STANDARD; break;
                case "CORP_CONNECT": fareType = Core.FareType.CORPORATE; break;
                case "OFFER_FARE_WITH_PNR": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "OFFER_FARE_WITHOUT_PNR": fareType = Core.FareType.OFFER_FARE_WITHOUT_PNR; break;
                case "PROMO": fareType = Core.FareType.PROMO; break;
                case "SALE": fareType = Core.FareType.PUBLISH; break;
                case "EXTRA": fareType = Core.FareType.EXTRA; break;
                case "LITE": fareType = Core.FareType.LITE; break;
                case "VALUE": fareType = Core.FareType.VALUE; break;
                case "BUSINESS": fareType = Core.FareType.BUSINESS; break;
                case "EXPRESS_VALUE": fareType = Core.FareType.EXPRESS_VALUE; break;
                case "VAL": fareType = Core.FareType.VAL; break;
                case "FLX": fareType = Core.FareType.FLX; break;
                case "SUPSAV": fareType = Core.FareType.SUPSAV; break;
                case "BIGLITE": fareType = Core.FareType.BIGLITE; break;
                case "BIGEASY": fareType = Core.FareType.BIGEASY; break;
                case "FLY": fareType = Core.FareType.FLY; break;
                case "SCOOTPLUS": fareType = Core.FareType.SCOOTPLUS; break;
                case "EXPRESS_FLEXI": fareType = Core.FareType.EXPRESS_FLEXI; break;
                case "COMFORT": fareType = Core.FareType.PROMO; break;
                case "OFFER_RETURN_FARE_WITH_PNR": fareType = Core.FareType.OFFER_RETURN_FARE_WITH_PNR; break;
                case "EXPRESS_MIXED": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "OFFER_RETURN_FARE_WITHOUT_PNR": fareType = Core.FareType.OFFER_RETURN_FARE_WITHOUT_PNR; break;
                case "LIGHT": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "CORPORATE_GOMORE": fareType = Core.FareType.CORPORATE; break;
                case "SOTO": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "VISTA_FLEX": fareType = Core.FareType.CORPORATE; break;
                case "FLEXIBLE": fareType = Core.FareType.FLEXIBLE; break;
                case "LATITUDE": fareType = Core.FareType.LATITUDE; break;
                case "ECONOMY FLEXI": fareType = Core.FareType.FLEXI; break;
                case "ECONOMY BASIC": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECONOMY SEMI FLEX": fareType = Core.FareType.FLEX; break;
                case "ECONOMY VALUE": fareType = Core.FareType.VALUE; break;
                case "ECONOMY FREEDOM": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "PREMIUM ECONOMY": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "PREMIUM ECONOMY FULLY FLEX": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "PREMIUM ECONOMY SAVER": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "PREMIUM ECONOMY BASE": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "PREMIUM ECONOMY FLEX": fareType = Core.FareType.FLEX; break;
                case "PREMIUM ECONOMY LOWEST": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "PREMIUM ECONOMY FLEXIBLE": fareType = Core.FareType.FLEXIBLE; break;
                case "ECOFLEXPROMOD2": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECOFLEX1": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECOFLEXPROMOD1": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "STANDARD ECONOMY": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECONOMY FULLY FLEX": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECONOMY CONVENIENCE": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECONOMY COMFORT": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECONOMY FLEX": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECONOMY FLEX PLUS": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECONOMY CHOICE": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECONOMY CHOICE PLUS": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECONOMY SAVER": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECONOMY BASE": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECO STANDARD": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECO VALUE": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECO FLEXI": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECONOMY ESSENTIAL": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECONOMY CLASSIC": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "RESTRICTED": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECOFLEX3": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECOFULL": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECONOMY SMART": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "EXPRESS VALUE": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECONOMY SELECT PRO": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECO FLEX": fareType = Core.FareType.FLEX; break;
                case "EXPRESS FLEXI": fareType = Core.FareType.FLEXI; break;
                case "SPECIAL": fareType = Core.FareType.PUBLISH; break;
                case "ECONOMY SELECT": fareType = Core.FareType.SPECIAL; break;
                case "ECO SEMI FLEX": fareType = Core.FareType.SPECIAL; break;
                case "ECOFLEX2": fareType = Core.FareType.FLEX; break;
                case "ECO BASIC": fareType = Core.FareType.SPECIAL; break;
                case "ECONOMY STANDARD": fareType = Core.FareType.SPECIAL; break;
                case "ECONOMY LITE": fareType = Core.FareType.LITE; break;
                case "PREMIUM ECONOMY FLEXI": fareType = Core.FareType.FLEXI; break;
                case "PREMIUM ECONOMY SELECT PRO": fareType = Core.FareType.SPECIAL; break;
                case "PREMIUM ECONOMY SELECT": fareType = Core.FareType.SPECIAL; break;
                case "PEY VALUE": fareType = Core.FareType.VALUE; break;
                case "PEY FLEXI": fareType = Core.FareType.FLEXI; break;
                case "PEY STANDARD": fareType = Core.FareType.SPECIAL; break;
                case "ECOSAVE1": fareType = Core.FareType.SPECIAL; break;
                case "ECO": fareType = Core.FareType.SPECIAL; break;
                case "SUPER SAVER": fareType = Core.FareType.PUBLISH; break;
                case "SPECIAL_PRIVATE_FARE": fareType = Core.FareType.PROMO; break;
                case "ECOFLEXPROMOC2": fareType = Core.FareType.FLEX; break;
                case "ECONOMY SALE": fareType = Core.FareType.SPECIAL; break;
                case "ECONOMY HAND BAGGAGE ONLY": fareType = Core.FareType.SPECIAL; break;
                case "ECOFLEXPROMOD3": fareType = Core.FareType.FLEX; break;
                case "ECONOMY SPECIAL": fareType = Core.FareType.SPECIAL; break;
                case "ECOFLEXPROMOC3": fareType = Core.FareType.FLEX; break;
                case "ECOFLEXPROMOC1": fareType = Core.FareType.FLEX; break;
                case "PREMIUM ECONOMY FLEX PLUS": fareType = Core.FareType.FLEX; break;
                case "FIRST FLEX": fareType = Core.FareType.FLEX; break;
                case "FIRST FLEX PLUS": fareType = Core.FareType.FLEX; break;
                case "FIRST": fareType = Core.FareType.SPECIAL; break;
                case "PREMIUM ECONOMY ESSENTIAL": fareType = Core.FareType.SPECIAL; break;
                case "ECONOMY PROMO RESTRICTED": fareType = Core.FareType.SPECIAL; break;
                case "PREMIUM ECONOMY STANDARD": fareType = Core.FareType.SPECIAL; break;
                case "COMFORT PLUS": fareType = Core.FareType.COMFORT; break;
                case "BUSINESS COMFORT PLUS": fareType = Core.FareType.COMFORT; break;
                case "PREMIUM ECONOMY VALUE": fareType = Core.FareType.ECONOMY; break;
                case "BUSINESS FLEX": fareType = Core.FareType.FLEX; break;
                case "BUSINESS FLEXI": fareType = Core.FareType.FLEX; break;
                case "BUSINESS VALUE": fareType = Core.FareType.VALUE; break;
                case "ECONOMY": fareType = Core.FareType.ECONOMY; break;
                case "ECONOMY BEST OFFER": fareType = Core.FareType.ECONOMY; break;
                case "ECONOMY BEST BUY": fareType = Core.FareType.ECONOMY; break;
                case "ECONOMY GOOD DEAL": fareType = Core.FareType.ECONOMY; break;
                case "ECONOMY PLUS": fareType = Core.FareType.ECONOMY; break;
                case "BUSINESS SAVER": fareType = Core.FareType.BUSINESS; break;
                case "BUSINESS TITANIUM": fareType = Core.FareType.BUSINESS; break;
                case "BUS SEMI FLEX": fareType = Core.FareType.FLEX; break;
                case "BUS FLEX": fareType = Core.FareType.FLEX; break;


                case "NONE": fareType = Core.FareType.TACTICAL; break;
            }
            if (fareType == Core.FareType.NONE)
            {
                LogCreater.CreateLogFile(fType + Environment.NewLine, "Log\\TripJack\\error", DateTime.Today.ToString("ddMMyyy") + ".txt");
            }
            return fareType;
        }
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


        public Core.FareType getFareType(string fType, string airline)
        {
            Core.FareType fareType = Core.FareType.NONE;
            switch (fType.ToUpper())
            {
                case "PUBLISHEDFARE": fareType = Core.FareType.PUBLISH; break;
                case "FLEXIFARE": fareType = Core.FareType.CORPORATE; break;


                case "PREMIUM": fareType = Core.FareType.PREMIUM; break;
                case "GOMORE": fareType = Core.FareType.GOMORE; break;
                case "CORPORATEFLEX": fareType = Core.FareType.CORPORATEFLEX; break;
                case "PREMIUM_FLEX": fareType = Core.FareType.FLEXI; break;
                case "GOMOREFARE": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "OFFERFARE": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "INSTANTOFFERFARE": fareType = Core.FareType.INSTANTPUR; break;
                case "STANDARD": fareType = Core.FareType.STANDARD; break;
                case "CORP_CONNECT": fareType = Core.FareType.CORPORATE; break;

                case "EXTRA": fareType = Core.FareType.PUBLISH; break;
                case "LITE": fareType = Core.FareType.LITE; break;
                case "VALUE": fareType = Core.FareType.VALUE; break;
                case "BUSINESS": fareType = Core.FareType.BUSINESS; break;
                case "EXPRESS_VALUE": fareType = Core.FareType.EXPRESS_VALUE; break;
                case "VAL": fareType = Core.FareType.VAL; break;
                case "FLX": fareType = Core.FareType.FLX; break;
                case "SUPSAV": fareType = Core.FareType.SUPSAV; break;
                case "BIGLITE": fareType = Core.FareType.BIGLITE; break;
                case "BIGEASY": fareType = Core.FareType.BIGEASY; break;
                case "FLY": fareType = Core.FareType.FLY; break;
                case "SCOOTPLUS": fareType = Core.FareType.SCOOTPLUS; break;
                case "EXPRESS_FLEXI": fareType = Core.FareType.EXPRESS_FLEXI; break;
                case "OFFER_RETURN_FARE_WITH_PNR": fareType = Core.FareType.OFFER_RETURN_FARE_WITH_PNR; break;
                case "EXPRESS_MIXED": fareType = Core.FareType.PUBLISH; break;
                case "OFFER_RETURN_FARE_WITHOUT_PNR": fareType = Core.FareType.OFFER_RETURN_FARE_WITHOUT_PNR; break;
                case "LIGHT": fareType = Core.FareType.PUBLISH; break;
                case "CORPORATE_GOMORE": fareType = Core.FareType.CORPORATE; break;
                case "PREMIUM ECONOMY": fareType = Core.FareType.PUBLISH; break;
                case "PREMIUM ECONOMY FULLY FLEX": fareType = Core.FareType.PUBLISH; break;
                case "PREMIUM ECONOMY SAVER": fareType = Core.FareType.PUBLISH; break;
                case "PREMIUM ECONOMY BASE": fareType = Core.FareType.PUBLISH; break;
                case "PREMIUM ECONOMY FLEX": fareType = Core.FareType.PUBLISH; break;
                case "PREMIUM ECONOMY LOWEST": fareType = Core.FareType.PUBLISH; break;
                case "ECOFLEXPROMOD2": fareType = Core.FareType.PROMO; break;
                case "ECOFLEX1": fareType = Core.FareType.PUBLISH; break;
                case "ECOFLEXPROMOD1": fareType = Core.FareType.PUBLISH; break;
                case "STANDARD ECONOMY": fareType = Core.FareType.PUBLISH; break;
                case "RESTRICTED": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECOFLEX3": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECOFULL": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "EXPRESS VALUE": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECONOMY SELECT PRO": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "ECO FLEX": fareType = Core.FareType.FLEX; break;
                case "EXPRESS FLEXI": fareType = Core.FareType.FLEXI; break;
                case "SPECIAL": fareType = Core.FareType.PUBLISH; break;
                case "ECONOMY SELECT": fareType = Core.FareType.SPECIAL; break;
                case "ECO SEMI FLEX": fareType = Core.FareType.SPECIAL; break;
                case "ECOFLEX2": fareType = Core.FareType.FLEX; break;
                case "ECO BASIC": fareType = Core.FareType.SPECIAL; break;
                case "PREMIUM ECONOMY SELECT PRO": fareType = Core.FareType.SPECIAL; break;
                case "PREMIUM ECONOMY SELECT": fareType = Core.FareType.SPECIAL; break;
                case "ECOSAVE1": fareType = Core.FareType.SPECIAL; break;
                case "ECO": fareType = Core.FareType.SPECIAL; break;





                // AI fare Type
                case "BUSINESS BASIC": fareType = Core.FareType.BUSINESS; break;
                case "BUSINESS CHOICE PLUS": fareType = Core.FareType.BUSINESS; break;
                case "BUSINESS COMFORT PLUS": fareType = Core.FareType.BUSINESS; break;
                case "BUSINESS FLEX": fareType = Core.FareType.BUSINESS; break;
                case "PRIVATE": fareType = Core.FareType.PROMO; break;
                case "BUSINESS FLEX PLUS": fareType = Core.FareType.BUSINESS; break;
                case "BUSINESS FLEXI": fareType = Core.FareType.BUSINESS; break;
                case "BUSINESS FLEXIBLE": fareType = Core.FareType.BUSINESS; break;
                case "BUSINESS FREEDOM": fareType = Core.FareType.BUSINESS; break;
                case "BUSINESS SAVER": fareType = Core.FareType.BUSINESS; break;
                case "BUSINESS SEMI FLEX": fareType = Core.FareType.BUSINESS; break;
                case "BUSINESS STANDARD": fareType = Core.FareType.BUSINESS; break;
                case "BUSINESS VALUE": fareType = Core.FareType.BUSINESS; break;
                case "CCL FLEXI": fareType = Core.FareType.FLEXI; break;
                case "CCL STANDARD": fareType = Core.FareType.PUBLISH; break;
                case "CCL VALUE": fareType = Core.FareType.PUBLISH; break;
                case "COMFORT": fareType = Core.FareType.PROMO; break;
                case "COMFORT PLUS": fareType = Core.FareType.PUBLISH; break;
                case "CORPORATE": fareType = Core.FareType.CORPORATE; break;
                case "ECO FLEXI": fareType = Core.FareType.FLEXI; break;
                case "ECO LITE": fareType = Core.FareType.PUBLISH; break;
                case "ECO STANDARD": fareType = Core.FareType.PUBLISH; break;
                case "ECO VALUE": fareType = Core.FareType.PUBLISH; break;
                case "PUBLISH": fareType = Core.FareType.PUBLISH; break;
                case "ECONOMY": fareType = Core.FareType.PUBLISH; break;
                case "ECONOMY BASE": fareType = Core.FareType.PUBLISH; break;
                case "ECONOMY BASIC": fareType = Core.FareType.PUBLISH; break;
                case "ECONOMY CHOICE": fareType = Core.FareType.PUBLISH; break;
                case "ECONOMY CHOICE PLUS": fareType = Core.FareType.PUBLISH; break;
                case "ECONOMY CLASSIC": fareType = Core.FareType.PUBLISH; break;
                case "ECONOMY COMFORT": fareType = Core.FareType.PUBLISH; break;
                case "ECONOMY CONVENIENCE": fareType = Core.FareType.PUBLISH; break;
                case "ECONOMY ESSENTIAL": fareType = Core.FareType.PUBLISH; break;
                case "ECONOMY FLEX": fareType = Core.FareType.FLEXI; break;
                case "ECONOMY FLEX PLUS": fareType = Core.FareType.FLEXI; break;
                case "ECONOMY FLEXI": fareType = Core.FareType.FLEXI; break;
                case "ECONOMY FREEDOM": fareType = Core.FareType.PUBLISH; break;
                case "ECONOMY LIGHT": fareType = Core.FareType.PUBLISH; break;
                case "ECONOMY LITE": fareType = Core.FareType.PUBLISH; break;
                case "ECONOMY SAVER": fareType = Core.FareType.PUBLISH; break;
                case "ECONOMY SEMI FLEX": fareType = Core.FareType.FLEXI; break;
                case "ECONOMY SMART": fareType = Core.FareType.PUBLISH; break;
                case "ECONOMY STANDARD": fareType = Core.FareType.PUBLISH; break;
                case "ECONOMY VALUE": fareType = Core.FareType.PUBLISH; break;
                case "FALCON GOLD FLEX": fareType = Core.FareType.BUSINESS; break;
                case "FALCON GOLD SMART": fareType = Core.FareType.BUSINESS; break;
                case "FIRST": fareType = Core.FareType.BUSINESS; break;
                case "FIRST FLEX": fareType = Core.FareType.BUSINESS; break;
                case "FIRST FLEX PLUS": fareType = Core.FareType.BUSINESS; break;
                case "FLEX": fareType = Core.FareType.FLEXI; break;
                case "FLEXI": fareType = Core.FareType.FLEXI; break;
                case "FLEXI SAVER ECONOMY": fareType = Core.FareType.FLEXI; break;
                case "FLEXIBLE": fareType = Core.FareType.FLEXI; break;
                case "FLEXIBLE ECONOMY": fareType = Core.FareType.FLEXI; break;
                case "LATITUDE": fareType = Core.FareType.PUBLISH; break;
                case "OFFER_FARE_WITH_PNR": fareType = Core.FareType.OFFER_FARE_WITH_PNR; break;
                case "OFFER_FARE_WITHOUT_PNR": fareType = Core.FareType.OFFER_FARE_WITHOUT_PNR; break;
                case "PEY FLEXI": fareType = Core.FareType.FLEXI; break;
                case "PEY STANDARD": fareType = Core.FareType.PUBLISH; break;
                case "PEY VALUE": fareType = Core.FareType.PUBLISH; break;
                case "PREMIUM ECONOMY FLEXI": fareType = Core.FareType.FLEXI; break;
                case "PREMIUM ECONOMY FLEXIBLE": fareType = Core.FareType.FLEXI; break;
                case "PREMIUM ECONOMY STANDARD": fareType = Core.FareType.PUBLISH; break;
                case "PREMIUM ECONOMY VALUE": fareType = Core.FareType.PUBLISH; break;
                case "PUBLISHED": fareType = Core.FareType.PUBLISH; break;
                case "SME": fareType = Core.FareType.SME; break;
                case "SOTO": fareType = Core.FareType.PUBLISH; break;
                case "SPECIAL_RETURN": fareType = Core.FareType.SPECIALRETURN; break;
                case "SUPER FLEXIBLE ": fareType = Core.FareType.BUSINESS; break;
                case "SUPER VALUE BUSINESS": fareType = Core.FareType.BUSINESS; break;
                case "SUPER VALUE ECONOMY": fareType = Core.FareType.PUBLISH; break;
                case "WEB SPECIAL BUSINESS": fareType = Core.FareType.BUSINESS; break;
                case "WEB SPECIAL ECONOMY": fareType = Core.FareType.PUBLISH; break;

                // I5 fare Type
                case "VISTA_FLEX": fareType = Core.FareType.FLEXI; break;
                case "Xpress_Value": fareType = Core.FareType.PUBLISH; break;
                case "FAMILY": fareType = Core.FareType.FAMILYFARE; break;
                case "PROMO": fareType = Core.FareType.PROMO; break;

                // 6E fare Type

                case "TACTICAL": fareType = Core.FareType.PROMO; break;
                case "SME.CRPCON": fareType = Core.FareType.SME; break;
                case "SAVER": fareType = Core.FareType.PUBLISH; break;
                case "SUPER6E": fareType = Core.FareType.PUBLISH; break;
                case "COUPON": fareType = Core.FareType.PROMO; break;
                case "SALE": fareType = Core.FareType.SALE; break;
                case "FLEXI_PLUS": fareType = Core.FareType.FLEXI; break;

                    //NDC Fare




                case "NONE": fareType = Core.FareType.NONE; break;
            }
            LogCreater.CreateLogFile(fType + "_" + airline + Environment.NewLine, "Log\\TripJack\\fare", DateTime.Today.ToString("ddMMyyy") + ".txt");
            if (fareType == Core.FareType.NONE)
            {
                LogCreater.CreateLogFile(fType + "_" + airline + Environment.NewLine, "Log\\TripJack\\error", DateTime.Today.ToString("ddMMyyy") + ".txt");
            }
            return fareType;
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
