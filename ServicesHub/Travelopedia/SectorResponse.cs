using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Travelopedia.TravelopediaClass.SectorResponse
{
   public class SectorResponse
    {
         public ResponseHeader Response_Header { get; set; }
        public List<SectorsPI> SectorsPIs { get; set; }
    }

    public class SectorsPI
    {
        public string AvailableDates { get; set; }
        public string Destination { get; set; }
        public string MaxTravelDate { get; set; }
        public string Origin { get; set; }
    }

    public class ResponseHeader
    {
        public string Error_Code { get; set; }
        public string Error_Desc { get; set; }
        public string Error_InnerException { get; set; }
        public string Request_Id { get; set; }
        public string Status_Id { get; set; }
    }

}
