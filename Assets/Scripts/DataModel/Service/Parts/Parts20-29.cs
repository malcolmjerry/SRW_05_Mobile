using System.Linq;

//墮天使の翅
public class Parts_20 : IPhaseStart {

  public void BuffPhaseStart( MapFightingUnit unit ) {
    unit.PilotInfo.RemainSp += 10;
  }

}

//太陽能板
public class Parts_21 : IPhaseStart {

  public void BuffPhaseStart( MapFightingUnit unit ) {
    unit.RobotInfo.EN += 30;
  }

}

//高性能照準器
public class Parts_22 : IBuffUnit {
  public void BuffUnit( MapFightingUnit unit ) {
    RobotInfo robotInfo = unit.RobotInfo;
    robotInfo.MaxEN += 120;
  }
}

//補助GS
public class Parts_23 : IBuffUnit {
  public void BuffUnit( MapFightingUnit unit ) {
    RobotInfo robotInfo = unit.RobotInfo;
    robotInfo.MaxEN += 50;
  }
}

//精密照準鏡
public class Parts_24 : IBuffUnit {
  public void BuffUnit( MapFightingUnit unit ) {
    RobotInfo robotInfo = unit.RobotInfo;
    robotInfo.HitRate += 20;
  }
}
