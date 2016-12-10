using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Extensions;

namespace PachowStudios.Framework.Assertions
{
  public abstract class IEnumerableAssertion<TSubject, TSelf> : ReferenceTypeAssertion<IEnumerable<TSubject>, TSelf>
    where TSelf : IEnumerableAssertion<TSubject, TSelf>
  {
    protected IEnumerableAssertion(IEnumerable<TSubject> subject)
      : base(subject) { }

    public AndConstraint<TSelf> BeEmpty(string reason = null)
      => Assert(Subject.IsEmpty(), "be", "empty", reason);

    public AndConstraint<TSelf> NotBeEmpty(string reason = null)
      => Assert(Subject.Any(), "not be", "empty", reason);

    public AndConstraint<TSelf> HaveNoneWhere(Func<TSubject, bool> condition, string reason = null)
      => Assert(Subject.None(condition), "have none where", $"{condition} is true", reason);

    public AndConstraint<TSelf> HaveAtLeast(int amount, string reason = null)
      => Assert(Subject.HasAtLeast(amount), "have at lesast", $"{amount} items", reason);

    public AndConstraint<TSelf> HaveMoreThan(int amount, string reason = null)
      => Assert(Subject.HasMoreThan(amount), "have more than", $"{amount} items", reason);

    public AndConstraint<TSelf> HaveAtMost(int amount, string reason = null)
      => Assert(Subject.HasAtMost(amount), "have at most", $"{amount} items", reason);

    public AndConstraint<TSelf> HaveLessThan(int amount, string reason = null)
      => Assert(Subject.HasLessThan(amount), "have less than", $"{amount} items", reason);

    public AndConstraint<TSelf> HaveExactly(int amount, string reason = null)
      => Assert(Subject.HasExactly(amount), "have exactly", $"{amount} items", reason);

    public AndConstraint<TSelf> HaveSingleItem(string reason = null)
      => Assert(Subject.HasSingle(), "have", "a single item", reason);

    public AndConstraint<TSelf> HaveMultipleItems(string reason = null)
      => Assert(Subject.HasMultiple(), "have", "multiple items", reason);
  }

  public class IEnumerableAssertion<T> : IEnumerableAssertion<T, IEnumerableAssertion<T>>
  {
    public IEnumerableAssertion(IEnumerable<T> subject)
      : base(subject) { }
  }
}