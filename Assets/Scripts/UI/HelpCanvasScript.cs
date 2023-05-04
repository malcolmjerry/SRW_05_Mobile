using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpCanvasScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

  bool active = false;

	// Update is called once per frame
	void Update () {
    if (Input.GetKeyDown( KeyCode.Tab )) {
      active = !active;
      if (active) EffectSoundController.PLAY_MENU_CONFIRM();
      else        EffectSoundController.PLAY_BACK_CANCEL();

      transform.Find( "Background" ).gameObject.SetActive( active );
    }
    /*
    else if (Input.GetKeyUp( KeyCode.Tab )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      transform.Find( "Background" ).gameObject.SetActive( false );
    }*/
  }
}
