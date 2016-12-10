using System;
using JetBrains.Annotations;

namespace PachowStudios.Framework.Messaging
{
  public interface IEventAggregator : IDisposable
  {
    [NotNull]
    IEventAggregator CreateChildContext();

    void Subscribe<THandler>([NotNull] THandler subscriber)
      where THandler : class, IHandle;

    void Unsubscribe<THandler>([NotNull] THandler subscriber)
      where THandler : class, IHandle;

    void Publish<TMessage>([NotNull] TMessage message)
      where TMessage : IMessage;

    void PublishLocally<TMessage>([NotNull] TMessage message)
      where TMessage : IMessage;

    [Pure]
    bool SubscriberExistsFor<TMessage>()
      where TMessage : IMessage;
  }
}
