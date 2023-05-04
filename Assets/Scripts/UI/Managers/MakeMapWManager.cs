using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AttackData;

public class MakeMapWManager : MonoBehaviour {

  private UnitInfo fromUnitInfo;
  //private WeaponInfo fromWeapon;

  //private Action backToCaller;

  private List<AttackData> attDataList;

  private MapManager mapManager;

  // Use this for initialization
  void Start () {

  }
	
	// Update is called once per frame

	void Update () {
    if (Input.GetButtonDown( "Confirm" )) {
      PlayDamageOnMapByRangeList(index+1);
    }
  }

  public void Setup( UnitInfo fromUnitInfo, WeaponInfo fromWeapon, List<GameObject> inRangeUnits, Action backToCaller ) {
    enabled = false;
    this.fromUnitInfo = fromUnitInfo;
    //this.fromWeapon = fromWeapon;
    //this.backToCaller = backToCaller;
    this.attDataList = new List<AttackData>();
    index = -1;
    done = false;
    mapManager = GameObject.Find( "MapManager" ).GetComponent<MapManager>();

    foreach (var unitGo in inRangeUnits) {
      var attData = new AttackData( fromUnitInfo, fromWeapon, unitGo.GetComponent<UnitInfo>(), true, AttackTypeEnum.MapW, CounterTypeEnum.Unable );
      attData.RunResult();
      attData.AfterData();
      attDataList.Add( attData );
    }

    fromUnitInfo.PilotInfo.ActiveSPCommandList.RemoveAll( s => s.Period == SPCommand.PeriodEnum.Attack );

    if (fromWeapon.MaxBullets > 0 && fromWeapon.RemainBullets > 0) {
      fromWeapon.RemainBullets--;
    }
    fromUnitInfo.RobotInfo.EN -= fromWeapon.EN;

    mapManager.MoveCamToCenterObject( fromUnitInfo.gameObject );

    string modelName = fromUnitInfo.RobotInfo.RobotInstance.Robot.Name;
    IUnitPlayWeapon unitPlay = (IUnitPlayWeapon)fromUnitInfo.transform.Find($"Renderer/{modelName}(Clone)").GetComponent( "Play_" + modelName );
    unitPlay.SetupWeapon();
    unitPlay.UnitInfo = fromUnitInfo;

    CoroutineCommon.CallWaitForSeconds( 0.0f, () => {
      unitPlay.PlayWeapon( fromWeapon, () => {
        PlayDamageOnMapByRangeList(0);
      } );
    } );
    
  }

  //public GameObject m_MapDamageCanvasPrefab;
  private GameObject mapDamageCanvasGo;
  private int index;
  private bool done;

  private void PlayDamageOnMapByRangeList( int i ) {
    if (i>index) {
      index = i;
    }
    else {
      return;
    }

    enabled = false;
    Destroy( mapDamageCanvasGo );

    if (attDataList.Count <= i) {
      if (!done) done = true;
      else return;

      //fromUnitInfo.GetComponent<UnitController>().EndAction();
      mapManager.BackToMap( true );
      Destroy( gameObject );
      GameObject.Find( "StageManager" ).GetComponent<StageManager>().AfterMapW( attDataList );
      fromUnitInfo.GetComponent<UnitController>().EndAction();
      return;
    }

    CoroutineCommon.CallWaitForSeconds( 1.2f, () => {    
      AttackData attData = attDataList[i];

      Camera.main.GetComponent<MainCamera>().MoveCamToCenterObject( attData.ToUnitInfo.transform );

      showDamageCanvas( attData );

      CoroutineCommon.CallWaitForSeconds( 1f, () => { enabled = true; } );

      //CoroutineCommon.CallWaitForSeconds( 10f, () => {
        //PlayDamageOnMapByRangeList( i+1 );
      //} );

    } );

  }


  private Text damageTxt;
  private Text hpTxt;
  private Slider hpSlider;
  private int beforeHp;
  private int unitMaxHp;
  private Text battleInfoTxt1;
  private Text battleInfoTxt2;
  private Text battleInfoTxt3;
  private Text battleInfoTxt4;
  private Text battleInfoTxt5;

