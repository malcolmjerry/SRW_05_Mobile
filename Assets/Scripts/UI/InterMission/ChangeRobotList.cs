using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TerrainHelper;
using System.Linq;
using DataModel.Service;

public class ChangeRobotList : SelectableMenu {

  //private Action callback;
  //private Action next;

  //private Transform totalTf, robotTf, pilotTf, saveTf, loadTf, systemTf, nextTf;
  private Transform headerTf;
  //private List<Transform> menuItemTfList = new List<Transform>();
  private Transform row10, row11;  //展示 Robot其他資料, 如Parts, 地形, 駕駛Lv, 照準 等

  protected GameDataService gameDataService;
  private List<MapFightingUnit> unitList = new List<MapFightingUnit>();
  private MapFightingUnit unit;

  void Awake() {
    headerTf = transform.Find( "Header" );

    menuItemTfList.Add( transform.Find( "Row1" ) ); menuItemTfList.Add( transform.Find( "Row2" ) ); menuItemTfList.Add( transform.Find( "Row3" ) );
    menuItemTfList.Add( transform.Find( "Row4" ) ); menuItemTfList.Add( transform.Find( "Row5" ) ); menuItemTfList.Add( transform.Find( "Row6" ) );
    menuItemTfList.Add( transform.Find( "Row7" ) ); menuItemTfList.Add( transform.Find( "Row8" ) ); menuItemTfList.Add( transform.Find( "Row9" ) );
    menuItemTfList.Add( transform.Find( "Row10" ) );

    gameDataService = DIContainer.Instance.GameDataService;
  }

  // Use this for initialization
  void Start() {

  }

  void OnEnable() {
    unitList = gameDataService.HouseUnits.Where( m => m.RobotInfo != null && m.RobotInfo.RobotInstance.Robot.DriveType.HasValue
                 //&& m.RobotInfo.RobotInstance.Robot.DriveType == unit.PilotInfo.PilotInstance.Pilot.DriveType
                 && m.RobotInfo != unit.RobotInfo
               )
               .OrderByDescending( mfu => mfu.RobotInfo.MaxHP ).ToList();

    if (unitList == null || unitList.Count <= 0) {
      noRecordBack();
    }

    setupBase( unitList.Count, false, true );
    /*
    if (lastRobotSeqNo.HasValue) {
      int index = unitList.FindIndex( u => u.RobotInfo.RobotInstance.SeqNo == lastRobotSeqNo );
      SetPageAndRowBySelected( index );
    }*/

    transform.parent.Find( "PageArrow" ).gameObject.SetActive( maxPage > 1 );

    updateDisplayList();
    UpdateSelectedItem();
  }

  public void Setup( MapFightingUnit unit, Action callback ) {
    Setup( callback );
    //this.callback = callback;
    this.unit = unit;
  }

  // Update is called once per frame
  protected override void Update() {
    moveCursor();
    if (directionEnum != Direction.Zero) {
      updateDisplayList();
      UpdateSelectedItem();
    }

    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      closeSelf();
    }
    else if (Input.GetButtonDown( "Confirm" )) {
      confirm();
    }
    else if (Input.GetButtonDown( "CursorSpeed" )) {   //--I
      transform.parent.GetComponent<AbilityController2>().Setup( unitList[currentSelected], () => {
        gameObject.SetActive( true );
      } );
      gameObject.SetActive( false );
    }
    else if (Input.GetButtonDown( "Info" )) {   //---J
      transform.parent.GetComponent<AbilityController2>().Setup( unit, () => {
        gameObject.SetActive( true );
      } );
      gameObject.SetActive( false );
    }
  }

  private void updateDisplayList() {
    var pageList = GetPageList<MapFightingUnit>( unitList );

    for (int i = 0; i<maxRow; i++) {
      var item = menuItemTfList[i];

      if (i>=pageList.Count) {
        item.Find( "Robot" ).GetComponent<Text>().text = "";
        item.Find( "HP" ).GetComponent<Text>().text = "";
        item.Find( "Delimiter" ).GetComponent<Text>().text = "";
        item.Find( "EN" ).GetComponent<Text>().text = "";
        item.Find( "Pilot" ).GetComponent<Text>().text = "";
        item.Find( "LV" ).GetComponent<Text>().text = "";
      }
      else {
        item.Find( "Robot" ).GetComponent<Text>().text = pageList[i].RobotInfo.RobotInstance.Robot.FullName;
        item.Find( "HP" ).GetComponent<Text>().text = pageList[i].RobotInfo.MaxHP.ToString();
        item.Find( "Delimiter" ).GetComponent<Text>().text = "／";
        item.Find( "EN" ).GetComponent<Text>().text = pageList[i].RobotInfo.MaxEN.ToString();
        item.Find( "Pilot" ).GetComponent<Text>().text = pageList[i].PilotInfo?.PilotInstance.Pilot.ShortName?? "— — — — —";
        item.Find( "LV" ).GetComponent<Text>().text = pageList[i].PilotInfo?.Level.ToString()?? "--";
      }
    }
    headerTf.Find( "Page" ).GetComponent<Text>().text = page.ToString();
    headerTf.Find( "MaxPage" ).GetComponent<Text>().text = maxPage.ToString();
  }

  protected override void confirm() {
    gameObject.SetActive( false );
    EffectSoundController.PLAY_MENU_CONFIRM();

    var tf = transform.parent.Find( "ChangePilot" );
    tf.GetComponent<ChangePilot>().Setup(
      unit: unit,
      preUnit: unitList[currentSelected],
      callback: () => gameObject.SetActive( true ),
      done: callback
    );

    tf.gameObject.SetActive( true );
  }

  private void noRecordBack() {
    EffectSoundController.PLAY_ACTION_FAIL();
    closeSelf();
  }

}
