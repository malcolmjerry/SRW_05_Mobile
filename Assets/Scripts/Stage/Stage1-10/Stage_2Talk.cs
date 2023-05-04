using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DataModel.Service;
using UnityEngine.SceneManagement;
using System;

public partial class Stage_2 : StageBase {

  Pilot 孫中山, 蔡英文, 馬克思, 慈禧;
  Hero hero1, hero2;

  Vector3 talkCenter = new Vector3( -5, 5, -34 );

  //public override List<int> TalkEventList { get; set; }

  public override void SetupTalk() {
    孫中山 = pilotService.LoadPilotBase( 1307 );
    蔡英文 = pilotService.LoadPilotBase( 405 );
    馬克思 = pilotService.LoadPilotBase( 204 );
    慈禧 = pilotService.LoadPilotBase( 307 );
    hero1 = pilotService.LoadHero( 1 );
    hero2 = pilotService.LoadHero( 2 );
    setupTalkBeforeBattle();
  }

  //private int block = 0;
  //private List<List<TalkParam>> talkBlockList;
  //private List<TalkParam> talkBlock;
  private void initTalkBlock_startMap() {
    talkBlock = new List<TalkParam> {
        new TalkParam( 慈禧 ) { Say1 = "孫中山!?,", Say2 = "竟在這個年代也遇到你, 而且你也有機動兵器啊 !?", Say3 = "看來那些監察者也有幫助你" },
        new TalkParam( 孫中山 ) { Say1 = "我是前兩天才被傳送到這個年代, ", Say2 = "還沒完全弄清楚這130多年的歷史呢,", Say3 = "也不明白那些 \"五維生物\"" },
        new TalkParam( 孫中山 ) { Say1 = "為什麼要把我們傳送到這麼多年後,", Say2 = "但這場宿命中的決戰還是要延續啊... ", Say3 = "" },
        new TalkParam( 孫中山 ) { Say1 = "\"驅除韃虜，恢復中華\" 在這年代已不再適用了,", Say2 = "但看來你們滿清亡靈還是必須要除掉... ", Say3 = "" },
        new TalkParam( 慈禧 ) { Say1 = "我比你早來幾個月,", Say2 = "已聽說了這一百多來的歷史,", Say3 = "你的國民黨早就失敗了," },
        new TalkParam( 慈禧 ) { Say1 = "現在只要我把你們國民軍的餘孽全部消滅, ", Say2 = "就可以恢復我大清的統一!", Say3 = "" },
        new TalkParam( 孫中山 ) { Say1 = "國民黨當年的確戰敗了, ", Say2 = "但還是在台灣這處地方活了下來啊," },
        new TalkParam( 孫中山 ) { Say1 = "雖然我們現在軍力相差巨大,", Say2 = "但因為你的出現,", Say3 = "國共兩黨又再次合作了呢!" },
        new TalkParam( 馬克思 ) { Say1 = "太后, 你那時還沒認識我吧, ", Say2 = "我是一個月前被吸進了一個像黑洞的東西 ", Say3 = "才轉移到這個時代," },
        new TalkParam( 馬克思 ) { Say1 = "前兩天就遇到孫先生, ", Say2 = "這時的孫先生比我在資料中認識的那位要年輕一點.. ", Say3 = "" },
        new TalkParam( 馬克思 ) { Say1 = "對了, ", Say2 = "我是在1844年被吸進那個 五維空間...", Say3 = "" },
        new TalkParam( 馬克思 ) { Say1 = "既然太后你已來了3個月, ", Say2 = "你應該從歷史中聽說過我吧,", Say3 = "共產主義你也認識了吧" },
        new TalkParam( 慈禧 ) { Say1 = "共產主義最後也證明不可行了,", Say2 = "人們都是披着馬克思的皮", Say3 = "在骨子裡實行國家資本主義而已." },
        new TalkParam( 馬克思 ) { Say1 = "這個國家的接班人稱這個是中國特色社會主義呢,", Say2 = "我覺得這條路沒有走錯啊.", Say3 = "" },
        new TalkParam( 孫中山 ) { Say1 = "看來比我的 \"三民主義\" 還要走得好呢,", Say2 = "本來三民主義就是共產主義,", Say3 = "後來台灣上的中華民國繼承者也沒有真正實行到啊." },
        new TalkParam( 孫中山 ) { Say1 = "不管怎樣, 中國不可能恢復帝制,", Say2 = "慈禧, 你死心吧!", Say3 = "" },
        new TalkParam( 慈禧 ) { Say1 = "我也考慮過君主立憲,", Say2 = "但這要在收復台灣,", Say3 = "統一全國之後才能進行." },
        new TalkParam( 慈禧 ) { Say1 = "你們投降吧,", Say2 = "我們軍力相差5倍,", Say3 = "你們沒有勝機的!" },
        new TalkParam( 孫中山 ) { Say1 = "革命尚未成功,", Say2 = "同志仍需努力! ", Say3 = "" },
        new TalkParam( 馬克思 ) { Say1 = "沒錯,", Say2 = "不能讓滿清亡靈再次把這個國家推向深淵!", Say3 = "" },
        new TalkParam( 蔡英文 ) { Say1 = "不管共產黨怎樣, 但皇帝專制獨裁是最糟糕的,", Say2 = "不能讓大清帝國死灰復燃,", Say3 = "所以現在必須把妳打倒! " },
        new TalkParam( 蔡英文 ) { Say1 = "我也必須守護台灣這片土地啊,", Say2 = "雖然對我來說是25年後的...", Say3 = "" },
        new TalkParam( 蔡英文 ) { Say1 = "聽說這時候的我已經不在了呢,", Say2 = "所以沒有見到另一個自己,", Say3 = "否則可能會發生時空崩壞現象." },
        new TalkParam( 馬克思 ) { Say1 = "對了, 還沒跟蔡總統你詳談過,", Say2 = "那時候兩岸關係還好嗎, 不會是敵對關係吧!", Say3 = "為什麼經歷了數十年中國還沒統一起來啊?" },
        new TalkParam( 蔡英文 ) { Say1 = "還好啊, 那時候我們稱為一中各表,", Say2 = "名義上還是一個中國,", Say3 = "數十年來都有經濟文化上的交流," },
        new TalkParam( 蔡英文 ) { Say1 = "還沒有發生戰爭呢,", Say2 = "只是在我被轉移過來的那一年,", Say3 = "關係還是有點緊張." },
        new TalkParam( 孫中山 ) { Say1 = "好了, 敵人要攻過來了,", Say2 = "我們先聯手一起把滿清軍打回去! ", Say3 = "" },    
    };
  }

