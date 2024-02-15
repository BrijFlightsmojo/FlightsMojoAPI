using Core.Flight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;

namespace Core
{
    public class FlightUtility
    {
        public static List<Airport> AirportList { get; set; }
        public static List<Airline> AirlineList { get; set; }
        public static List<AircraftDetail> AircraftDetails { get; set; }
        //public static List<FlightSupplier> lstFlightProviderList { get; set; }
        public static List<FlightSupplierNew> SupplierList { get; set; }
        public static List<BookingManagements> BookingManagementsList { get; set; }
        public static List<AirlineBlock> airlineBlockList { get; set; }
        public static List<Affiliate> AffiliateList { get; set; }
        public static List<SupplierFareType> FareTypeList { get; set; }
        public static List<AirlineCommissionRule> AirlineCommissionRuleList { get; set; }
        public static bool isWriteLog = false;
        public static bool isWriteLogSearch = false;
        public static bool isMasterDataLoaded = false;
        public static void LoadMasterData()
        {
            FlightUtility obj = new FlightUtility();
            AirportList = obj.getAllAirport();
            AirlineList = obj.getAllAirline();
            //AircraftDetails = obj.getAllAircraftDetail();
            //lstFlightProviderList = obj.GetFlightProviderList();
            SupplierList = obj.GetSupplierList();
            BookingManagementsList = obj.GetBookingManagementsList();
            airlineBlockList = obj.GetAllBlackListedAirline();
            AffiliateList = obj.GetAllAffiliate();
            FareTypeList = obj.GetAllFareType();
            AirlineCommissionRuleList = obj.GetAllAirlineCommissionRule();
            isMasterDataLoaded = true;
            isWriteLog = Convert.ToBoolean(ConfigurationManager.AppSettings["isWriteLog"] != null ? ConfigurationManager.AppSettings["isWriteLog"] : "false");
            isWriteLogSearch = Convert.ToBoolean(ConfigurationManager.AppSettings["isWriteLogSearch"] != null ? ConfigurationManager.AppSettings["isWriteLogSearch"] : "false");
        }
        public static Airport GetAirport(string AirportCode)
        {
            if (!FlightUtility.isMasterDataLoaded)
            {
                FlightUtility.LoadMasterData();
            }

            List<Airport> ResAirportCode = FlightUtility.AirportList.Where(x => (x.airportCode.Equals(AirportCode, StringComparison.OrdinalIgnoreCase))).ToList();
            if (ResAirportCode.Count > 0)
            {
                return ResAirportCode[0];
            }
            else
            {
                return new Airport()
                {
                    airportCode = AirportCode,
                    airportName = AirportCode,
                    cityCode = AirportCode,
                    cityName = AirportCode,
                    countryCode = AirportCode.ToUpper(),
                    countryName = AirportCode
                };
            }
        }
        public static Airline GetAirline(string AirlineCode)
        {
            if (!FlightUtility.isMasterDataLoaded)
            {
                FlightUtility.LoadMasterData();
            }

            List<Airline> ResAirlineCode = FlightUtility.AirlineList.Where(x => (x.code.Equals(AirlineCode, StringComparison.OrdinalIgnoreCase))).ToList();
            if (ResAirlineCode.Count > 0)
            {
                return ResAirlineCode[0];
            }
            else
            {
                return new Airline()
                {
                    code = AirlineCode,
                    name = AirlineCode
                };
            }
        }
        public static AircraftDetail GetAircraftDetail(string AircraftCode)
        {
            if (!FlightUtility.isMasterDataLoaded)
            {
                FlightUtility.LoadMasterData();
            }

            List<AircraftDetail> Aircraft = FlightUtility.AircraftDetails.Where(x => (x.aircraftCode.Equals(AircraftCode, StringComparison.OrdinalIgnoreCase))).ToList();
            if (Aircraft.Count > 0)
            {
                return Aircraft[0];
            }
            else
            {
                return new AircraftDetail()
                {
                    aircraftCode = AircraftCode,
                    aircraftName = "No Information",
                    bodyType = "",
                    formOfPropulsion = "",
                    NoOfSeat = ""
                };
            }
        }
        public static Affiliate GetAffiliate(string SourceMedia)
        {
            if (!FlightUtility.isMasterDataLoaded)
            {
                FlightUtility.LoadMasterData();
            }

            Affiliate aff = FlightUtility.AffiliateList.Where(x => (x.AffiliateId.Equals(SourceMedia, StringComparison.OrdinalIgnoreCase))).ToList().FirstOrDefault();
            if (aff == null)
            {
                aff = new Affiliate()
                {
                    AffiliateName = "",
                    CardConFee = "",
                    AffiliateId = "",
                    EmiConFee = "",
                    NetBankingConFee = "",
                    PayLaterConFee = "",
                    UPIConFee = "",
                    WalletConFee = ""

                };
            }
            return aff;
        }
        public static List<string> MajorAirline = ("AI,UK,QP,6E,SG,I5,IX").Split(',').ToList();
        public static Core.MojoFareType GetFmFareType(string fareType, string Airline, GdsType gds)
        {
            if (!FlightUtility.isMasterDataLoaded)
            {
                FlightUtility.LoadMasterData();
            }
            if (MajorAirline.Contains(Airline))
            {
                SupplierFareType fType = FlightUtility.FareTypeList.Where(x => (x.Airline.Equals(Airline, StringComparison.OrdinalIgnoreCase) &&
                x.ProviderFareType.Equals(fareType, StringComparison.OrdinalIgnoreCase) && x.Provider == gds)).ToList().FirstOrDefault();
                if (fType == null)
                {
                    return MojoFareType.Unknown;
                }
                else
                {
                    return fType.FMFareType;
                }
            }
            else
            {
                return MojoFareType.Publish;
            }
        }
        #region GetData From DB
        public List<Airport> getAllAirport()
        {
            List<Airport> arpList = new List<Airport>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("select * from airport_details1  order by AirportCode, CityName ", con))
                {
                    try
                    {
                        con.Open();
                        SqlDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                Airport arp = new Airport();
                                arp.airportCode = dr["AirportCode"].ToString().ToUpper();
                                arp.airportName = dr["AirportName"].ToString();
                                arp.cityCode = dr["CityCode"].ToString().ToUpper();
                                arp.cityName = dr["CityName"].ToString();
                                arp.countryCode = dr["Country"].ToString().ToUpper();
                                arp.countryName = dr["CountryName"].ToString();

                                if (arp.countryName.ToUpper() == "USA")
                                {
                                    arp.countryName = arp.countryName.ToUpper();
                                }
                                arpList.Add(arp);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return arpList;
        }

        public List<Airline> getAllAirline()
        {
            List<Airline> arlList = new List<Airline>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Airline_details order by [Name]", con))
                {
                    try
                    {
                        con.Open();
                        SqlDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                Airline arp = new Airline();
                                arp.code = dr["Code"].ToString().ToUpper();
                                //arp.name = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(dr["Name"].ToString().ToLower());
                                arp.name = dr["Name"].ToString();
                                arlList.Add(arp);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return arlList;
        }

        public List<AirlineBlock> GetAllBlackListedAirline()
        {
            List<AirlineBlock> bla = new List<AirlineBlock>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM [AirlineBlocksNew] where isActive='true' ", con))
                {
                    try
                    {
                        con.Open();
                        SqlDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                AirlineBlock objBLA = new AirlineBlock();
                                objBLA.SiteId = (SiteId)Convert.ToInt32(dr["SiteId"]);
                                if (string.IsNullOrEmpty(Convert.ToString(dr["AffiliateId"])) == false)
                                {
                                    objBLA.AffiliateId = Convert.ToString(dr["AffiliateId"]).Split('-').ToList();
                                }
                                if (string.IsNullOrEmpty(Convert.ToString(dr["AffiliateId_Not"])) == false)
                                {
                                    objBLA.AffiliateId_Not = Convert.ToString(dr["AffiliateId_Not"]).Split('-').ToList();
                                }
                                if (string.IsNullOrEmpty(Convert.ToString(dr["CountryFrom"])) == false)
                                {
                                    List<string> lstString = Convert.ToString(dr["CountryFrom"]).Split('-').ToList();
                                    List<string> lstCountry = new List<string>();
                                    foreach (string item in lstString)
                                    {
                                        if (item.Length == 2)
                                        {
                                            objBLA.CountryFrom.Add(item);
                                        }
                                        else if (item.Length == 4 || item.Length == 5)
                                        {
                                            setContinentCountry(item, ref lstCountry);
                                        }
                                    }
                                    foreach (string item in lstCountry)
                                    {
                                        objBLA.CountryFrom.Add(item);
                                    }
                                }
                                if (string.IsNullOrEmpty(Convert.ToString(dr["CountryFrom_Not"])) == false)
                                {
                                    List<string> lstString = Convert.ToString(dr["CountryFrom_Not"]).Split('-').ToList();
                                    List<string> lstCountry = new List<string>();
                                    foreach (string item in lstString)
                                    {
                                        if (item.Length == 2)
                                        {
                                            objBLA.CountryFrom_Not.Add(item);
                                        }
                                        else if (item.Length == 4 || item.Length == 5)
                                        {
                                            setContinentCountry(item, ref lstCountry);
                                        }
                                    }
                                    foreach (string item in lstCountry)
                                    {
                                        objBLA.CountryFrom_Not.Add(item);
                                    }
                                }

                                if (string.IsNullOrEmpty(Convert.ToString(dr["CountryTo"])) == false)
                                {
                                    List<string> lstString = Convert.ToString(dr["CountryTo"]).Split('-').ToList();
                                    List<string> lstCountry = new List<string>();
                                    foreach (string item in lstString)
                                    {
                                        if (item.Length == 2)
                                        {
                                            objBLA.CountryTo.Add(item);
                                        }
                                        else if (item.Length == 4 || item.Length == 5)
                                        {
                                            setContinentCountry(item, ref lstCountry);
                                        }
                                    }
                                    foreach (string item in lstCountry)
                                    {
                                        objBLA.CountryTo.Add(item);
                                    }
                                }
                                if (string.IsNullOrEmpty(Convert.ToString(dr["CountryTo_Not"])) == false)
                                {
                                    List<string> lstString = Convert.ToString(dr["CountryTo_Not"]).Split('-').ToList();
                                    List<string> lstCountry = new List<string>();
                                    foreach (string item in lstString)
                                    {
                                        if (item.Length == 2)
                                        {
                                            objBLA.CountryTo_Not.Add(item);
                                        }
                                        else if (item.Length == 4 || item.Length == 5)
                                        {
                                            setContinentCountry(item, ref lstCountry);
                                        }
                                    }
                                    foreach (string item in lstCountry)
                                    {
                                        objBLA.CountryTo_Not.Add(item);
                                    }
                                }
                                if (string.IsNullOrEmpty(Convert.ToString(dr["WeekOfDays"])) == false)
                                {
                                    foreach (string str in Convert.ToString(dr["WeekOfDays"]).Split('-').ToList())
                                    {
                                        if (str != "" && str != "0")
                                            objBLA.WeekOfDays.Add((WeekDays)Convert.ToInt16(str));
                                    }
                                }

                                objBLA.Supplier = (GdsType)Convert.ToInt32(dr["Supplier"]);
                                if (string.IsNullOrEmpty(Convert.ToString(dr["Airline"])) == false)
                                {
                                    objBLA.airline = Convert.ToString(dr["Airline"]).Split('-').ToList();
                                }
                                if (!string.IsNullOrEmpty(dr["FareType"].ToString()))
                                {
                                    foreach (var item in Convert.ToString(dr["FareType"]).Split('-').ToList())
                                    {
                                        if (!string.IsNullOrEmpty(item))
                                        {
                                            objBLA.FareType.Add((MojoFareType)Convert.ToInt32(item));
                                        }
                                    }
                                }

                                objBLA.Action = (AirlineBlockAction)Convert.ToInt32(dr["Action"]);
                                bla.Add(objBLA);
                            }
                        }

                        try
                        {
                            dr.Close();
                            con.Close();
                        }
                        catch { }

                    }
                    catch //(Exception ex)
                    {

                    }
                }
            }
            return bla;
        }

