using UnityEngine;
using System.Collections;
//using UnityEditor;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DataModel.Service;
using System.Threading;
using System.Threading.Tasks;

public class StageManager : MonoBehaviour {

  public MapManager mapManager;
  public CounterSummary CounterSummary;
  public MapStory mapStory;
  public Transform ExpResult;

  public int StageNum;

  public GameObject unitRootPrefab;

  public List<GameObject> StageUnits { get; set; } = new List<GameObject>();

  [HideInInspector]
  public List<UnitInfo> StageUnitsInfo { get { return StageUnits.Select( su => su.GetComponent<UnitInfo>() ).ToList(); } }

  public List<PartsInstance> StageParts { get; set; } = new List<PartsInstance>();

  [HideInInspector]
  public List<GameObject> StageUnits_Player { get { return StageUnits.Where( u => u.GetComponent<UnitInfo>().Team == UnitInfo.TeamEnum.Player && !u.GetComponent<UnitInfo>().IsDestroyed ).ToList(); } }

  [HideInInspector]
  public List<GameObject> StageUnits_Player_OnMap { get { 
      return StageUnits.Where( u => u.GetComponent<UnitInfo>().Team == UnitInfo.TeamEnum.Player 
                                && !u.GetComponent<UnitInfo>().IsDestroyed && !u.GetComponent<UnitInfo>().IsOnShip ).ToList(); 
  } }

  [HideInInspector]
  public List<GameObject> StageUnits_Enemy { get { return StageUnits.Where( u => u.GetComponent<UnitInfo>().Team == UnitInfo.TeamEnum.Enermy && !u.GetComponent<UnitInfo>().IsDestroyed ).ToList(); } }

  List<UnitInfo> enemyList;
  List<UnitInfo> playerList;

  private List<Transform> respawnPoints;

  [HideInInspector]
  public bool IsPlayerPhase { get; set; }

  [HideInInspector]
  public EnermyAI enermyAI;

  //[HideInInspector]
  //public int eventStatus = 0;  //0 初始狀態

  public StageBase StageBase;

  void Awake() {

  }

  // Use this for initialization
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  public void InitStageBase( SaveContinue SaveContinue = null ) {
    StageNum = DIContainer.Instance.GameDataService.GameData.Stage;

    //respawnPoints = GameObject.Find( "RespawnPoints" ).transform.Cast<Transform>().ToList();
    var respawnPointsPrefab = Resources.Load<GameObject>( $"StageMap/RespawnPoints/RespawnPoints_{StageNum}" );
    var respawnPointsGo = Instantiate( respawnPointsPrefab, null );
    respawnPoints = respawnPointsGo.transform.Cast<Transform>().ToList();

    //Stage_1
    StageBase = (StageBase)gameObject.AddComponent( Type.GetType( "Stage_" + StageNum ) );
    //StageBase = (StageBase)GetComponent( Type.GetType( "Stage_" + StageNum ) );
    StageBase.SetManager( this, mapManager );

    var terrainPrefab = Resources.Load<GameObject>( $"Terrains/{StageBase.TerrainName}" );
    var terrainGo = Instantiate( terrainPrefab, null );
    terrainGo.name = "Terrain";

    //enermyAI = new EnermyAI();
    enermyAI = GetComponent<EnermyAI>();
    enermyAI.SetStageManager( this );

    StageBase.RespawnPoints = respawnPoints;
    StageBase.Setup();
    BGMController.SET_BGM( StageBase.PlayerBGM );

    if (SaveContinue == null) {
      DIContainer.Instance.GameDataService.InitTurn();
      StageBase.InitMapFightUnits();  //要在 InitStage 前生成所有機體, 否則對話無法找到位置
      StageBase.FirstTalk();
    }
    else {
      RestoreContinue( SaveContinue );
      IsPlayerPhase = true;
      HandlePhase();
    }
  }

