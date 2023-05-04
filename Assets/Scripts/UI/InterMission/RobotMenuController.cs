using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMenuController : SelectableMenu {

  private MapFightingUnit unit;

  void Awake() {
    menuItemTfList.Add( transform.Find( "Improve" ) );
    menuItemTfList.Add( transform.Find( "SetParts" ) );
    menuItemTfList.Add( transform.Find( "Ability" ) );
  }

  // Use this for initialization
  void Start () {
    setupBase( 3, false, true );
    UpdateSelectedItem();
  }

  // Update is called once per frame
  protected override void Update () {
    base.Update();
    /*
    moveCursor();
    if (directionEnum != Direction.Zero)
      updateSelectedItem();

    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      closeSelf();
    }
    else if (Input.GetButtonDown( "Confirm" )) {
      confirm();
    }
    */
  }

  Action showAbility;
  public void Setup( Action callback, MapFightingUnit unit, Action showAbility ) {
    Setup( callback );
    this.unit = unit;
    this.showAbility = showAbility;
  }

  override protected void confirm() {
    EffectSoundController.PLAY_MENU_CONFIRM();
    switch (currentSelected+1) {
      case 1:
        gameObject.SetActive( false );
        transform.parent.gameObject.SetActive( false );
        var robotImproveTf = transform.parent.parent.Find( "RobotImprove" );
        robotImproveTf.gameObject.SetActive( true );
        robotImproveTf.GetComponent<RobotImproveController>().Setup( callback, unit );
        break;
      case 2:
        gameObject.SetActive( false );
        transform.parent.gameObject.SetActive( false );
        var partsTf = transform.parent.parent.Find( "PartsChange" );
        partsTf.gameObject.SetActive( true );
        partsTf.GetComponent<ChangeParts>().Setup( unit, callback );
        break;
      case 3:
        //transform.parent.parent.GetComponent<AbilityController>().Setup( unit, callback, AbilityController.PAGE.Robot );
        showAbility();
        gameObject.SetActive( false );
        break;
      default:
        break;
    }
  }

}
