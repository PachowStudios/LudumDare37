using JetBrains.Annotations;

namespace PachowStudios.Framework.Messaging
{
  public partial class EventAggregator
  {
    private interface IHandler
    {
      bool IsAlive { get; }

      void Handle<TMessage>([NotNull] TMessage message)
        where TMessage : IMessage;

      [Pure]
      bool Handles<TMessage>()
        where TMessage : IMessage;

      [Pure]
      bool RefersTo<T>([NotNull] T instance)
        where T : class, IHandle;
    }
  }
}
