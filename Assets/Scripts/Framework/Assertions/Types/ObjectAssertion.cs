namespace PachowStudios.Framework.Assertions
{
  public class ObjectAssertion : ReferenceTypeAssertion<object, ObjectAssertion>
  {
    public ObjectAssertion(object subject)
      : base(subject) { }

    public AndConstraint<ObjectAssertion> Equal(object @object, string reason = null)
      => Assert(Equals(Subject, @object), "equal", @object, reason);

    public AndConstraint<ObjectAssertion> NotEqual(object @object, string reason = null)
      => Assert(!Equals(Subject, @object), "not equal", @object, reason);
  }
}