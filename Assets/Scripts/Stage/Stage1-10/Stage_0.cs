using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DataModel.Service;
using UnityEngine.SceneManagement;
using System;
using static DataModel.Service.GameDataService;

public partial class Stage_0 : StageBase {

  /// <summary> 
  /// 0. 初始狀態  <br/>
  /// 1. 敵全滅 <br/></summary>
  public override int EventStatus { get; set; }
  public override List<int> EventList { get; set; } = new List<int>();

  override public void FirstTalk() {
    RobotInstance robot1 = robotService.CreateRobotInstance( 22, false, new List<PartsInstance>() {  // 10-鐮刀怪  11-Impulse  13-RZ-02  22-Jefty
      partsService.CreatePartsInstanceByPartsID( 1, false ),
      partsService.CreatePartsInstanceByPartsID( 2, false ),
      partsService.CreatePartsInstanceByPartsID( 3, false )
    } );

    partsService.CreatePartsInstanceByPartsID( 1, false );
    partsService.CreatePartsInstanceByPartsID( 1, false );
    partsService.CreatePartsInstanceByPartsID( 1, false );

    MapFightingUnit player1 = gameDataService.CreateMapFightingUnit( robot1, pilotService.CreatePilotInstance( 701, level: 1 ) );  // 701 伍索
    //MapFightingUnit player2 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstanceByRobotID( 1, true ), pilotService.CreatePilotInstanceByPilotID( 101, 40000, 1 ) );
    //MapFightingUnit player3 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstanceByRobotID( 10, true ), pilotService.CreatePilotInstanceByPilotID( 104, 0, 1 ) );
    //MapFightingUnit player4 = gameDataService.CreateMapFightingUnit( null, pilotService.CreatePilotInstanceByPilotID( 102, 0, 1 ) );

    //增加戰鬥單位至格納庫
    gameDataService.HouseUnits.Add( player1 );
    //gameDataService.MapFightingUnits.Add( player2 );
    //gameDataService.MapFightingUnits.Add( player3 );
    //gameDataService.MapFightingUnits.Add( player4 );

    MapFightingUnit enermy1 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 10, false ), pilotService.CreatePilotInstance( 702, isPlayer: false, level: 10 ) );
    MapFightingUnit enermy2_1 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 10, false ), pilotService.CreatePilotInstance( 9901, isPlayer: false, level: 5 ) ); //鐮刀 + AI
    MapFightingUnit enermy2_2 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 10, false ), pilotService.CreatePilotInstance( 9901, isPlayer: false, level: 5 ) ); //鐮刀 + AI
    MapFightingUnit enermy2_3 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 10, false ), pilotService.CreatePilotInstance( 9901, isPlayer: false, level: 5 ) ); //鐮刀 + AI
    MapFightingUnit enermy3 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 2, false ), pilotService.CreatePilotInstance( 102, isPlayer: false, level: 12 ) ); //多魯基斯+撒古斯

    stageManager.StageUnits.Add( stageManager.createUnit( "Player", new Vector3( -5, 0.1f, -5 ), new Vector3( 0, 0, 0 ), player1, UnitInfo.TeamEnum.Player, isNewToPlayer: true ) );
    //stageManager.MapFightingUnits_Player.Add( stageManager.createUnit( "Player", new Vector3( -9, 0.1f, -6 ), new Vector3( 0, 0, 0 ), player2, UnitInfo.TeamEnum.Player ) );
    //stageManager.MapFightingUnits_Player.Add( stageManager.createUnit( "Player", new Vector3( -13, 0.1f, -7 ), new Vector3( 0, 0, 0 ), player3, UnitInfo.TeamEnum.Player ) );

    stageManager.StageUnits.Add( stageManager.createUnit( "Enermy", new Vector3( 2, 0.1f, 2 ), new Vector3( 0, 225, 0 ), enermy1, UnitInfo.TeamEnum.Enermy ) );
    stageManager.StageUnits.Add( stageManager.createUnit( "Enermy", new Vector3( 14, 0.1f, 12 ), new Vector3( 0, 225, 0 ), enermy2_1, UnitInfo.TeamEnum.Enermy ) );   //鐮刀 + AI
    stageManager.StageUnits.Add( stageManager.createUnit( "Enermy", new Vector3( 19, 0.1f, 7 ), new Vector3( 0, 225, 0 ), enermy2_2, UnitInfo.TeamEnum.Enermy ) );   //鐮刀 + AI
    stageManager.StageUnits.Add( stageManager.createUnit( "Enermy", new Vector3( 15, 0.1f, 20 ), new Vector3( 0, 225, 0 ), enermy2_3, UnitInfo.TeamEnum.Enermy ) );   //鐮刀 + AI
    stageManager.StageUnits.Add( stageManager.createUnit( "Enermy", new Vector3( 38, 0.1f, 34 ), new Vector3( 0, 225, 0 ), enermy3, UnitInfo.TeamEnum.Enermy ) );  //多魯基斯+撒古斯

    //Temp test
    /*
    MapFightingUnit newPlayer = gameDataService.CreateMapFightingUnit(
                                  robotService.CreateRobotInstanceByRobotID( 12, true ),
                                  pilotService.CreatePilotInstanceByPilotID( 505, 40000, 1 )
                                );   //布拉度
    newPlayer.PilotInfo.Willpower += 20;
    gameDataService.MapFightingUnits.Add( newPlayer );   //增加戰鬥單位至格納庫
    stageManager.MapFightingUnits_Player.Add( stageManager.createUnit( "Player", new Vector3( -5, 0.1f, -5 ), new Vector3( 0, 0, 0 ), newPlayer, UnitInfo.TeamEnum.Player ) );
    */
    //Temp test

    initTalkBlock_1();

    stageManager.IsPlayerPhase = true;
    //stageManager.mapManager.enabled = true;

    //Just Test save, must comment out after
    //robotService.SaveActiveRobotInstance( 1 );

    gameDataService.AddMoney( 100 );
    //gameDataService.GameData.Chapter = 1;
    //gameDataService.GameData.Stage = 1;

    //SceneManager.UnloadSceneAsync( "stage_001" );
    //SceneManager.LoadScene( $"InterMission", LoadSceneMode.Additive );

    /*
    stageManager.mapStory.DoTalkList( talkBlockList[0], () => {   //測試時跳過對話
      stageManager.mapManager.BackToMap();
    } );
    */
    stageManager.mapManager.BackToMap();
  }

  override public void InitMapFightUnits() { 
  }

  /*
  public void AfterBattle() {
    //特殊對話, 事件, Flag
    //預留

    //處理爆炸, 移除被消滅單位
    SetBeDestroyed();
    if (destroyedUnit == null || noBoom( destroyedUnit )) DoExp( afterEvent );
    else stageManager.DestroyUnit( destroyedUnit.GetComponent<UnitController>(), () => { DoExp( afterEvent ); } );
  }
  */

  //執行本關獨有特殊事件
  public override void DoEvent() {
    //Debug.Log( "doEvent()" );
    //stageManager.mapManager.myMapCursor.SetActive( false );  //好像沒有用, 要研究

    if (destroyPendingList.Count > 0) {   //存在 HP 歸0 仍未爆炸的unit, 執行 event
      var unitInfo = destroyPendingList[0];
      destroyPendingList.RemoveAt( 0 );

      if (unitInfo.PilotInfo.PilotInstance.PilotID == 702 && EventStatus == 0) {  //我方增援1, 克羅那撤退
        event1( unitInfo );
        return;
      }
    }

    if (stageManager.StageUnits_Enemy.Count< 1) {
      gameDataService.StoryPhase = StoryPhaseEnum.After;  //After

      initTalkBlock_8();
      stageManager.mapStory.DoTalkList( talkBlockList[0], () => {
        //stageManager.mapManager.myMapCursor.GetComponent<Cursor>().enabled = false;
        //stageManager.mapManager.enabled = false;

        GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeOut( 2f, () => {
          SceneManager.UnloadSceneAsync( "stage_001" );
          //SceneManager.LoadScene( $"InterMission", LoadSceneMode.Additive );
          SceneManager.LoadScene( $"Story", LoadSceneMode.Additive );
        } );
      } );
      return;
    }

    //特殊事件後 基本處理流程
    afterEvent();
  }

  void event1( UnitInfo unitInfo ) {   //味方增援, 克羅那撤退
    EventStatus = 1;
    initTalkBlock_2();
    stageManager.mapStory.DoTalkList( talkBlockList[0], () => {
      //stageManager.mapStory.moveCursorAndLookAt( new Vector3( -4, 0.1f, -32 ) );

      PreReinforces( new List<ReinforceParam>() {
        //new OurReinforceParam( 12, true, 505, 40000, 1, true, 20, "Player", new Vector3( -4, 0.1f, -32 ), new Vector3( 0, 0, 0 ), UnitInfo.TeamEnum.Player ),
        //new OurReinforceParam( 1, true, 101, 40000, 1, true, 0, "Player", new Vector3( -9, 0.1f, -6 ), new Vector3( 0, 0, 0 ), UnitInfo.TeamEnum.Player ),
        //new OurReinforceParam( 10, true, 104, 38000, 1, true, 0, "Player", new Vector3( -13, 0.1f, -7 ), new Vector3( 0, 0, 0 ), UnitInfo.TeamEnum.Player )
      }, 
      () => {
        initTalkBlock_7();
        stageManager.mapStory.DoTalkList( talkBlockList[0], () => {
          destroyPendingList.Remove( unitInfo );
          stageManager.UnitLeave( unitInfo.GetComponent<UnitController>(), DoEvent );
        } );
      } );
    } );
  }

  //判斷是否免爆炸
  protected override bool noBoom( UnitInfo unitInfo ) {
    if (unitInfo.PilotInfo.PilotInstance.PilotID == 702 && EventStatus == 0) return true;    //克羅那

    return false;
  }

}