        public List<FlightSupplierNew> GetSupplierList()
        {
            List<FlightSupplierNew> lstFlightSupplier = new List<FlightSupplierNew>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM SupplierManagementsNew where IsActive=1", con))
                {
                    try
                    {
                        con.Open();
                        SqlDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                try
                                {
                                    FlightSupplierNew objSupp = new FlightSupplierNew() { FromAirport = new List<string>(), FromCountry = new List<string>(), ToAirport = new List<string>(), ToCountry = new List<string>(), FarePriority = 1, Provider = 0, siteId = 0, SourceMedia = new List<string>(), SourceMedia_Not = new List<string>() };
                                    objSupp.Provider = (GdsType)Convert.ToInt32(dr["Supplier"]);
                                    if (string.IsNullOrEmpty(Convert.ToString(dr["SourceMedia"])) == false)
                                    {
                                        objSupp.SourceMedia = Convert.ToString(dr["SourceMedia"]).Split('-').ToList();
                                    }

                                    if (string.IsNullOrEmpty(Convert.ToString(dr["SourceMedia_Not"])) == false)
                                    {
                                        objSupp.SourceMedia_Not = Convert.ToString(dr["SourceMedia_Not"]).Split('-').ToList();
                                    }


                                    if (string.IsNullOrEmpty(Convert.ToString(dr["Origin"])) == false)
                                    {
                                        List<string> lstString = Convert.ToString(dr["Origin"]).Split('-').ToList();
                                        List<string> lstCountry = new List<string>();
                                        foreach (string item in lstString)
                                        {
                                            if (item.Length == 2)
                                            {
                                                objSupp.FromCountry.Add(item);
                                            }
                                            else if (item.Length == 3)
                                            {
                                                objSupp.FromAirport.Add(item);
                                            }
                                            else if (item.Length == 4 || item.Length == 5)
                                            {
                                                setContinentCountry(item, ref lstCountry);
                                            }
                                        }
                                        foreach (string item in lstCountry)
                                        {
                                            objSupp.FromCountry.Add(item);
                                        }
                                    }
                                    if (string.IsNullOrEmpty(Convert.ToString(dr["Destination"])) == false)
                                    {
                                        List<string> lstString = Convert.ToString(dr["Destination"]).Split('-').ToList();
                                        List<string> lstCountry = new List<string>();
                                        foreach (string item in lstString)
                                        {
                                            if (item.Length == 2)
                                            {
                                                objSupp.ToCountry.Add(item);
                                            }
                                            else if (item.Length == 3)
                                            {
                                                objSupp.ToAirport.Add(item);
                                            }
                                            else if (item.Length == 4 || item.Length == 5)
                                            {
                                                setContinentCountry(item, ref lstCountry);
                                            }
                                        }
                                        foreach (string item in lstCountry)
                                        {
                                            objSupp.ToCountry.Add(item);
                                        }
                                    }
                                    objSupp.isAirIQ = string.IsNullOrEmpty(dr["IsAirIQ"].ToString()) ? false : Convert.ToBoolean(dr["IsAirIQ"]);
                                    objSupp.FarePriority = Convert.ToInt16(dr["Priority"]);
                                    objSupp.siteId = (SiteId)Convert.ToInt32(dr["SiteId"]);
                                    lstFlightSupplier.Add(objSupp);
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            return lstFlightSupplier;
        }

        public List<BookingManagements> GetBookingManagementsList()
        {

            List<BookingManagements> lstFlightSupplier = new List<BookingManagements>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM BookingManagements where IsActive=1", con))
                {
                    try
                    {
                        con.Open();
                        SqlDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                try
                                {
                                    BookingManagements objSupp = new BookingManagements()
                                    {

                                        FromCountry = new List<string>(),
                                        FromCountry_Not = new List<string>(),
                                        ToCountry = new List<string>(),
                                        ToCountry_Not = new List<string>(),

                                        AffiliateId = new List<string>(),
                                        AffiliateId_Not = new List<string>(),
                                        Airline = new List<string>(),
                                        Airline_Not = new List<string>(),
                                        BookingAction = BookingAction.NONE,
                                        FareType = new List<MojoFareType>(),
                                        SiteId = SiteId.FlightsMojoIN,
                                        Supplier = GdsType.None
                                    };
                                    objSupp.SiteId = (SiteId)Convert.ToInt32(dr["SiteId"]);
                                    objSupp.Supplier = (GdsType)Convert.ToInt32(dr["Supplier"]);
                                    objSupp.BookingAction = (BookingAction)Convert.ToInt32(dr["BookingAction"]);
                                    if (string.IsNullOrEmpty(Convert.ToString(dr["AffiliateId"])) == false)
                                    {
                                        objSupp.AffiliateId = Convert.ToString(dr["AffiliateId"]).Split('-').ToList();
                                    }
                                    if (string.IsNullOrEmpty(Convert.ToString(dr["AffiliateId_Not"])) == false)
                                    {
                                        objSupp.AffiliateId_Not = Convert.ToString(dr["AffiliateId_Not"]).Split('-').ToList();
                                    }

                                    if (string.IsNullOrEmpty(Convert.ToString(dr["FromCountry"])) == false)
                                    {
                                        objSupp.FromCountry = Convert.ToString(dr["FromCountry"]).Split('-').ToList();
                                    }
                                    if (string.IsNullOrEmpty(Convert.ToString(dr["FromCountry_Not"])) == false)
                                    {
                                        objSupp.FromCountry_Not = Convert.ToString(dr["FromCountry_Not"]).Split('-').ToList();
                                    }

                                    if (string.IsNullOrEmpty(Convert.ToString(dr["ToCountry"])) == false)
                                    {
                                        objSupp.FromCountry = Convert.ToString(dr["ToCountry"]).Split('-').ToList();
                                    }
                                    if (string.IsNullOrEmpty(Convert.ToString(dr["ToCountry_Not"])) == false)
                                    {
                                        objSupp.FromCountry_Not = Convert.ToString(dr["ToCountry_Not"]).Split('-').ToList();
                                    }

                                    if (string.IsNullOrEmpty(Convert.ToString(dr["Airline"])) == false)
                                    {
                                        objSupp.Airline = Convert.ToString(dr["Airline"]).Split('-').ToList();
                                    }
                                    if (string.IsNullOrEmpty(Convert.ToString(dr["Airline_Not"])) == false)
                                    {
                                        objSupp.Airline_Not = Convert.ToString(dr["Airline_Not"]).Split('-').ToList();
                                    }
                                    if (!string.IsNullOrEmpty(dr["FareType"].ToString()))
                                    {
                                        foreach (var item in Convert.ToString(dr["FareType"]).Split('-').ToList())
                                        {
                                            if (!string.IsNullOrEmpty(item))
                                            {
                                                objSupp.FareType.Add((MojoFareType)Convert.ToInt32(item));
                                            }
                                        }
                                    }
                                    lstFlightSupplier.Add(objSupp);
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }

            return lstFlightSupplier;
        }

        public List<Affiliate> GetAllAffiliate()
        {
            List<Affiliate> CF = new List<Affiliate>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM AffiliateDetailes where IsActive=1", con))
                {
                    try
                    {
                        con.Open();
                        SqlDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                Affiliate objCF = new Affiliate();
                                objCF.SiteID = (SiteId)Convert.ToInt32(dr["SiteID"]);
                                objCF.AffiliateId = dr["AffiliateId"].ToString();
                                objCF.AffiliateName = dr["AffiliateName"].ToString();
                                objCF.EmiConFee = dr["EmiConFee"].ToString();
                                objCF.PayLaterConFee = dr["PayLaterConFee"].ToString();
                                objCF.WalletConFee = dr["WalletConFee"].ToString();
                                objCF.NetBankingConFee = dr["NetBankingConFee"].ToString();
                                objCF.CardConFee = dr["CardConFee"].ToString();
                                objCF.UPIConFee = dr["UPIConFee"].ToString();
                                CF.Add(objCF);
                            }
                        }

                        try
                        {
                            dr.Close();
                            con.Close();
                        }
                        catch { }

                    }
                    catch //(Exception ex)
                    {

                    }
                }
            }
            return CF;
        }

        public List<SupplierFareType> GetAllFareType()
        {
            List<SupplierFareType> FT = new List<SupplierFareType>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM FareTypes", con))
                {
                    try
                    {
                        con.Open();
                        SqlDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                SupplierFareType objFT = new SupplierFareType();
                                objFT.FMFareType = string.IsNullOrEmpty(dr["FMFareType"].ToString() == "0" ? "" : dr["FMFareType"].ToString()) ? MojoFareType.Unknown : (MojoFareType)Convert.ToInt32(dr["FMFareType"]);

                                objFT.Airline = dr["Airline"].ToString();
                                objFT.ProviderFareType = dr["FareType"].ToString();
                                objFT.Provider = string.IsNullOrEmpty(dr["Provider"].ToString()) ? GdsType.None : (GdsType)Convert.ToInt32(dr["Provider"]);
                                FT.Add(objFT);
                            }
                        }
                        try
                        {
                            dr.Close();
                            con.Close();
                        }
                        catch { }
                    }
                    catch //(Exception ex)
                    {

                    }
                }
            }
            return FT;
        }

