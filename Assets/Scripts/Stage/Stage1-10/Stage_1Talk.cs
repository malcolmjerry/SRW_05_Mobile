using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DataModel.Service;
using UnityEngine.SceneManagement;
using System;

public partial class Stage_1 : StageBase {

  Pilot 孫中山, 同盟會兵士, 義和團, 慈禧;
  Hero hero1;

  Vector3 talkCenter = new Vector3( -5, 5, -34 );

  public override void SetupTalk() {
    孫中山 = pilotService.LoadPilotBase( 1307 );
    同盟會兵士 = pilotService.LoadPilotBase( 9931 );
    義和團 = pilotService.LoadPilotBase( 9935 );
    慈禧 = pilotService.LoadPilotBase( 307 );
    hero1 = pilotService.LoadHero( 1 );
  }

  //private int block = 0;
  private List<List<TalkParam>> talkBlockList;
  private void initTalkBlock_1() {
    talkBlockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        //臨時抽起
        new TalkParam( 孫中山 ) { Say1 = "前面就是紫禁城,", Say2 = "我們要攻破那裡, 把慈禧抓出來！" },
        new TalkParam( 同盟會兵士 ) { Say1 = "前方發現敵人！", Say2 = "" },
        new TalkParam( 義和團 ) { Say1 = "不能讓叛軍越過防線!", Say2 = "大家守住！" },
        new TalkParam( 孫中山 ) { Say1 = "繼續前進！", Say2 = "盡快把敵人擊破！" },
        new TalkParam( 同盟會兵士 ) { Say1 = "看我們革命軍的力量！", Say2 = "" }
      }
    };
  }

  private void initTalkBlock_2() {
    talkBlockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        new TalkParam( 同盟會兵士 ) { Say1 = "敵增援多數出現！", Say2 = "" },
      }
    };
  }

  private void initTalkBlock_2_1() {
    talkBlockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        new TalkParam( 孫中山 ) { Say1 = "想不到紫禁城還保有這麼多戰力..！", Say2 = "" },
      }
    };
  }

  private void initTalkBlock_3() {
    talkBlockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        new TalkParam( 同盟會兵士 ) { Say1 = "！？ 還有增援嗎？", Say2 = "" },
        new TalkParam( hero1 ) {  Say1 = "孫先生, 我是來支援你的", Say2 = "" },
        new TalkParam( 孫中山 ) { Say1 = "你是...?", Say2 = "" },
        new TalkParam( hero1 ) {  Say1 = "志同道合的人吧", Say2 = "現在先不要問我的來歷, 先把眼前的敵人解決!" },
        new TalkParam( 孫中山 ) { Say1 = "我同意! ", Say2 = "與新出現的機體是形成共同戰線!" },
        new TalkParam( 同盟會兵士 ) { Say1 = "明白!", Say2 = "" },
      }
    };
  }

  private void initTalkBlock_4() {
    talkBlockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        new TalkParam( 9904, "太監A", talkCenter ) { Say1 = "啟稟太后, 情勢危急,", Say2 = "革命軍已經攻到城門了, 我們要立刻撤退!" },
        new TalkParam( 慈禧, talkCenter ) { Say1 = "孫文那個逆賊,", Say2 = "想不到短短幾年, 叛軍已成長到這種程度...！" }
      }
    };
  }

  private void initTalkBlock_5() {
    talkBlockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        new TalkParam( 9904, "太監B", talkCenter ) { Say1 = "稟告太后, 外面發生了異常現象！", Say2 = "所有事物都好像扭曲了！" },
        new TalkParam( 慈禧, talkCenter ) { Say1 = "這是怎麼回事, 我從沒見過這種情況...", Say2 = "是敵人的秘密武器嗎？" },
        new TalkParam( 9904, "太監B", talkCenter ) { Say1 = "不知道...一個黑色的球狀物,", Say2 = "正在擴大.. 啊...！" }
      }
    };
  }

  private void initTalkBlock_6() {
    talkBlockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        new TalkParam( 慈禧, talkCenter ) { Say1 = "...是要把我吸到哪裡去嗎, 這東西！", Say2 = "" },
        new TalkParam( 孫中山 ) { Say1 = "這是什麼, 是慈禧製造的東西嗎...！？ ", Say2 = "啊... ！！！" },
        new TalkParam( hero1 ) {  Say1 = "(這時代也有重力異常現象...？)", Say2 = "" },
      }
    };
  }

  protected override List<ObjectiveData> objDataList { get; set; } = new List<ObjectiveData>() {
    new ObjectiveData() {
      WinStrings = new string[] { "敵全滅" },
      LoseStrings = new string[] { "味方任何體機被撃墜" },
      HintStrings = new string[] { "敵人將有一次增援", "需保存戰力" },
    },
    new ObjectiveData() {
      WinStrings = new string[] { "敵全滅" },
      LoseStrings = new string[] { "味方任何體機被撃墜" },
      HintStrings = new string[] {}  //長度為0時, 清空該塊
    }
  };

}
