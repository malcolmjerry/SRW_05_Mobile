using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DataModel.Service;
using UnityEngine.SceneManagement;
using System;
using static DataModel.Service.GameDataService;
using System.Linq;

public partial class Stage_2 : StageBase {

  /// <summary> 
  /// 0. 初始狀態 <br/>
  /// 1. 敵增援1 <br/>
  /// 2. 味方增援1 <br/>
  /// </summary>
  public override int EventStatus { get; set; }

  /// <summary> 
  /// 0. Boss  0: 未消滅, 1: 已消滅, 2: 已執行消滅程序 <br/>
  /// 1. Boss戰鬥次數 <br/>
  /// 2. 味方增援的預計回合 <br/>
  /// 3. 初始敵人數量 <br/>
  /// </summary>
  public override List<int> EventList { get; set; } = new List<int>() { 0, 0, -1, 6 };

  public override string PlayerBGM { get; set; } = "AL_Mobile Suit Battle War";
  public override string EnemyBGM { get; set; } = "F_ジェノサイドマシーン";
  public override string TerrainName { get; set; } = "TaiwanSeaTerrain";
  public override string BattleTerrainName { get; set; } = "TaiwanSeaTerrain";

  override public void FirstTalk() {
    initTalkBlock_startMap();

    stageManager.IsPlayerPhase = true;
    stageManager.mapStory.DoTalkList( talkBlock, DoEvent );  //測試時跳過對話

    // 測試時直接跳到整備畫面
    //DIContainer.Instance.GameDataService.AddMoney( 200000 );
    //SceneManager.UnloadSceneAsync( "stage_001" );
    //SceneManager.LoadScene( $"InterMission", LoadSceneMode.Additive );
    //
  }

