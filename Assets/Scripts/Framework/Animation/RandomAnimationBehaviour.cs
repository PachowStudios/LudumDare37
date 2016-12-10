using UnityEngine;

namespace PachowStudios.Framework.Animation
{
  [AddComponentMenu("Pachow Studios/Animation/Random Animation Behaviour")]
  public class RandomAnimationBehaviour : StateMachineBehaviour
  {
    [SerializeField] private int states = 0;
    [SerializeField] private string parameter = null;

    public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
      => animator.SetInteger(this.parameter, Random.Range(0, this.states));
  }
}