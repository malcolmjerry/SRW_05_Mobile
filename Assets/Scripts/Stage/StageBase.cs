using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DataModel.Service;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using static DataModel.Service.GameDataService;

public abstract class StageBase : MonoBehaviour /*, IStageHandler*/ {

  public List<Transform> RespawnPoints { get; set; } = new List<Transform>();

  protected StageManager stageManager;
  protected MapManager mapManager;

  protected RobotService robotService;
  protected PilotService pilotService;
  protected PartsService partsService;
  protected GameDataService gameDataService;

  public virtual string PlayerBGM { get; set; }  = "F_PowerAndSkill";
  public virtual string EnemyBGM { get; set; } = "2G_EnermyPhase";
  public virtual string TerrainName { get; set; } = "ForbiddenCityTerrain";
  public virtual string BattleTerrainName { get; set; } = "ForbiddenCityTerrain";

  public virtual int EventStatus { get; set; }      //整體狀態
  public virtual List<int> EventList { get; set; }  //每個事件的發生狀態 (0/1) 或 其他參數
  //public abstract List<int> TalkEventList { get; set; }  //每個對話的發生狀態  0 未使用   1 本回合已用   2 歸檔
  protected virtual List<TalkBeforeBattle> TalkBeforeBattleList { get; set; } = new List<TalkBeforeBattle>();  //人物對戰時對話

  public int ObjectiveIndex { get; set; }  //作戰目的編號

  public void SetManager( StageManager stageManager, MapManager mapManager ) {
    this.stageManager = stageManager;
    this.mapManager = mapManager;
  }

  public StageBase() {
    robotService = DIContainer.Instance.RobotService;
    pilotService = DIContainer.Instance.PilotService;
    partsService = DIContainer.Instance.PartsService;
    gameDataService = DIContainer.Instance.GameDataService;
  }

  public void Setup() {
    mapManager.ObjectiveBox.GetComponent<ObjectiveCtl>().SetData( objDataList[ObjectiveIndex] );
    SetupTalk();
  }

  public abstract void SetupTalk();

  public abstract void FirstTalk();

  public abstract void InitMapFightUnits();

  protected abstract List<ObjectiveData> objDataList { get; set; }

  public List<TalkParam> talkBlock;

  /*
  protected MapFightingUnit CreateMapFightingUnit( RobotInstance robotInstance, PilotInstance pilotInstance ) {
    if (pilotInstance != null ) pilotInstance.RobotInstanceSeqNo = robotInstance?.SeqNo;

    MapFightingUnit unit = new MapFightingUnit();
    unit.RobotInfo = robotInstance?.CloneInfo( unit );
    unit.PilotInfo = pilotInstance?.CloneInfo();
    //unit.RobotInfo.InitWeapons();
    unit.Update();
    return unit;
  }*/

  /*
  protected void SetBeDestroyed() {
    if (stageManager.mapManager.GetComponent<MakeBattleManager>().ToUnitInfo.RobotInfo.HP <= 0)
      destroyedUnit = stageManager.mapManager.GetComponent<MakeBattleManager>().ToUnitInfo;
    else if (stageManager.mapManager.GetComponent<MakeBattleManager>().FromUnitInfo.RobotInfo.HP <= 0)
      destroyedUnit = stageManager.mapManager.GetComponent<MakeBattleManager>().FromUnitInfo;
    else destroyedUnit = null;
  }
  */

  List<ReinforceParam> ourReinforces;
  int index;
  Action reinforceCallback;
  public void PreReinforces( List<ReinforceParam> ourReinforces, Action callback ) {
    this.ourReinforces = ourReinforces;
    index = 0;
    reinforceCallback = callback;

    stageManager.StageUnits.ForEach( mf => mf.GetComponent<Rigidbody>().isKinematic = false );
    doReinforces();
  }

  void doReinforces() {
    CoroutineCommon.CallWaitForSeconds( .8f, () => {
      if (index >= ourReinforces.Count) {
        CoroutineCommon.CallWaitForSeconds( 1f, () => stageManager.StageUnits.ForEach( mf => mf.GetComponent<Rigidbody>().isKinematic = true ) );
        reinforceCallback();
        return;
      }

      var param = ourReinforces[index++];
      //OurReinforce( param.robotID, param.isPlayerRobot, param.pilotID, param.exp, param.enable, param.isPlayerPilot, param.addWillPower, param.layer, 
                    //param.position, param.euler, param.teamEnum, param.isJoinUs );
      Reinforce( param.mapFightingUnit, param.addWillPower, param.layer, param.position, param.euler, param.teamEnum, param.dropParts, param.isJoinUs, param.remainMoveAttackTurns );
      doReinforces();
    } );
  }

