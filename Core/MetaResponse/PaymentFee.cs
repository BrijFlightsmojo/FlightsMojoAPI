using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Core.Meta
{
    [DataContract]
    public class PaymentFee
    {
        [DataMember]
        public string Amount { get; set; }
        [DataMember]
        public string FeeCode { get; set; }
        [DataMember]
        public string CardName { get; set; }
    }
}
