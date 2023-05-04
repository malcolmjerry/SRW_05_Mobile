using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;
using static AttackData;

public class MakeBattleManager : MonoBehaviour {

  public GameObject battleEntryPrefab;
  //public string stageName;

  [HideInInspector] public UnitInfo FromUnitInfo;
  [HideInInspector] public UnitInfo ToUnitInfo;
  [HideInInspector] public WeaponInfo FromWeapon;
  [HideInInspector] public WeaponInfo ToWeapon;

  private Action backToCaller;
  private Action endCaller;

  public GameObject m_BattleSummaryPrefab;
  private GameObject myBattleSummary;

  private int fromHitRate;
  //private int fromHitRateText;
  private int toHitRate;
  //private int toHitRateText;
  //private int fromDamage;
  //private int toDamage;
  //private bool fromHit;
  //private bool toHit;

  public GameObject m_AimPrefab;

  private bool fromRight;

  //private List<AttackData> attDataList = new List<AttackData>();

  public AttackData FirstAttack;      //攻方
  public AttackData CounterAttack;    //反方
  //private AttackData SupportAttack;    //支援攻擊
  //private AttackData AssistantAttack;  //援護攻擊

  public StageManager stageManager;
  public MapManager mapManager;
  public Light myLight;

  // Use this for initialization
  void Start () {
    //stageManager = GameObject.Find( "StageManager" ).GetComponent<StageManager>();
    //mapManager = GameObject.Find( "MapManager" ).GetComponent<MapManager>();
  }
	
	// Update is called once per frame
	void Update () {

    if (Input.GetButtonDown( "Back" )) {
      if (backToCaller != null) {
        EffectSoundController.PLAY_BACK_CANCEL();
        this.enabled = false;
        Destroy( myBattleSummary );
        backToCaller();
      }
      else {

      }
    } else if (Input.GetButtonDown( "Confirm" )) {
      EffectSoundController.PLAY_MENU_CONFIRM();
      this.enabled = false;
      Destroy( myBattleSummary );
      endCaller?.Invoke();

      if (ToUnitInfo.PilotInfo.PilotInstance.Pilot.BgmPriority > FromUnitInfo.PilotInfo.PilotInstance.Pilot.BgmPriority && !string.IsNullOrWhiteSpace( ToUnitInfo.PilotInfo.PilotInstance.Pilot.BGM ))
        BGMController.SET_BGM( ToUnitInfo.PilotInfo.PilotInstance.Pilot.BGM );
      else BGMController.SET_BGM( FromUnitInfo.RobotInfo.RobotInstance.BGM );

      FromUnitInfo.transform.Find( "Renderer" ).rotation = FromUnitInfo.transform.rotation;
      FromUnitInfo.transform.LookAt( ToUnitInfo.transform );
      //FromUnitInfo.GetComponent<UnitController>().myLookAtCam();
      ToUnitInfo.transform.Find( "Renderer" ).rotation = ToUnitInfo.transform.rotation;
      ToUnitInfo.transform.LookAt( FromUnitInfo.transform );

      ////mapManager.unitSelected = FromUnitInfo.GetComponent<UnitController>();  //2021-11-12 移除此句

      int? heroSeq = ToUnitInfo.PilotInfo.PilotInstance.Hero?.SeqNo;
      if (!heroSeq.HasValue)
        heroSeq = FromUnitInfo.PilotInfo.PilotInstance.Hero?.SeqNo;

      Action theGetTalkBlock = stageManager.StageBase.TalkBeforeBattle( FromUnitInfo.PilotInfo.PilotInstance.PilotID, ToUnitInfo.PilotInfo.PilotInstance.PilotID, heroSeq );
      if (theGetTalkBlock != null) {
        theGetTalkBlock.Invoke();
        stageManager.mapStory.DoTalkList( stageManager.StageBase.talkBlock, toBattle );
        return;
      }
      toBattle();
    }

  }

  void toBattle() {
    CoroutineCommon.CallWaitForSeconds( .8f, () => {
      FromUnitInfo.RobotInfo.IsMoved = false;
      StartCoroutine( loadBattleAsync() );
    } );
  }

