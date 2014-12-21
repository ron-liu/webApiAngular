using System;
using System.Collections.Generic;
using System.Linq;
using LeaveManager.Api.Infrastructure;
using Ninject;

namespace LeaveManager.Api.Domain
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

	public class LeaveById : IQuery
	{
		public Guid LeaveId { get; set; }

		public class Handler : IQueryHandler<LeaveById, Leave>
		{
			[Inject]
			public LeaveReadModelRepository LeaveReadModelRepository { get; set; }

			public Leave Query(LeaveById input)
			{
				return LeaveReadModelRepository.GetLeaveById(input.LeaveId);
			}
		}
	}

	public class ListLeavesByUserName : IQuery
	{
		public string UserName { get; set; }

		public class Handler : IQueryHandler<ListLeavesByUserName, IEnumerable<Leave>>
		{
			[Inject]
			public LeaveReadModelRepository LeaveReadModelRepository { get; set; }

			public IEnumerable<Leave> Query(ListLeavesByUserName passIn)
			{
				return LeaveReadModelRepository.GetLeavesByUserName(passIn.UserName);
			}
		}
	}

	public class ListLeavesToEvaluate : IQuery
	{
		public string UserName { get; set; }

		public class Handler : IQueryHandler<ListLeavesToEvaluate, IEnumerable<Leave>>
		{
			[Inject]
			public LeaveReadModelRepository LeaveReadModelRepository { get; set; }

			public IEnumerable<Leave> Query(ListLeavesToEvaluate input)
			{
				return LeaveReadModelRepository.ListLeavesToEvaluate();
			}
		}
	}

	public class GetWorkingDays : IQuery
	{
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public class Handler : IQueryHandler<GetWorkingDays, int>
		{
			readonly DateTime [] _holidays = 
			{
				new DateTime(2014, 12, 25),
				new DateTime(2014, 12, 26),
				new DateTime(2015, 1, 1),
				new DateTime(2015, 1, 26),
				new DateTime(2015, 3, 9),
				new DateTime(2015, 4, 3),
				new DateTime(2015, 4, 4),
				new DateTime(2015, 4, 6),
				new DateTime(2015, 4, 25),
				new DateTime(2015, 6, 8),
				new DateTime(2015, 11, 3),
				new DateTime(2015, 12, 25),
				new DateTime(2015, 11, 26),
			}; 

			public int Query(GetWorkingDays input)
			{
				var count = 0;
				for (var d = input.StartDate.Date; d <= input.EndDate.Date; d = d.AddDays(1))
				{
					if (d.DayOfWeek == DayOfWeek.Sunday || d.DayOfWeek == DayOfWeek.Saturday) continue;
					if (_holidays.Contains(d)) continue;
					count ++;
				}
				return count;
			}
		}
	}

	public class OverlapWithApprovedLeaves : IQuery
	{
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }

		public class Handler : IQueryHandler<OverlapWithApprovedLeaves, OverlapCheckResult>
		{
			[Inject] public LeaveReadModelRepository LeaveReadModelRepository { get; set; }

			public OverlapCheckResult Query(OverlapWithApprovedLeaves input)
			{
				var leaves = LeaveReadModelRepository.OverlappedLeaves(input.StartDate, input.EndDate).ToList();
				
				return leaves.Any() 
					? new OverlapCheckResult{Overlaped = true, OverlappedLeave = leaves.First()} 
					: new OverlapCheckResult();
			}
		}

		public class OverlapCheckResult
		{
			public bool Overlaped { get; set; }
			public Leave OverlappedLeave { get; set; }
		}
	}

	public class GetThisFinacialStartDate : IQuery
	{
		public DateTime? AsAt { get; set; }

		public class Handler : IQueryHandler<GetThisFinacialStartDate, DateTime>
		{
			public DateTime Query(GetThisFinacialStartDate input)
			{
				var asAt = input.AsAt ?? DateTime.Now;
				return asAt.Month >= 7 ? new DateTime(asAt.Year, 7, 1) : new DateTime(asAt.Year - 1, 7, 1);
			}
		}
	}

	public class AprovedLeavesGroupByReason : IQuery
	{
		public class Handler : IQueryHandler<AprovedLeavesGroupByReason, IEnumerable<ReasonItem>>
		{
			[Inject] public LeaveReadModelRepository LeaveReadModelRepository { get; set; }
			[Inject] public IQueryHandler<GetThisFinacialStartDate, DateTime> GetThisFinacialStartDate { get; set; }

			public IEnumerable<ReasonItem> Query(AprovedLeavesGroupByReason input)
			{
				var items = LeaveReadModelRepository.ApprovedLeavesGroupByReason(GetThisFinacialStartDate.Query(new GetThisFinacialStartDate())).ToList();
				foreach (var e in Enum.GetValues(typeof(Leave.ReasonEnum)).Cast<Leave.ReasonEnum>())
				{
					if (!items.Any(x=>x.Reason == e)) items.Add(new ReasonItem{Reason = e, Count = 0});
				}
				return items;
			}
		}
	}

	public class LeavesGroupByStatus: IQuery
	{
		public class Handler : IQueryHandler<LeavesGroupByStatus, IEnumerable<StatusItem>>
		{
			[Inject] public LeaveReadModelRepository LeaveReadModelRepository { get; set; }
			[Inject] public IQueryHandler<GetThisFinacialStartDate, DateTime> GetThisFinacialStartDate { get; set; }

			public IEnumerable<StatusItem> Query(LeavesGroupByStatus input)
			{
				var items = LeaveReadModelRepository.LeavesGroupByStatus(GetThisFinacialStartDate.Query(new GetThisFinacialStartDate())).ToList();
				foreach (var e in Enum.GetValues(typeof(Leave.StatusEnum)).Cast<Leave.StatusEnum>())
				{
					if (!items.Any(x => x.Status == e)) items.Add(new StatusItem { Status = e, Count = 0 });
				}
				return items;
			}
		}
	}

}