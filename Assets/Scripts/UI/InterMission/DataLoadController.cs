using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TerrainHelper;
using System.Linq;
using DataModel.Service;
using UnityEngine.SceneManagement;

public class DataLoadController : SelectableMenu {

  //private Action callback;
  //private Action next;

  //private Transform totalTf, robotTf, pilotTf, saveTf, loadTf, systemTf, nextTf;
  private Transform headerTf;
  //private List<Transform> menuItemTfList = new List<Transform>();
  private Transform row10, row11;  //展示 詳細資料, 如主角名字, 資金 等

  protected GameDataService gameDataService;
  protected PilotService pilotService;
  private List<GameData> dataList;

  void Awake() {
    headerTf = transform.Find( "Header" );

    menuItemTfList.Add( transform.Find( "Row1" ) ); menuItemTfList.Add( transform.Find( "Row2" ) ); menuItemTfList.Add( transform.Find( "Row3" ) );
    menuItemTfList.Add( transform.Find( "Row4" ) ); menuItemTfList.Add( transform.Find( "Row5" ) ); menuItemTfList.Add( transform.Find( "Row6" ) );
    menuItemTfList.Add( transform.Find( "Row7" ) ); menuItemTfList.Add( transform.Find( "Row8" ) ); 

    row10 = transform.Find( "Row10" );
    row11 = transform.Find( "Row11" );

    gameDataService = DIContainer.Instance.GameDataService;
    pilotService = DIContainer.Instance.PilotService;
  }

  // Use this for initialization
  void Start() {
    //unitList = gameDataService.MapFightingUnits.OrderByDescending( mfu => mfu.RobotInfo.MaxHP ).ToList();
    //setupBase( unitList.Count, 9, true, true );

    //updateDisplayList();
    //updateSelected();
    BGMController.SET_RANDOM_BGM( "F_出撃準備する", "AL_苦難の先に待つものは" );

    callback = () => {
      Debug.Log( "DataLoad -> callback" );
      GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeOut( .5f, 
        () => {
          var mySceneManager = GameObject.Find( "MySceneManager" ).GetComponent<MySceneManager>();
          //StartCoroutine( mySceneManager.UnloadAndLoadStageAsync( SceneManager.GetActiveScene().name ) );
          //StartCoroutine( UnloadAndLoadTitle() );
          mySceneManager.UnloadAndLoadScene( "Title", SceneManager.GetActiveScene() );
        },
        doProcess: null, 
        blackTime: 0f,
        hold: true
      );
    };
  }

  void OnEnable() {
    dataList = gameDataService.GetAll();
    setupBase( dataList.Count, false, true );

    UpdateDisplayList();
    UpdateSelectedItem();
    transform.parent.Find( "PageArrow" ).gameObject.SetActive( maxPage > 1 );
  }

  void OnDisable() {
    transform.parent.Find( "PageArrow" ).gameObject.SetActive( false );
  }

  //protected override void Update() {}

  public override void UpdateDisplayList() {
    var pageList = GetPageList<GameData>( dataList );
    
    for (int i = 0; i<maxRow; i++) {
      var item = menuItemTfList[i];
      GameData gameData = pageList[i];

      if (string.IsNullOrWhiteSpace( gameData.SaveTime )) {
        item.Find( "Chapter" ).GetComponent<Text>().text = "—　—　—";
        item.Find( "StageName" ).GetComponent<Text>().text = "—　—　—　—　—　—";
        item.Find( "Turns" ).GetComponent<Text>().text = "- -";
        item.Find( "SaveTime" ).GetComponent<Text>().text = "—　—　—　—　—　—";
      }
      else {
        item.Find( "Chapter" ).GetComponent<Text>().text = $"第{gameData.Chapter}話";
        string stageName = StageMap.StageMapList[gameData.Stage];
        item.Find( "StageName" ).GetComponent<Text>().text = $"{stageName}";
        item.Find( "Turns" ).GetComponent<Text>().text = $"{gameData.HistoryTurns}";
        item.Find( "SaveTime" ).GetComponent<Text>().text = $"{gameData.SaveTime}";
      }
    }
    headerTf.Find( "Page" ).GetComponent<Text>().text = page.ToString();
    headerTf.Find( "MaxPage" ).GetComponent<Text>().text = maxPage.ToString();
  }