  /*
  public void AfterBattleBase() {
    //var b = GameObject.Find( "Main Camera" ).GetComponent<AudioListener>().enabled;
    //var a = Camera.main.GetComponent<AudioListener>().enabled;
    //var c = 1;
    //Camera.main.GetComponent<AudioListener>().enabled = true;
    //mapManager.enabled = false;
    //stageHandler.AfterBattle();
    StartCoroutine( AfterBattleBaseAsync() );
  }*/
  /*
  public IEnumerator AfterBattleBaseAsync( BattleEntry entry ) {
    var asyncLoad = SceneManager.UnloadSceneAsync( "Battle" );
    while (!asyncLoad.isDone) {
      yield return null;
    }
    //Camera.main.GetComponent<AudioListener>().enabled = true;
    mapManager.enabled = false;
    stageHandler.AfterBattle();
  }*/

  public void AfterBattle( List<AttackData> attackDataList ) {
    StartCoroutine( AfterBattleAsync( attackDataList ) );
  }

  public IEnumerator AfterBattleAsync( List<AttackData> attackDataList ) {
    //MyCanvas.FadeOut( 0, () => { }, blackTime: 0, hold: true );
    //Scene stageScene = SceneManager.GetSceneByName( GetSceneName() );  //2021-11-25
    //SceneManager.SetActiveScene( stageScene );     ////2021-11-25

    var asyncLoad = SceneManager.UnloadSceneAsync( "Battle" );
    while (!asyncLoad.isDone) {
      yield return null;
    }
    mapManager.enabled = false;

    MyCanvas.FadeOut( 0, () => {}, blackTime: 0 );
    StageBase.AfterBattle( attackDataList );
  }

  public void AfterMapW( List<AttackData> attackDataList ) {
    mapManager.enabled = false;
    StageBase.AfterBattle( attackDataList );
  }

  public void DestroyUnit( UnitController unit, Action callBack ) {
    mapManager.myMapCursor.SetActive( true );
    mapManager.myMapCursor.GetComponent<Cursor>().SetPosition( new Vector3( unit.transform.position.x, mapManager.myMapCursor.GetComponent<Cursor>().defaultY, unit.transform.position.z ) );
    //mapManager.unitSelected = null;

    CoroutineCommon.CallWaitForSeconds( 0.5f, () => {
      unit.MyDestroyed( () => {
        //StageUnits.Remove( unit.gameObject );
        //Destroy( unit.gameObject );
        unit.GetComponent<UnitInfo>().IsDestroyed = true;
        mapManager.myMapCursor.GetComponent<Cursor>().SetDisable( true );
        unit.gameObject.SetActive( false );
        callBack();
      } );
    } );
  }

  public void UnitLeave( UnitController unit, Action callBack, float wait = .5f, float waitAfter = 1f ) {
    mapManager.myMapCursor.SetActive( true );
    //mapManager.myMapCursor.transform.position = new Vector3( unit.transform.position.x, mapManager.myMapCursor.GetComponent<Cursor>().defaultY, unit.transform.position.z );
    mapManager.myMapCursor.GetComponent<Cursor>().SetPosition( unit.transform.position );

    CoroutineCommon.CallWaitForSeconds( wait, () => {
      unit.MyLeave( () => {
        //StageUnits.Remove( unit.gameObject );
        //Destroy( unit.gameObject );
        unit.GetComponent<UnitInfo>().IsDestroyed = true;
        mapManager.myMapCursor.GetComponent<Cursor>().SetDisable( true );
        unit.gameObject.SetActive( false );
        callBack();
      }, waitAfter );
    } );
  }

  List<GameObject> leaveUnits;
  Action leaveUnitsCb;
  int leaveCount;
  public void UnitsLeave( List<GameObject> units, Action callBack ) {
    leaveUnits = units;
    leaveUnitsCb = callBack;
    leaveCount = 0;
    LoopUnitLeave();
  }