  override public void InitMapFightUnits() {
    // 3: 元祖  1: WZ_EW  2: T1  17: AZ_0  16: RZ_0(Unlimited)  4: GM2  5: GM3  7: Zaku  26: 百式
    RobotInstance robot1 = robotService.CreateRobotInstance( 3, false, new List<PartsInstance>() {  
      partsService.CreatePartsInstanceByPartsID( 7, false ),  //Booster
      partsService.CreatePartsInstanceByPartsID( 9, false )  //高性能雷達
    } );

    MapFightingUnit player1 = gameDataService.CreateMapFightingUnit( robot1, pilotService.CreatePilotInstance( 204, level: 10 ) );  // 204 馬克思
    player1.PilotInfo.Willpower += 10;

    RobotInstance robot2 = robotService.CreateRobotInstance( 26, false, new List<PartsInstance>() {  // 3 元祖高達   11 V高達    26: 百式
      partsService.CreatePartsInstanceByPartsID( 24, false ),  //精密照準鏡
      partsService.CreatePartsInstanceByPartsID( 10, false )  //對 Beam Coating
    } );

    MapFightingUnit player2 = gameDataService.CreateMapFightingUnit( robot2, pilotService.CreatePilotInstance( 405, level: 10 ) );  //Robot 11: V高達  405: 小英

    //增加戰鬥單位至格納庫
    //gameDataService.HouseUnits.Add( player1 );
    //gameDataService.HouseUnits.Add( player2 );

    stageManager.StageUnits.Add( stageManager.createUnit( "Player", RespawnPoints[0], player1, UnitInfo.TeamEnum.Player, isNewToPlayer: true ) );
    //stageManager.StageUnits.Add( stageManager.createUnit( "Player", RespawnPoints[1], player2, UnitInfo.TeamEnum.Player ) );
    stageManager.StageUnits.Add( stageManager.createUnit( "Player", RespawnPoints[1].position, RespawnPoints[1].eulerAngles, player2, UnitInfo.TeamEnum.Player, isNewToPlayer: true ) );
    stageManager.StageUnits.Add( stageManager.createUnit( "Player", RespawnPoints[2], gameDataService.HouseUnitsFightables[0], UnitInfo.TeamEnum.Player ) );
    stageManager.StageUnits.Add( stageManager.createUnit( "Player", RespawnPoints[3], gameDataService.HouseUnitsFightables[1], UnitInfo.TeamEnum.Player ) );
    
    for (int i = 5; i <= 9; i++) {
      MapFightingUnit enermy = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 10, false ), pilotService.CreatePilotInstance( 9935, isPlayer: false, level: 20 ) ); //10: 鐮刀 + 義和團  2: T1
      stageManager.StageUnits.Add( stageManager.createUnit( "Enermy", RespawnPoints[i], enermy, UnitInfo.TeamEnum.Enermy, remainMoveAttackTurns: 0 ) );   //鐮刀 + 義和團
    }

    MapFightingUnit boss = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 13, false ), pilotService.CreatePilotInstance( 307, isPlayer: false, level: 20 ) );  //13: RZ-02(black) + 慈禧
    boss.SetAIBuff( 14000, 100 );
    stageManager.StageUnits.Add( stageManager.createUnit( "Enermy", RespawnPoints[10], boss, UnitInfo.TeamEnum.Enermy, remainMoveAttackTurns: 5,
        dropParts: new List<int>() { 6 } ) ); //Hybrid裝甲 HP+1000 裝甲+100
  }

  //執行本關獨有特殊事件
  public override void DoEvent() {
    //Debug.Log( "doEvent()" );
    //(敵軍數量減至一半 / 與慈禧太后戰鬥一次) && 慈禧未消滅 -> 敵軍增援1
    if ((stageManager.StageUnits_Enemy.Count <= (EventList[3] / 2f) || EventList[1] >= 1) && EventStatus == 0 && EventList[0] == 0) {  
      if (objDataList.Count > 1) {
        ObjectiveIndex = 1;
        mapManager.ObjectiveBox.GetComponent<ObjectiveCtl>().SetData( objDataList[ObjectiveIndex], true );
      }
      event1();
      return;
    }

    // Boss被消滅, 執行消滅程序/對話 (->狀態2), 回到refreshMap 會爆炸 (Boss消滅狀態 < 2 時不爆)
    if (EventList[0] == 1) {
      bossDestroyed();
      return;
    }

    //敵軍增援1 後下一個我方回合/ 或與慈禧戰鬥三次 / 或慈禧被消滅 -> 味方增援1
    if ((EventList[1] >= 3  || EventList[0] == 2 || gameDataService.GameData.Turns == EventList[2]) && EventStatus == 1) {  
      event2();
      return;
    }

    //特殊事件後 基本處理流程
    afterEvent();
  }

  void event1() {  // 敵增援1
    EventStatus = 1;
    EventList[2] = gameDataService.GameData.Turns + 1;

    initTalkBlock_event1();    //敵增援1

    List<ReinforceParam> reinList = new List<ReinforceParam>();

    for (int i = 11; i <= 15; i++) {
      MapFightingUnit enermy = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 2, false ), pilotService.CreatePilotInstance( 9932, isPlayer: false, level: 20 ) ); //4: GM2 + 大清帝國兵 //2 :T1
      reinList.Add( new ReinforceParam( enermy, 0, "Enermy", RespawnPoints[i].position, RespawnPoints[i].eulerAngles, UnitInfo.TeamEnum.Enermy ) );
    }

    BGMController.SET_BGM2( "2G_EnermysCome", stageManager.IsPlayerPhase? PlayerBGM : EnemyBGM );
    PreReinforces( reinList, () => {
      stageManager.mapStory.DoTalkList( talkBlock, DoEvent );
    } );

  }

  /// <summary> 味方增援1 </summary>
  void event2() { 
    EventStatus = 2;

    PilotInstance heroPilotInstance = pilotService.LoadHeroPilotInstance( 2 );
    RobotInstance robot = robotService.CreateRobotInstance( 17, false, new List<PartsInstance>() {  // 17: AZ-0
      partsService.CreatePartsInstanceByPartsID( 1, false ),  //Chobham裝甲 HP+500; 裝甲+150
      partsService.CreatePartsInstanceByPartsID( 2, false )   //磁層關節 運動性+10
    } );
    MapFightingUnit heroUnit = gameDataService.CreateMapFightingUnit( robot, heroPilotInstance ); //AZ-0 + 主角2
    heroUnit.PilotInfo.AddExp( 8000 );
    heroUnit.UpdateInit();

    BGMController.SET_BGM( "2G_FriendsCome" );
    PreReinforces( new List<ReinforceParam>() {
      new ReinforceParam( heroUnit, 25, "Player", RespawnPoints[4].position, RespawnPoints[4].localEulerAngles, UnitInfo.TeamEnum.Player, isJoinUs: true ),
    }, () => {
      initTalkBlock_event2();
      stageManager.mapStory.DoTalkList( talkBlock, DoEvent );
    } );
  }

  void bossDestroyed() {   //Boss被擊倒
    //EventStatus = 3;
    EventList[0] = 2;
    initTalkBlock_bossDestroyed();
    stageManager.mapStory.DoTalkList( talkBlock, refreshMap );
  }

  //判斷是否免爆炸
  protected override bool noBoom( UnitInfo unitInfo ) {
    if (unitInfo.PilotInfo.PilotInstance.PilotID == 慈禧.ID && EventList[0] < 2) {
      //EventList[0] = 1;
      return true;    //慈禧
    }

    return false;
  }

  protected override void updateEventList( List<UnitInfo> destroyPendingList, AttackData attData ) {
    if (destroyPendingList.Any( p => p.PilotInfo.PilotInstance.PilotID == 慈禧.ID ))
      EventList[0] = 1;

    if (attData.ToUnitInfo.PilotInfo.PilotInstance.PilotID == 慈禧.ID || attData.FromUnitInfo.PilotInfo.PilotInstance.PilotID == 慈禧.ID)
      EventList[1]++;
  }

  public override bool CheckGameOver() {
    if (base.CheckGameOver())
      return true;

    var result = stageManager.StageUnits.Any( u => u.GetComponent<UnitInfo>().IsDestroyed && u.GetComponent<UnitInfo>().Team == UnitInfo.TeamEnum.Player );
    return result;
  }

}
