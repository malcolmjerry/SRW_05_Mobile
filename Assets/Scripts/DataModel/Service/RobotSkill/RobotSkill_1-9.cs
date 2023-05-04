/// <summary> 零式系統 </summary>
public class RobotSkill_1 : IBuffUnit, ICheckOnOff {

  public void BuffUnit( MapFightingUnit unit ) {
    if (isEnable( unit )) {
      //unit.PilotInfo.Melee += 30;
      //unit.PilotInfo.Dodge += 30;
      //unit.PilotInfo.Hit += 30;
      //unit.PilotInfo.Dex += 30;
      unit.PilotInfo.BuffMelee += 30;
      unit.PilotInfo.BuffDodge += 30;
      unit.PilotInfo.BuffHit += 30;
      unit.PilotInfo.BuffDex += 30;

      unit.PilotInfo.Melee = unit.PilotInfo.Melee + 30;
      unit.PilotInfo.Dodge = unit.PilotInfo.Dodge + 30;
      unit.PilotInfo.Hit = unit.PilotInfo.Hit + 30;
      unit.PilotInfo.Dex = unit.PilotInfo.Dex + 30;
    }
  }

  public bool IsHighlight( MapFightingUnit unit ) {
    return isEnable( unit );
  }

  private bool isEnable( MapFightingUnit unit ) {
    bool enable =
      unit.PilotInfo.Willpower >= 130 ||
      unit.PilotInfo.PilotInstance.Pilot.Ace1 == 2 && unit.PilotInfo.PilotInstance.Kills >= 50  && unit.PilotInfo.Willpower >= 110 ||
      unit.PilotInfo.PilotInstance.Pilot.Ace2 == 2 && unit.PilotInfo.PilotInstance.Kills >= 100 && unit.PilotInfo.Willpower >= 110 ||
      unit.PilotInfo.PilotInstance.Pilot.Ace3 == 2 && unit.PilotInfo.PilotInstance.Kills >= 150 && unit.PilotInfo.Willpower >= 110 ||
      unit.PilotInfo.PilotInstance.Pilot.Ace4 == 2 && unit.PilotInfo.PilotInstance.Kills >= 200 && unit.PilotInfo.Willpower >= 110;
    return enable;
  }

}

/// <summary> Beam Coat </summary>
public class RobotSkill_2 : BarrierBase, IBarrier {

  private readonly static int barrierID = 1;
  override public int BarrierID { get { return barrierID; } }

  private readonly static string name = "Beam coat";
  override public string Name { get { return name; } }

  private readonly static int amount = 1200;
  override public int Amount { get { return amount; } }

  private readonly static int en = 5;
  override public int EN { get { return en; } }

  override public bool CanActive( AttackData atkData ) {
    return atkData.ToUnitInfo.PilotInfo.Willpower > 0 && atkData.ToUnitInfo.RobotInfo.EN >= en && atkData.WeaponInfo.WeaponInstance.Weapon.IsBeam;
  }

  override protected float Damage( float damage ) {
    return damage - amount;
  }

}

//I 力場
public class RobotSkill_3 : BarrierBase, IBarrier {

  private readonly static int barrierID = 2;
  override public int BarrierID { get { return barrierID; } }

  private readonly static string name = "I-Field";
  override public string Name { get { return name; } }

  private readonly static int amount = 2500;
  override public int Amount { get { return amount; } }

  private readonly static int en = 15;
  override public int EN { get { return en; } }

  override public bool CanActive( AttackData atkData ) {
    return atkData.ToUnitInfo.PilotInfo.Willpower > 0 && atkData.ToUnitInfo.RobotInfo.EN >= en && atkData.WeaponInfo.WeaponInstance.Weapon.IsBeam;
  }

  override protected float Damage( float damage ) {
    if (damage <= amount) {
      return 0;
    }
    return damage * 0.5f;
  }

}

// 分身
public class RobotSkill_4 : AvatarBase {

  private readonly static int avatarID = 1;         // 0 不生效  1 分身, God Shadow, 馬哈障眼法
  override public int AvatarID { get { return avatarID; } }

