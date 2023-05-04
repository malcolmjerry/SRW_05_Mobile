using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmDialog : SelectableMenu {

  private Action fail;
  //private string message;

  //private

  void Awake() {
    menuItemTfList.Add( transform.Find( "Cancel" ) );
    menuItemTfList.Add( transform.Find( "Confirm" ) );
    setupBase( 2, true, false, maxRow: 1 );
  }

	// Use this for initialization
	void Start () {

  }
	
	// Update is called once per frame
	protected override void Update () {
    moveCursor();

    if (directionEnum == Direction.Left || directionEnum == Direction.Right)
      UpdateSelectedItem();

    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      Cancel();
    }
    else if (Input.GetButtonDown( "Confirm" )) {
      if (currentSelected == 0)
        Cancel();
      else if (currentSelected == 1)
        confirm();
    }
  }

  public void Setup( string message, Action callback, Action fail ) {
    this.callback = callback;
    this.fail = fail;
    //this.message = message;

    transform.Find( "Message" ).GetComponent<Text>().text = message;
    gameObject.SetActive( true );
    reset();
    UpdateSelectedItem();
  }

  public void Cancel() {
    gameObject.SetActive( false );
    EffectSoundController.PLAY_BACK_CANCEL();

    fail();
  }

  protected override void confirm() {
    gameObject.SetActive( false );
    EffectSoundController.PLAY_MENU_CONFIRM();

    callback();
  }
 
  override protected void UpdateSelectedItem() {
    foreach (var menuItem in menuItemTfList) {
      menuItem.Find( "Selected" ).gameObject.SetActive( false );
    }

    menuItemTfList[currentSelectedHori].Find( "Selected" ).gameObject.SetActive( true );
  }
  
}
