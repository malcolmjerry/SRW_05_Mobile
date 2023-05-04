using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class MySceneManager : MonoBehaviour {

  //public Object MapManagerPrefab;

  public int defaultDo = 1;

	// Use this for initialization
	void Start () {
    Application.runInBackground = true;
    var gameDataService = DIContainer.Instance.GameDataService;

    if (defaultDo == 1) {                      //正常進入Title
      StartCoroutine( loadSceneAsync( "Title" ) );
    }

    else if (defaultDo == 2) {                //測試時直接進入Stage場景
      //測試時 生成主角
      HeroData preData = new HeroData();
      Hero mainHero = preData.GetRandomHero( 1 );
      mainHero.SeqNo = 1;
      Hero subHero = preData.GetRandomHero( 2 );
      subHero.SeqNo = 2;
      DIContainer.Instance.PilotService.CreatePilotInstance( mainHero.GetPilotIdByCharacter(), isPlayer: true, hero: mainHero );
      DIContainer.Instance.PilotService.CreatePilotInstance( subHero.GetSubPilotIdByCharacter(), isPlayer: true, hero: subHero );
      //
      StartCoroutine( loadStageAsync() );
    }

    else if (defaultDo == 3)                //測試時直接進入Story場景
      StartCoroutine( loadSceneAsync( "Story" ) );

    else if (defaultDo == 4) {   //自動通過一些關卡
      HeroData preData = new HeroData();
      Hero mainHero = preData.GetRandomHero( 1 );
      mainHero.SeqNo = 1;
      Hero subHero = preData.GetRandomHero( 2 );
      subHero.SeqNo = 2;
      DIContainer.Instance.PilotService.CreatePilotInstance( mainHero.GetPilotIdByCharacter(), isPlayer: true, hero: mainHero );
      DIContainer.Instance.PilotService.CreatePilotInstance( subHero.GetSubPilotIdByCharacter(), isPlayer: true, hero: subHero );

      var autoPlay = DIContainer.Instance.AutoPlayService;
      autoPlay.Play();
      SceneManager.LoadScene( $"InterMission", LoadSceneMode.Additive );
      /*
      gameDataService.SaveBeforeStage();
      gameDataService.GenerateNewStage();
      GameObject.Find( "MyCanvas" ).GetComponent<ChapterShow>().show( gameDataService.GameData.Chapter, gameDataService.GameData.Stage );
      */
    }
  }

  // Update is called once per frame
  void Update () {
	  
	}

  IEnumerator loadStageAsync() {
    DIContainer.Instance.GameDataService.GenerateNewStage();
    string stageName = "stage";  //DIContainer.Instance.GameDataService.GetStageName();
    AsyncOperation async = SceneManager.LoadSceneAsync( stageName, LoadSceneMode.Additive );

    while (!async.isDone) {
      yield return new WaitForEndOfFrame();
    }
    Scene stageScene = SceneManager.GetSceneByName( stageName );  //stage_001
    SceneManager.SetActiveScene( stageScene );

    StageManager stageManager = GameObject.Find( "StageManager" ).GetComponent<StageManager>();
    stageManager.enabled = true;
    stageManager.InitStageBase();

    //var mapManagerGO = Instantiate( MapManagerPrefab, Vector3.zero, Quaternion.identity ) as GameObject;
    //mapManagerGO.name = "MapManager";
  }

  SaveContinue saveCon;

  public void UnloadAndLoadStage( string unloadScene ) {
    StartCoroutine( UnloadAndLoadStageAsync( unloadScene ) );
  }

  IEnumerator UnloadAndLoadStageAsync( string unloadScene ) {
    //GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FullOut();
    yield return loadStageContinueAsync();
    //Debug.Log( "UnloadAndLoadStageAsync after loadStageContinueAsync()" );
    AsyncOperation async = SceneManager.UnloadSceneAsync( unloadScene );
    
    while (!async.isDone) {
      Debug.Log( "UnloadSceneAsync yield return" );
      yield return new WaitForEndOfFrame();
    }
    Debug.Log( "UnloadSceneAsync isDone" );
      
    CoroutineCommon.CallWaitForSeconds( .0f, () => {
      GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FullIn();
    } );   
    
  }

  IEnumerator loadStageContinueAsync() {
    saveCon = DIContainer.Instance.GameDataService.LoadContinue();
    //string stageName = DIContainer.Instance.GameDataService.GetStageName();
    AsyncOperation async = SceneManager.LoadSceneAsync( /*stageName*/ "stage", LoadSceneMode.Additive );

    while (!async.isDone) {
      Debug.Log( "loadStageContinueAsync yield return" );
      yield return new WaitForEndOfFrame();
    }
    Scene stageScene = SceneManager.GetSceneByName( /*stageName*/ "stage" );  //stage_001
    SceneManager.SetActiveScene( stageScene );

    var stageManager = GameObject.Find( "StageManager" ).GetComponent<StageManager>();
    stageManager.InitStageBase( saveCon );
    stageManager.enabled = true;
  }

  bool isHoldBlack;
  public void UnloadAndLoadScene( string loadTheScene, Scene unloadTheScene, bool holdBlack = false ) {
    isHoldBlack = holdBlack;
    StartCoroutine( UnloadAndLoadSceneAsync( loadTheScene, unloadTheScene ) );
  }

  IEnumerator UnloadAndLoadSceneAsync( string loadTheScene, Scene unloadTheScene ) {
    //Debug.Log( $"[MySceneManager] -> UnloadAndLoadSceneAsync ready: load {loadTheScene}, unload {unloadTheScene}" );

    yield return loadSceneAsync( loadTheScene );
    //Debug.Log( $"[MySceneManager] -> UnloadAndLoadSceneAsync loadSceneAsync finished." );

    AsyncOperation async = SceneManager.UnloadSceneAsync( unloadTheScene );

    while (!async.isDone) {
      //Debug.Log( "[MySceneManager] -> UnloadAndLoadSceneAsync UnloadSceneAsync yield return" );
      yield return new WaitForEndOfFrame();
    }

    if (!isHoldBlack)
      GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FullIn();

  }

  IEnumerator loadSceneAsync( string sceneName ) {
    AsyncOperation async = SceneManager.LoadSceneAsync( sceneName, LoadSceneMode.Additive );

    while (!async.isDone) {
      //Debug.Log( "[MySceneManager] -> loadSceneAsync yield return" );
      yield return new WaitForEndOfFrame();
    }
    Scene stageScene = SceneManager.GetSceneByName( sceneName ); 
    SceneManager.SetActiveScene( stageScene );
  }

  public void UnloadAndLoadChapter( Scene unloadTheScene ) {
    StartCoroutine( LoadChapterAsync( unloadTheScene ) );
  }

  protected IEnumerator LoadChapterAsync( Scene unloadTheScene ) {
    AsyncOperation async = SceneManager.UnloadSceneAsync( unloadTheScene );

    while (!async.isDone) {
      //Debug.Log( "[StoryBase] -> UnLoadAsync yield return" );
      yield return new WaitForEndOfFrame();
    }

    var gameDataService = DIContainer.Instance.GameDataService;
    GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FullIn();
    GameObject.Find( "MyCanvas" ).GetComponent<ChapterShow>().show( gameDataService.GameData.Chapter, gameDataService.GameData.Stage );
  }
}
