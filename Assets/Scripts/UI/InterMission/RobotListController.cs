using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TerrainHelper;
using System.Linq;
using DataModel.Service;

public class RobotListController : SelectableMenu {

  //private Action callback;
  private Action next;

  //private Transform totalTf, robotTf, pilotTf, saveTf, loadTf, systemTf, nextTf;
  private Transform headerTf;
  //private List<Transform> menuItemTfList = new List<Transform>();
  private Transform row10, row11;  //展示 Robot其他資料, 如Parts, 地形, 駕駛Lv, 照準 等

  protected GameDataService gameDataService;
  private List<MapFightingUnit> unitList;

  void Awake() {
    transform.Find( "RobotMenu" ).gameObject.SetActive( false );

    headerTf = transform.Find( "Header" );

    menuItemTfList.Add( transform.Find( "Row1" ) ); menuItemTfList.Add( transform.Find( "Row2" ) ); menuItemTfList.Add( transform.Find( "Row3" ) );
    menuItemTfList.Add( transform.Find( "Row4" ) ); menuItemTfList.Add( transform.Find( "Row5" ) ); menuItemTfList.Add( transform.Find( "Row6" ) );
    menuItemTfList.Add( transform.Find( "Row7" ) ); menuItemTfList.Add( transform.Find( "Row8" ) ); menuItemTfList.Add( transform.Find( "Row9" ) );

    row10 = transform.Find( "Row10" );
    row11 = transform.Find( "Row11" );

    gameDataService = DIContainer.Instance.GameDataService;
  }

  // Use this for initialization
  void Start() {
    //unitList = gameDataService.MapFightingUnits.OrderByDescending( mfu => mfu.RobotInfo.MaxHP ).ToList();
    //setupBase( unitList.Count, 9, true, true );

    //updateDisplayList();
    //updateSelected();
  }

