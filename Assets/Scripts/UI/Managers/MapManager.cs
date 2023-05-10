using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using System.Linq;
//using UnityEditor;

public class MapManager : MonoBehaviour {

  //public GameObject m_MapCursorPrefab;
  public GameObject myMapCursor;

  public GameObject TotalBox;
  public GameObject ObjectiveBox;

  public GameObject m_MapMenuPrefab;
  public GameObject myUnitMenu;  //2021-11-28 changed to public

  private int myStatus = 0; // 0: move cursor  1. menu1  2. Doing Other
  public enum MapManStatus { MOVE_CUR = 0, Menu1, DOING_OTHER, SHOW_MOVE_RANGE }

  private Menu menu = null;
  [HideInInspector] public UnitController unitSelected;
  [HideInInspector] public UnitWeaponController unitWC;

  public StageManager stageManager;
  private GameObject myCanvasGo;

  public GameObject LaunchListCanvasGo;
  public GameObject UnitListCanvasGo;

  public AbilityController3 ability;

  MySRWInput mySRWInput;

  // Use this for initialization
  void Awake() {
    mySRWInput = new MySRWInput();
    //myUnitMenu = Instantiate( m_MapMenuPrefab ) as GameObject;
    //myUnitMenu.SetActive( false );

    mySRWInput.UI.Submit.performed += ctx => {
      if (myStatus == (int)MapManStatus.MOVE_CUR) {   //正在地圖上移動游標
        EffectSoundController.PLAY_MENU_CONFIRM();
        ShowMenu();
      }
      else if (myStatus == (int)MapManStatus.Menu1) {   //正在顯示一級菜單
        processMenuCmd();
      }
      else if (myStatus == (int)MapManStatus.SHOW_MOVE_RANGE) {   //正在顯示移動範圍
        exitMoveRangeMode();
        CoroutineCommon.CallWaitForOneFrame( () => {
          EffectSoundController.PLAY_MENU_CONFIRM();
          ShowMenu();
        } );
      }
    };
  }

  void Start() {
    myUnitMenu.SetActive( false );
    myMapCursor.GetComponent<Cursor>().MODE_ENUM = Cursor.ModeEnum.MAP;
    unitWC = GetComponent<UnitWeaponController>();
    //GetComponent<AbilityController2>().MyAwake();

    myCanvasGo = GameObject.Find( "MyCanvas" );
  }

