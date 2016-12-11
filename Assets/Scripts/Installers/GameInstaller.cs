using InControl;
using PachowStudios.Framework.Messaging;
using UnityEngine;
using Zenject;

namespace PachowStudios.LudumDare37.Installers
{
  [CreateAssetMenu(menuName = "LD37/Installers/Game Installer")]
  public class GameInstaller : ScriptableObjectInstaller
  {
    [SerializeField] private InControlManager inControlManager = null;

    public override void InstallBindings()
    {
      Container.Bind<InControlManager>().FromPrefab(this.inControlManager).AsSingle().NonLazy();

      Container.BindAllInterfaces<EventAggregator>().To<EventAggregator>().AsSingle();
    }
  }
}
