using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DataModel.Service;
using UnityEngine.SceneManagement;
using System;
using static DataModel.Service.GameDataService;
using System.Linq;

public partial class Stage_4 : StageBase {

  /// <summary> 
  /// 0. 慈禧  0: 未消滅, 1: 已消滅或達到撤退條件, 2: 已執行消滅或撤退程序  3: 撤退後會話已執行 <br/>
  /// 1. 山本  0: 未消滅, 1: 已消滅或達到撤退條件, 2: 已執行消滅或撤退程序  3: 撤退後會話已執行 <br/>
  /// 2. 初始敵人數量 <br/>
  /// 3. 敵增援  0: 未出現  1: 增援1 已出現
  /// </summary>
  public override List<int> EventList { get; set; } = new List<int>() { 0, 0, 19, 0 };

  public override string PlayerBGM { get; set; } = "F_To the Distant Place";
  public override string EnemyBGM { get; set; } = "AL_それぞれの大義のために"; //"AL_それぞれの大義のために"; stage4
  public override string TerrainName { get; set; } = "TaiwanNoSeaTerrain";
  public override string BattleTerrainName { get; set; } = "TaiwanSeaTerrain";

  override public void FirstTalk() {
    initTalkBlock_startMap();

    stageManager.IsPlayerPhase = true;
    stageManager.mapStory.DoTalkList( talkBlock, refreshMap );  //測試時跳過對話
  }