  // Update is called once per frame
  void Update() {
    if (myStatus == (int)MapManStatus.MOVE_CUR) {   //正在地圖上移動游標
      if (Input.GetButtonDown( "Confirm" )) {
        //audioSource.PlayOneShot( (AudioClip)Resources.Load( "SFX/menuConfirm" ) );
        //EffectSoundController.PLAY_MENU_CONFIRM();   //Replaced with new Input System
        //ShowMenu();  //Replaced with new Input System
      }
      else if (Input.GetButtonDown( "Info" )) {
        if (myMapCursor.GetComponent<Cursor>().unitSelected != null) {
          EffectSoundController.PLAY_MENU_CONFIRM();
          showInfo();
        }
        else EffectSoundController.PLAY_ACTION_FAIL();
      }
      else if (Input.GetButtonDown( "Back" )) {
        EffectSoundController.PLAY_MENU_CONFIRM();

        unitSelected = myMapCursor.GetComponent<Cursor>().unitSelected;
        if (unitSelected)
          toMoveRangeMode();
        else {
          //顯示地形信息
        }
      }
      else if (Input.GetButtonDown( "NextUnit" )) {
        findUnitAndPlaceCursorOnIt( stageManager.StageUnits_Player_OnMap, UnitInfo.TeamEnum.Player, 1 );
      }
      else if (Input.GetButtonDown( "PrevUnit" )) {
        findUnitAndPlaceCursorOnIt( stageManager.StageUnits_Player_OnMap, UnitInfo.TeamEnum.Player, -1 );
      }
      else if (Input.GetButtonDown( "NextEnemy" )) {
        findUnitAndPlaceCursorOnIt( stageManager.StageUnits_Enemy, UnitInfo.TeamEnum.Enermy, 1 );
      }
      else if (Input.GetButtonDown( "PrevEnemy" )) {
        findUnitAndPlaceCursorOnIt( stageManager.StageUnits_Enemy, UnitInfo.TeamEnum.Enermy, -1 );
      }
    }
    else if (myStatus == (int)MapManStatus.Menu1) {   //正在顯示一級菜單
      if (menu.status == 0 && (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" ))) {
        //這幾行沒有用, 由 menu 自己處理後 再返回 map
        //myUnitMenu.SetActive( false );
        //audioSource.PlayOneShot( (AudioClip)Resources.Load( "SFX/menuBack" ), 4 );
        //EffectSoundController.PLAY_BACK_CANCEL();
        //BackToMap( false );
      }
      /* //Replaced with new Input System
      else if (Input.GetButtonDown( "Confirm" )) {
        processMenuCmd();
      }*/
    }
    else if (myStatus == (int)MapManStatus.SHOW_MOVE_RANGE) {   //正在顯示移動範圍
      if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" ))
        exitMoveRangeMode();
      else if (Input.GetButtonDown( "Info" )) {
        exitMoveRangeMode();
        CoroutineCommon.CallWaitForOneFrame( () => {
          EffectSoundController.PLAY_MENU_CONFIRM();
          showInfo();
        } );
      }
      /* //Replaced with new Input System
      else if (Input.GetButtonDown( "Confirm" )) {
        exitMoveRangeMode();
        CoroutineCommon.CallWaitForOneFrame( () => {
          EffectSoundController.PLAY_MENU_CONFIRM();
          ShowMenu();
        } );
      }*/
    }
  }

  public void ShowMenu() {
    menu = myUnitMenu.GetComponent<Menu>();
    unitSelected = myMapCursor.GetComponent<Cursor>().unitSelected;

    if (myMapCursor.GetComponent<Cursor>().unitSelected) {
      //unitSelected = myMapCursor.GetComponent<Cursor>().unitSelected;
      unitSelected.myMapManager = this;
      //Destroy( myUnitMenu );
      //myUnitMenu = Instantiate( m_MapMenuPrefab ) as GameObject;
      menu.CmEnumList = new List<Menu.CmdEnum>();

      if (unitSelected.GetComponent<UnitInfo>().Team != UnitInfo.TeamEnum.Player) {
        menu.CmEnumList = new List<Menu.CmdEnum> { Menu.CmdEnum.INFO };
        menu.createCommands( Menu.CMD_WIDTH_SHORT, BackToMap );
        doShowMenu();
        return;
      }

      /*
      if (unitSelected.status == (int)UnitController.UnitStatusEnum.ACTION_END ||
          !(new List<int>() { LayerMask.NameToLayer( "Player" ), LayerMask.NameToLayer( "Friend_NPC" ) }.Contains( unitSelected.gameObject.layer ))) {
        menu.CmEnumList = new List<Menu.CmdEnum> { Menu.CmdEnum.INFO };
      }
      else {
        menu.CmEnumList = new List<Menu.CmdEnum> { Menu.CmdEnum.MOVE, Menu.CmdEnum.ATK, Menu.CmdEnum.SPCMD, Menu.CmdEnum.INFO };
      }
      */
      if (unitSelected.status == UnitController.UnitStatusEnum.ACTION_END)
        menu.CmEnumList = new List<Menu.CmdEnum> { Menu.CmdEnum.INFO };
      else {
        menu.CmEnumList = new List<Menu.CmdEnum> { Menu.CmdEnum.MOVE, Menu.CmdEnum.ATK, Menu.CmdEnum.SPCMD, Menu.CmdEnum.INFO };

        if (unitSelected.GetComponent<UnitInfo>().ShipUnit)
          menu.CmEnumList.Insert( menu.CmEnumList.Count - 1, Menu.CmdEnum.ON_SHIP );
      }

      if (unitSelected.GetComponent<UnitInfo>().StoringUnits?.Count > 0)
        menu.CmEnumList.Insert( menu.CmEnumList.Count - 1, Menu.CmdEnum.OFF_SHIP );

      menu.createCommands( Menu.CMD_WIDTH_SHORT, BackToMap );
    }
    else {
      menu.CmEnumList = new List<Menu.CmdEnum> {
        Menu.CmdEnum.PHASE_END, Menu.CmdEnum.TEAMLIST, Menu.CmdEnum.ENEMYLIST,
        Menu.CmdEnum.OBJECTIVE, Menu.CmdEnum.BACK_TO_MAIN, /*Menu.CmdEnum.MAPLOAD,*/ Menu.CmdEnum.MAPSAVE };
      menu.createCommands( Menu.CMD_WIDTH_LONG, BackToMap );
    }
    doShowMenu();
  }

  /*
  private void showStatus() {
    //Debug.Log( "This is show status.." );
    if (myMapCursor.GetComponent<Cursor>().unitSelected != null) {
      unitSelected = myMapCursor.GetComponent<Cursor>().unitSelected;
      unitSelected.myMapManager = this;

      GetComponent<UnitStatusController>().Setup( unitSelected.GetComponent<UnitInfo>(), BackToMap );
    }
    else {
      //menu.createCommands( new int[] { (int)Menu.CmdEnum.PHASE_END, (int)Menu.CmdEnum.STAGEINFO, (int)Menu.CmdEnum.TEAMINFO, (int)Menu.CmdEnum.MAPSAVE }, Menu.CMD_WIDTH_LONG, BackToMap );
      return;
    }
    myMapCursor.GetComponent<Cursor>().SetDisable();
    this.enabled = false;
  }
  */

  private void showInfo() {
    unitSelected = myMapCursor.GetComponent<Cursor>().unitSelected;
    unitSelected.myMapManager = this;

    //GetComponent<AbilityController2>().Setup( unitSelected.GetComponent<UnitInfo>().MapFightingUnit, BackToMap );
    ability.Setup( unitSelected.GetComponent<UnitInfo>().MapFightingUnit, BackToMap );

    myMapCursor.GetComponent<Cursor>().SetDisable();
    this.enabled = false;
  }

  private void toMoveRangeMode() {
    unitSelected.GetComponent<UnitMovement>().ShowMoveRange();
    myMapCursor.GetComponent<Cursor>().SetDisable();
    myStatus = (int)MapManStatus.SHOW_MOVE_RANGE;
  }

  private void exitMoveRangeMode() {
    unitSelected.GetComponent<UnitMovement>().CancelMoveRange();
    BackToMap();
  }

  public void BackToMap() {
    BackToMap( false );
  }

  public void BackToMap( bool moveCenter = false ) {
    enabled = false;
    unitSelected = myMapCursor.GetComponent<Cursor>().unitSelected;
    if (unitSelected != null) {
      //myMapCursor.transform.position = new Vector3( unitSelected.transform.position.x, myMapCursor.GetComponent<Cursor>().defaultY, unitSelected.transform.position.z );
      myMapCursor.GetComponent<Cursor>().SetPosition( new Vector3( unitSelected.transform.position.x, myMapCursor.GetComponent<Cursor>().defaultY, unitSelected.transform.position.z ) );
    }

    //unitSelected = null;
    myUnitMenu.SetActive( false );
    TotalBox.SetActive( false );

    //myMapCursor.SetActive( false );  //不知道為什麼要刷新一下, 否則碰撞器會有問題
    myMapCursor.SetActive( true );
    myMapCursor.GetComponent<Cursor>().enabled = true;
    myMapCursor.GetComponent<Cursor>().MODE_ENUM = Cursor.ModeEnum.MAP;
    //Debug.Log( "myMapCursor.GetComponent<Cursor>().resetBoxCollider()" );
    myMapCursor.GetComponent<Cursor>().resetBoxCollider();

    myStatus = (int)MapManStatus.MOVE_CUR;

    if (moveCenter) {
      MoveCamToCenterObject( myMapCursor );
    }

    CoroutineCommon.CallWaitForOneFrame( () => { enabled = true; } );
    
  }

  void processMenuCmd() {
    Menu.CmdEnum cmdEnum = menu.GetSelectedAction();
    myUnitMenu.SetActive( false );
    TotalBox.SetActive( false );
    myStatus = (int)MapManStatus.DOING_OTHER;

    switch (cmdEnum) {
      case Menu.CmdEnum.MOVE:
        EffectSoundController.PLAY_MENU_CONFIRM();
        unitSelected.PreMoveMode();
        ////unitSelected.enabled = true;
        break;
      case Menu.CmdEnum.ATK:
        EffectSoundController.PLAY_MENU_CONFIRM();
        toWeaponMenu();
        //StartCoroutine( loadBattleAsync() );
        break;
      case Menu.CmdEnum.ON_SHIP:
        doOnShip();        
        break;
      case Menu.CmdEnum.OFF_SHIP:
        doOffShip();
        break;
      case Menu.CmdEnum.SPCMD:
        EffectSoundController.PLAY_MENU_CONFIRM();
        GetComponent<SpComMenuController>().Setup( unitSelected.GetComponent<UnitInfo>(), BackToMenu, BackToMap );

        break;
      case Menu.CmdEnum.INFO:
        EffectSoundController.PLAY_MENU_CONFIRM();
        //GetComponent<AbilityController2>().Setup( unitSelected.GetComponent<UnitInfo>().MapFightingUnit, BackToMenu );
        ability.Setup( unitSelected.GetComponent<UnitInfo>().MapFightingUnit, BackToMenu );
        this.enabled = false;
        //Debug.Log( $"Go to Info ...   this.enabled is {this.enabled}" );
        //abCtrl.enabled = true;
        /*
        CoroutineCommon.CallWaitForOneFrame( () => {
          abCtrl.Setup( unitSelected.GetComponent<UnitInfo>(), BackToMenu );
        } );*/
        break;
      case Menu.CmdEnum.PHASE_END:
        EffectSoundController.PLAY_MENU_CONFIRM();

        GameObject.Find( "MyCanvas" ).transform.Find( "ConfirmDialog" ).GetComponent<ConfirmDialog>().Setup(
          "結束本回合, 進入敵方回合",
          () => {
            GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeOut(
              sec: .5f,
              callback: () => {
                //var stageManager = GameObject.Find( "StageManager" ).GetComponent<StageManager>();
                stageManager.ToEnemyPhase();
              },
              null,
              .3f
            );
          },
          BackToMenu
        );

        enabled = false;  
        break;
      case Menu.CmdEnum.TEAMLIST:
        EffectSoundController.PLAY_MENU_CONFIRM();
        UnitListCanvasGo.GetComponent<UnitListCtl>().Setup(
          stageManager.StageUnits_Player.Select( u => u.GetComponent<UnitInfo>() ).ToList(),
          unitInfo => {
            myMapCursor.GetComponent<Cursor>().SetPosition( unitInfo.transform.position );
            CoroutineCommon.CallWaitForFrames( 2, () => { unitSelected = myMapCursor.GetComponent<Cursor>().unitSelected; } );
            BackToMap( true );
          },
          BackToMenu
        );
        UnitListCanvasGo.SetActive( true );
        break;
      case Menu.CmdEnum.ENEMYLIST:
        EffectSoundController.PLAY_MENU_CONFIRM();
        UnitListCanvasGo.GetComponent<UnitListCtl>().Setup(
          stageManager.StageUnits_Enemy.Select( u => u.GetComponent<UnitInfo>() ).ToList(),
          unitInfo => {
            myMapCursor.GetComponent<Cursor>().SetPosition( unitInfo.transform.position );
            CoroutineCommon.CallWaitForFrames( 2, () => { unitSelected = myMapCursor.GetComponent<Cursor>().unitSelected; } );
            BackToMap( true );
          },
          BackToMenu
        );
        UnitListCanvasGo.SetActive( true );
        break;
      case Menu.CmdEnum.BACK_TO_MAIN:
        EffectSoundController.PLAY_MENU_CONFIRM();

        myCanvasGo.transform.Find( "ConfirmDialog" ).GetComponent<ConfirmDialog>().Setup(
          "回到主選單",
          () => {
            myCanvasGo.GetComponent<FadeInOut>().FadeOut( 1f,
              () => {
                var mySceneManager = GameObject.Find( "MySceneManager" ).GetComponent<MySceneManager>();
                mySceneManager.UnloadAndLoadScene( "Title", SceneManager.GetActiveScene() );
              }, blackTime: 0, hold: true
            );
          },
          BackToMenu
        );

        enabled = false;
        break;
      case Menu.CmdEnum.MAPSAVE:
        EffectSoundController.PLAY_MENU_CONFIRM();
        myCanvasGo.transform.Find( "ConfirmDialog" ).GetComponent<ConfirmDialog>().Setup(
          "保存當前進度, 原有的地圖紀錄將會被覆蓋",
          //() => DIContainer.Instance.GameDataService.SaveContinue(),
          () => {
            var save = stageManager.CreateSave();  //預先把地圖上的 Unit 放進 Save

            GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeOut(
              sec: .5f,
              callback: BackToMap,
              doProcess: () => { DIContainer.Instance.GameDataService.SaveContinue( save ); },
              blackTime: 0f
            );
          },
          BackToMenu
        );
        break;
      case Menu.CmdEnum.MAPLOAD:
        myCanvasGo.transform.Find( "ConfirmDialog" ).GetComponent<ConfirmDialog>().Setup(
          "讀取上次地圖進度, 現在的地圖狀態將會被覆蓋",
          () => GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeOut(
            sec: .5f,
            BackToMap,
            doProcess: () => { DIContainer.Instance.GameDataService.LoadContinue(); },
            blackTime: 0f
          ),
          BackToMenu
        );
        break;
      case Menu.CmdEnum.OBJECTIVE:
        EffectSoundController.PLAY_MENU_CONFIRM();
        ObjectiveBox.GetComponent<ObjectiveCtl>().Setup( BackToMenu );
        break;
      default:
        EffectSoundController.PLAY_ACTION_FAIL();
        myUnitMenu.SetActive( true );
        showTotalCanvas();
        myStatus = (int)MapManStatus.Menu1;
        break;
    }

    //unitSelected.ProcessCmd( cmdIndex );
  }

  private void showTotalCanvas() {
    if (!unitSelected)
      TotalBox.SetActive( true );
  }

  private void doShowMenu() {
    myUnitMenu.SetActive( true );
    showTotalCanvas();
    myMapCursor.GetComponent<Cursor>().SetDisable();
    myStatus = (int)MapManStatus.Menu1;
  }

  public void BackToMenu() {
    myUnitMenu.SetActive( true );
    showTotalCanvas();
    myStatus = (int)MapManStatus.Menu1;
    this.enabled = true;
  }

  void toWeaponMenu() {
    var unitWC = GetComponent<UnitWeaponController>();
    unitWC.Setup( unitSelected.GetComponent<UnitInfo>(), true, BackToMenu );

    this.enabled = false;
  }

  void doOnShip() {
    if (!unitSelected) {
      EffectSoundController.PLAY_ACTION_FAIL();
      return;
    }

    EffectSoundController.PLAY_MENU_CONFIRM();
    enabled = false;
    myCanvasGo.transform.Find( "ConfirmDialog" ).GetComponent<ConfirmDialog>().Setup(
      "進入戰艦，將會消耗一次行動力",
      () => {
        int index = unitSelected.GetComponent<UnitInfo>().ShipUnit.StoringUnits.FindIndex( u => u.RobotInfo.HP < unitSelected.GetComponent<UnitInfo>().RobotInfo.HP );
        if (index < 0) index = unitSelected.GetComponent<UnitInfo>().ShipUnit.StoringUnits.Count;

        var ship = unitSelected.GetComponent<UnitInfo>().ShipUnit;
        ship.StoringUnits.Insert( index, unitSelected.GetComponent<UnitInfo>() );

        //unitSelected.GetComponent<UnitInfo>().ShipUnit.StoringUnits.Add( unitSelected.GetComponent<UnitInfo>() );
        //unitSelected.GetComponent<UnitInfo>().ShipUnit.StoringUnits.Sort( ( a, b ) => b.RobotInfo.HP.CompareTo( a.RobotInfo.HP ) );
        unitSelected.gameObject.SetActive( false );
        unitSelected.GetComponent<UnitInfo>().IsOnShip = true;
        unitSelected.EndAction();
        myMapCursor.GetComponent<Cursor>().SetDisable( true );
        myMapCursor.GetComponent<Cursor>().unitSelected = ship.GetComponent<UnitController>();
        EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/MapUnitLeave" ), 5 );
        BackToMap();
      },
      () => {
        BackToMenu();
      } //Cancel
    );

    //EffectSoundController.PLAY_MENU_CONFIRM();
  }

  void doOffShip() {
    if (!unitSelected) {
      EffectSoundController.PLAY_ACTION_FAIL();
      return;
    }

    EffectSoundController.PLAY_MENU_CONFIRM();
    enabled = false;

    LaunchListCanvasGo.GetComponent<LaunchListController>().Setup(
      unitSelected.GetComponent<UnitInfo>(),
      unitSelected.GetComponent<UnitInfo>().StoringUnits, 
      (UnitInfo unitInfo, Action callback) => {
        unitInfo.gameObject.SetActive( true );
        unitInfo.IsOnShip = false;
        unitInfo.transform.position = unitSelected.transform.position;
        unitInfo.GetComponent<UnitController>().PreMoveMode( callback );
        /*
        unitInfo.GetComponent<UnitMoveRandom>().enabled = true;
        CoroutineCommon.CallWaitForOneFrame( () => {
          unitInfo.GetComponent<UnitMoveRandom>().enabled = false;
          unitInfo.GetComponent<UnitController>().PreMoveMode( callback );
        } );
        */
        /*
        unitInfo.GetComponent<UnitMovement>().MoveOneStep( () => {
          unitInfo.GetComponent<UnitController>().PreMoveMode( callback );
        } );
        */
      },
      () => {
        BackToMenu();
      },
      this
    );
    LaunchListCanvasGo.SetActive( true );
  }

  public void MoveCamToCenterObject( GameObject go ) {
    Camera.main.GetComponent<MainCamera>().MoveCamToCenterObject( go.transform );
  }

  private void findUnitAndPlaceCursorOnIt( List<GameObject> list, UnitInfo.TeamEnum team, int nextOrPrev ) {
    GameObject go = getNextUnit( list, team, nextOrPrev );
    //myMapCursor.SetActive( true );
    myMapCursor.GetComponent<Cursor>().SetPosition( go.transform.position );

    CoroutineCommon.CallWaitForFrames( 2, () => { unitSelected = myMapCursor.GetComponent<Cursor>().unitSelected; } );
  }

  int unitIndex = -1;
  private GameObject getNextUnit( List<GameObject> list, UnitInfo.TeamEnum team, int nextOrPrev ) {
    if (unitSelected && unitSelected.GetComponent<UnitInfo>().Team == team) {
      int index = list.IndexOf( unitSelected.gameObject );
      if (index == -1) {
        unitIndex = nextOrPrev > 0 ? 0 : (list.Count - 1);
        return list[unitIndex];
      }
      unitIndex = index + nextOrPrev;
    }
    else unitIndex = unitIndex + nextOrPrev;

    if (unitIndex >= list.Count) unitIndex = 0;
    else if (unitIndex < 0) unitIndex = list.Count - 1;
    return list[unitIndex];
  }

  void OnEnable() {
    //FinishCmd();
    mySRWInput.Enable();
  }

  void OnDisable() {
    //FinishCmd();
    mySRWInput.Disable();
  }

  /*
  private Plane plane = new Plane( Vector3.up, Vector3.zero );
  private Vector3 v3Center = new Vector3( 0.5f, 0.5f, 0.0f );
  public void MoveCamToCenterObject( GameObject go ) {
    //Ray ray = Camera.main.ViewportPointToRay( v3Center );

    Vector3 v3Hit = GetPositionByRay( v3Center );
    Vector3 goPos = new Vector3( go.transform.position.x, 0, go.transform.position.z );
    Vector3 v3Delta = goPos - v3Hit;
    var camPos = Camera.main.transform.position;
    Camera.main.transform.position = new Vector3( camPos.x + v3Delta.x, camPos.y, camPos.z + v3Delta.z );
  }

  public Vector3 GetPositionByRay( Vector3 viewPort ) {
    Ray ray = Camera.main.ViewportPointToRay( viewPort );
    float fDist;
    plane.Raycast( ray, out fDist );
    Vector3 v3Hit = ray.GetPoint( fDist );
    return v3Hit;
  }
  */
}
