using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

public class BGMController : MonoBehaviour {

  //public static BGMController Instance;
  static AudioSource audioSource;

  void Awake() {
    //this.InstantiateController();
    
    audioSource = GetComponent<AudioSource>();
    /*
    AudioClip clip = (AudioClip)Resources.Load( "BGM/04" );
    audioSource.clip = clip;
    audioSource.loop = true;
    audioSource.Play();*/
  }

  /*
  private void InstantiateController() {
    if (Instance == null) {
      Instance = this;
      DontDestroyOnLoad( this );
    } else if (this != Instance) {
      Destroy( this.gameObject );
    }
  }*/
  /*
  public static void SET_BGM( AudioClip clip, bool loop = true, float volumnScale = 1 ) {
    if (audioSource.clip == clip)
      return;

    audioSource.clip = clip;
    audioSource.Stop();
    audioSource.loop = loop;
    audioSource.volume = volumnScale;
    audioSource.Play();
  }
  */

  static AudioClip bgmStart;
  static AudioClip bgmLoop;
  static bool isLoop;
  public static void SET_BGM( string name, bool loop = true, float volumnScale = 10 ) {
    bgmStart = Resources.Load<AudioClip>( $"BGM/{name}" );
    bgmLoop = Resources.Load<AudioClip>( $"BGM/{name}-Loop" );

    if (bgmLoop == null)
      bgmLoop = bgmStart;

    if (audioSource.clip == bgmStart || audioSource.clip == bgmLoop)
      return;

    //audioSource.Stop();
    //isSettingNew = true;
    isLoop = loop;
    audioSource.loop = false;
    audioSource.volume = volumnScale;
    audioSource.clip = bgmStart;
    audioSource.Play();
  }

  public static void SET_BGM2( string name, string name2, bool loop = true, float volumnScale = 10 ) {

    bgmStart = Resources.Load<AudioClip>( $"BGM/{name}" );
    bgmLoop = Resources.Load<AudioClip>( $"BGM/{name2}" );

    if (bgmLoop == null)
      bgmLoop = bgmStart;

    if (audioSource.clip == bgmStart)
      return;

    isLoop = loop;
    audioSource.loop = false;
    audioSource.volume = volumnScale;
    audioSource.clip = bgmStart;
    audioSource.Play();
  }

  private void Update() {
    if (!audioSource.isPlaying && isLoop) {
      audioSource.clip = bgmLoop;
      audioSource.Play();
    }
  }

  public static void STOP_BGM() {
    audioSource.Stop();
    isLoop = false;
  }

  public static void SET_RANDOM_BGM( params string[] bgmList ) {
    if (bgmList.Length == 0)
      return;

    int index = Random.Range( 0, bgmList.Length );
    SET_BGM( bgmList[index] );
  }

}