  public void Setup( UnitInfo fromUnitInfo, UnitInfo toUnitInfo, WeaponInfo fromWeapon, WeaponInfo toWeapon, bool fromRight, 
                     Action backToCaller, Action endCaller = null ) {
    this.FromUnitInfo = fromUnitInfo;
    this.ToUnitInfo = toUnitInfo;
    this.FromWeapon = fromWeapon;
    this.fromRight = fromRight;
    this.backToCaller = backToCaller;
    this.endCaller = endCaller;

    //this.ToWeapon = toWeapon == null ? toUnitInfo.RobotInfo.WeaponList[0] : toWeapon;
    if (toWeapon == null) {
      ToWeapon = toUnitInfo.FindWeaponCounter( fromUnitInfo );
    }

    Destroy( myBattleSummary );
    myBattleSummary = Instantiate( m_BattleSummaryPrefab ) as GameObject;
    //culculate();

    FirstAttack = new AttackData( FromUnitInfo, FromWeapon, ToUnitInfo, fromRight, AttackTypeEnum.Normal, CounterTypeEnum.Normal );
    CounterAttack = new AttackData( ToUnitInfo, ToWeapon, FromUnitInfo, !fromRight, ToWeapon == null? AttackTypeEnum.Unable : AttackTypeEnum.Normal, CounterTypeEnum.Normal );
    //FirstAttack.PreData();
    //CounterAttack.PreData();

    updateSummary();
    this.enabled = true;
  }

  /*
  void setBGM() {
    var gms = SceneManager.GetSceneByName( "GameManagerScen" );
    var gameObjectList = gms.GetRootGameObjects();
    var bgmController = gameObjectList.FirstOrDefault( g => g.name == "BGMController" );
    //bgmController.GetComponent<BGMController>().SET_BGM( (AudioClip)Resources.Load( "BGM/07" ) );
    BGMController.SET_BGM( (AudioClip)Resources.Load( "BGM/07" ) );
  }*/
  
  IEnumerator loadBattleAsync() {
    //DontDestroyOnLoad( entry );

    //Scene stageScene = SceneManager.GetSceneByName( "stage_01" );
    Scene stageScene = SceneManager.GetActiveScene();

    GameObject temp = new GameObject( "stageTempGO" );

    GameObject[] allObjects = stageScene.GetRootGameObjects();

    foreach (GameObject go in allObjects) {
      go.transform.SetParent( temp.transform, false );
    }
    //temp.SetActive( false );

    var battleEntryGO = Instantiate( battleEntryPrefab, Vector3.zero, Quaternion.identity ) as GameObject;
    battleEntryGO.name = "BattleEntryGO";
    var entry = battleEntryGO.GetComponent<BattleEntry>();

    prepareBattle( entry );
    
    entry.AllGameObjects = allObjects;
    entry.TempGO = temp;

    AsyncOperation async = SceneManager.LoadSceneAsync( "Battle", LoadSceneMode.Additive );

    while (!async.isDone) {
      yield return new WaitForEndOfFrame();
    }

    //SceneManager.LoadScene( "Battle", LoadSceneMode.Additive );
    Scene battleScene = SceneManager.GetSceneByName( "Battle" );
    SceneManager.MoveGameObjectToScene( battleEntryGO, battleScene );
    SceneManager.SetActiveScene( battleScene );
    GameObject.Find( "BattleManager" ).GetComponent<BattleManager>().MyStart( stageManager.GetSceneName(), stageManager.StageBase.BattleTerrainName, myLight.intensity );
    //Camera.main.GetComponent<AudioListener>().enabled = false;
    temp.SetActive( false );
    //SceneManager.UnloadScene( "stage_01" );
    //GetComponent<MapManager>().BackToMap();

    FromUnitInfo.GetComponent<UnitController>().EndAction();    //測試時, 無限行動
    //FromUnitInfo.GetComponent<UnitController>().myMapManager.BackToMap();
    //GetComponent<MapManager>().BackToMap();
  }

  private void prepareBattle( BattleEntry entry ) {
    entry.AttackDataList = new List<AttackData>();

    FirstAttack.RunResult();
    FirstAttack.AfterData();
    entry.AttackDataList.Add( FirstAttack );

    if (!FirstAttack.IsDefeated) {
      CounterAttack.RunResult();
      CounterAttack.AfterData();
      entry.AttackDataList.Add( CounterAttack );
    }

  }