  protected override void UpdateSelectedItem() {
    base.UpdateSelectedItem();
    GameData gameData = dataList[currentSelected];
    row10.Find( "FileNo" ).GetComponent<Text>().text = $"{gameData.SaveSlot.ToString().PadLeft( 2, '0' )}";

    if (string.IsNullOrWhiteSpace( gameData.SaveTime )) {
      row10.Find( "HeroName" ).GetComponent<Text>().text = "—　—　—　—　—　—";
      row10.Find( "Lv" ).GetComponent<Text>().text = "--";
      row10.Find( "Kills" ).GetComponent<Text>().text = "- - -";
      row11.Find( "Herorine" ).GetComponent<Text>().text = $"—　—　—　—　—　—";
      row11.Find( "Money" ).GetComponent<Text>().text = $"- - - - -";
      row11.Find( "HistoryMoney" ).GetComponent<Text>().text = $"- - - - -";
    }
    else {
      row10.Find( "HeroName" ).GetComponent<Text>().text = gameData.HeroName;
      row10.Find( "Lv" ).GetComponent<Text>().text = gameData.HeroLv.ToString();
      row10.Find( "Kills" ).GetComponent<Text>().text = gameData.HeroKills.ToString();
      row11.Find( "Herorine" ).GetComponent<Text>().text = gameData.Herorine;
      row11.Find( "Money" ).GetComponent<Text>().text = $"{gameData.Money}";
      row11.Find( "HistoryMoney" ).GetComponent<Text>().text = $"{gameData.HistoryMoney}";
    }

    //headerTf.Find( "Page" ).GetComponent<Text>().text = page.ToString();
    //headerTf.Find( "MaxPage" ).GetComponent<Text>().text = maxPage.ToString();
  }

  protected override void confirm() {
    enabled = false;
    doLoad();
   
  }

  private void doLoad() {
    GameData gameData = dataList[currentSelected];

    if (string.IsNullOrWhiteSpace( gameData.SaveTime )) {
      EffectSoundController.PLAY_ACTION_FAIL();
      this.enabled = true;
      return;
    }

    EffectSoundController.PLAY_MENU_CONFIRM();
    this.enabled = false;
    GameObject.Find( "MyCanvas" ).transform.Find( "ConfirmDialog" ).GetComponent<ConfirmDialog>().Setup(
      "讀取存檔.",
      () => {
        GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeOut( .5f, 
          callback: () => {
            //enabled = true;
            //closeSelf();
            //transform.parent.GetComponent<InterMenuController>().Setup();
            //gameDataService.Load( gameData.SaveSlot );
            var mySceneManager = GameObject.Find( "MySceneManager" ).GetComponent<MySceneManager>();
            mySceneManager.UnloadAndLoadScene( "InterMission", SceneManager.GetActiveScene() );
          },
          doProcess: () => { gameDataService.Load( gameData.SaveSlot ); },
          blackTime: 0f,
          hold: true
        );
      },
      () => {
        enabled = true;
      }
    );
  }

  private void doDelete() {
    GameData gameData = dataList[currentSelected];

    if (string.IsNullOrWhiteSpace( gameData.SaveTime )) {
      EffectSoundController.PLAY_ACTION_FAIL();
      this.enabled = true;
      return;
    }

    EffectSoundController.PLAY_MENU_CONFIRM();
    this.enabled = false;
    GameObject.Find( "MyCanvas" ).transform.Find( "ConfirmDialog" ).GetComponent<ConfirmDialog>().Setup(
      "你正在刪除紀錄, 刪除後, Data永遠無法再復原.",
      () => {
        GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeOut(
          sec: 1f,
          callback: () => { this.enabled = true; },
          doProcess: () => { gameDataService.Delete( gameData.SaveSlot ); },
          blackTime: 0f
        );
      },
      () => {
        this.enabled = true;
      }
    );
  }

  protected override void CursorSpeed() {
    doDelete();
  }
  protected override void Info() {
    doDelete();
  }

}

