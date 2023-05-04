using UnityEngine;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityORM;

[Serializable]
public class MapFightingUnit {

  //public enum TeamEnum { Enermy = 0, Player, NPC_Friend, NPC_Yellow }

  //public int Team;  // 0 敵方(紅)  1 味方(藍)  2 味方 NPC(紫)  3 中立 (黃)

  //public TeamEnum Team { set; get; }

  public float MovePower;

  public int ActionCount = 1;

  public RobotInfo RobotInfo = null;

  public PilotInfo PilotInfo = null;

  public List<WeaponInfo> WeaponList = new List<WeaponInfo>();

  //public void SetupParts( PartsInstance partsInstance, int order ) {
  //DIContainer.Instance.RobotService.SetupParts( RobotInfo.RobotInstance, partsInstance, order );
  //}

  public void Update() {
    PilotInfo?.Update();

    if (RobotInfo != null) {     
      RobotInfo.Update();
      UpdateWeapons();
      UpdateInfoByRobotSkills();
      UpdateInfoByParts();
      MovePower = RobotInfo.MovePower;
    }

    if (RobotInfo != null && PilotInfo != null) {
      UpdateInfoByPilotSkills();
      UpdateInfoByActiveCommands();
    }
  }

  public void UpdateInit() {
    Update();
    RobotInfo?.InitValue();  //設置 HP 和 EN
    WeaponList.ForEach( w => w.RemainBullets = w.MaxBullets );
    //PilotInfo?.InitValue();  //設置 SP, 氣力

    if (PilotInfo != null) {
      PilotInfo.InitValue();  //設置 SP, 氣力
      var skill_16 = PilotInfo.PilotInstance.PilotSkillInstanceList.FirstOrDefault( psi => psi.PilotSkillID == 16 ); //16 : 鬥爭心

      if (skill_16 != null) {
        Type pilotSkillType = Type.GetType( "PilotSkill_" + 16 );
        PilotSkillBase psiBase = Activator.CreateInstance( pilotSkillType ) as PilotSkillBase;
        if (psiBase.IsEnabled( this, skill_16.OrderSort ))
          PilotInfo.Willpower += 8;
      }
    }
  }

  public void UpdateWeapons() {
    foreach (var weaponInstance in RobotInfo.RobotInstance.WeaponInstanceList) {
      var exist = WeaponList.FirstOrDefault( w => w.WeaponInstance.WeaponID == weaponInstance.WeaponID );
      if (exist == null)
        WeaponList.Add( new WeaponInfo( RobotInfo, PilotInfo, weaponInstance ) );
      else
        exist.Update();
    }
  }

  public void UpdateInfoByRobotSkills() {
    if (RobotInfo.RobotInstance.RobotSkill1?.IsBuffUnit == true) updateInfoByRobotSkill( RobotInfo.RobotInstance.RobotSkill1 );
    if (RobotInfo.RobotInstance.RobotSkill2?.IsBuffUnit == true) updateInfoByRobotSkill( RobotInfo.RobotInstance.RobotSkill2 );
    if (RobotInfo.RobotInstance.RobotSkill3?.IsBuffUnit == true) updateInfoByRobotSkill( RobotInfo.RobotInstance.RobotSkill3 );
    if (RobotInfo.RobotInstance.RobotSkill4?.IsBuffUnit == true) updateInfoByRobotSkill( RobotInfo.RobotInstance.RobotSkill4 );
    if (RobotInfo.RobotInstance.RobotSkill5?.IsBuffUnit == true) updateInfoByRobotSkill( RobotInfo.RobotInstance.RobotSkill5 );
    if (RobotInfo.RobotInstance.RobotSkill6?.IsBuffUnit == true) updateInfoByRobotSkill( RobotInfo.RobotInstance.RobotSkill6 );
  }

  private void updateInfoByRobotSkill( RobotSkill rbs ) {
    Type partsType = Type.GetType( "RobotSkill_" + rbs.ID );
    (Activator.CreateInstance( partsType ) as IBuffUnit)?.BuffUnit( this );
  }

  public void UpdateInfoByParts() {
    foreach (PartsInstance parts in RobotInfo.RobotInstance.PartsInstanceList) {
      if (parts != null) {
        Type partsType = Type.GetType( "Parts_" + parts.PartsID );
        (Activator.CreateInstance( partsType ) as IBuffUnit)?.BuffUnit( this );
      }
    }
  }

  public void UpdateInfoByPilotSkills() {
    foreach (PilotSkillInstance psi in PilotInfo.PilotInstance.PilotSkillInstanceList) {
      if (psi != null && psi.PilotSkill.IsBuffUnit) {
        Type psType = Type.GetType( "PilotSkill_" + psi.PilotSkillID );
        (Activator.CreateInstance( psType ) as IBuffUnitByPilotSkill)?.BuffUnit( this, psi.OrderSort );
      }
    }
  }

