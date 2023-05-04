using DataModel.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotImproveController : SelectableMenu {

  private MapFightingUnit unit;
  private MapFightingUnit preUnit;

  private int movePowerAdd, hpAdd, enAdd, motilityAdd, armorAdd, hitRateAdd, weaponAdd;
  private int movePowerMoney, hpMoney, enMoney, motilityMoney, armorMoney, hitRateMoney, weaponMoney;

  private int needMoney;
  protected GameDataService gameDataService;

  void Awake() {
    gameDataService = DIContainer.Instance.GameDataService;
    menuItemTfList.Add( transform.Find( "MovePowerRow" ) );
    menuItemTfList.Add( transform.Find( "HpRow" ) );
    menuItemTfList.Add( transform.Find( "EnRow" ) );
    menuItemTfList.Add( transform.Find( "MotilityRow" ) );
    menuItemTfList.Add( transform.Find( "ArmorRow" ) );
    menuItemTfList.Add( transform.Find( "HitRateRow" ) );
    menuItemTfList.Add( transform.Find( "WeaponRow" ) );
  }

  // Use this for initialization
  void Start () {
    setupBase( 7, false, true );
  }
	
	// Update is called once per frame
	protected override void Update () {
    moveCursor();
    if (directionEnum != Direction.Zero) {
      if (directionEnum == Direction.Up || directionEnum == Direction.Down) {
        UpdateSelectedItem();
      }
      else if (directionEnum == Direction.Left || directionEnum == Direction.Right) {
        changeAdd();
        updateMoney();
      }
    }

    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      EffectSoundController.PLAY_BACK_CANCEL();

      if (movePowerAdd + hpAdd + enAdd + motilityAdd + armorAdd + hitRateAdd + weaponAdd <= 0) {
        Destroy( model );
        closeSelf();
      }
      else {
        movePowerAdd = hpAdd = enAdd = motilityAdd = armorAdd = hitRateAdd = weaponAdd = 0;
        movePowerMoney = hpMoney = enMoney = motilityMoney = armorMoney = hitRateMoney = weaponMoney = 0;
        updatePage();
      }
    }
    else if (Input.GetButtonDown( "Confirm" )) {
      confirm();
    }
    else if (Input.GetButtonDown( "CursorSpeed" ) || Input.GetButtonDown( "Info" )) {
      transform.parent.GetComponent<AbilityController2>().Setup( unit, () => {
        gameObject.SetActive( true );
      } );
      gameObject.SetActive( false );
    }
  }

  public void Setup( Action callback, MapFightingUnit unit ) {
    Setup( callback );
    this.unit = unit;
    try {
      preUnit = unit.Clone(); //MyHelper.DeepClone<MapFightingUnit>( unit );
    }
    catch (Exception e) {
      Debug.Log( e );
    }
    createModel();

    transform.Find( "RobotName/Label" ).GetComponent<Text>().text = unit.RobotInfo.RobotInstance.Robot.FullName;
    UpdateSelectedItem();
    updatePage();
  }

  private GameObject model;
  private void createModel() {
    GameObject prefab = Resources.Load( "Battle/Units/" + unit.RobotInfo.RobotInstance.Robot.Name ) as GameObject;
    //model = Instantiate( prefab, new Vector3( 0, 128, 0 ), prefab.transform.rotation );
    model = Instantiate( prefab, transform );
    //model.transform.SetParent( transform );
    model.transform.localPosition = new Vector3( 0, 128, 0 );
    model.transform.localRotation = prefab.transform.localRotation;
    model.AddComponent<Rotating>();
  }

  private void changeAdd() {
    int add = directionEnum == Direction.Right ? 1 : -1;
    switch (row) {
      case 1: changeMove( add ); break;
      case 2: changeHP( add ); break;
      case 3: changeEN( add ); break;
      case 4: changeMotility( add ); break;
      case 5: changeArmor( add );  break;
      case 6: changeHitRate( add );  break;
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
    int weaponLv = unit.WeaponList[0].WeaponInstance.Level;
    if (weaponLv + weaponAdd >= 10 && add > 0)
      return;
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/menuMove" ), 3 );
    weaponAdd += add;
    //preUnit.RobotInfo.RobotInstance.HitLv = unit.RobotInfo.RobotInstance.HitLv + hitRateAdd;
    //preUnit.Update();
    updateValue( "WeaponRow", weaponLv, weaponLv + weaponAdd, weaponLv, weaponAdd );
    weaponMoney = calculateMoney( weaponLv, weaponAdd, 12000 );
  }

  private void updatePage() {
    updateUnit( preUnit );
    updateValue( "MovePowerRow", unit.MovePower, preUnit.MovePower, unit.RobotInfo.RobotInstance.MovePowerLv, movePowerAdd );
    updateValue( "HpRow", unit.RobotInfo.MaxHP, preUnit.RobotInfo.MaxHP, unit.RobotInfo.RobotInstance.HPLv, hpAdd );
    updateValue( "EnRow", unit.RobotInfo.MaxEN, preUnit.RobotInfo.MaxEN, unit.RobotInfo.RobotInstance.ENLv, enAdd );
    updateValue( "MotilityRow", unit.RobotInfo.Motility, preUnit.RobotInfo.Motility, unit.RobotInfo.RobotInstance.MotilityLv, motilityAdd );
    updateValue( "ArmorRow", unit.RobotInfo.Armor, preUnit.RobotInfo.Armor, unit.RobotInfo.RobotInstance.ArmorLv, armorAdd );
    updateValue( "HitRateRow", unit.RobotInfo.HitRate, preUnit.RobotInfo.HitRate, unit.RobotInfo.RobotInstance.HitLv, hitRateAdd );
    int weaponLv = unit.WeaponList[0].WeaponInstance.Level;
    updateValue( "WeaponRow", weaponLv, weaponLv + weaponAdd, weaponLv, weaponAdd );
    updateMoney();
  }

  private void updateUnit( MapFightingUnit targetUnit ) {
    //必須兩者相加
    targetUnit.RobotInfo.RobotInstance.MovePowerLv = unit.RobotInfo.RobotInstance.MovePowerLv + movePowerAdd;
    targetUnit.RobotInfo.RobotInstance.HPLv = unit.RobotInfo.RobotInstance.HPLv + hpAdd;
    targetUnit.RobotInfo.RobotInstance.ENLv = unit.RobotInfo.RobotInstance.ENLv + enAdd;
    targetUnit.RobotInfo.RobotInstance.MotilityLv = unit.RobotInfo.RobotInstance.MotilityLv + motilityAdd;
    targetUnit.RobotInfo.RobotInstance.ArmorLv = unit.RobotInfo.RobotInstance.ArmorLv + armorAdd;
    targetUnit.RobotInfo.RobotInstance.HitLv = unit.RobotInfo.RobotInstance.HitLv + hitRateAdd;

    targetUnit.WeaponList.ForEach( w => {
      w.WeaponInstance.Level = w.WeaponInstance.Level + weaponAdd;
    } );

    targetUnit.UpdateInit();
  }

  private void updateValue( string rowName, float oldVal, float newVal, int lv, int lvAdd ) {
    var rowTf = transform.Find( rowName );
    rowTf.Find( "Old" ).GetComponent<Text>().text = oldVal.ToString();
    rowTf.Find( "New" ).GetComponent<Text>().text = newVal.ToString();
    rowTf.Find( "ImproveBarGo" ).Find( "ImproveBar2" ).GetComponent<ImproveBarScript>().SetLevel( lv );
    rowTf.Find( "ImproveBarGo" ).Find( "ImproveBar2" ).GetComponent<ImproveBarScript>().SetPreLevel( lvAdd );
  }

  private void updateValue( string rowName, int oldVal, int newVal, int lv, int lvAdd ) {
    var rowTf = transform.Find( rowName );
    rowTf.Find( "Old" ).GetComponent<Text>().text = oldVal.ToString();
    rowTf.Find( "New" ).GetComponent<Text>().text = newVal.ToString();
    rowTf.Find( "ImproveBarGo" ).Find( "ImproveBar2" ).GetComponent<ImproveBarScript>().SetLevel( lv );
    rowTf.Find( "ImproveBarGo" ).Find( "ImproveBar2" ).GetComponent<ImproveBarScript>().SetPreLevel( lvAdd );
  }

  private int calculateMoney( int baseLv, int addLv, int baseMoney = 2000 ) {
    if (addLv <= 0) return 0;

    int minLv = baseLv + 1;
    int maxLv = baseLv + addLv;
    float averageLv = (minLv + maxLv)/2f;
    return (int)(averageLv * baseMoney * addLv);
  }

  private long moneyBalance;
  private void updateMoney() {
    needMoney = movePowerMoney + hpMoney + enMoney + motilityMoney + armorMoney + hitRateMoney + weaponMoney;
    moneyBalance = DIContainer.Instance.GameDataService.GameData.Money - needMoney;

    transform.Find( "MoneyShow/CurrentMoney" ).GetComponent<Text>().text = gameDataService.GameData.Money.ToString();
    transform.Find( "MoneyShow/NeedMoney" ).GetComponent<Text>().text = needMoney.ToString();
    var newMoneyTextCom = transform.Find( "MoneyShow/NewMoney" ).GetComponent<Text>();
    newMoneyTextCom.text = moneyBalance.ToString();

    newMoneyTextCom.color = moneyBalance < 0? Color.red : Color.white;
  }

  protected override void confirm() {
    if (movePowerAdd + hpAdd + enAdd + motilityAdd + armorAdd + hitRateAdd + weaponAdd <= 0) return;

    if (moneyBalance < 0) {
      EffectSoundController.PLAY_ACTION_FAIL();
      return;
    }

    updateUnit( unit );
    gameDataService.UseMoney( needMoney );

    movePowerAdd = hpAdd = enAdd = motilityAdd = armorAdd = hitRateAdd = weaponAdd = 0;
    movePowerMoney = hpMoney = enMoney = motilityMoney = armorMoney = hitRateMoney = weaponMoney = 0;
    updatePage();

    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/SpCom/Sp_2" ) );
  }

}


