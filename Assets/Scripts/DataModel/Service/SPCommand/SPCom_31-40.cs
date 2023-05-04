using System.Linq;
using UnityEngine;

//精神 31  攪亂 , 使用時反映到全部敵人, PlayerTeam無效
public class SPCom_31 : IBuffUnit {
  public void BuffUnit( MapFightingUnit unit ) {
    unit.RobotInfo.HP = Mathf.Min( (int)(unit.RobotInfo.HP + unit.RobotInfo.MaxHP * .8f), unit.RobotInfo.MaxHP );
  }
}

//精神 32  期待, 使用時直接反映到(被使用者)的 MapFightUnitInfo, SP回復 30, 味方限定
public class SPCom_32 : IConsumable, ICheckOnOff {
  public void Consume( MapFightingUnit unit, MapFightingUnit useBy = null ) {
    unit.PilotInfo.RemainSp = unit.PilotInfo.RemainSp + 30;
  }
  public bool IsHighlight( MapFightingUnit unit ) {
    return unit.PilotInfo.Willpower < unit.PilotInfo.MaxWillpower;
  }
}


//精神 33  突擊, 使用時直接加到使用者的精神狀態列表
public class SPCom_33 : IBuffUnit {
  public void BuffUnit( MapFightingUnit unit ) {
    unit.WeaponList.ForEach( w => w.IsMove = true );
  }
}


//精神 34  Hit & Run, 使用時直接加到使用者的精神狀態列表

//精神 35  分析, 使用時直接反映到(被使用者)的 的精神狀態列表, 一回合、對指定敵方單位攻擊力1.1倍、被攻擊0.9倍


//精神 36 奇跡  
public class SPCom_36 : IConsumable {
  public void Consume( MapFightingUnit unit, MapFightingUnit useBy = null ) {
    unit.PilotInfo.Willpower = unit.PilotInfo.Willpower + 30;
    unit.RobotInfo.EN = unit.RobotInfo.EN + 100;

    SPCommand spCom;

    spCom = DIContainer.Instance.PilotService.GetSPComByID( 1 ); //必中
    if (!unit.PilotInfo.ActiveSPCommandList.Any( s => s.ID == spCom.ID )) unit.PilotInfo.ActiveSPCommandList.Add( DIContainer.Instance.PilotService.GetSPComByID( spCom.ID ) );

    spCom = DIContainer.Instance.PilotService.GetSPComByID( 6 ); //必閃
    if (!unit.PilotInfo.ActiveSPCommandList.Any( s => s.ID == spCom.ID )) unit.PilotInfo.ActiveSPCommandList.Add( DIContainer.Instance.PilotService.GetSPComByID( spCom.ID ) );

    spCom = DIContainer.Instance.PilotService.GetSPComByID( 7 ); //集中
    if (!unit.PilotInfo.ActiveSPCommandList.Any( s => s.ID == spCom.ID )) unit.PilotInfo.ActiveSPCommandList.Add( DIContainer.Instance.PilotService.GetSPComByID( spCom.ID ) );

    spCom = DIContainer.Instance.PilotService.GetSPComByID( 10 ); //極速
    if (!unit.PilotInfo.ActiveSPCommandList.Any( s => s.ID == spCom.ID )) unit.PilotInfo.ActiveSPCommandList.Add( DIContainer.Instance.PilotService.GetSPComByID( spCom.ID ) );

    spCom = DIContainer.Instance.PilotService.GetSPComByID( 17 ); //努力
    if (!unit.PilotInfo.ActiveSPCommandList.Any( s => s.ID == spCom.ID )) unit.PilotInfo.ActiveSPCommandList.Add( DIContainer.Instance.PilotService.GetSPComByID( spCom.ID ) );

    spCom = DIContainer.Instance.PilotService.GetSPComByID( 11 ); //幸運
    if (!unit.PilotInfo.ActiveSPCommandList.Any( s => s.ID == spCom.ID )) unit.PilotInfo.ActiveSPCommandList.Add( DIContainer.Instance.PilotService.GetSPComByID( spCom.ID ) );

    spCom = DIContainer.Instance.PilotService.GetSPComByID( 3 ); //熱血
    if (!unit.PilotInfo.ActiveSPCommandList.Any( s => s.ID == spCom.ID )) unit.PilotInfo.ActiveSPCommandList.Add( DIContainer.Instance.PilotService.GetSPComByID( spCom.ID ) );

    spCom = DIContainer.Instance.PilotService.GetSPComByID( 4 ); //魂
    if (!unit.PilotInfo.ActiveSPCommandList.Any( s => s.ID == spCom.ID )) unit.PilotInfo.ActiveSPCommandList.Add( DIContainer.Instance.PilotService.GetSPComByID( spCom.ID ) );
  }
}

//精神 37 勇氣
public class SPCom_37 : IConsumable {
  public void Consume( MapFightingUnit unit, MapFightingUnit useBy = null ) {
    unit.PilotInfo.Willpower = unit.PilotInfo.Willpower + 10;

    SPCommand spCom;

    spCom = DIContainer.Instance.PilotService.GetSPComByID( 1 ); //必中
    if (!unit.PilotInfo.ActiveSPCommandList.Any( s => s.ID == spCom.ID )) unit.PilotInfo.ActiveSPCommandList.Add( DIContainer.Instance.PilotService.GetSPComByID( spCom.ID ) );

    spCom = DIContainer.Instance.PilotService.GetSPComByID( 5 ); //不屈
    if (!unit.PilotInfo.ActiveSPCommandList.Any( s => s.ID == spCom.ID )) unit.PilotInfo.ActiveSPCommandList.Add( DIContainer.Instance.PilotService.GetSPComByID( spCom.ID ) );

    spCom = DIContainer.Instance.PilotService.GetSPComByID( 7 ); //集中
    if (!unit.PilotInfo.ActiveSPCommandList.Any( s => s.ID == spCom.ID )) unit.PilotInfo.ActiveSPCommandList.Add( DIContainer.Instance.PilotService.GetSPComByID( spCom.ID ) );

    spCom = DIContainer.Instance.PilotService.GetSPComByID( 9 ); //加速
    if (!unit.PilotInfo.ActiveSPCommandList.Any( s => s.ID == spCom.ID )) unit.PilotInfo.ActiveSPCommandList.Add( DIContainer.Instance.PilotService.GetSPComByID( spCom.ID ) );

    spCom = DIContainer.Instance.PilotService.GetSPComByID( 17 ); //努力
    if (!unit.PilotInfo.ActiveSPCommandList.Any( s => s.ID == spCom.ID )) unit.PilotInfo.ActiveSPCommandList.Add( DIContainer.Instance.PilotService.GetSPComByID( spCom.ID ) );

    spCom = DIContainer.Instance.PilotService.GetSPComByID( 3 ); //熱血
    if (!unit.PilotInfo.ActiveSPCommandList.Any( s => s.ID == spCom.ID )) unit.PilotInfo.ActiveSPCommandList.Add( DIContainer.Instance.PilotService.GetSPComByID( spCom.ID ) );
  }
}