  /*
  private void prepareBattle_old( BattleEntry entry ) {
    UnitInfo UnitInfoR;
    UnitInfo UnitInfoL;
    string fromSide, toSide;

    if (fromRight) {
      UnitInfoR = FromUnitInfo;
      UnitInfoL = ToUnitInfo;
      fromSide = "R"; toSide = "L";
    }
    else {
      UnitInfoR = ToUnitInfo;
      UnitInfoL = FromUnitInfo;
      fromSide = "L"; toSide = "R";
    }

    entry.RightModelName = UnitInfoR.RobotInfo.RobotInstance.Robot.Name; //"WZ_EW";
    entry.LeftModelName = UnitInfoL.RobotInfo.RobotInstance.Robot.Name; //"T1";

    entry.FullHpR = UnitInfoR.RobotInfo.MaxHP;
    entry.HpR = UnitInfoR.RobotInfo.HP;
    entry.FullEnR = UnitInfoR.RobotInfo.MaxEN;
    entry.EnR = UnitInfoR.RobotInfo.EN;
    entry.FullHpL = UnitInfoL.RobotInfo.MaxHP;
    entry.HpL = UnitInfoL.RobotInfo.HP;
    entry.FullEnL = UnitInfoL.RobotInfo.MaxEN;
    entry.EnL = UnitInfoL.RobotInfo.EN;

    entry.UnitInfoR = UnitInfoR;
    entry.UnitInfoL = UnitInfoL;

    entry.cmdForAnims = new List<CmdForAnim>();

    entry.cmdForAnims.Add( new CmdForAnim() { Side = fromSide, Weapon = FromWeapon.WeaponInstance.Weapon.PlayIndex, TotalDamage = fromDamage, SpendEn = FromWeapon.EN, HitMiss = fromHit } );

    ToUnitInfo.GetComponent<UnitInfo>().RobotInfo.HP -= fromDamage;
    FromUnitInfo.GetComponent<UnitInfo>().RobotInfo.EN -= FromWeapon.EN;
    FromUnitInfo.GetComponent<UnitInfo>().PilotInfo.Willpower += 30;
    //if (ToUnitInfo.GetComponent<UnitInfo>().CurrentRobotInfo.HP > 0) {
    if (ToUnitInfo.RobotInfo.HP > 0) {
      if (ToWeapon != null) {
        entry.cmdForAnims.Add( new CmdForAnim() { Side = toSide, Weapon = ToWeapon.WeaponInstance.Weapon.PlayIndex, TotalDamage = toDamage, SpendEn = ToWeapon.EN, HitMiss = toHit } );
        //FromUnitInfo.GetComponent<UnitInfo>().CurrentRobotInfo.HP -= ToWeapon.HitPoint;
        FromUnitInfo.RobotInfo.HP -= toDamage;
        ToUnitInfo.RobotInfo.EN -= ToWeapon.EN;

        if (FromUnitInfo.RobotInfo.HP <= 0) {
          entry.cmdForAnims.Add( new CmdForAnim() { Side = fromSide, Weapon = -1 } );
        }
      }
    }
    else {
      entry.cmdForAnims.Add( new CmdForAnim() { Side = toSide, Weapon = -1 } );
    }

    //entry.BGM_Name = FromUnitInfo.CurrentRobotInfo.BGM;
  }*/