  private void LoopUnitLeave() {
    if (leaveCount < leaveUnits.Count) {
      UnitLeave( leaveUnits[leaveCount++].GetComponent<UnitController>(), LoopUnitLeave, .3f, .3f );
    }
    else
      leaveUnitsCb();
  }

  public GameObject createUnit( string layer, Vector3 position, Vector3 euler, MapFightingUnit fightingUnit, UnitInfo.TeamEnum team, 
    int actionCount = -1, int remainMoveAttackTurns = 0, List<int> dropParts = null, bool isNewToPlayer = false ) {
    if (fightingUnit.RobotInfo == null || fightingUnit.PilotInfo == null) return null;

    GameObject clone = Instantiate( unitRootPrefab, position, Quaternion.identity );
    clone.GetComponent<UnitController>().Setup( fightingUnit.RobotInfo.RobotInstance.Robot.Name );   //"WZ_EW"
    clone.transform.Rotate( euler );
    clone.layer = LayerMask.NameToLayer( layer );

    //clone.GetComponent<UnitInfo>().Team = team;
    //clone.GetComponent<UnitInfo>().MapFightingUnit = fightingUnit;
    clone.GetComponent<UnitInfo>().Setup( team, fightingUnit, remainMoveAttackTurns, dropParts, isNewToPlayer );
    //clone.GetComponent<UnitInfo>().RobotInfo = fightingUnit.RobotInfo;
    //clone.GetComponent<UnitInfo>().PilotInfo = fightingUnit.PilotInfo;
    clone.GetComponent<UnitHealth>().enabled = true;
    clone.GetComponent<UnitController>().SetActionCount( actionCount <= -1? fightingUnit.ActionCount : actionCount );

    //clone.GetComponent<UnitController>().enabled = true;
    return clone;
    /*
    Transform canvasTransform = clone.transform.Find( "Canvas" );
    canvasTransform.localScale = new Vector3( fightingUnit.RobotInfo.Radius, fightingUnit.RobotInfo.Radius, 1 );

    canvasTransform.Find( "HealthSlider/Fill Area/Fill" ).GetComponent<Image>().color = getColorByTeam( team, 255 );
    //canvasTransform.Find( "HealthSlider/Background" ).GetComponent<Image>().color = getColorByTeam( fightingUnit.Team, 130 );

    clone.GetComponent<CapsuleCollider>().radius = fightingUnit.RobotInfo.Radius;
    clone.GetComponent<CapsuleCollider>().height *= fightingUnit.RobotInfo.Radius;
    clone.GetComponent<CapsuleCollider>().center = new Vector3( 0, fightingUnit.RobotInfo.Radius, 0 );
    //clone.GetComponent<UnitInfo>().Ready = true;
    return clone;
    */
  }

  public GameObject createUnit( string layer, Transform respawnPoint, MapFightingUnit fightingUnit, UnitInfo.TeamEnum team, 
    int actionCount = -1, int remainMoveAttackTurns = 0, List<int> dropParts = null, bool isNewToPlayer = false ) {
    if (fightingUnit.RobotInfo == null || fightingUnit.PilotInfo == null)
      return null;

    GameObject clone = Instantiate( unitRootPrefab, respawnPoint.position, Quaternion.identity );
    clone.GetComponent<UnitController>().Setup( fightingUnit.RobotInfo.RobotInstance.Robot.Name );   //"WZ_EW"
    clone.transform.Rotate( respawnPoint.eulerAngles );
    clone.layer = LayerMask.NameToLayer( layer );
    clone.GetComponent<UnitInfo>().Setup( team, fightingUnit, remainMoveAttackTurns, dropParts, isNewToPlayer );
    clone.GetComponent<UnitHealth>().enabled = true;
    clone.GetComponent<UnitController>().SetActionCount( actionCount <= -1 ? fightingUnit.ActionCount : actionCount );
    return clone;
  }

