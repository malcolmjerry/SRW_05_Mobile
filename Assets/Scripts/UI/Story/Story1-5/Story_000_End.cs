using DataModel.Service;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DataModel.Service.GameDataService;

public class Story_000_End : StoryBase {

  private void Awake() {

  }

  // Use this for initialization
  void Start() {

    var go = Instantiate( Resources.Load( "Story/EarthCanvas" ) );
    go.name = "EarthCanvas";
    BGMController.SET_BGM( "2G_WorldMapTalk", true, 10 );

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
  void Update() {

  }

  protected override void doBlock() {
    if (blockNo >= blockList.Count) {
      //GameObject.Find( "MyCanvas" ).GetComponent<ChapterShow>().show( gameDataService.GameData.Chapter, gameDataService.GetChapterName(), gameDataService.GameData.Stage );
      GameObject.Find( "MyCanvas" ).GetComponent<FadeInOut>().FadeOut( 2f, () => {
        gameDataService.StoryPhase = StoryPhaseEnum.Start;
        SceneManager.UnloadSceneAsync( "Story" );
        SceneManager.LoadScene( $"InterMission", LoadSceneMode.Additive );
      } );
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
    Pilot 慈禧 = pilotService.LoadPilotBase( 307 );

    blockList = new List<List<TalkParam>> {
      //Paragraph 1
      new List<TalkParam> {
        /*
        new TalkParam( 10001, "???" ){ Say1 = $"{pilotService.LoadPilotBase( 701 ).ShortName} 君， 你是", Say2 = "那台 V高達 的駕駛員是嗎？" }, 
        new TalkParam(){ Pilot = 伍索, Say1 = "並不是因為喜歡才去當駕駛員啊，",  Say2 = "只是為了生存而已。"  }, //伍索
        new TalkParam(){ Pilot = 甲兒, Say1 = "喂 你，",  Say2 = "能不能更像個小孩呀！"  }, //甲兒
        new TalkParam(){ Pilot = 伍索, Say1 = "請不要以大人的情況",  Say2 = "來判斷小孩。"  }, //伍索
        new TalkParam(){ Pilot = 甲兒, Say1 = "哼 可惡! ",  Say2 = "你這小子!！"  }, //甲兒
        new TalkParam(){ Pilot = 沙也加, Say1 = $"好了 {pilotService.LoadPilotBase( 1301 ).ShortName} 君。 ",  Say2 = "這樣不像個大人了。"  }, //甲兒
        new TalkParam(){ Pilot = 阿寶, Say1 = $"．．．{pilotService.LoadPilotBase( 701 ).ShortName} 君， ", Say2 = "我明白你想要說的。" },    //阿寶
        new TalkParam(){ Pilot = 阿寶, Say1 = $"若果我們更努力做得更好的話， ", Say2 = "就不用使你這樣的小孩子去戰鬥了。" },    //阿寶
        new TalkParam(){ Pilot = 伍索, Say1 = "．．．",  Say2 = ""  }, //伍索
        new TalkParam(){ Pilot = 馬貝特, Say1 = "．．．那又是呢．．．",  Say2 = $"就像 {pilotService.LoadPilotBase( 201 ).ShortName} 說的那樣。"  }, //馬貝特
        new TalkParam(){ Pilot = 馬貝特, Say1 = $"{pilotService.LoadPilotBase( 701 ).ShortName} 作為駕駛員的那種資質",  Say2 = $"如果我也能有的話．．．"  }, //馬貝特
        new TalkParam(){ Pilot = 馬貝特, Say1 = $"對不起 {pilotService.LoadPilotBase( 701 ).ShortName}。",  Say2 = $"你的心情之前沒有考慮到 ．．．"  }, //馬貝特
        new TalkParam(){ Pilot = 馬貝特, Say1 = $"{pilotService.LoadPilotBase( 701 ).ShortName} 的話 在這裡離開也沒有問題。",  Say2 = $"之後就由我們來想辦法吧。"  }, //馬貝特
        new TalkParam(){ Pilot = 伍索, Say1 = "．．．讓我好好考慮",  Say2 = ""  }, //伍索
        new TalkParam(){ Pilot = 馬貝特, Say1 = $"噢! {pilotService.LoadPilotBase( 701 ).ShortName} ！？",  Say2 = $""  }, //馬貝特
        */
      },
      //Paragraph 2
      /*
      new List<TalkParam> {
        new TalkParam(){ Pilot = 武藏, Say1 = $"喂喂 {pilotService.LoadPilotBase( 1602 ).ShortName}。", Say2 = "利加．密特亞 是什麼來的？" },    //武藏
        new TalkParam(){ Pilot = 隼人, Say1 = $"你有沒有聽書的 {pilotService.LoadPilotBase( 1603 ).ShortName}．．．",
                                       Say2 = $"就在剛才 {pilotService.LoadPilotBase( 505 ).ShortName} 艦長已經說明過了吧。" },    //隼人
        new TalkParam(){ Pilot = 武藏, Say1 = $"這種事，", Say2 = "你覺得老子我會記得嗎？" },    //武藏
        new TalkParam(){ Pilot = 隼人, Say1 = $"你在自慢些什麼，",
                                      Say2 = $"想知道的話， 就去問 {pilotService.LoadPilotBase( 1601 ).LastName} 好了。" },    //隼人
        new TalkParam(){ Pilot = 武藏, Say1 = $"車， 好冷淡啊。", Say2 = $"喂 {pilotService.LoadPilotBase( 1601 ).LastName} ! 聽到了吧？",　Say3 = "告訴我啊。" },    //武藏
        new TalkParam(){ Pilot = 龍馬, Say1 = $"真沒辦法。", Say2 = $"．．．這次別再忘了，好嗎。",　Say3 = "" },    //流龍馬
        new TalkParam(){ Pilot = 龍馬, Say1 = $"利亞．密特亞 這個嘛．．．", Say2 = $"",　Say3 = "" },    //流龍馬
        new TalkParam(){ Pilot = 龍馬, Say1 = $"由民間組成，", Say2 = $"抵抗 DC 的組織。", Say3 = "" },    //流龍馬
        new TalkParam(){ Pilot = 龍馬, Say1 = $"我們也正在得到", Say2 = $"他們的援助。", Say3 = "" },    //流龍馬
        new TalkParam(){ Pilot = 武藏, Say1 = $"噢， 那樣的話，", Say2 = $"到目前為止，我們得到的補給", Say3 = "都是由 利加．密特亞 提供的喔。" },    //武藏
        new TalkParam(){ Pilot = 武藏, Say1 = $"但是， 只在民間就能製造出這樣的組織，", Say2 = $"真的很厲害。", Say3 = "" },    //武藏
        new TalkParam(){ Pilot = 龍馬, Say1 = $"話說是由 辛．加拿姆 這樣的人物，", Say2 = $"作為 Leader 指揮著的。", Say3 = "" },    //流龍馬
        new TalkParam(){ Pilot = 武藏, Say1 = $"辛．加拿姆 啊．．．", Say2 = $"", Say3 = "" },    //武藏
      
      }*/
    };
  }

}
