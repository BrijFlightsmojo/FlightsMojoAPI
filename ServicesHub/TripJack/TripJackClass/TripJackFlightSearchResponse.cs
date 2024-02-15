using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.TripJack.TripJackClass
{

    public class TripJackFlightSearchResponse
    {
        public SearchResult searchResult { get; set; }
        public Status status { get; set; }
    }
    public class SearchResult
    {
        public TripInfos tripInfos { get; set; }
    }
    public class TripInfos
    {
        public List<ONWARD> ONWARD { get; set; }
        public List<ONWARD> RETURN { get; set; }
        public List<ONWARD> COMBO { get; set; }
    }
    public class ONWARD
    {
        public List<SI> sI { get; set; }
        public List<TotalPriceList> totalPriceList { get; set; }
    }
    public class TotalPriceList
    {
        public Fd2 fd { get; set; }
        public string fareIdentifier { get; set; }
        public string id { get; set; }
        public List<string> msri { get; set; }
        public Tai tai { get; set; }
        public string sri { get; set; }
        public int sR { get; set; }
    }
    public class SI
    {
        public string id { get; set; }
        public FD fD { get; set; }
        public int stops { get; set; }
        public List<Arp> so { get; set; }
        public int duration { get; set; }
        public AI oB { get; set; }
        public Arp da { get; set; }
        public Arp aa { get; set; }
        public DateTime dt { get; set; }
        public DateTime at { get; set; }
        public bool iand { get; set; }
        public bool isRs { get; set; }
        public int sN { get; set; }
    }
    public class Arp
    {
        public string code { get; set; }
        public string name { get; set; }
        public string cityCode { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string countryCode { get; set; }
        public string terminal { get; set; }
    }
    public class ADULT
    {
        public FC fC { get; set; }
        public AfC afC { get; set; }
        public int sR { get; set; }
        public BI bI { get; set; }
        public int rT { get; set; }
        public string cc { get; set; }
        public string cB { get; set; }
        public string fB { get; set; }
        public bool mI { get; set; }
        public string iB { get; set; }
    }
    public class AfC
    {
        public TAF TAF { get; set; }
        public NCM NCM { get; set; }

    }

    public class NCM
    {
        public decimal OT { get; set; }
        public decimal TDS { get; set; }
    }

    public class AI
    {
        public string code { get; set; }
        public string name { get; set; }
        public bool isLcc { get; set; }
    }
    public class BI
    {
        public string iB { get; set; }
        public string cB { get; set; }
    }
    public class CHILD
    {
        public FC fC { get; set; }
        public AfC afC { get; set; }
        public int sR { get; set; }
        public BI bI { get; set; }
        public int rT { get; set; }
        public string cc { get; set; }
        public string cB { get; set; }
        public string fB { get; set; }
        public bool mI { get; set; }
        public string iB { get; set; }
    }
    //public class Da
    //{
    //    public string code { get; set; }
    //    public string name { get; set; }
    //    public string cityCode { get; set; }
    //    public string city { get; set; }
    //    public string country { get; set; }
    //    public string countryCode { get; set; }
    //    public string terminal { get; set; }
    //}
    public class FC
    {
        public decimal TAF { get; set; }
        public decimal BF { get; set; }
        public decimal NF { get; set; }
        public decimal TF { get; set; }
        public decimal NCM { get; set; }
    }
    public class FD
    {
        public AI aI { get; set; }
        public string fN { get; set; }
        public string eT { get; set; }
    }
    public class Fd2
    {
        public ADULT ADULT { get; set; }
        public INFANT INFANT { get; set; }
        public CHILD CHILD { get; set; }
    }
    public class INFANT
    {
        public FC fC { get; set; }
        public AfC afC { get; set; }
        public BI bI { get; set; }
        public int rT { get; set; }
        public bool mI { get; set; }
        public int? sR { get; set; }
        public string cc { get; set; }
        public string cB { get; set; }
        public string fB { get; set; }
        public string iB { get; set; }
    }
    public class TAF
    {
        public decimal MFT { get; set; }
        public decimal MF { get; set; }
        public decimal OT { get; set; }
        public decimal YQ { get; set; }
        public decimal AGST { get; set; }
        public decimal? YR { get; set; }
    }
    public class Tai
    {
        public Tbi tbi { get; set; }
    }
    public class Tbi
    {
        public List<ACI> _859 { get; set; }
        public List<ACI> _726 { get; set; }
        public List<ACI> _60 { get; set; }
        public List<ACI> _517 { get; set; }
        public List<ACI> _118 { get; set; }
        public List<ACI> _358 { get; set; }
        public List<ACI> _447 { get; set; }
        public List<ACI> _616 { get; set; }
        public List<ACI> _540 { get; set; }
        public List<ACI> _450 { get; set; }
        public List<ACI> _821 { get; set; }
        public List<ACI> _399 { get; set; }
        public List<ACI> _272 { get; set; }
        public List<ACI> _986 { get; set; }
        public List<ACI> _26 { get; set; }
        public List<ACI> _670 { get; set; }
        public List<ACI> _425 { get; set; }
        public List<ACI> _663 { get; set; }
        public List<ACI> _571 { get; set; }
        public List<ACI> _509 { get; set; }
        public List<ACI> _543 { get; set; }
        public List<ACI> _529 { get; set; }
        public List<ACI> _914 { get; set; }
        public List<ACI> _526 { get; set; }
        public List<ACI> _17 { get; set; }
        public List<ACI> _464 { get; set; }
        public List<ACI> _564 { get; set; }
        public List<ACI> _763 { get; set; }
    }
    public class ACI
    {
        public ADULT ADULT { get; set; }
        public INFANT INFANT { get; set; }
        public CHILD CHILD { get; set; }
    }
    public class Status
    {
        public bool success { get; set; }
        public int httpStatus { get; set; }
    }
}
