using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TerrainHelper;
using System.Linq;
using UnityEngine.SceneManagement;

public class TitleController : BaseSelection {

  //public GameObject SpComMenuCanvasPrefab;
  //private GameObject spComMenuCanvas;

  private Transform menuTf;
  private Transform startTf;
  private Transform selectionBorderTf;

  private bool[] comListDisable = { false, false, false, true };

  //private SPComPilot selectSPComPilot;

  private int status = 1;  //1 Press Start  2 Menu

  void Awake() {
    //spComMenuCanvas = Instantiate( SpComMenuCanvasPrefab ) as GameObject;

    startTf = transform.Find( "PressStart" );
    menuTf = transform.Find( "Menu" );
    selectionBorderTf = menuTf.Find( "SelectionBorder" );

    //spComMenuCanvas.gameObject.SetActive( false );
    setupBase( 4, 1, false, true );
  }

  // Use this for initialization
  void Start() {
    BGMController.SET_BGM( "2G_時を越えて" );
    //BGMController.SET_BGM( "F_Time To Come" );

    enabled = false;
    CoroutineCommon.CallWaitForSeconds( .5f, () => { enabled = true; } );

    DIContainer.Instance.GameDataService.Reset();
    DIContainer.Instance.PilotService.Reset();
    DIContainer.Instance.RobotService.Reset();
    DIContainer.Instance.PartsService.Reset();
  }

  // Update is called once per frame
  void Update() {
    if (directionEnum != Direction.Zero)
      updateSelected();

    if (status == 2) {
      moveCursor();
      if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
        EffectSoundController.PLAY_BACK_CANCEL();

        menuTf.gameObject.SetActive( false );
        startTf.gameObject.SetActive( true );
        status = 1;
        enabled =false;
        CoroutineCommon.CallWaitForSeconds( .5f, () => { enabled = true; } );

      }
      else if (Input.GetButtonDown( "Confirm" )) {        
        confirm();
      }
    }
    else if (Input.anyKeyDown && !Input.GetMouseButtonDown( 0 ) && !Input.GetMouseButtonDown( 1 ) && !Input.GetMouseButtonDown( 2 )) {
      EffectSoundController.PLAY_MENU_CONFIRM();

      menuTf.gameObject.SetActive( true );
      startTf.gameObject.SetActive( false );
      status = 2;

      reset();
      updateSelected();

      enabled =false;
      CoroutineCommon.CallWaitForSeconds( .5f, () => { enabled = true; } );
    }

  }

  public void Setup() {
    //updateInfo();
    //reset();
    //updateSelected();
    //this.enabled = true;
  }


  private void updateInfo() {
    for (var i = 0; i < 4; i++) {
       menuTf.Find( $"item{i+1}" ).GetComponent<Text>().color = comListDisable[i] ? (Color)new Color32( 50, 50, 50, 255 ) : (Color)new Color32( 192, 192, 192, 255 );
    }   

  }

  private float[,] xArray = new float[,] { { 0.13f, 0.32f, 0.51f, 00.7f }, { 0.3f, 0.49f, 0.68f, 0.87f } };
  private float[,] yArray = new float[,] { { 0.17f, 0.08f }, { 0.27f, 0.18f } };

  private void updateSelected() {
    //selectSPComPilot = currentSelectedHori < spComList.Count ? spComList[currentSelectedHori] : null;

    //Debug.Log( $"currentSelected={currentSelected}, page={page}, row={row}" );
    //Debug.Log( $"min={xArray[0, page-1]}, {yArray[0, row-1]}; Max={xArray[1, page-1]}, {yArray[1, row-1]}" );

    updateInfo();

    selectionBorderTf.GetComponent<RectTransform>().anchorMin = new Vector2( xArray[0, page-1], yArray[0, row-1] );
    selectionBorderTf.GetComponent<RectTransform>().anchorMax = new Vector2( xArray[1, page-1], yArray[1, row-1] );
    if (!comListDisable[page-1])
      menuTf.Find( $"item{page}" ).GetComponent<Text>().color = Color.yellow;

  }

  SaveContinue saveCon;

  private void confirm() {
    enabled = false;
    var mySceneManager = GameObject.Find( "MySceneManager" ).GetComponent<MySceneManager>();

    if (comListDisable[currentSelectedHori]) {
      EffectSoundController.PLAY_ACTION_FAIL();
      CoroutineCommon.CallWaitForSeconds( .1f, () => { enabled = true; } );
      return;
    }

    EffectSoundController.PLAY_MENU_CONFIRM();

    if (currentSelectedHori == 0) { //Start
      GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeOut( 2, () => {
        mySceneManager.UnloadAndLoadScene( "HeroInput", SceneManager.GetActiveScene() );
      }, blackTime: 0f, hold: true );
    }
    else if (currentSelectedHori == 1) { //Load
      GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeOut( .5f, () => {
        mySceneManager.UnloadAndLoadScene( "DataLoad", SceneManager.GetActiveScene() );
      }, blackTime: 0f, hold: true );
    }
    else if (currentSelectedHori == 2) { //Continue
      GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeOut( .5f, 
        () => {
          mySceneManager.UnloadAndLoadStage( "Title" );
        },
        () => {
          try {
            //saveCon = DIContainer.Instance.GameDataService.LoadContinue();
          }
          catch (Exception e) {
            Debug.Log( e.Message );
          }
        }, 
        blackTime: 0f,
        hold: true
      );
    }
    else {
      CoroutineCommon.CallWaitForSeconds( .1f, () => { enabled = true; } );
    }

    //updateInfo();
  }

}
