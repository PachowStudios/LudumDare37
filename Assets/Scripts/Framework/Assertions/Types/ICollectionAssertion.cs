using System.Collections.Generic;

namespace PachowStudios.Framework.Assertions
{
  public abstract class ICollectionAssertion<TSubject, TSelf> : IEnumerableAssertion<TSubject, TSelf>
    where TSelf : ICollectionAssertion<TSubject, TSelf>
  {
    private new ICollection<TSubject> Subject => (ICollection<TSubject>)base.Subject;

    protected ICollectionAssertion(ICollection<TSubject> subject)
      : base(subject) { }

    public AndConstraint<TSelf> Contain(TSubject item, string reason = null)
      => Assert(Subject.Contains(item), "contain", item.ToString(), reason);

    public AndConstraint<TSelf> NotContain(TSubject item, string reason = null)
      => Assert(!Subject.Contains(item), "not contain", item.ToString(), reason);
  }

  public class ICollectionAssertion<T> : ICollectionAssertion<T, ICollectionAssertion<T>>
  {
    public ICollectionAssertion(ICollection<T> subject)
      : base(subject) { }
  }
}