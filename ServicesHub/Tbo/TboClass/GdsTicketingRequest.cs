using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tbo.TboClass
{
    [DataContract]
    public class GdsTicketingRequest
    {
        [DataMember]
        public string EndUserIp { get; set; }
        [DataMember]
        public string TokenId { get; set; }
        [DataMember]
        public string TraceId { get; set; }
        [DataMember]
        public string PNR { get; set; }
        [DataMember]
        public long BookingId { get; set; }
    }
}