  private void showDamageCanvas( AttackData attData ) {
    var mapDamageCanvasPrefab = Resources.Load( "StageMap/MapDamageCanvas" ) as GameObject;
    mapDamageCanvasGo = Instantiate( mapDamageCanvasPrefab );
    var canvasRect = mapDamageCanvasGo.GetComponent<RectTransform>();
    damageTxt = mapDamageCanvasGo.transform.Find( "Panel/DamageTxt" ).GetComponent<Text>();
    hpTxt = mapDamageCanvasGo.transform.Find( "Panel/HpTxt" ).GetComponent<Text>();
    hpSlider = mapDamageCanvasGo.transform.Find( "Panel/HpSlider" ).GetComponent<Slider>();
    beforeHp = attData.ToHP;
    unitMaxHp = attData.ToUnitInfo.RobotInfo.MaxHP;
    battleInfoTxt1 = mapDamageCanvasGo.transform.Find( "Panel/BattleInfoTxt1" ).GetComponent<Text>();
    battleInfoTxt2 = mapDamageCanvasGo.transform.Find( "Panel/BattleInfoTxt2" ).GetComponent<Text>();
    battleInfoTxt3 = mapDamageCanvasGo.transform.Find( "Panel/BattleInfoTxt3" ).GetComponent<Text>();
    battleInfoTxt4 = mapDamageCanvasGo.transform.Find( "Panel/BattleInfoTxt4" ).GetComponent<Text>();
    battleInfoTxt5 = mapDamageCanvasGo.transform.Find( "Panel/BattleInfoTxt5" ).GetComponent<Text>();
    battleInfoTxt1.text = "";
    battleInfoTxt2.text = "";
    battleInfoTxt3.text = "";
    battleInfoTxt4.text = "";
    battleInfoTxt5.text = "";

    RectTransform uiRect = mapDamageCanvasGo.transform.Find( "Panel" ).GetComponent<RectTransform>();

    //Vector2 previewPos = new Vector2( 0.75f, 0.5f );

    uiRect.anchoredPosition = new Vector2( 0.25f * canvasRect.sizeDelta.x, 0 );

    if (attData.IsDodge) {
      EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Weapon/MISS" ) );
      damageTxt.text = "MISS !!";
      playShake( attData.ToUnitInfo );
    }
    else if (attData.TotalDamage > 0) {  //傷害有效
      playGetHit( attData.ToUnitInfo, 1 );
      playShake( attData.ToUnitInfo );

      EffectSoundController.PLAY_1( (AudioClip)Resources.Load( "SFX/Weapon/BeHit" ), 5 );   //播放被彈音效
      damageTxt.text = $"-{attData.TotalDamage}";
    }
    else {
      //命中但傷害為 0, 播放防護罩音效
    }

    StartCoroutine( playDamage( attData.TotalDamage, 1f ) );
    attData.ToUnitInfo.GetComponent<UnitHealth>().SetHealthUI();
    //CoroutineCommon.CallWaitForSeconds( 2f, () => { Destroy( mapDamageCanvasGo ); } );
  }

  private IEnumerator playDamage( int damage, float duration ) {
    int resultHp;
    for (float timer = 0; timer < duration; timer += Time.deltaTime) {
      float progress = timer / duration;
      var currentDamage = (int)Mathf.Lerp( 0, damage, progress );
      resultHp = beforeHp - currentDamage;
      hpTxt.text = (resultHp >= 0 ? resultHp : 0) + " / " + unitMaxHp;
      hpSlider.value = (float)resultHp / unitMaxHp;
      yield return null;
    }
    resultHp = beforeHp - damage;
    hpTxt.text = (resultHp >= 0 ? resultHp : 0) + " / " + unitMaxHp;
    hpSlider.value = (float)resultHp / unitMaxHp;
  }

  private void playGetHit( UnitInfo toUnitInfo, int speed ) {
    var opposAnimation = toUnitInfo.transform.Find($"Renderer/{toUnitInfo.RobotInfo.RobotInstance.Robot.Name}(Clone)").GetComponent<Animation>();

    opposAnimation["getHit"].speed = 1;
    opposAnimation["getHit"].time = 0; //speed > 0 ? 0 : opposAnimation["getHit"].length;
    opposAnimation.Play( "getHit" );

    CoroutineCommon.CallWaitForSeconds( 1f, () => {
      opposAnimation["getHit"].speed = -1;
      opposAnimation["getHit"].time = opposAnimation["getHit"].length;
      opposAnimation.Play( "getHit" );
    } );
  }

  protected void playShake( UnitInfo toUnitInfo ) {
    var shaker = toUnitInfo.GetComponent<MyShake>();

    shaker.enabled = true;
    
    CoroutineCommon.CallWaitForSeconds( 1f, () => {
      shaker.enabled = false;
    } );
  }


}