  /// <summary> 敵機數量少於一半 </summary>
  private void initTalkBlock_event1() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 慈禧 ) { Say1 = "看到我們戰力差距了吧, 呵呵!!", Say2 = "" },
      new TalkParam( 孫中山 ) { Say1 = "敵人數量太多, 再這樣下去...", Say2 = "切! ", Say3 = "" },
      new TalkParam( 馬克思 ) { Say1 = "(現在是海岸守衛戰, 無法用游擊戰術...)", Say2 = "", Say3 = "" },
      new TalkParam( 蔡英文 ) { Say1 = "但現在也不能放棄! 必須堅持下去!", Say2 = "", Say3 = "" },
    };
  }

  /// <summary> 敵增援1 後次回合 </summary>
  private void initTalkBlock_event2() {
    talkBlock = new List<TalkParam> {
      new TalkParam( hero2 ) { Say1 = "孫先生, 馬克思先生, 蔡總統, 我是奉命來支援你們的.", Say2 = "只要把敵人的指揮官打倒, ", Say3 = "敵人就會暫時撤退了吧." },
      new TalkParam( 孫中山 ) { Say1 = "你是誰啊?", Say2 = " ", Say3 = "" },
      new TalkParam( hero1 ) { Say1 = "他是我的同伴, 他是來協助我們的.", Say2 = " ", Say3 = "" },
      new TalkParam( 馬克思 ) { Say1 = "那好, 我們組成共同戰線吧!", Say2 = "", Say3 = "" },
      new TalkParam( 蔡英文 ) { Say1 = "看來你們也是有着相同理念的人呢,", Say2 = "我的直覺告訴我 能相信你們...", Say3 = "" },
      new TalkParam( hero2 ) { Say1 = "目標就是敵人那個新型機器人, ", Say2 = "看來慈禧就在上面...", Say3 = "" },
    };
  }

  private void initTalkBlock_bossDestroyed() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 慈禧 ) { Say1 = "你們真頑強,", Say2 = "但我大清今次只是調動了軍隊很少的一部分而已," },
      new TalkParam( 慈禧 ) { Say1 = "下次大軍壓境之時,", Say2 = "就是你們滅亡之日,", Say3 = "這一天很快就會到 !" },
    };
  }

  protected override List<ObjectiveData> objDataList { get; set; } = new List<ObjectiveData>() {
    new ObjectiveData() {
      WinStrings = new string[] { "敵全滅" },
      LoseStrings = new string[] { "味方任何體機被撃墜" },
      HintStrings = new string[] { "敵人只有一次增援!", "" },
    },
    new ObjectiveData() {
      WinStrings = new string[] { "敵全滅" },
      LoseStrings = new string[] { "味方任何體機被撃墜" },
      HintStrings = new string[] {}  //長度為0時, 清空該塊
    }
  };


  // 角色對戰時對話--------------------------------------------------------------------------------------------------------

  public void initTalkBlockBattle_01() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 慈禧 ) { Say1 = "在這裡我就不會讓你的革命成功!", Say2 = "" },
      new TalkParam( 孫中山 ) { Say1 = "放棄權力吧, 我保證會善待你們皇室的人!", Say2 = "" },
      new TalkParam( 慈禧 ) { Say1 = "大清不能毀在我手, ", Say2 = "君主立憲的事可以商量." },
      new TalkParam( 孫中山 ) { Say1 = "現在君主立憲已經太遲了...", Say2 = "" },
    };
  }

  public void initTalkBlockBattle_02() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 慈禧 ) { Say1 = "共產主義對現在的人來說太遙遠了,", Say2 = "人類至少要再經歷300年的精神提升才有可能最終實現" },
      new TalkParam( 馬克思 ) { Say1 = "這點我無法否認.", Say2 = "" },
      new TalkParam( 馬克思 ) { Say1 = "但目前來說,", Say2 = "中國不可能再復辟帝制,", Say3 = "這是不能接受的! " },
      new TalkParam( 馬克思 ) { Say1 = "你還是好好養老吧,", Say2 = "不要再干政了!", Say3 = " " },
      new TalkParam( 慈禧 ) { Say1 = "以我們大清目前的軍事實力,", Say2 = "再也不怕洋人了, 你就等着看,", Say3 = "我會讓中國再強大起來!" },
    };
  }

  public void initTalkBlockBattle_03() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 慈禧 ) { Say1 = "中華民國在距今98年前已經亡國了,", Say2 = "你怎麼還不放棄?" },
      new TalkParam( 蔡英文 ) { Say1 = "只要還有一絲希望,", Say2 = "我都不能放棄,", Say3 = "必須守護這片中華民族最後的自由之地..." },
      new TalkParam( 蔡英文 ) { Say1 = "滿清的亡靈,", Say2 = "還是回歸你的黃泉之國吧!", Say3 = "" },
      new TalkParam( 慈禧 ) { Say1 = "呵呵, 我還沒死啊,", Say2 = "現在就要把台灣也收復了,", Say3 = "那就讓我來給你最後的絕望吧! " }
    };
  }

  public void initTalkBlockBattle_04() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 慈禧 ) { Say1 = "你是什麼人? ", Say2 = "" },
      new TalkParam( hero1 ) { Say1 = "我是未來人.", Say2 = "對不起, 雖和你沒有怨恨,", Say3 = "但我的任務是支援孫中山." },
      new TalkParam( 慈禧 ) { Say1 = "(...那幫觀察者到底有什麼目的!?)", Say2 = "", Say3 = "" }
    };
  }

  private void setupTalkBeforeBattle() {
    TalkBeforeBattleList.Add( new TalkBeforeBattle() { PilotIds = new List<int>() { 慈禧.ID, 孫中山.ID }, GetTalkBlock = initTalkBlockBattle_01 } );
    TalkBeforeBattleList.Add( new TalkBeforeBattle() { PilotIds = new List<int>() { 慈禧.ID, 馬克思.ID }, GetTalkBlock = initTalkBlockBattle_02 } );
    TalkBeforeBattleList.Add( new TalkBeforeBattle() { PilotIds = new List<int>() { 慈禧.ID, 蔡英文.ID }, GetTalkBlock = initTalkBlockBattle_03 } );
    TalkBeforeBattleList.Add( new TalkBeforeBattle() { PilotIds = new List<int>() { 慈禧.ID }, HeroSeq = hero1.SeqNo, GetTalkBlock = initTalkBlockBattle_04 } );
    //intTalkEventList( TalkBeforeBattleList.Count );
  }

}
