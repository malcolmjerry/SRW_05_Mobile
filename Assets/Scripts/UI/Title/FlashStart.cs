using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
    waitToggle(false);
  }
	
	// Update is called once per frame
	void Update () {
		
	}

  private void waitToggle( bool show ) {
    float waitTime = show? 0.3f : 1f;

    CoroutineCommon.CallWaitForSeconds( waitTime, () => {
      gameObject.SetActive( !gameObject.activeSelf );
      waitToggle( !show );
    } );
  }

}
