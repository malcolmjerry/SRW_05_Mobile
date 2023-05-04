using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DataModel.Service;
using UnityEngine.SceneManagement;
using System;
using static DataModel.Service.GameDataService;
using System.Linq;

public partial class Stage_1 : StageBase {

  /// <summary> 
  /// 0. 初始狀態  <br/>
  /// 1. 第一次敵全滅: 敵增援1 + 味方增援1  <br/>
  /// 2. 第二次敵全滅: 黑洞出現 + 對話  <br/> 
  /// 3. 黑洞再次擴大 + 對話 <br/></summary>
  public override int EventStatus { get; set; }
  public override List<int> EventList { get; set; } = new List<int>();

  public GameObject BlackHolePrefab;
  public Transform BlackHolePoint;
  private GameObject blackHole;

  public override string PlayerBGM { get; set; } = "2G_PlayerPhaseSpace";
  public override string EnemyBGM { get; set; } = "2G_EnermyPhase";
  public override string TerrainName { get; set; } = "ForbiddenCityTerrain";
  public override string BattleTerrainName { get; set; } = "ForbiddenCityTerrain";

  override public void FirstTalk() {
    initTalkBlock_1();

    stageManager.IsPlayerPhase = true;
    //gameDataService.AddMoney( 100 );

    stageManager.mapStory.DoTalkList( talkBlockList[0], () => {   //測試時跳過對話
      stageManager.HandlePhase();
    } );
    
    //stageManager.mapStory.DoTalkList( talkBlockList[0], DoEvent );  
    //stageManager.HandlePhase();

    // 測試時直接跳到整備畫面
    //DIContainer.Instance.GameDataService.AddMoney( 200000 );
    //SceneManager.UnloadSceneAsync( "stage_001" );
    //SceneManager.LoadScene( $"InterMission", LoadSceneMode.Additive );
    //
  }

