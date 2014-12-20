using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Http;
using LeaveManager.Api.Core;
using LeaveManager.Api.Query;
using NEventStore;
using Ninject;

namespace LeaveManager.Api.Controllers
{
	public class LeaveController : BaseApiController
	{
		[Inject]private IQueryHandler<ListLeavesByUserName, IEnumerable<Leave>> listLeavesByUserName { get; set; }
		[Inject]
		private IStoreEvents EventStore { get; set; }
		
		[Route("list")]
		[Authorize]
		[HttpGet]
		public IHttpActionResult List()
		{
			//return Content(HttpStatusCode.OK,
			//	listLeavesByUserName.Query(new ListLeavesByUserName { UserName = User.Identity.Name }));

			var msg = new LeaveApplied
			{
				Comment = "abc",
				StartDate = DateTime.Now,
				EndDate = DateTime.Now.AddDays(2),
				HappenedOn = DateTime.Now,
				Reason = Leave.ReasonEnum.Annual,
				SourceId = Guid.NewGuid(),
				UserName = "Ron",
				Version = 1
			};
			using (var store = EventStore.CreateStream(msg.SourceId))
			{
				store.Add(new EventMessage{Body = msg});
				store.CommitChanges(Guid.NewGuid());
			}
			return null;

		}
	}
}