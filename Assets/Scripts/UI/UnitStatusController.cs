using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TerrainHelper;

public class UnitStatusController : MonoBehaviour {

  public GameObject unitStatusCanvasPrefab;
  public GameObject unitStatusCanvas;

  private UnitInfo unitInfo;
  private Action callback;
  private Action next;

  void Awake() {

  }

  // Use this for initialization
  void Start() {
    //unitStatusCanvas = Instantiate( unitStatusCanvasPrefab ) as GameObject;
    //unitStatusCanvas.SetActive( false );
  }

  // Update is called once per frame
  void Update() {
    /*
    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      //unitStatusCanvas.SetActive( false );
      Destroy( unitStatusCanvas );
      this.enabled = false;

      callback();
    }
    else if (Input.GetButtonDown( "Confirm" )) {
      EffectSoundController.PLAY_MENU_CONFIRM();
      Destroy( unitStatusCanvas );
      this.enabled = false;

      showInfo();
    }
    */
  }

  public void Setup( UnitInfo unitInfo, Action callback = null ) {
    this.unitInfo = unitInfo;
    this.callback = callback;

    //unitStatusCanvas = Instantiate( unitStatusCanvasPrefab ) as GameObject;
    //unitStatusCanvas.SetActive( true );

    var pilotPart = unitStatusCanvas.transform.Find( "CommonBody" ).Find( "PilotPart" );
    var robotPart = unitStatusCanvas.transform.Find( "CommonBody" ).Find( "RobotPart" );

    pilotPart.Find( "PilotName" ).GetComponent<Text>().text = unitInfo.PilotInfo.ShortName;
    pilotPart.Find( "PilotPic" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + unitInfo.PilotInfo.PicNo + "_1" );
    pilotPart.Find( "LvText" ).GetComponent<Text>().text = unitInfo.PilotInfo.Level.ToString();
    pilotPart.Find( "WillPowerText" ).GetComponent<Text>().text = unitInfo.PilotInfo.Willpower.ToString();
    pilotPart.Find( "ExpText" ).GetComponent<Text>().text = unitInfo.PilotInfo.PilotInstance.Exp.ToString();
    pilotPart.Find( "SpText" ).GetComponent<Text>().text = unitInfo.PilotInfo.RemainSp.ToString();
    pilotPart.Find( "ExpText" ).GetComponent<Text>().text = unitInfo.PilotInfo.NextLevel.ToString();
    pilotPart.Find( "KillsText" ).GetComponent<Text>().text = unitInfo.PilotInfo.PilotInstance.Kills.ToString();

    robotPart.Find( "RobotName" ).GetComponent<Text>().text = unitInfo.RobotInfo.RobotInstance.Robot.FullName;
    robotPart.Find( "HpTxt" ).GetComponent<Text>().text = unitInfo.RobotInfo.HP + " / " + unitInfo.RobotInfo.MaxHP;
    robotPart.Find( "HpSlider" ).GetComponent<Slider>().value = (float)unitInfo.RobotInfo.HP / unitInfo.RobotInfo.MaxHP;
    robotPart.Find( "EnTxt" ).GetComponent<Text>().text = unitInfo.RobotInfo.EN + " / " + unitInfo.RobotInfo.MaxEN;
    robotPart.Find( "EnSlider" ).GetComponent<Slider>().value = (float)unitInfo.RobotInfo.EN / unitInfo.RobotInfo.MaxEN;
    robotPart.Find( "MovePowerText" ).GetComponent<Text>().text = unitInfo.MapFightingUnit.MovePower.ToString();
    robotPart.Find( "SizeText" ).GetComponent<Text>().text = unitInfo.RobotInfo.GetSizeStr();   
    robotPart.Find( "MoveTypeText" ).GetComponent<Text>().text = unitInfo.RobotInfo.GetMoveTypeStr();
    robotPart.Find( "SkyText" ).GetComponent<Text>().text = GET_TerrainRank( GetTerrain( unitInfo.MapFightingUnit, TerrainEnum.Sky ) );
    robotPart.Find( "LandText" ).GetComponent<Text>().text = GET_TerrainRank( GetTerrain( unitInfo.MapFightingUnit, TerrainEnum.Land ) );
    robotPart.Find( "SeaText" ).GetComponent<Text>().text = GET_TerrainRank( GetTerrain( unitInfo.MapFightingUnit, TerrainEnum.Sea ) );
    robotPart.Find( "SpaceText" ).GetComponent<Text>().text = GET_TerrainRank( GetTerrain( unitInfo.MapFightingUnit, TerrainEnum.Space ) );

    showSpComList();

    //this.enabled = true;
    unitStatusCanvas.SetActive( true );
    var canvasRect = unitStatusCanvas.GetComponent<RectTransform>();
    RectTransform uiRect = unitStatusCanvas.transform.Find( "CommonBody" ).GetComponent<RectTransform>();
    Vector2 viewportPositionUnit = Camera.main.WorldToViewportPoint( unitInfo.transform.position );
    Vector2 previewPos = viewportPositionUnit.x >= 0.5f ? new Vector2( 0.25f, 0.5f ) : new Vector2( 0.75f, 0.5f );
    Vector2 previewPosInCanvas = new Vector2( (previewPos.x - 0.5f) * canvasRect.sizeDelta.x, (previewPos.y - 0.5f) * canvasRect.sizeDelta.y );
    uiRect.anchoredPosition = previewPosInCanvas;
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/computer-beep-1" ) );
  }

  private void showSpComList() {
    var body = unitStatusCanvas.transform.Find( "CommonBody" );

    // 一共有20個精神使用狀態
    for (int i=1; i<=20; i++)
      body.Find( $"SP{i}Text" ).GetComponent<Text>().color = new Color32( 100, 100, 100, 255 );

    foreach (var spCom in unitInfo.PilotInfo.ActiveSPCommandList) {
      body.Find( $"SP{spCom.Place}Text" ).GetComponent<Text>().color = new Color32( 200, 200, 255, 255 );
    } 

  }
  /*
  private void showInfo() {
    GetComponent<AbilityController>().Setup( unitInfo.MapFightingUnit, callback );
  }
  */
  public void SetInactive() {
    unitStatusCanvas?.SetActive( false );
  }
}
