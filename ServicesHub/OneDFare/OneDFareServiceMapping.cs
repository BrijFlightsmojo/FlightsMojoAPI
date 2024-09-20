using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Core.Flight;
using System.Configuration;
using Core;
using Newtonsoft.Json;

namespace ServicesHub.OneDFare
{
    public class OneDFareServiceMapping
    {


        public FlightSearchResponseShort GetFlightResults(FlightSearchRequest request)
        {
            int totPax = request.adults + request.child + request.infants; 
            string errorMsg = string.Empty;
            FlightSearchResponseShort flightResponse = new FlightSearchResponseShort(request);

            StringBuilder sbLogger = new StringBuilder();

            System.Data.DataSet ds = new DAL.OneDFare.DalOneDFare().getOneDResult(request.segment[0].originAirport, request.segment[0].destinationAirport, request.segment[0].travelDate, request.adults + request.child);


            if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                int itinCtr = 155;
                List<Core.Flight.FlightResult> listFlightResult = new List<Core.Flight.FlightResult>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    if (Convert.ToInt32(ds.Tables[0].Rows[0]["NoOfSeat"].ToString()) >= request.adults + request.child)
                    {
                        Core.Flight.FlightResult result = new Core.Flight.FlightResult()
                        {
                            AirlineRemark = "",
                            Fare = new Core.Flight.Fare(),
                            IsLCC = true,
                            IsRefundable = false,
                            LastTicketDate = DateTime.Today.AddDays(1),
                            ResultIndex = "OD" + itinCtr,
                            FlightSegments = new List<Core.Flight.FlightSegment>(),
                            Source = 0,
                            TicketAdvisory = "",
                            cabinClass = request.cabinType,
                            gdsType = Core.GdsType.OneDFare,
                            valCarrier = dr["AirlineCode"].ToString(),
                            Color = "",
                            ffFareType = FareType.OFFERFARE,
                            FareList = new List<Core.Flight.Fare>()
                        };

                        #region set flight segment

                        string Airline = string.Empty;

                        Core.Flight.FlightSegment fs = new Core.Flight.FlightSegment() { Segments = new List<Core.Flight.Segment>(), Duration = 0, stop = 0, LayoverTime = 0, SegName = "Depart" };
                        //DateTime.ParseExact(FormColl["departure_date3"].ToString(), "yyyy-MM-dd", new CultureInfo("en-US"));
                        Core.Flight.Segment segment = new Core.Flight.Segment()
                        {
                            Airline = dr["AirlineCode"].ToString(),
                            ArrTime = Convert.ToDateTime(dr["ArrivalDate"]),
                            DepTime = Convert.ToDateTime(dr["DepartureDate"]),
                            Origin = dr["Origin"].ToString(),
                            Destination = dr["Destination"].ToString(),
                            Duration = 0,
                            FareClass = "",
                            FlightNumber = dr["FlightNo"].ToString(),
                            FromTerminal = dr["DepartureTerminal"].ToString(),
                            ToTerminal = dr["ArrvialTerminal"].ToString(),
                            IsETicketEligible = true,
                            OperatingCarrier = dr["AirlineCode"].ToString(),
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

                        decimal UnitFare = Convert.ToDecimal(dr["UnitFare"]);
                        Core.Flight.Fare fare = new Core.Flight.Fare()
                        {
                            FB_flight_id = Convert.ToInt32(dr["Id"]),
                            FB_static = "",
                            BaseFare = (UnitFare * 0.75m * (request.adults + request.child)) + (request.infants > 0 ? (1000 * request.infants) : 0),
                            Tax = (UnitFare * 0.25m * (request.adults + request.child)) + (request.infants > 0 ? (500 * request.infants) : 0),
                            Currency = request.currencyCode,
                            Markup = 0,
                            PublishedFare = (UnitFare * (request.adults + request.child)) + (request.infants > 0 ? (1500 * request.infants) : 0),
                            NetFare = (UnitFare * (request.adults + request.child)) + (request.infants > 0 ? (1500 * request.infants) : 0),
                            FareType = FareType.OFFER_FARE_WITH_PNR,
                            cabinType = result.cabinClass,
                            gdsType = GdsType.OneDFare,
                            SeatAvailable = Convert.ToInt32(dr["NoOfSeat"]),
                            subProvider = new SubProvider()
                        };
                        fare.mojoFareType = MojoFareType.SeriesFareWithPNR;

                        fare.fareBreakdown = new List<Core.Flight.FareBreakdown>();
                        #region set fare Breakup
                        if (request.infants > 0)
                        {
                            Core.Flight.FareBreakdown infFare = new Core.Flight.FareBreakdown();
                            infFare.BaseFare = 1000;
                            infFare.Tax = 500;
                            infFare.PassengerType = Core.PassengerType.Infant;
                            fare.fareBreakdown.Add(infFare);
                        }


                        Core.Flight.FareBreakdown adtFare = new Core.Flight.FareBreakdown();
                        adtFare.BaseFare = UnitFare * 0.75m;
                        adtFare.Tax = UnitFare * 0.25m;
                        adtFare.PassengerType = Core.PassengerType.Adult;
                        fare.fareBreakdown.Add(adtFare);
                        if (request.child > 0)
                        {
                            Core.Flight.FareBreakdown chdFare = new Core.Flight.FareBreakdown();
                            chdFare.BaseFare = UnitFare * 0.75m;
                            chdFare.Tax = UnitFare * 0.25m;
                            chdFare.PassengerType = Core.PassengerType.Child;
                            fare.fareBreakdown.Add(chdFare);
                        }

                        #endregion
                        fare.NetFare = fare.grandTotal = fare.PublishedFare + fare.Markup - fare.CommissionEarned;
                        if (request.cabinType == fare.cabinType)
                        {
                            #region BlockAirlines
                            if (Core.FlightUtility.airlineBlockList.Where(o => (o.Action == AirlineBlockAction.Block) && (o.Supplier == GdsType.OneDFare) &&
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
                            if (result.valCarrier == "SG" && request.segment[0].travelDate > DateTime.Today.AddDays(28) && (fare.mojoFareType == MojoFareType.SeriesFareWithoutPNR || fare.mojoFareType == MojoFareType.SeriesFareWithPNR))
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
                flightResponse.Results.Add(listFlightResult);
            }
            if (FlightUtility.isWriteLogSearch)
            {
                bookingLog(ref sbLogger, "OneDFare errorMsg", errorMsg);
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");

            }
            return flightResponse;
        }
        public Core.Flight.FareQuoteResponse GetFareQuote(Core.Flight.PriceVerificationRequest request)
        {
            StringBuilder sbLogger = new StringBuilder();

            FareQuoteResponse _response = new FareQuoteResponse() { flightResult = new List<FlightResult>(), isFareChange = false, responseStatus = new ResponseStatus(), fareIncreaseAmount = 0 };// Newfare = new List<Fare>(),

            try
            {
                int ctr = 0;
                foreach (FlightResult fr in request.flightResult)
                {
                    System.Data.DataSet ds = new DAL.OneDFare.DalOneDFare().getVerify(request.flightResult[ctr].Fare.FB_flight_id, request.adults + request.child);
                    if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows[0]["NoOfSeat"].ToString() != "0")
                    {
                        _response.fareIncreaseAmount = 0;
                        _response.VerifiedTotalPrice = request.flightResult[ctr].Fare.PublishedFare;
                    }
                    else
                    {
                        _response.responseStatus.status = TransactionStatus.Error;
                    }
                    ctr++;
                }
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");
            }
            catch (Exception ex)
            {
                bookingLog(ref sbLogger, "OneD Fare Original Request", JsonConvert.SerializeObject(request));
                bookingLog(ref sbLogger, "Exception", ex.ToString());
                new ServicesHub.LogWriter_New(ex.ToString(), request.userSearchID, "Exeption", "OneD Fare FareQuote Exeption");
            }
            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.userSearchID, "Search");
            return _response;
        }
        public void BookFlight(FlightBookingRequest request, ref FlightBookingResponse _response)
        {
            StringBuilder sbLogger = new StringBuilder();
            try
            {
                int ctr = 0;
                foreach (var item in request.flightResult)
                {
                    System.Data.DataSet ds = new DAL.OneDFare.DalOneDFare().getTicketed(request.flightResult[ctr].Fare.FB_flight_id, request.adults + request.child);
                    if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        _response.PNR = ds.Tables[0].Rows[0]["PNR"].ToString();
                        bookingLog(ref sbLogger, " OneDFare  PNR ", "_response.PNR:" + _response.PNR);
                        _response.responseStatus.message += "; OutBoundPnr-" + _response.PNR;
                        _response.invoice.Add(new Invoice() { InvoiceAmount = request.flightResult[ctr].Fare.NetFare, InvoiceNo = _response.PNR });
                        _response.bookingStatus = BookingStatus.Ticketed;
                    }
                    else
                    {
                        _response.bookingStatus = BookingStatus.Failed;
                        _response.responseStatus.message += "; Booking Fail Due to seat not available";
                    }
                    ctr++;
                }
            }
            catch (Exception ex)
            {
                bookingLog(ref sbLogger, "OneDFare Exption", ex.ToString());
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.bookingID.ToString(), "Error");
            }

            bookingLog(ref sbLogger, "OneDFare  return Response", JsonConvert.SerializeObject(_response));
            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.bookingID.ToString(), "Booking");
            //return _response;
        }


