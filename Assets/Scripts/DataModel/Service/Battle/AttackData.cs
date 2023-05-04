using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AttackData {
  //Pre Data
  public UnitInfo FromUnitInfo { set; get; }
  public WeaponInfo WeaponInfo { set; get; }
  public UnitInfo ToUnitInfo { set; get; }

  public int FromHP { set; get; }
  public int FromEN { set; get; }

  public int ToHP { set; get; }
  public int ToEN { set; get; }
  /*
  public string FromModelName { set; get; }
  public int FromMaxHP { set; get; }
  public int FromMaxEN { set; get; }
  public int FromWeaponIndex { set; get; }

  public string ToModelName { set; get; }
  public int ToMaxHP { set; get; }

  public int ToMaxEN { set; get; }

  */
  public int FromUseEN { set; get; }

  public bool FromRight { set; get; }
  public int HitRate { set; get; }
  public int CriRate { set; get; }
  public float ExpectedDamage { set; get; }

  public AttackTypeEnum AttackType { set; get; }
  public enum AttackTypeEnum { Normal, MapW, Support, Assistance, Unable, CounterSkill, Quit }

  public CounterTypeEnum CounterType { set; get; }
  public enum CounterTypeEnum { Normal, Defense, Dodge, Unable }

  //Pre Data

  //Pre Buff
  public int HitRateAdd { set; get; }
  public float HitRateMultiply { set; get; }   //攪亂, 回避
  public int CriAdd { set; get; }

  //public float FinalHitRateMultiply { set; get; }   //攪亂, 回避
  public int FinalHitRateAdd { set; get; }
  public int FinalCriAdd { set; get; }

  public float AvatarAdd { set; get; }    //分身 Buff
  public float CutAdd { set; get; }       //切拂 Buff
  public float ShieldAdd { set; get; }    //盾防 Buff

  //public float DamageRateAdd { set; get; }    // 爆擊 +0.25, 熱血 +1, 魂 +1.4
  public float DamageMultiply { set; get; }    //傷害乘算 Buff. 野生化 *1.1 , 力"一卜" * 0.8 , 防禦 0.5, 爆擊 1.25, 熱血 2.0, 魂 2.4

  public float MoneyRateAdd { set; get; }    //幸運, 強運
  public float ExpRateAdd { set; get; }      //努力 2.0
  public float PpRateAdd { set; get; }       //努力 1.2
  //Pre Buff

  //Result
  public int AvatarID { set; get; }       // 0 不生效  1 分身, God Shadow, 馬哈障眼法
  //public string AvatarName { set; get; }    

  public int CutType { set; get; }      // 0 無切拂  1 彈開  2 爆炸

  public bool IsDodge { set; get; }   //一般回避

  //public bool IsHit { set; get; }      // 真正命中

  public bool IsCritical { set; get; }  // 會心一擊

  public int BarrierID { set; get; }   // 0 表示沒有防護罩  2 Beam coat  3 I-Field  ? FF-Field  ? PS裝甲  ? 念動-Field  ? Pinpoint Barrier  ? 歪曲-Field  ? Beam吸收  ? Gravity Barrier
                                       // ?? AT Field  ?? Aura Barrier

  //public string BarrierName { set; get; }

  public bool IsShield { set; get; }   // 發動盾防

  public bool IsGuard { set; get; }   // 發動力"一卜"

  public bool IsDefeated { set; get; }
  
  public bool IsNoKill { set; get; }

  public int TotalDamage { set; get; }

  public List<string> ActiveSkillList = new List<string>();

  public List<PilotDialog> AtkDialogs { set; get; }
  public List<PilotDialog> DefDialogs { set; get; }

  //Result

  //After Effect
  public int ToUseEN { set; get; }

  public int ToBuffWillPower { set; get; }    //戰鬥後 影響 氣力值
  public int ToBuffArmor { set; get; }        //戰鬥後 影響 裝甲值
  public int ToBuffMotility { set; get; }     //戰鬥後 影響 運動性
  public int ToBuffSP { set; get; }           //戰鬥後 影響 SP值
  public int ToBuffHitRate { set; get; }      //戰鬥後 影響 命中值
  //After Effect

  /*
  public List<string> UnitBuffNameList {
    get {
      List<string> results = new List<string>();
      if (AvatarID > 0) results.Add( "分身" );
      if (IsCut) results.Add( "切拂" );
      if (IsCritical) results.Add( "Critical" );
      if (BarrierID > 0) results.Add( "XX防護罩" );
      if (IsShield) results.Add( "盾防" );
      if (IsGuard)  results.Add( "防守技巧" );

      return results;
    }
  }
  */

  public AttackData( UnitInfo fromUnitInfo, WeaponInfo weaponInfo, UnitInfo toUnitInfo, bool fromRight, AttackTypeEnum AttackType, CounterTypeEnum CounterType ) {
    this.FromUnitInfo = fromUnitInfo;
    this.WeaponInfo = weaponInfo;
    this.ToUnitInfo = toUnitInfo;

    this.FromRight = fromRight;
    this.AttackType = AttackType;
    this.CounterType = CounterType;

    if (AttackType == AttackTypeEnum.Unable || AttackType == AttackTypeEnum.Quit)
      return;

    fromUnitInfo.MapFightingUnit.Update();
    toUnitInfo.MapFightingUnit.Update();

    //Pre Data
    /*
    HitRate = 0;
    CriRate = 0;
    ExpectedDamage = 0f;
    FromUseEN = weaponInfo.EN;
    */
    /*
    FromModelName = fromUnitInfo.RobotInfo.RobotInstance.Robot.Name;
    FromMaxHP = fromUnitInfo.RobotInfo.MaxHP;
    FromHP =  fromUnitInfo.RobotInfo.HP;
    FromMaxEN = fromUnitInfo.RobotInfo.MaxEN;
    FromEN = fromUnitInfo.RobotInfo.EN;
    FromWeaponIndex = weaponInfo.WeaponInstance.Weapon.PlayIndex;

    ToModelName = toUnitInfo.RobotInfo.RobotInstance.Robot.Name;
    ToMaxHP = toUnitInfo.RobotInfo.MaxHP;
    ToHP =  toUnitInfo.RobotInfo.HP;
    ToMaxEN = toUnitInfo.RobotInfo.MaxEN;
    ToEN = toUnitInfo.RobotInfo.EN;
    */
    //Pre Data
    /*
    //Pre Buff
    HitRateAdd = 0;
    HitRateMultiply = 1f;
    CriAdd = 0;

    //FinalHitRateMultiply = 1;
    FinalHitRateAdd = 0;
    FinalCriAdd = 0;

    AvatarAdd = 0;
    CutAdd = 0;
    ShieldAdd = 0;
    //DamageRateAdd = 1f;
    DamageMultiply = 1f;

    MoneyRateAdd = 1f;
    ExpRateAdd = 1f;
    PpRateAdd = 1f;
    //Pre Buff

    //Result
    AvatarID = 0;
    //AvatarName = "";
    CutType = 0;
    IsDodge = false;
    //IsHit = false;
    IsCritical = false;
    BarrierID = 0;  // 0 表示沒有防護罩  1 Beam coat  2 I-Field  3 FF-Field  4 PS裝甲  5 念動-Field  6 Pinpoint Barrier  7 歪曲-Field  8 Beam吸收  9 Gravity Barrier
                    // 10 AT Field  11 Aura Barrier
    //BarrierName = "";
    IsShield = false;   // 發動盾防
    IsGuard = false;    // 發動力"一卜"
    IsDefeated = false;
    IsNoKill = false;
    TotalDamage = 0;
    //Result

    //After Effect
    ToUseEN = 0;
    ToBuffWillPower = 0;    //戰鬥後 影響 氣力值
    ToBuffArmor = 0;        //戰鬥後 影響 裝甲值
    ToBuffMotility = 0;     //戰鬥後 影響 運動性
    ToBuffSP = 0;           //戰鬥後 影響 SP值
    ToBuffHitRate = 0;      //戰鬥後 影響 命中值
    //After Effect
    */

    PreBuff();
    //PreData();
  }

  private void initBuff() {
    ActiveSkillList = new List<string>();

    //Pre Data
    HitRate = 0;
    CriRate = 0;
    ExpectedDamage = 0f;
    FromUseEN = WeaponInfo.EN;
    //Pre Data

    //Pre Buff
    HitRateAdd = 0;
    HitRateMultiply = 1f;
    CriAdd = 0;

    //FinalHitRateMultiply = 1;
    FinalHitRateAdd = 0;
    FinalCriAdd = 0;

    AvatarAdd = 0;
    CutAdd = 0;
    ShieldAdd = 0;
    //DamageRateAdd = 1f;
    DamageMultiply = 1f;

    MoneyRateAdd = 1f;
    ExpRateAdd = 1f;
    PpRateAdd = 1f;
    //Pre Buff

    //Result
    AvatarID = 0;
    //AvatarName = "";
    CutType = 0;
    IsDodge = false;
    //IsHit = false;
    IsCritical = false;
    BarrierID = 0;  // 0 表示沒有防護罩  1 Beam coat  2 I-Field  3 FF-Field  4 PS裝甲  5 念動-Field  6 Pinpoint Barrier  7 歪曲-Field  8 Beam吸收  9 Gravity Barrier
                    // 10 AT Field  11 Aura Barrier
                    //BarrierName = "";
    IsShield = false;   // 發動盾防
    IsGuard = false;    // 發動力"一卜"
    IsDefeated = false;
    IsNoKill = false;
    TotalDamage = 0;
    //Result

    //After Effect
    ToUseEN = 0;
    ToBuffWillPower = 0;    //戰鬥後 影響 氣力值
    ToBuffArmor = 0;        //戰鬥後 影響 裝甲值
    ToBuffMotility = 0;     //戰鬥後 影響 運動性
    ToBuffSP = 0;           //戰鬥後 影響 SP值
    ToBuffHitRate = 0;      //戰鬥後 影響 命中值
    //After Effect
  }

  public void PreBuff() {
    initBuff();

    //處理攻方駕駛員技能 + 機體能力 + 精神指令 + ACE Bonus
    RobotSkillBuffByAttacker();
    PilotSkillBuffByAttacker();
    SpComBuffByAttacker();

    //處理守方駕駛員技能 + 機體能力 + 精神指令 + ACE Bonus
    RobotSkillBuffByDefenser();
    PilotSkillBuffByDefenser();
    SpComBuffByDef();

    if (CounterType == CounterTypeEnum.Dodge) HitRateMultiply *= 0.5f;
    //else if (CounterType == CounterTypeEnum.Defense) DamageMultiply *= 0.5f;  //0.6f?  //留到計算總傷害及護罩減算後, 才計算防禦減半效果

    PreData();
  }

  private void PreData() {
    /*
    if (AttackType == AttackTypeEnum.Unable)
      return;
    PreBuff();
    */
    //計算命中率 & 會心一擊率 & 預期傷害
    HitRate = PreBattleFormula.HIT_RATE_FINAL( this );
    CriRate = PreBattleFormula.CRI_RATE_FINAL( this );
    ExpectedDamage = PreBattleFormula.DAMAGE_EXPECTED( FromUnitInfo, ToUnitInfo, WeaponInfo, DamageMultiply );
  }

  public void RunResult() {
    if (AttackType == AttackTypeEnum.Unable || AttackType == AttackTypeEnum.Quit)
      return;

    //if (FromUnitInfo.RobotInfo.EN < WeaponInfo.EN || (WeaponInfo.MaxBullets > 0 && WeaponInfo.RemainBullets == 0)) {
    if (!WeaponInfo.IsUsable) { 
      AttackType = AttackTypeEnum.Unable;
      return;
    }

    PilotSkillBuffByAttackerHide();
    PilotSkillBuffByDefenserHide();
    PreData();

    bool hitMiss = HitRate > UnityEngine.Random.Range( 0, 100 );
    IsCritical = CriRate > UnityEngine.Random.Range( 0, 100 );  

    if (ToUnitInfo.MapFightingUnit.ActiveAvatar( this )) return;

    if (ToUnitInfo.MapFightingUnit.ActiveCutter( this )) return;

    if (!hitMiss) {
      IsDodge = true;
      return;
    }

    if (IsCritical) {
      DamageMultiply *= 1.25f; /*DamageRateAdd += 0.25f;*/
      ActiveSkillList.Add( "Critical" );
    }

    ExpectedDamage = PreBattleFormula.DAMAGE_EXPECTED( FromUnitInfo, ToUnitInfo, WeaponInfo, DamageMultiply );

    if (ToUnitInfo.MapFightingUnit.ActiveBarrier( this ) <= 0) return;

    if (CounterType != CounterTypeEnum.Defense)
      ToUnitInfo.MapFightingUnit.ActiveShield( this );  //if (ToUnitInfo.MapFightingUnit.ActiveShield( this )) ExpectedDamage = ExpectedDamage / 2;
    //else if (CounterType == CounterTypeEnum.Defense)
    else {
      //DamageMultiply *= 0.5f;  //0.6f?
      ExpectedDamage = ExpectedDamage / 2;
    }

    TotalDamage = Mathf.CeilToInt( ExpectedDamage );
    if (TotalDamage > 0) TotalDamage = Mathf.Max( 10, TotalDamage );

    if (TotalDamage >= ToUnitInfo.RobotInfo.HP) {
      if (IsNoKill) TotalDamage = ToUnitInfo.RobotInfo.HP - 10;
      else IsDefeated = true;
    }
    
  }

  public void AfterData() {    //戰鬥結束後Data變化
    FromHP = FromUnitInfo.RobotInfo.HP;
    FromEN = FromUnitInfo.RobotInfo.EN;
    ToHP = ToUnitInfo.RobotInfo.HP;
    ToEN = ToUnitInfo.RobotInfo.EN;

    if (AttackType == AttackTypeEnum.Unable) {
      AtkDialogs = DIContainer.Instance.PilotService.GetPilotDialogsWhenUnable( FromUnitInfo.PilotInfo.PilotInstance.PilotID );
      return;
    }

    if (AttackType == AttackTypeEnum.Quit)
      return;

    AtkDialogs = DIContainer.Instance.PilotService.GetPilotDialogsWhenAttack( FromUnitInfo.PilotInfo.PilotInstance.PilotID, ToUnitInfo.PilotInfo.PilotInstance.PilotID, WeaponInfo.WeaponInstance.WeaponID,
           WeaponInfo.WeaponInstance.Weapon.NameType, WeaponInfo.WeaponInstance.Weapon.IsMelee );
    DefDialogs = DIContainer.Instance.PilotService.GetPilotDialogsWhenDef( this );

    //非地圖炮
    if (!WeaponInfo.WeaponInstance.Weapon.IsMap) {
      if (WeaponInfo.MaxBullets > 0 && WeaponInfo.RemainBullets > 0)
        WeaponInfo.RemainBullets--;
      
      FromUnitInfo.RobotInfo.EN -= FromUseEN;

      if (AvatarID == 0 && CutType < 1 && !IsDodge) {
        //命中後
        ToUnitInfo.RobotInfo.EN -= ToUseEN;

        if (IsDefeated) { //成功擊破敵人, 攻方氣力+3
          FromUnitInfo.PilotInfo.Willpower += 3;
          FromUnitInfo.PilotInfo.PilotInstance.Kills++;
          //移除攻擊方 在擊破敵人時的精神指令 (Ex. 幸運)
          FromUnitInfo.PilotInfo.ActiveSPCommandList.RemoveAll( s => s.Period == SPCommand.PeriodEnum.DestroyEnermy );
        }
        //命中時 攻方氣力+1
        FromUnitInfo.PilotInfo.Willpower += 1;
        //氣力+(命中) 
        var fromSkillHit = FromUnitInfo.PilotInfo.PilotInstance.PilotSkillInstanceList.FirstOrDefault( p => p.PilotSkillID == 32 );
        if (fromSkillHit != null) FromUnitInfo.PilotInfo.Willpower++;
        // 氣力+(被彈)
        var toSkillHit = ToUnitInfo.PilotInfo.PilotInstance.PilotSkillInstanceList.FirstOrDefault( p => p.PilotSkillID == 33 );
        if (toSkillHit != null)
          ToUnitInfo.PilotInfo.Willpower++;

        ToUnitInfo.RobotInfo.HP = Math.Max( ToUnitInfo.RobotInfo.HP - TotalDamage, 0 );
        afterDataBuff();

        //移除被彈時的 精神指令 (Ex. 不屈)
        ToUnitInfo.PilotInfo.ActiveSPCommandList.RemoveAll( s => s.Period == SPCommand.PeriodEnum.BeHit );
      }
      else {
        //氣力+(回避)
        var toSkillDodge = ToUnitInfo.PilotInfo.PilotInstance.PilotSkillInstanceList.FirstOrDefault( p => p.PilotSkillID == 31 );
        if (toSkillDodge != null) ToUnitInfo.PilotInfo.Willpower++;
      }

      //移除攻擊者和被攻擊方的精神指令   
      removeAttackerSPCommand();
      ToUnitInfo.PilotInfo.ActiveSPCommandList.RemoveAll( s => s.Period == SPCommand.PeriodEnum.BeAttacked );
    }
    else {   //攻方為地圖炮時 機體和精神消耗在外圍統一處理
      if (AvatarID == 0 && CutType < 1 && !IsDodge) {     //命中後
        ToUnitInfo.RobotInfo.EN -= ToUseEN;
        ToUnitInfo.RobotInfo.HP = Math.Max( ToUnitInfo.RobotInfo.HP - TotalDamage, 0 );
        afterDataBuff();
        //移除被彈時的 精神指令 (Ex. 不屈)
        ToUnitInfo.PilotInfo.ActiveSPCommandList.RemoveAll( s => s.Period == SPCommand.PeriodEnum.BeHit );
      }
      ToUnitInfo.PilotInfo.ActiveSPCommandList.RemoveAll( s => s.Period == SPCommand.PeriodEnum.BeAttacked );
    }
  }

  private void removeAttackerSPCommand() {
    FromUnitInfo.OtherUnit = null;

    //熱血和魂同時存在時, 只移除魂
    var idList = new int[] { 3, 4 };
    if (FromUnitInfo.PilotInfo.ActiveSPCommandList.Count( s => idList.Contains( s.ID ) ) == idList.Length) {
      FromUnitInfo.PilotInfo.ActiveSPCommandList.RemoveAll( s => s.Period == SPCommand.PeriodEnum.Attack && s.ID != 3);
    }
    else
      FromUnitInfo.PilotInfo.ActiveSPCommandList.RemoveAll( s => s.Period == SPCommand.PeriodEnum.Attack );
  } 

  private void PilotSkillBuffByAttacker() {
    List<PilotSkillInstance> psiList = FromUnitInfo.MapFightingUnit.PilotInfo.PilotInstance.PilotSkillInstanceList.Where( s => s.PilotSkill.IsBattleAttacker ).ToList();
    List<int> duplicateSkillIDs = new List<int>();

    foreach (PilotSkillInstance psi in psiList) {
      if (duplicateSkillIDs.Contains( psi.PilotSkillID )) continue;
      duplicateSkillIDs.Add( psi.PilotSkillID );
      Type psiType = Type.GetType( "PilotSkill_" + psi.PilotSkillID );
      (Activator.CreateInstance( psiType ) as IBattleAttacker)?.EffectAttacker( this, psi.OrderSort );
    }
  }

  private void PilotSkillBuffByDefenser() {
    List<PilotSkillInstance> psiList = ToUnitInfo.MapFightingUnit.PilotInfo.PilotInstance.PilotSkillInstanceList.Where( s => s.PilotSkill.IsBattleDefenser ).ToList();
    List<int> duplicateSkillIDs = new List<int>();

    foreach (PilotSkillInstance psi in psiList) {
      if (duplicateSkillIDs.Contains( psi.PilotSkillID )) continue;
      duplicateSkillIDs.Add( psi.PilotSkillID );
      Type psiType = Type.GetType( "PilotSkill_" + psi.PilotSkillID );
      (Activator.CreateInstance( psiType ) as IBattleDefenser)?.EffectDefenser( this, psi.OrderSort );
    }
  }

  private void PilotSkillBuffByAttackerHide() {
    List<PilotSkillInstance> psiList = FromUnitInfo.MapFightingUnit.PilotInfo.PilotInstance.PilotSkillInstanceList.Where( s => s.PilotSkill.IsBattleAtkHide ).ToList();
    List<int> duplicateSkillIDs = new List<int>();

    foreach (PilotSkillInstance psi in psiList) {
      if (duplicateSkillIDs.Contains( psi.PilotSkillID )) continue;
      duplicateSkillIDs.Add( psi.PilotSkillID );
      Type psiType = Type.GetType( "PilotSkill_" + psi.PilotSkillID );
      (Activator.CreateInstance( psiType ) as IBattleAttackerHide)?.EffectAttacker( this, psi.OrderSort );
    }
  }

  private void PilotSkillBuffByDefenserHide() {
    List<PilotSkillInstance> psiList = ToUnitInfo.MapFightingUnit.PilotInfo.PilotInstance.PilotSkillInstanceList.Where( s => s.PilotSkill.IsBattleDefHide ).ToList();
    List<int> duplicateSkillIDs = new List<int>();

    foreach (PilotSkillInstance psi in psiList) {
      if (duplicateSkillIDs.Contains( psi.PilotSkillID )) continue;
      duplicateSkillIDs.Add( psi.PilotSkillID );
      Type psiType = Type.GetType( "PilotSkill_" + psi.PilotSkillID );
      (Activator.CreateInstance( psiType ) as IBattleDefenserHide)?.EffectDefenser( this, psi.OrderSort );
    }
  }

  List<int> duplicateRobotSkillIDs;
  private void RobotSkillBuffByAttacker() {
    duplicateRobotSkillIDs = new List<int>();

    if (FromUnitInfo.RobotInfo.RobotInstance.RobotSkill1?.IsBattleAttacker == true) attackerBuffByRobotSkillID( FromUnitInfo.RobotInfo.RobotInstance.RobotSkill1.ID );
    if (FromUnitInfo.RobotInfo.RobotInstance.RobotSkill2?.IsBattleAttacker == true) attackerBuffByRobotSkillID( FromUnitInfo.RobotInfo.RobotInstance.RobotSkill2.ID );
    if (FromUnitInfo.RobotInfo.RobotInstance.RobotSkill3?.IsBattleAttacker == true) attackerBuffByRobotSkillID( FromUnitInfo.RobotInfo.RobotInstance.RobotSkill3.ID );
    if (FromUnitInfo.RobotInfo.RobotInstance.RobotSkill4?.IsBattleAttacker == true) attackerBuffByRobotSkillID( FromUnitInfo.RobotInfo.RobotInstance.RobotSkill4.ID );
    if (FromUnitInfo.RobotInfo.RobotInstance.RobotSkill5?.IsBattleAttacker == true) attackerBuffByRobotSkillID( FromUnitInfo.RobotInfo.RobotInstance.RobotSkill5.ID );
    if (FromUnitInfo.RobotInfo.RobotInstance.RobotSkill6?.IsBattleAttacker == true) attackerBuffByRobotSkillID( FromUnitInfo.RobotInfo.RobotInstance.RobotSkill6.ID );
  }

  private void attackerBuffByRobotSkillID( int id ) {
    if (duplicateRobotSkillIDs.Contains( id ))
      return;
    duplicateRobotSkillIDs.Add( id );
    Type rbsType = Type.GetType( "RobotSkill_" + id );
    (Activator.CreateInstance( rbsType ) as IBattleAttacker)?.EffectAttacker( this, 0 );
  }

  List<int> duplicateDefenserRobotSkillIDs;
  private void RobotSkillBuffByDefenser() {
    duplicateDefenserRobotSkillIDs = new List<int>();

    if (ToUnitInfo.RobotInfo.RobotInstance.RobotSkill1?.IsBattleAttacker == true) defenserBuffByRobotSkillID( ToUnitInfo.RobotInfo.RobotInstance.RobotSkill1.ID );
    if (ToUnitInfo.RobotInfo.RobotInstance.RobotSkill2?.IsBattleAttacker == true) defenserBuffByRobotSkillID( ToUnitInfo.RobotInfo.RobotInstance.RobotSkill2.ID );
    if (ToUnitInfo.RobotInfo.RobotInstance.RobotSkill3?.IsBattleAttacker == true) defenserBuffByRobotSkillID( ToUnitInfo.RobotInfo.RobotInstance.RobotSkill3.ID );
    if (ToUnitInfo.RobotInfo.RobotInstance.RobotSkill4?.IsBattleAttacker == true) defenserBuffByRobotSkillID( ToUnitInfo.RobotInfo.RobotInstance.RobotSkill4.ID );
    if (ToUnitInfo.RobotInfo.RobotInstance.RobotSkill5?.IsBattleAttacker == true) defenserBuffByRobotSkillID( ToUnitInfo.RobotInfo.RobotInstance.RobotSkill5.ID );
    if (ToUnitInfo.RobotInfo.RobotInstance.RobotSkill6?.IsBattleAttacker == true) defenserBuffByRobotSkillID( ToUnitInfo.RobotInfo.RobotInstance.RobotSkill6.ID );
  }

  private void defenserBuffByRobotSkillID( int id ) {
    if (duplicateDefenserRobotSkillIDs.Contains( id ))
      return;
    duplicateDefenserRobotSkillIDs.Add( id );
    Type rbsType = Type.GetType( "RobotSkill_" + id );
    (Activator.CreateInstance( rbsType ) as IBattleDefenser)?.EffectDefenser( this, 0 );
  }


  private void SpComBuffByAttacker() {
    List<SPCommand> spComList = FromUnitInfo.MapFightingUnit.PilotInfo.ActiveSPCommandList.Where( s => s.IsAtk ).ToList();
    bool haveSoul = spComList.Any( s => s.ID == 4 );
    foreach (SPCommand spCom in spComList) {
      switch (spCom.ID) {
        case 1: FinalHitRateAdd += 300; break;
        case 4: DamageMultiply *= 2.5f; /*DamageRateAdd += 1.4f;*/ break;
        case 3: if (!haveSoul) DamageMultiply *= 2; break;
        case 7: FinalHitRateAdd += 30; break;
        case 8: CriAdd += 50; break;
        case 11: MoneyRateAdd += 1; break;
        case 15: if (FromUnitInfo.PilotInfo.Dex > ToUnitInfo.PilotInfo.Dex)
                   IsNoKill = true;
                 break;
        case 17: ExpRateAdd += 1f; PpRateAdd += 0.2f; break;  //努力
        case 31: HitRateMultiply *= 0.7f; break;
        case 35: DamageMultiply *= 0.9f; break;
      }
    }
  }

  private void SpComBuffByDef() {
    List<SPCommand> spComList = ToUnitInfo.MapFightingUnit.PilotInfo.ActiveSPCommandList.Where( s => s.IsDef ).ToList();
    //bool haveSoul = spComList.Any( s => s.ID == 4 );
    foreach (SPCommand spCom in spComList) {
      switch (spCom.ID) {
        case 2: DamageMultiply *= 0.25f; break;
        case 5: DamageMultiply *= 0.1f;  break;
        case 6: FinalHitRateAdd = -600; break;
        case 7: FinalHitRateAdd -= 30; break;
        case 16: HitRateAdd += 20; break;   //被偵察方(回避率-20%) = 攻方命中率 +20%
        case 35: DamageMultiply *= 1.1f; break;
      }
    }
  }

  private void afterDataBuff() {
    if (ToBuffArmor > 0 && ToBuffArmor >= ToUnitInfo.RobotInfo.DeBuffArmor) {
      //ToUnitInfo.RobotInfo.DeBuffArmor = Mathf.Max( ToUnitInfo.RobotInfo.DeBuffArmor, ToBuffArmor );            //戰鬥後 影響 裝甲值
      ToUnitInfo.RobotInfo.DeBuffArmor = ToBuffArmor;
      ToUnitInfo.RobotInfo.DeBuffArmorPhase = 2;
    }

    if (ToBuffMotility > 0 && ToBuffMotility >= ToUnitInfo.RobotInfo.DeBuffMotility) {
      ToUnitInfo.RobotInfo.DeBuffMotility = ToBuffMotility;  //戰鬥後 影響 運動性
      ToUnitInfo.RobotInfo.DeBuffMotilityPhase = 2;
    }

    if (ToBuffHitRate > 0 && ToBuffHitRate >= ToUnitInfo.RobotInfo.DeBuffHitRate) {
      ToUnitInfo.RobotInfo.DeBuffHitRate = ToBuffHitRate;     //戰鬥後 影響 命中值
      ToUnitInfo.RobotInfo.DeBuffHitRatePhase = 2;
    }
    ToUnitInfo.PilotInfo.Willpower = ToUnitInfo.PilotInfo.Willpower - ToBuffWillPower + 1;  //戰鬥後 影響 氣力值 及 被彈時氣力+1
    ToUnitInfo.PilotInfo.RemainSp -= ToBuffSP;              //戰鬥後 影響 SP值
  }

  public void SelectDefence() {
    //DamageMultiply *= 0.5f;
    CounterType = CounterTypeEnum.Defense;
  }

  public void SelectDodge() {
    //FinalHitRateMultiply *= 0.5f;
    CounterType = CounterTypeEnum.Dodge;
  }

  public string AttackSingleWord() {
    switch (AttackType) {
      case AttackTypeEnum.Assistance:  return "援";
      case AttackTypeEnum.Support: return "支";
      case AttackTypeEnum.CounterSkill: return "反";
      case AttackTypeEnum.Normal: return "攻";
      case AttackTypeEnum.Unable: return "無";
      default: return "無";
    }
  }

  public string CounterSideSingleWord() {
    switch (CounterType) {
      case CounterTypeEnum.Defense: return "防";
      case CounterTypeEnum.Dodge: return "回";
      case CounterTypeEnum.Normal: return "反";
      case CounterTypeEnum.Unable: return "無";
      default: return "無";
    }
  }

}