  override public void InitMapFightUnits() {
    /*
    var terrainGo = GameObject.Find( "Terrain" );
    if (terrainGo) {
      terrainGo.GetComponent<RefreshChild>().enabled = false;
      CoroutineCommon.CallWaitForOneFrame( () => {
        terrainGo.transform.Find( "Sea" ).gameObject.SetActive( false );
        Debug.Log( $"{terrainGo.name} set Active(false)" );
      } );
    }
    */

    var theShip = gameDataService.HouseUnits.Where( h => h.RobotInfo.RobotInstance.Robot.IsShip == true ).FirstOrDefault();
    var units = gameDataService.HouseUnits.Where( h => h.RobotInfo.RobotInstance.Robot.IsShip != true ).ToList();

    stageManager.StageUnits.Add( stageManager.createUnit( "Player", RespawnPoints[0], theShip, UnitInfo.TeamEnum.Player ) );

    for (int i=0; i<7; i++) {
      if (i == units.Count)
        break;

      stageManager.StageUnits.Add( stageManager.createUnit( "Player", RespawnPoints[i+1], units[i], UnitInfo.TeamEnum.Player ) );
    }
    

    for (int i = 8; i <= 16; i++) {
      MapFightingUnit enermy = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 4, false ), pilotService.CreatePilotInstance( 9935, isPlayer: false, level: 35 ) ); //4: GM2 + 義和團
      enermy.SetAIBuff( AI_HP: 2000, AI_Motility: 5, AI_Armor: 100, AI_HitRate: 0 );
      stageManager.StageUnits.Add( stageManager.createUnit( "Enermy", RespawnPoints[i], enermy, UnitInfo.TeamEnum.Enermy, remainMoveAttackTurns: 0 ) );
    }

    for (int i = 17; i <= 24; i++) {
      MapFightingUnit enermy = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 10, false ), pilotService.CreatePilotInstance( 9932, isPlayer: false, level: 35 ) ); //4: 鐮刀怪 + 大清帝國兵
      enermy.SetAIBuff( AI_HP: 2000, AI_Motility: 10, AI_Armor: 200, AI_HitRate: 15 );
      stageManager.StageUnits.Add( stageManager.createUnit( "Enermy", RespawnPoints[i], enermy, UnitInfo.TeamEnum.Enermy, remainMoveAttackTurns: 0 ) );
    }

    MapFightingUnit boss1 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 13, false ), pilotService.CreatePilotInstance( 307, isPlayer: false, level: 34 ) );  //13: RZ-02(black) + 慈禧
    boss1.SetAIBuff( 43600, 150, 0, 200, 21, 0, 0, 200 );
    stageManager.StageUnits.Add( stageManager.createUnit( "Enermy", RespawnPoints.Where( t => t.name == "Boss1" ).FirstOrDefault(), 
      boss1, UnitInfo.TeamEnum.Enermy, remainMoveAttackTurns: 8, dropParts: new List<int>() { 17 } ) ); //化地瑪

    MapFightingUnit boss2 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 18, false ), pilotService.CreatePilotInstance( 604, isPlayer: false, level: 34 ) );  //18: 魔龍將軍 + 山本五十六
    boss2.SetAIBuff( 23767, 15, 10, 100, 14, 0, 0, 200 );
    stageManager.StageUnits.Add( stageManager.createUnit( "Enermy", RespawnPoints.Where( t => t.name == "Boss2" ).FirstOrDefault(),
      boss2, UnitInfo.TeamEnum.Enermy, remainMoveAttackTurns: 5, dropParts: new List<int>() { 19 } ) ); //Halo
  }

  //執行本關獨有特殊事件
  public override void DoEvent() {
    // Boss2 (山本) 被消滅或符合撤退條件, 執行敵軍程序/對話 (狀態1)
    if (EventList[1] == 1) {
      initTalkBlock_Boss2Defeated();
      EventList[1] = 2;
      stageManager.mapStory.DoTalkList( talkBlock, refreshMap );
      return;
    }

    // 慈禧被消滅或符合撤退條件, 執行敵軍程序/對話 (狀態1)
    if (EventList[0] == 1) {
      initTalkBlock_BossDefeated();
      EventList[0] = 2;
      stageManager.mapStory.DoTalkList( talkBlock, refreshMap );
      return;
    }

    // 慈禧已爆炸 (狀態2) 且敵全滅
    if (EventList[0] == 2 && stageManager.StageUnits_Enemy.Count <= 0) {
      initTalkBlock_AfterBossDefeated();
      stageManager.mapStory.DoTalkList( talkBlock, refreshMap ); 
      EventList[0] = 3;
      return;
    }

    //敵機數量少於一半, 敵增援1
    if ((stageManager.StageUnits_Enemy.Count <= (EventList[2] / 2f) && EventList[3] == 0)) {
      if (objDataList.Count > 1) {
        ObjectiveIndex = 1;
        mapManager.ObjectiveBox.GetComponent<ObjectiveCtl>().SetData( objDataList[ObjectiveIndex], true );
      }
      enermyRein1();
      return;
    }

    //沒有事件命中, 執行事件後流程
    afterEvent();
  }

  void enermyRein1() {  // 敵增援1
    EventList[3] = 1;

    initTalkBlock_EnemyReinforce1();    //敵增援1

    List<ReinforceParam> reinList = new List<ReinforceParam>();

    var zeroPoints = RespawnPoints.Where( rp => rp.name.Contains( "Zero" ) ).ToList();

    MapFightingUnit enemyBoss = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 14, false ),  //零戰21
      pilotService.CreatePilotInstance( 606, isPlayer: false, level: 34 ) ); //606 宮部久藏 
    enemyBoss.SetAIBuff( 17000, 0, 0, 200, 21, 0, 0, 200 );
    reinList.Add( new ReinforceParam( enemyBoss, 0, "Enermy", zeroPoints[0].position, zeroPoints[0].eulerAngles, UnitInfo.TeamEnum.Enermy, dropParts: new List<int>() { 14 } ) );  //Apogee Motor


    for (int i = 1; i < zeroPoints.Count; i++) {
      var zeroPoint = zeroPoints[i];
      MapFightingUnit enemy = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 14, false ),   //零戰21
        pilotService.CreatePilotInstance( 9903, isPlayer: false, level: 35 ) ); //9903 :自律回路
      enemy.SetAIBuff( 5000, 0, 0, 200, 21, 0, 0, 200 );
      reinList.Add( new ReinforceParam( enemy, 0, "Enermy", zeroPoint.position, zeroPoint.eulerAngles, UnitInfo.TeamEnum.Enermy ) );
    }

    BGMController.SET_BGM2( "2G_EnermysCome", stageManager.IsPlayerPhase ? PlayerBGM : EnemyBGM );
    PreReinforces( reinList, () => {
      stageManager.mapStory.DoTalkList( talkBlock, DoEvent );
    } );

  }

  //判斷是否免爆炸
  protected override bool noBoom( UnitInfo unitInfo ) {
    if (unitInfo.PilotInfo.PilotInstance.PilotID == 山本五十六.ID && EventList[0] < 2) {
      return true; 
    }

    if (unitInfo.PilotInfo.PilotInstance.PilotID == 慈禧.ID && EventList[0] < 2) {
      return true;
    }

    return false;
  }

  protected override void updateEventList( List<UnitInfo> destroyPendingList, AttackData attData ) {
    if (destroyPendingList.Any( u => u.PilotInfo.PilotInstance.PilotID == 山本五十六.ID))
      EventList[1] = 1;

    if (destroyPendingList.Any( u => u.PilotInfo.PilotInstance.PilotID == 慈禧.ID ))
      EventList[0] = 1;
  }

}
