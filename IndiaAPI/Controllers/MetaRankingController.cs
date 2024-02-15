using Core.MetaResponse;
using Newtonsoft.Json;
using ServicesHub;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IndiaAPI.Controllers
{
    [RoutePrefix("MetaRanking")]
    public class MetaRankingController : ApiController
    {
        public static string SecurityCode = "fl1asdfghasdftmsdfghjkoasdfjado2o";
        public static bool authorizeRequest(string securityCodeGet)
        {
            return SecurityCode == securityCodeGet;
        }
        [HttpPost]
        [Route("getMetaResponse")]
        public HttpResponseMessage getMetaResponse(string authcode, SkyscannerRankingResponse response)
        {
            if (!authorizeRequest(authcode))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden);
            }

            LogCreater.CreateLogFile(JsonConvert.SerializeObject(response), "Log\\MetaRank","Response"+ DateTime.Now.ToString("ddMMMyy_HHmmss")+ ".txt");
            return Request.CreateResponse(HttpStatusCode.OK, true);
           
        }
    }
}
