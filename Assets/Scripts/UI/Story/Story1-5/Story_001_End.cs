using DataModel.Service;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static DataModel.Service.GameDataService;

public class Story_001_End : StoryBase {

  private void Awake() {

  }

  // Use this for initialization
  void Start() {

    var go = Instantiate( Resources.Load( "Story/EarthCanvas" ) );
    go.name = "EarthCanvas";
    BGMController.SET_BGM( "AL_全能なる調停者", true, 10 );

    CoroutineCommon.CallWaitForOneFrame( () => {
      smc = GameObject.Find( "EarthCanvas" ).GetComponent<StoryMapCanvas>();
      tc  = GameObject.Find( "TalkCanvas" ).GetComponent<TalkCanvas>();

      smc.transform.Find( "Background" ).GetComponent<Image>().color = Color.black;

      smc.SetZoomPos( 2932, 778 );  //Pivot (0.5, 0),  Min (0, 0.5), Max (1, 0.5)
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
        //tc.TurnOff();
        //CoroutineCommon.CallWaitForSeconds( 1f, () => smc.MoveTo( 2416.7f, 525.6f, 3f ) );
        //CoroutineCommon.CallWaitForSeconds( 4f, TalkNext );
        break;
      default:
        Debug.Log( $"No this block({blockNo})!" );
        break;
    }
  }

  private void initBlockList() {
    Pilot 慈禧 = pilotService.LoadPilotBase( 307 );

    blockList = new List<List<TalkParam>> {
      //Paragraph 1
      new List<TalkParam> {
        new TalkParam( 10001, "???" ){ Say1 = $"慈禧啊, 大清氣數已盡, ", Say2 = "但我給你一次反勝的機會, 你要嗎？" }, 
        new TalkParam( 慈禧 ){ Say1 = "你是誰？  想把我帶到哪裡去？ "  },
        new TalkParam( 10001, "???" ){ Say1 = $"我們是地球監察者,", Say2 = "覺得你們地球人類數千年來的戰爭很有趣, ", Say3 = "想試試看更多的可能性." },
        new TalkParam( 10001, "???" ){ Say1 = $"這些新型機動兵器可以幫你打敗孫中山,", Say2 = "我們會派代理人教你們怎麼操作的.", Say3 = "" },
        new TalkParam( 慈禧 ){ Say1 = "這些機器的科技最少領先我們超過 150 年,", Say2 = "這個人到底是...？"  },
      }
    };
  }

}
