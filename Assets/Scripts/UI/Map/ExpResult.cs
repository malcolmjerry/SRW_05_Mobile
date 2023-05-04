using DataModel.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpResult : MonoBehaviour {

  public Transform LvUpBlock, ExpBlock;
  public List<Transform> PartsBlockList;
  Action callback;

  PilotInfo pilotInfo;
  int exp;
  int oldLv, newLv;

  public PilotLvUpDetails PilotLvUpDetails;

  GameDataService gameDataService;

  private void Awake() {
    gameDataService = DIContainer.Instance.GameDataService;
  }

  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" ) || Input.GetButtonDown( "Confirm" ) || Input.GetButtonDown( "Start" )) {
      gameObject.SetActive( false );
      if (newLv > oldLv) {
        PilotLvUpDetails.Setup( exp, pilotInfo, callback );
      }
      else {
        pilotInfo.PilotInstance.Exp += exp;  //由於沒有升級, 重新加回這次增加的經驗值, 不需更新能力值和技能等
        callback();
      }
    }
  }

  public void Setup( int money, int exp, PilotInfo pilotInfo, List<PartsInstance> partsList, Action callback ) {
    this.callback = callback;
    this.pilotInfo = pilotInfo;
    this.exp = exp;

    oldLv = pilotInfo.Level;
    bool isLvUp = pilotInfo.AddExp( exp );
    newLv = pilotInfo.Level;

    if (isLvUp) {  //如有升級才會進入下一個界面
      LvUpBlock.gameObject.SetActive( true );
      LvUpBlock.Find( "OldLvTxt" ).GetComponent<Text>().text = oldLv.ToString();
      LvUpBlock.Find( "NewLvTxt" ).GetComponent<Text>().text = newLv.ToString();
      EffectSoundController.PLAY_LV_UP1();
    }
    else LvUpBlock.gameObject.SetActive( false );

    ExpBlock.Find( "GetMoneyTxt" ).GetComponent<Text>().text = money.ToString();
    ExpBlock.Find( "GetExpTxt" ).GetComponent<Text>().text = exp.ToString();

    gameDataService.AddMoney( money );

    for (int i=0; i<PartsBlockList.Count; i++) {
      var block = PartsBlockList[i];
      if (partsList.Count > i) {
        block.gameObject.SetActive( true );
        block.Find( "GetPartsTxt" ).GetComponent<Text>().text = partsList[i].Parts.Name;
      }
      else block.gameObject.SetActive( false );
    }
    gameObject.SetActive( true );
    enabled = false;

    pilotInfo.PilotInstance.Exp -= exp;  //減去這次增加的經驗值, 以便下一個界面顯示前後的技能,精神和能力的差值
    CoroutineCommon.CallWaitForSeconds( .5f, () => enabled = true );  //禁止立刻關閉窗口
  }

}
