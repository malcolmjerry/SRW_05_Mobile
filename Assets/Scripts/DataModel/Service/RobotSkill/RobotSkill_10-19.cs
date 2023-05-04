using System.Linq;
/// <summary> HP回復(小) </summary>
public class RobotSkill_10 : IPhaseStart {

  public void BuffPhaseStart( MapFightingUnit unit ) {
    unit.RobotInfo.HP = (int)(unit.RobotInfo.HP + unit.RobotInfo.MaxHP * 0.1f);
  }

}

/// <summary> HP回復(中) </summary>
public class RobotSkill_11 : IPhaseStart {

  public void BuffPhaseStart( MapFightingUnit unit ) {
    unit.RobotInfo.HP = (int)(unit.RobotInfo.HP + unit.RobotInfo.MaxHP * .2f);
  }

}

/// <summary> HP回復(大) </summary>
public class RobotSkill_12 : IPhaseStart {

  public void BuffPhaseStart( MapFightingUnit unit ) {
    unit.RobotInfo.HP = (int)(unit.RobotInfo.HP + unit.RobotInfo.MaxHP * .3f);
  }

}

/// <summary> EN回復(小) </summary>
public class RobotSkill_13 : IPhaseStart {

  public void BuffPhaseStart( MapFightingUnit unit ) {
    unit.RobotInfo.EN = (int)(unit.RobotInfo.EN + unit.RobotInfo.MaxEN * 0.1f);
  }

}

/// <summary> EN回復(中) </summary>
public class RobotSkill_14 : IPhaseStart {

  public void BuffPhaseStart( MapFightingUnit unit ) {
    unit.RobotInfo.EN = (int)(unit.RobotInfo.EN + unit.RobotInfo.MaxEN * .2f);
  }

}

/// <summary> EN回復(大) </summary>
public class RobotSkill_15 : IPhaseStart {

  public void BuffPhaseStart( MapFightingUnit unit ) {
    unit.RobotInfo.EN = (int)(unit.RobotInfo.EN + unit.RobotInfo.MaxEN * .3f);
  }

}

/// <summary> Psycho Frame </summary>
public class RobotSkill_16 : IBuffUnit, ICheckOnOff {

  public void BuffUnit( MapFightingUnit unit ) {
    if (IsHighlight( unit )) {
      unit.RobotInfo.Motility = unit.RobotInfo.Motility + 30;
      unit.RobotInfo.HitRate = unit.RobotInfo.HitRate + 30;

      foreach (var weaponInfo in unit.WeaponList?.Where( w => w.WeaponInstance.Weapon.IsNT > 0 )) {
        weaponInfo.MaxRange += 1;
      }
    }
  }

  public bool IsHighlight( MapFightingUnit unit ) {
    return unit.PilotInfo.Willpower >= 130;
  }

}

/// <summary> 超級模式 </summary>
public class RobotSkill_17 : IBuffUnit, ICheckOnOff {

  public void BuffUnit( MapFightingUnit unit ) {
    if (IsHighlight( unit )) {
      unit.RobotInfo.Motility = unit.RobotInfo.Motility + 20;
      unit.MovePower = unit.MovePower + 1;

      foreach (var weaponInfo in unit.WeaponList) {
        weaponInfo.HitPoint = weaponInfo.HitPoint + 200;
      }
    }
  }

  public bool IsHighlight( MapFightingUnit unit ) {
    return unit.PilotInfo.Willpower >= 130;
  }

}

/// <summary> S2機關 </summary>
public class RobotSkill_18 : IPhaseStart {

  public void BuffPhaseStart( MapFightingUnit unit ) {
    unit.RobotInfo.HP = (int)(unit.RobotInfo.HP + unit.RobotInfo.MaxHP * 0.1f);
    unit.RobotInfo.EN = (int)(unit.RobotInfo.EN + unit.RobotInfo.MaxEN * 0.1f);
  }

}
