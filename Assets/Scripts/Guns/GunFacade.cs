using UnityEngine;
using Zenject;

namespace PachowStudios.LudumDare37.Guns
{
  [AddComponentMenu("LD37/Gun/Gun Facade")]
  public class GunFacade : MonoBehaviour
  {
    public bool IsShooting
    {
      get { return Model.IsShooting; }
      set { Model.IsShooting = value; }
    }

    public Vector2 AimDirection
    {
      get { return Model.AimDirection; }
      set { Model.AimDirection = value; }
    }

    private GunModel Model { get; set; }

    [Inject]
    public void Construct(GunModel model) => Model = model;
  }
}
