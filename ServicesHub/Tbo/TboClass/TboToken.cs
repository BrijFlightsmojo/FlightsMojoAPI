using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ServicesHub.Tbo.TboClass
{
   public class TboToken
    {
        public int id { set; get; }
        public string tokenID { set; get; }
        public string status { set; get; }
        public DateTime creationDateTime { set; get; }
        public int MemberId { set; get; }
        public int AgencyId { set; get; }
        public TboToken()
        {

        }
    }
    [DataContract]
    public class LoginRequest
    {
        [DataMember]
        public string ClientId { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Password { get; set; }
        [DataMember]
        public string EndUserIp { get; set; }

    }
    [DataContract]
    public class LoginResponse
    {
        [DataMember]

        public int Status { get; set; }
        [DataMember]
        public string TokenId { get; set; }
        [DataMember]
        public Error Error { get; set; }
        [DataMember]
        public Member Member { get; set; }
    }
    [DataContract]
    public class Member
    {
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public int MemberId { get; set; }
        [DataMember]
        public int AgencyId { get; set; }
        [DataMember]
        public string LoginName { get; set; }
        [DataMember]
        public string LoginDetails { get; set; }
        [DataMember]
        public bool isPrimaryAgent { get; set; }
    }
    [DataContract]
    public class Error
    {
        [DataMember]
        public int ErrorCode { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
    }
    [DataContract]
    public class LogOutRequest
    {
        [DataMember]
        public string ClientId { get; set; }
        [DataMember]
        public string EndUserIp { get; set; }
        [DataMember]
        public int TokenAgencyId { get; set; }
        [DataMember]
        public int TokenMemberId { get; set; }
        [DataMember]
        public string TokenId { get; set; }
    }
    [DataContract]
    public class LogOutResponse
    {
        public Error Error { get; set; }
        public int Status { get; set; }
    }
}