  public void Reinforce( MapFightingUnit mapFightingUnit, int addWillPower, string layer, Vector3 position, Vector3 euler, UnitInfo.TeamEnum teamEnum, 
    List<int> dropParts, bool isJoinUs, int remainMoveAttackTurns ) {
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/MapUnitComeOut" ), 2 );
    /*
    MapFightingUnit newPlayer = gameDataService.CreateMapFightingUnit(
                                  robotService.CreateRobotInstanceByRobotID( robotID, isPlayerRobot ),
                                  pilotService.CreatePilotInstanceByPilotID( pilotID, exp, enable, isPlayerPilot )
                                ); 
    */
    mapFightingUnit.PilotInfo.Willpower += addWillPower;

    var go = stageManager.createUnit( layer, position, euler, mapFightingUnit, teamEnum, -1, remainMoveAttackTurns, dropParts, isJoinUs );
    go.GetComponent<Rigidbody>().isKinematic = false;
    //go.GetComponent<UnitMovement>().MoveOneStep( () => { } );
    go.transform.position = new Vector3( go.transform.position.x + 0.1f, go.transform.position.y, go.transform.position.z );

    stageManager.StageUnits.Add( go );
    mapManager.myMapCursor.GetComponent<Cursor>().MODE_ENUM = Cursor.ModeEnum.Talk;
    stageManager.mapStory.moveCursorAndLookAt( position );
  }

  public void BeforeAI() {
    stageManager.enermyAI.DoAction();

    //可能根據回合數或血量等 發生一些事件
    /*
    switch (EventStatus) {
      case 0: stageManager.enermyAI.StayAction(); break;
      case 1: stageManager.enermyAI.NextAction(); break;
      default: stageManager.enermyAI.NextAction(); break;
    }
    */
  }

  List<AttackData> attackDataList = null;
  protected List<UnitInfo> destroyPendingList = new List<UnitInfo>();
  protected List<UnitInfo> destroyNowList;
  private bool isExpDone;

  public void AfterBattle( List<AttackData> attackDataList ) {
    mapManager.GetComponent<MapManager>().myMapCursor.GetComponent<Cursor>().MODE_ENUM = Cursor.ModeEnum.Talk;
    this.attackDataList = attackDataList;
    isExpDone = false;
    //找出所有戰鬥後 HP歸0 的 Unit 
    destroyPendingList = attackDataList.Where( att => att.ToUnitInfo.RobotInfo.HP <= 0 ).Select( att => att.ToUnitInfo ).ToList();

    //如果有機體被擊破, 全體氣力+1
    if (destroyPendingList.Count > 0)
      stageManager.StageUnitsInfo.Where( u => !u.IsDestroyed ).ToList().ForEach( u => u.GetComponent<UnitInfo>().PilotInfo.Willpower++ );

    updateEventList( destroyPendingList, attackDataList[0] );
    refreshMap();
  }

  protected void refreshMap() {
    destroyNowList = destroyPendingList.Where( dp => !noBoom( dp ) ).ToList();
    destroyPendingList.RemoveAll( dp => !noBoom( dp ) );

    destroyRecursion();
  }

  void destroyRecursion() {
    if (destroyNowList.Count > 0) {
      stageManager.DestroyUnit( destroyNowList[0].GetComponent<UnitController>(), destroyRecursion );
      destroyNowList.RemoveAt( 0 );
      return;
    }

    if (!isExpDone && attackDataList?.Count > 0)
      DoExp( DoEvent );
    else
      DoEvent();
  }

