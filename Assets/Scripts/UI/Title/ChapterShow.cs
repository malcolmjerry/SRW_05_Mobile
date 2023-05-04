using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Experimental.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChapterShow : MonoBehaviour {

  private Transform chapterImgTf;

  private bool canSkip_Time;
  private bool canSkip_Loaded;
  private int stage;

  private void Awake() {
    chapterImgTf = transform.Find( "ChapterImg" );
  }

  void Start () {

  }
	
	// Update is called once per frame
	void Update () {
    if (canSkip_Time && canSkip_Loaded && Input.GetButtonDown( "Confirm" ))
      goMap( stage );
	}

  public void show( int chapterNum, int stage ) {
    this.stage = stage;
    enabled = true;

    chapterImgTf.gameObject.SetActive( true );
    canSkip_Time = false;
    canSkip_Loaded = false;
    //CoroutineCommon.CallWaitForSeconds( 5f, () => { canSkip_Time = true; } );
    CoroutineCommon.CallWaitForSeconds( 5f, () => { canSkip_Time = true; } );  //測試時改為 .5f秒 正式 5秒

    Text chapNumText = chapterImgTf.Find( "ChapterNum" ).GetComponent<Text>();
    Text chapTitleText = chapterImgTf.Find( "ChapterTitle" ).GetComponent<Text>();
    chapNumText.text = $"第 {chapterNum} 話";
    chapTitleText.text = StageMap.StageMapList[stage]; //chapterTitle;

    BGMController.SET_BGM( "AL_開かれた砲門", false );
    StartCoroutine( loadStageAsync( stage ) );

    CoroutineCommon.CallWaitForSeconds( 7f, () => {
      if (enabled) goMap( stage );
    } );
  }

  private void goMap( int stage ) {
    BGMController.STOP_BGM();
    enabled = false;
    //GetComponent<FadeInOut>().FadeOut( 2f, () => {   //正式
    GetComponent<FadeInOut>().FadeOut( 1f, () => {   //測試
      chapterImgTf.gameObject.SetActive( false );
      StartCoroutine( startStageAsync() );
    } );
  }

  AsyncOperation loadStageAsyncDone;

  IEnumerator loadStageAsync( int stage ) {
    string stageNum = stage.ToString().PadLeft( 3, '0' );

    //loadStageAsyncDone = SceneManager.LoadSceneAsync( $"stage_{stageNum}", LoadSceneMode.Additive );
    loadStageAsyncDone = SceneManager.LoadSceneAsync( $"stage", LoadSceneMode.Additive );

    while (!loadStageAsyncDone.isDone) {
      yield return new WaitForEndOfFrame();
    }

    canSkip_Loaded = true;

    //Scene stageScene = SceneManager.GetSceneByName( $"stage_{stageNum}" );
    Scene stageScene = SceneManager.GetSceneByName( $"stage" );
    SceneManager.SetActiveScene( stageScene );

    //GameObject.Find( "StageManager" ).GetComponent<StageManager>().enabled = true;
  }

  IEnumerator startStageAsync() {
    while (!loadStageAsyncDone.isDone) {
      yield return new WaitForEndOfFrame();
    }
    StageManager stageManager = GameObject.Find( "StageManager" ).GetComponent<StageManager>();
    stageManager.enabled = true;
    stageManager.InitStageBase();
  }

}
