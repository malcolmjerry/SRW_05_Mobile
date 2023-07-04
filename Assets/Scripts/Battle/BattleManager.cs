using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour {

  public Light light;

  GameObject battleEntryGO;
  BattleEntry entry;

  //List<CmdForAnim> cmdList;
  FightAnimController fightAnimController;
  int cmdIndex;
  string sceneName;
  bool isBacked;
  bool canBack;

  MySRWInput mySRWInput;

  private void Awake() {
    mySRWInput.UI.CancelBack.performed += ctx => {
      //StartCoroutine( loadStageMapAsync() );
      //StopAllCoroutines();
      //fightAnimController.StopAllThing();
      if (canBack) {
        isBacked = true;
        loadStageMap();
      }
    };
  }

  // Use this for initialization
  void Start () {
    //MyStart(); 
    //sceneName = $"stage_{DIContainer.Instance.GameDataService.GameData.Stage.ToString().PadLeft( 3, '0' )}";
  }
	
  public void MyStart( string stageName, string terrainName, float lightIntensity = 0.15f ) {
    sceneName = stageName;
    Application.runInBackground = true;
    //GetComponent<AudioListener>().enabled = true;

    //Debug.Log( entry.AttackModelName + " " + entry.CounterModelName + " " + entry.AttackWeaponNum + " " + entry.CounterWeaponNum );
    battleEntryGO = GameObject.Find( "BattleEntryGO" );//.GetComponent<BattleEntry>();
    //entry.TempGO.SetActive( false );
    fightAnimController = GameObject.Find( "FightArea" ).GetComponent<FightAnimController>();//.PlayAttack();

    //GameObject.Find( "TerrainMaps" ).transform.Find( terrainName ).gameObject.SetActive( true );
    var bgR = GameObject.Find( "BackgroundR" ).transform;
    var bgL = GameObject.Find( "BackgroundL" ).transform;
    var terrainPrefab = Resources.Load<GameObject>( $"Terrains/Battle/{terrainName}" );
    var terrainRGo = Instantiate( terrainPrefab, bgR );
    terrainRGo.name = "TerrainR";
    var terrainLGo = Instantiate( terrainPrefab, bgL );
    terrainLGo.name = "TerrainL";

    light.intensity = lightIntensity;

    //AudioClip clip;
    if (battleEntryGO == null) {
      //clip = (AudioClip)Resources.Load( "BGM/06" );
      //cmdList = new List<CmdForAnim> { new CmdForAnim() { Side = "R", Weapon = 2, TotalDamage = 2200 }, new CmdForAnim() { Side = "L", Weapon = 1, TotalDamage = 2000 } };
      //fightAnimController.Setup( "WZ_EW", "T1", 6200, 6200, 6000, 6000, 220, 220, 200, 200 );
    } else {
      entry = battleEntryGO.GetComponent<BattleEntry>();
      //clip = (AudioClip)Resources.Load( "BGM/" + entry.BGM_Name );
      //cmdList = entry.cmdForAnims;
      /*
      fightAnimController.Setup( entry.FullHpR, entry.HpR, entry.FullHpL, entry.HpL, entry.FullEnR, entry.EnR, entry.FullEnL, entry.EnL,
                                 entry.UnitInfoR, entry.UnitInfoL);*/
    }

    /*
    var audio = GetComponent<AudioSource>();

    audio.clip = clip;
    audio.loop = true;
    audio.Play();*/

    cmdIndex = 0;
    CoroutineCommon.CallWaitForSeconds( 3f, () => canBack = true );
    ProcessCmd();
  }

	// Update is called once per frame
	void Update () {  
    /*
    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      //StartCoroutine( loadStageMapAsync() );
      //StopAllCoroutines();
      //fightAnimController.StopAllThing();
      if (canBack) {
        isBacked = true;
        loadStageMap();
      }
    }
    */
  }

  public void ProcessCmd() {
    if (cmdIndex >= entry.AttackDataList.Count) {
      //StopAllCoroutines();
      CoroutineCommon.CallWaitForSeconds( 1.5f, () => {
        if (!isBacked)
          loadStageMap();
      } );
      return;
    }

    float waitTime = cmdIndex > 0 ? 0.5f : 0;

    AttackData att = entry.AttackDataList[cmdIndex];

    CoroutineCommon.CallWaitForSeconds( waitTime, () => {
      fightAnimController.PlayBattle( att,
        () => {
          cmdIndex++;
          ProcessCmd();
        } );
    } );

  }
  /*
  public void ProcessCmd() {
    if (cmdIndex >= cmdList.Count) {
      //StopAllCoroutines();
      CoroutineCommon.CallWaitForSeconds( 1.5f, () => {
        loadStageMap();
      } );
      return;
    }

    float waitTime = cmdIndex > 0 ? 0.5f : 0;

    CoroutineCommon.CallWaitForSeconds( waitTime, () => {
      fightAnimController.PlayBattle( cmdList[cmdIndex].Weapon, cmdList[cmdIndex].Side, cmdList[cmdIndex].TotalDamage, cmdList[cmdIndex].SpendEn, cmdList[cmdIndex].HitMiss,
        () => {
          cmdIndex++;
          ProcessCmd();
      } );
    } );

  }*/

  void loadStageMap() {
    MyCanvas.FadeOut( 0, () => { }, blackTime: 0, hold: true );
    Scene stageScene = SceneManager.GetSceneByName( sceneName );  //2021-11-25

    //SceneManager.MoveGameObjectToScene( battleEntryGO, stageScene );
    SceneManager.SetActiveScene( stageScene );     ////2021-11-25

    foreach (var go in entry.AllGameObjects) {
      go.transform.SetParent( null );

      /*
      if (go.name.Contains( "UnitRobot" )) {
        Debug.Log( "After Battle Fill color: " + go.transform.Find("Canvas").Find( "HealthSlider/Fill Area/Fill" ).GetComponent<Image>().color );
      }*/
    }
    Destroy( entry.TempGO );
    //GetComponent<AudioListener>().enabled = false;

    //var asyncLoad = SceneManager.UnloadSceneAsync( "Battle" );

    /*
    while (!asyncLoad.isDone) {
      yield return null;
    }*/

    //GameObject.Find( "StageManager" ).GetComponent<StageManager>().AfterBattleBase();
    //StartCoroutine( AfterBattleBaseAsync() );
    GameObject.Find( "StageManager" ).GetComponent<StageManager>().AfterBattle( entry.AttackDataList );
  }

  private void OnEnable() {
    mySRWInput.Enable();
  }

  private void OnDisable() {
    mySRWInput.Disable();
  }

  /*
  public IEnumerator AfterBattleBaseAsync() {
    var asyncLoad = SceneManager.UnloadSceneAsync( "Battle" );
    while (!asyncLoad.isDone) {
      yield return null;
    }
    GameObject.Find( "StageManager" ).GetComponent<StageManager>().AfterBattle( entry.AttackDataList );
  }
  */

  /*
  IEnumerator loadStageMapAsync() {
    AsyncOperation async = SceneManager.LoadSceneAsync( "stage_01", LoadSceneMode.Additive );

    while (!async.isDone) {
      yield return new WaitForEndOfFrame();
    }

    Scene stageScene = SceneManager.GetSceneByName( "stage_01" );
    SceneManager.SetActiveScene( stageScene );

    SceneManager.UnloadScene( "Battle" );

    //var mapManagerGO = Instantiate( MapManagerPrefab, Vector3.zero, Quaternion.identity ) as GameObject;
    //mapManagerGO.name = "MapManager";

  }*/
}
