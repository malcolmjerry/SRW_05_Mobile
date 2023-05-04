using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IStageHandler {

  void AfterBattle( List<AttackData> attackDataList );

  List<Transform> RespawnPoints { get; set; }

  void InitTalk();

  void InitBGM();

  void InitMapFightUnits();

  //void RestoreContinue( SaveContinue saveContinue );

  void SetManager( StageManager stageManager, MapManager mapManager );

  void BeforeAI();

  string PlayerBGM { get; set; }
  string EnemyBGM { get; set; }
  int EventStatus { get; set; } //0 初始狀態  1 味方增援
}
