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

            strFare_BestPricing.Append("  <pricingOptionGroup>");
            strFare_BestPricing.Append("    <pricingOptionKey>");
            strFare_BestPricing.Append("      <pricingOptionKey>NVO</pricingOptionKey>");
            strFare_BestPricing.Append("    </pricingOptionKey>");
            strFare_BestPricing.Append("  </pricingOptionGroup>");
            strFare_BestPricing.Append("</Fare_InformativeBestPricingWithoutPNR>");




            strFare_BestPricing.Append("</s:Body>");
            strFare_BestPricing.Append("</s:Envelope>");

            return strFare_BestPricing.ToString();
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
