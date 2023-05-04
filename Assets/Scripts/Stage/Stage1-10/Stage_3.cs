using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DataModel.Service;
using UnityEngine.SceneManagement;
using System;
using static DataModel.Service.GameDataService;
using System.Linq;

public partial class Stage_3 : StageBase {

  /// <summary> 
  /// 0. Boss  0: 未消滅, 1: 已消滅或達到撤退條件, 2: 已執行撤退程序  3: 撤退後會話已執行 <br/>
  /// </summary>
  public override List<int> EventList { get; set; } = new List<int>() { 0, 0, -1, 6 };

  public override string PlayerBGM { get; set; } = "F_PowerAndSkill";
  public override string EnemyBGM { get; set; } = "F_Heartful Mechanic"; //"AL_それぞれの大義のために"; stage4
  public override string TerrainName { get; set; } = "WashingtonDCTerrain";
  public override string BattleTerrainName { get; set; } = "WashingtonDCTerrain";

  override public void FirstTalk() {
    initTalkBlock_startMap();

    stageManager.IsPlayerPhase = true;
    stageManager.mapStory.DoTalkList( talkBlock, refreshMap );  //測試時跳過對話
  }

  override public void InitMapFightUnits() {
    //foreach (Transform rp in GameObject.Find( "RespawnPoints" ).transform)
      //RespawnPoints.Add( rp );

    // 3: 元祖  1: WZ_EW  2: T1  17: AZ_0  16: RZ_0(Unlimited)  4: GM2  5: GM3  7: Zaku  26: 百式
    RobotInstance robot1 = robotService.CreateRobotInstance( 1, false, new List<PartsInstance>() {  
      partsService.CreatePartsInstanceByPartsID( 15, false ),  //生物感應器
      partsService.CreatePartsInstanceByPartsID( 23, false )  //補助GS
    } );

    MapFightingUnit player1 = gameDataService.CreateMapFightingUnit( robot1, pilotService.CreatePilotInstance( 107, level: 10 ) );  // 107 侵侵
    //player1.PilotInfo.Willpower += 20;

    RobotInstance robot2 = robotService.CreateRobotInstance( 22, false, new List<PartsInstance>() {  // 22: Jefty
      partsService.CreatePartsInstanceByPartsID( 1, false ),  //Chobham 裝甲
      partsService.CreatePartsInstanceByPartsID( 10, false )  //對 Beam Coating
    } );

    MapFightingUnit player2 = gameDataService.CreateMapFightingUnit( robot2, pilotService.CreatePilotInstance( 113, level: 10 ) );  //113: 肥蓬

    RobotInstance robot3 = robotService.CreateRobotInstance( 12, false, new List<PartsInstance>() {  //12: 白色基地
      partsService.CreatePartsInstanceByPartsID( 7, false ),  //booster
      partsService.CreatePartsInstanceByPartsID( 4, false )   //分身發生器
    } );
    MapFightingUnit player3 = gameDataService.CreateMapFightingUnit( robot3, pilotService.CreatePilotInstance( 112, level: 10 ) );  //112: 彭斯

    stageManager.StageUnits.Add( stageManager.createUnit( "Player", RespawnPoints[0], player1, UnitInfo.TeamEnum.Player, isNewToPlayer: true ) );
    stageManager.StageUnits.Add( stageManager.createUnit( "Player", RespawnPoints[1], player2, UnitInfo.TeamEnum.Player, isNewToPlayer: true ) );
    stageManager.StageUnits.Add( stageManager.createUnit( "Player", RespawnPoints[2], player3, UnitInfo.TeamEnum.Player, isNewToPlayer: true ) );

    for (int i = 3; i <= 10; i++) {
      MapFightingUnit enermy = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 4, false ), pilotService.CreatePilotInstance( 9904, isPlayer: false, level: 20 ) ); //4: GM2 + 海依思
      stageManager.StageUnits.Add( stageManager.createUnit( "Enermy", RespawnPoints[i], enermy, UnitInfo.TeamEnum.Enermy, remainMoveAttackTurns: 0 ) );
    }

    MapFightingUnit boss = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 18, false ), pilotService.CreatePilotInstance( 108, isPlayer: false, level: 22 ) );  //13: DevilRobot + 拜登
    stageManager.StageUnits.Add( stageManager.createUnit( "Enermy", RespawnPoints[11], boss, UnitInfo.TeamEnum.Enermy, remainMoveAttackTurns: 10 ) );
    boss.SetAIBuff( AI_HP: 41000, willPower: 5 );

    MapFightingUnit enemy1 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 11, false ), pilotService.CreatePilotInstance( 114, isPlayer: false, level: 20 ) );  //13: Impulse + 賀錦麗
    stageManager.StageUnits.Add( stageManager.createUnit( "Enermy", RespawnPoints[12], enemy1, UnitInfo.TeamEnum.Enermy, remainMoveAttackTurns: 7, dropParts: new List<int>() { 16 } ) );  //賽可膠骨架 - 運動性+25

    MapFightingUnit enemy2 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 2, false ), pilotService.CreatePilotInstance( 9906, isPlayer: false, level: 20 ) );  //2: T1 + 光明會親衛隊
    stageManager.StageUnits.Add( stageManager.createUnit( "Enermy", RespawnPoints[13], enemy2, UnitInfo.TeamEnum.Enermy, remainMoveAttackTurns: 7, dropParts: new List<int>() { 8 } ) ); //大型推進器-移動力+2

    MapFightingUnit enemy3 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 2, false ), pilotService.CreatePilotInstance( 9905, isPlayer: false, level: 20 ) );  //2: T1 + 共濟會特種兵
    stageManager.StageUnits.Add( stageManager.createUnit( "Enermy", RespawnPoints[14], enemy3, UnitInfo.TeamEnum.Enermy, remainMoveAttackTurns: 7, dropParts: new List<int>() { 22 } ) ); //大型 Generator - EN + 120

    MapFightingUnit enemy4 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 2, false ), pilotService.CreatePilotInstance( 9905, isPlayer: false, level: 20 ) );  //2: T1 + 共濟會特種兵
    stageManager.StageUnits.Add( stageManager.createUnit( "Enermy", RespawnPoints[15], enemy4, UnitInfo.TeamEnum.Enermy, remainMoveAttackTurns: 7, dropParts: new List<int>() { 18 } ) );  // 高性能照準器
  }

  //執行本關獨有特殊事件
  public override void DoEvent() {
    // Boss被消滅或符合撤退條件, 執行敵軍程序/對話 (狀態1)
    if (EventList[0] == 1) {
      bossDestroyed();
      return;
    }

    // 敵軍已撤退 (狀態2)
    if (EventList[0] == 2) {
      initTalkBlock_AfterBossRun();
      stageManager.mapStory.DoTalkList( talkBlock, refreshMap );  //可考慮不去refreshMap, 直接遞歸 DoEvent
      EventList[0] = 3;
      return;
    }

    //沒有事件命中, 執行事件後流程
    afterEvent();
  }

  void bossDestroyed() {   //Boss被擊倒 或 HP 少於 50%
    initTalkBlock_bossDestroyed();
    EventList[0] = 2;
    stageManager.mapStory.DoTalkList( talkBlock, () => {
      stageManager.UnitsLeave( stageManager.StageUnits_Enemy, DoEvent );
    } );
  }

  //判斷是否免爆炸
  protected override bool noBoom( UnitInfo unitInfo ) {
    if (unitInfo.PilotInfo.PilotInstance.PilotID == 拜登.ID) {
      return true;    //拜登
    }

    return false;
  }

  protected override void updateEventList( List<UnitInfo> destroyPendingList, AttackData attData ) {
    //if (destroyPendingList.Any( p => p.PilotInfo.PilotInstance.PilotID == 拜登.ID )) { 
      //EventList[0] = 1;

    if (stageManager.StageUnitsInfo.Any( u => u.PilotInfo.PilotInstance.PilotID == 拜登.ID && u.RobotInfo.HP < u.RobotInfo.MaxHP / 2 ))
      EventList[0] = 1;
  }

  public override bool CheckGameOver() {
    if (base.CheckGameOver())
      return true;

    var result = stageManager.StageUnits.Any( u => u.GetComponent<UnitInfo>().IsDestroyed && u.GetComponent<UnitInfo>().Team == UnitInfo.TeamEnum.Player );
    return result;
  }

}
