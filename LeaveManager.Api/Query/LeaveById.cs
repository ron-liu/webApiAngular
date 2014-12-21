using System;
using LeaveManager.Api.Domain;
using LeaveManager.Api.Infrastructure;
using Ninject;

namespace LeaveManager.Api.Query
{
	public class LeaveById : IQuery
	{
		public Guid LeaveId { get; set; }
	}

	public class LeaveByIdQueryHandler: IQueryHandler<LeaveById, Leave>
	{
		[Inject] public LeaveReadModelRepository LeaveReadModelRepository { get; set; }

		public Leave Query(LeaveById input)
		{
			return LeaveReadModelRepository.GetLeaveById(input.LeaveId);
		}
	}
}