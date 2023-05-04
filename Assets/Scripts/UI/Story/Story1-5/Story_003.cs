using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Story_003 : StoryBase {

  int moveTime = 0;
  Pilot 孫中山, 蔡英文, 國民革命軍, 馬克思, 拜登;
  Hero hero1, hero2;

  private void Awake() {

  }

  // Use this for initialization
  void Start() {

    var go = Instantiate( Resources.Load( "Story/EarthCanvas" ) );
    go.name = "EarthCanvas";
    BGMController.SET_BGM( "2G_WorldMapTalk", true, 10 );

    CoroutineCommon.CallWaitForOneFrame( () => {
      smc = GameObject.Find( "EarthCanvas" ).GetComponent<StoryMapCanvas>();
      tc = GameObject.Find( "TalkCanvas" ).GetComponent<TalkCanvas>();

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
        smc.SetZoomPos( 2540, 510 );  //台灣海岸
        CoroutineCommon.CallWaitForSeconds( 0f, () => tc.ShowPlace( "台灣海岸" ) );
        break;
      case 1:
        MyCanvas.FadeOut( 1f,
          () => {
            smc.SetZoomPos( -1620, 810 );  //華盛頓 D.C.
            BGMController.SET_BGM( "AL_災い来たりて" );
            tc.ShowPlace( "華盛頓 D.C." );
          },
          blackTime: 2f
        );
        break;
      default:
        Debug.Log( $"No this block({blockNo})!" );
        break;
    }
  }

  private void initBlockList() {
    孫中山 = pilotService.LoadPilotBase( 1307 );
    //國民革命軍 = pilotService.LoadPilotBase( 9933 );
    蔡英文 = pilotService.LoadPilotBase( 405 );
    拜登 = pilotService.LoadPilotBase( 108 );
    馬克思 = pilotService.LoadPilotBase( 204 );
    hero1 = pilotService.LoadHero( 1 );
    hero2 = pilotService.LoadHero( 2 );

    blockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        new TalkParam( 孫中山 ) { Say1 = "兩位, 該說說你們的來歷了吧?", Say2 = "", Say3 = "" },
        new TalkParam( hero1 ) { Say1 = "我們是從2093年的時空過來的, ", Say2 = "隸屬地球連邦軍第14獨立外部部隊." },
        new TalkParam( 馬克思 ) { Say1 = "你們是未來的人類,", Say2 = "那在五維時空中,", Say3 = "那把聲音是你們的嗎?" },
        new TalkParam( 馬克思 ) { Say1 = "是你們把這些新型機動兵器帶給我們的嗎?", Say2 = "難道慈禧也是...", Say3 = "" },
        new TalkParam( hero1 ) { Say1 = "不是, 我們沒有把兵器給慈禧,", Say2 = "但是我們的量子電腦發現了時間軸出現了異常,", Say3 = "所以派人來查看" },
        new TalkParam( hero1 ) { Say1 = "就發現了有第三者把慈禧大軍轉移了,", Say2 = "是有不知名的第三方啟動了時空兵器,", Say3 = "還把未來的機兵技術給了她們. " },
        new TalkParam( hero1 ) { Say1 = "暫時還不清楚這股神秘勢力的真正身份,", Say2 = "得知這個消息後,", Say3 = "我們就立刻被派遣前來支援" },
        new TalkParam( hero1 ) { Say1 = "不能讓這條世界線被帝制支配.", Say2 = "", Say3 = "" },
        new TalkParam( 蔡英文 ) { Say1 = "2093年... 就是50年之後,", Say2 = "人類已經製造出時間機器了嗎?", Say3 = "" },
        new TalkParam( hero2 ) { Say1 = "不是人類發明的~ ", Say2 = "在2070年的時候開始,", Say3 = "世界各處陸續出現了一些五維的時空球," },
        new TalkParam( hero2 ) { Say1 = "直至2093年, 地球上就有5個,", Say2 = "衛星軌道有一個, 月球上也有一個,", Say3 = "後來都被連邦政府重點保護起來了." },
        new TalkParam( hero2 ) { Say1 = "我們是通過進入五維時空球,", Say2 = "找到適當的時間點就從裡面出來的.", Say3 = "" },
        new TalkParam( 孫中山 ) { Say1 = "這麼說的話, ", Say2 = "跟我在1907年遇到的那個時空扭曲現象", Say3 = "是同樣的一個東西嗎..." },
        new TalkParam( hero2 ) { Say1 = "這個不好說, 但很大機會是相同的,", Say2 = "不知道是誰把這些五維空間投射到我們的三維世界.", Say3 = "" },
        new TalkParam( hero1 ) { Say1 = "人類有了時空穿越的能力,", Say2 = "就開始監控各條世界線上的歷史了.", Say3 = "" },
        new TalkParam( 馬克思 ) { Say1 = "這幾天我看了一些科學文獻, 就是那個多元世界理論吧,", Say2 = "有無數的並行世界存在,", Say3 = "每個世界都有些不一樣..." },
        new TalkParam( hero1 ) { Say1 = "是啊, 但在我們的年代,", Say2 = "有了量子電腦, 所以分析起來也不太需要人手,", Say3 = "電腦的運算速度很快," },
        new TalkParam( hero1 ) { Say1 = "幾秒鐘就能模擬一次宇宙大爆炸所產生的新宇宙了.", Say2 = "", Say3 = "" },
        new TalkParam( 蔡英文 ) { Say1 = "就是那種只要輸入所有的參數,", Say2 = "就能模擬一個宇宙的誕生, ", Say3 = "之後每分每秒所發生的事, 都能被計算出來," },
        new TalkParam( 蔡英文 ) { Say1 = "也就是一個絕對的宿命論因果鏈,", Say2 = "一個沒有自由意志的世界, ", Say3 = "未來都是可被預測的..." },
        new TalkParam( hero1 ) { Say1 = "我們只是戰鬥人員, 太複雜的理論我也不懂,", Say2 = "我們接到的命令是來支援你們對抗滿清帝國,", Say3 = "只要把他們擊敗我們就會回去了," },
        new TalkParam( hero1 ) { Say1 = "當然也會把你們送回屬於你們的時代", Say2 = "", Say3 = "" },
        new TalkParam( 孫中山 ) { Say1 = "這次事件, 感覺到背後有一股力量,", Say2 = "把我們都送來這一年,", Say3 = "到底是為了什麼呢, 那些自稱 \"觀察者\" 的人..." },
        new TalkParam( hero2 ) { Say1 = "觀察者...? 沒聽說過啊. ", Say2 = "但很可能就是他們搞出來的,", Say3 = "把你們和慈禧都轉移," },
        new TalkParam( hero2 ) { Say1 = "提供未來的技術, ", Say2 = "促進這次第三次世界大戰.", Say3 = "" },
      },
      new List<TalkParam> {
        new TalkParam( 拜登 ) { Say1 = "轉移到這個年代, ", Say2 = "難道也是光明會的意思...?" },
        new TalkParam( 9906, "美軍兵士" ) { Say1 = "總統先生! ", Say2 = "雷達探測到3個敵機蹤影!" },
        new TalkParam( 拜登 ) { Say1 = "全軍追擊! ", Say2 = "不能讓他們逃了!", Say3 = "" },
        new TalkParam( 9906, "美軍兵士" ) { Say1 = "Yes sir ! ", Say2 = "" },
        new TalkParam( 拜登 ) { Say1 = "(為什麼把他們也轉移了? ... ", Say2 = "光明會的目的到底是什麼...)" },
      },
    };
  }

}
