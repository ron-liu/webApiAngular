using System;
using System.Linq;
using Ninject;

namespace LeaveManager.Api.Infrastructure
{
	public interface IMessage{ }

	public interface IEvent : IMessage
	{
		Guid SourceId { get; set; }
		int Version { get; set; }
	}

	public interface IEventHandler {}
	public interface IEventHandler<in T> : IEventHandler where T : IEvent
	{
		void Handle(T @event);
	}

	public interface ICommand : IMessage{ }

	public interface ICommandHandler { }
	public interface ICommandHandler<T> : ICommandHandler where T : ICommand
	{
		void Handle(T command);
	}

	public interface ICommandSender
	{
		void Send<T>(T command) where T : ICommand;
	}

	public interface IEventPublisher
	{
		void Publish<T>(T @event) where T : IEvent;
	}

	public class FakeBus : ICommandSender, IEventPublisher
	{
		[Inject] private IKernel Kernel { get; set; }

		public void Send<T>(T command) where T : ICommand
		{
			var handler = Kernel.Get<ICommandHandler<T>>();
			if (handler == null) throw  new Exception(string.Format("Cannot find CommandHandler for {0}", typeof(T).Name));

			handler.Handle(command);
		}

		public void Publish<T>(T @event) where T : IEvent
		{
			// At beginning, I try to use Kernel.Get<IEventHandler<T>>() to work out, but bad lucky, 
			// have to play with reflection heavily
			var handlers = Kernel.GetAll(typeof (IEventHandler<>).MakeGenericType(@event.GetType())).ToArray();
			if (handlers == null) return;

			foreach (var h in handlers)
			{
				var method = h.GetType().GetMethod("Handle", new[] { @event.GetType() });
				method.Invoke(h, new object[] {@event});
			}
				
		}
	}
}
