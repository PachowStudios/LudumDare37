using PachowStudios.Framework.Messaging;
using UnityEngine;
using Zenject;

namespace PachowStudios.LudumDare37.Player
{
  [AddComponentMenu("LD37/Player/Player Facade")]
  public class PlayerFacade : MonoBehaviour
  {
    private PlayerModel Model { get; set; }
    private IEventAggregator EventAggregator { get; set; }

    [Inject]
    public void Construct(PlayerModel model, IEventAggregator eventAggregator)
    {
      Model = model;
      EventAggregator = eventAggregator;
    }
  }
}
