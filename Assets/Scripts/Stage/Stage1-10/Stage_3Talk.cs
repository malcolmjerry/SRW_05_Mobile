using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DataModel.Service;
using UnityEngine.SceneManagement;
using System;

public partial class Stage_3 : StageBase {

  Pilot 侵侵, 拜登, 彭斯, 肥蓬;

  Vector3 talkCenter = new Vector3( -5, 5, -34 );

  //public override List<int> TalkEventList { get; set; }

  protected override List<TalkBeforeBattle> TalkBeforeBattleList { get; set; } = new List<TalkBeforeBattle>();

  public override void SetupTalk() {
    侵侵 = pilotService.LoadPilotBase( 107 );
    拜登 = pilotService.LoadPilotBase( 108 );
    彭斯 = pilotService.LoadPilotBase( 112 );
    肥蓬 = pilotService.LoadPilotBase( 113 );
    setupTalkBeforeBattle();
  }

  private void initTalkBlock_startMap() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 侵侵 ) { Say1 = "拜登啊, ", Say2 = "那個自稱觀察者的人既然給了我們這種力量, ", Say3 = "那我們就來決一死戰吧," },
      new TalkParam( 侵侵 ) { Say1 = "當年你操控了選舉結果, ", Say2 = "法律無法制裁你, ", Say3 = "但這次你逃不掉了." },
      new TalkParam( 拜登 ) { Say1 = "看來觀察者給予我和你的力量並不對等, ", Say2 = "看你那破機兵, 能幹出什麼?", Say3 = "怎麼可能打倒我?" },
      new TalkParam( 拜登 ) { Say1 = "再說, ", Say2 = "選舉舞弊是你一面之詞, ", Say3 = "我可沒有幹過那種事!" },
      new TalkParam( 侵侵 ) { Say1 = "問答無用!", Say2 = "彭彭, 肥蓬, 要上了!", Say3 = "" },
      new TalkParam( 彭斯 ) { Say1 = "好! ", Say2 = "(聽說由於我們的出現,", Say3 = "這個年代本來的美國總統和他的班子都突然消失了," },
      new TalkParam( 彭斯 ) { Say1 = "到底是什麼人在背後圖謀着什麼...) ", Say2 = "", Say3 = "" },
      new TalkParam( 肥蓬 ) { Say1 = "明明那個五維的高等文明才是我們的最大敵人,", Say2 = "但現在不得不先處理眼前的障礙!", Say3 = "" },
    };
  }

  // 拜登HP 減至一半以下
  private void initTalkBlock_bossDestroyed() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 拜登 ) { Say1 = "看來我還沒習慣這台強大機兵的操作,", Say2 = "這次就玩到這裡,", Say3 = "下次就不會放過你們!" },
      new TalkParam( 侵侵 ) { Say1 = "想逃, 我不會讓你逃掉!", Say2 = "", Say3 = "" },
      new TalkParam( 彭斯 ) { Say1 = "收到消息, ", Say2 = "中國大軍已開始入侵台灣!", Say3 = "" },
      new TalkParam( 侵侵 ) { Say1 = "可惡, 竟在這時候...", Say2 = "", Say3 = "" },
      new TalkParam( 肥蓬 ) { Say1 = "蔡英文已向我們發出支援要請了.", Say2 = "", Say3 = "" },
      new TalkParam( 侵侵 ) { Say1 = "拜登這竊國賊下次再收拾,", Say2 = "我們先去台灣支援吧!", Say3 = "" },
      new TalkParam( 拜登 ) { Say1 = "我也要弄清楚光明會到底想要什麼,", Say2 = "全軍撤退!", Say3 = "" }
    };
  }

  // 拜登 撤退後
  private void initTalkBlock_AfterBossRun() {
    talkBlock = new List<TalkParam> {
      new TalkParam( 侵侵 ) { Say1 = "這機兵速度真慢,", Say2 = "全速橫過太平洋趕過去吧!", Say3 = "" },
      new TalkParam( 彭斯 ) { Say1 = "這次轉移就只有我們三個,", Say2 = "不知道台灣那邊情況怎樣.", Say3 = "這年代還有美軍駐守嗎? " },
      new TalkParam( 肥蓬 ) { Say1 = "..監控圖像顯示中國大軍的機兵數量是台灣的5倍,", Say2 = "戰力差距很大.", Say3 = "" },
      new TalkParam( 侵侵 ) { Say1 = "我們都是精英,", Say2 = "不用擔心太多!", Say3 = "那片土地不能失守!" }
    };
  }

  protected override List<ObjectiveData> objDataList { get; set; } = new List<ObjectiveData>() {
    new ObjectiveData() {
      WinStrings = new string[] { "拜登 HP 降至 50% 以下" },
      LoseStrings = new string[] { "味方任何體機被撃墜" },
      HintStrings = new string[] { "敵軍沒有增援", "較強大的敵人可能帶着寶物啊~" },
    }
  };


  // 角色對戰時對話--------------------------------------------------------------------------------------------------------


  private void setupTalkBeforeBattle() {

  }

}
