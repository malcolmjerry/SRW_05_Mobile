using DataModel.Service;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static DataModel.Service.GameDataService;

public class Story_002_End : StoryBase {

  private void Awake() {

  }

  // Use this for initialization
  void Start() {

    var go = Instantiate( Resources.Load( "Story/EarthCanvas" ) );
    go.name = "EarthCanvas";
    BGMController.SET_BGM( "F_作戦立てる", true, 10 );

    CoroutineCommon.CallWaitForOneFrame( () => {
      smc = GameObject.Find( "EarthCanvas" ).GetComponent<StoryMapCanvas>();
      tc  = GameObject.Find( "TalkCanvas" ).GetComponent<TalkCanvas>();

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
        TalkNext();  //Do nothing, just talk;
        break;
      default:
        Debug.Log( $"No this block({blockNo})!" );
        break;
    }
  }

  private void initBlockList() {
    Pilot 孫中山 = pilotService.LoadPilotBase( 1307 );
    //Pilot 國民革命軍 = pilotService.LoadPilotBase( 9933 );
    Pilot 蔡英文 = pilotService.LoadPilotBase( 405 );
    Pilot 馬克思 = pilotService.LoadPilotBase( 204 );
    Hero hero1 = pilotService.LoadHero( 1 );
    Hero hero2 = pilotService.LoadHero( 2 );

    blockList = new List<List<TalkParam>> {
      //Paragraph 1
      new List<TalkParam> {
        new TalkParam( 孫中山 ){ Say1 = "呼...終於把敵人擊退了,", Say2 = "但他們很快會再來,", Say3 = "我們要立刻重整軍力."  },
        new TalkParam( 馬克思 ){ Say1 = "這防衛線應該撐不住了,", Say2 = "要不引誘他們進內陸,", Say3 = "我們再以游擊戰術消耗他們？"  },
        new TalkParam( 蔡英文 ){ Say1 = "我已聯系美軍來支援了,", Say2 = "但對方回應的人讓我有些意外,", Say3 = "那個年代的美國總統看來也被轉移到了這個年代呢..."  },
        new TalkParam( hero1 ){ Say1 = $"能支援幾位前輩是我的榮幸,", Say2 = "那大家先回基地補給吧.", Say3 = "" },
        new TalkParam( hero2 ){ Say1 = $"我肚子很餓了！", Say2 = "", Say3 = "" },
      }
    };
  }

}
