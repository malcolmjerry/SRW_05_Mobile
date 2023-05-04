using System.Linq;
using UnityEngine;

//Chobham裝甲
public class Parts_1 : IBuffUnit {

  public void BuffUnit( MapFightingUnit unit ) {
    RobotInfo robotInfo = unit.RobotInfo;
    robotInfo.MaxHP += 500;
    robotInfo.Armor += 200;
  }

}

//磁層關節
public class Parts_2 : IBuffUnit {

  public void BuffUnit( MapFightingUnit unit ) {
    RobotInfo robotInfo = unit.RobotInfo;
    robotInfo.Motility += 10;
  }

}

//地形適性A
public class Parts_3 : IBuffUnit {

  public void BuffUnit( MapFightingUnit unit ) {
    RobotInfo robotInfo = unit.RobotInfo;
    robotInfo.TerrainSky = Mathf.Max( 4, robotInfo.TerrainSky );
    robotInfo.TerrainLand = Mathf.Max( 4, robotInfo.TerrainLand );
    robotInfo.TerrainSea = Mathf.Max( 4, robotInfo.TerrainSea );
    robotInfo.TerrainSpace = Mathf.Max( 4, robotInfo.TerrainSpace );

    unit.WeaponList.ForEach( w => {

    } );
  }

}

//分身發生器
public class Parts_4 : AvatarBase {

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
    else
      return false;
  }

}

//重力防護罩
public class Parts_5 : BarrierBase, IBarrier {

  private readonly static int barrierID = 9;
  override public int BarrierID { get { return barrierID; } }

  private readonly static string name = "I-Field";
  override public string Name { get { return name; } }

  private readonly static int amount = 1500;
  override public int Amount { get { return amount; } }

  private readonly static int en = 10;
  override public int EN { get { return en; } }

  override public bool CanActive( AttackData atkData ) {
    return atkData.ToUnitInfo.PilotInfo.Willpower > 0 && atkData.ToUnitInfo.RobotInfo.EN >= 10;
  }

  override protected float Damage( float damage ) {
    return damage - amount;
  }

}

//Hybrid 裝甲
public class Parts_6 : IBuffUnit {

  public void BuffUnit( MapFightingUnit unit ) {
    RobotInfo robotInfo = unit.RobotInfo;
    robotInfo.MaxHP += 1000;
    robotInfo.Armor += 100;
  }

}

//Booster
public class Parts_7 : IBuffUnit {

  public void BuffUnit( MapFightingUnit unit ) {
    RobotInfo robotInfo = unit.RobotInfo;
    robotInfo.MovePower += 1;
  }

}

//大型推進器
public class Parts_8 : IBuffUnit {

  public void BuffUnit( MapFightingUnit unit ) {
    RobotInfo robotInfo = unit.RobotInfo;
    robotInfo.MovePower += 2;
  }

}

//高性能雷達
public class Parts_9 : IBuffUnit {

  public void BuffUnit( MapFightingUnit unit ) {
    RobotInfo robotInfo = unit.RobotInfo;
    foreach (var wp in unit.WeaponList.Where( w => !w.WeaponInstance.Weapon.IsMap )) {
      wp.MaxRange += 1;
    }
  }

}


