using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static TerrainHelper;
using System.Linq;
using DataModel.Service;

public class DataListController : SelectableMenu {

  //private Action callback;
  //private Action next;

  //private Transform totalTf, robotTf, pilotTf, saveTf, loadTf, systemTf, nextTf;
  private Transform headerTf;
  //private List<Transform> menuItemTfList = new List<Transform>();
  private Transform row10, row11;  //展示 詳細資料, 如主角名字, 資金 等

  protected GameDataService gameDataService;
  protected PilotService pilotService;
  private List<GameData> dataList;

  private bool isSave;

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
  }

  int? lastIndex = null;
  void OnEnable() {
    dataList = gameDataService.GetAll();
    setupBase( dataList.Count, false, true );
    if (lastIndex.HasValue) {
      SetPageAndRowBySelected( lastIndex.Value );
    }

    updateDisplayList();
    updateSelected();
    transform.parent.Find( "PageArrow" ).gameObject.SetActive( maxPage > 1 );
  }

  void OnDisable() {
    transform.parent.Find( "PageArrow" ).gameObject.SetActive( false );
  }

  // Update is called once per frame
  protected override void Update() {
    moveCursor();
    if (directionEnum != Direction.Zero) {
      updateDisplayList();
      updateSelected();
    }

    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      EffectSoundController.PLAY_BACK_CANCEL();
      closeSelf();
    }
    else if (Input.GetButtonDown( "Confirm" )) {
      confirm();
    }
    else if (Input.GetButtonDown( "CursorSpeed" ) || Input.GetButtonDown( "Info" )) {
      doDelete();
    }
  }

  public void Setup( bool isSave, Action callback ) {
    this.isSave = isSave;
    //this.callback = callback;
    base.Setup( callback );
  }

  private void updateDisplayList() {
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

  }

  private void updateSelected() {
    UpdateSelectedItem();
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
      //var hero1 = pilotService.LoadHeroPilotInstance( 1 );
      //var hero2 = pilotService.LoadHeroPilotInstance( 2 );
      //row10.Find( "HeroName" ).GetComponent<Text>().text = hero1.Hero.ShortName;
      //row10.Find( "Lv" ).GetComponent<Text>().text = (hero1.Exp / 500 + 1).ToString();
      //row10.Find( "Kills" ).GetComponent<Text>().text = hero1.Kills.ToString(); 
      //row11.Find( "Herorine" ).GetComponent<Text>().text = hero2.Hero.ShortName;
      row10.Find( "HeroName" ).GetComponent<Text>().text = gameData.HeroName;
      row10.Find( "Lv" ).GetComponent<Text>().text = gameData.HeroLv.ToString();
      row10.Find( "Kills" ).GetComponent<Text>().text = gameData.HeroKills.ToString(); 
      row11.Find( "Herorine" ).GetComponent<Text>().text = gameData.Herorine;

      row11.Find( "Money" ).GetComponent<Text>().text = $"{gameData.Money}";
      row11.Find( "HistoryMoney" ).GetComponent<Text>().text = $"{gameData.HistoryMoney}";
    }

    headerTf.Find( "Page" ).GetComponent<Text>().text = page.ToString();
    headerTf.Find( "MaxPage" ).GetComponent<Text>().text = maxPage.ToString();
  }

  protected override void confirm() {
    //transform.Find( "RobotMenu" ).gameObject.SetActive( true );
    this.enabled = false;
    if (isSave) {
      doSave();
    }
    else {
      doLoad();
    }
  }

  private void doSave() {
    EffectSoundController.PLAY_MENU_CONFIRM();
    GameData savedGameData = dataList[currentSelected];

    if (!string.IsNullOrWhiteSpace( savedGameData.SaveTime )) {
      GameObject.Find( "MyCanvas" ).transform.Find( "ConfirmDialog" ).GetComponent<ConfirmDialog>().Setup(
        "保存後, 該儲存格的數據將會被覆蓋, 永遠無法復原.",
        () => { processSave(); },
        () => { this.enabled = true; } //Cancel
      );
    }
    else
      processSave();
  }

  private void processSave() {
    lastIndex = currentSelected;
    this.enabled = false;  //Save OK
    //gameDataService.Save( gameData.SaveSlot );
    var gameData = gameDataService.GameData;

    GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeOut( 
      sec: .5f, 
      callback: () => {
        enabled = true;
      }, 
      doProcess: () => {
        Debug.Log( "start save: " + DateTime.Now.ToString( "yyyyMMdd HH:mm:ss fff" ) );
        var hero1 = pilotService.LoadHeroPilotInstance( 1 );
        var hero2 = pilotService.LoadHeroPilotInstance( 2 );
        gameData.HeroName = hero1.Hero.ShortName;
        gameData.HeroLv = hero1.Exp / 500 + 1;
        gameData.HeroKills = hero1.Kills;
        gameData.Herorine = hero2.Hero.ShortName;
        gameDataService.Save( currentSelected + 1 );
        Debug.Log( "finish save: " + DateTime.Now.ToString( "yyyyMMdd HH:mm:ss fff" ) );
      },
      blackTime: 0f
    );
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
      "讀取Data後, 當前未儲存的數據將會丟失.",
      () => {
        //gameDataService.Load( gameData.SaveSlot );
        GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeOut( .5f, () => {
          enabled = true;
          closeSelf();
          transform.parent.GetComponent<InterMenuController>().Setup();
        },
        () => {
          Debug.Log( "start load: " + DateTime.Now.ToString( "yyyyMMdd HH:mm:ss fff" ) );
          gameDataService.Load( gameData.SaveSlot );
          Debug.Log( "start load: " + DateTime.Now.ToString( "yyyyMMdd HH:mm:ss fff" ) );
        },
        blackTime: 0f
        );
      },
      () => {
        this.enabled = true;
        //Debug.Log( "Load is cancelled..." );
      }
    );
  }

  private void doDelete() {
    GameData gameData = dataList[currentSelected];

    if (string.IsNullOrWhiteSpace( gameData.SaveTime )) {
      EffectSoundController.PLAY_ACTION_FAIL();
      return;
    }

    EffectSoundController.PLAY_MENU_CONFIRM();
    lastIndex = currentSelected;
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

}

