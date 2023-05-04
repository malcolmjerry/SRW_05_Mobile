using DataModel.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PartsRobotList : SelectableMenu {

  private Transform headerTf;
  private Action<PartsInstance> next;
  private List<MapUnitParts> mapUnitPartsList;
  private MapFightingUnit unit;
  private Text partsName;
  private Text robotName;
  private GameDataService gameDataService;

  void Awake() {
    headerTf = transform.Find( "Header" );
    partsName = transform.Find( "PreHead1/PartsName" ).GetComponent<Text>();
    robotName = transform.Find( "PreHead1/RobotName" ).GetComponent<Text>();

    menuItemTfList.Add( transform.Find( "Row1" ) ); menuItemTfList.Add( transform.Find( "Row2" ) ); menuItemTfList.Add( transform.Find( "Row3" ) );
    menuItemTfList.Add( transform.Find( "Row4" ) ); menuItemTfList.Add( transform.Find( "Row5" ) ); menuItemTfList.Add( transform.Find( "Row6" ) );
    menuItemTfList.Add( transform.Find( "Row7" ) ); menuItemTfList.Add( transform.Find( "Row8" ) ); menuItemTfList.Add( transform.Find( "Row9" ) );

    gameDataService = DIContainer.Instance.GameDataService;
  }

  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  protected override void Update() {
    base.Update();
  }

  void OnEnable() {
    transform.parent.Find( "PageArrow" ).gameObject.SetActive( maxPage > 1 );
    reset();
  }

  public void Setup( MapFightingUnit unit, List<MapUnitParts> mapUnitPartsList, Action<PartsInstance> next , Action callback ) {
    this.next = next;
    this.callback = callback;
    this.mapUnitPartsList = mapUnitPartsList;
    this.unit = unit;
    autoCloseSelf = true;

    partsName.text = mapUnitPartsList[0].partsIn.Parts.Name;
    robotName.text = unit.RobotInfo.RobotInstance.Robot.FullName;

    setupBase( Math.Max( mapUnitPartsList.Count, 1 ), false, true );
    updateDisplayList();
  }

  private void updateDisplayList() {
    var unitList = mapUnitPartsList.Select( m => m.mapUnit ).ToList();
    var pageList = GetPageList<MapFightingUnit>( unitList );

    for (int i = 0; i < maxRow; i++) {
      var item = menuItemTfList[i];

      if (i >= pageList.Count) {
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
        item.Find( "Pilot" ).GetComponent<Text>().text = pageList[i].PilotInfo.ShortName;
      }
    }
    headerTf.Find( "Page" ).GetComponent<Text>().text = page.ToString();
    headerTf.Find( "MaxPage" ).GetComponent<Text>().text = maxPage.ToString();
  }

  protected override void confirm() {
    gameObject.SetActive( false );
    EffectSoundController.PLAY_MENU_CONFIRM();
    next( mapUnitPartsList[currentSelected].partsIn );
  }
}
