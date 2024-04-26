using Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ServicesHub
{
    public class SMCommanMethod
    {
        public static string GetCardCode(string cardCode)
        {
            if (cardCode.Equals("Visa", StringComparison.OrdinalIgnoreCase) || cardCode.Equals("VI", StringComparison.OrdinalIgnoreCase))
            {
                return "VI";
            }
            else if (cardCode.Equals("MasterCard", StringComparison.OrdinalIgnoreCase) || cardCode.Equals("Master Card", StringComparison.OrdinalIgnoreCase) || cardCode.Equals("CA", StringComparison.OrdinalIgnoreCase))
            {
                return "CA";
            }
            else if (cardCode.Equals("AmericanExpress", StringComparison.OrdinalIgnoreCase) || cardCode.Equals("American Express", StringComparison.OrdinalIgnoreCase) || cardCode.Equals("AX", StringComparison.OrdinalIgnoreCase))
            {
                return "AX";
            }
            else if (cardCode.Equals("Discover", StringComparison.OrdinalIgnoreCase) || cardCode.Equals("DS", StringComparison.OrdinalIgnoreCase))
            {
                return "DS";
            }
            else if (cardCode.Equals("DinersClub", StringComparison.OrdinalIgnoreCase) || cardCode.Equals("Diners Club", StringComparison.OrdinalIgnoreCase) || cardCode.Equals("DC", StringComparison.OrdinalIgnoreCase))
            {
                return "DC";
            }
            else if (cardCode.Equals("CarteBlanche", StringComparison.OrdinalIgnoreCase) || cardCode.Equals("Carte Blanche", StringComparison.OrdinalIgnoreCase) || cardCode.Equals("R", StringComparison.OrdinalIgnoreCase))
            {
                return "R";
            }
            else
            {
                return "VI";
            }
        }
        public static string GetExpYY(string expYear)
        {
            return (expYear.Length == 4 ? expYear.Substring(2, 2) : expYear);
        }
        public static string GetExpMM(string expMonth)
        {
            return (expMonth.Length == 1 ? ("0" + expMonth) : expMonth);
        }
        public static string getExpYYYY(string strYear)
        {
            if (strYear.Length == 2)
            {
                strYear = "20" + strYear;
            }
            return strYear;
        }
        public static string replaceSpecialChar(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            else if (str.Trim().IndexOf(" ") != -1)
            {
                string ss = "";
                foreach (string kk in str.Trim().Replace("   ", " ").Replace("  ", " ").Split(' '))
                {
                    ss += (System.Text.RegularExpressions.Regex.Replace(kk, "[^a-zA-Z0-9]+", "") + " ");
                }
                return ss.Trim();
            }
            else
            {
                return System.Text.RegularExpressions.Regex.Replace(str, "[^a-zA-Z0-9]+", "");
            }
        }
        public static string replaceSpecialCharOld(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return "";
            }
            else
            {
                return System.Text.RegularExpressions.Regex.Replace(str, "[^a-zA-Z0-9]+", "");
            }
        }
        public static string GetCabinCode(string CabinClass)
        {
            switch (CabinClass)
            {
                case "e":
                case "1":
                case "Economy":
                case "economy":
                case "ECONOMY":
                    return "ECON";
                case "p":
                case "2":
                case "PREMECON":
                case "PremEcon":
                case "premecon":
                    return "PREMECON";
                case "b":
                case "3":
                case "BUSINESS":
                case "Business":
                case "business":
                    return "BUSINESS";
                case "f":
                case "4":
                case "FIRST":
                case "first":
                case "First":
                    return "FIRST";
                default:
                    return "ECON";
            }
        }
        public static CabinType getCabinTypeName(string cabin, string gdsType = "")
        {
            CabinType objCabinType = CabinType.None;
            if (cabin == "Y")
            {
                //if (gdsType == "SABRE")
                //{
                //    objCabinType = CabinType.EconomyStandard;
                //}
                //else
                //{
                objCabinType = CabinType.Economy;
                //}

            }
            else if (cabin == "M")
            {
                objCabinType = CabinType.Economy;
            }
            else if (cabin == "W" || cabin == "S")
            {
                objCabinType = CabinType.PremiumEconomy;
            }
            else if (cabin == "C")
            {
                objCabinType = CabinType.Business;
            }
            else if (cabin == "F")
            {
                objCabinType = CabinType.First;
            }
            return objCabinType;

        }
        public static Core.MojoFareType getMojoFareType(FareType strfType)
        {
            Core.MojoFareType fmFareType = MojoFareType.None;
            if (strfType == FareType.PUBLISH || strfType == FareType.REGULAR || strfType == FareType.PRIVATE || strfType == FareType.FAMILYFARE || strfType == FareType.SALE
                || strfType == FareType.SUPER6E || strfType == FareType.SAVER || strfType == FareType.Eco_Lite)
            {
                fmFareType = Core.MojoFareType.Publish;
            }
            else if (strfType == FareType.CORPORATEFLEX || strfType == FareType.CORP_CONNECT || strfType == FareType.CORPORATE || strfType == FareType.CORPORATE_GOMORE
                || strfType == FareType.FLEXI || strfType == FareType.FLEXIFARE || strfType == FareType.FLEXIPLUS || strfType == FareType.PREMIUMFLEX
                || strfType == FareType.FLX || strfType == FareType.FLEX || strfType == FareType.EXPRESS_FLEXI
                || strfType == FareType.GOFLEXI || strfType == FareType.FLEXIBLE
                || strfType == FareType.VISTA_FLEX || strfType == FareType.SMECRPCON || strfType == FareType.SME || strfType == FareType.ECONOMY || strfType == FareType.VALUE
                  || strfType == FareType.FareFlexi_Saver_Economy || strfType == FareType.Super_Flexible_Business
                  || strfType == FareType.Flexible_Economy || strfType == FareType.Flexi_Saver_Economy
                )
            {
                fmFareType = Core.MojoFareType.Corporate;
            }
            else if (strfType == FareType.OFFER_FARE_WITHOUT_PNR || strfType == FareType.OFFER_RETURN_FARE_WITHOUT_PNR
                || strfType == FareType.RTSPLFARE)
            {
                fmFareType = Core.MojoFareType.SeriesFareWithoutPNR;
            }
            else if (strfType == FareType.OFFERFARE || strfType == FareType.OFFER_FARE_WITH_PNR || strfType == FareType.OFFER_RETURN_FARE_WITH_PNR
                || strfType == FareType.DEALFARE || strfType == FareType.RETAILFARE || strfType == FareType.INST_SERIESPUR || strfType == FareType.INSTANTPUR)
            {
                fmFareType = Core.MojoFareType.SeriesFareWithPNR;
            }
            else if (strfType == FareType.SPECIALRETURN)
            {
                fmFareType = Core.MojoFareType.SpecialReturn;
            }
            //else if (strfType == FareType.SPICEMAX || strfType == FareType.COUPON || strfType == FareType.COUPON_FARE)
            //{
            //    fmFareType = Core.MojoFareType.Coupon;
            //}
            else if (strfType == FareType.PROMO || strfType == FareType.TACTICAL || strfType == FareType.SPECIAL || strfType == FareType.GOMOREFARE
                || strfType == FareType.GOMORE || strfType == FareType.COMFORT || strfType == FareType.SPICEMAX || strfType == FareType.COUPON || strfType == FareType.COUPON_FARE)
            {
                fmFareType = Core.MojoFareType.Promo;
            }

            else if (strfType == FareType.Business_Standard || strfType == FareType.Web_Special_Business || strfType == FareType.Super_Value_Business
                || strfType == FareType.BUSINESS || strfType == FareType.Business_Comfort)
            {
                fmFareType = Core.MojoFareType.Business;
            }

            else if (strfType == FareType.NONE)
            {
                fmFareType = Core.MojoFareType.Unknown;
            }
            else
            {
                fmFareType = Core.MojoFareType.Unknown;
            }
            return fmFareType;
        }


        public static SubProvider getSubProvider(string subProvider)
        {
            SubProvider sp = SubProvider.None;
            if (!string.IsNullOrEmpty(subProvider))
            {
                subProvider = subProvider.Replace(" ", "").ToUpper();
                if ("AIRIQ".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.AirIQ; }
                else if ("YATRATRAVELSESERVICES".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.YatraTravelsEServices; }
                else if ("ECONOMICTRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.EconomicTravels; }
                else if ("INDIANGAMERS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.IndianGamers; }
                else if ("KAVERITRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.KAVERITRAVELS; }
                else if ("CHEAPFIXDEPARTURE".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.CheapFixDeparture; }
                else if ("TRANSGLOBALHOLIDAYS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.TransglobalHolidays; }
                else if ("FLYNEXT".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.FLYNEXT; }
                else if ("JUSTMYTRIP.IN".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.JUSTMYTRIP_IN; }
                else if ("SATKARTRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.SATKARTRAVELS; }
                else if ("AVMHOLIDAYS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.AVMHOLIDAYS; }
                else if ("METROPOLITANTRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.METROPOLITANTRAVELS; }
                else if ("OMTOURSANDTRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.OMTOURSANDTRAVELS; }
                else if ("DIAMONDAIRSERVICESPVT.LTD.".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.DIAMONDAIRSERVICESPVT_LTD; }
                else if ("SSTRAVELHOUSE".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.SSTRAVELHOUSE; }
                else if ("QIBLATAINTRAVELSPVTLTD".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.QIBLATAINTRAVELSPVTLTD; }
                else if ("AIRTICKETSERVICEINDIAPVTLTD".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.AIRTICKETSERVICEINDIAPVTLTD; }
                else if ("DESTINYTOURSANDTRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.DESTINYTOURSANDTRAVELS; }
                else if ("ABHISHEKAIRTICKET".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.ABHISHEKAIRTICKET; }
                else if ("BESTFARES".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.BESTFARES; }
                else if ("FDWALA".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.FDWALA; }
                else if ("METROTRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.METROTRAVELS; }
                else if ("TRIPCIRCUITHOLIDAYSPVTLTD".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.TRIPCIRCUITHOLIDAYSPVTLTD; }
                else if ("TRIPMAKETRAVELPVT.LTD.".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.TRIPMAKETRAVELPVT_LTD; }
                else if ("YATRATRAVELS&ESERVICES".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.YATRATRAVELSANDESERVICES; }
                else if ("ONLINESERVICES".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.ONLINESERVICES; }
                else if ("KALAWATITOUR&TRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.KALAWATITOURANDTRAVELS; }
                else if ("MAHALAXMITOURSANDTRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.MAHALAXMITOURSANDTRAVELS; }
                else if ("JOURNEYWITHUS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.JOURNEYWITHUS; }
                else if ("BALAJITOURANDTRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.BALAJITOURANDTRAVELS; }
                else if ("MALITRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.MALITRAVELS; }
                else if ("SETURTRIP".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.SETURTRIP; }
                else if ("CLICKMYLINKSHOLIDAYS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.CLICKMYLINKSHOLIDAYS; }
                else if ("HRTRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.HRTRAVELS; }
                else if ("FLYBIHAR".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.FLYBIHAR; }
                else if ("MAHAVEERTHETRAVELSHOPPVTLTD".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.MAHAVEERTHETRAVELSHOPPVTLTD; }
                else if ("GOFARE".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.GOFARE; }
                else if ("STARTRAVELS & HOLIDAY".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.STARTRAVELSANDHOLIDAY; }
                else if ("OMSAITOURSANDTRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.OMSAITOURSANDTRAVELS; }
                else if ("AIRTB".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.AIRTB; }
                else if ("TRIPFACTORY".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.TRIPFACTORY; }
                else if ("BARKATTOURS&TRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.BARKATTOURS_TRAVELS; }
                else if ("TRAVELMASTER".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.TRAVELMASTER; }
                else if ("SADATTRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.SADATTRAVELS; }
                else if ("AIRBLR".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.AIRBLR; }
                else if ("DOLPHINTRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.DOLPHINTRAVELS; }
                else if ("SANJAYTOURANDTRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.SANJAYTOURANDTRAVELS; }
                else if ("TRAVELMASTER.IN".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.TRAVELMASTER_IN; }
                else if ("ALELAHITRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.ALELAHITRAVELS; }

                else if ("AIRTB(ANGRAGVENTURES)".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.AIRTB_ANGRAGVENTURES; }
                else if ("SASTITICKET".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.SASTITICKET; }
                else if ("STARTRAVELS&HOLIDAY".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.STARTRAVELS_HOLIDAY; }
                else if ("SAADATTRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.SAADATTRAVELS; }
                else if ("CHOUDHARYTRAVELS(AIRBLR)".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.CHOUDHARYTRAVELS_AIRBLR; }
                else if ("ABHISHEKAIRTICKET(AIRABHI)".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.ABHISHEKAIRTICKET_AIRABHI; }
                else if ("DEALFARE".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.DEALFARE; }
                else if ("FREQUENTFLYERS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.FREQUENTFLYERS; }
                else if ("SKYLINETRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.SKYLINETRAVELS; }

                else if ("PLANMYESCAPE".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.PLANMYESCAPE; }
                else if ("SKYAMARTRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.SKYAMARTRAVELS; }
                else if ("BOOKMYFLYINDIAPVTLTD".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.BOOKMYFLYINDIAPVTLTD; }
                else if ("KUMAWATTRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.KUMAWATTRAVELS; }
                else if ("SAMRATTOURANDTRAVELS".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.SAMRATTOURANDTRAVELS; }

                else if ("VINAYAKINTERNATIONAL".Equals(subProvider, StringComparison.OrdinalIgnoreCase)) { sp = SubProvider.VINAYAKINTERNATIONAL; }
            }
            if (sp == SubProvider.None)
            {
                LogCreater.CreateLogFile(subProvider, "Log\\SubProvider", DateTime.Today.ToString("ddMMyyy"), "SubProvider.txt");
            }
            return sp;

        }

        public static string RemoveAllNamespaces(string xmlDocument)
        {
            XElement xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Parse(xmlDocument));

            return xmlDocumentWithoutNs.ToString();
        }
        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                XElement xElement = new XElement(xmlDocument.Name.LocalName);
                xElement.Value = xmlDocument.Value;

                foreach (XAttribute attribute in xmlDocument.Attributes())
                    xElement.Add(attribute);

                return xElement;
            }
            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespaces(el)));
        }
        //public static XElement RemoveNamespaces(string strXML)
        //{
        //    XElement xDocument = XElement.Parse(strXML);
        //    if (!xDocument.HasElements)
        //    {
        //        XElement xElement = new XElement(xDocument.Name.LocalName);
        //        xElement.Value = xDocument.Value;

        //        foreach (XAttribute attribute in xDocument.Attributes())
        //            xElement.Add(attribute);

        //        return xElement;
        //    }
        //    return new XElement(xDocument.Name.LocalName, xDocument.Elements().Select(el => RemoveAllNamespaces(el)));
        //}
    }
}
