using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Since1999.Filters
{
    public class CustomAuthorizeAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            // Verifica se l'utente è autenticato
            if (!actionContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                // L'utente non è autenticato, restituisci una risposta 401 Unauthorized
                actionContext.Response = actionContext.Request.CreateErrorResponse(
                    HttpStatusCode.Unauthorized, "Utente non autorizzato");
            }
        }
    }
}
