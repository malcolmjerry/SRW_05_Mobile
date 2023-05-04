using UnityEngine;
using System.Collections;

public static class AnimationExtensions {

  public static IEnumerator WhilePlaying( this Animation animation ) {
    do {
      yield return null;
    } while (animation.isPlaying);
  }
  
  public static IEnumerator WhilePlaying( this Animator animator, string animationName ) {
    do {
      yield return null;
    } while (animator.GetCurrentAnimatorStateInfo( 0 ).normalizedTime <= 1);
  }
  
}