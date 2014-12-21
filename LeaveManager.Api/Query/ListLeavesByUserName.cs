using System.Collections.Generic;
using LeaveManager.Api.Domain;
using LeaveManager.Api.Infrastructure;

namespace LeaveManager.Api.Query
{
	public class ListLeavesByUserName : IQuery
	{
		public string UserName { get; set; }
	}

	public class ListLeavesByUserNameQueryHandler : IQueryHandler<ListLeavesByUserName, IEnumerable<Leave>>
	{
		public IEnumerable<Leave> Query(ListLeavesByUserName passIn)
		{
			return new[] {new Leave(),new Leave(), };
		}
	}
}