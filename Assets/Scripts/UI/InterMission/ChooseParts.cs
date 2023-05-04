using DataModel.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ChooseParts : SelectableMenu {

  private MapFightingUnit unit;
  //private MapFightingUnit preUnit;

  protected GameDataService gameDataService;
  protected RobotService robotService;
  protected PartsService partsService;

  Action back;
  Action<PartsInstance> preSelect, done;

  List<PartsGroup> partsGroupList;
  private Transform headerTf;

  public GameObject PartsRobotList;

  void Awake() {
    gameDataService = DIContainer.Instance.GameDataService;
    robotService = DIContainer.Instance.RobotService;
    partsService = DIContainer.Instance.PartsService;

    for (int i=1; i<=10; i++)
      menuItemTfList.Add( transform.Find( $"PartsList/Row{i}" ) );

    headerTf = transform.Find( "PartsList/Header" );
  }

  // Use this for initialization
  void Start() {

  }

  // Update is called once per frame
  protected override void Update() {
    moveCursor();
    if (directionEnum != Direction.Zero) {
      if (directionEnum == Direction.Up || directionEnum == Direction.Down) {
        updateSelected();
      }
      else if ((directionEnum == Direction.Left || directionEnum == Direction.Right) && maxPage > 1) {
        UpdateDisplayList();
        updateSelected();
      }
      preSelect( partsGroupList[currentSelected].PartsIn );
    }

    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      enabled = false;
      hideSelected();
      back();
    }
    else if (Input.GetButtonDown( "Confirm" )) {
      confirm();
    }
  }

  void OnEnable() {
    updateSelected();
    preSelect( partsGroupList[currentSelected].PartsIn );
  }

  void OnDisable() {
    UpdateSelectedItem();
  }

  public void Setup( MapFightingUnit unit, Action<PartsInstance> preSelect, Action<PartsInstance> done, Action back ) {
    this.preSelect = preSelect;
    this.done = done;
    this.back = back;
    this.unit = unit;

    //preUnit = MyHelper.DeepClone<MapFightingUnit>( unit );
    //transform.Find( "RobotName/Label" ).GetComponent<Text>().text = unit.RobotInfo.RobotInstance.Robot.FullName;

    partsGroupList = partsService.GetPartsGroupList();
    setupBase( partsGroupList.Count, false, true );

    UpdateDisplayList();
    updateSelected();
  }

  private void updateSelected() {
    PartsGroup pg = partsGroupList[currentSelected];
    string desc = pg.PartsIn.Parts.Desc;
    List<string> descList = desc.Split( '\n' ).ToList();

    int addRow = (5 - descList.Count)/2;
    for (int i = 0; i<addRow; i++)
      descList.Insert( 0, "" );

    for (int i = 1; i<=5; i++)
      transform.Find( $"Desc/Content{i}" ).GetComponent<Text>().text = i<=descList.Count ? descList[i-1] : "";

    UpdateSelectedItem();
  }

  public override void UpdateDisplayList() {
    partsGroupList = partsService.GetPartsGroupList();
    var pageList = GetPageList<PartsGroup>( partsGroupList );

    for (int i = 0; i<maxRow; i++) {
      var item = menuItemTfList[i];

      if (i>=pageList.Count) {
        item.Find( "PartsName" ).GetComponent<Text>().text = "";
        item.Find( "Count" ).GetComponent<Text>().text = "";
        item.Find( "Total" ).GetComponent<Text>().text = "";
      }
      else {
        item.Find( "PartsName" ).GetComponent<Text>().text = pageList[i].PartsIn.Parts.Name;
        item.Find( "Count" ).GetComponent<Text>().text = pageList[i].Count.ToString();
        item.Find( "Total" ).GetComponent<Text>().text = pageList[i].Total.ToString();
      }
    }
    headerTf.Find( "Page" ).GetComponent<Text>().text = page.ToString();
    headerTf.Find( "MaxPage" ).GetComponent<Text>().text = maxPage.ToString();
  }

  /*
  private void updateUnit( MapFightingUnit targetUnit ) {
    targetUnit.Update();
  }

  private void updateValue( string rowName, float oldVal, float newVal, int lv ) {
    var rowTf = transform.Find( rowName );
    rowTf.Find( "Old" ).GetComponent<Text>().text = oldVal.ToString();
    rowTf.Find( "New" ).GetComponent<Text>().text = newVal.ToString();

    if (newVal > oldVal) rowTf.Find( "New" ).GetComponent<Text>().color = Color.green;
    else if (newVal < oldVal) rowTf.Find( "New" ).GetComponent<Text>().color = Color.red;
    else rowTf.Find( "New" ).GetComponent<Text>().color = Color.white;

    rowTf.Find( "ImproveBarGo" ).Find( "ImproveBar2" ).GetComponent<ImproveBarScript>().SetLevel( lv );
  }

  private void updateValue( string rowName, int oldVal, int newVal, int lv ) {
    var rowTf = transform.Find( rowName );
    rowTf.Find( "Old" ).GetComponent<Text>().text = oldVal.ToString();
    rowTf.Find( "New" ).GetComponent<Text>().text = newVal.ToString();

    if (newVal > oldVal) rowTf.Find( "New" ).GetComponent<Text>().color = Color.green;
    else if (newVal < oldVal) rowTf.Find( "New" ).GetComponent<Text>().color = Color.red;
    else rowTf.Find( "New" ).GetComponent<Text>().color = Color.white;

    rowTf.Find( "ImproveBarGo" ).Find( "ImproveBar2" ).GetComponent<ImproveBarScript>().SetLevel( lv );
  }
  */

  protected override void confirm() {
    var partsGroup = partsGroupList[currentSelected];

    if (partsGroup.Total < 1) {
      EffectSoundController.PLAY_ACTION_FAIL();
      return;
    }

    //PartsInstance usable = partsService.GetUsableByPartsID( partsGroupList[currentSelected].PartsID );
    List<PartsInstance> allParts = partsService.GetAllPartsById( partsGroup.PartsID );
    var usable = allParts.FirstOrDefault( p => !p.RobotInstanceSeqNo.HasValue );
    if (usable != null) {  //找到有未裝備的零件 可直接套上
      done( usable );
      UpdateDisplayList();
      enabled = false;
      EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/SpCom/Sp_2" ) );
      hideSelected();
      return;
    }

    List<MapUnitParts> mapUnitPartsList = allParts.Select( pi => new MapUnitParts() {
      mapUnit = gameDataService.HouseUnits.FirstOrDefault( m => m.RobotInfo.RobotInstance.SeqNo == pi.RobotInstanceSeqNo ),     
      partsIn = pi
    } ).OrderByDescending( m => m.mapUnit.RobotInfo.HP ).ToList();

    gameObject.SetActive( false );

    EffectSoundController.PLAY_MENU_CONFIRM();
    PartsRobotList.gameObject.SetActive( true );
    PartsRobotList.GetComponent<PartsRobotList>().Setup( unit, mapUnitPartsList,
      next: (PartsInstance pi) => {
        gameObject.SetActive( true );
        done( pi );
        UpdateDisplayList();
        enabled = false;
        EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/SpCom/Sp_2" ) );
        hideSelected();
      },
      callback: () => {
        gameObject.SetActive( true );
      }
    );

  }

}

public class MapUnitParts {
  public MapFightingUnit mapUnit;
  public PartsInstance partsIn;
}


