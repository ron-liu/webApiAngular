using System;
using System.Collections.Generic;
using System.Linq;
using NEventStore;
using Ninject;

namespace LeaveManager.Api.Infrastructure
{
	public interface IEventSteamRepository<TAggregate>
	{
		TAggregate Get(Guid id);
		void Save(TAggregate aggregate);
	}

	public class EventSteamRepository<TAggregate> : IEventSteamRepository<TAggregate> where TAggregate : EventSourcedAggregate
	{
		[Inject]
		public IStoreEvents EventStore { get; set; }

		public TAggregate Get(Guid id)
		{
			var constructor = typeof(TAggregate).GetConstructor(new[] { typeof(Guid), typeof(IEnumerable<IEvent>) });
			if (constructor == null) 
				throw new Exception(string.Format("Event sourced aggregate {0} was not found.", typeof(TAggregate).Name));
			Func<Guid, IEnumerable<IEvent>, TAggregate> entityFactory = 
				(aid, events) => (TAggregate)constructor.Invoke(new object[] { aid, events });

			using (var stream = EventStore.OpenStream(id))
			{
				var events = stream.CommittedEvents.Select(x => x.Body).Cast<IEvent>();
				return entityFactory.Invoke(id, events);
			}
		}

		public void Save(TAggregate aggregate)
		{
			using (var stream = EventStore.OpenStream(aggregate.Id, 0))
			{
				foreach (var e in aggregate.Events)
					stream.Add(new EventMessage { Body = e });
				stream.CommitChanges(Guid.NewGuid());
			}
		}
	}
}