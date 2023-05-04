using System;
using System.Linq;
using UnityEngine;

//精神 21 脫力, 使用時直接反映到(被使用者)的 MapFightUnitInfo, 敵人限定  
public class SPCom_21 : IConsumable, ICheckOnOff {
  public void Consume( MapFightingUnit unit, MapFightingUnit useBy = null ) {
    unit.PilotInfo.Willpower = Math.Max( unit.PilotInfo.Willpower - 10, 100 );
  }
  public bool IsHighlight( MapFightingUnit unit ) {
    return unit.PilotInfo.Willpower > 100;
  }
}

//精神 22 覺醒, 使用時直接加到的精神狀態列表

//精神 23 再動, 使用時直接反映到(被使用者)的 MapFightUnitInfo = 使用覺醒, 味方限定  

//精神 24 復活, 使用時從 DeadList 中選擇味方機體進行復活, 味方限定  
public class SPCom_24 {
  public bool CanUse() {
    // 判斷死亡列表是否為空
    return true;
  }
}

//精神 25 友情  使用時直接反映到(被使用者)的 MapFightUnitInfo, 雙方氣力+3, 味方限定  
public class SPCom_25 : IConsumable, ICheckOnOff {
  public void Consume( MapFightingUnit unit, MapFightingUnit useBy = null ) {
    unit.RobotInfo.HP = Math.Min( (int)(unit.RobotInfo.HP + unit.RobotInfo.MaxHP * .8f), unit.RobotInfo.MaxHP );
  }
  public bool IsHighlight( MapFightingUnit unit ) {
    return unit.RobotInfo.HP < unit.RobotInfo.MaxHP;
  }
}

//精神 26 愛   自身使用必閃、必中、熱血、幸運，指定味方單位 HP全回復, 自身氣力+5, 味方限定  
public class SPCom_26 : IConsumable {
  public void Consume( MapFightingUnit unit, MapFightingUnit useBy = null ) {
    unit.RobotInfo.HP = unit.RobotInfo.MaxHP;

    SPCommand spCom;

    spCom = DIContainer.Instance.PilotService.GetSPComByID( 1 ); //必中
    if (!useBy.PilotInfo.ActiveSPCommandList.Any( s => s.ID == spCom.ID )) unit.PilotInfo.ActiveSPCommandList.Add( DIContainer.Instance.PilotService.GetSPComByID( spCom.ID ) );

    spCom = DIContainer.Instance.PilotService.GetSPComByID( 6 ); //必閃
    if (!useBy.PilotInfo.ActiveSPCommandList.Any( s => s.ID == spCom.ID )) unit.PilotInfo.ActiveSPCommandList.Add( DIContainer.Instance.PilotService.GetSPComByID( spCom.ID ) );

    spCom = DIContainer.Instance.PilotService.GetSPComByID( 11 ); //幸運
    if (!useBy.PilotInfo.ActiveSPCommandList.Any( s => s.ID == spCom.ID )) unit.PilotInfo.ActiveSPCommandList.Add( DIContainer.Instance.PilotService.GetSPComByID( spCom.ID ) );

    spCom = DIContainer.Instance.PilotService.GetSPComByID( 3 ); //熱血
    if (!useBy.PilotInfo.ActiveSPCommandList.Any( s => s.ID == spCom.ID )) unit.PilotInfo.ActiveSPCommandList.Add( DIContainer.Instance.PilotService.GetSPComByID( spCom.ID ) );

  }
}

//精神 27 狙擊  使用時直接加到的精神狀態列表, Update Unit時檢查, 影響到 WeaponList
public class SPCom_27 : IBuffUnit {
  public void BuffUnit( MapFightingUnit unit ) {
    unit.WeaponList.ForEach( w => w.IsMove = true );
  }
}

//精神 28 激勵  使用時直接反映到(被使用者)的 MapFightUnitInfo, 對方氣力+5, 自身氣力+1, 味方限定

//精神 29 大激勵  使用時直接反映到(被使用者s)的 MapFightUnitInfo, 對方氣力+4, 自身氣力+1, 味方限定

//精神 30 補給  使用時直接反映到(被使用者)的 MapFightUnitInfo, 自身可使用