  public void UpdateInfoByActiveCommands() {
    foreach (SPCommand spc in PilotInfo.ActiveSPCommandList.Where( s => s.IsBuff )) {
      Type psType = Type.GetType( "SPCom_" + spc.ID );
      if (psType == null) {
        Debug.Log( $"Type SPCom_{spc.ID} not exist." );
        continue;
      }
      var spBuff = Activator.CreateInstance( psType ) as IBuffUnit;
      if (spBuff != null)
        spBuff.BuffUnit( this );
      else
        Debug.Log( $"Type SPCom_{spc.ID} is not implemented IBuffUnit interface." );    
    }
  }

  public ItemOnOff GetPartsItem( int order ) {
    if (order > RobotInfo.PartsSlot) return null;

    Parts parts = RobotInfo.RobotInstance.PartsInstanceList[order - 1]?.Parts;
    if (parts == null) return null;

    return new ItemOnOff() { Highlight = false, Name = parts.Name, Desc = parts.Desc };
  }

  public bool ActiveAvatar( AttackData atkData ) {
    if (RobotInfo.RobotInstance.RobotSkill1?.IsAvatar == true)      return AvatarByRobotSkill( RobotInfo.RobotInstance.RobotSkill1, atkData );
    else if (RobotInfo.RobotInstance.RobotSkill2?.IsAvatar == true) return AvatarByRobotSkill( RobotInfo.RobotInstance.RobotSkill2, atkData );
    else if (RobotInfo.RobotInstance.RobotSkill3?.IsAvatar == true) return AvatarByRobotSkill( RobotInfo.RobotInstance.RobotSkill3, atkData );
    else if (RobotInfo.RobotInstance.RobotSkill4?.IsAvatar == true) return AvatarByRobotSkill( RobotInfo.RobotInstance.RobotSkill4, atkData );
    else if (RobotInfo.RobotInstance.RobotSkill5?.IsAvatar == true) return AvatarByRobotSkill( RobotInfo.RobotInstance.RobotSkill5, atkData );
    else if (RobotInfo.RobotInstance.RobotSkill6?.IsAvatar == true) return AvatarByRobotSkill( RobotInfo.RobotInstance.RobotSkill6, atkData );
    else return false;
  }

  public bool AvatarByRobotSkill( RobotSkill rbSkill, AttackData atkData ) {
    Type rbSkillType = Type.GetType( "RobotSkill_" + rbSkill.ID );
    AvatarBase avatarBase = Activator.CreateInstance( rbSkillType ) as AvatarBase;
    return avatarBase.Effect( atkData );
  }

  public bool ActiveCutter( AttackData atkData ) {
    var playSkillInstance = PilotInfo.PilotInstance.PilotSkillInstanceList.FirstOrDefault( s => s.PilotSkillID == 2 );
    if (!RobotInfo.RobotInstance.Robot.Cutter || playSkillInstance == null || atkData.WeaponInfo.WeaponInstance.Weapon.CutType < 1 ) return false;

    PilotSkill_2 pilotSkill_2 = new PilotSkill_2();
    if (PilotSkillBase.GetSumLv( atkData.ToUnitInfo.MapFightingUnit.PilotInfo, playSkillInstance.OrderSort ) < 1) return false;

    float chance = pilotSkill_2.GetChance( atkData.FromUnitInfo.MapFightingUnit, atkData.ToUnitInfo.MapFightingUnit, playSkillInstance.OrderSort );
    int chanceInt = (int)(chance * 100);

    bool result = chanceInt > UnityEngine.Random.Range( 0, 100 );
    if (result) {
      atkData.CutType = atkData.WeaponInfo.WeaponInstance.Weapon.CutType;
      atkData.ActiveSkillList.Add( "切り払い" );
    }
    return result;
  }

  public bool ActiveShield( AttackData atkData ) {
    var playSkillInstance = PilotInfo.PilotInstance.PilotSkillInstanceList.FirstOrDefault( s => s.PilotSkillID == 1 );
    if (!RobotInfo.RobotInstance.Robot.Shield || playSkillInstance == null) return false;

    PilotSkill_1 pilotSkill_1 = new PilotSkill_1();
    if (PilotSkillBase.GetSumLv( atkData.ToUnitInfo.MapFightingUnit.PilotInfo, playSkillInstance.OrderSort ) < 1) return false;

    float chance = pilotSkill_1.GetChance( atkData.FromUnitInfo.MapFightingUnit, atkData.ToUnitInfo.MapFightingUnit, playSkillInstance.OrderSort );
    int chanceInt = (int)(chance * 100);

    bool result = chanceInt > UnityEngine.Random.Range( 0, 100 );
    atkData.IsShield = result;
    if (result) {
      atkData.ActiveSkillList.Add( "盾防禦" );
      atkData.ExpectedDamage = atkData.ExpectedDamage / 2;
      //atkData.AttackType = AttackData.AttackTypeEnum.Normal;
      //atkData.ActiveSkillList.Add( pilotSkill_1.GetNameWithLv( atkData.ToUnitInfo.MapFightingUnit, playSkillInstance.Order ) );
    }

    return result;
  }

