using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PrologController : MonoBehaviour {

  private Transform textTf;

  private bool isChanged;

	// Use this for initialization
	void Start () {
    var gameDataService = DIContainer.Instance.GameDataService;

    textTf = transform.Find( "Text" );

    /*
    int bgm = Random.Range( 1, 3 );
    switch (bgm) {
      case 1: BGMController.SET_BGM( "2G_SadEnd" ); break;
      case 2: BGMController.SET_BGM( "F_予感" ); break;
    }
    */
    if (gameDataService.GameData.Stage == 1)
      BGMController.SET_BGM( "F_予感" );
    else
      BGMController.SET_BGM( "2G_SadEnd" );

    //DIContainer.Instance.GameDataService.PrologInitStage();

    enabled =false;
    //CoroutineCommon.CallWaitForSeconds( 1f, () => { enabled = true; } );

    GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeIn( 2f, () => {
      enabled = true;
    } );

    CoroutineCommon.CallWaitForAnimator( textTf.GetComponent<Animator>(), "Prolog", () => {
      if (!isChanged) {
        isChanged = true;
        loadStory();
      }
    } );
  }
	
	// Update is called once per frame
	void Update () {
    textTf.GetComponent<Animator>().speed = 1f;
    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" )) {
      /*
      enabled = false;
      GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeOut( 1f, () => {
        SceneManager.UnloadSceneAsync( "Prologue" );
        SceneManager.LoadScene( "Title", LoadSceneMode.Additive );
      } );
      */
    }
    else if (Input.GetButtonDown( "Start" )) {
      /*
      enabled = false;
      if (!isChanged) {
        isChanged = true;
        loadStory();
      }
      */
    }
    else if (Input.GetButton( "Confirm" )) {
      textTf.GetComponent<Animator>().speed = 5f;
    }
  }

  void loadChapter() {
    SceneManager.UnloadSceneAsync( "Prologue" );
    //CoroutineCommon.CallWaitForOneFrame( () => { GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().AfterFadeOut(); } );
    GameObject.Find( "MyCanvas" ).GetComponent<ChapterShow>().show( 1, 1 );
  }

  IEnumerator loadStoryAsync() {
    SceneManager.UnloadSceneAsync( "Prologue" );
    AsyncOperation loadStageAsyncDone = SceneManager.LoadSceneAsync( "Story", LoadSceneMode.Additive );
    while (!loadStageAsyncDone.isDone) {
      yield return new WaitForEndOfFrame();
    }

    //Scene stageScene = SceneManager.GetSceneByName( $"Story" );
    //SceneManager.SetActiveScene( stageScene );
  }

  void loadStory() {
    //SceneManager.UnloadSceneAsync( "Prologue" );
    //SceneManager.LoadScene( "Story", LoadSceneMode.Additive );
    //SceneManager.LoadScene( "HeroInput", LoadSceneMode.Additive );
    var mySceneManager = GameObject.Find( "MySceneManager" ).GetComponent<MySceneManager>();
    GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeOut( .5f, () => {   //測試 .5, 正式 2f
      mySceneManager.UnloadAndLoadScene( "Story", SceneManager.GetActiveScene() );
    }, blackTime: 0f, hold: true );
  }

  /*
  IEnumerator loadStageAsync() {
    AsyncOperation async = SceneManager.LoadSceneAsync( "stage_01", LoadSceneMode.Additive );

    while (!async.isDone) {
      yield return new WaitForEndOfFrame();
    }

    Scene stageScene = SceneManager.GetSceneByName( "stage_01" );
    SceneManager.SetActiveScene( stageScene );

    GameObject.Find( "StageManager" ).GetComponent<StageManager>().enabled = true;

    SceneManager.UnloadSceneAsync( "Prologue" );
    CoroutineCommon.CallWaitForOneFrame( () => { GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().AfterFadeOut(); } );
    //var mapManagerGO = Instantiate( MapManagerPrefab, Vector3.zero, Quaternion.identity ) as GameObject;
    //mapManagerGO.name = "MapManager";
  }
  */

}
