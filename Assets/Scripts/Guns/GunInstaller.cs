using PachowStudios.Framework.Messaging;
using UnityEngine;
using Zenject;

namespace PachowStudios.LudumDare37.Guns
{
  [AddComponentMenu("LD37/Gun/Gun Installer")]
  public partial class GunInstaller : MonoInstaller
  {
    [SerializeField] private Settings config = null;

    private IEventAggregator EventAggregator { get; set; }

    [Inject]
    public void Construct(IEventAggregator eventAggregator) => EventAggregator = eventAggregator;

    public override void InstallBindings()
    {
      Container.Bind<GunModel>().AsSingle();
      Container.BindInstance(this.config.Components).WhenInjectedInto<GunModel>();

      Container.BindAllInterfaces<EventAggregator>().FromMethod(c => EventAggregator.CreateChildContext()).AsSingle();
    }
  }
}
