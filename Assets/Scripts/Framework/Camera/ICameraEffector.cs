namespace PachowStudios.Framework.Camera
{
  public interface ICameraEffector : ICameraPositionAssertion
  {
    float GetEffectorWeight();
  }
}