  /*
  public void DoDestroy( bool noBoomFromUnit, bool noBoomToUnit, Action callback ) {
    fromUnit = mapManager.GetComponent<MakeBattleManager>().FromUnitInfo;
    toUnit = mapManager.GetComponent<MakeBattleManager>().ToUnitInfo;
    
    //Transform canvasTransform = fromUnit.transform.Find( "Canvas" );
    //Debug.Log( "After Battle Fill color: " + canvasTransform.Find( "HealthSlider/Fill Area/Fill" ).GetComponent<Image>().color );
    //canvasTransform.Find( "HealthSlider/Fill Area/Fill" ).GetComponent<Image>().color = getColorByTeam( fromUnit.Team );
    
    fromUnit.GetComponent<UnitController>().EndAction();

    if (toUnit.RobotInfo.HP <= 0 && !noBoomToUnit) {
      DestroyUnit( toUnit.GetComponent<UnitController>(), () => { DoExp( callback ); } );
    }
    else if (fromUnit.RobotInfo.HP <= 0 && !noBoomFromUnit) {
      DestroyUnit( fromUnit.GetComponent<UnitController>(), () => { DoExp( callback ); } );
    }
    else {
      DoExp( callback );
    }
  }*/

  public void HandlePhase() {
    if (mapManager.ObjectiveBox.GetComponent<ObjectiveCtl>().IsEdited) {
      mapManager.ObjectiveBox.GetComponent<ObjectiveCtl>().Setup( HandlePhase );
      return;
    }

    if (IsPlayerPhase) {
      mapManager.BackToMap( true );
      /*
      if (!MapFightingUnits_Player.Any( u => u.GetComponent<UnitController>().status == (int)UnitController.UnitStatusEnum.READY )) {
        Debug.Log( "All action end" );
      }*/
    }
    else {
      mapManager.myMapCursor.GetComponent<Cursor>().SetDisable( true );
      if (!StageUnits_Enemy.Any( u => u.GetComponent<UnitController>().status == UnitController.UnitStatusEnum.READY )) {
        toPlayerPhase();
      }
      else {
        StageBase.BeforeAI();
      }
    }
  }

  private void toPlayerPhase() {
    BGMController.SET_BGM( StageBase.PlayerBGM );
    foreach (var enermy in StageUnits_Enemy) {
      enermy.GetComponent<UnitController>().RechargeAction();
      enermy.GetComponent<UnitInfo>().RemainMoveAttackTurns--;
    }

    IsPlayerPhase = true;
    playerList = StageUnits_Player.Select( u => u.GetComponent<UnitInfo>() ).ToList();

    MyCanvas.SHOW_PHASE( "Player Phase",
      () => {
        buffPhase( playerList );
        DIContainer.Instance.GameDataService.AddTurn();
      },
      //HandlePhase
      StageBase.DoEvent
    );
  }

  public void ToEnemyPhase() {
    BGMController.SET_BGM( StageBase.EnemyBGM );
    foreach (var player in StageUnits_Player) {
      if (player == null) continue;
      player.GetComponent<UnitController>().RechargeAction();
    }

    IsPlayerPhase = false;
    mapManager.enabled = false;
    //stageHandler.BeforeAI();

    enemyList = StageUnits_Enemy.Select( u => u.GetComponent<UnitInfo>() ).ToList();

    MyCanvas.SHOW_PHASE( "Enemy Phase", 
      () => {
        buffPhase( enemyList );
      },
      //HandlePhase
      StageBase.DoEvent
    );
  }

