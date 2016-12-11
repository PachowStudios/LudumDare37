using PachowStudios.Framework.Messaging;
using PachowStudios.LudumDare37.Guns;
using UnityEngine;
using Zenject;

namespace PachowStudios.LudumDare37.Player
{
  [AddComponentMenu("LD37/Player/Player Installer")]
  public partial class PlayerInstaller : MonoInstaller
  {
    [SerializeField] private Settings config = null;

    private IEventAggregator EventAggregator { get; set; }

    [Inject]
    public void Construct(IEventAggregator eventAggregator) => EventAggregator = eventAggregator;

    public override void InstallBindings()
    {
      Container.Bind<PlayerModel>().AsSingle();
      Container.BindInstance(this.config.Components).WhenInjectedInto<PlayerModel>();

      Container.Bind<PlayerInput>().AsSingle();

      Container.BindAllInterfaces<PlayerMoveHandler>().To<PlayerMoveHandler>().AsSingle();
      Container.BindInstance(this.config.MoveHandler).WhenInjectedInto<PlayerMoveHandler>();

      Container.BindAllInterfaces<PlayerRotationHandler>().To<PlayerRotationHandler>().AsSingle();
      Container.BindAllInterfaces<PlayerShootHandler>().To<PlayerShootHandler>().AsSingle();

      Container.BindAllInterfaces<PlayerGunSelector>().To<PlayerGunSelector>().AsSingle();
      Container.BindInstance(this.config.GunSelector).WhenInjectedInto<PlayerGunSelector>();

      Container.BindAllInterfaces<EventAggregator>().FromMethod(c => EventAggregator.CreateChildContext()).AsSingle();
    }
  }
}
