using DataModel.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static TerrainHelper;
using static UnitController;

public class LaunchListController : SelectableMenu {

  private new Action<UnitInfo, Action> next;
  private Transform headerTf;

  UnitInfo shipUnit;

  //protected GameDataService gameDataService;
  private List<UnitInfo> unitList;
  private int groupNo = 1;

  public AbilityController3 ability;

  MapManager mapManager;

  void Awake() {
    headerTf = transform.Find( "RobotList/Header" );
    menuItemTfList.Add( transform.Find( "RobotList/Row1" ) ); menuItemTfList.Add( transform.Find( "RobotList/Row2" ) ); 
    menuItemTfList.Add( transform.Find( "RobotList/Row3" ) ); menuItemTfList.Add( transform.Find( "RobotList/Row4" ) ); 
    menuItemTfList.Add( transform.Find( "RobotList/Row5" ) ); menuItemTfList.Add( transform.Find( "RobotList/Row6" ) );
    menuItemTfList.Add( transform.Find( "RobotList/Row7" ) ); menuItemTfList.Add( transform.Find( "RobotList/Row8" ) ); 
    menuItemTfList.Add( transform.Find( "RobotList/Row9" ) ); menuItemTfList.Add( transform.Find( "RobotList/Row10" ) ); 
    menuItemTfList.Add( transform.Find( "RobotList/Row11" ) );
    //gameDataService = DIContainer.Instance.GameDataService;
    //GetComponent<AbilityController2>().MyAwake();
  }

  // Use this for initialization
  void Start() {
    
  }

  private void OnEnable() {
    setupBase( Math.Max( unitList.Count, 1 ), false, true );
    if (lastRobotSeqNo.HasValue) {
      int index = unitList.FindIndex( u => u.RobotInfo.RobotInstance.SeqNo == lastRobotSeqNo );
      if (index >= 0)
        SetPageAndRowBySelected( index );
    }
    transform.Find( "PageArrow" ).gameObject.SetActive( maxPage > 1 );

    updateDisplayList();
    updateSelected();
    updateGroup();
  }

  public void Setup( UnitInfo shipUnit, List<UnitInfo> unitList, Action<UnitInfo, Action> next, Action callback, MapManager mapManager ) {
    this.shipUnit = shipUnit;
    this.unitList = unitList;
    this.callback = callback;
    this.next = next;
    autoCloseSelf = true;
    this.mapManager = mapManager;
  }

