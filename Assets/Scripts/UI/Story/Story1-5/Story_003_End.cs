using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static DataModel.Service.GameDataService;

public class Story_003_End : StoryBase {

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

      smc.SetZoomPos( 5940, 810 );  //華盛頓 D.C.
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
      case 1:
        tc.TurnOff();
        CoroutineCommon.CallWaitForSeconds( 1f, () => smc.MoveTo( 2550, 490, 5f ) );  //台灣
        CoroutineCommon.CallWaitForSeconds( 6f, () => tc.ShowPlace( "台灣" ) );
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

    blockList = new List<List<TalkParam>> {
      new List<TalkParam> {
        new TalkParam( 彭斯 ){ Say1 = "大家抓穩!", Say2 = "白色基地, 最大戰速!", Say3 = "目標: 台灣島"  },
      },
      new List<TalkParam> {
        new TalkParam( 侵侵 ){ Say1 = "光明會...", Say2 = "背後操控着無數地球企業和政府高官的幕後黑手,", Say3 = "到底和這次轉移事件有沒有關係...?"  },
        new TalkParam( 肥蓬 ){ Say1 = "看來這邊剛經歷了一場激烈的戰鬥,", Say2 = "我們先和台灣軍合流吧", Say3 = ""  },
        new TalkParam( 彭斯 ){ Say1 = "不知道敵軍的後盾有多強大,", Say2 = "大家也要做好激戰的準備...", Say3 = ""  },
      },
    };
  }

}
