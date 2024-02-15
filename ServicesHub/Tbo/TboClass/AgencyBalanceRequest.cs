using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tbo.TboClass
{
    [DataContract]
    public class AgencyBalanceRequest
    {
        [DataMember]
        public string ClientId { get; set; }
        [DataMember]
        public string TokenAgencyId { get; set; }
        [DataMember]
        public string TokenMemberId { get; set; }
        [DataMember]
        public string EndUserIp { get; set; }
        [DataMember]
        public string TokenId { get; set; }

    }
}
