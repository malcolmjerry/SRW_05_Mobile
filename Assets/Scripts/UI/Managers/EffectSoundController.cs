using UnityEngine;
using System.Collections;

public class EffectSoundController : MonoBehaviour {

  static AudioSource audioSource0;
  static AudioSource audioSource1;

  public const int EffectSoundSale = 3;

  // Use this for initialization
  void Awake() {
    audioSource0 = GetComponents<AudioSource>()[0];
    audioSource1 = GetComponents<AudioSource>()[1];
  }

  public static void PLAY( AudioClip clip, float volumnScale = 1 ) {
    //audioSource.clip = clip;
    audioSource0?.PlayOneShot( clip, volumnScale );
  }

  public static void STOP() {
    audioSource0.Stop();
  }

  public static void PLAY_MENU_CONFIRM() {
    audioSource0?.PlayOneShot( (AudioClip)Resources.Load( "SFX/menuConfirm" ), 1.5f );
  }

  public static void PLAY_MENU_MOVE() {
    audioSource0.PlayOneShot( (AudioClip)Resources.Load( "SFX/menuMove" ), 3f );
  }

  public static void PLAY_ACTION_FAIL() {
    audioSource0.PlayOneShot( (AudioClip)Resources.Load( "SFX/actionFail" ), 2f );
  }

  public static void PLAY_BACK_CANCEL() {
    audioSource0?.PlayOneShot( (AudioClip)Resources.Load( "SFX/menuBack" ), 4f );
  }

  public static void PLAY_PHASE_CTL() {
    audioSource0.PlayOneShot( (AudioClip)Resources.Load( "SFX/SpCom/Sp_2" ), 2f );
  }

  public static void PLAY_LV_UP1() {
    audioSource0.PlayOneShot( Resources.Load<AudioClip>( "SFX/LevelUp1" ), 2f );
  }

  public static void PLAY_LV_UP2() {
    audioSource1.PlayOneShot( Resources.Load<AudioClip>( "SFX/LevelUp2" ), 2f );
  }

  public static void PLAY_1( AudioClip clip, float volumnScale = 1 ) {
    //audioSource.clip = clip;
    audioSource1.PlayOneShot( clip, volumnScale );
  }

  public static void PLAY_1( int id, float volumnScale = 1 ) {
    var audioClip = Resources.Load<AudioClip>( $"SFX/Voice/{id}" );

    if (audioClip)
      audioSource1.PlayOneShot( audioClip, volumnScale );
  }
}
