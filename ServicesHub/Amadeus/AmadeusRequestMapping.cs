using Core;
using Core.Flight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServicesHub.Amadeus
{
    public class AmadeusRequestMapping
    {
        public string Fare_InformativeBestPricingWithoutPNR(GfPriceVerifyRequest Request, ref int adtQnty, ref int chdQnty, ref int infQnty)
        {
            //var depSeg = bookingRequest.Flight.Segments.Where(o => o.IsReturnFlight == false).ToList();
            int quantity = 1;
            int measurementValue = 1;

            StringBuilder strFare_BestPricing = new StringBuilder();

            strFare_BestPricing.Append("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">");
            strFare_BestPricing.Append(GetSoapHeader(AmadeusConfiguration.GetAmadeusSoapAction(AmadeusSoapActionType.Fare_InformativeBestPricingWithoutPNR), false));
            strFare_BestPricing.Append("<s:Body xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");

            strFare_BestPricing.Append("<Fare_InformativeBestPricingWithoutPNR xmlns=\"http://xml.amadeus.com/" + AmadeusConfiguration.GetAmadeusSoapAction(AmadeusSoapActionType.Fare_InformativeBestPricingWithoutPNR) + "\">");

            if (Request.adults > 0)
            {
                adtQnty = quantity;
                strFare_BestPricing.Append(" <passengersGroup>");
                strFare_BestPricing.Append("<segmentRepetitionControl>");
                strFare_BestPricing.Append(" <segmentControlDetails>");
                strFare_BestPricing.Append("	<quantity>" + quantity + "</quantity>");
                strFare_BestPricing.Append("	<numberOfUnits>" + Request.adults + "</numberOfUnits>");
                strFare_BestPricing.Append(" </segmentControlDetails>");
                strFare_BestPricing.Append("</segmentRepetitionControl>");
                strFare_BestPricing.Append(" <travellersID>");

                for (int j = 1; j <= Request.adults; j++)
                {

                    strFare_BestPricing.Append(" <travellerDetails>");
                    strFare_BestPricing.Append(" <measurementValue>" + measurementValue + "</measurementValue>");
                    strFare_BestPricing.Append(" </travellerDetails>");
                    measurementValue++;
                }
                strFare_BestPricing.Append(" </travellersID>");
                //strFare_BestPricing.Append("<ptcGroup>");
                strFare_BestPricing.Append(" <discountPtc>");
                strFare_BestPricing.Append("	<valueQualifier>ADT</valueQualifier>");
                strFare_BestPricing.Append(" </discountPtc>");
                //strFare_BestPricing.Append("</ptcGroup>");
                strFare_BestPricing.Append(" </passengersGroup>");
                quantity++;
            }

            if (Request.child > 0)
            {
                chdQnty = quantity;
                strFare_BestPricing.Append(" <passengersGroup>");
                strFare_BestPricing.Append("<segmentRepetitionControl>");
                strFare_BestPricing.Append(" <segmentControlDetails>");
                strFare_BestPricing.Append("	<quantity>" + quantity + "</quantity>");
                strFare_BestPricing.Append("	<numberOfUnits>" + Request.child + "</numberOfUnits>");
                strFare_BestPricing.Append(" </segmentControlDetails>");
                strFare_BestPricing.Append("</segmentRepetitionControl>");
                strFare_BestPricing.Append(" <travellersID>");
                for (int j = 1; j <= Request.child; j++)
                {

                    strFare_BestPricing.Append(" <travellerDetails>");
                    strFare_BestPricing.Append(" <measurementValue>" + measurementValue + "</measurementValue>");
                    strFare_BestPricing.Append(" </travellerDetails>");
                    measurementValue++;
                }
                strFare_BestPricing.Append(" </travellersID>");
                //strFare_BestPricing.Append("<ptcGroup>");
                strFare_BestPricing.Append(" <discountPtc>");
                strFare_BestPricing.Append("	<valueQualifier>CH</valueQualifier>");
                strFare_BestPricing.Append(" </discountPtc>");
                //strFare_BestPricing.Append("</ptcGroup>");
                strFare_BestPricing.Append(" </passengersGroup>");
                quantity++;
            }

            if (Request.infantsWs > 0)
            {
                //measurementValue = 1;
                strFare_BestPricing.Append(" <passengersGroup>");
                strFare_BestPricing.Append("<segmentRepetitionControl>");
                strFare_BestPricing.Append(" <segmentControlDetails>");
                strFare_BestPricing.Append("	<quantity>" + quantity + "</quantity>");
                strFare_BestPricing.Append("	<numberOfUnits>" + Request.infantsWs + "</numberOfUnits>");
                strFare_BestPricing.Append(" </segmentControlDetails>");
                strFare_BestPricing.Append("</segmentRepetitionControl>");
                strFare_BestPricing.Append(" <travellersID>");
                for (int j = 1; j <= Request.infantsWs; j++)
                {

                    strFare_BestPricing.Append(" <travellerDetails>");
                    strFare_BestPricing.Append(" <measurementValue>" + measurementValue + "</measurementValue>");
                    strFare_BestPricing.Append(" </travellerDetails>");
                    measurementValue++;
                }
                strFare_BestPricing.Append(" </travellersID>");
                //strFare_BestPricing.Append("<ptcGroup>");
                strFare_BestPricing.Append(" <discountPtc>");
                strFare_BestPricing.Append("	<valueQualifier>INS</valueQualifier>");
                strFare_BestPricing.Append(" </discountPtc>");
                //strFare_BestPricing.Append("</ptcGroup>");
                strFare_BestPricing.Append(" </passengersGroup>");
                quantity++;
            }
            if (Request.infants > 0)
            {
                infQnty = quantity;
                strFare_BestPricing.Append("<passengersGroup>");
                strFare_BestPricing.Append("<segmentRepetitionControl>");
                strFare_BestPricing.Append("<segmentControlDetails>");
                strFare_BestPricing.Append("<quantity>" + quantity + "</quantity>");
                strFare_BestPricing.Append("<numberOfUnits>" + Request.infants + "</numberOfUnits>");
                strFare_BestPricing.Append("</segmentControlDetails>");
                strFare_BestPricing.Append("</segmentRepetitionControl>");
                strFare_BestPricing.Append(" <travellersID>");
                measurementValue = 1;
                for (int j = 1; j <= Request.infants; j++)
                {

                    strFare_BestPricing.Append("<travellerDetails>");
                    strFare_BestPricing.Append("<measurementValue>" + measurementValue + "</measurementValue>");
                    strFare_BestPricing.Append("</travellerDetails>");
                    measurementValue++;
                }
                strFare_BestPricing.Append("</travellersID>");
                //strFare_BestPricing.Append("<ptcGroup>");
                strFare_BestPricing.Append("<discountPtc>");
                strFare_BestPricing.Append("<valueQualifier>INF</valueQualifier>");
                strFare_BestPricing.Append("<fareDetails>");
                strFare_BestPricing.Append("<qualifier>766</qualifier>");
                strFare_BestPricing.Append("</fareDetails>");

                strFare_BestPricing.Append("</discountPtc>");
                //strFare_BestPricing.Append("</ptcGroup>");
                strFare_BestPricing.Append("</passengersGroup>");
                quantity++;
            }

            int itemNumber = 1;
            int flightIndicator = 1;
            //foreach (var item in Request.googleFlightRequest.flightSlice)
            //{
            foreach (FlightSegment fs in Request.flightResult[0].FlightSegments)
            {


                foreach (var item in fs.Segments)
                {

                    strFare_BestPricing.Append("<segmentGroup>");
                    strFare_BestPricing.Append(" <segmentInformation>");
                    strFare_BestPricing.Append("	<flightDate>");
                    strFare_BestPricing.Append("	<departureDate>" + item.DepTime.ToString("ddMMyy") + "</departureDate>");
                    strFare_BestPricing.Append("	</flightDate>");
                    strFare_BestPricing.Append("	<boardPointDetails>");
                    strFare_BestPricing.Append("	<trueLocationId>" + item.Origin + "</trueLocationId>");
                    strFare_BestPricing.Append("	</boardPointDetails>");
                    strFare_BestPricing.Append("	<offpointDetails>");
                    strFare_BestPricing.Append("	<trueLocationId>" + item.Destination + "</trueLocationId>");
                    strFare_BestPricing.Append("	</offpointDetails>");
                    strFare_BestPricing.Append("	<companyDetails>");
                    strFare_BestPricing.Append("	<marketingCompany>" + item.Airline + "</marketingCompany>");
                    strFare_BestPricing.Append("	</companyDetails>");
                    strFare_BestPricing.Append("	<flightIdentification>");
                    strFare_BestPricing.Append("	<flightNumber>" + item.FlightNumber + "</flightNumber>");
                    strFare_BestPricing.Append("	<bookingClass>" + item.resDesignCode + "</bookingClass>");
                    strFare_BestPricing.Append("	</flightIdentification>");
                    strFare_BestPricing.Append("    <flightTypeDetails>");
                    strFare_BestPricing.Append("    <flightIndicator>" + flightIndicator + "</flightIndicator>");
                    strFare_BestPricing.Append("    </flightTypeDetails>");
                    strFare_BestPricing.Append("    <itemNumber>" + itemNumber + "</itemNumber>");
                    strFare_BestPricing.Append(" </segmentInformation>");
                    strFare_BestPricing.Append("</segmentGroup>");
                    itemNumber++;
                }
                //flightIndicator++;
            }

            strFare_BestPricing.Append("  <pricingOptionGroup>");
            strFare_BestPricing.Append("    <pricingOptionKey>");
            strFare_BestPricing.Append("      <pricingOptionKey>RP</pricingOptionKey>");
            strFare_BestPricing.Append("    </pricingOptionKey>");
            strFare_BestPricing.Append("  </pricingOptionGroup>");
            strFare_BestPricing.Append("  <pricingOptionGroup>");
            strFare_BestPricing.Append("    <pricingOptionKey>");
            strFare_BestPricing.Append("      <pricingOptionKey>RU</pricingOptionKey>");
            strFare_BestPricing.Append("    </pricingOptionKey>");
            strFare_BestPricing.Append("  </pricingOptionGroup>");
            //strFare_BestPricing.Append("  <pricingOptionGroup>");
            //strFare_BestPricing.Append("    <pricingOptionKey>");
            //strFare_BestPricing.Append("      <pricingOptionKey>RN</pricingOptionKey>");
            //strFare_BestPricing.Append("    </pricingOptionKey>");
            //strFare_BestPricing.Append("  </pricingOptionGroup>");
            strFare_BestPricing.Append("  <pricingOptionGroup>");
            strFare_BestPricing.Append("    <pricingOptionKey>");
            strFare_BestPricing.Append("      <pricingOptionKey>RLA</pricingOptionKey>");
            strFare_BestPricing.Append("    </pricingOptionKey>");
            strFare_BestPricing.Append("  </pricingOptionGroup>");
            //strFare_BestPricing.Append("  <pricingOptionGroup>");
            //strFare_BestPricing.Append("    <pricingOptionKey>");
            //strFare_BestPricing.Append("      <pricingOptionKey>RLO</pricingOptionKey>");
            //strFare_BestPricing.Append("    </pricingOptionKey>");
            //strFare_BestPricing.Append("  </pricingOptionGroup>");

            //strFare_BestPricing.Append("  <pricingOptionGroup>");
            //strFare_BestPricing.Append("    <pricingOptionKey>");
            //strFare_BestPricing.Append("      <pricingOptionKey>NVO</pricingOptionKey>");
            //strFare_BestPricing.Append("    </pricingOptionKey>");
            //strFare_BestPricing.Append("  </pricingOptionGroup>");
            strFare_BestPricing.Append("</Fare_InformativeBestPricingWithoutPNR>");




            strFare_BestPricing.Append("</s:Body>");
            strFare_BestPricing.Append("</s:Envelope>");

            return strFare_BestPricing.ToString();
        }
        public string Air_SellFromRecommendation_1(FlightBookingRequest flightBookingRequest)
        {
            int totalPax = (flightBookingRequest.adults + flightBookingRequest.child + flightBookingRequest.infantsWs);
            //flightBookingRequest.Infant +
            StringBuilder strXml = new StringBuilder();
            strXml.Append("<Air_SellFromRecommendation xmlns=\"http://xml.amadeus.com/ITAREQ_05_2_IA\">");
            strXml.Append("<messageActionDetails>");
            strXml.Append("<messageFunctionDetails>");
            strXml.Append("<messageFunction>183</messageFunction>");
            //183 Lowest fare across airline, flight, class criteria.
            strXml.Append("<additionalMessageFunction>M1</additionalMessageFunction>");
            strXml.Append("</messageFunctionDetails>");
            strXml.Append("</messageActionDetails>");
            #region Segment Details
            foreach (var fSeg in flightBookingRequest.flightResult[0].FlightSegments)
            {
                if (fSeg.Segments.Count > 0)
                {
                    //var outBoundFlights = flightBookingRequest.flightResult.out.Where(x => x.IsReturnFlight == false).ToList().OrderBy(x => x.SequenceNumber);
                    strXml.Append("<itineraryDetails>");
                    strXml.Append("<originDestinationDetails>");
                    strXml.Append("<origin>" + fSeg.Segments.ElementAt(0).Origin + "</origin>");
                    strXml.Append("<destination>" + fSeg.Segments.LastOrDefault().Destination + "</destination>");
                    strXml.Append("</originDestinationDetails>");
                    strXml.Append("<message>");
                    strXml.Append("<messageFunctionDetails>");
                    strXml.Append("<messageFunction>183</messageFunction>");
                    strXml.Append("</messageFunctionDetails>");
                    strXml.Append("</message>");
                    foreach (var allData in fSeg.Segments)
                    {
                        strXml.Append("<segmentInformation>");
                        strXml.Append("<travelProductInformation>");
                        strXml.Append("<flightDate>");
                        strXml.Append("<departureDate>" + string.Format("{0:ddMMyy}", Convert.ToDateTime(allData.DepTime)) + "</departureDate>");
                        strXml.Append("</flightDate>");
                        strXml.Append("<boardPointDetails>");
                        strXml.Append("<trueLocationId>" + allData.Origin + "</trueLocationId>");
                        strXml.Append("</boardPointDetails>");
                        strXml.Append("<offpointDetails>");
                        strXml.Append("<trueLocationId>" + allData.Destination + "</trueLocationId>");
                        strXml.Append("</offpointDetails>");
                        strXml.Append("<companyDetails>");
                        strXml.Append("<marketingCompany>" + allData.Airline + "</marketingCompany>");
                        strXml.Append("</companyDetails>");
                        strXml.Append("<flightIdentification>");
                        strXml.Append("<flightNumber>" + allData.FlightNumber + "</flightNumber>");
                        strXml.Append("<bookingClass>" + allData.resDesignCode + "</bookingClass>");
                        strXml.Append("</flightIdentification>");
                        //if (flightBookingRequest.flightResult.isSliceDiceAvailable)
                        //{
                        //    strXml.Append("<flightTypeDetails>");
                        //    strXml.Append("<flightIndicator>LA</flightIndicator>");
                        //    strXml.Append("</flightTypeDetails>");
                        //}

                        strXml.Append("</travelProductInformation>");
                        strXml.Append("<relatedproductInformation>");
                        strXml.Append("<quantity>" + totalPax + "</quantity>");
                        strXml.Append("<statusCode>NN</statusCode>");
                        strXml.Append("</relatedproductInformation>");
                        strXml.Append("</segmentInformation>");
                    }
                    strXml.Append("</itineraryDetails>");
                }
            }
            #endregion

            strXml.Append("</Air_SellFromRecommendation>");

            #region AirSellFromRecommendation
            StringBuilder str = new StringBuilder();
            str.Append("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">");
            str.Append(GetSoapHeader(AmadeusConfiguration.GetAmadeusSoapAction(AmadeusSoapActionType.Air_SellFromRecommendation), true));
            str.Append("<s:Body xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
            str.Append(strXml.ToString());
            str.Append("</s:Body>");
            str.Append("</s:Envelope>");
            #endregion
            return str.ToString();
        }
        public string PNR_AddElementRequest_New(FlightBookingRequest flightBookingRequest, AmadeusSessionTemplate objSession)
        {
            StringBuilder strXml = new StringBuilder();
            strXml.Append("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">");
            strXml.Append(GetSessionSoapHeader(AmadeusConfiguration.GetAmadeusSoapAction(AmadeusSoapActionType.PNR_AddMultiElements), objSession, "InSeries"));
            #region PNRAddElements

            strXml.Append("<s:Body xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
            strXml.Append("<PNR_AddMultiElements xmlns=\"http://xml.amadeus.com/" + AmadeusConfiguration.GetAmadeusSoapAction(AmadeusSoapActionType.PNR_AddMultiElements) + "\">");
            strXml.Append("<pnrActions>");
            strXml.Append("<optionCode>0</optionCode>");
            strXml.Append("</pnrActions>");

            #region PaxDetails

            List<PassengerDetails> pInfant = flightBookingRequest.passengerDetails.Where<PassengerDetails>(o => o.passengerType == PassengerType.Infant).ToList<PassengerDetails>();

            List<PassengerDetails> pPAX = flightBookingRequest.passengerDetails.Where<PassengerDetails>(o => o.passengerType != PassengerType.Infant).ToList<PassengerDetails>();

            int Pax = 0;
            int infant = pInfant.Count;
            if (pPAX.Any())
            {
                byte adultInfantLoop = 0;
                string paxType = string.Empty;
                for (int i = 0; i < pPAX.Count; i++)
                {
                    Pax++;
                    strXml.Append("<travellerInfo>");
                    strXml.Append("<elementManagementPassenger>");
                    strXml.Append("<reference>");
                    strXml.Append("<qualifier>PR</qualifier>");
                    strXml.Append("<number>" + Pax + "</number>");
                    strXml.Append("</reference>");
                    strXml.Append("<segmentName>NM</segmentName>");
                    strXml.Append("</elementManagementPassenger>");
                    #region paxTypes
                    if (pPAX[i].passengerType == PassengerType.Adult)
                    {
                        paxType = "ADT";
                    }
                    else if (pPAX[i].passengerType == PassengerType.Child)
                    {
                        paxType = "CHD";
                    }
                    else if (pPAX[i].passengerType == PassengerType.InfantWs)
                    {
                        paxType = "INS";
                    }
                    #endregion
                    byte quantity = 1;
                    byte infantIndicator = 0;
                    #region InfantIndicator
                    if (flightBookingRequest.infants > adultInfantLoop)
                    {

                        quantity = 2;
                        if (String.IsNullOrEmpty(pInfant.ElementAt(adultInfantLoop).lastName.ToUpper().Trim()) == true)
                        {
                            // == pPAX.ElementAt(i).LastName.ToUpper().Trim()
                            infantIndicator = 2;
                        }
                        else
                        {
                            infantIndicator = 3;
                        }
                    }
                    #endregion
                    strXml.Append("<passengerData>");
                    strXml.Append("<travellerInformation>");
                    strXml.Append("<traveller>");
                    strXml.Append("<surname>" + SMCommanMethod.replaceSpecialChar(pPAX.ElementAt(i).lastName).ToUpper() + "</surname>");
                    strXml.Append("<quantity>" + quantity + "</quantity>");
                    strXml.Append("</traveller>");
                    strXml.Append("<passenger>");
                    strXml.Append("<firstName>" + SMCommanMethod.replaceSpecialChar(pPAX.ElementAt(i).firstName).ToUpper() + (string.IsNullOrEmpty(pPAX[i].middleName) ? "" : (" " + SMCommanMethod.replaceSpecialChar(pPAX[i].middleName))) + "</firstName>");
                    strXml.Append("<type>" + paxType + "</type>");
                    if (infantIndicator > 0)
                    {
                        strXml.Append("<infantIndicator>" + infantIndicator + "</infantIndicator>");
                    }
                    strXml.Append("</passenger>");
                    if (infantIndicator == 2)
                    {
                        strXml.Append("<passenger>");
                        strXml.Append("<firstName>" + SMCommanMethod.replaceSpecialChar(pInfant.ElementAt(adultInfantLoop).firstName).ToUpper() + "</firstName>");
                        strXml.Append("<type>INF</type>");
                        strXml.Append("</passenger>");
                    }
                    strXml.Append("</travellerInformation>");
                    if (paxType == "CHD")
                    {
                        strXml.Append("<dateOfBirth><dateAndTimeDetails>");
                        strXml.Append("<qualifier>706</qualifier>");
                        strXml.Append("<date>" + pPAX.ElementAt(i).dateOfBirth.ToString("ddMMyyyy") + "</date>");
                        strXml.Append("</dateAndTimeDetails>");
                        strXml.Append("</dateOfBirth>");
                    }
                    //if (infantIndicator == 2)
                    //{
                    //    strXml.Append("<dateOfBirth><dateAndTimeDetails>");
                    //    strXml.Append("<qualifier>706</qualifier>");
                    //    strXml.Append("<date>" + pInfant[adultInfantLoop].Dob.ToString("ddMMMyy").ToUpper() + "</date>");
                    //    strXml.Append("</dateAndTimeDetails>");
                    //    strXml.Append("</dateOfBirth>");
                    //}

                    strXml.Append("</passengerData>");

                    if (infantIndicator == 3)
                    {
                        strXml.Append("<passengerData>");
                        strXml.Append("<travellerInformation>");
                        strXml.Append("<traveller>");
                        strXml.Append("<surname>" + SMCommanMethod.replaceSpecialChar(pInfant[adultInfantLoop].lastName).ToUpper() + "</surname>");
                        strXml.Append("</traveller>");
                        strXml.Append("<passenger>");
                        strXml.Append("<firstName>" + SMCommanMethod.replaceSpecialChar(pInfant[adultInfantLoop].firstName).ToUpper() + (string.IsNullOrEmpty(pInfant[adultInfantLoop].middleName) ? "" : (" " + SMCommanMethod.replaceSpecialChar(pInfant[adultInfantLoop].middleName))) + "</firstName>");
                        strXml.Append("<type>INF</type>");
                        strXml.Append("</passenger>");
                        strXml.Append("</travellerInformation>");
                        strXml.Append("<dateOfBirth><dateAndTimeDetails>");
                        strXml.Append("<qualifier>706</qualifier>");
                        strXml.Append("<date>" + pInfant[adultInfantLoop].dateOfBirth.ToString("ddMMMyy").ToUpper() + "</date>");
                        strXml.Append("</dateAndTimeDetails>");
                        strXml.Append("</dateOfBirth>");
                        strXml.Append("</passengerData>");
                    }

                    strXml.Append(" </travellerInfo>");
                    adultInfantLoop++;
                }
            }

            #endregion


            List<string> lstAirline = new List<string>();
            foreach (var fSeg in flightBookingRequest.flightResult[0].FlightSegments)
            {
                foreach (var seg in fSeg.Segments)
                {
                    lstAirline.Add(seg.Airline);
                }
            }
            int dataElementSize = lstAirline.Count * (pPAX.Count + pInfant.Count);

            int markerNo = 0;

            strXml.Append("<dataElementsMaster>");
            strXml.Append("<marker1></marker1>");

            #region addSSRforSegments
            foreach (var item in lstAirline)
            {
                if (pPAX.Any())
                {
                    string paxType = string.Empty;
                    for (int i = 0; i < pPAX.Count; i++)
                    {
                        markerNo++;
                        strXml.Append("<dataElementsIndiv>");
                        strXml.Append("<elementManagementData>");
                        strXml.Append("<reference>");
                        strXml.Append("<qualifier>PT</qualifier>");
                        strXml.Append("<number>" + markerNo + "</number>");
                        strXml.Append("</reference>");
                        strXml.Append("<segmentName>SSR</segmentName>");
                        strXml.Append("</elementManagementData>");
                        strXml.Append("<serviceRequest>");
                        strXml.Append("<ssr>");
                        strXml.Append("<type>DOCS</type>");
                        strXml.Append("<status>HK</status>");
                        strXml.Append("<quantity>1</quantity>");
                        strXml.Append("<companyId>" + item + "</companyId>");
                        strXml.Append("<indicator>P01</indicator>");
                        string strFreeText = string.Empty;
                        strFreeText = "----" + pPAX[i].dateOfBirth.ToString("ddMMMyy") + "-" + (pPAX[i].gender == Gender.Male ? "M" : (pPAX[i].gender == Gender.Female ? "F" : "M")) + "--" + SMCommanMethod.replaceSpecialChar(pPAX[i].lastName) + "-" + SMCommanMethod.replaceSpecialChar(pPAX[i].firstName) + (string.IsNullOrEmpty(pPAX[i].middleName) ? "" : (" " + SMCommanMethod.replaceSpecialChar(pPAX[i].middleName)));
                        strXml.Append("<freetext>" + strFreeText + "</freetext>");

                        //strXml.Append("<indicator>P01</indicator>");
                        strXml.Append("</ssr>");
                        strXml.Append("</serviceRequest>");
                        #region FrequentFlyer
                        //strXml.Append("<frequentTravellerData>");
                        //strXml.Append("<frequentTraveller>");
                        //strXml.Append("<companyId>QF</companyId>");
                        //strXml.Append("<membershipNumber>QF1117622</membershipNumber>");
                        //strXml.Append("</frequentTraveller>");
                        //strXml.Append("</frequentTravellerData>");
                        #endregion
                        strXml.Append("<referenceForDataElement>");
                        strXml.Append("<reference>");
                        strXml.Append("<qualifier>PR</qualifier>");
                        strXml.Append("<number>" + (i + 1) + "</number>");
                        strXml.Append("</reference>");
                        strXml.Append("</referenceForDataElement>");
                        strXml.Append("</dataElementsIndiv>");

                    }

                    int paxinf = 1;
                    for (int i = 0; i < pInfant.Count; i++)
                    {
                        markerNo++;
                        strXml.Append("<dataElementsIndiv>");
                        strXml.Append("<elementManagementData>");
                        strXml.Append("<reference>");
                        strXml.Append("<qualifier>PT</qualifier>");
                        strXml.Append("<number>" + markerNo + "</number>");
                        strXml.Append("</reference>");
                        strXml.Append("<segmentName>SSR</segmentName>");
                        strXml.Append("</elementManagementData>");
                        strXml.Append("<serviceRequest>");
                        strXml.Append("<ssr>");
                        strXml.Append("<type>DOCS</type>");
                        strXml.Append("<status>HK</status>");
                        strXml.Append("<quantity>1</quantity>");
                        strXml.Append("<companyId>" + item + "</companyId>");
                        strXml.Append("<indicator>P01</indicator>");
                        string strFreeText = string.Empty;
                        strFreeText = "----" + pInfant[i].dateOfBirth.ToString("ddMMMyy") + "-" + (pInfant[i].gender == Gender.Male ? "M" : (pInfant[i].gender == Gender.Female ? "F" : "M")) + "--" + SMCommanMethod.replaceSpecialChar(pInfant[i].lastName) + "-" + SMCommanMethod.replaceSpecialChar(pInfant[i].firstName) + (string.IsNullOrEmpty(pInfant[i].middleName) ? "" : (" " + SMCommanMethod.replaceSpecialChar(pInfant[i].middleName)));
                        strXml.Append("<freetext>" + strFreeText + "</freetext>");
                        strXml.Append("</ssr>");
                        strXml.Append("</serviceRequest>");

                        strXml.Append("<referenceForDataElement>");
                        strXml.Append("<reference>");
                        strXml.Append("<qualifier>PR</qualifier>");
                        strXml.Append("<number>" + paxinf + "</number>");
                        strXml.Append("</reference>");
                        strXml.Append("</referenceForDataElement>");
                        strXml.Append("</dataElementsIndiv>");
                        paxinf++;
                    }
                }
            }

            #endregion// end segment SSR

            markerNo++;
            #region ReceivedFrom
            strXml.Append("<dataElementsIndiv>");
            strXml.Append("<elementManagementData>");
            strXml.Append("<reference>");
            strXml.Append("<qualifier>OT</qualifier>");
            strXml.Append("<number>" + markerNo + "</number>");
            strXml.Append("</reference>");
            strXml.Append("<segmentName>RF</segmentName>");
            strXml.Append("</elementManagementData>");
            strXml.Append("<freetextData>");
            strXml.Append("<freetextDetail>");
            strXml.Append("<subjectQualifier>3</subjectQualifier>");
            strXml.Append("<type>P22</type>");
            strXml.Append("</freetextDetail>");
            strXml.Append("<longFreetext>" + flightBookingRequest.siteID.ToString() + "</longFreetext>");//
            strXml.Append("</freetextData>");
            strXml.Append("</dataElementsIndiv>");

            #endregion

            #region BillAddress
            //if (flightBookingRequest.paymentDetails != null)
            //{
            //    markerNo++;
            //    strXml.Append("<dataElementsIndiv>");
            //    strXml.Append("<elementManagementData>");
            //    strXml.Append("<reference>");
            //    strXml.Append("<qualifier>OT</qualifier>");
            //    strXml.Append("<number>" + markerNo + "</number>");
            //    strXml.Append("</reference>");
            //    strXml.Append("<segmentName>ABU</segmentName>");
            //    strXml.Append("</elementManagementData>");
            //    strXml.Append("<freetextData>");
            //    strXml.Append("<freetextDetail>");
            //    strXml.Append("<subjectQualifier>3</subjectQualifier>");
            //    strXml.Append("<type>2</type>");
            //    strXml.Append("</freetextDetail>");
            //    strXml.Append("<longFreetext>" + GetBillingAddress(flightBookingRequest.paymentDetails) + "</longFreetext>");
            //    strXml.Append("</freetextData>");
            //    strXml.Append("</dataElementsIndiv>");
            //}
            #endregion

            #region Business Phone Number
            if (!string.IsNullOrEmpty(flightBookingRequest.phoneNo))
            {
                markerNo++;
                strXml.Append("<dataElementsIndiv>");
                strXml.Append("<elementManagementData>");
                strXml.Append("<reference>");
                strXml.Append("<qualifier>OT</qualifier>");
                strXml.Append("<number>" + markerNo + "</number>");//3
                strXml.Append("</reference>");
                strXml.Append("<segmentName>AP</segmentName>");
                strXml.Append("</elementManagementData>");
                strXml.Append("<freetextData>");
                strXml.Append("<freetextDetail>");
                strXml.Append("<subjectQualifier>3</subjectQualifier>");
                strXml.Append("<type>4</type>");
                strXml.Append("</freetextDetail>");
                strXml.Append("<longFreetext>" + flightBookingRequest.phoneNo + "</longFreetext>");
                strXml.Append("</freetextData>");
                strXml.Append("</dataElementsIndiv>");
            }
            #endregion

            #region Home Phone Number
            if (!string.IsNullOrEmpty(flightBookingRequest.mobileNo))
            {
                markerNo++;
                strXml.Append("<dataElementsIndiv>");
                strXml.Append("<elementManagementData>");
                strXml.Append("<reference>");
                strXml.Append("<qualifier>OT</qualifier>");
                strXml.Append("<number>" + markerNo + "</number>");//3
                strXml.Append("</reference>");
                strXml.Append("<segmentName>AP</segmentName>");
                strXml.Append("</elementManagementData>");
                strXml.Append("<freetextData>");
                strXml.Append("<freetextDetail>");
                strXml.Append("<subjectQualifier>3</subjectQualifier>");
                strXml.Append("<type>7</type>");
                strXml.Append("</freetextDetail>");
                strXml.Append("<longFreetext>" + flightBookingRequest.mobileNo + "</longFreetext>");
                strXml.Append("</freetextData>");
                strXml.Append("</dataElementsIndiv>");
            }
            #endregion


            #region TravelAgencyPhoneNo
            markerNo++;
            strXml.Append("<dataElementsIndiv>");
            strXml.Append("<elementManagementData>");
            strXml.Append("<reference>");
            strXml.Append("<qualifier>OT</qualifier>");
            strXml.Append("<number>" + markerNo + "</number>");//3
            strXml.Append("</reference>");
            strXml.Append("<segmentName>AP</segmentName>");
            strXml.Append("</elementManagementData>");
            strXml.Append("<freetextData>");
            strXml.Append("<freetextDetail>");
            strXml.Append("<subjectQualifier>3</subjectQualifier>");
            strXml.Append("<type>3</type>");
            strXml.Append("</freetextDetail>");
            strXml.Append("<longFreetext>" + flightBookingRequest.siteID.ToString() + "  - A</longFreetext>");
            strXml.Append("</freetextData>");
            strXml.Append("</dataElementsIndiv>");
            #endregion

            #region Email Address
            markerNo++;
            strXml.Append("<dataElementsIndiv>");
            strXml.Append("<elementManagementData>");
            strXml.Append("<reference>");
            strXml.Append("<qualifier>OT</qualifier>");
            strXml.Append("<number>" + markerNo + "</number>");//4
            strXml.Append("</reference>");
            strXml.Append("<segmentName>AP</segmentName>");
            strXml.Append("</elementManagementData>");
            strXml.Append("<freetextData>");
            strXml.Append("<freetextDetail>");
            strXml.Append("<subjectQualifier>3</subjectQualifier>");
            strXml.Append("<type>P02</type>");
            strXml.Append("</freetextDetail>");
            strXml.Append("<longFreetext>" + flightBookingRequest.emailID + "</longFreetext>");
            strXml.Append("</freetextData>");
            strXml.Append("</dataElementsIndiv>");
            #endregion


            string expDate = string.Empty;
            //if (flightBookingRequest.paymentDetails != null)
            //{
            //    expDate = string.Format("{0}{1}", SMCommanMethod.GetExpMM(flightBookingRequest.paymentDetails.expiryMonth), SMCommanMethod.GetExpYY(flightBookingRequest.paymentDetails.expiryYear));

            //}


            #region RC-Confidentials Remarks
            //markerNo++;
            //strXml.Append("<dataElementsIndiv>");
            //strXml.Append("<elementManagementData>");
            //strXml.Append("<reference>");
            //strXml.Append("<qualifier>OT</qualifier>");
            //strXml.Append("<number>" + markerNo + "</number>");
            //strXml.Append("</reference>");
            //strXml.Append("<segmentName>RC</segmentName>");
            //strXml.Append("</elementManagementData>");
            //strXml.Append("<miscellaneousRemark>");
            //strXml.Append("<remarks>");
            //strXml.Append("<type>RC</type>");
            //strXml.Append("<freetext>" + string.Format("{0}/{1},{2}", flightBookingRequest.Payment.CardNumber, expDate, flightBookingRequest.Payment.CvvNo) + "</freetext>");
            //strXml.Append("</remarks>");
            //strXml.Append("</miscellaneousRemark>");
            //strXml.Append("</dataElementsIndiv>");
            #endregion

            #region TKTOK
            markerNo++;
            strXml.Append("<dataElementsIndiv>");
            strXml.Append("<elementManagementData>");
            strXml.Append("<reference>");
            strXml.Append("<qualifier>OT</qualifier>");
            strXml.Append("<number>" + markerNo + "</number>");
            strXml.Append("</reference>");
            strXml.Append("<segmentName>TK</segmentName>");
            strXml.Append("</elementManagementData>");
            strXml.Append("<ticketElement>");
            strXml.Append("<ticket>");
            strXml.Append("<indicator>OK</indicator>");
            strXml.Append("</ticket>");
            strXml.Append("</ticketElement>");
            strXml.Append("</dataElementsIndiv>");
            #endregion

            #region form Of Payment
            if (flightBookingRequest.paymentDetails != null)
            {
                //markerNo++;
                //strXml.Append("<dataElementsIndiv>");
                //strXml.Append("<elementManagementData>");
                //strXml.Append("<reference>");
                //strXml.Append("<qualifier>OT</qualifier>");
                //strXml.Append("<number>" + markerNo + "</number>");
                //strXml.Append("</reference>");
                //strXml.Append("<segmentName>FP</segmentName>");
                //strXml.Append("</elementManagementData>");
                //strXml.Append("<formOfPayment>");
                //strXml.Append("<fop>");
                //strXml.Append("<identification>CC</identification>");//CA         
                //strXml.Append("<creditCardCode>" + SMCommanMethod.GetCardCode(flightBookingRequest.paymentDetails.cardCode) + "</creditCardCode>");//" + SMCommanMethod.GetCardCode(flightBookingRequest.Payment.CardCode) + "

                //strXml.Append("<accountNumber>" + flightBookingRequest.paymentDetails.cardNumber + "</accountNumber>");//" + flightBookingRequest.Payment.CardNumber + "
                //strXml.Append("<expiryDate>" + expDate + "</expiryDate>");//" + expDate + "

                //strXml.Append("</fop>");
                //strXml.Append("</formOfPayment>");
                //strXml.Append("</dataElementsIndiv>");
            }
            #endregion


            #region General Remark element
            markerNo++;
            strXml.Append("<dataElementsIndiv>");
            strXml.Append("<elementManagementData>");
            strXml.Append("<reference>");
            strXml.Append("<qualifier>OT</qualifier>");
            strXml.Append("<number>" + markerNo + "</number>");
            strXml.Append("</reference>");
            strXml.Append("<segmentName>RM</segmentName>");
            strXml.Append("</elementManagementData>");
            strXml.Append("<miscellaneousRemark>");
            strXml.Append("<remarks>");
            strXml.Append("<type>RM</type>");
            strXml.Append("<freetext>" + string.Format("ticket amt ${0} ///tax ${1} ///mco ${2}",
             flightBookingRequest.flightResult[0].Fare.BaseFare.ToString("f2"),
             flightBookingRequest.flightResult[0].Fare.Tax.ToString("f2"),
             flightBookingRequest.flightResult[0].Fare.Markup.ToString("f2")) + "</freetext>");
            strXml.Append("</remarks>");
            strXml.Append("</miscellaneousRemark>");
            strXml.Append("</dataElementsIndiv>");
            #endregion

            #region General Remark element
            //markerNo++;
            //strXml.Append("<dataElementsIndiv>");
            //strXml.Append("<elementManagementData>");
            //strXml.Append("<reference>");
            //strXml.Append("<qualifier>OT</qualifier>");
            //strXml.Append("<number>" + markerNo + "</number>");
            //strXml.Append("</reference>");
            //strXml.Append("<segmentName>RM</segmentName>");
            //strXml.Append("</elementManagementData>");
            //strXml.Append("<miscellaneousRemark>");
            //strXml.Append("<remarks>");
            //strXml.Append("<type>RM</type>");
            //strXml.Append("<freetext>" + string.Format("ticket ttl ${0} ///macp ${1}///cpn $-{2}",
            // ((((flightBookingRequest.Fare.AdultFare + flightBookingRequest.Fare.AdultTax + flightBookingRequest.Fare.Markup) * flightBookingRequest.Adult) +
            //   ((flightBookingRequest.Fare.ChildFare + flightBookingRequest.Fare.ChildTax + flightBookingRequest.Fare.Markup) * flightBookingRequest.Child) +
            //   ((flightBookingRequest.Fare.InfantFare + flightBookingRequest.Fare.InfantTax + flightBookingRequest.Fare.Markup) * flightBookingRequest.Infant) +
            //   ((flightBookingRequest.Fare.InfantWsFare + flightBookingRequest.Fare.InfantWsTax + flightBookingRequest.Fare.Markup) * flightBookingRequest.InfantWs) +
            //   (flightBookingRequest.finalSeatPrice > 0 ? ((decimal)flightBookingRequest.finalSeatPrice) : 0) +
            //   (isBRB ? flightBookingRequest.BrbProductPrice : 0) + (isInsurance ? InsuranceCost : 0) + flightBookingRequest.Fare.Macp) -
            //   (isCoupon ? Convert.ToDecimal(flightBookingRequest.couponsAmt) : 0) - flightBookingRequest.discountAmt).ToString("f2"),
            //   flightBookingRequest.Fare.Macp.ToString("f2"),
            //   (isCoupon ? flightBookingRequest.couponsAmt.ToString("f2") : "0")) + "</freetext>");
            //strXml.Append("</remarks>");
            //strXml.Append("</miscellaneousRemark>");
            //strXml.Append("</dataElementsIndiv>");
            #endregion


            #region Accounting details AI Tag
            markerNo++;
            strXml.Append("<dataElementsIndiv>");
            strXml.Append("<elementManagementData>");
            strXml.Append("<reference>");
            strXml.Append("<qualifier>OT</qualifier>");
            strXml.Append("<number>" + markerNo + "</number>");
            strXml.Append("</reference>");
            strXml.Append("<segmentName>AI</segmentName>");
            strXml.Append("</elementManagementData>");
            strXml.Append("<accounting>");
            strXml.Append("<account>");
            strXml.Append("<number>FMLDFW</number>");
            strXml.Append("</account>");
            strXml.Append("</accounting>");
            strXml.Append("</dataElementsIndiv>");
            #endregion

            #region General Remark element
            //markerNo++;
            //strXml.Append("<dataElementsIndiv>");
            //strXml.Append("<elementManagementData>");
            //strXml.Append("<reference>");
            //strXml.Append("<qualifier>OT</qualifier>");
            //strXml.Append("<number>" + markerNo + "</number>");
            //strXml.Append("</reference>");
            //strXml.Append("<segmentName>RM</segmentName>");
            //strXml.Append("</elementManagementData>");
            //strXml.Append("<miscellaneousRemark>");
            //strXml.Append("<remarks>");
            //strXml.Append("<type>RM</type>");
            //strXml.Append("<freetext>" + string.Format("CCH  =={0} , CVV =={1} ", replaceSpecialChar(flightBookingRequest.Payment.CardHolderName), flightBookingRequest.Payment.CvvNo) + "</freetext>");
            //strXml.Append("</remarks>");
            //strXml.Append("</miscellaneousRemark>");
            //strXml.Append("</dataElementsIndiv>");
            #endregion

            #region General Remark element
            //markerNo++;
            //strXml.Append("<dataElementsIndiv>");
            //strXml.Append("<elementManagementData>");
            //strXml.Append("<reference>");
            //strXml.Append("<qualifier>OT</qualifier>");
            //strXml.Append("<number>" + markerNo + "</number>");
            //strXml.Append("</reference>");
            //strXml.Append("<segmentName>RM</segmentName>");
            //strXml.Append("</elementManagementData>");
            //strXml.Append("<miscellaneousRemark>");
            //strXml.Append("<remarks>");
            //strXml.Append("<type>RM</type>");
            //strXml.Append("<freetext>Please issue the ticket,charge CC and MCO as well</freetext>");
            //strXml.Append("</remarks>");
            //strXml.Append("</miscellaneousRemark>");
            //strXml.Append("</dataElementsIndiv>");
            #endregion

            #region General Remark element
            markerNo++;
            strXml.Append("<dataElementsIndiv>");
            strXml.Append("<elementManagementData>");
            strXml.Append("<reference>");
            strXml.Append("<qualifier>OT</qualifier>");
            strXml.Append("<number>" + markerNo + "</number>");
            strXml.Append("</reference>");
            strXml.Append("<segmentName>RM</segmentName>");
            strXml.Append("</elementManagementData>");
            strXml.Append("<miscellaneousRemark>");
            strXml.Append("<remarks>");
            strXml.Append("<type>RM</type>");
            strXml.Append("<freetext>Booking Source: " + returnSiteName(flightBookingRequest.siteID) + (flightBookingRequest.sourceMedia == "" ? "" : ("-" + flightBookingRequest.sourceMedia)) + "</freetext>");//
            strXml.Append("</remarks>");
            strXml.Append("</miscellaneousRemark>");
            strXml.Append("</dataElementsIndiv>");
            #endregion

            #region FMLine
            //markerNo++;
            //strXml.Append("  <dataElementsIndiv>");
            //strXml.Append("  <elementManagementData>");
            //strXml.Append("     <reference>");
            //strXml.Append("         <qualifier>OT</qualifier>");
            //strXml.Append("         <number> " + markerNo + " </number>");
            //strXml.Append("     </reference>");
            //strXml.Append("     <segmentName>FM</segmentName>");
            //strXml.Append("  </elementManagementData>");
            //strXml.Append("  <commission>");
            //strXml.Append("     <commissionInfo>");
            //strXml.Append("         <percentage>0</percentage>");
            //strXml.Append("     </commissionInfo>");
            //strXml.Append("  </commission>");
            //strXml.Append(" </dataElementsIndiv>");
            #endregion
            strXml.Append("</dataElementsMaster>");
            strXml.Append("</PNR_AddMultiElements>");
            strXml.Append("</s:Body>");
            strXml.Append("</s:Envelope>");

            #endregion
            return strXml.ToString();
        }

        public string FarePricePNRWithBookingClassRequest3(string validatingCarrier, AmadeusSessionTemplate objSession, FlightBookingRequest fbr)
        {
            StringBuilder strXml = new StringBuilder();
            #region Fare_PricePNRWithBookingClass
            strXml.Append("<s:Envelope xmlns:s=\"http://schemas.xmlsoap.org/soap/envelope/\">");
            strXml.Append(GetSessionSoapHeader(AmadeusConfiguration.GetAmadeusSoapAction(AmadeusSoapActionType.Fare_PricePNRWithBookingClass), objSession, "InSeries"));
            strXml.Append("<s:Body xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");
            strXml.Append("<Fare_PricePNRWithBookingClass>");

            //strXml.Append(" <attributeDetails>");
            //strXml.Append(" <attributeType>NPO</attributeType>");
            //strXml.Append(" </attributeDetails>");
            //strXml.Append(" </overrideInformation>");

            strXml.Append("<pricingOptionGroup>");
            strXml.Append("<pricingOptionKey>");
            strXml.Append(" <pricingOptionKey>RLO</pricingOptionKey>");
            strXml.Append("</pricingOptionKey>");
            strXml.Append("</pricingOptionGroup>");

            strXml.Append("<pricingOptionGroup>");
            strXml.Append("<pricingOptionKey>");
            strXml.Append(" <pricingOptionKey>RP</pricingOptionKey>");
            strXml.Append("</pricingOptionKey>");
            strXml.Append("</pricingOptionGroup>");

            strXml.Append("<pricingOptionGroup>");
            strXml.Append("<pricingOptionKey>");
            strXml.Append(" <pricingOptionKey>RU</pricingOptionKey>");
            strXml.Append("</pricingOptionKey>");
            strXml.Append("</pricingOptionGroup>");

            //if (fbr.isBookAFF)
            //{
            //    //strXml.Append("<pricingOptionGroup>");
            //    //strXml.Append("<pricingOptionKey>");
            //    //strXml.Append(" <pricingOptionKey>PFF</pricingOptionKey>");
            //    //strXml.Append("</pricingOptionKey>");
            //    //strXml.Append("</pricingOptionGroup>");
            //}

            //strXml.Append(" <overrideInformation>");
            //strXml.Append(" <attributeDetails>");
            //strXml.Append(" <attributeType>RU</attributeType>");
            //strXml.Append(" </attributeDetails>");

            //strXml.Append(" <attributeDetails>");
            //strXml.Append(" <attributeType>RP</attributeType>");
            //strXml.Append(" </attributeDetails>");


            //strXml.Append(" <attributeDetails>");
            //strXml.Append(" <attributeType>RLO</attributeType>");
            //strXml.Append(" </attributeDetails>");



            //strXml.Append(" </overrideInformation>");

            strXml.Append("<pricingOptionGroup>");
            strXml.Append("  <pricingOptionKey>");
            strXml.Append("    <pricingOptionKey>VC</pricingOptionKey>");
            strXml.Append("  </pricingOptionKey>");
            strXml.Append("  <carrierInformation>");
            strXml.Append("    <companyIdentification>");
            strXml.Append("      <otherCompany>" + validatingCarrier + "</otherCompany>");
            strXml.Append("    </companyIdentification>");
            strXml.Append("  </carrierInformation>");
            strXml.Append("</pricingOptionGroup>");



            //if (string.IsNullOrWhiteSpace(validatingCarrier) == false)
            //{
            //    strXml.Append("<validatingCarrier>");
            //    strXml.Append("<carrierInformation>");
            //    strXml.Append("<carrierCode>" + validatingCarrier + "</carrierCode>");
            //    strXml.Append("</carrierInformation>");
            //    strXml.Append("</validatingCarrier>");
            //}


            strXml.Append("</Fare_PricePNRWithBookingClass>");
            strXml.Append("</s:Body>");
            strXml.Append("</s:Envelope>");
            #endregion

            return strXml.ToString();
        }
        private string GetBillingAddress(PaymentDetails payment)
        {

            string billingAdd = string.Empty;
            try
            {
                bool isAdd2 = false;
                string temp = string.Empty;
                if (payment != null)
                {
                    temp = string.Format("{0}, {1}", SMCommanMethod.replaceSpecialChar(payment.cardHolderName).Trim(), SMCommanMethod.replaceSpecialChar(payment.address1)).Trim();
                    if (!string.IsNullOrEmpty(payment.address2))
                    {
                        temp = string.Format("{0} {1}", temp, SMCommanMethod.replaceSpecialChar(payment.address2)).Trim();
                        isAdd2 = true;
                    }
                    temp = string.Format("{0}, {1}", temp, SMCommanMethod.replaceSpecialChar(payment.city)).Trim();
                    temp = string.Format("{0}, {1}", temp, SMCommanMethod.replaceSpecialChar(payment.state)).Trim();
                    if (!string.IsNullOrEmpty(payment.postalCode))
                    {
                        temp = string.Format("{0}, {1}", temp, payment.postalCode).Trim();
                    }
                    temp = string.Format("{0}, {1}", temp, payment.country).Trim();

                    if (temp.Length <= 199)
                    {
                        billingAdd = temp.ToUpper();
                    }
                    else
                    {
                        if (isAdd2)
                        {
                            billingAdd = temp.ToUpper().Replace(SMCommanMethod.replaceSpecialChar(payment.address2), "").ToUpper();
                            billingAdd = billingAdd.Length <= 199 ? billingAdd : billingAdd.Substring(0, 198);
                        }
                        else
                        {
                            billingAdd = billingAdd.Length <= 199 ? billingAdd : billingAdd.Substring(0, 198);
                        }
                    }
                }
            }
            catch //(Exception ex)
            {
                //Logger.Error(ex);

                billingAdd = SMCommanMethod.replaceSpecialChar(payment.cardHolderName).Trim().ToUpper();
            }
            return billingAdd;
        }

        private string returnSiteName(SiteId siteId)
        {
            switch (siteId)
            {
                case SiteId.FlightsMojoIN: return "FLIGHTSMOJO WEBSITE";
                default: return (siteId.ToString().ToUpper() + " WEBSITE");
            }
        }

        private string GetCabinType(CabinType cabinType)
        {
            string cabin = string.Empty;
            switch (cabinType)
            {
                case CabinType.Economy:
                    cabin = "M";
                    break;
                case CabinType.PremiumEconomy:
                    cabin = "W";
                    break;
                case CabinType.Business:
                    cabin = "C";
                    break;
                case CabinType.First:
                    cabin = "F";
                    break;
                default:
                    cabin = "M";
                    break;
            }
            return cabin;
        }

        public string GetSoapHeader(string soapAction, Boolean isSessionTrue)
        {
            AmadeusConfiguration objAmadeusConfiguration = new AmadeusConfiguration();
            StringBuilder str = new StringBuilder();
            str.Append("<s:Header>");
            #region WS-Addressing-header
            str.Append("<add:MessageID xmlns:add=\"http://www.w3.org/2005/08/addressing\">" + System.Guid.NewGuid() + "</add:MessageID>");
            str.Append("<add:Action xmlns:add=\"http://www.w3.org/2005/08/addressing\">http://webservices.amadeus.com/" + soapAction + "</add:Action>");//FMPTBQ_13_1_1A
            str.Append("<add:To xmlns:add=\"http://www.w3.org/2005/08/addressing\">" + AmadeusConfiguration.targetURL + objAmadeusConfiguration.wSAP + "</add:To>");
            #endregion
            str.Append("<link:TransactionFlowLink xmlns:link=\"http://wsdl.amadeus.com/2010/06/ws/Link_v1\" />");
            #region WS-Security-Header
            str.Append("<oas:Security xmlns:oas=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd\" xmlns:oas1=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd\">");

            str.Append("<oas:UsernameToken oas1:Id=\"UsernameToken-1\">");
            str.Append("<oas:Username>" + objAmadeusConfiguration.accountName + "</oas:Username>");
            str.Append("<oas:Nonce EncodingType=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-soap-message-security-1.0#Base64Binary\">" + objAmadeusConfiguration.nonce + "</oas:Nonce>");
            str.Append("<oas:Password Type=\"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordDigest\">" + objAmadeusConfiguration.password + "</oas:Password>");
            str.Append("<oas1:Created>" + objAmadeusConfiguration.timeStamp + "</oas1:Created>");
            str.Append("</oas:UsernameToken>");
            str.Append("</oas:Security>");
            #endregion
            #region Amadeus-Security-header
            str.Append("<AMA_SecurityHostedUser xmlns=\"http://xml.amadeus.com/2010/06/Security_v1\">");
            str.Append("<UserID AgentDutyCode=\"SU\" RequestorType=\"U\" PseudoCityCode=\"" + objAmadeusConfiguration.pseudoCityCode + "\" POS_Type=\"1\" />");
            str.Append("</AMA_SecurityHostedUser>");
            #endregion
            if (isSessionTrue == true)
            {
                str.Append("<awsse:Session TransactionStatusCode=\"Start\" xmlns:awsse=\"http://xml.amadeus.com/2010/06/Session_v3\"/>");
            }
            str.Append("</s:Header>");
            return str.ToString();
        }

        public string GetSessionSoapHeader(string soapAction, AmadeusSessionTemplate objSession, string TransactionStatusCode)
        {
            AmadeusConfiguration objAmadeusConfiguration = new AmadeusConfiguration();
            StringBuilder str = new StringBuilder();
            str.Append("<s:Header>");
            #region WS-Addressing-header
            str.Append("<add:MessageID xmlns:add=\"http://www.w3.org/2005/08/addressing\">" + System.Guid.NewGuid() + "</add:MessageID>");
            str.Append("<add:Action xmlns:add=\"http://www.w3.org/2005/08/addressing\">http://webservices.amadeus.com/" + soapAction + "</add:Action>");//FMPTBQ_13_1_1A
            str.Append("<add:To xmlns:add=\"http://www.w3.org/2005/08/addressing\">" + AmadeusConfiguration.targetURL + objAmadeusConfiguration.wSAP + "</add:To>");
            #endregion
            str.Append("<awsse:Session TransactionStatusCode=\"" + TransactionStatusCode + "\" xmlns:awsse=\"http://xml.amadeus.com/2010/06/Session_v3\">");
            str.Append(" <awsse:SessionId>" + objSession.SessionId + "</awsse:SessionId>");
            str.Append("<awsse:SequenceNumber>" + (objSession.SequenceNumber + 1) + "</awsse:SequenceNumber>");
            str.Append("<awsse:SecurityToken>" + objSession.SecurityToken + "</awsse:SecurityToken>");
            str.Append("</awsse:Session>");
            str.Append("</s:Header>");
            return str.ToString();
        }

    }
}
