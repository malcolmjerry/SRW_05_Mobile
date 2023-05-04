using System.Linq;

//對 Beam Coating
public class Parts_10 : BarrierBase, IBarrier {

  private readonly static int barrierID = 2;
  override public int BarrierID { get { return barrierID; } }

  private readonly static string name = "對 Beam Coating";
  override public string Name { get { return name; } }

  private readonly static int amount = 1500;
  override public int Amount { get { return amount; } }

  private readonly static int en = 10;
  override public int EN { get { return en; } }

  override public bool CanActive( AttackData atkData ) {
    return atkData.ToUnitInfo.PilotInfo.Willpower > 0 && atkData.ToUnitInfo.RobotInfo.EN >= en && atkData.WeaponInfo.WeaponInstance.Weapon.IsBeam;
  }

  override protected float Damage( float damage ) {
    return damage - amount;
  }

}

// I 力場
public class Parts_11 : BarrierBase, IBarrier {

  private readonly static int barrierID = 3;
  override public int BarrierID { get { return barrierID; } }

  private readonly static string name = "I 力場";
  override public string Name { get { return name; } }

  private readonly static int amount = 2500;
  override public int Amount { get { return amount; } }

  private readonly static int en = 15;
  override public int EN { get { return en; } }

  override public bool CanActive( AttackData atkData ) {
    return atkData.ToUnitInfo.PilotInfo.Willpower > 0 && atkData.ToUnitInfo.RobotInfo.EN >= en && atkData.WeaponInfo.WeaponInstance.Weapon.IsBeam;
  }

  override protected float Damage( float damage ) {
    if (damage <= amount) 
      return 0;
    
    return damage * 0.5f;
  }

}

// 超合金Z
public class Parts_12 : IBuffUnit {

  public void BuffUnit( MapFightingUnit unit ) {
    RobotInfo robotInfo = unit.RobotInfo;
    robotInfo.MaxHP += 2000;
    robotInfo.Armor += 200;
  }

}

// 超合金 New Z
public class Parts_13 : IBuffUnit {
  public void BuffUnit( MapFightingUnit unit ) {
    RobotInfo robotInfo = unit.RobotInfo;
    robotInfo.MaxHP += 1500;
    robotInfo.Armor += 250;
  }
}

//Apogee Motor
public class Parts_14 : IBuffUnit {
  public void BuffUnit( MapFightingUnit unit ) {
    RobotInfo robotInfo = unit.RobotInfo;
    robotInfo.Motility += 5;
    robotInfo.MovePower += 1;
  }
}

//生物感應器
public class Parts_15 : IBuffUnit {

  public void BuffUnit( MapFightingUnit unit ) {
    RobotInfo robotInfo = unit.RobotInfo;
    robotInfo.Motility += 15;
    robotInfo.HitRate += 15;
  }

}

//賽可膠骨架
public class Parts_16 : IBuffUnit {
  public void BuffUnit( MapFightingUnit unit ) {
    RobotInfo robotInfo = unit.RobotInfo;
    robotInfo.Motility += 25;
  }
}

//化地瑪
public class Parts_17 : IBuffUnit {
  public void BuffUnit( MapFightingUnit unit ) {
    RobotInfo robotInfo = unit.RobotInfo;
    robotInfo.Motility += 20;
    robotInfo.HitRate += 20;
    robotInfo.MovePower += 2;
  }
}

//高性能照準器
public class Parts_18 : IBuffUnit {
  public void BuffUnit( MapFightingUnit unit ) {
    RobotInfo robotInfo = unit.RobotInfo;
    robotInfo.HitRate += 30;
  }
}

// 八口
public class Parts_19 : IBuffUnit {
  public void BuffUnit( MapFightingUnit unit ) {
    RobotInfo robotInfo = unit.RobotInfo;
    robotInfo.Motility += 20;
    robotInfo.HitRate += 20;
    robotInfo.MovePower += 1;
    foreach (var wp in unit.WeaponList.Where( w => !w.WeaponInstance.Weapon.IsMap )) {
      wp.MaxRange += 1;
    }
  }
}