  private void updateSummary() {
    Transform fromChannel, toChannel;
    if (fromRight) {
      fromChannel = myBattleSummary.transform.Find( "Background/PlayerPanel" );
      toChannel = myBattleSummary.transform.Find( "Background/EnermyPanel" );
    } else {
      toChannel = myBattleSummary.transform.Find( "Background/PlayerPanel" );
      fromChannel = myBattleSummary.transform.Find( "Background/EnermyPanel" );
    }

    fromHitRate = FirstAttack.HitRate;
    toHitRate = CounterAttack.HitRate;

    fromChannel.Find( "HpStatus/HpTxt" ).GetComponent<Text>().text = FromUnitInfo.RobotInfo.HP + "/" + FromUnitInfo.RobotInfo.MaxHP;
    fromChannel.Find( "HpStatus/EnTxt" ).GetComponent<Text>().text = FromUnitInfo.RobotInfo.EN + "/" + FromUnitInfo.RobotInfo.MaxEN;
    fromChannel.Find( "HpStatus/HpSlider" ).GetComponent<Slider>().value = (float)FromUnitInfo.RobotInfo.HP/FromUnitInfo.RobotInfo.MaxHP;
    fromChannel.Find( "HpStatus/EnSlider" ).GetComponent<Slider>().value = (float)FromUnitInfo.RobotInfo.EN / FromUnitInfo.RobotInfo.MaxEN;
    fromChannel.Find( "SideTxt" ).GetComponent<Text>().text = FirstAttack.AttackSingleWord();
    fromChannel.Find( "RobotNameTxt" ).GetComponent<Text>().text = FromUnitInfo.RobotInfo.RobotInstance.Robot.FullName;
    fromChannel.Find( "WeaponNameTxt" ).GetComponent<Text>().text = FromWeapon.WeaponInstance.Weapon.Name;
    fromChannel.Find( "PilotNameTxt" ).GetComponent<Text>().text = FromUnitInfo.PilotInfo.ShortName;
    fromChannel.Find( "WillPowerTxt" ).GetComponent<Text>().text = FromUnitInfo.PilotInfo.Willpower.ToString();
    fromChannel.Find( "LvTxt" ).GetComponent<Text>().text = FromUnitInfo.PilotInfo.Level.ToString();
    fromChannel.Find( "HitRateTxt" ).GetComponent<Text>().text = fromHitRate + "%";

    toChannel.Find( "HpStatus/HpTxt" ).GetComponent<Text>().text = ToUnitInfo.RobotInfo.HP + "/" + ToUnitInfo.RobotInfo.MaxHP;
    toChannel.Find( "HpStatus/EnTxt" ).GetComponent<Text>().text = ToUnitInfo.RobotInfo.EN + "/" + ToUnitInfo.RobotInfo.MaxEN;
    toChannel.Find( "HpStatus/HpSlider" ).GetComponent<Slider>().value = (float)ToUnitInfo.RobotInfo.HP / ToUnitInfo.RobotInfo.MaxHP;
    toChannel.Find( "HpStatus/EnSlider" ).GetComponent<Slider>().value = (float)ToUnitInfo.RobotInfo.EN / ToUnitInfo.RobotInfo.MaxEN;
    toChannel.Find( "SideTxt" ).GetComponent<Text>().text = CounterAttack.CounterSideSingleWord();
    toChannel.Find( "RobotNameTxt" ).GetComponent<Text>().text = ToUnitInfo.RobotInfo.RobotInstance.Robot.FullName;
    toChannel.Find( "WeaponNameTxt" ).GetComponent<Text>().text = CounterAttack.AttackType == AttackTypeEnum.Unable? "反擊不能" : ToWeapon.WeaponInstance.Weapon.Name;
    toChannel.Find( "PilotNameTxt" ).GetComponent<Text>().text = ToUnitInfo.PilotInfo.ShortName;
    toChannel.Find( "WillPowerTxt" ).GetComponent<Text>().text = ToUnitInfo.PilotInfo.Willpower.ToString();
    toChannel.Find( "LvTxt" ).GetComponent<Text>().text = ToUnitInfo.PilotInfo.Level.ToString();
    toChannel.Find( "HitRateTxt" ).GetComponent<Text>().text = CounterAttack.AttackType == AttackTypeEnum.Unable ? "---" : (toHitRate + "%");
  }

#if false
  private void culculate() {
    //fromHitRate = 100 + (FromUnitInfo.PilotInfo.Hit + FromUnitInfo.CurrentRobotInfo.HitRate + FromWeapon.HitRate + 40 - ToUnitInfo.PilotInfo.Dodge - ToUnitInfo.CurrentRobotInfo.Motility );
    //toHitRate = 100 + (ToUnitInfo.PilotInfo.Hit + ToUnitInfo.CurrentRobotInfo.HitRate + ToWeapon.HitRate + 40 - FromUnitInfo.PilotInfo.Dodge - FromUnitInfo.CurrentRobotInfo.Motility);
    fromHitRate = PreBattleFormula.HIT_RATE( FromUnitInfo, ToUnitInfo, FromWeapon );

    //fromHitRateText = fromHitRate > 100? 100 : fromHitRate;
    //fromHitRateText = fromHitRate < 0? 0 : fromHitRateText;

    //fromHit = fromHitRate > UnityEngine.Random.Range( 0, 99 );
    /*
    if (fromHit) {
      fromDamage = FromWeapon.HitPoint;
    } else fromDamage = 0;
    */
    if (ToWeapon != null) {
      toHitRate = PreBattleFormula.HIT_RATE( ToUnitInfo, FromUnitInfo, ToWeapon );

      /*
      toHitRateText = toHitRate > 100 ? 100 : toHitRate;
      toHitRateText = toHitRate < 0 ? 0 : toHitRate;

      toHit = toHitRate > UnityEngine.Random.Range( 0, 99 );

      if (toHit) {
        toDamage = ToWeapon.HitPoint;
      }
      else toDamage = 0;*/
    }
  }
#endif

