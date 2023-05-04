using DataModel.Service;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Story_002 : StoryBase {

  int moveTime = 0;
  Pilot 孫中山, 蔡英文, 國民革命軍, 馬克思;
  Hero hero1;

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

      //smc.SetZoomPos( 2540, 510 );  //台灣海岸

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
      //SceneManager.UnloadSceneAsync( "Story" );
      //GameObject.Find( "MyCanvas" ).GetComponent<ChapterShow>().show( gameDataService.GameData.Chapter, gameDataService.GameData.Stage );
      LoadChapter();
      return;
    }
    talkList = blockList[blockNo];
    index = 0;

    switch (blockNo) {
      case 0:
        smc.SetZoomPos( 2540, 510 );  //台灣海岸
        CoroutineCommon.CallWaitForSeconds( 0f, () => tc.ShowPlace( "台灣海岸" ) );
        break;
      case 1:
        BGMController.SET_BGM( "AL_災い来たりて" );
        CoroutineCommon.CallWaitForSeconds( 1.5f, () => {
          TalkNext();
        } );    
        break;
      default:
        Debug.Log( $"No this block({blockNo})!" );
        break;
    }
  }

  private void initBlockList() {
    孫中山 = pilotService.LoadPilotBase( 1307 );
    國民革命軍 = pilotService.LoadPilotBase( 9933 );
    蔡英文 = pilotService.LoadPilotBase( 405 );
    馬克思 = pilotService.LoadPilotBase( 204 );
    hero1 = pilotService.LoadHero( 1 );

    blockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        new TalkParam( 孫中山 ) { Say1 = "我被傳送到這裡已經兩天了,", Say2 = "現在是2047年的3月,", Say3 = "世界已過了139年," },
        new TalkParam( 孫中山 ) { Say1 = "看了兩天的 \"近代史\",", Say2 = "這139年中國的變化真大..." },
        new TalkParam( 蔡英文 ) { Say1 = "我跟孫先生一樣才轉移過來兩天,", Say2 = "眼前發生的景象就像造夢,", Say3 = "難以置信..." },
        new TalkParam( 孫中山 ) { Say1 = "對岸的大清帝國比我們早轉移3個月,", Say2 = "已生產了大量的人型機動兵器,", Say3 = "要守住這海峽不容易啊." },
        new TalkParam( 蔡英文 ) { Say1 = "既然命運安排我們也轉移到這個年代,", Say2 = "只要我們要保持心裡的信念不滅,", Say3 = "我相信會有奇跡的!" },
        new TalkParam( 孫中山 ) { Say1 = "也是啊,", Say2 = "將目前手上的戰鬥力盡可能發揮,", Say3 = "總能做點什麼." },
        new TalkParam( 孫中山 ) { Say1 = "我也派人聯系了美國,", Say2 = "但那邊可能也發生了事,", Say3 = "等了兩天都沒回覆." },
        new TalkParam( 蔡英文 ) { Say1 = "那我們只能靠自己了...", Say2 = "", Say3 = "" },
        new TalkParam( 孫中山 ) { Say1 = "啊.. 好像有什麼來了.", Say2 = "", Say3 = "" },
      },
      new List<TalkParam> {
        new TalkParam( 國民革命軍 ) { Say1 = "孫先生,大清帝國軍要從海上攻過來了！", Say2 = "" },
        new TalkParam( 孫中山 ) { Say1 = "終於來到這一日了! 準備迎戰 ! ", Say2 = "" },
        new TalkParam( 蔡英文 ) { Say1 = "這是我第一次駕駛人型機動兵器參加實戰,", Say2 = "想不到第一仗就要面對如此大軍..!!", Say3 = "" },
        new TalkParam( 10002, "黑影" ) { Say1 = "......", Say2 = "", Say3 = "" },
        new TalkParam( hero1 ) { Say1 = "咦, 你是... ? ", Say2 = "" },
        new TalkParam( 馬克思 ) { Say1 = "上吧！", Say2 = "" },
      },
    };
  }

}
