using PachowStudios.LudumDare37.Player;
using UnityEngine;
using Zenject;

namespace PachowStudios.LudumDare37.Guns
{
  public partial class PlayerGunSelector : IInitializable
  {
    private Settings Config { get; }
    private PlayerModel Model { get; }
    private GunFactory GunFactory { get; }

    private GunFacade CurrentGun
    {
      get { return Model.CurrentGun; }
      set { Model.CurrentGun = value; }
    }

    public PlayerGunSelector(Settings config, PlayerModel model, GunFactory gunFactory)
    {
      Config = config;
      Model = model;
      GunFactory = gunFactory;
    }

    public void Initialize() => SelectGun(Config.StartingGun);

    private void SelectGun(GunType gunType)
    {
      var gun = GunFactory.Create(gunType);

      gun.ParentTo(Model.GunPoint);
      gun.transform.localPosition = Vector3.zero;

      CurrentGun.NullToRealNull()?.Destroy();
      CurrentGun = gun;
    }
  }
}
