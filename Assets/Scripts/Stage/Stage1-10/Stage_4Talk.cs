using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DataModel.Service;
using UnityEngine.SceneManagement;
using System;

public partial class Stage_4 : StageBase {

  Pilot 侵侵, 彭斯, 肥蓬, 孫中山, 蔡英文, 馬克思, 慈禧, 山本五十六, 宮部久藏;
  Hero hero1, hero2;

  Vector3 talkCenter = new Vector3( -5, 5, -34 );

  //public override List<int> TalkEventList { get; set; }

  protected override List<TalkBeforeBattle> TalkBeforeBattleList { get; set; } = new List<TalkBeforeBattle>();

  public override void SetupTalk() {
    侵侵 = pilotService.LoadPilotBase( 107 );
    彭斯 = pilotService.LoadPilotBase( 112 );
    肥蓬 = pilotService.LoadPilotBase( 113 );
    孫中山 = pilotService.LoadPilotBase( 1307 );
    蔡英文 = pilotService.LoadPilotBase( 405 );
    馬克思 = pilotService.LoadPilotBase( 204 );
    慈禧 = pilotService.LoadPilotBase( 307 );
    hero1 = pilotService.LoadHero( 1 );
    hero2 = pilotService.LoadHero( 2 );
    山本五十六 = pilotService.LoadPilotBase( 604 );
    宮部久藏 = pilotService.LoadPilotBase( 606 );
    setupTalkBeforeBattle();
  }

