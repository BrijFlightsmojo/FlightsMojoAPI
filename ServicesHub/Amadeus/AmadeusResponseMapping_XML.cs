using Core;
using Core.Flight;
using ServicesHub;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Globalization;
using Newtonsoft.Json;
using System.Xml;
namespace ServicesHub.Amadeus
{
    public class AmadeusResponseMapping_XML
    {


        public void getPriceVerifyResponse(string xmlResponse, GfPriceVerifyRequest request, ref GfPriceVerifyResponse Response, ref int adtQnty, ref int chdQnty, ref int infQnty)
        {
            Response.fare.fareBreakdown = new List<Core.Flight.FareBreakdown>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(SMCommanMethod.RemoveAllNamespaces(xmlResponse));

            if (xmlDoc.SelectSingleNode("Envelope/Body/Fare_InformativeBestPricingWithoutPNRReply/mainGroup/pricingGroupLevelGroup") != null)
            {
                foreach (XmlNode xmlNode in xmlDoc.SelectNodes("Envelope/Body/Fare_InformativeBestPricingWithoutPNRReply/mainGroup/pricingGroupLevelGroup"))
                {
                    if (xmlNode.SelectSingleNode("numberOfPax/segmentControlDetails/quantity") != null)
                    {
                        if (Convert.ToInt32(xmlNode.SelectSingleNode("numberOfPax/segmentControlDetails/quantity").InnerText) == adtQnty)
                        {
                            decimal totAmt = Convert.ToDecimal(xmlNode.SelectSingleNode("fareInfoGroup/fareAmount/otherMonetaryDetails/amount").InnerText);
                            decimal BaseFare = Convert.ToDecimal(xmlNode.SelectSingleNode("fareInfoGroup/fareAmount/monetaryDetails/amount").InnerText);
                            decimal Tax = totAmt - BaseFare;

                            Response.fare.BaseFare += (BaseFare * request.adults);
                            Response.fare.Tax += (Tax * request.adults);
                            Core.Flight.FareBreakdown fb = new Core.Flight.FareBreakdown() { BaseFare = BaseFare, Tax = Tax, PassengerType = PassengerType.Adult };
                            Response.fare.fareBreakdown.Add(fb);
                        }
                        if (Convert.ToInt32(xmlNode.SelectSingleNode("numberOfPax/segmentControlDetails/quantity").InnerText) == chdQnty)
                        {
                            decimal totAmt = Convert.ToDecimal(xmlNode.SelectSingleNode("fareInfoGroup/fareAmount/otherMonetaryDetails/amount").InnerText);
                            decimal BaseFare = Convert.ToDecimal(xmlNode.SelectSingleNode("fareInfoGroup/fareAmount/monetaryDetails/amount").InnerText);
                            decimal Tax = totAmt - BaseFare;

                            Response.fare.BaseFare += (BaseFare * request.child);
                            Response.fare.Tax += (Tax * request.child);
                            Core.Flight.FareBreakdown fb = new Core.Flight.FareBreakdown() { BaseFare = BaseFare, Tax = Tax, PassengerType = PassengerType.Child };
                            Response.fare.fareBreakdown.Add(fb);
                        }
                        if (Convert.ToInt32(xmlNode.SelectSingleNode("numberOfPax/segmentControlDetails/quantity").InnerText) == infQnty)
                        {
                            decimal totAmt = Convert.ToDecimal(xmlNode.SelectSingleNode("fareInfoGroup/fareAmount/otherMonetaryDetails/amount").InnerText);
                            decimal BaseFare = Convert.ToDecimal(xmlNode.SelectSingleNode("fareInfoGroup/fareAmount/monetaryDetails/amount").InnerText);
                            decimal Tax = totAmt - BaseFare;

                            Response.fare.BaseFare += (BaseFare * request.infants);
                            Response.fare.Tax += (Tax * request.infants);
                            Core.Flight.FareBreakdown fb = new Core.Flight.FareBreakdown() { BaseFare = BaseFare, Tax = Tax, PassengerType = PassengerType.Infant };
                            Response.fare.fareBreakdown.Add(fb);
                        }
                    }
                }
                Response.fare.NetFare = Response.fare.grandTotal = Response.fare.BaseFare + Response.fare.Tax;
            }
            else
            {
                Response.responseStatus.status = TransactionStatus.Error;
            }
        }

        private static CabinType GetCabinType(string cabin)
        {
            CabinType cabinReturn = CabinType.Economy;
            switch (cabin)
            {
                case "M":
                    cabinReturn = CabinType.Economy;
                    break;
                case "W":
                    cabinReturn = CabinType.PremiumEconomy;
                    break;
                case "C":
                    cabinReturn = CabinType.Business;
                    break;
                case "F":
                    cabinReturn = CabinType.First;
                    break;
                default:
                    cabinReturn = CabinType.None;
                    break;
            }
            return cabinReturn;
        }
        public string Totitle(string reqstr)
        {
            if (string.IsNullOrEmpty(reqstr) == false)
            {
                return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(reqstr.ToLower());
            }
            else
            {
                return string.Empty;
            }
        }
        private FareType GetFareType(string fareType)
        {
            FareType fareT = FareType.PUBLISH;
            switch (fareType.ToUpper())
            {
                case "RP":
                    fareT = FareType.PUBLISH;
                    break;
                default:
                    fareT = FareType.PRIVATE;
                    break;
            }
            return fareT;
        }
        public static int getTime(int result)
        {
            int min = result % 100;
            int hour = result / 100;
            var finalResult = min + (hour * 60);
            return finalResult;
        }

        public class referenceNumber
        {
            public string refNumber { get; set; }
            public string refQualifier { get; set; }
        }
        public class airlineRules
        {
            public string Rule { get; set; }
        }

        public class fareDetails
        {
            public string itemNumber { get; set; }
            public CabinType bookingClass { get; set; }
            public decimal netFare { get; set; }
            public decimal tax { get; set; }
            public List<referenceNumber> references { get; set; }
            public string valCarrier { get; set; }
            public List<string> rdb { get; set; }
            public List<string> FareType { get; set; }
            public List<string> lstCabin { get; set; }
            public Dictionary<string, List<string>> SNDCode { get; set; }
            public decimal adultFare { get; set; }
            public decimal adultTax { get; set; }
            public decimal childFare { get; set; }
            public decimal childTax { get; set; }
            public decimal infantFare { get; set; }
            public decimal infantTax { get; set; }
            public decimal infantWsFare { get; set; }
            public decimal infantWsTax { get; set; }
            public string maxSeat { get; set; }
            //public List<string> rules { get; set; }
        }
    }

}
