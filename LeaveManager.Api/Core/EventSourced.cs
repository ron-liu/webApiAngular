using System;
using System.CodeDom;
using System.Collections.Generic;

namespace LeaveManager.Api.Core
{
	public class Leave : EventSourced
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

		public Leave() : base(Guid.NewGuid())
		{
			
		}

		public Leave(Guid id) : base(id)
		{
			Status = StatusEnum.New;
			Comments = new List<CommentClass>();

			Handles<LeaveApplied>(OnLeaveApplied);
			Handles<LeaveEvaluated>(OnLeaveEvaluated);
		}

		public Leave(Guid id, IEnumerable<Event> pastEvents) : this(id)
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
			Comments.Add(new CommentClass{Comment = e.Comment, UserName = e.UserName});
			Reason = e.Reason;
			AppliedOn = e.HappenedOn;
			Status = StatusEnum.Applied;
		}

		private void OnLeaveEvaluated(LeaveEvaluated e)
		{
			Status = e.IsApproved ? StatusEnum.Approved : StatusEnum.Rejected;
			Comments.Add(new CommentClass{Comment = e.Comment, UserName = e.UserName});
			EvaluatedOn = e.HappenedOn;
		}
	}

	// Referenced from Microsoft cqrs jouney
	public abstract class EventSourced 
	{
		private readonly Dictionary<Type, Action<Event>> handlers = new Dictionary<Type, Action<Event>>();
		private readonly List<Event> pendingEvents = new List<Event>();

		private readonly Guid id;
		private int version = -1;

		protected EventSourced(Guid id)
		{
			this.id = id;
		}

		public Guid Id
		{
			get { return this.id; }
		}

		/// <summary>
		/// Gets the entity's version. As the entity is being updated and events being generated, the version is incremented.
		/// </summary>
		public int Version
		{
			get { return this.version; }
			protected set { this.version = value; }
		}

		/// <summary>
		/// Gets the collection of new events since the entity was loaded, as a consequence of command handling.
		/// </summary>
		public IEnumerable<Event> Events
		{
			get { return this.pendingEvents; }
		}

		/// <summary>
		/// Configures a handler for an event. 
		/// </summary>
		protected void Handles<TEvent>(Action<TEvent> handler)
			where TEvent : Event
		{
			this.handlers.Add(typeof(TEvent), @event => handler((TEvent)@event));
		}

		protected void LoadFrom(IEnumerable<Event> pastEvents)
		{
			foreach (var e in pastEvents)
			{
				this.handlers[e.GetType()].Invoke(e);
				this.version = e.Version;
			}
		}

		protected void Update(Event e)
		{
			e.SourceId = this.Id;
			e.Version = this.version + 1;
			this.handlers[e.GetType()].Invoke(e);
			this.version = e.Version;
			this.pendingEvents.Add(e);
		}
	}
}