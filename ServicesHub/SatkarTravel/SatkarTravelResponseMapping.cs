using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.SatkarTravel
{
    public class SatkarTravelResponseMapping
    {
        public void getResults(Core.Flight.FlightSearchRequest request, ref SatkarTravelClass.FlightResponse fsr, ref Core.Flight.FlightSearchResponseShort response)
        {
            int totPax = request.adults + request.child + request.infants;
            if (fsr.error.errorCode == 0 || fsr.responseStatus == 1)
            {


                foreach (List<SatkarTravelClass.ResultsItem> listItin in fsr.results)
                {
                    int itinCtr = 0;
                    List<Core.Flight.FlightResult> listFlightResult = new List<Core.Flight.FlightResult>();
                    foreach (SatkarTravelClass.ResultsItem Itin in listItin)
                    {
                        if (Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.SatkarTravel) &&
                      (o.SiteId == request.siteId) && (o.FareType.Count == 0) && o.airline.Contains(Itin.airlineCode) &&
                      ((o.CountryFrom.Any() && o.CountryFrom.Contains(request.segment[0].orgArp.countryCode)) || o.CountryFrom.Any() == false) &&
                      ((o.CountryTo.Any() && o.CountryTo.Contains(request.segment[0].destArp.countryCode)) || o.CountryTo.Any() == false) &&
                      (o.CountryFrom_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                      (o.CountryTo_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                      ((o.WeekOfDays.Any() && o.WeekOfDays.Contains((WeekDays)Enum.Parse(typeof(WeekDays), Convert.ToString(DateTime.Today.DayOfWeek)))) || o.WeekOfDays.Any() == false) &&
                      ((o.AffiliateId.Any() && o.AffiliateId.Contains(request.sourceMedia)) || o.AffiliateId.Any() == false) &&
                      ((o.NoOfPaxFrom <= totPax && o.NoOfPaxTo >= totPax)) &&
                      (o.AffiliateId_Not.Contains(request.sourceMedia) == false)&& (o.device == Device.None || o.device == request.device)).ToList().Count == 0)
                        {
                            //if (Itin.segments.FirstOrDefault().FirstOrDefault().noOfSeatAvailable >= request.adults + request.child)
                            //{
                            Core.Flight.FlightResult result = new Core.Flight.FlightResult()
                            {
                                AirlineRemark = Itin.airlineRemark,
                                Fare = new Core.Flight.Fare(),
                                IsLCC = true,
                                IsRefundable = false,
                                LastTicketDate = DateTime.Today.AddDays(1),
                                ResultIndex = "Sat" + itinCtr++,
                                FlightSegments = new List<Core.Flight.FlightSegment>(),
                                Source = 0,
                                TicketAdvisory = "",
                                cabinClass = request.cabinType,
                                gdsType = Core.GdsType.SatkarTravel,
                                valCarrier = Itin.airlineCode,
                                Color = "",
                                ffFareType = FareType.OFFERFARE,
                                FareList = new List<Core.Flight.Fare>()
                            };
                            #region set flight segment

                            Core.Flight.FlightSegment fs = new Core.Flight.FlightSegment()
                            {
                                Segments = new List<Core.Flight.Segment>(),
                                Duration = 0,
                                stop = 0,
                                LayoverTime = 0,
                                SegName = "Depart"
                            };

                            Core.Flight.Segment segment = new Core.Flight.Segment()
                            {
                                Airline = Itin.airlineCode,
                                ArrTime = DateTime.ParseExact(Itin.segments.FirstOrDefault().FirstOrDefault().destination.arrTime, "yyyy-MM-ddTHH:mm:ss", new System.Globalization.CultureInfo("en-US")),
                                DepTime = DateTime.ParseExact(Itin.segments.FirstOrDefault().FirstOrDefault().origin.depTime, "yyyy-MM-ddTHH:mm:ss", new System.Globalization.CultureInfo("en-US")),
                                Origin = Itin.segments.FirstOrDefault().FirstOrDefault().origin.airport.airportCode,
                                Destination = Itin.segments.FirstOrDefault().FirstOrDefault().destination.airport.airportCode,
                                Duration = 0,
                                FareClass = "",
                                FlightNumber = Itin.segments.FirstOrDefault().FirstOrDefault().airline.flightNumber,
                                FromTerminal = "",
                                ToTerminal = "",
                                IsETicketEligible = true,
                                OperatingCarrier = Itin.airlineCode,
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

                            Core.Flight.Fare fare = new Core.Flight.Fare()
                            {
                                Currency = request.currencyCode,
                                cgstax = Itin.fare.cgstax,
                                igstax = Itin.fare.igstax,
                                sgstax = Itin.fare.sgstax,
                                BaseFare = Itin.fare.baseFare,
                                Tax = Itin.fare.tax,
                                flat = Itin.fare.flat,
                                YQTax = Itin.fare.yQTax,
                                additionalTxnFeeOfrd = Itin.fare.additionalTxnFeeOfrd,
                                additionalTxnFeePub = Itin.fare.additionalTxnFeePub,
                                PGCharge = Itin.fare.pGCharge,
                                artGST = Itin.fare.artGST,
                                artGSTOnMFee = Itin.fare.artGSTOnMFee,
                                artTDS = Itin.fare.artTDS,
                                OtherCharges = Itin.fare.otherCharges,
                                Discount = Itin.fare.discount,
                                PublishedFare = Itin.fare.publishedFare,
                                CommissionEarned = Itin.fare.commissionEarned,
                                pLBEarned = Itin.fare.pLBEarned,
                                incentiveEarned = Itin.fare.incentiveEarned,
                                artIncentive = Itin.fare.artIncentive,
                                OfferedFare = Itin.fare.offeredFare,
                                TdsOnCommission = Itin.fare.tdsOnCommission,
                                TdsOnPLB = Itin.fare.tdsOnPLB,
                                TdsOnIncentive = Itin.fare.tdsOnIncentive,
                                ServiceFee = Itin.fare.serviceFee,
                                totalBaggageCharges = Itin.fare.totalBaggageCharges,
                                totalMealCharges = Itin.fare.totalMealCharges,
                                totalSeatCharges = Itin.fare.totalSeatCharges,
                                totalSpecialServiceCharges = Itin.fare.totalSpecialServiceCharges,
                                transactionFee = Itin.fare.transactionFee,
                                managementFee = Itin.fare.managementFee,
                                cGSTax = Itin.fare.cGSTax,
                                sGSTax = Itin.fare.sGSTax,
                                iGSTax = Itin.fare.iGSTax,
                                feeSurcharges = Itin.fare.feeSurcharges,
                                instantDiscontOnFare = Itin.fare.instantDiscontOnFare,
                                publishedFareSoldInSwapping = Itin.fare.publishedFareSoldInSwapping,
                                taxSoldInSwapping = Itin.fare.taxSoldInSwapping,
                                baseFareSoldInSwapping = Itin.fare.baseFareSoldInSwapping,
                                instantDiscountSoldInSwapping = Itin.fare.instantDiscountSoldInSwapping,
                                yqSoldInSwapping = Itin.fare.yqSoldInSwapping,
                                isFareSwapped = false,
                                Markup = 0,
                                cabinType = request.cabinType,
                                gdsType = GdsType.SatkarTravel,
                                SeatAvailable = Itin.segments.FirstOrDefault().FirstOrDefault().noOfSeatAvailable,
                                FareType = FareType.OFFER_FARE_WITH_PNR,
                                ST_ResultSessionID = Itin.resultSessionId
                            };

                            fare.mojoFareType = MojoFareType.SeriesFareWithPNR;

                            fare.fareBreakdown = new List<Core.Flight.FareBreakdown>();
                            #region set fare Breakup

                            fare.fareBreakdown = new List<Core.Flight.FareBreakdown>();
                            Core.Flight.FareBreakdown adtFare = new Core.Flight.FareBreakdown();
                            var adtBreakDown = Itin.fareBreakdown.Where(k => k.passengerType == 1).ToList();
                            adtFare.BaseFare = adtBreakDown[0].baseFare;
                            adtFare.Tax = adtBreakDown[0].tax;
                            adtFare.AdditionalTxnFeeOfrd = adtBreakDown[0].additionalTxnFeeOfrd;
                            adtFare.AdditionalTxnFeePub = adtBreakDown[0].additionalTxnFeePub;
                            adtFare.PGCharge = adtBreakDown[0].pGCharge;
                            adtFare.YQTax = adtBreakDown[0].yQTax;
                            adtFare.PassengerType = Core.PassengerType.Adult;
                            fare.fareBreakdown.Add(adtFare);
                            if (request.child > 0)
                            {
                                Core.Flight.FareBreakdown chdFare = new Core.Flight.FareBreakdown();
                                var chdBreakDown = Itin.fareBreakdown.Where(k => k.passengerType == 2).ToList();
                                chdFare.BaseFare = chdBreakDown[0].baseFare;
                                chdFare.Tax = chdBreakDown[0].tax;
                                chdFare.AdditionalTxnFeeOfrd = chdBreakDown[0].additionalTxnFeeOfrd;
                                chdFare.AdditionalTxnFeePub = chdBreakDown[0].additionalTxnFeePub;
                                chdFare.PGCharge = chdBreakDown[0].pGCharge;
                                chdFare.YQTax = chdBreakDown[0].yQTax;
                                chdFare.PassengerType = Core.PassengerType.Child;
                                fare.fareBreakdown.Add(chdFare);
                            }
                            if (request.infants > 0)
                            {
                                Core.Flight.FareBreakdown infFare = new Core.Flight.FareBreakdown();
                                var infBreakDown = Itin.fareBreakdown.Where(k => k.passengerType == 3).ToList();
                                infFare.BaseFare = infBreakDown[0].baseFare;
                                infFare.Tax = infBreakDown[0].tax;
                                infFare.AdditionalTxnFeeOfrd = infBreakDown[0].additionalTxnFeeOfrd;
                                infFare.AdditionalTxnFeePub = infBreakDown[0].additionalTxnFeePub;
                                infFare.PGCharge = infBreakDown[0].pGCharge;
                                infFare.YQTax = infBreakDown[0].yQTax;
                                infFare.PassengerType = Core.PassengerType.Infant;
                                fare.fareBreakdown.Add(infFare);
                            }

                            #endregion
                            fare.NetFare = fare.grandTotal = fare.PublishedFare + fare.Markup - fare.CommissionEarned;
                            if (request.cabinType == fare.cabinType)
                            {
                                #region BlockAirlines
                                if (Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.SatkarTravel) &&
                                             (o.SiteId == request.siteId) && (o.FareType.Any() && o.FareType.Contains(fare.mojoFareType)) &&
                                             ((o.airline.Any() && o.airline.Contains(result.valCarrier)) || o.airline.Any() == false) &&
                                             ((o.CountryFrom.Any() && o.CountryFrom.Contains(request.segment[0].orgArp.countryCode)) || o.CountryFrom.Any() == false) &&
                                             ((o.CountryTo.Any() && o.CountryTo.Contains(request.segment[0].destArp.countryCode)) || o.CountryTo.Any() == false) &&
                                             (o.CountryFrom_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                                             (o.CountryTo_Not.Contains(request.segment[0].orgArp.countryCode) == false) &&
                                             ((o.WeekOfDays.Any() && o.WeekOfDays.Contains((WeekDays)Enum.Parse(typeof(WeekDays), Convert.ToString(DateTime.Today.DayOfWeek)))) || o.WeekOfDays.Any() == false) &&
                                              ((o.AffiliateId.Any() && o.AffiliateId.Contains(request.sourceMedia)) || o.AffiliateId.Any() == false) &&
                                                   (o.AffiliateId_Not.Contains(request.sourceMedia) == false) &&
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
                            //  }
                        }
                    }
                    itinCtr++;
                    response.Results.Add(listFlightResult);
                }
            }
            else
            {
                response.Results.Add(new List<Core.Flight.FlightResult>());
            }
        }


        public void getFareQuoteResponse(ref Core.Flight.PriceVerificationRequest request,
           ref ST_FareQuote.FareQuoteResponse fqr, ref Core.Flight.FareQuoteResponse response, int ctr)
        {
            if (fqr.results.fare.publishedFare > request.flightResult[ctr].Fare.PublishedFare)
            {
                response.flightResult.Add(request.flightResult[ctr]);
                response.VerifiedTotalPrice = fqr.results.fare.publishedFare;
                response.fareIncreaseAmount += fqr.results.fare.publishedFare - request.flightResult[ctr].Fare.PublishedFare;
                response.STSessionID = fqr.results.resultSessionId;
                if (response.fareIncreaseAmount > 0)
                {
                    response.isFareChange = true;
                }
            }
            else
            {
                response.fareIncreaseAmount = 0;
                response.VerifiedTotalPrice = request.flightResult[ctr].Fare.PublishedFare;
                response.STSessionID = fqr.results.resultSessionId;
            }
        }
    }
}