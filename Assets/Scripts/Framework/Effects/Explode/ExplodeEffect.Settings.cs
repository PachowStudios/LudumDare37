using UnityEngine;

namespace PachowStudios.Framework.Effects
{
  public partial class ExplodeEffect
  {
    [InstallerSettings, CreateAssetMenu(menuName = "Pachow Studios/Effects/Explode Effect Settings")]
    public class Settings : ScriptableObject
    {
      public ExplodeEffectView ExplosionPrefab;
      public float Duration = 5f;
      public float ParticleLifetime = 1f;
    }
  }
}
