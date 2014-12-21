using System;
using System.Security.Cryptography;
using LeaveManager.Api.Domain;
using LeaveManager.Api.Infrastructure;
using Ninject;

namespace LeaveManager.Api.Query
{
	public interface IQuery
	{ }

	public interface IQueryHandler<in TIn, out TOut> where TIn : IQuery
	{
		TOut Query(TIn input);
	}

	public class MyLeaveById : IQuery
	{
		public string UserName { get; set; }
		public Guid LeaveId { get; set; }

		public class Handler : IQueryHandler<MyLeaveById, Leave>
		{
			[Inject] public LeaveReadModelRepository LeaveReadModelRepository { get; set; }

			public Leave Query(MyLeaveById input)
			{
				return LeaveReadModelRepository.GetMyLeaveById(input.LeaveId, input.UserName);
			}
		}
	}
}