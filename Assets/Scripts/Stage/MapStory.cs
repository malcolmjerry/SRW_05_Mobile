using DataModel.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapStory : MonoBehaviour {

  //StoryMapCanvas smc;
  TalkCanvas tc;

  private PilotService pilotService;
  //private GameDataService gameDataService;

  //private Scene scene;
  private GameObject myMapCursor;
  public StageManager stageManager;

  private void Awake() {
    pilotService = DIContainer.Instance.PilotService;
    //gameDataService = DIContainer.Instance.GameDataService;
  }

  // Use this for initialization
  void Start() {
    //scene = SceneManager.GetActiveScene();
    myMapCursor = GetComponent<MapManager>().myMapCursor;
    //BGMController.SET_BGM( (AudioClip)Resources.Load( "BGM/2G_Sad" ), true, 10 );
  }

  // Update is called once per frame
  void Update() {

  }

  private int index = 0;
  private Action callback;
  private List<TalkParam> talkList;

  public void DoTalkList( List<TalkParam> talkList, Action callback ) {
    this.talkList = talkList;
    this.callback = callback;

    GetComponent<MapManager>().enabled = false;
    //myMapCursor.GetComponent<Cursor>().enabled = false;
    myMapCursor.GetComponent<Cursor>().SetDisable();
    myMapCursor.GetComponent<Cursor>().MODE_ENUM = Cursor.ModeEnum.Talk;
    myMapCursor.SetActive( true );
    myMapCursor.GetComponent<Cursor>().resetBoxCollider();

    index = 0;

    if (tc == null) {
      tc  = GameObject.Find( "TalkCanvas" ).GetComponent<TalkCanvas>();
      tc.Setup( TalkNext );
    }

    TalkNext();
  }

  Vector3? pos = null;
  public void TalkNext() {
    if (index >= talkList.Count) {
      tc.TurnOff();
      callback();
      return;
    }

    TalkParam tp = talkList[index++];
    tp.UpDown = false;

    Vector3? newPos = null;
    if (tp.Position.HasValue) {
      newPos = tp.Position.Value;
      //moveCursorAndLookAt( tp.Position.Value );
    }
    else if (tp.PilotID != null) {
      newPos = stageManager.GetPosByPilot( tp.PilotID.Value );
    }
    else if (tp.HeroSeqNo != null) {
      newPos = stageManager.GetPosByHero( tp.HeroSeqNo.Value );
    }
    if (newPos == null)
      newPos = tp.DefaultPosition;

    if (newPos != null && newPos != pos) {
      pos = newPos;
      moveCursorAndLookAt( pos.Value );
    }

    if (!string.IsNullOrWhiteSpace( tp.BGM ))
      BGMController.SET_BGM( $"{tp.BGM}" );

    if (tp.Waiting > 0) {
      tc.TurnOff();
      CoroutineCommon.CallWaitForSeconds( tp.Waiting, () => {
        //tc.Say( pilotService.LoadPilotBase( tp.PilotID ), tp.FaceNo, tp.Say1, tp.Say2, tp.Say3, tp.UpDown );
        tc.Say( tp.PicId, tp.FaceNo, tp.ShortName, tp.Say1, tp.Say2, tp.Say3, tp.UpDown );
      } );
    }
    else {
      tc.Say( tp.PicId, tp.FaceNo, tp.ShortName, tp.Say1, tp.Say2, tp.Say3, tp.UpDown );
    }
  }

  public void moveCursorAndLookAt( Vector3 pos ) {
    myMapCursor.SetActive( true );
    myMapCursor.GetComponent<Cursor>().SetPosition( new Vector3( pos.x, myMapCursor.GetComponent<Cursor>().defaultY, pos.z ) );
    myMapCursor.GetComponent<Cursor>().SetMainCamToSelf();
  }

}
