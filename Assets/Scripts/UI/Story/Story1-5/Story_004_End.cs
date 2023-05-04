using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static DataModel.Service.GameDataService;

public class Story_004_End : StoryBase {

  // Use this for initialization
  void Start() {

    var go = Instantiate( Resources.Load( "Story/EarthCanvas" ) );
    go.name = "EarthCanvas";
    BGMController.SET_BGM( "2G_WorldMapTalk", true, 10 );

    CoroutineCommon.CallWaitForOneFrame( () => {
      smc = GameObject.Find( "EarthCanvas" ).GetComponent<StoryMapCanvas>();
      tc = GameObject.Find( "TalkCanvas" ).GetComponent<TalkCanvas>();

      smc.SetZoomPos( 2540, 510 );  //台灣海岸
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
      LoadInterMission();
      return;
    }
    talkList = blockList[blockNo];
    index = 0;

    switch (blockNo) {
      case 0:
        TalkNext();
        break;
      default:
        Debug.Log( $"No this block({blockNo})!" );
        break;
    }
  }

  private void initBlockList() {
    Pilot 彭斯 = pilotService.LoadPilotBase( 112 );
    Pilot 侵侵 = pilotService.LoadPilotBase( 107 );
    Pilot 肥蓬 = pilotService.LoadPilotBase( 113 );
    Pilot 孫文 = pilotService.LoadPilotBase( 1307 );
    Pilot 蔡英文 = pilotService.LoadPilotBase( 405 );
    Pilot 馬克思 = pilotService.LoadPilotBase( 204 );
    Hero hero1 = pilotService.LoadHero( 1 );
    Hero hero2 = pilotService.LoadHero( 2 );

    blockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        new TalkParam( 侵侵 ){ Say1 = "那群外星人終於來了,", Say2 = "之前我們一直都接收到來自那邊的信息.", Say3 = ""  },
        new TalkParam( 肥蓬 ){ Say1 = "艦隊在離開三體星系後,", Say2 = "進入了光速,", Say3 = "難道他們已經發明了曲率引擎了!?"  },
        new TalkParam( hero1 ){ Say1 = "我們要立刻到柯伊伯帶去攔截.", Say2 = "", Say3 = ""  },
        new TalkParam( 蔡英文 ){ Say1 = "這個年代人類還沒有恆星際飛船啊.", Say2 = "不可能在短時間內去到柯伊伯帶.", Say3 = ""  },
        new TalkParam( hero2 ){ Say1 = "跟我們轉移來的還有幾首恆星級戰艦,", Say2 = "就在地球軌道上,", Say3 = "我們現在上去吧!"  },

        new TalkParam( 孫文 ){ Say1 = "這是我第一次上宇宙,", Say2 = "國家的事還沒搞定,", Say3 = "就要去和外星人打交道了."  },
        new TalkParam( 馬克思 ){ Say1 = "我們是地球上唯一的戰鬥力,", Say2 = "只能由我們去阻止了.", Say3 = ""  },
        new TalkParam( 蔡英文 ){ Say1 = "我雖活在21世紀,", Say2 = "但也沒上過太空呢.", Say3 = ""  },
        new TalkParam( hero1 ){ Say1 = "各位, 做好準備, ", Say2 = "五分鐘後發射,", Say3 = "到了恆星際戰艦裡可以補給!"  },
        new TalkParam( hero2 ){ Say1 = "我肚子很餓了! ", Say2 = "要先吃點東西...", Say3 = ""  },
        new TalkParam( hero1 ){ Say1 = "現在不是吃飯的時候! ", Say2 = "再忍耐!", Say3 = ""  },
        new TalkParam( 侵侵 ){ Say1 = "希望地球會沒事,", Say2 = "拜登那傢伙...", Say3 = "不知道在圖謀些什麼"  },
        new TalkParam( 肥蓬 ){ Say1 = "說真的,", Say2 = "面對力量未知的外星人,", Say3 = "我們能活着回來嗎."  },
        new TalkParam( 彭斯 ){ Say1 = "只能一直前進了,", Say2 = "我們回來地球後,", Say3 = "再參選下屆總統吧!"  },
        new TalkParam( hero1 ){ Say1 = " 5,4,3,2,1...", Say2 = "發射!!!", Say3 = ""  },
      }
    };
  }

}