  private readonly static string name = "分身";
  override public string Name { get { return name; } }

  private readonly static int en = 0;
  override public int EN { get { return en; } }

  override protected bool CanActive( AttackData atkData ) {
    if (atkData.ToUnitInfo.PilotInfo.Willpower >= 130 && atkData.ToUnitInfo.RobotInfo.EN >= 0) {  //日後增加擴散類武器無法被分身閃避
      return 50 > UnityEngine.Random.Range( 0, 100 );
    }
    else return false;
  }

}

//魔神POWER
public class RobotSkill_5 : IBattleAttacker, ICheckOnOff {

  public void EffectAttacker( AttackData atkData, int order ) {
    if (IsHighlight( atkData.FromUnitInfo.MapFightingUnit )) {
      atkData.DamageMultiply *= 1.15f; 
    }    
  }

  public bool IsHighlight( MapFightingUnit unit ) {
    return unit.PilotInfo.Willpower >= 130;
  }
}


//PS裝甲
public class RobotSkill_6 : BarrierBase, IBarrier {

  private readonly static int barrierID = 2;
  override public int BarrierID { get { return barrierID; } }

  private readonly static string name = "PS裝甲";
  override public string Name { get { return name; } }

  private readonly static int amount = 1200;
  override public int Amount { get { return amount; } }

  private readonly static int en = 5;
  override public int EN { get { return en; } }

  override public bool CanActive( AttackData atkData ) {
    return atkData.ToUnitInfo.RobotInfo.EN >= en && atkData.WeaponInfo.WeaponInstance.Weapon.IsPhysical;
  }

  override protected float Damage( float damage ) {
    return damage - amount;
  }
}

/// <summary> 能力解放 </summary>
public class RobotSkill_7 : IBuffUnit, ICheckOnOff {

  public void BuffUnit( MapFightingUnit unit ) {
    if (isEnable( unit )) {
      unit.RobotInfo.Motility = unit.RobotInfo.Motility + 30;
      unit.RobotInfo.HitRate = unit.RobotInfo.HitRate + 30;
      unit.MovePower = unit.MovePower + 2;
    }
  }

  public bool IsHighlight( MapFightingUnit unit ) {
    return isEnable( unit );
  }

  private bool isEnable( MapFightingUnit unit ) {
    bool enable = unit.PilotInfo.Willpower >= 130;
    return enable;
  }

}

//生物電腦
public class RobotSkill_8 : BarrierBase, IBarrier, IBuffUnit, ICheckOnOff {

  private readonly static int barrierID = 2;
  override public int BarrierID { get { return barrierID; } }

  private readonly static string name = "精神防護罩";
  override public string Name { get { return name; } }

  private readonly static int amount = 3000;
  override public int Amount { get { return amount; } }

  private readonly static int en = 0;
  override public int EN { get { return en; } }

  override public bool CanActive( AttackData atkData ) {
    return atkData.ToUnitInfo.PilotInfo.Willpower > 130;
  }

  override protected float Damage( float damage ) {
    if (damage <= amount) {
      return 0;
    }
    return damage;
  }

  public bool IsHighlight( MapFightingUnit unit ) {
    return unit.PilotInfo.Willpower >= 140;
  }

  public void BuffUnit( MapFightingUnit unit ) {
    if (IsHighlight( unit )) {
      unit.RobotInfo.HitRate = unit.RobotInfo.HitRate + 100;
    }
  }

}

//絕對領域
public class RobotSkill_9 : BarrierBase, IBarrier {

  private readonly static int barrierID = 2;
  override public int BarrierID { get { return barrierID; } }

  private readonly static string name = "絕對領域";
  override public string Name { get { return name; } }

  private readonly static int amount = 4000;
  override public int Amount { get { return amount; } }

  private readonly static int en = 5;
  override public int EN { get { return en; } }

  override public bool CanActive( AttackData atkData ) {
    return atkData.ToUnitInfo.PilotInfo.Willpower > 105 && atkData.ToUnitInfo.RobotInfo.EN >= en;
  }

  override protected float Damage( float damage ) {
    if (damage <= amount) {
      return 0;
    }
    return damage;
  }

}
