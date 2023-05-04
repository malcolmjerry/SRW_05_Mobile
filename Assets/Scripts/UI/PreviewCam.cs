using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCam : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    float ajust = Input.GetAxis( "Mouse ScrollWheel" );

    if (ajust != 0) {
      if (Input.GetKey( KeyCode.LeftShift )) {
        float resultY = transform.localPosition.y + ajust;
        if (resultY > 134) resultY = 134;
        else if (resultY < 129) resultY = 129;
        transform.localPosition = new Vector3( transform.localPosition.x, resultY, transform.localPosition.z );
        //Debug.Log( "Preview Cam: " + transform.localPosition );
      }
      else {
        float resultZ = transform.localPosition.z + ajust;
        if (resultZ > 8) resultZ = 8;
        else if (resultZ < 2) resultZ = 2;

        transform.localPosition = new Vector3( transform.localPosition.x, transform.localPosition.y, resultZ );
        //Debug.Log( "Preview Cam: " + transform.localPosition );
      }
    }
  }
}
