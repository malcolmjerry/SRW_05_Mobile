using DataModel.Service;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InterMenuController : BaseSelection {

  //private Action callback;
  //private Action next;

  //private Transform totalTf, robotTf, pilotTf, saveTf, loadTf, systemTf, nextTf;
  private Transform totalTf, chapterTf, dataListTf; 
  private List<Transform> menuItemTfList = new List<Transform>(); 

  protected GameDataService gameDataService;

  public AbilityController3 ability;

  void Awake() {
    gameDataService = DIContainer.Instance.GameDataService;
    transform.Find( "RobotList" ).gameObject.SetActive( false );
    transform.Find( "PilotList" ).gameObject.SetActive( false );
    transform.Find( "RobotImprove" ).gameObject.SetActive( false );
    transform.Find( "PartsChange" ).gameObject.SetActive( false );
    transform.Find( "DataList" ).gameObject.SetActive( false );
    transform.Find( "PageArrow" ).gameObject.SetActive( false );
    transform.Find( "ChangeRobotList" ).gameObject.SetActive( false );
    transform.Find( "ChangePilot" ).gameObject.SetActive( false );

    totalTf = transform.Find( "Total" );
    chapterTf = transform.Find( "Chapter" );
    dataListTf = transform.Find( "DataList" );
    /*
    robotTf = transform.Find( "MainMenu/Robot" );
    pilotTf = transform.Find( "MainMenu/Pilot" );
    saveTf = transform.Find( "MainMenu/Save" );
    loadTf = transform.Find( "MainMenu/Load" );
    systemTf = transform.Find( "MainMenu/System" );
    nextTf = transform.Find( "MainMenu/Next" );
    */
    menuItemTfList.Add( transform.Find( "MainMenu/Robot" ) );
    menuItemTfList.Add( transform.Find( "MainMenu/Pilot" ) );
    menuItemTfList.Add( transform.Find( "MainMenu/Save" ) );
    menuItemTfList.Add( transform.Find( "MainMenu/Load" ) );
    menuItemTfList.Add( transform.Find( "MainMenu/System" ) );
    menuItemTfList.Add( transform.Find( "MainMenu/Next" ) );

    setupBase( 6, 3, false, true );

    toggleSelf( true );
  }

  // Use this for initialization
  void Start() {
    Setup();
    updateSelected();
    GetComponent<AbilityController2>().MyAwake();
  }

  // Update is called once per frame
  void Update() {
    moveCursor();
    if (directionEnum != Direction.Zero)
      updateSelected();

    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      //unitStatusCanvas.SetActive( false );
      //Destroy( spComMenuCanvas );
      //spComMenuCanvas.gameObject.SetActive( false );
      //this.enabled = false;

      //callback();  //Back to Map
    }
    else if (Input.GetButtonDown( "Confirm" )) {
      confirm();
    }
  }

  public void Setup() {
    this.enabled = true;

    int bgm = Random.Range( 1, 3 );
    switch (bgm) {
      case 1: BGMController.SET_BGM( "F_出撃準備する" ); break;
      case 2: BGMController.SET_BGM( "AL_苦難の先に待つものは" ); break;
    }

    totalTf.Find( "Content/MoneyText" ).GetComponent<Text>().text = gameDataService.GameData.Money.ToString();
    totalTf.Find( "Content/TurnText" ).GetComponent<Text>().text = gameDataService.GameData.HistoryTurns.ToString();
    chapterTf.Find( "ChapterText" ).GetComponent<Text>().text = gameDataService.GetChapterStr();
  }

  protected virtual void updateSelected() {
    foreach (var menuItem in menuItemTfList) {
      menuItem.Find( "Selected" ).gameObject.SetActive( false );
    }

    menuItemTfList[currentSelectedHori].Find( "Selected" ).gameObject.SetActive( true );
  }

  private void confirm() {    
    switch (currentSelectedHori+1) {
      case 1:
        EffectSoundController.PLAY_MENU_CONFIRM();
        toggleSelf( false );
        var robotListView = transform.Find( "RobotList" );
        robotListView.gameObject.SetActive( true );
        robotListView.GetComponent<RobotListController>().Setup( () => { toggleSelf( true ); } );
        break;
      case 2:
        EffectSoundController.PLAY_MENU_CONFIRM();
        toggleSelf( false );
        var pilotListView = transform.Find( "PilotList" );
        pilotListView.gameObject.SetActive( true );
        pilotListView.GetComponent<PilotListController>().Setup( () => { toggleSelf( true ); } );
        break;
      case 3:
        EffectSoundController.PLAY_MENU_CONFIRM();
        toggleSelf( false );
        dataListTf.gameObject.SetActive( true );
        dataListTf.GetComponent<DataListController>().Setup( true, () => { toggleSelf( true ); } );
        break;
      case 4:
        EffectSoundController.PLAY_MENU_CONFIRM();
        toggleSelf( false );
        dataListTf.gameObject.SetActive( true );
        dataListTf.GetComponent<DataListController>().Setup( false, () => { toggleSelf( true ); } );
        break;
      case 5:
        EffectSoundController.PLAY_ACTION_FAIL();
        break;
      case 6:
        //EffectSoundController.PLAY_MENU_CONFIRM();
        EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/SpCom/Sp_2" ) );
        this.enabled = false;

        GameObject.Find( "MyCanvas" ).transform.Find( "ConfirmDialog" ).GetComponent<ConfirmDialog>().Setup(
          "進入下一關",
          () => {
            GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeOut(
              sec: .5f,  //測試 .5, 正式 2f
              callback: () => {
                gameDataService.SaveBeforeStage();
                var mySceneManager = GameObject.Find( "MySceneManager" ).GetComponent<MySceneManager>();

                if (gameDataService.GameData.Stage == 1)
                  mySceneManager.UnloadAndLoadScene( "Prologue", SceneManager.GetActiveScene(), true );
                else if (gameDataService.GameData.Stage == 4)
                  mySceneManager.UnloadAndLoadScene( "Ending", SceneManager.GetActiveScene(), true );
                else
                  mySceneManager.UnloadAndLoadScene( "Story", SceneManager.GetActiveScene() );

                //SceneManager.UnloadSceneAsync( "InterMission" );
                //SceneManager.LoadScene( "Story", LoadSceneMode.Additive );
              },
              hold: true
            );
            ;
          },
          () => {
            this.enabled = true;
          }
        );

        break;
      default:
        break;
    }
  }

  private void toggleSelf( bool toggle ) {
    transform.Find( "MainMenu" ).gameObject.SetActive( toggle );
    totalTf.Find( "Content" ).gameObject.SetActive( toggle );
    transform.Find( "Chapter" ).gameObject.SetActive( toggle );
    enabled = toggle;

    if (toggle)
      totalTf.Find( "Content/MoneyText" ).GetComponent<Text>().text = gameDataService.GameData.Money.ToString();
  }

}
