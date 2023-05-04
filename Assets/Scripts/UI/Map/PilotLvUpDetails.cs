using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PilotLvUpDetails : MonoBehaviour {

  Action callback;
  PilotInfo pilotInfo;

  int oldLv;
  int[] oldSkillLvList = new int[7];
  int[] oldSpComList = new int[7];
  int[] oldStsList = new int[7];

  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {
    if (Input.GetButtonDown( "Back" ) || Input.GetButtonDown( "Cancel" ) || Input.GetButtonDown( "Confirm" ) || Input.GetButtonDown( "Start" )) {
      gameObject.SetActive( false );
      callback();
    }
  }

  public void Setup( int exp, PilotInfo pilotInfo, Action callback ) {
    this.callback = callback;
    this.pilotInfo = pilotInfo;

    EffectSoundController.PLAY_LV_UP2();

    transform.Find( "Photo/Photo" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + pilotInfo.PicNo + "_1" );
    transform.Find( "NameTxt" ).GetComponent<Text>().text = pilotInfo.PilotInstance.DisplayFullName;

    setOldPilotSkillLv();
    setOldSpComs();
    setOldSts();
    oldLv = pilotInfo.Level;

    pilotInfo.PilotInstance.Exp += exp;
    pilotInfo.Update();

    setLevelUp();
    setPilotSkills();
    setSpComs();
    setSts();

    gameObject.SetActive( true );
    enabled = false;
    CoroutineCommon.CallWaitForSeconds( 1f, () => enabled = true );
  }



  void setLevelUp() {
    transform.Find( "LvOldTxt" ).GetComponent<Text>().text = oldLv.ToString();
    transform.Find( "LvNewTxt" ).GetComponent<Text>().text = pilotInfo.Level.ToString();
  }

  void setOldPilotSkillLv() {
    for (int i = 0; i < 7; i++) {
      oldSkillLvList[i] = PilotSkillBase.GetSumLv( pilotInfo, i+1 );
    }
  }

  void setPilotSkills() {
    for (int i = 0; i < 7; i++) {
      setLine( "Skill", i+1, PilotSkillBase.GetNameWithLv( pilotInfo, i+1 ), PilotSkillBase.GetSumLv( pilotInfo, i+1 ) - oldSkillLvList[i] );
    }
  }

  void setOldSpComs() {  //原精神指令
    var spList = pilotInfo.PilotInstance.Pilot.SPComPilots;
    for (int i = 0; i < spList.Count; i++) {
      oldSpComList[i] = spList[i].Level > pilotInfo.Level ? 0 : 1;
    }
  }

  void setSpComs() {    //檢查精神指令是否新開放
    var spList = pilotInfo.PilotInstance.Pilot.SPComPilots;

    for (int i = 0; i < 7; i++) {
      string display;
      int up = 0;
      if (spList.Count <= i || spList[i] == null) {
        display = "------";
      }
      else if (spList[i].Level > pilotInfo.Level)
        display = "??????";
      else {
        display = spList[i].SPCommand.Name;
        up = 1 - oldSpComList[i];   //精神指令只有開關, 升級固定為1
      }
      setLine( "Sp", i + 1, display, up );
    }
  }

  void setOldSts() {
    oldStsList[0] = pilotInfo.MaxSp;
    oldStsList[1] = pilotInfo.Melee;
    oldStsList[2] = pilotInfo.Shoot;
    oldStsList[3] = pilotInfo.Defense;
    oldStsList[4] = pilotInfo.Dex;
    oldStsList[5] = pilotInfo.Dodge;
    oldStsList[6] = pilotInfo.Hit;
  }

  void setSts() {
    setLine( "Sts", 1, pilotInfo.MaxSp.ToString(), pilotInfo.MaxSp - oldStsList[0] );
    setLine( "Sts", 2, pilotInfo.Melee.ToString(), pilotInfo.Melee - oldStsList[1] );
    setLine( "Sts", 3, pilotInfo.Shoot.ToString(), pilotInfo.Shoot - oldStsList[2] );
    setLine( "Sts", 4, pilotInfo.Defense.ToString(), pilotInfo.Defense - oldStsList[3] );
    setLine( "Sts", 5, pilotInfo.Dex.ToString(), pilotInfo.Dex - oldStsList[4] );
    setLine( "Sts", 6, pilotInfo.Dodge.ToString(), pilotInfo.Dodge - oldStsList[5] );
    setLine( "Sts", 7, pilotInfo.Hit.ToString(), pilotInfo.Hit - oldStsList[6] );
  }

  void setLine( string blockName, int lineNo, string display, int up ) {
    var lineTf = transform.Find( $"{blockName}Block/{blockName}{lineNo}Line" );

    var uText = lineTf.Find( $"{blockName}{lineNo}" ).GetComponent<Text>();
    uText.text = display;
    uText.color = Color.white;

    lineTf.Find( $"{blockName}{lineNo}_UpArrow" ).gameObject.SetActive( up > 0 );
    var upIntTf = lineTf.Find( $"{blockName}{lineNo}_UpInt" );
    upIntTf?.gameObject.SetActive( up > 0 );
    if (up > 0) {
      uText.color = Color.yellow;
      if (upIntTf) {
        upIntTf.GetComponent<Text>().text = up.ToString();
      }
    }
  }

}