  private void buffPhase( List<UnitInfo> unitInfoList ) {
    foreach (var unitInfo in unitInfoList) {
      //SP+5
      unitInfo.PilotInfo.RemainSp = Math.Min( unitInfo.PilotInfo.RemainSp + 5, unitInfo.PilotInfo.MaxSp );

      //清除一回合有效的精神指令
      unitInfo.PilotInfo.ActiveSPCommandList.RemoveAll( s => s.Period == SPCommand.PeriodEnum.OnePhase );

      //戰艦回復
      if (unitInfo.RobotInfo.RobotInstance.Robot.IsShip == true) {
        foreach (var unit in unitInfo.StoringUnits) {
          unit.PilotInfo.Willpower = Math.Max( unit.PilotInfo.Willpower - 1, 100 );
          unit.RobotInfo.HP = Math.Min( (int)(unit.RobotInfo.HP + unit.RobotInfo.MaxHP * .3f), unit.RobotInfo.MaxHP );
          unit.RobotInfo.EN = Math.Min( (int)(unit.RobotInfo.EN + unit.RobotInfo.MaxEN * .3f), unit.RobotInfo.MaxEN );
          unit.MapFightingUnit.WeaponList.ForEach( w => w.RemainBullets = w.MaxBullets );
        }
      }
    }
  }

  public Vector3? GetPosByPilot( int pilotID ) {
    var unit = StageUnits.Select( m => m.GetComponent<UnitInfo>() ).FirstOrDefault( u => u.PilotInfo.PilotInstance.PilotID == pilotID );
    return unit?.transform.position;
  }

  public Vector3? GetPosByHero( int heroSeqNo ) {
    var unit = StageUnits.Select( m => m.GetComponent<UnitInfo>() ).FirstOrDefault( u => u.PilotInfo.PilotInstance.HeroSeqNo == heroSeqNo );
    return unit?.transform.position;
  }

  public string GetSceneName() {
    //return $"stage_{StageNum.ToString().PadLeft( 3, '0' )}";
    return "stage";
  }

  void RestoreContinue( SaveContinue saveContinue ) {
    StageBase.EventStatus = saveContinue.EventStatus;
    StageBase.EventList = saveContinue.EventList;
    StageBase.RestoreTalkEventList( saveContinue.TalkEventList );
    StageBase.ObjectiveIndex = saveContinue.ObjectiveIndex;
    StageBase.PlayerBGM = saveContinue.PlayerBGM;
    StageBase.EnemyBGM = saveContinue.EnemyBGM;
    mapManager.myMapCursor.transform.position = saveContinue.CursorPos.ToVector3();

    Dictionary<SaveMapUnit, UnitInfo> afterCheckDict = new Dictionary<SaveMapUnit, UnitInfo>();

    foreach (var saveUnit in saveContinue.SaveMapUnits) {
      var unit = createUnit( saveUnit.Layer, saveUnit.Position.ToVector3(), saveUnit.TheEuler.ToVector3(), saveUnit.MapFightingUnit, saveUnit.Team, 
                             saveUnit.RemainActionCount, saveUnit.RemainMoveAttackTurns, saveUnit.DropParts, saveUnit.IsNewToPlayer );
      StageUnits.Add( unit );
      unit.transform.Find( "Renderer" ).Rotate( saveUnit.RendererEuler.ToVector3() );
      var unitInfo = unit.GetComponent<UnitInfo>();
      unitInfo.IsNewToPlayer = saveUnit.IsNewToPlayer;
      unitInfo.IsDestroyed = saveUnit.IsDestroyed;
      unitInfo.IsOnShip = saveUnit.IsOnShip;

      if (unitInfo.IsDestroyed) {
        unit.SetActive( false );
        continue;
      }

      if (saveUnit.StoringUnitSeqNoList?.Count > 0 || saveUnit.ShipUnitSeqNo.HasValue || saveUnit.OtherUnitSeqNo.HasValue)
        afterCheckDict.Add( saveUnit, unit.GetComponent<UnitInfo>() );
    }

    foreach (var checkUnit in afterCheckDict) {
      // 固定攻擊目標 (挑發)
      if (checkUnit.Key.OtherUnitSeqNo.HasValue && checkUnit.Key.OtherUnitSeqNo.Value > 0) {
        int seqNo = checkUnit.Key.OtherUnitSeqNo.Value;
        var otherUnit = StageUnits_Player.Where( u => u.GetComponent<UnitInfo>().MapFightingUnit.RobotInfo.RobotInstance.SeqNo == seqNo )
                        .Select( u => u.GetComponent<UnitInfo>() ).FirstOrDefault();
        checkUnit.Value.OtherUnit = otherUnit;
      }
      // 附近的戰艦
      if (checkUnit.Key.ShipUnitSeqNo.HasValue && checkUnit.Key.ShipUnitSeqNo.Value > 0) {
        int seqNo = checkUnit.Key.ShipUnitSeqNo.Value;
        var otherUnit = StageUnits_Player.Where( u => u.GetComponent<UnitInfo>().MapFightingUnit.RobotInfo.RobotInstance.SeqNo == seqNo )
                        .Select( u => u.GetComponent<UnitInfo>() ).FirstOrDefault();
        checkUnit.Value.ShipUnit = otherUnit;
      }
      //戰艦中的機體列表
      if (checkUnit.Key.StoringUnitSeqNoList != null && checkUnit.Key.StoringUnitSeqNoList.Count > 0) {
        checkUnit.Value.StoringUnits = new List<UnitInfo>();
        foreach (var seqNo in checkUnit.Key.StoringUnitSeqNoList) {
          var storeUnit = StageUnits_Player.Where( u => u.GetComponent<UnitInfo>().MapFightingUnit.RobotInfo.RobotInstance.SeqNo == seqNo )
                          .Select( u => u.GetComponent<UnitInfo>() ).FirstOrDefault();
          checkUnit.Value.StoringUnits.Add( storeUnit );
          storeUnit.gameObject.SetActive( false );
        }
      }
    }

    StageParts = saveContinue.StageParts;
  }

