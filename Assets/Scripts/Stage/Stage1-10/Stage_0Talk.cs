using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DataModel.Service;
using UnityEngine.SceneManagement;
using System;

public partial class Stage_0 : StageBase {

  Pilot 克羅那, 伍索, 馬貝特;
  Pilot 海依思;
  Pilot 甲兒, 沙也加;
  Pilot 林友德, 阿寶;

  protected override List<TalkBeforeBattle> TalkBeforeBattleList { get; set; } = new List<TalkBeforeBattle>();

  public override void SetupTalk() {
    克羅那 = pilotService.LoadPilotBase( 702 );
    伍索 = pilotService.LoadPilotBase( 701 );
    馬貝特 = pilotService.LoadPilotBase( 705 );
    海依思 = pilotService.LoadPilotBase( 9904 );
    林友德 = pilotService.LoadPilotBase( 505 );
    阿寶 = pilotService.LoadPilotBase( 201 );
    甲兒 = pilotService.LoadPilotBase( 1301 );
    沙也加 = pilotService.LoadPilotBase( 1302 );
  }

  //private int block = 0;
  private List<List<TalkParam>> talkBlockList;
  private void initTalkBlock_1() {
    talkBlockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        /*
        new TalkParam() { Pilot = 克羅那, Say1 = "從哪裡冒出來的,", Say2 = "那部小型機 ！？" },
        new TalkParam() { Pilot = 伍索, Say1 = "！ 這 這家伙．．．", Say2 = "好纏人啊 ！！" },
        new TalkParam() { Pilot = 馬貝特, Say1 = "伍索 ！！", Say2 = "聽到嗎 ！？", Position = new Vector3( 9, 0, 0 ) },
        new TalkParam() { Pilot = 伍索, Say1 = $"！ {馬貝特.ShortName} ？", Say2 = "聽 我聽到啊 ！" },
        new TalkParam() { Pilot = 馬貝特, Say1 = "伍索 ！", Say2 = "準備合體 ！", Position = new Vector3( 9, 0, 0 ) },
        new TalkParam() { Pilot = 馬貝特, Say1 = "盡可能遠離敵人啊 ！", Say2 = "明白嗎 ！？ 伍索 ！！", Position = new Vector3( 9, 0, 0 ) },
        new TalkParam() { Pilot = 伍索, Say1 = "了解 ！！", Say2 = "" },
        new TalkParam() { Pilot = 馬貝特, Say1 = "要來了 ！", Say2 = "伍索 ！！", Position = new Vector3( 9, 0, 0 ) },
        new TalkParam() { Pilot = 伍索, Say1 = "了解 ！！", Say2 = "" },
        new TalkParam() { Pilot = 馬貝特, Say1 = "組件 ！", Say2 = "射出 ！！", Position = new Vector3( 9, 0, 0 ) },
        new TalkParam() { Pilot = 伍索, Say1 = "V Model 對準軸 ！！", Say2 = "", Waiting = 2f, BGM = "AL_DON’T STOP！CARRY ON！" },
        new TalkParam() { Pilot = 伍索, Say1 = "好了 ！！", Say2 = "" },
        new TalkParam() { Pilot = 克羅那, Say1 = "什麼 ！？", Say2 = "合體了．．． ！？" },
        */
      }
    };
  }
  private void initTalkBlock_2() {

    talkBlockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        //new TalkParam() { Pilot = 克羅那, Say1 = "！？ Sensor壞了嗎 ！！,", Say2 = "" }
      }
    };
  }
  private void initTalkBlock_3() {
    talkBlockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        /*
        new TalkParam() { Pilot = 海依思, Say1 = $"{克羅那.ShortName} 中尉！！,", Say2 = "你沒事吧 ！？" },
        new TalkParam() { Pilot = 克羅那, Say1 = $"拉肯基地 來的援軍嗎 ！,", Say2 = "得救了 ！" },
        new TalkParam() { Pilot = 克羅那, Say1 = $"我先撤回 拉肯基地 ！,", Say2 = "之後就交給你們了 ！！" },
        */
      }
    };
  }
  private void initTalkBlock_4() {
    talkBlockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        /*
        new TalkParam() { Pilot = 伍索, Say1 = "竟然有這麼多敵人 ！？", Say2 = "" },
        new TalkParam() { Pilot = 馬貝特, Say1 = "伍索 ！！ 別勉強！", Say2 = "很快救援就到了 ！！", Position = new Vector3( 9, 0, 0 ) },
        new TalkParam() { Pilot = 伍索, Say1 = "救援 ？", Say2 = "是什麼救援啊 ！？" },
        new TalkParam() { Pilot = 馬貝特, Say1 = "比起這種事 ", Say2 = "現在你還是先想辦法活下去 ！！", Position = new Vector3( 9, 0, 0 ) },
        new TalkParam() { Pilot = 馬貝特, Say1 = "好嗎 ？", Say2 = "絕對不要硬來啊 ！", Position = new Vector3( 9, 0, 0 ) },            //馬貝特
        new TalkParam() { Pilot = 伍索, Say1 = "．．．了解 ！！", Say2 = "" },
        new TalkParam() { Pilot = 伍索, Say1 = "不要硬來但又要活下去，", Say2 = "這種事，能做到嗎。。。" },  //胡索
        */
      }
    };
  }
  private void initTalkBlock_5() {
    talkBlockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        /*
        new TalkParam() { Pilot = 甲兒, Say1 = "什麼 ！？ 除了我們以外 ", Say2 = "竟然還有其他人和 DC 戰鬥啊 ！？" },  //甲兒
        new TalkParam() { Pilot = 沙也加, Say1 = "看來是了。", Say2 = "我試試連絡看看吧。" },   //沙也加
        new TalkParam() { Pilot = 沙也加, Say1 = "那邊的 白色機器人的人 ！", Say2 = "你也在和 DC 戰鬥是吧 ？" },   //沙也加
        new TalkParam() { Pilot = 沙也加, Say1 = "如果是的話 請回答！", Say2 = "我們也來幫忙！" },   //沙也加
        new TalkParam() { Pilot = 伍索, Say1 = $"{馬貝特.ShortName}姐、 ", Say2 = "說句話啊 ！", Say3= "那就是救援嗎 ！？" },  //胡索
        new TalkParam() { Pilot = 馬貝特, Say1 = "不是、 搞錯了啊．．．", Say2 = "但既然也是和 DC 戰鬥的人，", Say3 = "或許能幫忙我們。", Position = new Vector3( 9, 0, 0 ) },
        new TalkParam() { Pilot = 伍索, Say1 = $"那麼、拜託他們幫忙就好了！？" },  //胡索
        new TalkParam() { Pilot = 伍索, Say1 = $"我是 {伍索.FirstName}={伍索.LastName}！", Say2 = "這部 V 鋼彈的駕駛員 ！" },  //胡索
        new TalkParam() { Pilot = 甲兒, Say1 = "搞什．．． 小孩嗎！？", Say2 = "" },  //甲兒
        new TalkParam() { Pilot = 伍索, Say1 = $"小孩又好 什麼都好，", Say2 = "為了生存下去，", Say3 = "不得不戰鬥吧 ！？" },  //胡索
        new TalkParam() { Pilot = 伍索, Say1 = $"比起這種事，", Say2 = "到底是要幫忙、", Say3 = "還是不幫啊 ！？" },  //胡索
        new TalkParam() { Pilot = 甲兒, Say1 = "切、 真不是個純真的小孩啊．．．", Say2 = "明白了！ 就幫你們啦 ！" },  //甲兒
        new TalkParam() { Pilot = 沙也加, Say1 = "修理這類的支援就交給我吧 ！", Say2 = "你也在和 DC 戰鬥是吧 ？" },   //沙也加
        new TalkParam() { Pilot = 伍索, Say1 = $"．．．多謝了 ！", Say2 = "", Say3 = "" },  //胡索
        */
      }
    };
  }
  private void initTalkBlock_6() {
    talkBlockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        /*
        new TalkParam() { Pilot = 伍索, Say1 = $"什麼．．． 這種感覺．．．", Say2 = "有誰要來了 ？", Say3 = "" },  //胡索
        new TalkParam() { Pilot = 甲兒, Say1 = $"喂！ {pilotService.LoadPilotBase( 701 ).ShortName} ！", Say2 = "搞什麼站著發呆啊 ！？", Position = stageManager.GetPosByPilotOrRobot( 1301 ) },  //甲兒
        new TalkParam() { Pilot = 伍索, Say1 = $"有些什麼要來了 ！", Say2 = "", Say3 = "", Position = stageManager.GetPosByPilotOrRobot( 701 ) },  //胡索
        */
      }
    };
  }
  private void initTalkBlock_7() {
    talkBlockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        /*
        new TalkParam() { Pilot = 林友德, Say1 = $"是那個嗎 ？", Say2 = "就是 利加．密特亞 的 V高達．．．", Say3 = "" },  //布拉度
        new TalkParam() { Pilot = 阿寶, Say1 = $"那邊一起戰鬥的機器人是什麼呢 ？", Say2 = "看來不像是 高達．．．", Say3 = "" },  //阿寶
        //new TalkParam() { PilotID = 1601, Say1 = $"那不是 鐵甲萬能俠 嗎 ！", Say2 = "原來 甲兒君 也平安無事呢 ！", Say3 = "", Position = stageManager.GetPosByPilotOrRobot( 1601 ) },  //龍馬
        //new TalkParam() { PilotID = 505, Say1 = $"是認識的嗎？", Say2 = "", Say3 = "", Position = stageManager.GetPosByPilotOrRobot( 505 ) },  //布拉度
        //new TalkParam() { PilotID = 1601, Say1 = $"嗯嗯 ！　話說是一起戰鬥過的同伴。", Say2 = "可以成為我們的力量啊 。", Say3 = "", Position = stageManager.GetPosByPilotOrRobot( 1601 ) },  //龍馬
        //new TalkParam() { PilotID = 705, Say1 = "來了 ！！", Say2 = $"{pilotService.LoadPilotBase( 701 ).ShortName}，", Say3 = "救援終於到了 ！！", Position = new Vector3( 9, 0, 0 ) },   //馬貝特
        //new TalkParam() { PilotID = 701, Say1 = $"高達類型的機動戰士．．．", Say2 = "不、那是 Data 中記載著的，", Say3 = "RX-78-2 高達 ！？", Position = stageManager.GetPosByPilotOrRobot( 701 ) },  //胡索
        //new TalkParam() { PilotID = 701, Say1 = $"那麼說的話，", Say2 = $"就是那個　{pilotService.LoadPilotBase( 201 ).ShortName} 和 白色基地 是吧 ！？", Say3 = "", Position = stageManager.GetPosByPilotOrRobot( 701 ) },  //胡索
        //new TalkParam() { PilotID = 1301, Say1 = $"呵！ 那不是 三一萬能俠 嗎！？", Say2 = "好久不見．．．", Say3 = "但不是說這種話的時候呢。", Position = stageManager.GetPosByPilotOrRobot( 1301 ) },  //甲兒
        //new TalkParam() { PilotID = 1301, Say1 = $"不好意思，", Say2 = "這些傢伙的數量實在太多了！", Say3 = "幫幫忙吧 !", Position = stageManager.GetPosByPilotOrRobot( 1301 ) },  //甲兒
        //new TalkParam() { PilotID = 505, Say1 = $"明白了 ！", Say2 = "各機 把敵人逐個擊破 ！", Say3 = "", Position = stageManager.GetPosByPilotOrRobot( 505 ) },  //布拉度
        //new TalkParam() { PilotID = 201, Say1 = $"{pilotService.LoadPilotBase( 201 ).ShortName} 高達、", Say2 = "去～啦 ！", Say3 = "", Position = stageManager.GetPosByPilotOrRobot( 201 ) },  //阿寶
        //new TalkParam() { PilotID = 1601, Say1 = $"要去了 ！", Say2 = $"{pilotService.LoadPilotBase( 1602 ).ShortName} 、 {pilotService.LoadPilotBase( 1603 ).ShortName} !", Position = stageManager.GetPosByPilotOrRobot( 1601 ) },  //龍馬
        //new TalkParam() { PilotID = 1602, Say1 = $"哦哦", Position = stageManager.GetPosByPilotOrRobot( 1602 ) },  //隼人
        //new TalkParam() { PilotID = 1603, Say1 = $"好 ！", Position = stageManager.GetPosByPilotOrRobot( 1603 ) },  //武藏
        new TalkParam() { Pilot = 克羅那, Say1 = "我先撤退了, 後面就交給你們" }
        */
      }
    };
  }
  private void initTalkBlock_8() {
    talkBlockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        /*
        new TalkParam() { Pilot = 林友德, Say1 = $"好， 戰鬥完了。", Say2 = "各機、歸還命令。", Say3 = "" },  //布拉度
        new TalkParam() { Pilot = 馬貝特, Say1 = $"{林友德.ShortName} 艦長是嗎?", Say2 = $"感謝你的到來。", Say3 = "", Position = new Vector3( 9, 0, 0 ) },   //馬貝特
        new TalkParam() { Pilot = 馬貝特, Say1 = $"我是 {馬貝特.FirstName}={馬貝特.LastName}，",
                          Say2 = $"是 利加．密特亞 的一員。", Say3 = "", Position = new Vector3( 9, 0, 0 ) },   //馬貝特
        new TalkParam() { Pilot = 林友德, Say1 = $"能趕上 太好了。", Say2 = "因受到 利亞．密特亞 各位的照顧呢。", Say3 = "" },  //布拉度
        new TalkParam() { Pilot = 馬貝特, Say1 = $"就在這裡前方 雖然很小，", Say2 = $"有我們的基地。", Say3 = "", Position = new Vector3( 9, 0, 0 ) },   //馬貝特
        new TalkParam() { Pilot = 馬貝特, Say1 = $"請在那裡補給吧。", Say2 = "", Say3 = "", Position = new Vector3( 9, 0, 0 ) },   //馬貝特
        new TalkParam() { Pilot = 伍索, Say1 = $"．．．為什麼要活著", Say2 = "．．．我到底．．．？", Say3 = "" },  //胡索
        new TalkParam() { Pilot = 馬貝特, Say1 = $"{伍索.ShortName}!", Say2 = $"怎麼了 {伍索.ShortName} !?", Say3 = "", Position = new Vector3( 9, 0, 0 ) },   //馬貝特
        new TalkParam() { Pilot = 伍索, Say1 = $"啊．．．嗯．．．{馬貝特.ShortName}．．．", Say2 = "什麼都．．．什麼都沒有呢。", Say3 = "" },  //胡索
        */
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
