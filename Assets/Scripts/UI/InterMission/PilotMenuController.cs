using DataModel.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PilotMenuController : SelectableMenu {

  private MapFightingUnit unit;
  protected GameDataService gameDataService;
  private bool canChangeRobot;

  void Awake() {
    menuItemTfList.Add( transform.Find( "Improve" ) );
    menuItemTfList.Add( transform.Find( "Change" ) );
    menuItemTfList.Add( transform.Find( "Ability" ) );

    gameDataService = DIContainer.Instance.GameDataService;
  }

  // Use this for initialization
  void Start() {
    setupBase( 3, false, true );
    UpdateSelectedItem();
  }

  /*
  protected override void Update() {
    base.Update();
  }
  */

  private void OnEnable() {
    //reset();
    //updateSelectedItem();
  }

  Action showAbility;
  public void Setup( Action callback, MapFightingUnit unit, Action showAbility ) {
    Setup( callback );
    this.unit = unit;
    this.showAbility = showAbility;
    canChangeRobot = checkCanChangeRobot();
    menuItemTfList[1].GetComponent<Text>().color = canChangeRobot? Color.white : Color.red;
  }

  protected override void confirm() {
    switch (currentSelected+1) {
      case 1:
        EffectSoundController.PLAY_ACTION_FAIL();
        break;
      case 2:
        if (!canChangeRobot) {
          EffectSoundController.PLAY_ACTION_FAIL();
          return;
        }

        gameObject.SetActive( false );
        transform.parent.gameObject.SetActive( false );
        EffectSoundController.PLAY_MENU_CONFIRM();
        var tf = transform.parent.parent.Find( "ChangeRobotList" );
        tf.GetComponent<ChangeRobotList>().Setup( unit, callback );
        tf.gameObject.SetActive( true );
        break;
      case 3:
        EffectSoundController.PLAY_MENU_CONFIRM();
        //transform.parent.parent.GetComponent<AbilityController>().Setup( unit, callback, AbilityController.PAGE.Pilot );
        showAbility();
        gameObject.SetActive( false );
        break;
      default:
        break;
    }
  }

  private bool checkCanChangeRobot() {
    if (unit.PilotInfo == null || !unit.PilotInfo.PilotInstance.Pilot.DriveType.HasValue)
      return false;

    return gameDataService.HouseUnits.Any( m => m.RobotInfo != null && m.RobotInfo.RobotInstance.Robot.DriveType.HasValue
             //&& m.RobotInfo.RobotInstance.Robot.DriveType == unit.PilotInfo.PilotInstance.Pilot.DriveType
             && m.RobotInfo != unit.RobotInfo
           );
  }

}