  public SaveContinue CreateSave() {
    SaveContinue saveCon = new SaveContinue();
    saveCon.EventStatus = StageBase.EventStatus;
    saveCon.EventList = StageBase.EventList;
    saveCon.TalkEventList = StageBase.TalkEventList;
    saveCon.ObjectiveIndex = StageBase.ObjectiveIndex;
    saveCon.PlayerBGM = StageBase.PlayerBGM;
    saveCon.EnemyBGM = StageBase.EnemyBGM;
    saveCon.CursorPos = new MyVector3( mapManager.myMapCursor.transform.position );

    foreach (var unit in StageUnits) {
      var unitInfo = unit.GetComponent<UnitInfo>();

      SaveMapUnit saveUnit = new SaveMapUnit() {
        Team = unitInfo.Team,
        IsNewToPlayer = unitInfo.IsNewToPlayer,
        IsDestroyed = unitInfo.IsDestroyed,
        IsOnShip = unitInfo.IsOnShip,
        RemainMoveAttackTurns = unitInfo.RemainMoveAttackTurns,
        Layer = LayerMask.LayerToName( unit.layer ),
        Position = new MyVector3( unit.transform.position ),
        TheEuler = new MyVector3( unit.transform.eulerAngles ),
        RendererEuler = new MyVector3( unit.transform.Find( "Renderer" ).localEulerAngles ),
        MapFightingUnit = unit.GetComponent<UnitInfo>().MapFightingUnit,
        RemainActionCount = unit.GetComponent<UnitController>().RemainActionCount,
        OtherUnitSeqNo = unit.GetComponent<UnitInfo>().OtherUnit?.RobotInfo.RobotInstance.SeqNo, //挑發專用      
        ShipUnitSeqNo = unit.GetComponent<UnitInfo>().ShipUnit?.RobotInfo.RobotInstance.SeqNo, //鄰近的戰艦
        StoringUnitSeqNoList = unit.GetComponent<UnitInfo>().StoringUnits?.Select( u => u.RobotInfo.RobotInstance.SeqNo ).ToList(),  //戰艦中的機
        DropParts = unit.GetComponent<UnitInfo>().DropParts  //被擊墜時 (HP=0) 掉落的零件
      };

      saveCon.SaveMapUnits.Add( saveUnit );
    }

    saveCon.StageParts = StageParts;

    return saveCon;
  }

}