  override public void InitMapFightUnits() {
    // 1: WZ_EW  2: T1  3: 元祖  17: AZ_0  16: RZ_0(Unlimited)  4: GM2  5: GM3  7: Zaku  9: 鐵甲  13: RZ-02  26: 百式  14: Zero21  18: 魔龍  17: AZ-0
    RobotInstance robot1 = robotService.CreateRobotInstance( 9, false, new List<PartsInstance>() { 
      partsService.CreatePartsInstanceByPartsID( 1, false ),
      partsService.CreatePartsInstanceByPartsID( 2, false ),
      partsService.CreatePartsInstanceByPartsID( 3, false )
    } );

    //生成強化部件 放於倉庫
    //partsService.CreatePartsInstanceByPartsID( 1, true );
    //partsService.CreatePartsInstanceByPartsID( 1, true );
    //partsService.CreatePartsInstanceByPartsID( 1, true );

    MapFightingUnit player1 = gameDataService.CreateMapFightingUnit( robot1, pilotService.CreatePilotInstance( 1307, isPlayer: false, level: 1 ) );  // 1307 中山樵  113 肥蓬
    player1.PilotInfo.Willpower += 0;
    MapFightingUnit player2 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 7, false ), pilotService.CreatePilotInstance( 9931 ) );  //7: 渣古  16: 主角(測試)
    player2.PilotInfo.Willpower += 0; //測試
    MapFightingUnit player3 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 7, false ), pilotService.CreatePilotInstance( 9931 ) );
    //MapFightingUnit player4 = gameDataService.CreateMapFightingUnit( null, pilotService.CreatePilotInstanceByPilotID( 102, 0, 1 ) ); //只增加駕駛員到倉庫, 但沒有機體

    //增加戰鬥單位至格納庫
    //gameDataService.HouseUnits.Add( player1 );
    //gameDataService.HouseUnits.Add( player2 );  //正式時不加
    //gameDataService.HouseUnits.Add( player3 );  //正式時不加
    //gameDataService.MapFightingUnits.Add( player4 );
    stageManager.StageUnits.Add( stageManager.createUnit( "Player", RespawnPoints[0], player1, UnitInfo.TeamEnum.Player, isNewToPlayer: true ) );  
    stageManager.StageUnits.Add( stageManager.createUnit( "Player", RespawnPoints[1].position, new Vector3( 0, 135, 0 ), player2, UnitInfo.TeamEnum.Player ) );
    stageManager.StageUnits.Add( stageManager.createUnit( "Player", RespawnPoints[2].position, new Vector3( 0, 135, 0 ), player3, UnitInfo.TeamEnum.Player ) );

    MapFightingUnit enermy2_1 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 10, false ), pilotService.CreatePilotInstance( 9935, isPlayer: false, level: 10 ) ); //鐮刀 + 義和團
    MapFightingUnit enermy2_2 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 10, false ), pilotService.CreatePilotInstance( 9935, isPlayer: false, level: 10 ) ); //鐮刀 + 義和團
    MapFightingUnit enermy2_3 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 10, false ), pilotService.CreatePilotInstance( 9935, isPlayer: false, level: 10 ) ); //鐮刀 + 義和團

    //stageManager.MapFightingUnits.Add( stageManager.createUnit( "Enermy", new Vector3( 2, 0.1f, 2 ), new Vector3( 0, 225, 0 ), enermy1, UnitInfo.TeamEnum.Enermy ) );
    stageManager.StageUnits.Add( stageManager.createUnit( "Enermy", RespawnPoints[3].position/*new Vector3( -28, 0.1f, -6 )*/, new Vector3( 0, 315, 0 ), enermy2_1, UnitInfo.TeamEnum.Enermy ) );   //鐮刀 + 義和團
    stageManager.StageUnits.Add( stageManager.createUnit( "Enermy", RespawnPoints[4].position/*new Vector3( -20, 0.1f, -1 )*/, new Vector3( 0, 315, 0 ), enermy2_2, UnitInfo.TeamEnum.Enermy ) );   //鐮刀 + 義和團
    stageManager.StageUnits.Add( stageManager.createUnit( "Enermy", RespawnPoints[5].position/*new Vector3( -14, 0.1f, 5 )*/, new Vector3( 0, 315, 0 ), enermy2_3, UnitInfo.TeamEnum.Enermy ) );   //鐮刀 + 義和團

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
  }

  //執行本關獨有特殊事件
  public override void DoEvent() {
    //Debug.Log( "doEvent()" );
    if (stageManager.StageUnits_Enemy.Count < 1 && EventStatus == 0) {  //初始狀態 -> 第一次敵全滅
      if (objDataList.Count > 1) {
        ObjectiveIndex = 1;
        mapManager.ObjectiveBox.GetComponent<ObjectiveCtl>().SetData( objDataList[ObjectiveIndex], true );
      }
      event1();
      return;
    }

    if (stageManager.StageUnits_Enemy.Count < 1 && EventStatus == 1) {  //第一次敵全滅 -> 第二次敵全滅, 黑洞出現
      event2();
      return;
    }

    if (EventStatus == 2) {  //第二次敵全滅, 黑洞出現 -> 黑洞再次擴大
      event3();
      return;
    }

    //特殊事件後 基本處理流程
    afterEvent();
  }

  void event1() {  //第一次敵全滅: 敵增援1 + 味方增援1
    EventStatus = 1;      
    initTalkBlock_2();    //敵增援多數出現
    stageManager.mapStory.DoTalkList( talkBlockList[0], () => {
      MapFightingUnit enermy2_1 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 4, false ), pilotService.CreatePilotInstance( 9932, isPlayer: false, level: 10 ) ); //GM2 + 大清帝國兵
      MapFightingUnit enermy2_2 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 4, false ), pilotService.CreatePilotInstance( 9932, isPlayer: false, level: 10 ) ); //GM2 + 大清帝國兵
      MapFightingUnit enermy2_3 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 4, false ), pilotService.CreatePilotInstance( 9932, isPlayer: false, level: 10 ) ); //GM2 + 大清帝國兵
      MapFightingUnit enermy2_4 = gameDataService.CreateMapFightingUnit( robotService.CreateRobotInstance( 4, false ), pilotService.CreatePilotInstance( 9932, isPlayer: false, level: 10 ) ); //GM2 + 大清帝國兵

      PreReinforces( new List<ReinforceParam>() {
        new ReinforceParam( enermy2_1, 0, "Enermy", RespawnPoints[6].position/*new Vector3( -20, 0.1f, -14 )*/, new Vector3( 0, 315, 0 ), UnitInfo.TeamEnum.Enermy ),
        new ReinforceParam( enermy2_2, 0, "Enermy", RespawnPoints[7].position/*new Vector3( -9, 0.1f, -6 )*/, new Vector3( 0, 315, 0 ), UnitInfo.TeamEnum.Enermy ),
        new ReinforceParam( enermy2_3, 0, "Enermy", RespawnPoints[8].position/*new Vector3( -17, 0.1f, -10 )*/, new Vector3( 0, 315, 0 ), UnitInfo.TeamEnum.Enermy ),
        new ReinforceParam( enermy2_4, 0, "Enermy", RespawnPoints[9].position/*new Vector3( -17, 0.1f, -10 )*/, new Vector3( 0, 315, 0 ), UnitInfo.TeamEnum.Enermy ),
      }, 
      () => {
        initTalkBlock_2_1();
        stageManager.mapStory.DoTalkList( talkBlockList[0], () => {  //想不到紫禁城還保有這麼多戰力..！
          //stageManager.UnitLeave( unitInfo.GetComponent<UnitController>(), doEvent );
          PilotInstance heroPilotInstance = pilotService.LoadHeroPilotInstance( 1 );
          RobotInstance robot = robotService.CreateRobotInstance( 16, false, new List<PartsInstance>() {  // 1-WZ-EW  16: RZ_0
            partsService.CreatePartsInstanceByPartsID( 1, false ),  //Chobham裝甲 HP+500; 裝甲+150
            partsService.CreatePartsInstanceByPartsID( 2, false )   //磁層關節 運動性+10
          } );
          MapFightingUnit heroUnit = gameDataService.CreateMapFightingUnit( robot, heroPilotInstance ); //WZ-EW + 主角1
          //gameDataService.HouseUnits.Add( heroUnit );
          BGMController.SET_BGM( "F_Time To Come" );
          PreReinforces( new List<ReinforceParam>() {
            new ReinforceParam( heroUnit, 5, "Player", RespawnPoints[1].position, new Vector3( 0, 0, 0 ), UnitInfo.TeamEnum.Player, isJoinUs: true ),
          }, () => {
            initTalkBlock_3();
            stageManager.mapStory.DoTalkList( talkBlockList[0], () => {  //還有增援嗎？
              DoEvent();
            } );  
          } );

        } );
      } );
    } );
  }

  void event2() {   //第二次敵全滅: 紫禁城會話 + 黑洞出現 + 對話
    EventStatus = 2;

    initTalkBlock_4();  //紫禁城內對話
    stageManager.mapStory.DoTalkList( talkBlockList[0], () => {
      mapManager.enabled = false;
      //mapManager.myMapCursor.GetComponent<Cursor>().enabled = false;
      //mapManager.myMapCursor.GetComponent<Cursor>().MODE_ENUM = Cursor.ModeEnum.Talk;
      mapManager.myMapCursor.SetActive( false );

      BlackHolePrefab = Resources.Load<GameObject>( "Building/BlackHole" );
      BlackHolePoint = RespawnPoints.Where( t => t.name == "BlackHolePoint" ).FirstOrDefault();

      blackHole = Instantiate( BlackHolePrefab, BlackHolePoint.position, BlackHolePrefab.transform.localRotation );
      mapManager.MoveCamToCenterObject( blackHole );

      var animator = blackHole.GetComponent<Animator>();
      animator.Play( "BlackHole1" );

      CoroutineCommon.CallWaitForAnimator( animator, "BlackHole1", () => {
        initTalkBlock_5();  //紫禁城內重力異常報告
        stageManager.mapStory.DoTalkList( talkBlockList[0], DoEvent );
      } );
    } );  
  }

  void event3() {   //黑洞擴大: 完場前會話
    EventStatus = 3;
    mapManager.enabled = false;
    mapManager.myMapCursor.SetActive( false );
    mapManager.MoveCamToCenterObject( blackHole );

    var animator = blackHole.GetComponent<Animator>();
    animator.Play( "BlackHole2" );

    CoroutineCommon.CallWaitForAnimator( animator, "BlackHole2", () => {
      initTalkBlock_6();  //完場前會話

      stageManager.mapStory.DoTalkList( talkBlockList[0], () => {
        GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeOut( 2f, () => {
          gameDataService.StoryPhase = StoryPhaseEnum.After;  //After
          AfterMap();
          SceneManager.UnloadSceneAsync( stageManager.GetSceneName() );
          SceneManager.LoadScene( $"Story", LoadSceneMode.Additive );
        } );
      } );  
    } );
  }

  //判斷是否免爆炸
  protected override bool noBoom( UnitInfo unitInfo ) {
    if (unitInfo.PilotInfo.PilotInstance.PilotID == 702 && EventStatus == 0) 
      return true;    //克羅那

    return false;
  }

}
