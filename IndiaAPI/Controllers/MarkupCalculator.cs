using Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace IndiaAPI.Controllers
{
    public class MarkupCalculator
    {

        Random rnd = new Random( );
       


        public void SetMarkup(ref Core.Flight.FlightSearchRequest fsr, ref Core.Flight.FlightSearchResponse flightSearchResponse)
        {
            StringBuilder sbLogger = new StringBuilder();
            //int eftCtr = 0;

            TravelType travelType = TravelType.International;
            if (fsr.segment[0].orgArp == null)
            {
                fsr.segment[0].orgArp = FlightUtility.GetAirport(fsr.segment[0].originAirport);
            }
            if (fsr.segment[0].destArp == null)
            {
                fsr.segment[0].destArp = FlightUtility.GetAirport(fsr.segment[0].destinationAirport);
            }
            if (fsr.segment[0].orgArp.countryCode == "IN" && fsr.segment[0].destArp.countryCode == "IN")
            {
                travelType = TravelType.Domestic;
            }
            int totpax = (fsr.adults + fsr.child + fsr.infants);
            List<Core.Markup.skyScannerMetaRankData> metaData = new List<Core.Markup.skyScannerMetaRankData>();
            //List<Core.Markup.FlightMarkupNew> lstMarkup = new DAL.Markup.MarkupTransaction().getFlightMarkupNew((int)fsr.cabinType, ((int)travelType), fsr.sourceMedia);
            List<Core.Markup.FlightMarkupNew> lstMarkup = new DAL.Markup.MarkupTransaction().getFlightMarkupWithSkyScanner((int)fsr.cabinType, ((int)travelType), fsr.sourceMedia, 
                fsr.segment[0].originAirport, fsr.segment[0].destinationAirport, fsr.segment[0].travelDate,fsr.device, ref metaData,totpax);

          
            if (flightSearchResponse != null && flightSearchResponse.Results != null && flightSearchResponse.Results.Count() > 0
                && flightSearchResponse.Results[0].Count > 0 && flightSearchResponse.Results.LastOrDefault().Count > 0)
            {
                foreach (var res in flightSearchResponse.Results)
                {
                    foreach (var item in res)
                    {
                        if (item != null)
                        {
                            List<Core.Markup.skyScannerMetaRankData> md = metaData.Where(x => x.flightNo.Equals(item.FlightSegments[0].Segments[0].FlightNumber) && x.Airline.Equals(item.FlightSegments[0].Segments[0].Airline)).ToList();
                            foreach (var itemFare in item.FareList)
                            {
                                itemFare.scComprefare = 0;
                                

                                if (md.Count > 0 && (fsr.sourceMedia == "1015" || (itemFare.gdsType == GdsType.FareBoutique)) && item.FlightSegments[0].Segments.Count==1)
                                {
                                    decimal totFare = md[0].Amount;
                                    totFare = (totFare * (fsr.adults + fsr.child)) + (1500 * fsr.infants);
                                      decimal diff = totFare - (itemFare.grandTotal-(itemFare.CommissionEarned+itemFare.pLBEarned));
                                   // decimal diff = totFare - itemFare.grandTotal;
                                    if (diff > 100)
                                    {
                                        int num = rnd.Next(1, 10);
                                        itemFare.scComprefare = md[0].Amount;
                                        itemFare.Markup = diff - num;
                                        itemFare.markupID = "=>RankingMarkup: " + itemFare.Markup + " Diff=>" + diff;
                                        itemFare.grandTotal = (itemFare.BaseFare + itemFare.Tax + itemFare.OtherCharges + itemFare.ServiceFee + itemFare.ConvenienceFee + itemFare.Markup);
                                    }
                                }

                                if (itemFare.scComprefare == 0)
                                {
                                    if (lstMarkup.Count > 0)
                                    {
                                        StringBuilder sb = new StringBuilder();
                                        #region setMarkup
                                        if (item.valCarrier == "I5" && itemFare.mojoFareType==MojoFareType.SeriesFareWithPNR)
                                        {

                                        }
                                        var extractMarkup = lstMarkup.Where(x =>
                                        ((x.Airline.Any() && x.Airline.Contains(item.FlightSegments[0].Segments[0].Airline)) || x.Airline.Any() == false) &&
                                        (x.AirlineNot.Contains(item.FlightSegments[0].Segments[0].Airline) == false) &&
                                        ((x.FmFareType.Any() && x.FmFareType.Contains(itemFare.mojoFareType)) || x.FmFareType.Any() == false) &&
                                        ((x.GdsType == (int)itemFare.gdsType) || (x.GdsType == (int)GdsType.None)) &&
                                        ((x.SubProvider.Any() && itemFare.subProvider != SubProvider.None && x.SubProvider.Contains(itemFare.subProvider)) || x.SubProvider.Any() == false)
                                        );
                                        if (extractMarkup.Any())
                                        {
                                            var markup = extractMarkup.FirstOrDefault();
                                            if (markup.AmountType == 1)
                                            {
                                                itemFare.Markup = markup.Amount * totpax;
                                                itemFare.markupID = markup.RuleName + "=>TotalMakup: " + itemFare.Markup + "";
                                                itemFare.grandTotal = (itemFare.BaseFare + itemFare.Tax + itemFare.OtherCharges + itemFare.ServiceFee + itemFare.ConvenienceFee + itemFare.Markup);
                                            }
                                            else
                                            {
                                                itemFare.grandTotal = (itemFare.BaseFare + itemFare.Tax + itemFare.OtherCharges + itemFare.ServiceFee + itemFare.ConvenienceFee);
                                                itemFare.Markup = Math.Round((itemFare.grandTotal * markup.Amount) / 100, 2);
                                                itemFare.grandTotal = (itemFare.BaseFare + itemFare.Tax + itemFare.OtherCharges + itemFare.ServiceFee + itemFare.ConvenienceFee + itemFare.Markup);
                                                itemFare.markupID = markup.RuleName + "=>TotalMakup: " + itemFare.Markup + "";
                                            }
                                        }
                                        else
                                        {
                                            itemFare.grandTotal = (itemFare.BaseFare + itemFare.Tax + itemFare.OtherCharges + itemFare.ServiceFee + itemFare.ConvenienceFee);
                                            itemFare.markupID = "NoRule";
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        itemFare.grandTotal = (itemFare.BaseFare + itemFare.Tax + itemFare.OtherCharges + itemFare.ServiceFee + itemFare.ConvenienceFee);
                                        itemFare.markupID = "NoRule";
                                    }
                                }
                            }
                            #region Set Airline and Airport Library
                            foreach (var fs in item.FlightSegments)
                            {
                                foreach (var seg in fs.Segments)
                                {
                                    if (flightSearchResponse.airline.Where(o => o.code.Equals(seg.Airline, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
                                    {
                                        flightSearchResponse.airline.Add(Core.FlightUtility.GetAirline(seg.Airline));
                                    }
                                    if (flightSearchResponse.airline.Where(o => o.code.Equals(seg.OperatingCarrier, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
                                    {
                                        flightSearchResponse.airline.Add(Core.FlightUtility.GetAirline(seg.OperatingCarrier));
                                    }
                                    if (flightSearchResponse.airport.Where(o => o.airportCode.Equals(seg.Origin, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
                                    {
                                        flightSearchResponse.airport.Add(Core.FlightUtility.GetAirport(seg.Origin));
                                    }

                                    if (flightSearchResponse.airport.Where(o => o.airportCode.Equals(seg.Destination, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
                                    {
                                        flightSearchResponse.airport.Add(Core.FlightUtility.GetAirport(seg.Destination));
                                    }
                                }
                            }


                            #endregion

                            if (item.Fare == null)
                            {
                                item.Fare = item.FareList.OrderBy(k => k.grandTotal).FirstOrDefault();
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }

        }
        public void SetNoMarkup(ref Core.Flight.FlightSearchRequest fsr, ref Core.Flight.FlightSearchResponse flightSearchResponse)
        {


            if (flightSearchResponse != null && flightSearchResponse.Results != null && flightSearchResponse.Results.Count() > 0
                && flightSearchResponse.Results[0].Count > 0 && flightSearchResponse.Results.LastOrDefault().Count > 0)
            {
                foreach (var res in flightSearchResponse.Results)
                {
                    foreach (var item in res)
                    {
                        if (item != null)
                        {

                            #region Set Airline and Airport Library
                            foreach (var fs in item.FlightSegments)
                            {
                                foreach (var seg in fs.Segments)
                                {
                                    if (flightSearchResponse.airline.Where(o => o.code.Equals(seg.Airline, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
                                    {
                                        flightSearchResponse.airline.Add(Core.FlightUtility.GetAirline(seg.Airline));
                                    }
                                    if (flightSearchResponse.airline.Where(o => o.code.Equals(seg.OperatingCarrier, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
                                    {
                                        flightSearchResponse.airline.Add(Core.FlightUtility.GetAirline(seg.OperatingCarrier));
                                    }
                                    if (flightSearchResponse.airport.Where(o => o.airportCode.Equals(seg.Origin, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
                                    {
                                        flightSearchResponse.airport.Add(Core.FlightUtility.GetAirport(seg.Origin));
                                    }

                                    if (flightSearchResponse.airport.Where(o => o.airportCode.Equals(seg.Destination, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
                                    {
                                        flightSearchResponse.airport.Add(Core.FlightUtility.GetAirport(seg.Destination));
                                    }
                                }
                            }


                            #endregion

                            if (item.Fare == null)
                            {
                                item.Fare = item.FareList.OrderBy(k => k.grandTotal).FirstOrDefault();
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }

        }
        public void SetMarkupOld(ref Core.Flight.FlightSearchRequest fsr, ref Core.Flight.FlightSearchResponse flightSearchResponse)
        {
            StringBuilder sbLogger = new StringBuilder();
            //int eftCtr = 0;

            TravelType travelType = TravelType.International;
            if (fsr.segment[0].orgArp == null)
            {
                fsr.segment[0].orgArp = FlightUtility.GetAirport(fsr.segment[0].originAirport);
            }
            if (fsr.segment[0].destArp == null)
            {
                fsr.segment[0].destArp = FlightUtility.GetAirport(fsr.segment[0].destinationAirport);
            }
            if (fsr.segment[0].orgArp.countryCode == "IN" && fsr.segment[0].destArp.countryCode == "IN")
            {
                travelType = TravelType.Domestic;
            }
            List<Core.Markup.FlightMarkupNew> lstMarkup = new DAL.Markup.MarkupTransaction().getFlightMarkupNew((int)fsr.cabinType, ((int)travelType), fsr.sourceMedia);

            int totpax = (fsr.adults + fsr.child + fsr.infants);
            if (flightSearchResponse != null && flightSearchResponse.Results != null && flightSearchResponse.Results.Count() > 0
                && flightSearchResponse.Results[0].Count > 0 && flightSearchResponse.Results.LastOrDefault().Count > 0)
            {
                foreach (var res in flightSearchResponse.Results)
                {
                    foreach (var item in res)
                    {
                        if (item != null)
                        {
                            foreach (var itemFare in item.FareList)
                            {


                                if (lstMarkup.Count > 0)
                                {
                                    StringBuilder sb = new StringBuilder();
                                    #region setMarkup
                                    if (item.valCarrier == "SG" || item.FlightSegments[0].Segments[0].Airline == "SG")
                                    {

                                    }
                                    var extractMarkup = lstMarkup.Where(x =>
                                    ((x.Airline.Any() && x.Airline.Contains(item.FlightSegments[0].Segments[0].Airline)) || x.Airline.Any() == false) &&
                                    (x.AirlineNot.Contains(item.FlightSegments[0].Segments[0].Airline) == false) &&
                                    ((x.FmFareType.Any() && x.FmFareType.Contains(itemFare.mojoFareType)) || x.FmFareType.Any() == false) &&
                                    ((x.GdsType == (int)itemFare.gdsType) || (x.GdsType == (int)GdsType.None)) &&
                                    ((x.SubProvider.Any() && itemFare.subProvider != SubProvider.None && x.SubProvider.Contains(itemFare.subProvider)) || x.SubProvider.Any() == false)
                                    );
                                    if (extractMarkup.Any())
                                    {
                                        var markup = extractMarkup.FirstOrDefault();
                                        if (markup.AmountType == 1)
                                        {
                                            itemFare.Markup = markup.Amount * totpax;
                                            itemFare.markupID = markup.RuleName + "=>TotalMakup: " + itemFare.Markup + "";
                                            itemFare.grandTotal = (itemFare.BaseFare + itemFare.Tax + itemFare.OtherCharges + itemFare.ServiceFee + itemFare.ConvenienceFee + itemFare.Markup);
                                        }
                                        else
                                        {
                                            itemFare.grandTotal = (itemFare.BaseFare + itemFare.Tax + itemFare.OtherCharges + itemFare.ServiceFee + itemFare.ConvenienceFee);
                                            itemFare.Markup = Math.Round((itemFare.grandTotal * markup.Amount) / 100, 2);
                                            itemFare.grandTotal = (itemFare.BaseFare + itemFare.Tax + itemFare.OtherCharges + itemFare.ServiceFee + itemFare.ConvenienceFee + itemFare.Markup);
                                            itemFare.markupID = markup.RuleName + "=>TotalMakup: " + itemFare.Markup + "";
                                        }
                                    }
                                    else
                                    {
                                        itemFare.grandTotal = (itemFare.BaseFare + itemFare.Tax + itemFare.OtherCharges + itemFare.ServiceFee + itemFare.ConvenienceFee);
                                        itemFare.markupID = "NoRule";
                                    }
                                    #endregion
                                }
                                else
                                {
                                    itemFare.grandTotal = (itemFare.BaseFare + itemFare.Tax + itemFare.OtherCharges + itemFare.ServiceFee + itemFare.ConvenienceFee);
                                    itemFare.markupID = "NoRule";
                                }
                            }
                            #region Set Airline and Airport Library
                            foreach (var fs in item.FlightSegments)
                            {
                                foreach (var seg in fs.Segments)
                                {
                                    if (flightSearchResponse.airline.Where(o => o.code.Equals(seg.Airline, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
                                    {
                                        flightSearchResponse.airline.Add(Core.FlightUtility.GetAirline(seg.Airline));
                                    }
                                    if (flightSearchResponse.airline.Where(o => o.code.Equals(seg.OperatingCarrier, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
                                    {
                                        flightSearchResponse.airline.Add(Core.FlightUtility.GetAirline(seg.OperatingCarrier));
                                    }
                                    if (flightSearchResponse.airport.Where(o => o.airportCode.Equals(seg.Origin, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
                                    {
                                        flightSearchResponse.airport.Add(Core.FlightUtility.GetAirport(seg.Origin));
                                    }

                                    if (flightSearchResponse.airport.Where(o => o.airportCode.Equals(seg.Destination, StringComparison.OrdinalIgnoreCase)).ToList().Count == 0)
                                    {
                                        flightSearchResponse.airport.Add(Core.FlightUtility.GetAirport(seg.Destination));
                                    }
                                }
                            }


                            #endregion

                            if (item.Fare == null)
                            {
                                item.Fare = item.FareList.OrderBy(k => k.grandTotal).FirstOrDefault();
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }

        }


        public bool checkAirline(List<string> segAirline, List<string> airline, AirlineMatchType airlineMatchType)
        {
            bool returnVal = true;
            var NotMatchAirline = segAirline.Except(airline, StringComparer.OrdinalIgnoreCase).ToList();
            if (airlineMatchType == AirlineMatchType.ExactMatch)
            {
                returnVal = NotMatchAirline.Count == 0;
            }
            else if (airlineMatchType == AirlineMatchType.ConatinsAny)
            {
                returnVal = segAirline.Count > NotMatchAirline.Count;
            }
            else if (airlineMatchType == AirlineMatchType.DoesNotContain)
            {
                returnVal = segAirline.Count == NotMatchAirline.Count;
            }
            return returnVal;
        }

        public bool checkOptrAirline(List<string> segOptrAirline, List<string> airline, CheckOperatedBy checkOperatedBy)
        {
            bool returnVal = true;
            if (checkOperatedBy != CheckOperatedBy.None)
            {
                if (segOptrAirline.Count == airline.Count)
                {
                    for (int i = 0; i < segOptrAirline.Count; i++)
                    {
                        if (!segOptrAirline[i].Equals(airline[i], StringComparison.OrdinalIgnoreCase))
                        {
                            returnVal = false;
                        }
                    }
                }
                else
                {
                    returnVal = false;
                }
            }
            if (returnVal == false)
            {

            }
            return returnVal;
        }

        public bool checkAirlineClass(List<string> segAirlineClass, List<string> airlineClass, AirlineClassMatchType airlineClassMatchType)
        {
            bool returnVal = true;
            var NotMatchAirlineClass = segAirlineClass.Except(airlineClass, StringComparer.OrdinalIgnoreCase).ToList();
            if (airlineClassMatchType == AirlineClassMatchType.ExactMatch)
            {
                returnVal = NotMatchAirlineClass.Count == 0;
            }
            else if (airlineClassMatchType == AirlineClassMatchType.ContainsAny)
            {
                returnVal = segAirlineClass.Count > NotMatchAirlineClass.Count;
            }
            return returnVal;
        }

    }
}