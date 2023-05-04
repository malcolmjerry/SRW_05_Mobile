using DataModel.Service;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Story_000 : StoryBase {

  private void Awake() {

  }

  // Use this for initialization
  void Start() {

    var go = Instantiate( Resources.Load( "Story/EarthCanvas" ) );
    go.name = "EarthCanvas";
    BGMController.SET_BGM( "2G_Sad", true, 10 );

    CoroutineCommon.CallWaitForOneFrame( () => {
      smc = GameObject.Find( "EarthCanvas" ).GetComponent<StoryMapCanvas>();
      tc  = GameObject.Find( "TalkCanvas" ).GetComponent<TalkCanvas>();

      smc.SetZoomPos( 2932, 778 );  //Pivot (0.5, 0),  Min (0, 0.5), Max (1, 0.5)
      tc.Setup( TalkNext );

      initBlockList();
      blockNo = 0;
      index = 0;

      doBlock();
      //Debug.Log( "Do Block Start." );
    } );
  }
  
	
	// Update is called once per frame
	void Update () {
		
	}

  protected override void doBlock() {
    if (blockNo >= blockList.Count) {
      SceneManager.UnloadSceneAsync( "Story" );
      GameObject.Find( "MyCanvas" ).GetComponent<ChapterShow>().show( gameDataService.GameData.Chapter, gameDataService.GameData.Stage );
      return;
    }
    talkList = blockList[blockNo];
    index = 0;

    switch (blockNo) {
      case 0:
        TalkNext();  //Do nothing, just talk;
        break;
      case 1:
        tc.TurnOff();
        CoroutineCommon.CallWaitForSeconds( 1f, () => smc.MoveTo( 2416.7f, 525.6f, 3f ) );
        CoroutineCommon.CallWaitForSeconds( 4f, TalkNext );
        break;
      default:
        Debug.Log( $"No this block({blockNo})!" );
        break;
    }
  }

  private void initBlockList() {
    Pilot 沙蒂 = pilotService.LoadPilotBase( 704 );

    blockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        /*
        new TalkParam(){ Pilot = 沙蒂, Say1 = "DC突然傳訊到我們住的加列尼亞" },
        new TalkParam(){ Pilot = 沙蒂, Say1 = "DC說若不交出那些人的話．．．" },
        new TalkParam(){ Pilot = 沙蒂, Say1 = "戰鬥中可不會有那種時間．．．" },
        new TalkParam(){ Pilot = 沙蒂, Say1 = "我只有恐懼、只想逃跑．．", Say2 = "但是，伍索卻不同" },
        new TalkParam(){ Pilot = 沙蒂, Say1 = "伍索奪取敵人的機動戰士，", Say2 = "和敵人戰鬥！" },
        new TalkParam(){ Pilot = 沙蒂, Say1 = "而且 利加.密特亞 的人民看到了", Say2 = "伍索作為駕駛員的能力" },
        new TalkParam(){ Pilot = 沙蒂, Say1 = "於是成為了V高達這部機動戰士的駕駛員", Say2 = "" },
        new TalkParam(){ Pilot = 沙蒂, Say1 = "就這樣，不管伍索是否願意，", Say2 = "在戰鬥中他被捲入了這場戰爭．．" },
        new TalkParam(){ Pilot = 沙蒂, Say1 = "但是, 這只是更恐怖的事情 ", Say2 = "的前兆而已．．．" },
        */
    } };
  }

}
