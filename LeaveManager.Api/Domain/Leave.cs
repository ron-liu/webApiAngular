using System;
using System.Collections.Generic;
using LeaveManager.Api.Infrastructure;

namespace LeaveManager.Api.Domain
{
	public class Leave : EventSourcedAggregate
	{
		public string UserName { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public ReasonEnum Reason { get; set; }
		public StatusEnum Status { get; set; }
		public List<CommentClass> Comments { get; set; }
		public DateTime AppliedOn { get; set; }
		public DateTime EvaluatedOn { get; set; }

		public enum ReasonEnum
		{
			Annual = 1, Personal, Compassionate, Parental
		}

		public enum StatusEnum
		{
			New = 1, Applied, Approved, Rejected
		}

		public class CommentClass
		{
			public string UserName { get; set; }
			public string Comment { get; set; }
		}

		public Leave()
			: base(Guid.NewGuid())
		{

		}

		public Leave(Guid id)
			: base(id)
		{
			Status = StatusEnum.New;
			Comments = new List<CommentClass>();

			Handles<LeaveApplied>(OnLeaveApplied);
			Handles<LeaveEvaluated>(OnLeaveEvaluated);
		}

		public Leave(Guid id, IEnumerable<IEvent> pastEvents)
			: this(id)
		{
			LoadFrom(pastEvents);
		}

		public void ApplyLeave(string userName, DateTime startDate, DateTime endDate, ReasonEnum reason, string comment, DateTime? asAt = null)
		{
			asAt = asAt ?? DateTime.Now;
			Update(new LeaveApplied
			{
				UserName = userName,
				Comment = comment,
				StartDate = startDate,
				EndDate = endDate,
				Reason = reason,
				SourceId = Id,
				HappenedOn = asAt.Value
			});
		}

		public void EvaluateLeave(string userName, bool isApproved, string comment, DateTime? asAt = null)
		{
			asAt = asAt ?? DateTime.Now;
			Update(new LeaveEvaluated
			{
				HappenedOn = asAt.Value,
				Comment = comment,
				UserName = userName,
				IsApproved = isApproved,
				SourceId = Id,
			});
		}

		private void OnLeaveApplied(LeaveApplied e)
		{
			StartDate = e.StartDate;
			EndDate = e.EndDate;
			Comments.Add(new CommentClass { Comment = e.Comment, UserName = e.UserName });
			Reason = e.Reason;
			AppliedOn = e.HappenedOn;
			Status = StatusEnum.Applied;
		}

		private void OnLeaveEvaluated(LeaveEvaluated e)
		{
			Status = e.IsApproved ? StatusEnum.Approved : StatusEnum.Rejected;
			Comments.Add(new CommentClass { Comment = e.Comment, UserName = e.UserName });
			EvaluatedOn = e.HappenedOn;
		}
	}
}