  // Update is called once per frame
  protected override void Update() {
    moveCursor();
    if (directionEnum != Direction.Zero) {
      updateDisplayList();
      updateSelected();
    }

    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      //gameObject.SetActive( false );
      //this.enabled = false;
      //callback();  //Back to up level menu
      closeSelf();
    }
    else if (Input.GetButtonDown( "Confirm" )) {
      confirm();
    }
    else if (Input.GetButtonDown( "CursorSpeed" ) || Input.GetButtonDown( "Info" )) {
      EffectSoundController.PLAY_MENU_CONFIRM();
      //transform.GetComponent<AbilityController2>().Setup( unitList[currentSelected].MapFightingUnit, () => {
      ability.Setup( unitList[currentSelected].MapFightingUnit, () => {
        gameObject.SetActive( true );
      }, AbilityController3.PAGE.Robot );
      gameObject.SetActive( false );
    }
    else if (Input.GetButtonDown( "NextUnit" ) || Input.GetButtonDown( "PrevUnit" )) {
      groupNo++;
      if (groupNo > 3) 
        groupNo = 1;
      updateGroup();
    }
    else if (Input.GetButtonDown( "NextEnemy" ) || Input.GetButtonDown( "PrevEnemy" )) {
      groupNo--;
      if (groupNo < 1) 
        groupNo = 3;
      updateGroup();
    }
  }

  /*
  public void Setup( Action callback ) {
    this.enabled = true;
    this.callback = callback;
  }*/

  private void updateDisplayList() {
    //Debug.Log( "updateDisplayList" );
    var pageList = GetPageList<UnitInfo>( unitList );

    for (int i = 0; i < maxRow; i++) {
      var item = menuItemTfList[i];

      if (i >= pageList.Count) {
        item.Find( "Robot" ).GetComponent<Text>().text = "";
        item.Find( "Group1/HpTxt" ).GetComponent<Text>().text = "";
        item.Find( "Group1/HpSlider" ).gameObject.SetActive( false );
        item.Find( "Group1/EnTxt" ).GetComponent<Text>().text = "";
        item.Find( "Group1/EnSlider" ).gameObject.SetActive( false );
        item.Find( "Group1/Move" ).GetComponent<Text>().text = "";
        item.Find( "Group1/Lv" ).GetComponent<Text>().text = "";
        item.Find( "Group1/Pilot" ).GetComponent<Text>().text = "";

        item.Find( "Group2/Lv" ).GetComponent<Text>().text = "";
        item.Find( "Group2/Exp" ).GetComponent<Text>().text = "";
        item.Find( "Group2/SP" ).GetComponent<Text>().text = "";
        item.Find( "Group2/Mobility" ).GetComponent<Text>().text = "";
        item.Find( "Group2/Armor" ).GetComponent<Text>().text = "";
        item.Find( "Group2/Hit" ).GetComponent<Text>().text = "";

        item.Find( "Group3/Sky" ).GetComponent<Text>().text = "";
        item.Find( "Group3/Land" ).GetComponent<Text>().text = "";
        item.Find( "Group3/Sea" ).GetComponent<Text>().text = "";
        item.Find( "Group3/Space" ).GetComponent<Text>().text = "";
        item.Find( "Group3/MoveType" ).GetComponent<Text>().text = "";
      }
      else {
        var unitInfo = pageList[i];
        var robotTxt = item.Find( "Robot" );
        robotTxt.GetComponent<Text>().text = unitInfo.RobotInfo.RobotInstance.Robot.FullName;
        //Debug.Log( "unitInfo.GetComponent<UnitController>().status " + unitInfo.GetComponent<UnitController>().status );
        robotTxt.GetComponent<Text>().color = unitInfo.GetComponent<UnitController>().status == UnitStatusEnum.ACTION_END ? Color.red : Color.white;

        item.Find( "Group1/HpTxt" ).GetComponent<Text>().text = unitInfo.RobotInfo.HP.ToString() + " / " + unitInfo.RobotInfo.MaxHP.ToString();
        var hpSlider = item.Find( "Group1/HpSlider" );
        hpSlider.GetComponent<Slider>().value = (float)unitInfo.RobotInfo.HP / unitInfo.RobotInfo.MaxHP;
        hpSlider.gameObject.SetActive( true );
        item.Find( "Group1/EnTxt" ).GetComponent<Text>().text = unitInfo.RobotInfo.EN.ToString() + " / " + unitInfo.RobotInfo.MaxEN.ToString();
        var enSlider = item.Find( "Group1/EnSlider" );
        enSlider.GetComponent<Slider>().value = (float)unitInfo.RobotInfo.EN / unitInfo.RobotInfo.MaxEN;
        enSlider.gameObject.SetActive( true );

        item.Find( "Group1/Move" ).GetComponent<Text>().text = unitInfo.MapFightingUnit.MovePower.ToString();
        item.Find( "Group1/Lv" ).GetComponent<Text>().text = unitInfo.PilotInfo.Level.ToString();
        item.Find( "Group1/Pilot" ).GetComponent<Text>().text = unitInfo.PilotInfo.ShortName;
        item.Find( "Group2/Lv" ).GetComponent<Text>().text = unitInfo.PilotInfo.Level.ToString();
        item.Find( "Group2/Exp" ).GetComponent<Text>().text = unitInfo.PilotInfo.NextLevel.ToString();
        item.Find( "Group2/SP" ).GetComponent<Text>().text = unitInfo.PilotInfo.RemainSp.ToString();
        item.Find( "Group2/Mobility" ).GetComponent<Text>().text = unitInfo.RobotInfo.Motility.ToString();
        item.Find( "Group2/Armor" ).GetComponent<Text>().text = unitInfo.RobotInfo.Armor.ToString();
        item.Find( "Group2/Hit" ).GetComponent<Text>().text = unitInfo.RobotInfo.HitRate.ToString();

        item.Find( "Group3/Sky" ).GetComponent<Text>().text = GET_TerrainRank( GetTerrain( unitInfo.MapFightingUnit, TerrainEnum.Sky ) );
        item.Find( "Group3/Land" ).GetComponent<Text>().text = GET_TerrainRank( GetTerrain( unitInfo.MapFightingUnit, TerrainEnum.Land ) );
        item.Find( "Group3/Sea" ).GetComponent<Text>().text = GET_TerrainRank( GetTerrain( unitInfo.MapFightingUnit, TerrainEnum.Sea ) );
        item.Find( "Group3/Space" ).GetComponent<Text>().text = GET_TerrainRank( GetTerrain( unitInfo.MapFightingUnit, TerrainEnum.Space ) );
        item.Find( "Group3/MoveType" ).GetComponent<Text>().text = unitInfo.RobotInfo.GetMoveTypeStr();
      }
    }
    headerTf.Find( "Page" ).GetComponent<Text>().text = page.ToString();
    headerTf.Find( "MaxPage" ).GetComponent<Text>().text = maxPage.ToString();
  }

  private void updateSelected() {
    UpdateSelectedItem();
  }

  private void updateGroup() {
    var pageList = GetPageList<UnitInfo>( unitList );

    headerTf.Find( "Group1" ).gameObject.SetActive( groupNo == 1 );
    headerTf.Find( "Group2" ).gameObject.SetActive( groupNo == 2 );
    headerTf.Find( "Group3" ).gameObject.SetActive( groupNo == 3 );

    for (int i = 0; i < maxRow; i++) {
      if (i < pageList.Count) {
        var item = menuItemTfList[i];
        item.Find( "Group1" ).gameObject.SetActive( groupNo == 1 );
        item.Find( "Group2" ).gameObject.SetActive( groupNo == 2 );
        item.Find( "Group3" ).gameObject.SetActive( groupNo == 3 );
      }
    }
  }

  private int? lastRobotSeqNo = null;

  protected override void confirm() {
    //transform.Find( "RobotMenu" ).gameObject.SetActive( true );
    //enabled = false;
    UnitInfo unitInfo = unitList[currentSelected];
    unitInfo.GetComponent<UnitController>().myMapManager = mapManager;

    if (unitInfo.GetComponent<UnitController>().status == UnitStatusEnum.ACTION_END) {
      EffectSoundController.PLAY_ACTION_FAIL();
      return;
    }

    gameObject.SetActive( false );
    EffectSoundController.PLAY_MENU_CONFIRM();

    lastRobotSeqNo = unitInfo.RobotInfo.RobotInstance.SeqNo;
    unitList.RemoveAt( currentSelected );
    row = 1;

    next( unitInfo, () => {
      int index = unitList.FindIndex( u => u.RobotInfo.HP < unitInfo.RobotInfo.HP );
      if (index < 0) index = unitList.Count;

      unitList.Insert( index, unitInfo );
      unitInfo.gameObject.SetActive( false );
      unitInfo.IsOnShip = true;

      //unitList.Add( unitInfo );
      gameObject.SetActive( true );
      Camera.main.GetComponent<MainCamera>().MoveCamToCenterObject( shipUnit.transform );
      //enabled = true;
    } );

    /*
    transform.Find( "RobotMenu" ).GetComponent<RobotMenuController>().Setup(
      () => {
        gameObject.SetActive( true );
        this.enabled = true;
      },
      unitList[currentSelected]
    );
    */
  }

}