  void OnEnable() {
    unitList = gameDataService.HouseUnits.Where( m => m.RobotInfo != null ).OrderByDescending( mfu => mfu.RobotInfo.MaxHP ).ToList();
    setupBase( Math.Max( unitList.Count, 1 ), false, true );
    //setupBase( unitList.Count, 9, true, true );

    if (lastRobotSeqNo.HasValue) {
      int index = unitList.FindIndex( u => u.RobotInfo.RobotInstance.SeqNo == lastRobotSeqNo );
      SetPageAndRowBySelected( index );
    }

    transform.parent.Find( "PageArrow" ).gameObject.SetActive( maxPage > 1 );

    updateDisplayList();
    updateSelected();
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
      showAbility();
      gameObject.SetActive( false );
    }
  }

  /*
  public void Setup( Action callback ) {
    this.enabled = true;
    this.callback = callback;
  }*/

  private void updateDisplayList() {
    var pageList = GetPageList<MapFightingUnit>( unitList );

    for (int i=0; i<maxRow; i++) {
      var item = menuItemTfList[i];

      if (i>=pageList.Count) {
        item.Find( "Robot" ).GetComponent<Text>().text = "";
        item.Find( "HP" ).GetComponent<Text>().text = "";
        item.Find( "Delimiter" ).GetComponent<Text>().text = "";
        item.Find( "EN" ).GetComponent<Text>().text = "";
        item.Find( "Move" ).GetComponent<Text>().text = ""; 
        item.Find( "Mobility" ).GetComponent<Text>().text = "";
        item.Find( "Armor" ).GetComponent<Text>().text = "";
        item.Find( "Pilot" ).GetComponent<Text>().text = "";
      }
      else {
        item.Find( "Robot" ).GetComponent<Text>().text = pageList[i].RobotInfo.RobotInstance.Robot.FullName;
        item.Find( "HP" ).GetComponent<Text>().text = pageList[i].RobotInfo.MaxHP.ToString();
        item.Find( "Delimiter" ).GetComponent<Text>().text = "／";
        item.Find( "EN" ).GetComponent<Text>().text = pageList[i].RobotInfo.MaxEN.ToString();
        item.Find( "Move" ).GetComponent<Text>().text = pageList[i].MovePower.ToString();
        item.Find( "Mobility" ).GetComponent<Text>().text = pageList[i].RobotInfo.Motility.ToString();
        item.Find( "Armor" ).GetComponent<Text>().text = pageList[i].RobotInfo.Armor.ToString();
        item.Find( "Pilot" ).GetComponent<Text>().text = pageList[i].PilotInfo?.ShortName?? "---";
      }
    }
    headerTf.Find( "Page" ).GetComponent<Text>().text = page.ToString();
    headerTf.Find( "MaxPage" ).GetComponent<Text>().text = maxPage.ToString();
  }

  private void updateSelected() {
    /*
    foreach (var menuItem in menuItemTfList) {
      menuItem.Find( "Selected" ).gameObject.SetActive( false );
    }

    menuItemTfList[row-1].Find( "Selected" ).gameObject.SetActive( true );
    */

    MapFightingUnit unit = unitList.Count == 0 ? null : unitList[currentSelected];
    RobotInfo robotInfo = unit?.RobotInfo;

    UpdateSelectedItem();
    row10.Find( "Parts1" ).GetComponent<Text>().text = robotInfo?.RobotInstance.PartsInstanceList[0]?.Parts.Name?? "— — —";
    try { row10.Find( "Parts2" ).GetComponent<Text>().text = robotInfo?.RobotInstance.PartsInstanceList[1]?.Parts.Name?? "— — —"; }
    catch { row10.Find( "Parts2" ).GetComponent<Text>().text = ""; }
    
    row10.Find( "Sky" ).GetComponent<Text>().text = robotInfo == null? "—" : TerrainHelper.GET_TerrainRank( robotInfo.TerrainSky );
    row10.Find( "Land" ).GetComponent<Text>().text = robotInfo == null ? "—" : TerrainHelper.GET_TerrainRank( robotInfo.TerrainLand );
    row10.Find( "Sea" ).GetComponent<Text>().text = robotInfo == null ? "—" : TerrainHelper.GET_TerrainRank( robotInfo.TerrainSea );
    row10.Find( "Space" ).GetComponent<Text>().text = robotInfo == null ? "—" : TerrainHelper.GET_TerrainRank( robotInfo.TerrainSpace );
    row10.Find( "Lv" ).GetComponent<Text>().text     = unit?.PilotInfo?.Level.ToString()?? "--";

    try { row11.Find( "Parts3" ).GetComponent<Text>().text = robotInfo?.RobotInstance.PartsInstanceList[2]?.Parts.Name?? "— — —"; }
    catch { row11.Find( "Parts3" ).GetComponent<Text>().text = ""; }
    try { row11.Find( "Parts4" ).GetComponent<Text>().text = robotInfo?.RobotInstance.PartsInstanceList[3]?.Parts.Name?? "— — —"; }
    catch { row11.Find( "Parts4" ).GetComponent<Text>().text = ""; }
    row11.Find( "HitRate" ).GetComponent<Text>().text = robotInfo?.HitRate.ToString()?? "---";
    row11.Find( "MoveType" ).GetComponent<Text>().text = robotInfo?.GetMoveTypeStr()?? "— — — —";
    
    //headerTf.Find( "Page" ).GetComponent<Text>().text = page.ToString();
    //headerTf.Find( "MaxPage" ).GetComponent<Text>().text = maxPage.ToString();
  }

  private int? lastRobotSeqNo = null;

  protected override void confirm() {
    transform.Find( "RobotMenu" ).gameObject.SetActive( true );
    this.enabled = false;
    EffectSoundController.PLAY_MENU_CONFIRM();

    lastRobotSeqNo = unitList[currentSelected].RobotInfo.RobotInstance.SeqNo;

    transform.Find( "RobotMenu" ).GetComponent<RobotMenuController>().Setup(
      () => {
        gameObject.SetActive( true );
        enabled = true;
      },
      unitList[currentSelected],
      showAbility
    );
  }

  void NextPrev( int nextPrev, int head ) {
    if (nextPrev > 0)
      moveDown();
    else
      moveUp();

    showAbility( (AbilityController2.PAGE)head );
  }

  void showAbility() {
    showAbility( AbilityController2.PAGE.Robot );
  }

  void showAbility( AbilityController2.PAGE abPage ) {
    lastRobotSeqNo = unitList[currentSelected].RobotInfo.RobotInstance.SeqNo;
    transform.parent.GetComponent<AbilityController2>().Setup( unitList[currentSelected], 
      () => {
        gameObject.SetActive( true );
        enabled = true;
      },
      abPage, 
      NextPrev 
    );
  }

}