  public float ActiveBarrier( AttackData atkData ) {
    IBarrier barrier = FindBestBarrier( atkData );
    if (barrier == null) return atkData.ExpectedDamage;

    return barrier.Effect( atkData );
  }

  public IBarrier FindBestBarrier( AttackData atkData ) {
    IBarrier barrier = null;

    if (RobotInfo.RobotInstance.RobotSkill1?.IsBarrier == true) barrier = Activator.CreateInstance( Type.GetType( "RobotSkill_" + RobotInfo.RobotInstance.Skill1ID ) ) as IBarrier;

    if (RobotInfo.RobotInstance.RobotSkill2?.IsBarrier == true) {
      IBarrier b2 = Activator.CreateInstance( Type.GetType( "RobotSkill_" + RobotInfo.RobotInstance.Skill2ID ) ) as IBarrier;
      if (b2 != null && b2.CanActive( atkData ) && (barrier == null || b2.Amount > barrier.Amount)) barrier = b2;
    }

    if (RobotInfo.RobotInstance.RobotSkill3?.IsBarrier == true) {
      IBarrier b3 = Activator.CreateInstance( Type.GetType( "RobotSkill_" + RobotInfo.RobotInstance.Skill3ID ) ) as IBarrier;
      if (b3 != null && b3.CanActive( atkData ) && (barrier == null || b3.Amount > barrier.Amount)) barrier = b3;
    }

    if (RobotInfo.RobotInstance.RobotSkill4?.IsBarrier == true) {
      IBarrier b4 = Activator.CreateInstance( Type.GetType( "RobotSkill_" + RobotInfo.RobotInstance.Skill4ID ) ) as IBarrier;
      if (b4 != null && b4.CanActive( atkData ) && (barrier == null || b4.Amount > barrier.Amount)) barrier = b4;
    }

    if (RobotInfo.RobotInstance.RobotSkill5?.IsBarrier == true) {
      IBarrier b5 = Activator.CreateInstance( Type.GetType( "RobotSkill_" + RobotInfo.RobotInstance.Skill5ID ) ) as IBarrier;
      if (b5 != null && b5.CanActive( atkData ) && (barrier == null || b5.Amount > barrier.Amount)) barrier = b5;
    }

    if (RobotInfo.RobotInstance.RobotSkill6?.IsBarrier == true) {
      IBarrier b6 = Activator.CreateInstance( Type.GetType( "RobotSkill_" + RobotInfo.RobotInstance.Skill6ID ) ) as IBarrier;
      if (b6 != null && b6.CanActive( atkData ) && (barrier == null || b6.Amount > barrier.Amount)) barrier = b6;
    }

    //var partsList = RobotInfo.RobotInstance.PartsInstanceList?.Where( p => p?.Parts?.Type == Parts.PartsTypeEnum.Barrier ).ToList();
    var partsList = RobotInfo.RobotInstance.PartsInstanceList?.Where( p => p?.Parts?.IsBarrier == true ).ToList();
    foreach (var b in partsList) {
      IBarrier parts = Activator.CreateInstance( Type.GetType( "Parts_" + b.PartsID ) ) as IBarrier;
      if (parts != null && parts.CanActive( atkData ) && (barrier == null || parts.Amount > barrier.Amount)) barrier = parts;
    }

    return barrier;
  }

  public void ChangePilot( MapFightingUnit otherUnit ) {
    PilotInfo tempOther = otherUnit.PilotInfo;
    otherUnit.PilotInfo = this.PilotInfo;
    PilotInfo = tempOther;

    if (PilotInfo != null) {
      PilotInfo.PilotInstance.RobotInstanceSeqNo = RobotInfo?.RobotInstance.SeqNo;
      UpdateInit();
    }

    if (otherUnit.PilotInfo != null) {
      otherUnit.PilotInfo.PilotInstance.RobotInstanceSeqNo = otherUnit.RobotInfo?.RobotInstance.SeqNo;
      otherUnit.UpdateInit();
    }

  }


  public void SetAIBuff( int AI_HP = 0, int AI_EN = 0, int AI_Motility = 0, int AI_Armor = 0, int AI_HitRate = 0, int AI_Range = 0, int willPower = 0, int hitPower = 0 ) {
    RobotInfo.AI_HP = AI_HP;
    RobotInfo.AI_EN = AI_EN;
    RobotInfo.AI_Motility = AI_Motility;
    RobotInfo.AI_Armor = AI_Armor;
    RobotInfo.AI_HitRate = AI_HitRate;
    RobotInfo.AI_Range = AI_Range;
    RobotInfo.AI_HitPower = hitPower;

    PilotInfo.BuffWillPower = willPower;

    UpdateInit();
  }

  public bool IsFightable => RobotInfo != null && PilotInfo != null;

}
