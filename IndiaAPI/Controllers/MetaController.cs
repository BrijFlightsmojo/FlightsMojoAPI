using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IndiaAPI.Controllers
{
    [RoutePrefix("Meta")]
    public class MetaController : ApiController
    {
        public static string SecurityCode = "asjfa9qwrjoiufsewnfhsadfksadflkhsa";
        public static bool authorizeRequest(string securityCodeGet)
        {
            return SecurityCode == securityCodeGet;
        }
        [Route("GetRoute")]
        [HttpGet]
        public HttpResponseMessage GetRoute(string securityCode)
        {
            if (!authorizeRequest(securityCode))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            DAL.Meta.DalMeta obj = new DAL.Meta.DalMeta();
            return Request.CreateResponse(HttpStatusCode.OK, obj.getAllMetaRoute());
        }
        [Route("GetRouteSyc")]
        [HttpGet]
        public HttpResponseMessage GetRouteSyc(string securityCode)
        {
            if (!authorizeRequest(securityCode))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }
            DAL.Meta.DalMeta obj = new DAL.Meta.DalMeta();
            return Request.CreateResponse(HttpStatusCode.OK, obj.getAllMetaRouteSyc());
        }
    }
}
