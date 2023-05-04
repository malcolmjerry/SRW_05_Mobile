using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DamageController : MonoBehaviour {

  [HideInInspector]public GameObject StatusRectL;
  [HideInInspector]public GameObject StatusRectR;
  [HideInInspector] public GameObject WorkingStatusRect;

  Text hp_L, en_L, hp_R, en_R, workingHpTxt, workingEnTxt;
  Text damageTxt;

  Slider hpSilder_L, enSilder_L, hpSilder_R, enSilder_R, workingHpSilder, workingEnSilder;

  int rightFullHp, rightHp, leftFullHp, leftHp, workingFullHp, workingHp;
  int rightFullEn, rightEn, leftFullEn, leftEn, workingFullEn, workingEn;

  //string sideDir;

  //string pilotPicR, pilotPicL;

	// Use this for initialization
	void Start () {
    StatusRectL = transform.Find( "HpStatusL" ).gameObject;
    StatusRectR = transform.Find( "HpStatusR" ).gameObject;
    hp_L = StatusRectL.transform.Find( "HpTxtL" ).GetComponent<Text>();
    hp_R = StatusRectR.transform.Find( "HpTxtR" ).GetComponent<Text>();
    en_L = StatusRectL.transform.Find( "EnTxtL" ).GetComponent<Text>();
    en_R = StatusRectR.transform.Find( "EnTxtR" ).GetComponent<Text>();
    hpSilder_L = StatusRectL.transform.Find( "HpSliderL" ).GetComponent<Slider>();
    hpSilder_R = StatusRectR.transform.Find( "HpSliderR" ).GetComponent<Slider>();
    enSilder_L = StatusRectL.transform.Find( "EnSliderL" ).GetComponent<Slider>();
    enSilder_R = StatusRectR.transform.Find( "EnSliderR" ).GetComponent<Slider>();
    damageTxt = transform.Find( "DamageTxt" ).GetComponent<Text>();
  }
	
	// Update is called once per frame
	void Update () {
	
	}
  /*
  public void Setup( int rightFullHp, int rightHp, int leftFullHp, int leftHp,
                     int rightFullEn, int rightEn, int leftFullEn, int leftEn ) { 
                     //string pilotPicR, string pilotPicL ) {
    this.rightFullHp = rightFullHp;
    this.rightHp = rightHp;
    this.leftFullHp = leftFullHp;
    this.leftHp = leftHp;
    this.rightFullEn = rightFullEn;
    this.rightEn = rightEn;
    this.leftFullEn = leftFullEn;
    this.leftEn = leftEn;
    //this.pilotPicR = pilotPicR;
    //this.pilotPicL = pilotPicL;
  }*/

  public void SetupFromRight( AttackData attData ) {
    this.rightFullHp = attData.FromUnitInfo.RobotInfo.MaxHP;
    //this.rightHp = attData.FromUnitInfo.RobotInfo.HP;
    this.rightHp = attData.FromHP;
    this.leftFullHp = attData.ToUnitInfo.RobotInfo.MaxHP;
    //this.leftHp = attData.ToUnitInfo.RobotInfo.HP;
    this.leftHp = attData.ToHP;

    this.rightFullEn = attData.FromUnitInfo.RobotInfo.MaxEN;
    //this.rightEn = attData.FromUnitInfo.RobotInfo.EN;
    this.rightEn = attData.FromEN;
    this.leftFullEn = attData.ToUnitInfo.RobotInfo.MaxEN;
    //this.leftEn = attData.ToUnitInfo.RobotInfo.EN;
    this.leftEn = attData.ToEN;
    clearActiveSkills();
  }

  public void SetupFromLeft( AttackData attData ) {
    this.leftFullHp = attData.FromUnitInfo.RobotInfo.MaxHP;
    //this.leftHp = attData.FromUnitInfo.RobotInfo.HP;
    this.leftHp = attData.FromHP;
    this.rightFullHp = attData.ToUnitInfo.RobotInfo.MaxHP;
    //this.rightHp = attData.ToUnitInfo.RobotInfo.HP;
    this.rightHp = attData.ToHP;

    this.leftFullEn = attData.FromUnitInfo.RobotInfo.MaxEN;
    //this.leftEn = attData.FromUnitInfo.RobotInfo.EN;
    this.leftEn = attData.FromEN;
    this.rightFullEn = attData.ToUnitInfo.RobotInfo.MaxEN;
    //this.rightEn = attData.ToUnitInfo.RobotInfo.EN;
    this.rightEn = attData.ToEN;

    clearActiveSkills();
  }

  private void clearActiveSkills() {
    foreach (Transform bc in transform) {
      if (!bc.name.StartsWith( "ActiveSkillText" ))
        continue;
      bc.GetComponent<Text>().text = "";
    }
  }

  public void SetSide( string sideDir, bool active, bool oposActive = false ) {
    //this.sideDir = sideDir;
    if (sideDir == "R") {
      StatusRectL.SetActive( oposActive );
      StatusRectR.SetActive( active );
      WorkingStatusRect = StatusRectR;
      workingFullHp = rightFullHp;
      workingHp = rightHp;
      workingFullEn = rightFullEn;
      workingEn = rightEn;
      workingHpSilder = hpSilder_R;
      workingEnSilder = enSilder_R;
      workingHpTxt = hp_R;
      workingEnTxt = en_R;

    } else if (sideDir == "L") {
      StatusRectL.SetActive( active );
      StatusRectR.SetActive( oposActive );
      WorkingStatusRect = StatusRectL;
      workingFullHp = leftFullHp;
      workingHp = leftHp;
      workingFullEn = leftFullEn;
      workingEn = leftEn;
      workingHpSilder = hpSilder_L;
      workingEnSilder = enSilder_L;
      workingHpTxt = hp_L;
      workingEnTxt = en_L;
    }
    workingHpTxt.text = (workingHp >= 0 ? workingHp : 0) + " / " + workingFullHp;
    workingEnTxt.text = (workingEn >= 0 ? workingEn : 0) + " / " + workingFullEn;
    workingHpSilder.value = (float)workingHp / workingFullHp;
    workingEnSilder.value = (float)workingEn / workingFullEn;
  }

  int lastDamage;
  int currentDamage;
  public void Play( float duration, int lastDamage, int endDamage ) {
    WorkingStatusRect.SetActive( true );
    this.lastDamage = lastDamage;
    transform.Find( "DamageTxt" ).gameObject.SetActive( true );

    //gameObject.SetActive( true );
    StartCoroutine( CountTo( endDamage, duration ) );
  }

  IEnumerator CountTo( int endDamage, float duration ) {
    int resultHp;
    for (float timer = 0; timer < duration; timer += Time.deltaTime) {
      float progress = timer / duration;
      //currentDamage = (int)Mathf.Lerp( lastDamage, endDamage, progress );
      currentDamage = (int)Mathf.Lerp( 0, endDamage - lastDamage, progress );
      //resultHp = workingHp - currentDamage + lastDamage;
      resultHp = workingHp - currentDamage;
      workingHpTxt.text = (resultHp >= 0 ? resultHp : 0) + " / " + workingFullHp;
      workingHpSilder.value = (float)resultHp / workingFullHp;

      damageTxt.text = lastDamage + currentDamage + "";
      yield return null;
    }
    resultHp = workingHp - endDamage + lastDamage;
    workingHpTxt.text = (resultHp >= 0 ? resultHp : 0) + " / " + workingFullHp;
    workingHp = resultHp;
    workingHpSilder.value = (float)workingHp / workingFullHp;
    damageTxt.text = endDamage.ToString();
    //saveSideHpAndEn();
  }

  int currentEn;
  public void PlayEn( int spendEn, float duration = .5f ) {
    WorkingStatusRect.SetActive( true );
    //transform.Find( "DamageTxt" ).gameObject.SetActive( false );
    //clearActiveSkills();
    StartCoroutine( CountToEn( spendEn, duration ) );
  }

  IEnumerator CountToEn( int spendEn, float duration ) {
    int resultEn;
    for (float timer = 0; timer < duration; timer += Time.deltaTime) {
      float progress = timer / duration;
      currentEn = (int)Mathf.Lerp( 0, spendEn, progress );
      resultEn = workingEn - currentEn;
      workingEnTxt.text = (resultEn >= 0 ? resultEn : 0) + " / " + workingFullEn;
      workingEnSilder.value = (float)resultEn / workingFullEn;
      yield return null;
    }
    resultEn = workingEn - spendEn;
    workingEnTxt.text = (resultEn >= 0 ? resultEn : 0) + " / " + workingFullEn;
    workingEn = resultEn;
    workingEnSilder.value = (float)workingEn / workingFullEn;
    //saveSideHpAndEn();
  }

  public void clear() {
    transform.Find( "DamageTxt" ).gameObject.SetActive( false );
    clearActiveSkills();
  }

  /*
  private void saveSideHpAndEn() {
    if (sideDir == "R") {
      rightHp = workingHp;
      rightEn = workingEn;

    } else if (sideDir == "L") {
      leftHp = workingHp;
      leftEn = workingEn;
    }
  }
  */
}