  protected void DoExp( Action cbDoEvent ) {
    isExpDone = true;
    //根據 attackDataList 中的我方 Player 殺敵/傷害 累計exp, 顯示 獲得exp 和升級畫面, 然後callback afterevent
    var playAttList = attackDataList.Where( at => at.FromUnitInfo.Team == UnitInfo.TeamEnum.Player && at.TotalDamage > 0 ).ToList();

    if (playAttList == null || playAttList.Count == 0 || playAttList[0].FromUnitInfo.IsDestroyed) {
      cbDoEvent();
      return;
    }

    List<PartsInstance> partsList = new List<PartsInstance>();
    int resultExp = 0;
    int resultMoney = 0;
    float tempMoney = 0;
    for (int i=0; i < playAttList.Count; i++) {
      var att = playAttList[i];
      int expBase = att.ToUnitInfo.PilotInfo.PilotInstance.Pilot.ExpDead;
      int LvGap = att.ToUnitInfo.PilotInfo.Level - att.FromUnitInfo.PilotInfo.Level;
      int exp = expBase;
      if (LvGap > 0) {
        var extraRate = 1 + LvGap * .6f;
        exp = (int)(expBase * extraRate);
      }
      else if (LvGap < 0) {
        LvGap = Math.Abs( LvGap );
        if (LvGap > 10) LvGap = 10;
        exp = (int)(expBase * Mathf.Pow( .7f, LvGap ));
        if (exp < 10) exp = 10;
      }

      exp = i > 0? exp / 3 : exp;

      if (!att.IsDefeated)
        exp /= 10;

      if (exp < 1) exp = 1;

      resultExp += exp;

      //計算獲得金錢及強化Parts
      if (att.IsDefeated) {
        foreach (int partId in att.ToUnitInfo.DropParts) {
          var partsIn = partsService.CreatePartsInstanceByPartsID( partId, false );
          stageManager.StageParts.Add( partsIn );
          partsList.Add( partsIn );
        }

        tempMoney += att.ToUnitInfo.RobotInfo.RobotInstance.Robot.RepairPrice * (1f + att.ToUnitInfo.PilotInfo.Level / 10f); //gameDataService.GameData.Chapter / 10f);
      }
    }

    var tempExp = resultExp * playAttList[0].ExpRateAdd;
    resultExp = (int)tempExp;
    tempMoney = tempMoney * playAttList[0].MoneyRateAdd;
    resultMoney = (int)tempMoney;

    PilotInfo pilotInfo = playAttList[0].FromUnitInfo.PilotInfo;

    stageManager.ExpResult.gameObject.SetActive( true );
    stageManager.ExpResult.GetComponent<ExpResult>().Setup( resultMoney, resultExp, pilotInfo, partsList, 
      () => {
        //顯示駕駛員升級/技能學習/精神學精變化
        cbDoEvent();
      } 
    );
  }

  protected virtual bool noBoom( UnitInfo unitInfo ) {
    return false;
  }

  protected virtual void updateEventList( List<UnitInfo> destroyPendingList, AttackData attData ) { 
  }

  public virtual void DoEvent() {
    afterEvent();
  }

  protected void afterEvent() {
    //stageManager.mapManager.myMapCursor.SetActive( false );

    if (destroyPendingList.Count > 0) {   //存在 HP 歸0 仍未爆炸的unit, 執行 event
      var unitInfo = destroyPendingList[0];
      destroyPendingList.RemoveAt( 0 );

      //沒有 event 適用, 自動撤退
      stageManager.UnitLeave( unitInfo.GetComponent<UnitController>(), afterEvent );
      //DoEvent(); //沒有用, 上一句撤退後, callback回到 afterevent
      return;
    }

    //敵全滅 沒有 event, 直接過版
    if (stageManager.StageUnits_Enemy.Count < 1) {
      gameDataService.StoryPhase = StoryPhaseEnum.After;  //After

      GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeOut( 2f, () => {
        AfterMap();

        SceneManager.UnloadSceneAsync( SceneManager.GetActiveScene() );
        SceneManager.LoadScene( $"Story", LoadSceneMode.Additive );
      } );
      return;
    }

    if (CheckGameOver()) {
      MyCanvas.SHOW_GAMEOVER( null,
        () => {
          MyCanvas.FadeOut( 2f, () => {
            gameDataService.LoadBeforeStage();

            SceneManager.UnloadSceneAsync( SceneManager.GetActiveScene() );
            SceneManager.LoadScene( $"Title", LoadSceneMode.Additive );
          } );
        }
      );
      return;
    }

    //回到正常回合處理流程
    stageManager.HandlePhase();
  }