        public void BookFlightInProgress(FlightBookingRequest request, ref FlightBookingResponse _response)
        {
            StringBuilder sbLogger = new StringBuilder();
            try
            {
                int ctr = 0;
                foreach (var item in request.flightResult)
                {
                    System.Data.DataSet ds = new DAL.OneDFare.DalOneDFare().getTicketedInprogress(request.flightResult[ctr].Fare.FB_flight_id, request.adults + request.child);
                    if (ds != null && ds.Tables != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {

                        _response.bookingStatus = BookingStatus.InProgress;
                        _response.responseStatus.message = "InProgress";
                    }
                    else
                    {
                        _response.bookingStatus = BookingStatus.Failed;
                        _response.responseStatus.message += "; Booking Fail Due to seat not available";
                    }
                    ctr++;
                }
            }
            catch (Exception ex)
            {
                bookingLog(ref sbLogger, "OneDFare Exption", ex.ToString());
                new ServicesHub.LogWriter_New(sbLogger.ToString(), request.bookingID.ToString(), "Error");
            }

            bookingLog(ref sbLogger, "OneDFare  return Response", JsonConvert.SerializeObject(_response));
            new ServicesHub.LogWriter_New(sbLogger.ToString(), request.bookingID.ToString(), "Booking");
            //return _response;
        }

        public void bookingLog(ref StringBuilder sbLogger, string requestTitle, string logText)
        {
            sbLogger.Append(Environment.NewLine + "---------------------------------------------" + requestTitle + "" + DateTime.Now.ToLongTimeString() + " " + DateTime.Now.ToLongDateString() + "---------------------------------------------");
            sbLogger.Append(Environment.NewLine + logText);
            sbLogger.Append(Environment.NewLine + "------------------------------------------------------" + Environment.NewLine + Environment.NewLine + Environment.NewLine);
        }

    }
}
