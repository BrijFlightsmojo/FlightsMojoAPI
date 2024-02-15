using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IndiaAPI.Controllers
{
    [RoutePrefix("Routes")]
    public class RoutesController : ApiController
    {
        public static string SecurityCode = "fl1asdfghasdftmoasdfjado2o";
        public static bool authorizeRequest(string securityCodeGet)
        {
            return SecurityCode == securityCodeGet;
        }
        [HttpGet]
        [Route("test")]
        public HttpResponseMessage Test()
        {
            ServicesHub.FareBoutique.FareBoutiqueServiceMapping obj = new ServicesHub.FareBoutique.FareBoutiqueServiceMapping();
            obj.getSectors();

         
            return Request.CreateResponse(HttpStatusCode.OK, true);
        }
        [HttpGet]
        [Route("setRoute")]
        public HttpResponseMessage setRoute()
        {
            ServicesHub.SatkarTravel.SatkarTravelServiceMapping obj = new ServicesHub.SatkarTravel.SatkarTravelServiceMapping();
            new DAL.FixDepartueRoute.RoutesDetails().DeleteSatkarRouteswithDate((int)Core.GdsType.SatkarTravel);
            obj.getSectors();

            ServicesHub.AirIQ.AirIQServiceMapping objAirIQ = new ServicesHub.AirIQ.AirIQServiceMapping();
            new DAL.FixDepartueRoute.RoutesDetails().DeleteSatkarRouteswithDate((int)Core.GdsType.AirIQ);
            objAirIQ.getSectors();

            ServicesHub.FareBoutique.FareBoutiqueServiceMapping objFB = new ServicesHub.FareBoutique.FareBoutiqueServiceMapping();
            new DAL.FixDepartueRoute.RoutesDetails().DeleteSatkarRouteswithDate((int)Core.GdsType.FareBoutique);
            objFB.getSectors();
            return Request.CreateResponse(HttpStatusCode.OK, true);
        }
    }
}
