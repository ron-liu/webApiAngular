using System.Collections.Generic;
using LeaveManager.Api.Domain;
using LeaveManager.Api.Infrastructure;
using Ninject;

namespace LeaveManager.Api.Query
{
	public class ListLeavesToEvaluate : IQuery
	{
		public string UserName { get; set; }
	}

	public class ListLeavesToEvaluateQueryHandler : IQueryHandler<ListLeavesToEvaluate, IEnumerable<Leave>>
	{
		[Inject]
		public LeaveReadModelRepository LeaveReadModelRepository { get; set; }

		public IEnumerable<Leave> Query(ListLeavesToEvaluate input)
		{
			return LeaveReadModelRepository.ListLeavesToEvaluate();
		}
	}
}