        public List<AirlineCommissionRule> GetAllAirlineCommissionRule()
        {
            List<AirlineCommissionRule> FT = new List<AirlineCommissionRule>();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["con"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand("Select * from AirlineCommissionRule where IsCommissionMinus='true'", con))
                {
                    try
                    {
                        con.Open();
                        SqlDataReader dr = cmd.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                AirlineCommissionRule objFT = new AirlineCommissionRule() { Airline = new List<string>(), SourceMedia = new List<string>() ,Provider=new List<GdsType>()};

                                if (string.IsNullOrEmpty(Convert.ToString(dr["SourceMedia"])) == false)
                                {
                                    objFT.SourceMedia = Convert.ToString(dr["SourceMedia"]).Split('-').ToList();
                                }
                                if (string.IsNullOrEmpty(Convert.ToString(dr["Airline"])) == false)
                                {
                                    objFT.Airline = Convert.ToString(dr["Airline"]).Split('-').ToList();
                                }
                                if (string.IsNullOrEmpty(Convert.ToString(dr["Provider"])) == false)
                                {                                   

                                    foreach (string ss in Convert.ToString(dr["Provider"]).Split('-').ToList())
                                    {
                                        objFT.Provider.Add((GdsType)Convert.ToInt32(ss));
                                    }
                                }
                               
                                FT.Add(objFT);
                            }
                        }
                        try
                        {
                            dr.Close();
                            con.Close();
                        }
                        catch { }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return FT;
        }

