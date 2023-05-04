using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpComSelectTarget : MonoBehaviour {

  //public GameObject m_targetCursorPrefab;
  public GameObject myTargetCursor;

  private Action backToCaller;
  private Action<UnitInfo> next;
  private List<GameObject> rangeList;

  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      myTargetCursor.GetComponent<Cursor>().SetDisable();
      backToCaller();
      enabled = false;
    }
    else if (Input.GetButtonDown( "Confirm" )) {
      var unitSelected = myTargetCursor.GetComponent<Cursor>().unitSelected;
      if (!unitSelected || !rangeList.Contains( unitSelected.gameObject )) {
        EffectSoundController.PLAY_ACTION_FAIL();
        return;
      }

      next( unitSelected.GetComponent<UnitInfo>() );
      myTargetCursor.GetComponent<Cursor>().SetDisable();
      enabled = false;
    }
  }

  public void Setup( Vector3 pos, List<GameObject> rangeList, Action backToCaller, Action<UnitInfo> next ) {
    this.backToCaller = backToCaller;
    this.next = next;
    this.rangeList = rangeList;

    //myTargetCursor = Instantiate( m_targetCursorPrefab ) as GameObject;
    //myTargetCursor.name = "SpComSelectTargetCursor";
    myTargetCursor.SetActive( true );
    myTargetCursor.GetComponent<Cursor>().SetPosition( pos );
    myTargetCursor.GetComponent<Cursor>().ChangeModeWithDistance( Cursor.ModeEnum.MAP, null, rangeList, null );
    enabled = true;
  }

}
