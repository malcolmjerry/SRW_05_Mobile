using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static AttackData;

public class CounterSummary : SelectableMenu {

  public GameObject battleEntryPrefab;
  //public string stageName;

  [HideInInspector] public UnitInfo FromUnitInfo;
  [HideInInspector] public UnitInfo ToUnitInfo;
  [HideInInspector] public WeaponInfo FromWeapon;
  //[HideInInspector] public WeaponInfo ToWeapon;

  private Action backToCaller;

  //public GameObject m_BattleSummaryPrefab;
  //private GameObject myBattleSummary;

  private int fromHitRate;
  private int toHitRate;

  public GameObject m_AimPrefab;
  private bool fromRight;

  private AttackData firstAttack;      //攻方
  private AttackData counterAttack;    //反方

  private StageManager stageManager;

  public Light myLight;

  private void Awake() {
    menuItemTfList.Add( transform.Find( "ActionList/SPCom1" ) );
    menuItemTfList.Add( transform.Find( "ActionList/SPCom2" ) );
    menuItemTfList.Add( transform.Find( "ActionList/SPCom3" ) );
    menuItemTfList.Add( transform.Find( "ActionList/SPCom4" ) );
  }

  // Start is called before the first frame update
  void Start() {
    stageManager = GameObject.Find( "StageManager" ).GetComponent<StageManager>();
    setupBase( 4, false, true, 1 );
    UpdateSelectedItem();
  }

