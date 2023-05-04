using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

[Serializable]
public class RobotInfo {

  //public int RobotID { set; get; }  //RobotInstance ID
  public RobotInstance RobotInstance { set; get; }

  public int MaxHP { get; set; }

  private int _HP;
  public int HP { get { return _HP; } 
    set {
      _HP = Math.Min( value, MaxHP );
  } }

  public int MaxEN { set; get; }

  private int _EN;
  public int EN {
    get { return _EN; }
    set {
      _EN = Math.Min( value, MaxEN );
    }
  }


  public float MovePower { set; get; }

  public float Radius { set; get; }
  //public static readonly float RADIUS = 1.6f;

  public int Motility { set; get; }

  public int HitRate { set; get; }

  public int Armor { set; get; }

  //public string Terrain { set; get; }

  public int TerrainSky { set; get; }

  public int TerrainLand { set; get; }

  public int TerrainSea { set; get; }

  public int TerrainSpace { set; get; }

  public int MoveSky { set; get; }

  public int MoveLand { set; get; }

  public int MoveSea { set; get; }

  public int MoveSpace { set; get; }

  public int PartsSlot { set; get; }

  //public string BGM { set; get; }

  //public int Size { set; get; }

  //public int Team { set; get; }   // 0 敵方(紅)  1 味方(藍)  2 味方 NPC(紫)  3 中立 (黃)

  public bool IsMoved { set; get; } = false;

  //public List<WeaponInfo> WeaponList;
  //public List<RobotSkill> RobotSkillList;

  public string GetMoveTypeStr() {
    StringBuilder sb = new StringBuilder();
    sb.Append( MoveSky > 0?  "空 " : "— " );
    sb.Append( MoveLand > 0? "陸 " : "— " );
    sb.Append( MoveSea > 0? "海 ": "— " );
    sb.Append( MoveSpace > 0? "宇 ": "— " );
    return sb.ToString();
  }

  /*
  public string[] GetTerrainStr() {
    string[] terrainArr = new string[4];
    terrainArr[0] = terrainLvStr[TerrainSky];
    terrainArr[1] = terrainLvStr[TerrainLand];
    terrainArr[2] = terrainLvStr[TerrainSea];
    terrainArr[3] = terrainLvStr[TerrainSpace];
    return terrainArr;
  }*/

  public string GetSizeStr() {
    switch (RobotInstance.Robot.Size) {
      case 1: return "SS";
      case 2: return "S";
      case 3: return "M";
      case 4: return "L";
      case 5: return "LL";
      case 6: return "3L";
      default: return "--";
    }
  }

  public RobotInfo() { }

  /*
  public RobotInfo( RobotInstance robotInstance ) {
    RobotInstance = robotInstance;
    Update();
    InitValue();
  }*/

  public RobotInfo( RobotInstance robotInstance ) {
    RobotInstance = robotInstance;
    Update();
  }


  public void NewPhase() {
    if (--DeBuffArmorPhase == 0) DeBuffArmor = 0;
    if (--DeBuffMotilityPhase == 0) DeBuffMotility = 0;
    if (--DeBuffHitRatePhase == 0) DeBuffHitRate = 0;

  }

  public void Update() {
    initMaxHP();
    initMaxEN();
    initMotility();
    initArmor();
    initHitRate();
    initMovePower();
    initTerrain();
    initMoveTerrainType();
    Radius = RobotInstance.Robot.Radius;
    PartsSlot = RobotInstance.Robot.PartsSlot;

    if (RobotInstance.Bonus == 8) PartsSlot++;

    //InitWeapons();
  }

  /*
  public void InitWeapons() {
    WeaponList = new List<WeaponInfo>();
    foreach (var weaponInstance in RobotInstance.WeaponInstanceList) {
      WeaponList.Add( weaponInstance.CloneInfo( this ) );
    }
  }
  */

  private void initMaxHP() {
    //MaxHP = AI_HP!=0? AI_HP : GenMaxHP( RobotInstance.HPLv );
    MaxHP = GenMaxHP( RobotInstance.HPLv ) + AI_HP;
  }

  public int GenMaxHP( int lv ) {
    int sourceHP = RobotInstance.Robot.HP;
    //int improveHP = RobotInstance.Robot.HP / 20;
    //if (improveHP < 35) improveHP = 250;
    //else if (improveHP < 75) improveHP = 350;
    //else improveHP = 500;

    int improveHP = 350;
    int improvedHP = sourceHP + lv * improveHP;

    //改造滿級之後選擇第一種獎勵
    if (RobotInstance.Bonus == 1) {
      //improvedHP += (int)(improvedHP * 0.3f);
      improvedHP += 3000;
    }
   return improvedHP;
  }

  private void initMaxEN() {
    //MaxEN = AI_EN!=0? AI_EN : GenMaxEN( RobotInstance.ENLv ); 
    MaxEN = GenMaxEN( RobotInstance.ENLv ) + AI_EN;
  }

  public int GenMaxEN( int lv ) {
    int sourceEN = RobotInstance.Robot.EN;
    //int improveEN = RobotInstance.Robot.EN / 20;
    //if (improveEN < 10) improveEN = 12;
    //else if (improveEN < 16) improveEN = 13;
    //else if (improveEN < 21) improveEN = 14;
    //else improveEN = 15;
    int improveEN = 12;
    int improvedEN = sourceEN + lv * improveEN;

    if (RobotInstance.Bonus == 2) {
      //improvedEN += (int)(improvedEN * 0.3f);
      improvedEN += 100;
    }
    return improvedEN;
  }

