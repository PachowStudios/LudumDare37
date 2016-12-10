using JetBrains.Annotations;

namespace PachowStudios.Framework.Messaging
{
  public interface IHandle { }

  public interface IHandle<in TMessage> : IHandle
    where TMessage : IMessage
  {
    void Handle([NotNull] TMessage message);
  }
}