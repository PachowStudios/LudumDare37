using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Zenject
{
  public static class ZenjectExtensions
  {
    [NotNull]
    public static T InstantiateComponentPrefab<T>([NotNull] this IInstantiator instantiator, [NotNull] T prefab)
      where T : MonoBehaviour
      => instantiator.InstantiatePrefabForComponent<T>(prefab);

    [NotNull]
    public static ConditionBinder ByPrefabLookup<TKey, TPrefab>(
      [NotNull] this FactorySubContainerBinder<TKey, TPrefab> binder,
      IDictionary<TKey, TPrefab> prefabs)
      where TPrefab : UnityObject
      => binder.ByMethod((c, k) => c.Bind<TPrefab>().FromPrefab(prefabs[k]));

    [NotNull]
    public static ConditionBinder ByPrefabLookup<TParam1, TKey, TPrefab>(
      [NotNull] this FactorySubContainerBinder<TParam1, TKey, TPrefab> binder,
      IDictionary<TKey, TPrefab> prefabs)
      where TPrefab : UnityObject
      => binder.ByMethod(
        (c, p1, k) =>
        {
          c.Bind<TPrefab>().FromPrefab(prefabs[k]);
          c.BindInstance(p1).WhenInjectedInto<TPrefab>();
        });
  }
}