        #endregion

        public bool isDomestic(string FromAirportCode, string ToAirportCode)
        {
            List<Airport> FromArp = FlightUtility.AirportList.Where(x => (x.airportCode.Equals(FromAirportCode, StringComparison.OrdinalIgnoreCase))).ToList();
            List<Airport> ToArp = FlightUtility.AirportList.Where(x => (x.airportCode.Equals(ToAirportCode, StringComparison.OrdinalIgnoreCase))).ToList();
            if (FromArp.Count > 0 && FromArp[0].countryCode.Equals("IN", StringComparison.OrdinalIgnoreCase) && ToArp.Count > 0 && ToArp[0].countryCode.Equals("IN", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
                return false;
        }
        public TravelType getTravelType(string FromCountry, string ToCountry)
        {
            if (FromCountry.Equals("IN", StringComparison.OrdinalIgnoreCase) && ToCountry.Equals("IN", StringComparison.OrdinalIgnoreCase))
            {
                return TravelType.Domestic;
            }
            else
                return TravelType.International;
        }
        public void setContinentCountry(string Continent, ref List<string> country)
        {
            if (Continent.Equals("cont1", StringComparison.OrdinalIgnoreCase))
            {
                string[] arr = { "AR", "BO", "BR", "CL", "CO", "EC", "FK", "GF", "GY", "PE", "PY", "SR", "UY", "VE" };
                foreach (string item in arr)
                {
                    country.Add(item);
                }
            }

            if (Continent.Equals("cont2", StringComparison.OrdinalIgnoreCase))
            {
                string[] arr = { "AS", "AU", "CK", "FJ", "FM", "GU", "KI", "MH", "MP", "NC", "NF", "NR", "NU", "NZ", "PF", "PG", "PN", "PW", "SB", "TK", "TO", "TV", "UM", "VU", "WF", "WS" };
                foreach (string item in arr)
                {
                    country.Add(item);
                }
            }

            if (Continent.Equals("cont3", StringComparison.OrdinalIgnoreCase))
            {
                string[] arr = { "BZ", "CR", "SV", "GT", "HN", "NI", "PA" };
                foreach (string item in arr)
                {
                    country.Add(item);
                }
            }

            if (Continent.Equals("cont4", StringComparison.OrdinalIgnoreCase))
            {
                string[] arr = { "AG", "AI", "AN", "AW", "BB", "BL", "BM", "BS", "CU", "DM", "DO", "GD", "GL", "GP", "HT", "JM", "KN", "KY", "LC", "MF", "MQ", "MS", "PM", "PR", "TC", "TT", "VC", "VG", "VI" };
                foreach (string item in arr)
                {
                    country.Add(item);
                }
            }

            if (Continent.Equals("cont5", StringComparison.OrdinalIgnoreCase))
            {
                string[] arr = { "US" };
                foreach (string item in arr)
                {
                    country.Add(item);
                }
            }

            if (Continent.Equals("cont6", StringComparison.OrdinalIgnoreCase))
            {
                string[] arr = { "CA" };
                foreach (string item in arr)
                {
                    country.Add(item);
                }
            }

            if (Continent.Equals("cont7", StringComparison.OrdinalIgnoreCase))
            {
                string[] arr = { "MX" };
                foreach (string item in arr)
                {
                    country.Add(item);
                }
            }

            if (Continent.Equals("cont8", StringComparison.OrdinalIgnoreCase))
            {
                string[] arr = { "AD", "AL", "AT", "AX", "BA", "BE", "BG", "BY", "CH", "CZ", "DE", "DK", "EE", "ES", "EU", "FI", "FO", "FR", "FX", "GB", "GG", "GI", "GR", "HR", "HU", "IE", "IM", "IS", "IT", "JE", "LI", "LT", "LU", "LV", "MC", "MD", "ME", "MK", "MT", "NL", "NO", "PL", "PT", "RO", "RS", "RU", "SE", "SI", "SJ", "SK", "SM", "TR", "UA", "VA" };
                foreach (string item in arr)
                {
                    country.Add(item);
                }
            }

            if (Continent.Equals("cont9", StringComparison.OrdinalIgnoreCase))
            {
                string[] arr = { "AE", "AF", "AM", "AP", "AZ", "BD", "BH", "BN", "BT", "CC", "CN", "CX", "CY", "GE", "HK", "ID", "IL", "IN", "IO", "IQ", "IR", "JO", "JP", "KG", "KH", "KP", "KR", "KW", "KZ", "LA", "LB", "LK", "MM", "MN", "MO", "MV", "MY", "NP", "OM", "PH", "PK", "PS", "QA", "SA", "SG", "SY", "TH", "TJ", "TL", "TM", "TW", "UZ", "VN", "YE" };
                foreach (string item in arr)
                {
                    country.Add(item);
                }
            }

            if (Continent.Equals("cont10", StringComparison.OrdinalIgnoreCase))
            {
                string[] arr = { "AO", "BF", "BI", "BJ", "BW", "CD", "CF", "CG", "CI", "CM", "CV", "DJ", "DZ", "EG", "EH", "ER", "ET", "GA", "GH", "GM", "GN", "GQ", "GW", "KE", "KM", "LR", "LS", "LY", "MA", "MG", "ML", "MR", "MU", "MW", "MZ", "NA", "NE", "NG", "RE", "RW", "SC", "SD", "SH", "SL", "SN", "SO", "ST", "SZ", "TD", "TG", "TN", "TZ", "UG", "YT", "ZA", "ZM", "ZW" };
                foreach (string item in arr)
                {
                    country.Add(item);
                }
            }
        }
    }
}
namespace CoreLogWriter
{
    public class LogWriter
    {
        private string m_exePath = string.Empty;
        public LogWriter(string logMessage, string FileName)
        {
            LogWrite(logMessage, FileName);
        }
        public LogWriter(string logMessage, string FileName, string FolderName)
        {
            LogWrite(logMessage, FileName, FolderName);
        }
        public void LogWrite(string logMessage, string FileName)
        {
            //try
            //{
            using (StreamWriter w = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + "\\Log\\" + FileName + ".txt"))
            {
                Log(logMessage, w);
            }
            //}
            //catch (Exception ex)
            //{
            //}
        }
        public void LogWrite(string logMessage, string FileName, string FolderName)
        {
            try
            {
                using (StreamWriter w = File.AppendText(AppDomain.CurrentDomain.BaseDirectory + "\\log\\" + FolderName + "\\" + FileName + ".txt"))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception ex)
            {
            }
        }
        public void Log(string logMessage, TextWriter txtWriter)
        {
            //try
            //{
            //txtWriter.Write("\r\nLog Entry : ");
            //txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
            //    DateTime.Now.ToLongDateString());
            //txtWriter.WriteLine("  :");
            txtWriter.WriteLine("{0}", logMessage);
            //txtWriter.WriteLine("-------------------------------");
            //}
            //catch (Exception ex)
            //{
            //}
        }
    }
}