  private GameObject fromGO;
  private GameObject toGO;
  RectTransform canvasRect;
  // 沒有用了
  public void SetupByAI( UnitInfo fromUnitInfo, UnitInfo toUnitInfo, WeaponInfo fromWeapon, WeaponInfo toWeapon, Action backToCaller ) {
    fromGO = fromUnitInfo.gameObject;
    toGO = toUnitInfo.gameObject;

    //moveCamToCenterObject( fromGO );

    prepareMoveAim( () => {
      //Debug.Log( "Aim End." );

      Setup( fromUnitInfo, toUnitInfo, fromWeapon, toWeapon, fromUnitInfo.Team != UnitInfo.TeamEnum.Enermy, backToCaller );   //From right 可以根據是否味方NPC 再調整
    } );
  }

  private void prepareMoveAim( Action callback ) {
    var myAimCanvas = Instantiate( m_AimPrefab ) as GameObject;

    //first you need the RectTransform component of your canvas
    canvasRect = myAimCanvas.GetComponent<RectTransform>();

    //this is the ui element
    RectTransform uiRect = myAimCanvas.transform.Find( "AimImg" ).GetComponent<RectTransform>();

    //then you calculate the position of the UI element
    //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. 
    //Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
    Vector2 viewportPosition_From = Camera.main.WorldToViewportPoint( fromGO.transform.position );
    Vector2 viewportPosition_To = Camera.main.WorldToViewportPoint( toGO.transform.position );
    //Debug.Log( $"canvasRect.sizeDelta.x {canvasRect.sizeDelta.x}, canvasRect.sizeDelta.y {canvasRect.sizeDelta.y}" );
    //Vector2 ScreenPosition_From = new Vector2( ((viewportPosition_From.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)), ((viewportPosition_From.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)) );
    //Vector2 ScreenPosition_To = new Vector2( ((viewportPosition_To.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)), ((viewportPosition_To.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)) );
    Vector2 ScreenPosition_From = new Vector2( (viewportPosition_From.x - 0.5f) * canvasRect.sizeDelta.x, (viewportPosition_From.y - 0.5f) * canvasRect.sizeDelta.y );
    Vector2 ScreenPosition_To = new Vector2( (viewportPosition_To.x - 0.5f ) * canvasRect.sizeDelta.x, (viewportPosition_To.y - 0.5f) * canvasRect.sizeDelta.y );
    //now you can set the position of the ui element
    uiRect.anchoredPosition = ScreenPosition_From;

    CoroutineCommon.CallWaitForSeconds( .5f, () => {
      StartCoroutine( moveAim( ScreenPosition_From, ScreenPosition_To, uiRect, 1, () => {
        Destroy( myAimCanvas );
        callback();
      } ) );
    } );
  }

  private IEnumerator moveAim( Vector2 ScreenPosition_From, Vector2 ScreenPosition_To, RectTransform uiRect, float remainTime, Action callback ) {
    Vector3 viewPort = new Vector3( 0, 0, 0 );

    for (float timer = 0; timer < remainTime; timer += Time.deltaTime) {
      float progress = timer / 1;
      uiRect.anchoredPosition = Vector2.Lerp( ScreenPosition_From, ScreenPosition_To, progress );
      viewPort = Camera.main.ScreenToViewportPoint( uiRect.anchoredPosition );
      yield return null;
    }

    float targetWaitTime = .5f;
    if (Mathf.Abs( viewPort.x ) > 0.5 || Mathf.Abs( viewPort.y ) > 0.5) {
      GetComponent<MapManager>().MoveCamToCenterObject( toGO );
      //uiRect.anchoredPosition = new Vector2( ((v3Center.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)), ((v3Center.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)) );
      uiRect.anchoredPosition = new Vector2( 0, 0 );
      targetWaitTime = 1f; //1.5f;
    }

    CoroutineCommon.CallWaitForSeconds( targetWaitTime, callback );
  }


}