  private void initMotility() {
    //Motility = AI_Motility!=0? AI_Motility : GenMotility( RobotInstance.MotilityLv );
    Motility = GenMotility( RobotInstance.MotilityLv ) + AI_Motility;
  }

  public int GenMotility( int lv ) {
    int motility = RobotInstance.Robot.Motility;
    //int improveMotility = RobotInstance.Robot.Motility / 20;
    //if (improveMotility < 7) improveMotility = 5;
    //else improveMotility = 6;
    int improveMotility = 6;
    int improvedMotility = motility + lv * improveMotility;

    if (RobotInstance.Bonus == 3) {
      //improvedMotility += (int)(improvedMotility * 0.15f);
      improvedMotility += 20;
    }
    return improvedMotility - DeBuffMotility;
  }

  private void initArmor() {    
    //Armor = AI_Armor!=0? AI_Armor : GenArmor( RobotInstance.ArmorLv );
    Armor = GenArmor( RobotInstance.ArmorLv ) + AI_Armor;
  }

  public int GenArmor( int lv ) {
    int armor = RobotInstance.Robot.Armor;
    /*
    int improveArmor = RobotInstance.Robot.Armor / 20;
    improveArmor = improveArmor <= 50 ? 50 : improveArmor;
    improveArmor = improveArmor >= 80 ? 80 : improveArmor;
    */
    int improveArmor = 75;
    int improvedArmor = armor + lv * improveArmor;

    if (RobotInstance.Bonus == 4) {
      //improvedArmor = (int)(improvedArmor * 1.15f);
      improvedArmor += 300;
    }
    return improvedArmor - DeBuffArmor;

  }

  private void initHitRate() {
    //HitRate = AI_HitRate!=0? AI_HitRate : GenHitRate( RobotInstance.HitLv );
    HitRate = GenHitRate( RobotInstance.HitLv ) + AI_HitRate;
  }

  public int GenHitRate( int lv ) {
    int hitRate = RobotInstance.Robot.HitRate;
    //int improveHitRate = RobotInstance.Robot.HitRate / 20;
    //if (improveHitRate < 5) improveHitRate = 4;
    //else improveHitRate = 5;

    int improveHitRate = 7;
    int improvedHitRate = hitRate + lv * improveHitRate;

    if (RobotInstance.Bonus == 5) {
      //improvedHitRate += (int)(improvedHitRate * 0.3f);
      improvedHitRate += 40;
    }
    return improvedHitRate - DeBuffHitRate;
  }

  private void initMovePower() {
    MovePower = GenMovePower( RobotInstance.MovePowerLv );
  }

  public float GenMovePower( int lv ) {
    float movePower = RobotInstance.Robot.MovePower;
    movePower += lv * 0.2f;

    if (RobotInstance.Bonus == 6) {
      movePower += 2;
    }
    return movePower;
  }

  private void initTerrain() {
    TerrainSky = Mathf.Min( 5, RobotInstance.Robot.TerrainSky + RobotInstance.TerrainSkyLv );
    TerrainLand = Mathf.Min( 5, RobotInstance.Robot.TerrainLand + RobotInstance.TerrainLandLv );
    TerrainSea = Mathf.Min( 5, RobotInstance.Robot.TerrainSea + RobotInstance.TerrainSeaLv );
    TerrainSpace = Mathf.Min( 5, RobotInstance.Robot.TerrainSpace + RobotInstance.TerrainSpaceLv );
    if (RobotInstance.Bonus == 7) {
      TerrainSky = Mathf.Min( TerrainSky, 4 );       //4=A
      TerrainLand = Mathf.Min( TerrainLand, 4 );     //4=A
      TerrainSea = Mathf.Min( TerrainSea, 4 );       //4=A
      TerrainSpace = Mathf.Min( TerrainSpace, 4 );   //4=A
    }
  }

  private void initMoveTerrainType() {
    MoveSky = Mathf.Min( 1, RobotInstance.Robot.MoveSky + RobotInstance.MoveSky );
    MoveLand = Mathf.Min( 1, RobotInstance.Robot.MoveLand + RobotInstance.MoveLand );
    MoveSea = Mathf.Min( 1, RobotInstance.Robot.MoveSea + RobotInstance.MoveSea );
    MoveSpace = Mathf.Min( 1, RobotInstance.Robot.MoveSpace + RobotInstance.MoveSpace );
    if (RobotInstance.Bonus == 8) {
      MoveSky = 1;      
      MoveLand = 1;
      MoveSea = 1;
      MoveSpace = 1;  
    }
  }

  public void InitValue() {
    this.HP = MaxHP;
    this.EN = MaxEN;
  }

  public int DeBuffArmor { get; set; }           //裝甲值 DeBuff
  public int DeBuffArmorPhase { get; set; }      //裝甲值 DeBuff 維持回合

  public int DeBuffMotility { get; set; }        //運動性 DeBuff
  public int DeBuffMotilityPhase { get; set; }

  public int DeBuffHitRate { get; set; }         //命中值 DeBuff
  public int DeBuffHitRatePhase { get; set; }

  //AI only buff
  public int AI_HP { get; set; }
  public int AI_EN { get; set; }
  public int AI_Motility { get; set; }
  public int AI_Armor { get; set; }
  public int AI_HitRate { get; set; }

  public int AI_Range { get; set; }
  public int AI_HitPower { get; set; }
}
