using DataModel.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TerrainHelper;

public class ChangePilot : SelectableMenu {

  private MapFightingUnit unit;
  private MapFightingUnit preUnit;

  private int movePowerAdd, hpAdd, enAdd, motilityAdd, armorAdd, hitRateAdd, weaponAdd;
  private int movePowerMoney, hpMoney, enMoney, motilityMoney, armorMoney, hitRateMoney, weaponMoney;

  private int needMoney;
  protected GameDataService gameDataService;

  private Action done;

  void Awake() {
    gameDataService = DIContainer.Instance.GameDataService;
    menuItemTfList.Add( transform.Find( "ConfirmBox/No" ) );
    menuItemTfList.Add( transform.Find( "ConfirmBox/Yes" ) );
  }

  // Use this for initialization
  void Start() {
    setupBase( 2, false, true );
  }

  private void OnEnable() {
    UpdateSelectedItem();
    updatePage();
  }

  // Update is called once per frame
  protected override void Update() {
    moveCursor();

    if (directionEnum == Direction.Up || directionEnum == Direction.Down) {
      UpdateSelectedItem();
    }

    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      closeSelf(); 
    }
    else if (Input.GetButtonDown( "Confirm" )) {
      confirm();
    }
    else if (Input.GetButtonDown( "CursorSpeed" )) {   //--I
      transform.parent.GetComponent<AbilityController2>().Setup( preUnit, () => {
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

  public void Setup( MapFightingUnit unit, MapFightingUnit preUnit, Action callback, Action done ) {
    Setup( callback );  //Just back to upper menu
    this.done = done;
    this.unit = unit;
    this.preUnit = preUnit;
    createModel();

    transform.Find( "RobotName/Label" ).GetComponent<Text>().text = preUnit.RobotInfo.RobotInstance.Robot.FullName;
    //transform.Find( "PilotInfo/PilotPic" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( $"Character/{unit.PilotInfo.PilotInstance.PilotID}_1" );
    transform.Find( "PilotInfo/PilotPic" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + unit.PilotInfo.PicNo + "_1" );
    transform.Find( "PilotInfo/PilotNameTxt" ).GetComponent<Text>().text = unit.PilotInfo.ShortName;
    transform.Find( "PilotInfo/LvTxt" ).GetComponent<Text>().text = unit.PilotInfo.Level.ToString();
    transform.Find( "PilotInfo/NextTxt" ).GetComponent<Text>().text = unit.PilotInfo.NextLevel.ToString();
    transform.Find( "PilotInfo/KillsTxt" ).GetComponent<Text>().text = unit.PilotInfo.PilotInstance.Kills.ToString();
  }

  private GameObject model;
  private void createModel() {
    Destroy( model );
    GameObject prefab = Resources.Load( "Battle/Units/" + preUnit.RobotInfo.RobotInstance.Robot.Name ) as GameObject;
    //model = Instantiate( prefab, new Vector3( 0, 128, 0 ), prefab.transform.rotation );
    model = Instantiate( prefab, transform );
    //model.transform.SetParent( transform );
    model.transform.localPosition = new Vector3( 0, 128, 0 );
    model.transform.localRotation = prefab.transform.localRotation;
    model.AddComponent<Rotating>();
  }

  private void updatePage() {
    updateValue( "MovePowerRow", unit.RobotInfo==null? null : (float?)unit.MovePower, preUnit.MovePower );
    updateValue( "HpRow", unit.RobotInfo?.MaxHP, preUnit.RobotInfo.MaxHP );
    updateValue( "EnRow", unit.RobotInfo?.MaxEN, preUnit.RobotInfo.MaxEN );
    updateValue( "MotilityRow", unit.RobotInfo?.Motility, preUnit.RobotInfo.Motility );
    updateValue( "ArmorRow", unit.RobotInfo?.Armor, preUnit.RobotInfo.Armor );
    updateValue( "HitRateRow", unit.RobotInfo?.HitRate, preUnit.RobotInfo.HitRate );

    updateTerrain( "Sky", TerrainEnum.Sky );
    updateTerrain( "Land", TerrainEnum.Land );
    updateTerrain( "Sea", TerrainEnum.Sea );
    updateTerrain( "Space", TerrainEnum.Space );
  }

  private void updateValue( string rowName, float? oldVal, float newVal ) {
    var rowTf = transform.Find( rowName );
    rowTf.Find( "Old" ).GetComponent<Text>().text = oldVal?.ToString()?? "---";
    rowTf.Find( "New" ).GetComponent<Text>().text = newVal.ToString();

    if (newVal > oldVal) rowTf.Find( "New" ).GetComponent<Text>().color = Color.green;
    else if (newVal < oldVal) rowTf.Find( "New" ).GetComponent<Text>().color = Color.red;
    else rowTf.Find( "New" ).GetComponent<Text>().color = Color.white;
  }

  private void updateValue( string rowName, int? oldVal, int newVal ) {
    var rowTf = transform.Find( rowName );
    rowTf.Find( "Old" ).GetComponent<Text>().text = oldVal?.ToString()?? "---";
    rowTf.Find( "New" ).GetComponent<Text>().text = newVal.ToString();

    if (newVal > oldVal) rowTf.Find( "New" ).GetComponent<Text>().color = Color.green;
    else if (newVal < oldVal) rowTf.Find( "New" ).GetComponent<Text>().color = Color.red;
    else rowTf.Find( "New" ).GetComponent<Text>().color = Color.white;
  }

  private void updateTerrain( string terrainName, TerrainEnum terrainEnum ) {
    float oldScore = GetTerrain( unit, terrainEnum );
    float newScore = GetTerrain( preUnit, terrainEnum );

    transform.Find( $"TerrainRow/Old{terrainName}" ).GetComponent<Text>().text = oldScore == 0? "—" : GET_TerrainRank( oldScore );

    Text textCom = transform.Find( $"TerrainRow/New{terrainName}" ).GetComponent<Text>();
    textCom.text = GET_TerrainRank( newScore );

    if (newScore > oldScore) textCom.color = Color.green;
    else if (newScore < oldScore) textCom.color = Color.red;
    else textCom.color = Color.white;
  }

  protected override void confirm() {
    if (row == 1) {
      EffectSoundController.PLAY_BACK_CANCEL();
      closeSelf();
      return;
    }

    if (row == 2) {
      EffectSoundController.PLAY_MENU_CONFIRM();
      enabled = false;
      GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeOut(
        sec: .5f,
        callback: () => {
          enabled = true;
          gameObject.SetActive( false );
          done();
        },
        doProcess: () => {
          unit.ChangePilot( preUnit );
          gameDataService.CheckAndRemoveUnit( unit );
        },
        blackTime: 0f
      );
      return;
    }
  }

}


