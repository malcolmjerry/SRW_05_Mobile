using DataModel.Service;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreateHeroCtr : MonoBehaviour {

  protected PilotService pilotService;

  public HeroMenu HeroMenu;

  Hero mainHero, subHero = null;

  int status; //0 新狀態   1 已完成主角, 將要設置戀人   2 已完成戀人, 將開始遊戲

  ConfirmDialog confirmDialog;
  GameObject myCanvas;
  HeroData preData = new HeroData();

  private void Awake() {
    pilotService = DIContainer.Instance.PilotService;
  }

  // Start is called before the first frame update
  void Start() {
    //HeroData preData = new HeroData();
    mainHero = preData.GetRandomHero();
    HeroMenu.gameObject.SetActive( true );
    HeroMenu.Setup( mainHero, null, "主人公設定", Callback, NextByHeroMenu );
    HeroMenu.gameObject.SetActive( true );
    myCanvas = GameObject.Find( "MyCanvas" );
    confirmDialog = myCanvas.transform.Find( "ConfirmDialog" ).GetComponent<ConfirmDialog>();
    BGMController.SET_BGM( "F_Time To Come" );


  }

  // Update is called once per frame
  void Update() {

  }

  void Callback() {
    //enabled = true;
    if (status == 0) {
      confirmDialog.Setup(
        "退回到主選單, 當前編輯的信息將會丟失",
        () => myCanvas.GetComponent<FadeInOut>().FadeOut( 1f,
          () => {
            var mySceneManager = GameObject.Find( "MySceneManager" ).GetComponent<MySceneManager>();
            mySceneManager.UnloadAndLoadScene( "Title", SceneManager.GetActiveScene() );
          }, blackTime: 0, hold: true
        ),
        () => { HeroMenu.enabled = true; }  //Cancel
      );
    }
    else {
      confirmDialog.Setup(
        "回到主人公編輯",
        () => myCanvas.GetComponent<FadeInOut>().FadeOut( .5f,
          () => {
            status = 0;
            HeroMenu.Setup( mainHero, null, "主人公設定", Callback, NextByHeroMenu );
            HeroMenu.enabled = true;
          },
          () => {

          },
          blackTime: .5f
        ),
        () => { HeroMenu.enabled = true; }   //Cancel
      );
    }
  }

  void NextByHeroMenu( dynamic hero ) {
    //enabled = true;

    if (status == 0)
      mainHero = hero;
    else if (status == 1)
      subHero = hero;

    status++;

    if (status == 1) {  //將要設定主人公戀人
      confirmDialog.Setup(
        "主人公輸入完成",
        () => myCanvas.GetComponent<FadeInOut>().FadeOut( .5f,
          () => {
            HeroMenu.Setup( subHero, mainHero.PicNo, "主人公の戀人設定", Callback, NextByHeroMenu );
            HeroMenu.enabled = true;
          },
          () => {
            if (subHero == null) {
              subHero = preData.GetRandomHero( mainHero.Sex, mainHero.PicNo );
            }
          },
          blackTime: .5f
        ),
        () => { HeroMenu.enabled = true; }   //Cancel
      );
    }
    else {
      confirmDialog.Setup(
        "戀人輸入完成, 開始遊戲",
        () => myCanvas.GetComponent<FadeInOut>().FadeOut( 1f,
          () => {
            ToNextScene();
          },
          blackTime: .5f
        ),
        () => { HeroMenu.enabled = true; }   //Cancel
      );
    }
  }

  void ToNextScene() {
    mainHero.SeqNo = 1;
    subHero.SeqNo = 2;

    pilotService.CreatePilotInstance( mainHero.GetPilotIdByCharacter(), isPlayer: true, hero: mainHero );
    pilotService.CreatePilotInstance( subHero.GetSubPilotIdByCharacter(), isPlayer: true, hero: subHero, enable: 0 );

    SceneManager.UnloadSceneAsync( "HeroInput" );
    SceneManager.LoadScene( "Story", LoadSceneMode.Additive );
  }

}
