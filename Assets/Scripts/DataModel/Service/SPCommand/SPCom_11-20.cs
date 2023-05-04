using System.Linq;
using UnityEngine;

//精神 11 幸運, 直接在 AttackData 中實現  
public class SPCom_11 {
  //沒有用
  //public void EffectAttacker( AttackData attData ) {
    //attData.MoneyRateAdd += 1;
    //return;
  //}
}

//精神 12 根性  使用時直接反映到 MapFightUnitInfo (IConsumable)
public class SPCom_12 : IConsumable, ICheckOnOff {
  public void Consume( MapFightingUnit unit, MapFightingUnit useBy = null ) {
    unit.RobotInfo.HP = Mathf.Min( (int)(unit.RobotInfo.HP + unit.RobotInfo.MaxHP * .3f), unit.RobotInfo.MaxHP );
  }
  public bool IsHighlight( MapFightingUnit unit ) {
    return unit.RobotInfo.HP < unit.RobotInfo.MaxHP;
  }
}

//精神 13 卜根性  使用時直接反映到 MapFightUnitInfo
public class SPCom_13 : IConsumable, ICheckOnOff {
  public void Consume( MapFightingUnit unit, MapFightingUnit useBy = null ) {
    unit.RobotInfo.HP = Mathf.Min( (int)(unit.RobotInfo.HP + unit.RobotInfo.MaxHP * .8f), unit.RobotInfo.MaxHP );
  }
  public bool IsHighlight( MapFightingUnit unit ) {
    return unit.RobotInfo.HP < unit.RobotInfo.MaxHP;
  }
}

//精神 14 挑發  使用時直接加到(被使用者)的精神狀態列表

//精神 15 手加減  使用時加到精神狀態列表, 在 AttackData 中實現  

//精神 16 偵察  使用時直接加到(被使用者)的精神狀態列表, 在 AttackData 中實現  

//精神 17 努力  使用時加到精神狀態列表, 在 AttackData 中實現  

//精神 18 信賴  使用時反映到(被使用者)的 MapFightUnitInfo
public class SPCom_18 : IConsumable, ICheckOnOff {
  public void Consume( MapFightingUnit unit, MapFightingUnit useBy = null ) {
    unit.RobotInfo.HP = unit.RobotInfo.HP + 3000;
  }
  public bool IsHighlight( MapFightingUnit unit ) {
    return unit.RobotInfo.HP < unit.RobotInfo.MaxHP;
  }
}

//精神 19 氣合
public class SPCom_19 : IConsumable, ICheckOnOff {
  public void Consume( MapFightingUnit unit, MapFightingUnit useBy = null ) {
    unit.PilotInfo.Willpower = unit.PilotInfo.Willpower + 10;
  }
  public bool IsHighlight( MapFightingUnit unit ) {
    return unit.PilotInfo.Willpower < unit.PilotInfo.MaxWillpower;
  }
}

//精神 20 氣迫
public class SPCom_20 : IConsumable, ICheckOnOff {
  public void Consume( MapFightingUnit unit, MapFightingUnit useBy = null ) {
    unit.PilotInfo.Willpower = unit.PilotInfo.Willpower + 25;
  }
  public bool IsHighlight( MapFightingUnit unit ) {
    return unit.PilotInfo.Willpower < unit.PilotInfo.MaxWillpower;
  }
}

