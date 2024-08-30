using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Security.Principal;

namespace IndiaAPI.Controllers
{
    public class BasicAuthenticationAttribute: AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                actionContext.Response.Headers.Add("WWW-Authenticate", "Basic realm=\"MyWebAPI\"");
            }
            else
            {
                var authHeader = actionContext.Request.Headers.Authorization.Parameter;
                var decodedAuthHeader = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader));
                var usernamePasswordArray = decodedAuthHeader.Split(':');
                var username = usernamePasswordArray[0];
                var password = usernamePasswordArray[1];

                if (IsAuthorizedUser(username, password))
                {
                    Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(username), null);
                }
                else
                {
                    actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
                    actionContext.Response.Headers.Add("WWW-Authenticate", "Basic realm=\"MyWebAPI\"");
                }
            }
        }

        public static bool IsAuthorizedUser(string username, string password)
        {
            // Replace with your own logic to validate username and password
            return username == "gfsearch" && password == "F!igHT#$%9514238";
        }
    }
}