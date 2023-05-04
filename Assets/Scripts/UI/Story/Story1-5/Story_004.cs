using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Story_004 : StoryBase {

  int moveTime = 0;
  Pilot 孫文, 蔡英文, 馬克思, 彭斯, 侵侵, 肥蓬;
  Hero hero1, hero2;

  private void Awake() {

  }

  // Use this for initialization
  void Start() {

    var go = Instantiate( Resources.Load( "Story/EarthCanvas" ) );
    go.name = "EarthCanvas";
    BGMController.SET_BGM( "F_作戦立てる", true, 10 );

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
        tc.enabled = false;
        BGMController.SET_BGM( "AL_災い来たりて" );
        CoroutineCommon.CallWaitForSeconds( 1f, () => tc.enabled = true );
        break;
      default:
        Debug.Log( $"No this block({blockNo})!" );
        break;
    }
  }

  private void initBlockList() {
    孫文 = pilotService.LoadPilotBase( 1307 );
    蔡英文 = pilotService.LoadPilotBase( 405 );
    馬克思 = pilotService.LoadPilotBase( 204 );
    彭斯 = pilotService.LoadPilotBase( 112 );
    侵侵 = pilotService.LoadPilotBase( 107 );
    肥蓬 = pilotService.LoadPilotBase( 113 );
    hero1 = pilotService.LoadHero( 1 );
    hero2 = pilotService.LoadHero( 2 );

    blockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        new TalkParam( 蔡英文 ) { Say1 = "侵侵, 想不到你和拜登竟然也來了,", Say2 = "聽說這個年代的美國總統", Say3 = "和他的幕僚都突然消失了..." },
        new TalkParam( 侵侵 ) { Say1 = "詳細情況我也不清楚,", Say2 = "剛才也遇到拜登了,", Say3 = "剛好擊退他的時候就收到你的支援要請." },
        new TalkParam( 馬克思 ) { Say1 = "看來大家都是被一種力量引領來到這個年代啊,", Say2 = "當務之急是要把滿清的力量消滅,", Say3 = "否則中國就要倒退140年了." },
        new TalkParam( 彭斯 ) { Say1 = "原來馬克思先生也被傳送過來了啊,", Say2 = "我有熟讀<<馬克思主義>>! ", Say3 = "" },
        new TalkParam( 肥蓬 ) { Say1 = "馬克思先生被轉移的時候應該還沒有完成<<馬克思主義>>,", Say2 = "但這幾天你應該都從歷史資料中看過了吧. ", Say3 = "" },
        new TalkParam( 馬克思 ) { Say1 = "是的, 我看過了,", Say2 = "大部分想法跟我現在所想的都吻合,", Say3 = "但還是有一部分我不敢想像竟是未來的自己說的..." },
        new TalkParam( 侵侵 ) { Say1 = "在我們的認知裡,", Say2 = "你的主張跟後世的共產主義國家實踐很不一樣. ", Say3 = "" },
        new TalkParam( 馬克思 ) { Say1 = "這個不奇怪,", Say2 = "我早就料到會有人扭曲了馬克思主義.", Say3 = "" },
        new TalkParam( 孫文 ) { Say1 = "好了, 今天我們要讓歷史再重演一次,", Say2 = "推翻封建帝制,", Say3 = "以三民主義統一中國!" },
        new TalkParam( 侵侵 ) { Say1 = "沒想到能見到孫先生本人,", Say2 = "真是榮幸!", Say3 = "" },
        new TalkParam( 孫文 ) { Say1 = "美國是我們的盟友,", Say2 = "沒有你們投放的原子彈,", Say3 = "我們早晚會被日軍佔領全國." },
        new TalkParam( 肥蓬 ) { Say1 = "可惜你們後來的民主化不太順利.", Say2 = "", Say3 = "" },
        new TalkParam( 孫文 ) { Say1 = "我應該早些注意到自己的健康,", Say2 = "如果不是那麼早就仙遊,", Say3 = "說不定這個國家的道路不會那麼崎嶇." },
        new TalkParam( 馬克思 ) { Say1 = "後來也有人繼承了你的民主革命,", Say2 = "把三民主義發展成社會主義.", Say3 = "" },
        new TalkParam( 孫文 ) { Say1 = "如果我們日後被傳送回去原本的年代,", Say2 = "那對時間線的影響會很大,", Say3 = "因為我們的理念都會超前了很多," },
        new TalkParam( 孫文 ) { Say1 = "很可能會改變中國這100多年的歷史.", Say2 = "", Say3 = "" },
        new TalkParam( 馬克思 ) { Say1 = "說不定到時候我們可以合作創造一個更美好的世界.", Say2 = "", Say3 = "" },
        new TalkParam( 孫文 ) { Say1 = "說不定共產主義可以提前100年真正地實現了.", Say2 = "", Say3 = "" },
      },
      new List<TalkParam> {
        new TalkParam( hero1 ) { Say1 = "大清帝國軍攻過來了, ", Say2 = "我們要準備迎戰!" },
        new TalkParam( 侵侵 ) { Say1 = "你就是他們提到的未來人..?", Say2 = "" },
        new TalkParam( hero2 ) { Say1 = "是我們.", Say2 = "", Say3 = "" },
        new TalkParam( 馬克思 ) { Say1 = "看來這次的規模比上次還要大得多!", Say2 = "", Say3 = "" },
        new TalkParam( 蔡英文 ) { Say1 = "日軍的戰鬥機也在,", Say2 = "難道大清和日本聯手了?", Say3 = "" },
        new TalkParam( 肥蓬 ) { Say1 = "放心, 這次我們也助陣,", Say2 = "應該可以把他們打回去!" },
        new TalkParam( 彭斯 ) { Say1 = "全員第一種戰鬥配置,", Say2 = "出擊吧!" },
      },
    };
  }

}
