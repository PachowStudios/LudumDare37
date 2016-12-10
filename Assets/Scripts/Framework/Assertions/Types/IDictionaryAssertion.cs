using System.Collections.Generic;

namespace PachowStudios.Framework.Assertions
{
  public sealed class IDictionaryAssertion<TKey, TValue> : ICollectionAssertion<KeyValuePair<TKey, TValue>, IDictionaryAssertion<TKey, TValue>>
  {
    private new IDictionary<TKey, TValue> Subject => (IDictionary<TKey, TValue>)base.Subject;

    public IDictionaryAssertion(IDictionary<TKey, TValue> subject)
      : base(subject) { }

    public AndConstraint<IDictionaryAssertion<TKey, TValue>> ContainKey(TKey key, string reason = null)
      => Assert(Subject.ContainsKey(key), "contain key", key.ToString(), reason);

    public AndConstraint<IDictionaryAssertion<TKey, TValue>> NotContainKey(TKey key, string reason = null)
      => Assert(!Subject.ContainsKey(key), "not contain key", key.ToString(), reason);
  }
}