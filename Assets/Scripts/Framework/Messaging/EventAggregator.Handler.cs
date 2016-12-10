using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Extensions;
using System.Reflection;
using JetBrains.Annotations;

namespace PachowStudios.Framework.Messaging
{
  public partial class EventAggregator
  {
    private class Handler<THandler> : IHandler
      where THandler : IHandle
    {
      public bool IsAlive => Subscriber.IsAlive;

      private WeakReference Subscriber { get; }
      private Dictionary<Type, MethodInfo> HandlerMethods { get; }

      public Handler([NotNull] THandler subscriber)
      {
        Subscriber = new WeakReference(subscriber);
        HandlerMethods = typeof(THandler)
          .GetInterfaces()
          .Where(i => i.IsAssignableFrom<IHandle<IMessage>>() && i.IsGenericType)
          .Select(i => i.GetGenericArguments().Single())
          .ToDictionary(m => m, m => typeof(THandler).GetMethod(nameof(IHandle<IMessage>.Handle), new[] { m }));
      }

      public void Handle<TMessage>(TMessage message)
        where TMessage : IMessage
        => HandlerMethods
          .Where(h => h.Key.IsAssignableFrom<TMessage>())
          .ForEach(h => h.Value.Invoke(Subscriber.Target, new object[] { message }));

      [Pure]
      public bool Handles<TMessage>()
        where TMessage : IMessage
        => HandlerMethods.Keys.Any(h => h.IsAssignableFrom<TMessage>());

      [Pure]
      public bool RefersTo<T>(T instance)
        where T : class, IHandle
        => Subscriber.Target.RefersTo(instance);
    }
  }
}