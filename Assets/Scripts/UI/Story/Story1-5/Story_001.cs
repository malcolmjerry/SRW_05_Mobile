using DataModel.Service;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Story_001 : StoryBase {

  Pilot 孫中山, 同盟會兵士;

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
      //MyCanvas.FadeOut( 0, () => { }, blackTime: 0, hold: true );
      //SceneManager.UnloadSceneAsync( "Story" );
      //GameObject.Find( "MyCanvas" ).GetComponent<ChapterShow>().show( gameDataService.GameData.Chapter, gameDataService.GameData.Stage );
      LoadChapter();
      return;
    }
    talkList = blockList[blockNo];
    index = 0;

    switch (blockNo) {
      case 0:
        //TalkNext();  //Do nothing, just talk;
        smc.SetZoomPos( 2932, 778 );  //日本
        CoroutineCommon.CallWaitForSeconds( 0f, () => tc.ShowPlace( "日本" ) );
        break;
      case 1:      
        tc.TurnOff();
        CoroutineCommon.CallWaitForSeconds( 1f, () => smc.MoveTo( 2464f, 828, 2f ) ); //北京 2464, 828
        CoroutineCommon.CallWaitForSeconds( 3f, () => tc.ShowPlace( "北京" ) );
        break;
      default:
        Debug.Log( $"No this block({blockNo})!" );
        break;
    }
  }

  private void initBlockList() {
    孫中山 = pilotService.LoadPilotBase( 1307 );
    同盟會兵士 = pilotService.LoadPilotBase( 9931 );

    blockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        new TalkParam( 孫中山 ) { Say1 = "驅除韃虜, 恢復中華...", Say2 = "" },
        new TalkParam( 孫中山 ) { Say1 = "革命的時候到了, ", Say2 = "今天同盟會就要攻進北京, 推翻滿清！" },
        new TalkParam( 同盟會兵士 ) { Say1 = "大家出發！", Say2 = "" }
      },
      new List<TalkParam> {
        new TalkParam( 孫中山 ) { Say1 = "看來敵人已準備好迎戰, 大家不要鬆懈！", Say2 = "" },
        new TalkParam( 同盟會兵士 ) { Say1 = "知道！ ", Say2 = "敵人的數量不多,", Say3 = "一鼓作氣消滅敵人吧!"},
        new TalkParam( 孫中山 ) { Say1 = "不能輕敵!", Say2 = "" },
        new TalkParam( 孫中山 ) { Say1 = "(光緒皇帝還在那裡嗎?...)", Say2 = "(只要推翻了滿清, 就能建立民主中國嗎...?)" },
        new TalkParam( 同盟會兵士 ) { Say1 = "三民主義萬歲！", Say2 = "" },
      },
    };
  }

}
