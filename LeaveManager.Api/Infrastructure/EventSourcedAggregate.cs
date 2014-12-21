using System;
using System.Collections.Generic;

namespace LeaveManager.Api.Infrastructure
{
	// Referenced from Microsoft cqrs jouney
	public abstract class EventSourcedAggregate 
	{
		private readonly Dictionary<Type, Action<IEvent>> handlers = new Dictionary<Type, Action<IEvent>>();
		private readonly List<IEvent> pendingEvents = new List<IEvent>();

		private readonly Guid id;
		private int version = -1;

		protected EventSourcedAggregate(Guid id)
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
		public IEnumerable<IEvent> Events
		{
			get { return this.pendingEvents; }
		}

		/// <summary>
		/// Configures a handler for an event. 
		/// </summary>
		protected void Handles<TEvent>(Action<TEvent> handler)
			where TEvent : IEvent
		{
			this.handlers.Add(typeof(TEvent), @event => handler((TEvent)@event));
		}

		protected void LoadFrom(IEnumerable<IEvent> pastEvents)
		{
			foreach (var e in pastEvents)
			{
				this.handlers[e.GetType()].Invoke(e);
				this.version = e.Version;
			}
		}

		protected void Update(IEvent e)
		{
			e.SourceId = this.Id;
			e.Version = this.version + 1;
			this.handlers[e.GetType()].Invoke(e);
			this.version = e.Version;
			this.pendingEvents.Add(e);
		}
	}
}