using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TerrainHelper;
using System.Linq;

public class SpComMenuController : BaseSelection {

  //public GameObject SpComMenuCanvasPrefab;
  public GameObject spComMenuCanvas;

  private UnitInfo unitInfo;
  //private Action callback;
  //private Action next;

  private List<SPComPilot> spComList;
  private Transform pilotPart;
  private Transform spComDetailTransform;
  private Transform spListTransform;
  private Transform selectionBorderTransform;
  private Text con1, con2, con3;
  private bool[] comListDisable = { false, false, false, false, false, false, false, false };
  private Transform commandDetailTransform;

  private SPComPilot selectSPComPilot;

  private float lastComTime;
  private bool isBlockedAll;
  private float lastBlockTime;

  void Awake() {
    //spComMenuCanvas = Instantiate( SpComMenuCanvasPrefab ) as GameObject;

    pilotPart = spComMenuCanvas.transform.Find( "PilotInfo" );
    spComDetailTransform = spComMenuCanvas.transform.Find( "SpComDetail" );
    commandDetailTransform = spComMenuCanvas.transform.Find( "CommandDetail" );
    con1 = spComDetailTransform.Find( "Content1" ).GetComponent<Text>();
    con2 = spComDetailTransform.Find( "Content2" ).GetComponent<Text>();
    con3 = spComDetailTransform.Find( "Content3" ).GetComponent<Text>();

    spListTransform = spComMenuCanvas.transform.Find( "SpList" );
    selectionBorderTransform = spListTransform.Find( "SelectionBorder" );

    spComMenuCanvas.gameObject.SetActive( false );
    setupBase( 8, 2, false, true );
  }

  // Use this for initialization
  void Start() {

  }

