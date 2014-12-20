using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NEventStore;
using NEventStore.Dispatcher;
using Ninject;

namespace LeaveManager.Api.Core
{
	public class FakeBus : ICommandSender, IEventPublisher
	{
		private readonly Dictionary<Type, List<Action<IMessage>>> _routes = new Dictionary<Type, List<Action<IMessage>>>();

		public void RegisterHandler<T>(Action<T> handler) where T : IMessage
		{
			List<Action<IMessage>> handlers;
			if (!_routes.TryGetValue(typeof(T), out handlers))
			{
				handlers = new List<Action<IMessage>>();
				_routes.Add(typeof(T), handlers);
			}
			handlers.Add(DelegateAdjuster.CastArgument<IMessage, T>(x => handler(x)));
		}

		public void Send<T>(T command) where T : Command
		{
			List<Action<IMessage>> handlers;
			if (_routes.TryGetValue(typeof(T), out handlers))
			{
				if (handlers.Count != 1) throw new InvalidOperationException("cannot send to more than one handler");
				handlers[0](command);
			}
			else
			{
				throw new InvalidOperationException("no handler registered");
			}
		}

		public void Publish<T>(T @event) where T : Event
		{
			List<Action<IMessage>> handlers;
			if (!_routes.TryGetValue(@event.GetType(), out handlers)) return;
			foreach (var handler in handlers)
			{
				//dispatch on thread pool for added awesomeness
				var handler1 = handler;
				ThreadPool.QueueUserWorkItem(x => handler1(@event));
			}
		}
	}

	public interface Handles<T>
	{
		void Handle(T message);
	}

	public interface ICommandSender
	{
		void Send<T>(T command) where T : Command;

	}
	public interface IEventPublisher
	{
		void Publish<T>(T @event) where T : Event;
	}

	public class FakeBusDispatcher : IDispatchCommits
	{
		[Inject] private IEventPublisher EventPublisher { get; set; }
		

		public void Dispose()
		{}

		public void Dispatch(ICommit commit)
		{
			foreach (var e in commit.Events.Cast<Event>())
			{
				EventPublisher.Publish(e);
			}
		}
	}
}
