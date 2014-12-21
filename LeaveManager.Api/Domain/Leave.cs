using System;
using System.Collections.Generic;
using LeaveManager.Api.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LeaveManager.Api.Domain
{
	public class Leave : EventSourcedAggregate
	{
		public string Applicant { get; set; }
		public string Evaluator { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		[JsonConverter(typeof(StringEnumConverter))]
		public ReasonEnum Reason { get; set; }
		[JsonConverter(typeof(StringEnumConverter))]
		public StatusEnum Status { get; set; }
		public string ApplicantComment { get; set; }
		public string EvaluatorComment { get; set; }
		public DateTime AppliedOn { get; set; }
		public DateTime? EvaluatedOn { get; set; }
		public int WorkingDays { get; set; }

		public enum ReasonEnum
		{
			Annual = 1, Personal, Compassionate, Parental
		}

		public enum StatusEnum
		{
			New = 1, Applied, Approved, Rejected
		}

		public Leave()
		{}

		public Leave(Guid id)
			: base(id)
		{
			Status = StatusEnum.New;
			Handles<LeaveApplied>(OnLeaveApplied);
			Handles<LeaveEvaluated>(OnLeaveEvaluated);
		}

		public Leave(Guid id, IEnumerable<IEvent> pastEvents)
			: this(id)
		{
			LoadFrom(pastEvents);
		}

		public void ApplyLeave(string userName, DateTime startDate, DateTime endDate, int workingDays, ReasonEnum reason, string comment, DateTime? asAt = null)
		{
			asAt = asAt ?? DateTime.Now;
			Update(new LeaveApplied
			{
				WorkingDays = workingDays,
				UserName = userName,
				Comment = comment,
				StartDate = startDate,
				EndDate = endDate,
				Reason = reason,
				SourceId = Id,
				HappenedOn = asAt.Value,
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
			Applicant = e.UserName;
			StartDate = e.StartDate;
			EndDate = e.EndDate;
			ApplicantComment = e.Comment;
			Reason = e.Reason;
			AppliedOn = e.HappenedOn;
			Status = StatusEnum.Applied;
			WorkingDays = e.WorkingDays;
		}

		private void OnLeaveEvaluated(LeaveEvaluated e)
		{
			Status = e.IsApproved ? StatusEnum.Approved : StatusEnum.Rejected;
			Evaluator = e.UserName;
			EvaluatorComment = e.Comment;
			EvaluatedOn = e.HappenedOn;
		}
	}
}