  // Update is called once per frame
  void Update() {

    lastComTime += Time.deltaTime;
    lastBlockTime += Time.deltaTime;
    if (commandDetailTransform.gameObject.activeSelf && lastComTime > 3f) {
      commandDetailTransform.gameObject.SetActive( false );
    }

    if (isBlockedAll && lastBlockTime > 1f) {
      isBlockedAll = false;
      updateInfo();
    }

    moveCursor();
    if (directionEnum != Direction.Zero)
      updateSelected();

    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      //unitStatusCanvas.SetActive( false );
      //Destroy( spComMenuCanvas );
      spComMenuCanvas.gameObject.SetActive( false );
      this.enabled = false;
      isBlockedAll = false;

      callback();  //Back to Map
    }
    else if (Input.GetButtonDown( "Confirm" )) {
      confirm();
    }
  }

  public void Setup( UnitInfo unitInfo, Action callback, Action next ) {
    this.unitInfo = unitInfo;
    this.callback = callback;
    //this.next = next;

    pilotPart.Find( "PilotName" ).GetComponent<Text>().text = commandDetailTransform.Find( "PilotNameText" ).GetComponent<Text>().text = unitInfo.PilotInfo.ShortName;
    pilotPart.Find( "MaxSpText" ).GetComponent<Text>().text = unitInfo.PilotInfo.MaxSp.ToString();

    spComList = unitInfo.PilotInfo.PilotInstance.Pilot.SPComPilots;
    updateInfo();

    reset();
    updateSelected();

    spComMenuCanvas.gameObject.SetActive( true );
    showHideSelf( true );
    commandDetailTransform.gameObject.SetActive( false );
    this.enabled = true;
  }

  private void updateInfo() {
    pilotPart.Find( "SpText" ).GetComponent<Text>().text = unitInfo.PilotInfo.RemainSp.ToString();

    for (var i = 0; i < 8; i++) {
      if (spComList.Count <= i) continue;

      bool isActived = unitInfo.PilotInfo.ActiveSPCommandList.Any( s => s.ID == spComList[i].SPComID );
      bool isLowerLevel = spComList[i].Level > unitInfo.PilotInfo.Level;
      bool isLowerSp = spComList[i].SP > unitInfo.PilotInfo.RemainSp;

      bool isEnable = true;
      try {
        Type spType = Type.GetType( "SPCom_" + spComList[i].SPComID );
        if (Activator.CreateInstance( spType ) is ICheckOnOff checkOnOff)
          isEnable = checkOnOff.IsHighlight( unitInfo.MapFightingUnit );
      }
      catch (Exception) { }

      comListDisable[i] = isLowerLevel || isLowerSp || isActived || isBlockedAll || !isEnable;
      spListTransform.Find( $"SPCom{i+1}" ).GetComponent<Text>().text = !isLowerLevel ? $"{spComList[i].SPCommand.Name} ({spComList[i].SP})" : "？？？ ";
      spListTransform.Find( $"SPCom{i+1}" ).GetComponent<Text>().color = isLowerSp || isLowerLevel || isActived || isBlockedAll || !isEnable? (Color)new Color32(255, 140, 0, 255) : Color.white;
    }
  }

  private float[,] xArray = new float[,] { { 0.13f, 0.32f, 0.51f, 00.7f }, { 0.3f, 0.49f, 0.68f, 0.87f } };
  private float[,] yArray = new float[,] { { 0.17f, 0.08f }, { 0.27f, 0.18f } };

  private void updateSelected() {
    selectSPComPilot = currentSelectedHori < spComList.Count? spComList[currentSelectedHori] : null;

    string[] skillConArr = selectSPComPilot != null ? selectSPComPilot.SPCommand.Desc.Split( '\n' ) : null;

    con1.text = skillConArr?.Length > 0? skillConArr[0] : "— — —";
    con2.text = skillConArr?.Length > 1? skillConArr[1] : "";
    con3.text = skillConArr?.Length > 2? skillConArr[2] : "";

    //Debug.Log( $"currentSelected={currentSelected}, page={page}, row={row}" );
    //Debug.Log( $"min={xArray[0, page-1]}, {yArray[0, row-1]}; Max={xArray[1, page-1]}, {yArray[1, row-1]}" );

    selectionBorderTransform.GetComponent<RectTransform>().anchorMin = new Vector2( xArray[0, page-1], yArray[0, row-1] );
    selectionBorderTransform.GetComponent<RectTransform>().anchorMax = new Vector2( xArray[1, page-1], yArray[1, row-1] );
  }

  private void confirm() {
    if (selectSPComPilot == null || comListDisable[currentSelectedHori]) {
      EffectSoundController.PLAY_ACTION_FAIL();
      return;
    }

    if (selectSPComPilot.SPCommand.Target > 1) {  //可以選擇用在其他機體
      string targetHint = "任意機體";

      List<GameObject> rangeList = new List<GameObject>();
      if (selectSPComPilot.SPCommand.Target == 2) {
        rangeList = GetComponent<MapManager>().stageManager.StageUnits_Player;
        targetHint = "味方";
      }

      if (selectSPComPilot.SPCommand.Target == 3) {
        rangeList = GetComponent<MapManager>().stageManager.StageUnits_Enemy;
        targetHint = "敵方";
      }

      //selectTarget( selectSPComPilot );
      enabled = false;
      showHideSelf( false );
      MyCanvas.SHOW_MSG( $"{selectSPComPilot.SPCommand.Name}: 請選擇使用對象 ({targetHint})" );
      EffectSoundController.PLAY_MENU_CONFIRM();
      GetComponent<SpComSelectTarget>().Setup( unitInfo.transform.position,
        rangeList: rangeList,
        backToCaller: () => {
          showHideSelf( true );
          GetComponent<MapManager>().MoveCamToCenterObject( unitInfo.gameObject );
          enabled = true;
        },
        next: useSpCom
      );
      return;
    }
    else
      useSpCom( unitInfo );  //用在自機
  }

  private void afterUse() {
    unitInfo.PilotInfo.RemainSp -= selectSPComPilot.SP;
    isBlockedAll = true;
    lastBlockTime = 0;
    updateInfo();

    CoroutineCommon.CallWaitForSeconds( 0.2f, () => {
      EffectSoundController.PLAY( (AudioClip)Resources.Load( $"SFX/SpCom/Sp_{selectSPComPilot.SPComID}" ), 2 );
      commandDetailTransform.gameObject.SetActive( true );
      commandDetailTransform.Find( "SpComNameText" ).GetComponent<Text>().text = selectSPComPilot.SPCommand.Name;
      lastComTime = 0;
    } );
  }

  private void showHideSelf(bool showHide) {
    pilotPart.gameObject.SetActive( showHide ); //= spComMenuCanvas.transform.Find( "PilotInfo" );
    spComDetailTransform.gameObject.SetActive( showHide );  // = spComMenuCanvas.transform.Find( "SpComDetail" );
    spListTransform.gameObject.SetActive( showHide );
  }

  void useSpCom( UnitInfo useUnit ) {
    //IConsume consumer;
    Type spType = null;
    try {
      spType = Type.GetType( "SPCom_" + selectSPComPilot.SPCommand.ID );
    }
    catch (Exception) { }

    bool isUsed = false;

    if (spType != null) {
      var consumerObj = Activator.CreateInstance( spType );
      if (consumerObj is IConsumable consumer) {
        consumer.Consume( useUnit.MapFightingUnit );
        isUsed = true;
      }
    }

    if (!useUnit.PilotInfo.ActiveSPCommandList.Any( s => s.ID == selectSPComPilot.SPComID ) && selectSPComPilot.SPCommand.Place > 0) {
      useUnit.PilotInfo.ActiveSPCommandList.Add( selectSPComPilot.SPCommand );
      isUsed = true;

      if (selectSPComPilot.SPCommand.ID == 14) {   //挑發  特殊處理
        useUnit.OtherUnit = unitInfo;
      }

    }

    if (isUsed) {
      showHideSelf( true );
      MyCanvas.Hide_MSG();
      enabled = true;
      afterUse();
    }
    else
      EffectSoundController.PLAY_ACTION_FAIL();
  }

}
