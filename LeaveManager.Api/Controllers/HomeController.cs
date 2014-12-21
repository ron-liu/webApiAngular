using System.Net;
using System.Web.Http;
using System.Web.UI.WebControls;

namespace LeaveManager.Api.Controllers
{
	public class HomeController : BaseApiController
	{
		[Route("")]
		[HttpGet]
		public IHttpActionResult Index()
		{
			return Content(HttpStatusCode.OK, "Please visit http://localhost:{port}/assets/index.html to launch the app, thanks");
		}
	}
}