  public virtual bool CheckGameOver() {
    return stageManager.StageUnits_Player_OnMap.Count < 1;
  }

  /// <summary> 地圖結束後之處理, 例如我方機體加到 HouseUnits, 強化部件加到 PartsInstanceList </summary>
  public virtual void AfterMap() {
    //如果有要加入的 機組 或 機體 或 人物, 在Stage_X 裡 override本方法, 直接 以 isPlayer = true 的形式創建 Instance, 便可直接加到 House 中, 最後別忘記調用 base方法

    foreach (var p in stageManager.StageParts)
      partsService.AddPartsInstance( p );
    
    // 如有新加入的機體, 加至 HouseUnits
    
    foreach (var p in stageManager.StageUnits_Player) {
      UnitInfo unitInfo = p.GetComponent<UnitInfo>();
      if (!unitInfo.IsNewToPlayer)
        continue;

      robotService.AddRobotInstance( unitInfo.RobotInfo.RobotInstance );  //重新獲取 Robot SeqNo

      unitInfo.PilotInfo.PilotInstance.RobotInstanceSeqNo = unitInfo.RobotInfo.RobotInstance.SeqNo;  //修復 Pilot 對應的  Robot SeqNo
      unitInfo.PilotInfo.PilotInstance.Enable = 1;
      pilotService.AddPilotInstance( unitInfo.PilotInfo.PilotInstance );

      foreach (var partsIn in unitInfo.RobotInfo.RobotInstance.PartsInstanceList)
        partsService.AddPartsInstance( partsIn );

      gameDataService.HouseUnits.Add( unitInfo.MapFightingUnit );
    }
    
    foreach (var unit in gameDataService.HouseUnits.Where( u => u.IsFightable ))
      unit.UpdateInit();
  }

  public void RestoreTalkEventList(List<int> talkEventList) {
    for (int i = 0; i < TalkBeforeBattleList.Count; i++)
      TalkBeforeBattleList[i].IsTalked = talkEventList[i];
  }

  /// <summary> 每個對話的發生狀態  0 未使用   1 本回合已用   2 歸檔 </summary>
  public List<int> TalkEventList => TalkBeforeBattleList.Select( t => t.IsTalked ).ToList();

  public Action TalkBeforeBattle( int fromPilotId, int toPilotId, int? heroSeqNo ) {
    if (heroSeqNo != null) {
      var heroTalkB4Battle = TalkBeforeBattleList.Where( t => (t.PilotIds.Contains( fromPilotId ) || t.PilotIds.Contains( toPilotId )) && t.IsTalked == 0 && t.HeroSeq == heroSeqNo ).FirstOrDefault();
      if (heroTalkB4Battle != null) {
        heroTalkB4Battle.IsTalked = 2;
        return heroTalkB4Battle.GetTalkBlock;
      }
      return null;
    }

    var talkB4Battle = TalkBeforeBattleList.Where( t => t.PilotIds.Contains( fromPilotId ) && t.PilotIds.Contains( toPilotId ) && t.IsTalked == 0 ).FirstOrDefault();
    if (talkB4Battle != null) {
      talkB4Battle.IsTalked = 2;
      return talkB4Battle.GetTalkBlock;
    }
    return null;
  }
}

public class ReinforceParam {

  public ReinforceParam( MapFightingUnit mapFightingUnit, int addWillPower,
                          string layer, Vector3 position, Vector3 euler, UnitInfo.TeamEnum teamEnum, 
                          List<int> dropParts = null, bool isJoinUs = false, int remainMoveAttackTurns = 0 ) {
    this.mapFightingUnit = mapFightingUnit;
    this.addWillPower = addWillPower;
    this.layer = layer;
    this.position = position;
    this.euler = euler;
    this.teamEnum = teamEnum;
    this.dropParts = dropParts;
    this.isJoinUs = isJoinUs;
    this.remainMoveAttackTurns = remainMoveAttackTurns;
  }

  public MapFightingUnit mapFightingUnit;
  public int addWillPower;
  public string layer;
  public Vector3 position;
  public Vector3 euler;
  public UnitInfo.TeamEnum teamEnum;
  public bool isJoinUs;
  public List<int> dropParts = null;
  public int remainMoveAttackTurns;

}