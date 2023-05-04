using DataModel.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static TerrainHelper;

public class ChangeParts : SelectableMenu {

  private MapFightingUnit unit;
  private MapFightingUnit preUnit;

  protected GameDataService gameDataService;
  protected RobotService robotService;

  void Awake() {
    gameDataService = DIContainer.Instance.GameDataService;
    robotService = DIContainer.Instance.RobotService;

    menuItemTfList.Add( transform.Find( "PartsOnRobot/Parts1" ) );
    menuItemTfList.Add( transform.Find( "PartsOnRobot/Parts2" ) );
    menuItemTfList.Add( transform.Find( "PartsOnRobot/Parts3" ) );
    menuItemTfList.Add( transform.Find( "PartsOnRobot/Parts4" ) );

    this.enabled = false;
    GetComponent<ChooseParts>().enabled = false;
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
      else if (directionEnum == Direction.Left || directionEnum == Direction.Right) {
        //changeAdd();
        //updateMoney();
      }
    }

    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      closeSelf();
    }
    else if (Input.GetButtonDown( "Confirm" )) {
      confirm();
    }
    else if (Input.GetButtonDown( "CursorSpeed" )) {    //I - Ability
      EffectSoundController.PLAY_MENU_CONFIRM();
      transform.parent.GetComponent<AbilityController2>().Setup( unit, () => {
        gameObject.SetActive( true );
      } );
      gameObject.SetActive( false );
    }
    else if (Input.GetButtonDown( "Info" )) {   //J - unplug part
      if (unit.RobotInfo.RobotInstance.PartsInstanceList[currentSelected] == null) {
        EffectSoundController.PLAY_ACTION_FAIL();
        return;
      }

      EffectSoundController.PLAY_BACK_CANCEL();

      robotService.SetupParts( unit.RobotInfo.RobotInstance, null, row );
      unit.UpdateInit();
      preUnit = unit.Clone(); //MyHelper.DeepClone<MapFightingUnit>( this.unit );
      updatePage();
      updateSelected();
      GetComponent<ChooseParts>().UpdateDisplayList();
    }
  }

  public void Setup( MapFightingUnit unit, Action callback ) {
    Setup( callback );
    this.unit = unit;
    this.enabled = true;

    preUnit = unit.Clone(); //MyHelper.DeepClone<MapFightingUnit>( unit );
    setupBase( unit.RobotInfo.RobotInstance.PartsInstanceList.Count, false, true );
    //reset();

    transform.Find( "RobotName/Label" ).GetComponent<Text>().text = unit.RobotInfo.RobotInstance.Robot.FullName;
    updatePage();

    GetComponent<ChooseParts>().Setup(
      unit,
      preSelect: ( PartsInstance partsInstance ) => {
        var clonePartsIn = partsInstance.Clone(); //MyHelper.DeepClone<PartsInstance>( partsInstance );
        clonePartsIn.UnplugFromRobot();
        robotService.SetupParts( preUnit.RobotInfo.RobotInstance, clonePartsIn, row );
        updatePage();
      },
      done: (PartsInstance partsInstance) => {
        this.enabled = true;
        var oldUnit = gameDataService.HouseUnits.Where( h => h.RobotInfo.RobotInstance.SeqNo == partsInstance.RobotInstanceSeqNo ).FirstOrDefault();
        robotService.SetupParts( unit.RobotInfo.RobotInstance, partsInstance, row );
        oldUnit?.UpdateInit();
        unit.UpdateInit();

        preUnit = unit.Clone(); //MyHelper.DeepClone<MapFightingUnit>( this.unit );
        updatePage();
        updateSelected();
        //menuItemTfList[row].Find( "Name" ).GetComponent<Text>().color = Color.white;
      },
      back: () => {
        preUnit = unit.Clone();
        updatePage();
        enabled = true; 
      }
    );

    updateSelected();
  }

  /*
  private void changeAdd() {
    int add = directionEnum == Direction.Right ? 1 : -1;
    switch (row) {
      case 1: changeMove( add ); break;
      case 2: changeHP( add ); break;
      case 3: changeEN( add ); break;
      case 4: changeMotility( add ); break;
      case 5: changeArmor( add ); break;
      case 6: changeHitRate( add ); break;
      case 7: changeWeapon( add ); break;
    }
  }

  private void changeMove( int add ) {
    if (movePowerAdd <= 0 && add < 0) return;
    if (preUnit.RobotInfo.RobotInstance.MovePowerLv >= 10 && add > 0) return;
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuMove" ), 3 );
    movePowerAdd += add;
    preUnit.RobotInfo.RobotInstance.MovePowerLv = unit.RobotInfo.RobotInstance.MovePowerLv + movePowerAdd;
    preUnit.Update();
    updateValue( "MovePowerRow", unit.MovePower, preUnit.MovePower, unit.RobotInfo.RobotInstance.MovePowerLv, movePowerAdd );
    movePowerMoney = calculateMoney( unit.RobotInfo.RobotInstance.MovePowerLv, movePowerAdd );
  }

  private void changeHP( int add ) {
    if (hpAdd <= 0 && add < 0) return;
    if (preUnit.RobotInfo.RobotInstance.HPLv >= 10 && add > 0) return;
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuMove" ), 3 );
    hpAdd += add;
    preUnit.RobotInfo.RobotInstance.HPLv = unit.RobotInfo.RobotInstance.HPLv + hpAdd;
    preUnit.Update();
    updateValue( "HpRow", unit.RobotInfo.MaxHP, preUnit.RobotInfo.MaxHP, unit.RobotInfo.RobotInstance.HPLv, hpAdd );
    hpMoney = calculateMoney( unit.RobotInfo.RobotInstance.HPLv, hpAdd );
  }

  private void changeEN( int add ) {
    if (enAdd <= 0 && add < 0) return;
    if (preUnit.RobotInfo.RobotInstance.ENLv >= 10 && add > 0) return;
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuMove" ), 3 );
    enAdd += add;
    preUnit.RobotInfo.RobotInstance.ENLv = unit.RobotInfo.RobotInstance.ENLv + enAdd;
    preUnit.Update();
    updateValue( "EnRow", unit.RobotInfo.MaxEN, preUnit.RobotInfo.MaxEN, unit.RobotInfo.RobotInstance.ENLv, enAdd );
    enMoney = calculateMoney( unit.RobotInfo.RobotInstance.ENLv, enAdd );
  }

  private void changeMotility( int add ) {
    if (motilityAdd <= 0 && add < 0) return;
    if (preUnit.RobotInfo.RobotInstance.MotilityLv >= 10 && add > 0) return;
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuMove" ), 3 );
    motilityAdd += add;
    preUnit.RobotInfo.RobotInstance.MotilityLv = unit.RobotInfo.RobotInstance.MotilityLv + motilityAdd;
    preUnit.Update();
    updateValue( "MotilityRow", unit.RobotInfo.Motility, preUnit.RobotInfo.Motility, unit.RobotInfo.RobotInstance.MotilityLv, motilityAdd );
    motilityMoney = calculateMoney( unit.RobotInfo.RobotInstance.MotilityLv, motilityAdd );
  }

  private void changeArmor( int add ) {
    if (armorAdd <= 0 && add < 0) return;
    if (preUnit.RobotInfo.RobotInstance.ArmorLv >= 10 && add > 0) return;
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuMove" ), 3 );
    armorAdd += add;
    preUnit.RobotInfo.RobotInstance.ArmorLv = unit.RobotInfo.RobotInstance.ArmorLv + armorAdd;
    preUnit.Update();
    updateValue( "ArmorRow", unit.RobotInfo.Armor, preUnit.RobotInfo.Armor, unit.RobotInfo.RobotInstance.ArmorLv, armorAdd );
    armorMoney = calculateMoney( unit.RobotInfo.RobotInstance.ArmorLv, armorAdd );
  }

  private void changeHitRate( int add ) {
    if (hitRateAdd <= 0 && add < 0) return;
    if (preUnit.RobotInfo.RobotInstance.HitLv >= 10 && add > 0) return;
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuMove" ), 3 );
    hitRateAdd += add;
    preUnit.RobotInfo.RobotInstance.HitLv = unit.RobotInfo.RobotInstance.HitLv + hitRateAdd;
    preUnit.Update();
    updateValue( "HitRateRow", unit.RobotInfo.HitRate, preUnit.RobotInfo.HitRate, unit.RobotInfo.RobotInstance.HitLv, hitRateAdd );
    hitRateMoney = calculateMoney( unit.RobotInfo.RobotInstance.HitLv, hitRateAdd );
  }

  private void changeWeapon( int add ) {
    if (weaponAdd <= 0 && add < 0)
      return;
    int weaponLv = unit.RobotInfo.WeaponList[0].WeaponInstance.Level;
    if (weaponLv + weaponAdd >= 10 && add > 0)
      return;
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuMove" ), 3 );
    weaponAdd += add;
    //preUnit.RobotInfo.RobotInstance.HitLv = unit.RobotInfo.RobotInstance.HitLv + hitRateAdd;
    //preUnit.Update();
    updateValue( "WeaponRow", weaponLv, weaponLv + weaponAdd, weaponLv, weaponAdd );
    weaponMoney = calculateMoney( weaponLv, weaponAdd, 12000 );
  }
  */

  private void updateSelected() {
    Parts parts = unit.RobotInfo.RobotInstance.PartsInstanceList[currentSelected]?.Parts;
    string desc = parts?.Desc?? "";
    List<string> descList = desc.Split( '\n' ).ToList();

    int addRow = (5 - descList.Count)/2;
    for (int i = 0; i<addRow; i++)
      descList.Insert( 0, "" );

    for (int i = 1; i<=5; i++)
      transform.Find( $"Desc/Content{i}" ).GetComponent<Text>().text = i<=descList.Count? descList[i-1] : "";

    UpdateSelectedItem();
  }

  private void updatePage() {
    unit.Update();
    preUnit.Update();
    updateValue( "MovePowerRow", unit.MovePower, preUnit.MovePower, unit.RobotInfo.RobotInstance.MovePowerLv );
    updateValue( "HpRow", unit.RobotInfo.MaxHP, preUnit.RobotInfo.MaxHP, unit.RobotInfo.RobotInstance.HPLv );
    updateValue( "EnRow", unit.RobotInfo.MaxEN, preUnit.RobotInfo.MaxEN, unit.RobotInfo.RobotInstance.ENLv );
    updateValue( "MotilityRow", unit.RobotInfo.Motility, preUnit.RobotInfo.Motility, unit.RobotInfo.RobotInstance.MotilityLv );
    updateValue( "ArmorRow", unit.RobotInfo.Armor, preUnit.RobotInfo.Armor, unit.RobotInfo.RobotInstance.ArmorLv );
    updateValue( "HitRateRow", unit.RobotInfo.HitRate, preUnit.RobotInfo.HitRate, unit.RobotInfo.RobotInstance.HitLv );

    updateTerrain( "Sky", TerrainEnum.Sky );
    updateTerrain( "Land", TerrainEnum.Land );
    updateTerrain( "Sea", TerrainEnum.Sea );
    updateTerrain( "Space", TerrainEnum.Space );

    var pList = unit.RobotInfo.RobotInstance.PartsInstanceList;
    for (int i = 0; i<4; i++) {
      menuItemTfList[i].Find( "Name" ).GetComponent<Text>().text = i >= pList.Count ? "" : pList[i]?.Parts.Name?? "— — — — — —";
    }
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

  private void updateTerrain( string terrainName, TerrainEnum terrainEnum ) {
    float oldScore = GetTerrain( unit, terrainEnum );
    float newScore = GetTerrain( preUnit, terrainEnum );

    transform.Find( $"TerrainRow/Old{terrainName}" ).GetComponent<Text>().text = GET_TerrainRank( oldScore );

    Text textCom = transform.Find( $"TerrainRow/New{terrainName}" ).GetComponent<Text>();
    textCom.text = GET_TerrainRank( newScore );

    if (newScore > oldScore) textCom.color = Color.green;
    else if (newScore < oldScore) textCom.color = Color.red;
    else textCom.color = Color.white;
  }

  protected override void confirm() {

    //updateUnit( unit );
    //updatePage();

    EffectSoundController.PLAY_MENU_CONFIRM();
    this.enabled = false;
    GetComponent<ChooseParts>().enabled = true;

    menuItemTfList[row-1].Find( "Name" ).GetComponent<Text>().color = Color.yellow;
  }

  void OnEnable() {
    updateSelected();

    for (int i = 0; i<menuItemTfList.Count; i++) {
      menuItemTfList[i].Find( "Name" ).GetComponent<Text>().color = Color.white;
    }
  }

  void OnDisable() {
    UpdateSelectedItem();
  }
}


