using DataModel.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DataModel.Service.GameDataService;

public abstract class StoryBase : MonoBehaviour {

  protected StoryMapCanvas smc;
  protected TalkCanvas tc;

  protected PilotService pilotService;
  protected GameDataService gameDataService;

  void Awake() {

  }

  void OnEnable() {
    pilotService = DIContainer.Instance.PilotService;
    gameDataService = DIContainer.Instance.GameDataService;
  }

  // Update is called once per frame
  void Update() {

  }

  protected List<List<TalkParam>> blockList;
  protected int blockNo;
  protected int index;
  protected List<TalkParam> talkList;

  protected abstract void doBlock();

  protected void LoadChapter() {
    MyCanvas.FadeOut( 1f,
      () => {
        var mySceneManager = GameObject.Find( "MySceneManager" ).GetComponent<MySceneManager>();
        mySceneManager.UnloadAndLoadChapter( SceneManager.GetActiveScene() );
        //StartCoroutine( LoadChapterAsync() );
      },
      blackTime: 0, hold: true
    );
  }

  protected void LoadInterMission() {
    gameDataService.StoryPhase = StoryPhaseEnum.Start;
    MyCanvas.FadeOut( 1f,
      () => {
        var mySceneManager = GameObject.Find( "MySceneManager" ).GetComponent<MySceneManager>();
        mySceneManager.UnloadAndLoadScene( "InterMission", SceneManager.GetActiveScene() );
      },
      blackTime: 0, hold: true
    );
  }

  public void TalkNext() {
    if (index >= talkList.Count) {
      blockNo++;
      doBlock();
      return;
    }
    TalkParam tp = talkList[index++];

    if (!string.IsNullOrWhiteSpace( tp.BGM ))
      BGMController.SET_BGM( $"{tp.BGM}" );

    tc.Say( tp.PicId, tp.FaceNo, tp.ShortName, tp.Say1, tp.Say2, tp.Say3, tp.UpDown );
    /*
    if (tp.IsPlace)
      tc.ShowPlace( "Demo Place" );
    else
      tc.Say( tp.PicId, tp.FaceNo, tp.ShortName, tp.Say1, tp.Say2, tp.Say3, tp.UpDown );
    */
  }

}