  private void initTalkBlock_startMap() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 慈禧 ) { Say1 = "山本大將, ", Say2 = "由中日主導組建大東亞共榮圈的想法很不錯,", Say3 = "大清將會實行君主立憲制," },
      new TalkParam( 慈禧 ) { Say1 = "君主成為國家虛位元首. ", Say2 = "不過我們首要障礙就是這些革命分子和美國人.", Say3 = "" },
      new TalkParam( 山本五十六 ) { Say1 = "大日本帝國海軍將會助清國一臂之力. ", Say2 = "", Say3 = "" },
      new TalkParam( 慈禧 ) { Say1 = "孫文, 梁啟超的想法沒有錯, ", Say2 = "你們一意孤行要中國在民智未開的狀況下", Say3 = "立即變成共和制," },
      new TalkParam( 慈禧 ) { Say1 = "結果換來數十年軍閥割據的時代,", Say2 = "抗日之戰傷亡慘重,", Say3 = "還發生了內戰, 你們的做法是錯了." },
      new TalkParam( 慈禧 ) { Say1 = "如果中國先實行君主立憲,", Say2 = "由清帝作為虛位元首領幫助國家團結,", Say3 = "這不是更好? " },
      new TalkParam( 孫中山 ) { Say1 = "雖然英國在這方面成功了,", Say2 = "但法國失敗了,", Say3 = "誰也不知道經歷了數千年帝制的中國, 會變成怎樣. " },
      new TalkParam( 孫中山 ) { Say1 = "怎保證你們滿人會真正下放權力呢. ", Say2 = "", Say3 = "" },
      new TalkParam( 山本五十六 ) { Say1 = "中山樵啊, ", Say2 = "你應該明白這一點,", Say3 = "日本的君主立憲不是很成功嗎? " },
      new TalkParam( 孫中山 ) { Say1 = "你們明治維新很成功,", Say2 = "但後來卻變成軍國主義,", Say3 = "我們不能步後塵." },
      new TalkParam( 馬克思 ) { Say1 = "尤其是法西斯主義和軍國主義,", Say2 = "更是人類的最大敵人!", Say3 = "" },
      new TalkParam( 山本五十六 ) { Say1 = "我看過歷史檔案,", Say2 = "你們在21世紀不但沒有走向共產,", Say3 = "反倒更接近法西斯國家了." },
      new TalkParam( 馬克思 ) { Say1 = "有我在, 這種事就不會發生!", Say2 = "中國不會走法西斯道路.", Say3 = "" },
      new TalkParam( 侵侵 ) { Say1 = "二戰已經結束很久了,", Say2 = "全世界都走向民主共和,", Say3 = "你們真的沒有和談的可能嗎? " },
      new TalkParam( 侵侵 ) { Say1 = "演變成第3次大戰也不是好事.", Say2 = "", Say3 = "" },
      new TalkParam( 慈禧 ) { Say1 = "既然眼前出現了這次機會,", Say2 = "我總不能讓大清亡國的,", Say3 = "君主立憲已是最大讓步了. " },
      new TalkParam( 山本五十六 ) { Say1 = "我們大日本天皇也是君權神授,", Say2 = "君是所有人民的精神領袖,", Say3 = "皇室無可取替, 但治權掌握在國會." },
      new TalkParam( 山本五十六 ) { Say1 = "清國已經同意加入大東亞共榮圈了,", Say2 = "這個構想也是從中山樵先生你的大亞洲主義發展出來,", Say3 = "所以請你們也就不要再反抗了." },
      new TalkParam( 孫中山 ) { Say1 = "在東亞組成一個聯邦是可以,", Say2 = "但必須在推翻中國的帝制之後,", Say3 = "否則我們的辛亥革命就沒意義了." },
      new TalkParam( 馬克思 ) { Say1 = "你們先輩打嬴過甲午戰爭和日俄戰爭,", Say2 = "實力不能小看,", Say3 = "但我們不會退縮的!" },
      new TalkParam( 慈禧 ) { Say1 = "可惜了, 你們都是人材,", Say2 = "如不能招降, 只能除掉.", Say3 = "" },
      new TalkParam( 蔡英文 ) { Say1 = "孫先生, 一起上!", Say2 = "", Say3 = "" },
      new TalkParam( 孫中山 ) { Say1 = "好, 這是最後決戰了.", Say2 = "", Say3 = "" },
      new TalkParam( 彭斯 ) { Say1 = "(...這個年代看不到有美軍駐守台灣)", Say2 = "", Say3 = "" },
      new TalkParam( 肥蓬 ) { Say1 = "那我們也助孫先生一臂之力!", Say2 = "", Say3 = "" },
      new TalkParam( hero1 ) { Say1 = "（...任務是支援孫先生,", Say2 = "又要捲入一場紛爭了!）", Say3 = "" },
      new TalkParam( hero2 ) { Say1 = "（紅色巨龍對未來世界是個威脅...)", Say2 = "", Say3 = "" },
    };
  }

  // 敵增援1
  private void initTalkBlock_EnemyReinforce1() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 山本五十六 ) { Say1 = "神風特攻隊也來了.", Say2 = "", Say3 = "" },
      new TalkParam( 宮部久藏 ) { Say1 = "我們必須在這裡把美軍擊沉!", Say2 = "", Say3 = "" },
      new TalkParam( 侵侵 ) { Say1 = "那是二戰時候的飛機嗎!?", Say2 = "", Say3 = "" },
      new TalkParam( 宮部久藏 ) { Say1 = "經過這個時代科技的改造,", Say2 = "不要小看零戰!", Say3 = "" },
      new TalkParam( 彭斯 ) { Say1 = "零戰!? ", Say2 = "二戰的中期, 美軍一直吃虧,", Say3 = "直至後期才發現了它的弱點, 形勢就逆轉了." },
      new TalkParam( 侵侵 ) { Say1 = "零戰有什麼弱點!?", Say2 = "", Say3 = "" },
      new TalkParam( 肥蓬 ) { Say1 = "火力雖強大, 但裝甲薄弱, ", Say2 = "而時速達到371公里時, 它的向右轉向就不行了,", Say3 = "我們可以輕易擺脫它的攻擊." },
      new TalkParam( 宮部久藏 ) { Say1 = "現在的零戰經過改造,", Say2 = "所有弱點都已剋服了,", Say3 = "讓你再嘗一次苦頭!" },
      new TalkParam( 馬克思 ) { Say1 = "大家要小心了,", Say2 = "看來他說的是真的.", Say3 = "" },
      new TalkParam( 蔡英文 ) { Say1 = "他們還會使用神風特攻的招數嗎...?", Say2 = "", Say3 = "" },
      new TalkParam( hero1 ) { Say1 = "根據電腦分析,", Say2 = "零戰的裝甲加強了, 機動力並沒有減少,", Say3 = "相信還會使用神風特攻那招..." },
      new TalkParam( hero2 ) { Say1 = "就是自殺式撞擊嗎?", Say2 = "", Say3 = "" },
      new TalkParam( hero1 ) { Say1 = "看來這次他們撞擊後並不會自爆,", Say2 = "要小心不要被撞到!", Say3 = "" },
      new TalkParam( 彭斯 ) { Say1 = "看來要命中他們也並不容易,", Say2 = "真的要集中精神才行.", Say3 = "" },
    };
  }

  // 慈禧 被擊敗
  private void initTalkBlock_BossDefeated() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 慈禧 ) { Say1 = "老祖宗, 我已盡力了,", Say2 = "大清這片江山保不住了,", Say3 = "希望這個國家會迎來更好的未來." },
      new TalkParam( 孫中山 ) { Say1 = "慈禧, 脫出吧. ", Say2 = "我會善待皇室的.", Say3 = "" },
      new TalkParam( 慈禧 ) { Say1 = "不, 我的時辰已到,", Say2 = "孫文, 你要小心日本人,", Say3 = "不要讓他們利用溥儀." },
      new TalkParam( 慈禧 ) { Say1 = "偽滿州國, ", Say2 = "這種傀儡政權,", Say3 = "不能出現..." },
      new TalkParam( 孫中山 ) { Say1 = "那段歷史我也清楚,  ", Say2 = "放心吧,", Say3 = "我知道要做些什麼." },
      new TalkParam( 慈禧 ) { Say1 = "還有中華蘇維埃,", Say2 = "這種紅色邪惡,", Say3 = "你也要阻止啊..." },
      new TalkParam( 馬克思 ) { Say1 = "現在是2047年了, 我會重新審視要走什麼道路,", Say2 = "蘇聯已經解體了, 那一套不行,", Say3 = "中國會走自己特色的發展道路." },
      new TalkParam( 慈禧 ) { Say1 = "很好......", Say2 = "", Say3 = "" }
    };
  }

  // 慈禧 爆炸後
  private void initTalkBlock_AfterBossDefeated() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 侵侵 ) { Say1 = "中國終於結束數千年帝制了,", Say2 = "之後你們兩黨的事, 還是好好談判吧,", Say3 = "不要再內戰了." },
      new TalkParam( 孫中山 ) { Say1 = "正有此意.", Say2 = "", Say3 = "" },
      new TalkParam( 蔡英文 ) { Say1 = "一切聽國父的.", Say2 = "", Say3 = "" },
      new TalkParam( 馬克思 ) { Say1 = "好吧,", Say2 = "我們先過去大陸,", Say3 = "看看那邊的情況." },
      new TalkParam( 彭斯 ){ Say1 = "剛才收到新聞信息,", Say2 = "4.2光年外的一個三星系統,", Say3 = "向太陽系派出了艦隊!"  },
      new TalkParam( 馬克思 ){ Say1 = "南門二那邊嗎...", Say2 = "", Say3 = ""  },
    };
  }

  // 山本五十六 被擊敗後
  private void initTalkBlock_Boss2Defeated() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 山本五十六 ) { Say1 = "一時撤退!" },
      new TalkParam( 山本五十六 ) { Say1 = "大日本帝國不能在這裡止步," },
      new TalkParam( 山本五十六 ) { Say1 = "還必須繼續前進..." },
    };
  }

  // 角色對戰時對話--------------------------------------------------------------------------------------------------------

  public void initTalkBlockBattle_01() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 孫中山 ) { Say1 = "竟然跟日本人聯手,", Say2 = "你的靈魂賣給惡魔了嗎!" },
      new TalkParam( 慈禧 ) { Say1 = "組成大東亞聯邦, ", Say2 = "大清帝國可以永遠繁榮." },
      new TalkParam( 孫中山 ) { Say1 = "... 這次一定要將你徹底擊倒!", Say2 = "" },
    };
  }

  public void initTalkBlockBattle_02() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 慈禧 ) { Say1 = "蘇聯人扶植建立的紅色中國,", Say2 = "破壞了中國的傳統文化!" },
      new TalkParam( 馬克思 ) { Say1 = "國民黨一黨專政, 民不聊生,", Say2 = "當時不得不這麼做.", Say3 = "況且中國的後輩也正在復興中華傳統文化了." },
      new TalkParam( 慈禧 ) { Say1 = "你還不知道馬克思主義害死了多少人.", Say2 = "", Say3 = "" },
      new TalkParam( 馬克思 ) { Say1 = "馬克思主義被太多人扭曲了.", Say2 = "", Say3 = "" },
      new TalkParam( 慈禧 ) { Say1 = "我們就用實力來看誰比較適合統治中國.", Say2 = "", Say3 = "" },
      new TalkParam( 馬克思 ) { Say1 = "我只是一個思想家!", Say2 = "", Say3 = "" },
    };
  }

  public void initTalkBlockBattle_03() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 慈禧 ) { Say1 = "你竟然跟美國人聯手, 是漢奸嗎!", Say2 = "" },
      new TalkParam( 蔡英文 ) { Say1 = "我這樣做是為了拯救中國萬民,", Say2 = "我希望中國大陸上的人也能得到自由.", Say3 = "" },
      new TalkParam( 慈禧 ) { Say1 = "有一點我是認同共產黨的,", Say2 = "不能照搬西方那一套.", Say3 = "" },
      new TalkParam( 蔡英文 ) { Say1 = "在我看來,", Say2 = "這只是中共不肯放權的藉口.", Say3 = "" },
      new TalkParam( 慈禧 ) { Say1 = "你們台灣民主化了,", Say2 = "經濟不見得比大陸好.", Say3 = "" },
      new TalkParam( 蔡英文 ) { Say1 = "中國總理說過有6億人收入不到1000元呢.", Say2 = "", Say3 = "" },
      new TalkParam( 慈禧 ) { Say1 = "擊倒我之後,", Say2 = "中國就會由中共管治,", Say3 = "不想這樣的話, 你和我聯手吧." },
      new TalkParam( 蔡英文 ) { Say1 = "抱歉,", Say2 = "帝制是比共產主義更大的敵人.", Say3 = "" },
      new TalkParam( 慈禧 ) { Say1 = "那我就不手下留情了!", Say2 = "", Say3 = "" },
    };
  }

  public void initTalkBlockBattle_04() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 侵侵 ) { Say1 = "你們中國的事 我不想插手,", Say2 = "但你攻打台灣 ", Say3 = "我就不能坐視不管了." },
      new TalkParam( 慈禧 ) { Say1 = "八國聯軍也有你們份,", Say2 = "這仇就趁今次報了!" },
      new TalkParam( 侵侵 ) { Say1 = "那時候我沒出生,", Say2 = "不要找我報仇.", Say3 = "你們撤軍吧, 在談判桌上, " },
      new TalkParam( 侵侵 ) { Say1 = "你們國民黨,共產黨,清廷,", Say2 = "可以協商新的道路.", Say3 = "" },
      new TalkParam( 慈禧 ) { Say1 = "那些叛黨憑什麼跟我談判,", Say2 = "既然那些觀察者給了我這些機甲,", Say3 = "我們也不怕你們美國人了." },
      new TalkParam( 侵侵 ) { Say1 = "光緒皇帝還沒被你毒死吧?", Say2 = "", Say3 = "" },
      new TalkParam( 慈禧 ) { Say1 = "...你哪裡聽來的, 胡說!", Say2 = "", Say3 = "" },
    };
  }

  public void initTalkBlockBattle_05() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 慈禧 ) { Say1 = "外交官也上戰場幹嗎?", Say2 = "" },
      new TalkParam( 肥蓬 ) { Say1 = "一個老太婆上戰場就可以嗎?", Say2 = "", Say3 = "" },
      new TalkParam( 慈禧 ) { Say1 = "你這頭肥豬,", Say2 = "我要讓你說不出話來!", Say3 = "" },
      new TalkParam( 肥蓬 ) { Say1 = "告訴你, 1年後我已經成功減肥了. ", Say2 = "恭親王不在嗎?", Say3 = "聽說他對外國使節比較客氣," },
      new TalkParam( 肥蓬 ) { Say1 = "外交手段也比你高明.", Say2 = "", Say3 = "" },
      new TalkParam( 慈禧 ) { Say1 = "...恐怕我沒機會再見到他了", Say2 = "", Say3 = "" },
    };
  }

  public void initTalkBlockBattle_06() {
    talkBlock = new List<TalkParam> {
      new TalkParam( hero1 ) { Say1 = "你的意念很強大.", Say2 = "" },
      new TalkParam( 慈禧 ) { Say1 = "你是那個未來人吧,", Say2 = "跟觀察者有關係?", Say3 = "" },
      new TalkParam( hero1 ) { Say1 = "我不認識你們說的那個觀察者.", Say2 = "", Say3 = "" },
      new TalkParam( 慈禧 ) { Say1 = "雖然和你無仇,", Say2 = "但擋我去路者都不得不消滅.", Say3 = "" },
      new TalkParam( hero1 ) { Say1 = "我也不能放棄任務.", Say2 = "來吧!", Say3 = "" },
    };
  }

  public void initTalkBlockBattle_07() {
    talkBlock = new List<TalkParam> {
      new TalkParam( hero2 ) { Say1 = "你是殭屍嗎...好恐怖!", Say2 = "" },
      new TalkParam( 慈禧 ) { Say1 = "現在還不是...,", Say2 = "電視劇看太多了你?", Say3 = "" },
      new TalkParam( hero2 ) { Say1 = "我不欺負老太婆.", Say2 = "", Say3 = "" },
      new TalkParam( 慈禧 ) { Say1 = "那你退下吧.", Say2 = "", Say3 = "" },
      new TalkParam( hero2 ) { Say1 = "不行, 任務是要阻止你!", Say2 = "", Say3 = "" },
    };
  }

  public void initTalkBlockBattle_08() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 山本五十六 ) { Say1 = "孫先生,", Say2 = "我們是朋友,", Say3 = "請您不要與日本為敵!" },
      new TalkParam( 孫中山 ) { Say1 = "我感謝日本人支持我的革命事業,", Say2 = "但我的革命就是推翻滿清,", Say3 = "而你們竟然聯手了..." },
      new TalkParam( 山本五十六 ) { Say1 = "時代變了,", Say2 = "慈禧認同大東亞共榮圈,", Say3 = "他們比共產黨和國民黨都要好談." },
      new TalkParam( 孫中山 ) { Say1 = "請你回去吧,", Say2 = "我不想與日本發生全面戰爭.", Say3 = "" },
      new TalkParam( 山本五十六 ) { Say1 = "現在的我們,", Say2 = "就算美國人再投兩顆原子彈我也不怕!", Say3 = "" },
    };
  }

  public void initTalkBlockBattle_09() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 侵侵 ) { Say1 = "二戰的亡靈!", Say2 = "", Say3 = "" },
      new TalkParam( 山本五十六 ) { Say1 = "美國人不要插手東亞的事,", Say2 = "大東亞共榮圈成立後,", Say3 = "我們的合作機會還有很多!" },
      new TalkParam( 侵侵 ) { Say1 = "台灣是第一島鏈,", Say2 = "不能失守!", Say3 = "" },
      new TalkParam( 山本五十六 ) { Say1 = "說到底你們美國人也只關心自己的利益,", Say2 = "我們東亞人民必須團結起來", Say3 = "才能擺脫西方白人的奴役呢!" },
      new TalkParam( 侵侵 ) { Say1 = "不能讓你們法西斯軸心國在這個年代死灰復燃!", Say2 = "", Say3 = "" },
    };
  }

  public void initTalkBlockBattle_10() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 山本五十六 ) { Say1 = "這個人看來擁有豐富的戰鬥經驗...", Say2 = "", Say3 = "" },
      new TalkParam( 彭斯 ) { Say1 = "要讓你嘗嘗美軍的戰鬥方式", Say2 = "", Say3 = "" },
      new TalkParam( 山本五十六 ) { Say1 = "當年如果不是被美軍幸運拾獲了零式戰機殘骸,", Say2 = "找到了零式的弱點,", Say3 = "太平洋戰爭還是我們大日本帝國海軍佔了上風!" },
      new TalkParam( 彭斯 ) { Say1 = "是嗎, 運氣也是實力的一部分", Say2 = "", Say3 = "" },
    };
  }

  public void initTalkBlockBattle_11() {
    talkBlock = new List<TalkParam> {
      new TalkParam( hero1 ) { Say1 = "日本帝國海軍和陸軍為什麼會不和啊", Say2 = "", Say3 = "" },
      new TalkParam( 山本五十六 ) { Say1 = "如果日本沒有內鬥,", Say2 = "海軍和陸軍團結一致,", Say3 = "我們早就可以征服世界了!" },
      new TalkParam( hero1 ) { Say1 = "歷史沒有那麼多如果啊...", Say2 = "", Say3 = "" },
      new TalkParam( 山本五十六 ) { Say1 = "我們也不應該襲擊珍珠港,", Say2 = "把美國這個世界工廠捲進來,", Say3 = "他們的生產力和補給太強大了" },
      new TalkParam( hero1 ) { Say1 = "雖然神風突擊隊也消滅了不少美國航母,", Say2 = "但最後還是敗給了原子彈吧", Say3 = "" },
      new TalkParam( 山本五十六 ) { Say1 = "如果沒有原子彈,", Say2 = "還不知道要推多少人去特攻了...", Say3 = "" },
    };
  }

  public void initTalkBlockBattle_12() {
    talkBlock = new List<TalkParam> {
      new TalkParam( hero2 ) { Say1 = "你們的零式戰機真的好帥!", Say2 = "", Say3 = "" },
      new TalkParam( 山本五十六 ) { Say1 = "那你加入我們吧,", Say2 = "讓你坐上去試試", Say3 = "" },
      new TalkParam( hero2 ) { Say1 = "怎麼可以加入敵軍陣營呢", Say2 = "", Say3 = "" },
      new TalkParam( 山本五十六 ) { Say1 = "戰場上我不會手下留情的", Say2 = "", Say3 = "" },
      new TalkParam( hero2 ) { Say1 = "你打得中我再說~~嘻", Say2 = "", Say3 = "" },
    };
  }

  // END OF 角色對戰時對話--------------------------------------------------------------------------------------------------------

  protected override List<ObjectiveData> objDataList { get; set; } = new List<ObjectiveData>() {
    new ObjectiveData() {
      WinStrings = new string[] { "擊墜慈禧" },
      LoseStrings = new string[] { "味方全滅" },
      HintStrings = new string[] { "敵軍將有一次增援" },
    },
    new ObjectiveData() {
      WinStrings = new string[] { "擊墜慈禧" },
      LoseStrings = new string[] { "味方全滅" },
      HintStrings = new string[] { "注意神風特攻會造成極大的傷害" }  
    }
  };


  // 角色對戰時對話--------------------------------------------------------------------------------------------------------


  private void setupTalkBeforeBattle() {
    TalkBeforeBattleList.Add( new TalkBeforeBattle() { PilotIds = new List<int>() { 慈禧.ID, 孫中山.ID }, GetTalkBlock = initTalkBlockBattle_01 } );
    TalkBeforeBattleList.Add( new TalkBeforeBattle() { PilotIds = new List<int>() { 慈禧.ID, 馬克思.ID }, GetTalkBlock = initTalkBlockBattle_02 } );
    TalkBeforeBattleList.Add( new TalkBeforeBattle() { PilotIds = new List<int>() { 慈禧.ID, 蔡英文.ID }, GetTalkBlock = initTalkBlockBattle_03 } );
    TalkBeforeBattleList.Add( new TalkBeforeBattle() { PilotIds = new List<int>() { 慈禧.ID, 侵侵.ID }, GetTalkBlock = initTalkBlockBattle_04 } );
    TalkBeforeBattleList.Add( new TalkBeforeBattle() { PilotIds = new List<int>() { 慈禧.ID, 肥蓬.ID }, GetTalkBlock = initTalkBlockBattle_05 } );
    TalkBeforeBattleList.Add( new TalkBeforeBattle() { PilotIds = new List<int>() { 慈禧.ID }, HeroSeq = hero1.SeqNo, GetTalkBlock = initTalkBlockBattle_06 } );
    TalkBeforeBattleList.Add( new TalkBeforeBattle() { PilotIds = new List<int>() { 慈禧.ID }, HeroSeq = hero2.SeqNo, GetTalkBlock = initTalkBlockBattle_07 } );

    TalkBeforeBattleList.Add( new TalkBeforeBattle() { PilotIds = new List<int>() { 山本五十六.ID, 孫中山.ID }, GetTalkBlock = initTalkBlockBattle_08 } );
    TalkBeforeBattleList.Add( new TalkBeforeBattle() { PilotIds = new List<int>() { 山本五十六.ID, 侵侵.ID }, GetTalkBlock = initTalkBlockBattle_09 } );
    TalkBeforeBattleList.Add( new TalkBeforeBattle() { PilotIds = new List<int>() { 山本五十六.ID, 彭斯.ID }, GetTalkBlock = initTalkBlockBattle_10 } );
    TalkBeforeBattleList.Add( new TalkBeforeBattle() { PilotIds = new List<int>() { 山本五十六.ID }, HeroSeq = hero1.SeqNo, GetTalkBlock = initTalkBlockBattle_11 } );
    TalkBeforeBattleList.Add( new TalkBeforeBattle() { PilotIds = new List<int>() { 山本五十六.ID }, HeroSeq = hero2.SeqNo, GetTalkBlock = initTalkBlockBattle_12 } );
  }

}
