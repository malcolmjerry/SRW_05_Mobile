using UnityEngine;
using System.Collections;
using System;

public class CoroutineCommon {

  private static readonly MyMonoBehaviour mMonoBehaviour;

  /// <summary>
  /// コルーチンを管理するゲームオブジェクトを生成するコンストラクタ
  /// </summary>
  static CoroutineCommon() {
    var gameObject = new GameObject( "CoroutineCommon" );
    GameObject.DontDestroyOnLoad( gameObject );
    mMonoBehaviour = gameObject.AddComponent<MyMonoBehaviour>();
  }

  /// <summary>
  /// 1 フレーム待機してから Action デリゲートを呼び出します
  /// </summary>
  public static void CallWaitForOneFrame( Action act ) {
    mMonoBehaviour.StartCoroutine( DoCallWaitForOneFrame( act ) );
  }

  public static void CallWaitForFrames( int frameCount, Action act ) {
    mMonoBehaviour.StartCoroutine( DoCallWaitForFrames( frameCount, act ) );
  }

  /// <summary>
  /// 指定された秒数待機してから Action デリゲートを呼び出します
  /// </summary>
  public static void CallWaitForSeconds( float seconds, Action act ) {
    mMonoBehaviour.StartCoroutine( DoCallWaitForSeconds( seconds, act ) );
  }

  public static void CallWaitForAnimator( Animator animator, string animationName, Action act ) {
    mMonoBehaviour.StartCoroutine( DoCallWaitForAnimator( animator, animationName, act ) );
  }

  public static void CallWaitForAnimation( Animation animation, Action act ) {
    mMonoBehaviour.StartCoroutine( DoCallWaitForAnimation( animation, act ) );
  }

  /// <summary>
  /// 1 フレーム待機してから Action デリゲートを呼び出します
  /// </summary>
  private static IEnumerator DoCallWaitForOneFrame( Action act ) {
    yield return 0;
    act();
  }

  private static IEnumerator DoCallWaitForFrames( int frameCount, Action act ) {
    while (frameCount > 0) {
      frameCount--;
      yield return null;
    }
    act();
  }


  /// <summary>
  /// 指定された秒数待機してから Action デリゲートを呼び出します
  /// </summary>
  private static IEnumerator DoCallWaitForSeconds( float seconds, Action act ) {
    yield return new WaitForSeconds( seconds );
    act();
  }

  private static IEnumerator DoCallWaitForAnimator( Animator animator, string animationName, Action act ) {
    yield return animator.WhilePlaying( animationName );
    act();
  }

  private static IEnumerator DoCallWaitForAnimation( Animation animation, Action act ) {
    yield return animation.WhilePlaying();
    act();
  }

}

public class MyMonoBehaviour : MonoBehaviour {
}