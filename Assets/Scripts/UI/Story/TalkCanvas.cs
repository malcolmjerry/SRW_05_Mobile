using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkCanvas : MonoBehaviour {

  private Transform up, down, geo;
  private bool isUp = true;
  //private Pilot lastPilot;
  int lastPicId;
  string lastShortName;
  private Action callback;

  void Awake() {
    up = transform.Find( "Up" );
    down = transform.Find( "Down" );
    geo = transform.Find( "Geo" );
  }

  // Use this for initialization
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    if (Input.GetButtonDown( "Confirm" )) {
      //EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuConfirm" ) );
      enabled = false;
      callback();
    }
    /*
    else if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      //gameObject.SetActive( false );
      //this.enabled = false;
      //callback();  //Back to up level menu
      closeSelf();
    }*/
  }

  public void Setup( Action callback ) {
    this.callback = callback;

    up.GetComponent<TalkController>().Init();
    down.GetComponent<TalkController>().Init();
  }

  public void Say( /*Pilot pilot,*/ int picId, int faceNo, string shortName, string say1, string say2 = "", string say3 = "", bool? upDown = null ) {
    geo.gameObject.SetActive( false );
    if (upDown.HasValue)
      isUp = upDown.Value;
    else if (picId != lastPicId || shortName != lastShortName)
      isUp = !isUp;

    lastPicId = picId;
    lastShortName = shortName;
    var target = isUp ? up : down;
    var noTarget = isUp ? down : up;
    target.gameObject.SetActive( true );

    target.GetComponent<TalkController>().Setup( $"{picId}_{faceNo}", shortName, say1, say2, say3 );
    noTarget.GetComponent<TalkController>().Mask( true );

    enabled = false;
    CoroutineCommon.CallWaitForSeconds( .2f, () => enabled = true );   //測試時0, 正式時 0.5
  }

  public void ShowPlace( string shortName, float waitTime = 1f ) {
    up.gameObject.SetActive( false );
    down.gameObject.SetActive( false );
    geo.gameObject.SetActive( true );

    geo.GetComponent<GeoController>().Setup( shortName );
    enabled = false;
    CoroutineCommon.CallWaitForSeconds( waitTime, () => enabled = true );
  }

  public void TurnOff() {
    up.gameObject.SetActive( false );
    down.gameObject.SetActive( false );
    enabled = false;
  }

}
