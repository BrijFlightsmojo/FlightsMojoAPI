using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tripshope
{
    public class TripshopeResponseMapping
    {
        public void getResults(Core.Flight.FlightSearchRequest request, ref TripshopeClass.SearchResponse fsr, ref Core.Flight.FlightSearchResponseShort response)
        {
            int totPax = request.adults + request.child + request.infants;
            if (fsr.flightsearchresponse.totalresults > 0)
            {
                int itinCtr = 0;
                int ctrError = 0;

                foreach (var listItin in fsr.flightsearchresponse.flightjourneys)
                {
                    List<Core.Flight.FlightResult> flightReslut = new List<Core.Flight.FlightResult>();
                    foreach (var item in listItin.flightoptions)
                    {
                        var flightlist = item.recommendedflight.FirstOrDefault();

                        if (flightReslut != null && Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.TripShope) &&
                       (o.SiteId == request.siteId) && (o.FareType.Count == 0) && o.airline.Contains(flightlist.flightlegs[0].carrier) &&
                       ((o.CountryFrom.Any() && o.CountryFrom.Contains(request.segment[0].orgArp.countryCode)) || o.CountryFrom.Any() == false) &&
                       ((o.CountryTo.Any() && o.CountryTo.Contains(request.segment[0].destArp.countryCode)) || o.CountryTo.Any() == false) &&
                       (o.CountryFrom_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                       (o.CountryTo_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                       ((o.WeekOfDays.Any() && o.WeekOfDays.Contains((WeekDays)Enum.Parse(typeof(WeekDays), Convert.ToString(DateTime.Today.DayOfWeek)))) || o.WeekOfDays.Any() == false) &&
                       ((o.AffiliateId.Any() && o.AffiliateId.Contains(request.sourceMedia)) || o.AffiliateId.Any() == false) &&
                       ((o.NoOfPaxFrom <= totPax && o.NoOfPaxTo >= totPax)) &&
                       (o.AffiliateId_Not.Contains(request.sourceMedia) == false) &&
                       (o.device == Device.None || o.device == request.device)).ToList().Count == 0)
                        {
                            //if (flightlist.flightlegs.>= request.adults + request.child)
                            //{

                            //}


                            Core.Flight.FlightResult result = new Core.Flight.FlightResult()
                            {
                                Fare = new Core.Flight.Fare(),
                                IsLCC = flightlist.islcc,
                                IsRefundable = false,
                                LastTicketDate = DateTime.Today.AddDays(1),
                                ResultIndex = "TS" + itinCtr++,
                                FlightSegments = new List<Core.Flight.FlightSegment>(),
                                Source = 0,
                                TicketAdvisory = "",
                                cabinClass = request.cabinType,
                                gdsType = Core.GdsType.TripShope,
                                valCarrier = flightlist.flightlegs[0].validatingcarrier,
                                Color = "",
                                ffFareType = getFareType((flightlist.flightfare.faretype != null ? flightlist.flightfare.faretype : ""),(string.IsNullOrEmpty(flightlist.flightfare.refundableinfo)?true:Convert.ToBoolean(flightlist.flightfare.refundableinfo))),
                                FareList = new List<Core.Flight.Fare>()

                            };
                            Core.Flight.FlightSegment fs = new Core.Flight.FlightSegment()
                            {
                                Segments = new List<Core.Flight.Segment>(),
                                Duration = 0,
                                stop = 0,
                                LayoverTime = 0,
                                SegName = "Depart"
                            };
                            int layCtr = 1;
                            //string CabinClass = string.Empty;
                            foreach (TripshopeClass.Flightleg fseg in flightlist.flightlegs)
                            {
                                Core.Flight.Segment segment = new Core.Flight.Segment()
                                {
                                    Airline = fseg.validatingcarrier,
                                    DepTime = DateTime.ParseExact(fseg.depdate + " " + fseg.deptime, "yyyy-MM-dd HHmm", new System.Globalization.CultureInfo("en-US")),
                                    ArrTime = DateTime.ParseExact(fseg.arrdate + " " + fseg.arrtime, "yyyy-MM-dd HHmm", new System.Globalization.CultureInfo("en-US")),
                                    Origin = fseg.origin,
                                    Destination = fseg.destination,
                                    Duration = flightlist.totaljourneyduration,/*flightlist.totaljourneyduration*/ /*fseg.journeyduration*/
                                    FareClass = GetAirlineClass(request.cabinType),
                                    FlightNumber = fseg.flightnumber,
                                    FromTerminal = fseg.depterminal,
                                    ToTerminal = fseg.arrterminal,
                                    IsETicketEligible = true,
                                    OperatingCarrier = fseg.carriername,
                                    SegmentIndicator = 0,
                                    equipmentType = fseg.equipment,
                                    //CabinClass = request.cabinType,
                                    CabinClass = GetCabinType(fseg.cabinclass, request.cabinType),
                                };
                                result.ResultCombination += (segment.Airline + segment.FlightNumber + segment.DepTime.ToString("ddMMHHmm"));
                                // CabinClass = fseg.cabinclass;

                                #region LayOverTime
                                //if (fseg.Count > layCtr)
                                //{
                                //    TimeSpan ts = fseg[layCtr].Origin.DepTime - segment.ArrTime;
                                //    if (ts.Hours > 0 || ts.Minutes > 0)
                                //    {
                                //        if (ts.Hours > 0 && ts.Minutes > 0)
                                //        {
                                //            segment.layOverTime = (ts.Hours * 60) + ts.Minutes;
                                //        }
                                //        else if (ts.Hours > 0)
                                //        {
                                //            segment.layOverTime = ts.Hours * 60;
                                //        }
                                //        else if (ts.Minutes > 0)
                                //        {
                                //            segment.layOverTime = ts.Minutes;
                                //        }
                                //    }
                                //    else
                                //    {
                                //        segment.layOverTime = 0;
                                //    }
                                //    fs.LayoverTime += segment.layOverTime;
                                //}
                                //layCtr++;
                                #endregion

                                fs.stop++;
                                fs.Duration = segment.Duration++;
                                fs.Segments.Add(segment);
                            }
                            result.FlightSegments.Add(fs);


                            #region set flight fare

                            Core.Flight.Fare fare = new Core.Flight.Fare()
                            {
                                flightdeeplinkurl = flightlist.flightdeeplinkurl,
                                BaseFare = flightlist.flightfare.totalbasefare,
                                Currency = request.currencyCode,
                                Tax = flightlist.flightfare.totaltax,
                                YQTax = flightlist.flightfare.totalyq,
                                PublishedFare = flightlist.flightfare.totalnet,
                                Discount = flightlist.flightfare.discount,
                                artTDS = flightlist.flightfare.tds,
                                Markup = flightlist.flightfare.markup,
                                ServiceFee = flightlist.flightfare.servicefee + flightlist.flightfare.servicetax + flightlist.flightfare.handlingcharges,
                                pLBEarned = flightlist.flightfare.plb,
                                transactionFee = flightlist.flightfare.transactionfee,
                                FareType = getFareType((flightlist.flightfare.faretype != null ? flightlist.flightfare.faretype : ""), (string.IsNullOrEmpty(flightlist.flightfare.refundableinfo) ? true : Convert.ToBoolean(flightlist.flightfare.refundableinfo))),
                                mojoFareType = Core.FlightUtility.GetFmFareType(flightlist.flightfare.faretype != null ? flightlist.flightfare.faretype : "", result.valCarrier, GdsType.TripShope),
                                cabinType = result.FlightSegments[0].Segments[0].CabinClass,
                                gdsType = Core.GdsType.TripShope,
                                nextracustomstr = flightlist.nextracustomstr,
                                nextraflightkey = flightlist.nextraflightkey,
                           };
                            if (fare.mojoFareType == MojoFareType.None || fare.mojoFareType == MojoFareType.Unknown)
                            {
                                LogCreater.CreateLogFile(flightlist.flightfare.faretype + "~" + result.valCarrier, "Log\\FareType", "TS" + DateTime.Today.ToString("ddMMyyy") + ".txt");
                            }
                            #endregion

                            fare.fareBreakdown = new List<Core.Flight.FareBreakdown>();
                            #region set fare Breakup

                            fare.fareBreakdown = new List<Core.Flight.FareBreakdown>();
                            Core.Flight.FareBreakdown adtFare = new Core.Flight.FareBreakdown();

                            adtFare.BaseFare = flightlist.flightfare.adultbasefare;
                            adtFare.Tax = flightlist.flightfare.adulttax;
                            adtFare.YQTax = flightlist.flightfare.adultyq;
                            adtFare.PassengerType = Core.PassengerType.Adult;
                            fare.fareBreakdown.Add(adtFare);

                            if (request.child > 0)
                            {
                                Core.Flight.FareBreakdown chdFare = new Core.Flight.FareBreakdown();

                                chdFare.BaseFare = flightlist.flightfare.childbasefare;
                                chdFare.Tax = flightlist.flightfare.childtax;
                                chdFare.YQTax = flightlist.flightfare.childyq;
                                chdFare.PassengerType = Core.PassengerType.Child;
                                fare.fareBreakdown.Add(chdFare);
                            }
                            if (request.infants > 0)
                            {
                                Core.Flight.FareBreakdown infFare = new Core.Flight.FareBreakdown();

                                infFare.BaseFare = flightlist.flightfare.infantbasefare;
                                infFare.Tax = flightlist.flightfare.infanttax;
                                infFare.YQTax = flightlist.flightfare.infantyq;
                                infFare.PassengerType = Core.PassengerType.Infant;
                                fare.fareBreakdown.Add(infFare);
                            }

                            #endregion
                            fare.NetFare = fare.grandTotal = fare.PublishedFare;
                            if (request.cabinType == fare.cabinType)
                            {
                                #region BlockAirlines
                                if (Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.TripShope) &&
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
                            flightReslut.Add(result);
                            itinCtr++;
                        }
                    }
                    response.Results.Add(flightReslut);
                }
            }
        }

    

        public Core.FareType getFareType(string type, bool refundable)
        {
            Core.FareType ft = Core.FareType.NONE;
            if (!string.IsNullOrEmpty(type))
            {
                switch (type.ToLower())
                {
                    case "normal": ft = Core.FareType.PUBLISH; break;
                    case "special fares": ft = Core.FareType.INSTANTPUR; break;
                    case "saver": ft = Core.FareType.SAVER; break;
                    case "flexi": ft = Core.FareType.FLEXI; break;
                    case "sme": ft = Core.FareType.SME; break;
                    default: ft = (refundable ?Core.FareType.PUBLISH : Core.FareType.INSTANTPUR ); break;
                }
            }
            if (ft == Core.FareType.NONE)
            {
                LogCreater.CreateLogFile(type + Environment.NewLine, "Log\\FareType", "TS" + DateTime.Today.ToString("ddMMyyy") + ".txt");
            }
            return ft;
        }
        public Core.FareType getFareType(string type)
        {
            Core.FareType ft = Core.FareType.NONE;
            if (!string.IsNullOrEmpty(type))
            {
                switch (type.ToLower())
                {
                    case "normal": ft = Core.FareType.PUBLISH; break;
                    case "special fares": ft = Core.FareType.INSTANTPUR; break;
                    case "saver": ft = Core.FareType.SAVER; break;
                    case "flexi": ft = Core.FareType.FLEXI; break;
                    case "sme": ft = Core.FareType.SME; break;
                    case "regular": ft = Core.FareType.PUBLISH; break;
                    case "smefares": ft = Core.FareType.SME; break;
                    case "commindicator": ft = Core.FareType.PUBLISH; break;
                    case "roundtrip": ft = Core.FareType.PUBLISH; break;
                    case "special": ft = Core.FareType.INSTANTPUR; break;
                    case "family": ft = Core.FareType.PUBLISH; break;
                    case "fixed fare": ft = Core.FareType.PUBLISH; break;
                    case "regular Fare": ft = Core.FareType.PUBLISH; break;
                    case "coupon Fare": ft = Core.FareType.PUBLISH; break;
                    case "corporate": ft = Core.FareType.PUBLISH; break;
                    case "retail": ft = Core.FareType.PUBLISH; break;
                    case "special fare": ft = Core.FareType.PUBLISH; break;
                    case "regular fare": ft = Core.FareType.PUBLISH; break;
                    case "coupon fare": ft = Core.FareType.PUBLISH; break;
                    default: ft = Core.FareType.NONE; break;
                }
            }
            if (ft == Core.FareType.NONE)
            {
                LogCreater.CreateLogFile(type + Environment.NewLine, "Log\\FareType", "TS" + DateTime.Today.ToString("ddMMyyy") + ".txt");
            }
            return ft;
        }

        public string GetAirlineClass(CabinType ct)
        {

            if ((int)ct == 2)
            {
                return "M";
            }
            else if ((int)ct == 3)
            {
                return "C";
            }
            else if ((int)ct == 4)
            {
                return "F";
            }
            else
                return "Y";
        }




        public CabinType GetCabinType(string type, CabinType ct)
        {

            if (!string.IsNullOrEmpty(type))
            {
                switch (type.ToLower())
                {
                    case "economy": return Core.CabinType.Economy; break;
                    case "business": return Core.CabinType.Business; break;
                    case "first": return Core.CabinType.First; break;
                    case "premiumeconomy": return Core.CabinType.PremiumEconomy; break;
                    case "premium": return Core.CabinType.PremiumEconomy; break;
                    default: return ct; break;
                }
            }
            else
            {
                return ct;
            }


        }


        public void getFareQuoteResponse(ref Core.Flight.PriceVerificationRequest request, ref GetFareQuoteResponse.Root fqr, ref Core.Flight.FareQuoteResponse response, int ctr)
        {
            try
            {
                response.flightResult.Add(request.flightResult[ctr]);
                response.BookingKey = fqr.NextraPricingResponseV4.bookingkey;
                request.Bookingkey = fqr.NextraPricingResponseV4.bookingkey;

                if (fqr.NextraPricingResponseV4.pricechanged)
                {
                    response.isFareChange = true;
                    response.VerifiedTotalPrice = fqr.NextraPricingResponseV4.flightjourneys[0].flightoptions[0].recommendedflight[0].flightfare.totalnet;
                }
                if (fqr.NextraPricingResponseV4.statuscode == 200)
                {
                    response.VerifiedTotalPrice = fqr.NextraPricingResponseV4.flightjourneys[0].flightoptions[0].recommendedflight[0].flightfare.totalnet;

                    decimal diff = fqr.NextraPricingResponseV4.flightjourneys[0].flightoptions[0].recommendedflight[0].flightfare.totalnet - request.flightResult[ctr].Fare.PublishedFare;

                    if (diff > 0)
                    {
                        response.isFareChange = true;
                        response.fareIncreaseAmount += diff;
                    }
                }
                else
                {
                    response.responseStatus.status = Core.TransactionStatus.Error;
                    //response.responseStatus.message = fqr.NextraPricingResponseV4.Error.ErrorCode + ":" + fqr.Response.Error.ErrorMessage;
                    //response.ErrorCode = fqr.Response.Error.ErrorCode;
                    response.isRunFareQuoteFalse = true;
                }
            }
            catch
            {
                response.isRunFareQuoteFalse = true;
            }
        }
    }
}