  protected override void Update() {
    moveCursor();
    if (directionEnum != Direction.Zero)
      UpdateSelectedItem();

    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      //直接轉到武器選擇畫面
      toChooseWeapon();
    }
    else if (Input.GetButtonDown( "Confirm" )) {
      confirm();
    }
    else if (Input.GetButtonDown( "Start" )) { 
      //開始戰鬥
    }
  }

  public GameObject m_WeaponMenuPrefab;
  private GameObject myWeaponMenu;
  private WeaponMenuController wmc;

  protected override void confirm() {
    EffectSoundController.PLAY_MENU_CONFIRM();
    switch (currentSelected) {
      case 0:  //選擇武器
        toChooseWeapon();
        break;
      case 1:  //回避
        firstAttack.CounterType = CounterTypeEnum.Dodge;
        counterAttack.AttackType = AttackTypeEnum.Quit;
        firstAttack.PreBuff();
        break;
      case 2:
        firstAttack.CounterType = CounterTypeEnum.Defense; //防禦
        counterAttack.AttackType = AttackTypeEnum.Quit;
        firstAttack.PreBuff();
        break;
      case 3:
        transform.GetComponent<Canvas>().enabled = false;  //開始戰鬥
        enabled = false;
        //gameObject.SetActive( false );

        CoroutineCommon.CallWaitForOneFrame( () => {
          FromUnitInfo.RobotInfo.IsMoved = false;

          FromUnitInfo.transform.Find( "Renderer" ).rotation = FromUnitInfo.transform.rotation;
          FromUnitInfo.transform.LookAt( ToUnitInfo.transform );
          ToUnitInfo.transform.Find( "Renderer" ).rotation = ToUnitInfo.transform.rotation;
          ToUnitInfo.transform.LookAt( FromUnitInfo.transform );

          if (FromUnitInfo.PilotInfo.PilotInstance.Pilot.BgmPriority > ToUnitInfo.PilotInfo.PilotInstance.Pilot.BgmPriority && !string.IsNullOrWhiteSpace( FromUnitInfo.PilotInfo.PilotInstance.Pilot.BGM ))
            BGMController.SET_BGM( FromUnitInfo.PilotInfo.PilotInstance.Pilot.BGM );
          else
            BGMController.SET_BGM( ToUnitInfo.RobotInfo.RobotInstance.BGM );

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

        } );
        break;
      default:
        break;
    }
    updateSummary();
  }

  void toChooseWeapon() {
    transform.GetComponent<Canvas>().enabled = false;
    enabled = false;

    var weaponList = ToUnitInfo.MapFightingUnit.WeaponList.Where( w => !w.WeaponInstance.Weapon.IsMap ).OrderBy( w => w.HitPoint ).ToList();
    Destroy( myWeaponMenu );
    myWeaponMenu = Instantiate( m_WeaponMenuPrefab ) as GameObject;
    wmc = myWeaponMenu.GetComponent<WeaponMenuController>();
    wmc.Setup( weaponList, false,
      next: () => {
            //ToWeapon = wmc.GetSelectedWeapon();
            //counterAttack = new AttackData( ToUnitInfo, ToWeapon, FromUnitInfo, !fromRight, AttackTypeEnum.Normal, CounterTypeEnum.Normal );
            firstAttack.CounterType = CounterTypeEnum.Normal;
        counterAttack.AttackType = AttackTypeEnum.Normal;
        counterAttack.WeaponInfo = wmc.GetSelectedWeapon();
        firstAttack.PreBuff();
        counterAttack.PreBuff();
        Destroy( myWeaponMenu );
            //gameObject.SetActive( true );
            transform.GetComponent<Canvas>().enabled = true;
        enabled = true;
        updateSummary();
      },
      callback: () => {
        Destroy( myWeaponMenu );
            //gameObject.SetActive( true );
            transform.GetComponent<Canvas>().enabled = true;
        enabled = true;
      },
      fromUnitInfo: FromUnitInfo,
      toUnitInfo: ToUnitInfo
    );
    wmc.enabled = true;
    wmc.SelectFirst();
  }

  void toBattle() {
    CoroutineCommon.CallWaitForSeconds( .8f, () => {
      FromUnitInfo.RobotInfo.IsMoved = false;
      StartCoroutine( loadBattleAsync() );
    } );
  }

  IEnumerator loadBattleAsync() {
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

    Scene battleScene = SceneManager.GetSceneByName( "Battle" );
    SceneManager.MoveGameObjectToScene( battleEntryGO, battleScene );
    SceneManager.SetActiveScene( battleScene );
    GameObject.Find( "BattleManager" ).GetComponent<BattleManager>().MyStart( stageManager.GetSceneName(), stageManager.StageBase.BattleTerrainName, myLight.intensity );
    temp.SetActive( false );

    FromUnitInfo.GetComponent<UnitController>().EndAction();    //測試時, 無限行動
  }

  private void prepareBattle( BattleEntry entry ) {
    entry.AttackDataList = new List<AttackData>();

    firstAttack.RunResult();
    firstAttack.AfterData();
    entry.AttackDataList.Add( firstAttack );

    if (!firstAttack.IsDefeated) {
      counterAttack.RunResult();
      counterAttack.AfterData();
      entry.AttackDataList.Add( counterAttack );
    }

  }

  public void Setup( UnitInfo fromUnitInfo, UnitInfo toUnitInfo, WeaponInfo fromWeapon, WeaponInfo toWeapon, bool fromRight, Action backToCaller ) {
    this.FromUnitInfo = fromUnitInfo;
    this.ToUnitInfo = toUnitInfo;
    this.FromWeapon = fromWeapon;
    this.fromRight = fromRight;
    this.backToCaller = backToCaller;

    //this.ToWeapon = toWeapon == null ? toUnitInfo.RobotInfo.WeaponList[0] : toWeapon;
    if (toWeapon == null) {
      toWeapon = toUnitInfo.FindWeaponCounter( fromUnitInfo );
    }

    //Destroy( myBattleSummary );
    //myBattleSummary = Instantiate( m_BattleSummaryPrefab ) as GameObject;
    //gameObject.SetActive( true );
    transform.GetComponent<Canvas>().enabled = true;
    enabled = true;

    firstAttack = new AttackData( FromUnitInfo, FromWeapon, ToUnitInfo, fromRight, AttackTypeEnum.Normal, CounterTypeEnum.Normal );
    counterAttack = new AttackData( ToUnitInfo, toWeapon, FromUnitInfo, !fromRight, toWeapon == null ? AttackTypeEnum.Unable : AttackTypeEnum.Normal, CounterTypeEnum.Normal );

    reset();
    updateSummary();
  }

  private void updateSummary() {
    Transform fromChannel, toChannel;
    if (fromRight) {
      fromChannel = transform.Find( "Background/PlayerPanel" );
      toChannel = transform.Find( "Background/EnermyPanel" );
    }
    else {
      toChannel = transform.Find( "Background/PlayerPanel" );
      fromChannel = transform.Find( "Background/EnermyPanel" );
    }

    fromHitRate = firstAttack.HitRate;
    toHitRate = counterAttack.HitRate;

    fromChannel.Find( "HpStatus/HpTxt" ).GetComponent<Text>().text = FromUnitInfo.RobotInfo.HP + "/" + FromUnitInfo.RobotInfo.MaxHP;
    fromChannel.Find( "HpStatus/EnTxt" ).GetComponent<Text>().text = FromUnitInfo.RobotInfo.EN + "/" + FromUnitInfo.RobotInfo.MaxEN;
    fromChannel.Find( "HpStatus/HpSlider" ).GetComponent<Slider>().value = (float)FromUnitInfo.RobotInfo.HP / FromUnitInfo.RobotInfo.MaxHP;
    fromChannel.Find( "HpStatus/EnSlider" ).GetComponent<Slider>().value = (float)FromUnitInfo.RobotInfo.EN / FromUnitInfo.RobotInfo.MaxEN;
    fromChannel.Find( "SideTxt" ).GetComponent<Text>().text = firstAttack.AttackSingleWord();
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
    toChannel.Find( "SideTxt" ).GetComponent<Text>().text = counterAttack.CounterSideSingleWord();
    toChannel.Find( "RobotNameTxt" ).GetComponent<Text>().text = ToUnitInfo.RobotInfo.RobotInstance.Robot.FullName;
    toChannel.Find( "WeaponNameTxt" ).GetComponent<Text>().text = getCounterWeaponName();
    toChannel.Find( "PilotNameTxt" ).GetComponent<Text>().text = ToUnitInfo.PilotInfo.ShortName;
    toChannel.Find( "WillPowerTxt" ).GetComponent<Text>().text = ToUnitInfo.PilotInfo.Willpower.ToString();
    toChannel.Find( "LvTxt" ).GetComponent<Text>().text = ToUnitInfo.PilotInfo.Level.ToString();
    toChannel.Find( "HitRateTxt" ).GetComponent<Text>().text = counterAttack.AttackType == AttackTypeEnum.Unable || counterAttack.AttackType == AttackTypeEnum.Quit ? "---" : (toHitRate + "%");
  }

  private string getCounterWeaponName() {
    if (firstAttack.CounterType == CounterTypeEnum.Dodge)
      return "回避";

    if (firstAttack.CounterType == CounterTypeEnum.Defense)
      return "防禦";

    if (counterAttack.AttackType == AttackTypeEnum.Unable)
      return "反擊不能";

    return counterAttack.WeaponInfo.WeaponInstance.Weapon.Name;
  }

  private GameObject fromGO;
  private GameObject toGO;
  RectTransform canvasRect;
  public void SetupByAI( UnitInfo fromUnitInfo, UnitInfo toUnitInfo, WeaponInfo fromWeapon, WeaponInfo toWeapon, Action backToCaller ) {
    fromGO = fromUnitInfo.gameObject;
    toGO = toUnitInfo.gameObject;

    //moveCamToCenterObject( fromGO );

    prepareMoveAim( () => {
      Setup( fromUnitInfo, toUnitInfo, fromWeapon, toWeapon, fromUnitInfo.Team != UnitInfo.TeamEnum.Enermy, backToCaller );   //From right 可以根據是否味方NPC 再調整
      Debug.Log( "Setup By AI, prepareMoveAim OK" );
    } );
  }

  private void prepareMoveAim( Action callback ) {
    var myAimCanvas = Instantiate( m_AimPrefab ) as GameObject;
    canvasRect = myAimCanvas.GetComponent<RectTransform>();
    RectTransform uiRect = myAimCanvas.transform.Find( "AimImg" ).GetComponent<RectTransform>();

    Vector2 viewportPosition_From = Camera.main.WorldToViewportPoint( fromGO.transform.position );
    Vector2 viewportPosition_To = Camera.main.WorldToViewportPoint( toGO.transform.position );

    Vector2 ScreenPosition_From = new Vector2( (viewportPosition_From.x - 0.5f) * canvasRect.sizeDelta.x, (viewportPosition_From.y - 0.5f) * canvasRect.sizeDelta.y );
    Vector2 ScreenPosition_To = new Vector2( (viewportPosition_To.x - 0.5f) * canvasRect.sizeDelta.x, (viewportPosition_To.y - 0.5f) * canvasRect.sizeDelta.y );

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
      uiRect.anchoredPosition = new Vector2( 0, 0 );
      targetWaitTime = 1f; //1.5f;
    }

    CoroutineCommon.CallWaitForSeconds( targetWaitTime, callback );
  }

}
