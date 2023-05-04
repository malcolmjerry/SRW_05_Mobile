using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

[Serializable]
public class WeaponInfo {

  public WeaponInfo() {
  }

  public WeaponInfo( RobotInfo rbInfo, PilotInfo pilotInfo, WeaponInstance weaponInstance ) {
    this.RobotInfo = rbInfo;
    this.PilotInfo = pilotInfo;
    this.WeaponInstance = weaponInstance;
    Update();
    RemainBullets = MaxBullets;
  }

  //public int WeaponID { set; get; }   //WeaponInstance ID

  public RobotInfo RobotInfo { set; get; }

  public PilotInfo PilotInfo { set; get; }

  public WeaponInstance WeaponInstance { set; get; }

  //public int PlayIndex { set; get; }

  //public string Name { set; get; }

  //public bool IsMelee { set; get; }  //if false = shooting

  public int HitPoint { set; get; }

  public int HitRate { set; get; }

  public int CRI { set; get; }

  public int MinRange { set; get; }

  public int MaxRange { set; get; }

  public int EN { set; get; }

  public int MaxBullets { set; get; }

  public int RemainBullets { set; get; }

  public int WillPower { set; get; }

  public bool IsMove;

  public int TerrainSky;

  public int TerrainLand;

  public int TerrainSea;

  public int TerrainSpace;

  public WeaponInfo Update() {    
    initHitPoint();
    initHitRate();
    initMinRage();
    initMaxRage();
    initEN();
    initMaxBullets();
    initWillPower();
    initIsMove();
    initTerrain();
    initCRI();
    return this;
  }

  public bool IsUsable { 
    get {
      if (EN > RobotInfo.EN) return false;
      if (MaxBullets > 0 && RemainBullets <= 0) return false;
      if (PilotInfo != null && WillPower > PilotInfo.Willpower) return false;
      if (PilotInfo != null && WeaponInstance.Weapon.IsNT > 0) {
        var psi = PilotInfo.PilotInstance.PilotSkillInstanceList.FirstOrDefault( ps => ps.PilotSkillID >= 10 && ps.PilotSkillID <= 12 );
        //PilotSkillBase psiBase = new PilotSkillBase();
        int sumLv = PilotSkillBase.GetSumLv( PilotInfo, psi.OrderSort );
        if (sumLv < WeaponInstance.Weapon.IsNT) return false;
      }
      return true;
    }
  }

  public string TypeStr() {
    StringBuilder sb = new StringBuilder();

    if (IsMove) sb.Append( "P" );
    if (WeaponInstance.Weapon.IsBeam) sb.Append( "|B" );
    if (WeaponInstance.Weapon.IsEnergy) sb.Append( "|能" );
    if (WeaponInstance.Weapon.IsPhysical) sb.Append( "|物" );
    if (WeaponInstance.Weapon.CutType > 0) sb.Append( "|切" );

    var resultStr = sb.ToString();
    if (resultStr.StartsWith( "|" ))
      resultStr = resultStr.Substring( 1 );

    return resultStr;
  }

  private void initHitPoint() {
    int source = WeaponInstance.Weapon.HitPoint;
    int improved = source + WeaponInstance.UpgradHitPoint();
    HitPoint = improved + RobotInfo.AI_HitPower;
  }

  private void initHitRate() {
    HitRate = WeaponInstance.Weapon.HitRate;
    //if (WeaponInstance.RobotInstance.WeaponBonus == 6) {
    if (RobotInfo.RobotInstance.WeaponBonus == 6) {
      HitRate += 40;
    }
  }

  private void initMinRage() {
    int source = WeaponInstance.Weapon.MinRange;

    if (RobotInfo.RobotInstance.WeaponBonus == 1) {
      source = Mathf.Max( 1, source - 1 );
    }
    MinRange = source;
  }

  private void initMaxRage() {
    MaxRange = WeaponInstance.Weapon.MaxRange + RobotInfo.AI_Range;

    if (RobotInfo.RobotInstance.WeaponBonus == 1) {
      MaxRange++;
    }
  }

  private void initEN() {
    EN = WeaponInstance.Weapon.EN;

    float bonusRate = 1f;

    if (RobotInfo.RobotInstance.WeaponBonus == 3) {
      //EN = (int)(EN * 0.8f);
      bonusRate -= 0.2f;
    }

    if (PilotInfo != null && PilotSkillBase.IsEnabledBySkillID( PilotInfo, 18 )) {  //18: E-Save
      bonusRate -= 0.2f;
    }

    EN = (int)(EN * bonusRate);
  }

  private void initMaxBullets() {
    MaxBullets = WeaponInstance.Weapon.Bullets;

    float bonusRate = 1f;

    if (RobotInfo.RobotInstance.WeaponBonus == 4) {
      bonusRate += 0.5f;
    }

    if (PilotInfo != null && PilotSkillBase.IsEnabledBySkillID( PilotInfo, 23 )) {  //23: B-Save
      bonusRate += 0.5f;
    }

    MaxBullets = Mathf.CeilToInt(MaxBullets * bonusRate);
  }

  private void initWillPower() {
    WillPower = WeaponInstance.Weapon.WillPower;

    if (RobotInfo.RobotInstance.WeaponBonus == 8) {
      WillPower = -1;
    }
  }

  private void initIsMove() {
    IsMove = WeaponInstance.Weapon.IsMove;
    if (RobotInfo.RobotInstance.WeaponBonus == 2) {
      IsMove = true;
    }
  }

  private void initTerrain() {
    TerrainSky = WeaponInstance.Weapon.TerrainSky;
    TerrainLand = WeaponInstance.Weapon.TerrainLand;
    TerrainSea = WeaponInstance.Weapon.TerrainSea;
    TerrainSpace = WeaponInstance.Weapon.TerrainSpace;
    if (RobotInfo.RobotInstance.WeaponBonus == 5) {
      TerrainSky = Mathf.Max( TerrainSky, 4 );       //4=A
      TerrainLand = Mathf.Max( TerrainLand, 4 );     //4=A
      TerrainSea = Mathf.Max( TerrainSea, 4 );       //4=A
      TerrainSpace = Mathf.Max( TerrainSpace, 4 );   //4=A
    }
  }

  private void initCRI() {
    CRI = WeaponInstance.Weapon.CRI;
    if (RobotInfo.RobotInstance.WeaponBonus == 7) {
      CRI += 25;
    }
  }
}
