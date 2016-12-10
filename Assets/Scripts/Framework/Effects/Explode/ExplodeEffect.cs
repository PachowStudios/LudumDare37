using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace PachowStudios.Framework.Effects
{
  public partial class ExplodeEffect
  {
    private Settings Config { get; }
    private IInstantiator Instantiator { get; }

    public ExplodeEffect(Settings config, IInstantiator instantiator)
    {
      Config = config;
      Instantiator = instantiator;
    }

    public void Explode([NotNull] Transform target, Vector3 velocity, [NotNull] Sprite sprite, Material material = null)
      => Instantiator
        .InstantiateComponentPrefab(Config.ExplosionPrefab)
        .Explode(target, velocity, sprite, material);
  }
}