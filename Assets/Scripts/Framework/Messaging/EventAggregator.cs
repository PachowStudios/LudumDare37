using System.Collections.Generic;
using System.Linq;
using System.Linq.Extensions;
using JetBrains.Annotations;

namespace PachowStudios.Framework.Messaging
{
  public partial class EventAggregator : IEventAggregator
  {
    private List<IHandler> Handlers { get; } = new List<IHandler>();
    private EventAggregator ParentContext { get; }
    private List<IEventAggregator> ChildContexts { get; } = new List<IEventAggregator>();

    public EventAggregator() { }

    private EventAggregator(EventAggregator parentContext)
    {
      ParentContext = parentContext;
    }

    public IEventAggregator CreateChildContext()
    {
      var child = new EventAggregator(this);

      ChildContexts.Add(child);

      return child;
    }

    private void RemoveChildContext(IEventAggregator child)
      => ChildContexts.Remove(child);

    public void Subscribe<THandler>(THandler subscriber)
      where THandler : class, IHandle
    {
      if (Handlers.None(h => h.RefersTo(subscriber)))
        Handlers.Add(new Handler<THandler>(subscriber));
    }

    public void Unsubscribe<THandler>(THandler subscriber)
      where THandler : class, IHandle
      => Handlers.RemoveSingle(h => h.RefersTo(subscriber));

    public void Publish<TMessage>(TMessage message)
      where TMessage : IMessage
    {
      if (ParentContext != null)
        ParentContext.Publish(message);
      else
        PublishLocally(message);
    }

    public void PublishLocally<TMessage>(TMessage message)
      where TMessage : IMessage
    {
      Handlers.RemoveAll(h => !h.IsAlive);
      Handlers.ForEach(h => h.Handle(message));
      ChildContexts.ForEach(c => c.PublishLocally(message));
    }

    [Pure]
    public bool SubscriberExistsFor<TMessage>()
      where TMessage : IMessage
      => Handlers.Any(h => h.IsAlive && h.Handles<TMessage>());

    public void Dispose()
      => ParentContext?.RemoveChildContext(this);
  }
}
