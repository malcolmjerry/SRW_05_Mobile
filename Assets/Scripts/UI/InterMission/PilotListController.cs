using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TerrainHelper;
using System.Linq;
using DataModel.Service;
using System.Text;

public class PilotListController : SelectableMenu {

  private Transform headerTf;
  private Transform row10, row11;  //展示 Robot其他資料, 如Parts, 地形, 駕駛Lv, 照準 等

  protected GameDataService gameDataService;
  private List<MapFightingUnit> unitList;

  void Awake() {
    transform.Find( "PilotMenu" ).gameObject.SetActive( false );

    headerTf = transform.Find( "Header" );

    menuItemTfList.Add( transform.Find( "Row1" ) ); menuItemTfList.Add( transform.Find( "Row2" ) ); menuItemTfList.Add( transform.Find( "Row3" ) );
    menuItemTfList.Add( transform.Find( "Row4" ) ); menuItemTfList.Add( transform.Find( "Row5" ) ); menuItemTfList.Add( transform.Find( "Row6" ) );
    menuItemTfList.Add( transform.Find( "Row7" ) ); menuItemTfList.Add( transform.Find( "Row8" ) ); 

    row10 = transform.Find( "Row10" );
    row11 = transform.Find( "Row11" );

    gameDataService = DIContainer.Instance.GameDataService;
  }

  // Use this for initialization
  void Start() {
  }

  void OnEnable() {
    unitList = gameDataService.HouseUnits.Where( m => m.PilotInfo != null ).OrderByDescending( mfu => mfu.PilotInfo.Level ).ThenBy( m => m.PilotInfo.ShortName ).ToList();
    setupBase( Math.Max( 1, unitList.Count), false, true );

    if (lastSeqNo.HasValue) {
      int index = unitList.FindIndex( u => u.PilotInfo.PilotInstanceSeqNo == lastSeqNo );
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

    for (int i = 0; i<maxRow; i++) {
      var item = menuItemTfList[i];

      if (i>=pageList.Count) {
        item.Find( "Pilot" ).GetComponent<Text>().text = "";
        item.Find( "Lv" ).GetComponent<Text>().text = "";
        item.Find( "Shoot" ).GetComponent<Text>().text = "";
        item.Find( "Melee" ).GetComponent<Text>().text = "";
        item.Find( "Avoid" ).GetComponent<Text>().text = "";
        item.Find( "Hit" ).GetComponent<Text>().text = "";
        item.Find( "Def" ).GetComponent<Text>().text = "";
        item.Find( "Dex" ).GetComponent<Text>().text = "";
        item.Find( "Next" ).GetComponent<Text>().text = "";
      }
      else {
        item.Find( "Pilot" ).GetComponent<Text>().text = pageList[i].PilotInfo.ShortName;
        item.Find( "Lv" ).GetComponent<Text>().text = pageList[i].PilotInfo.Level.ToString();
        item.Find( "Shoot" ).GetComponent<Text>().text = pageList[i].PilotInfo.Shoot.ToString();
        item.Find( "Melee" ).GetComponent<Text>().text = pageList[i].PilotInfo.Melee.ToString();
        item.Find( "Avoid" ).GetComponent<Text>().text = pageList[i].PilotInfo.Dodge.ToString();
        item.Find( "Hit" ).GetComponent<Text>().text = pageList[i].PilotInfo.Hit.ToString();
        item.Find( "Def" ).GetComponent<Text>().text = pageList[i].PilotInfo.Defense.ToString();
        item.Find( "Dex" ).GetComponent<Text>().text = pageList[i].PilotInfo.Dex.ToString();
        item.Find( "Next" ).GetComponent<Text>().text =  pageList[i].PilotInfo.NextLevel.ToString();
      }
    }

  }

  private void updateSelected() {
    MapFightingUnit unit = unitList.Count == 0 ? null : unitList[currentSelected];

    UpdateSelectedItem();

    var pilotInfo = unit?.PilotInfo;
    var spList = pilotInfo?.PilotInstance.Pilot.SPComPilots;
    StringBuilder sb = new StringBuilder();
    if (spList != null)
      for (int i = 0; i<spList.Count; i++) {
        sb.Append( pilotInfo.Level >= spList[i].Level ? spList[i].SPCommand.Name : "???" );
        sb.Append( "　" );
      }

    row10.Find( "SpCom" ).GetComponent<Text>().text = sb.ToString();
    row10.Find( "Sky" ).GetComponent<Text>().text   = pilotInfo == null ? "—" : TerrainHelper.GET_TerrainRank( pilotInfo.TerrainSky );
    row10.Find( "Land" ).GetComponent<Text>().text  = pilotInfo == null ? "—" : TerrainHelper.GET_TerrainRank( pilotInfo.TerrainLand );
    row10.Find( "Sea" ).GetComponent<Text>().text   = pilotInfo == null ? "—" : TerrainHelper.GET_TerrainRank( pilotInfo.TerrainSea );
    row10.Find( "Space" ).GetComponent<Text>().text = pilotInfo == null ? "—" : TerrainHelper.GET_TerrainRank( pilotInfo.TerrainSpace );

    row11.Find( "RobotName" ).GetComponent<Text>().text = unit?.RobotInfo?.RobotInstance.Robot.FullName?? "---(無搭乘)---";
    row11.Find( "MovePower" ).GetComponent<Text>().text = unit?.RobotInfo?.MovePower.ToString()?? "------";

    row11.Find( "Kills" ).GetComponent<Text>().text = pilotInfo?.PilotInstance.Kills.ToString()?? "------";
    row11.Find( "Sp" ).GetComponent<Text>().text = pilotInfo == null? "---／---" : $"{pilotInfo.RemainSp}／{pilotInfo.MaxSp}";

    headerTf.Find( "Page" ).GetComponent<Text>().text = page.ToString();
    headerTf.Find( "MaxPage" ).GetComponent<Text>().text = maxPage.ToString();
  }

  private int? lastSeqNo = null;

  protected override void confirm() {
    transform.Find( "PilotMenu" ).gameObject.SetActive( true );
    this.enabled = false;
    EffectSoundController.PLAY_MENU_CONFIRM();

    lastSeqNo = unitList[currentSelected].PilotInfo.PilotInstanceSeqNo;

    transform.Find( "PilotMenu" ).GetComponent<PilotMenuController>().Setup(
      () => {
        gameObject.SetActive( true );
        this.enabled = true;
      },
      unitList[currentSelected], showAbility
    );
  }

  void showAbility() {
    showAbility( AbilityController2.PAGE.Pilot );
  }

  void showAbility( AbilityController2.PAGE abPage ) {
    lastSeqNo = unitList[currentSelected].PilotInfo.PilotInstanceSeqNo;
    transform.parent.GetComponent<AbilityController2>().Setup( unitList[currentSelected],
      () => {
        gameObject.SetActive( true );
        enabled = true;
      },
      abPage,
      NextPrev
    );
  }

  void NextPrev( int nextPrev, int head ) {
    if (nextPrev > 0)
      moveDown();
    else
      moveUp();

    showAbility( (AbilityController2.PAGE)head );
  }

}
