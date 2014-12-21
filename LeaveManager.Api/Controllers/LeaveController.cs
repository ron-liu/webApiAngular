using System.Collections.Generic;
using System.Web.Http;
using LeaveManager.Api.Domain;
using LeaveManager.Api.Query;
using Ninject;

namespace LeaveManager.Api.Controllers
{
	public class LeaveController : BaseApiController
	{
		private IQueryHandler<ListLeavesByUserName, IEnumerable<Leave>> listLeavesByUserName { get; set; }

		[Route("list")]
		[Authorize]
		[HttpGet]
		public IHttpActionResult List()
		{
			return null;
		}
	}
}