using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using UnityEditor;
using UnityEngine.UI;
using static AttackData;
using static MachineGunShoot;

public class UnitPlayWeaponBase : MonoBehaviour, IUnitPlayWeapon {

  //public Vector3 RightPos = new Vector3( 0, 0, -20 );
  //public Quaternion RightRotation = Quaternion.Euler( 0, -30, 0 );
  //public Vector3 LeftPos = new Vector3( 0, 0, 20 );
  //public Quaternion LeftRotation = Quaternion.Euler( 0, -150, 0 );
  public virtual float middleZ { get; set; } = 7;

  private Vector3 originPos;
  private Quaternion originRot;
  private Vector3 opposOriginPos;
  private Quaternion opposOriginRot;

  public float m_Speed = 12f;

  protected Transform fightArea;
  protected Transform modelSide;
  protected Animator animator;
  protected Animation myAnimation;

  protected Transform opposModelSide;
  protected GameObject opposModelGO;
  protected Animator opposAnimator;
  protected Animation opposAnimation;
  protected IUnitPlayWeapon opposPlay;

  protected GameObject camRight;
  protected GameObject camLeft;

  protected string sideDir;
  protected string opposSideDir;

  protected Vector3 middlePoint;

  protected GameObject geroBeamPF;
  protected GameObject geroBeamGO;
  protected GameObject geroBeamGO_2;

  protected Action callBack;

  private float backToTime = 1.5f;

  protected DamageController damageController;

  private GameObject m_ExplosionPrefab;
  private AudioSource m_ExplosionAudio;
  private ParticleSystem m_ExplosionParticles;

  protected bool hitMiss;

  public UnitInfo UnitInfo { get; set; }
  //public UnitInfo unitInfo;
  protected UnitInfo opposUnitInfo;
  //protected Image pilotImage;
  protected Transform battleCanvas;

  protected AttackData attData;

  //private bool isVoidMotion = false;
  //private bool isHitMotion = false;
  private string motionName = null;
  private string backMotionName = null;
  //private BackTypeEnum backType { set; get; }
  //public enum BackTypeEnum { Void }

  protected Camera tempCam;
  protected bool tempCamRotation = false;
  protected Vector3 startPos;
  protected Quaternion startRotation;
  protected Vector3 endPos;
  protected Quaternion endRotation;
  protected float startTime;
  protected float fractionLen;

  void Start() {

  }

  protected virtual void Update() {
    if (tempCam) {
      float distCovered = (Time.time - startTime) * 1;
      float fraction = distCovered / fractionLen;
      tempCam.transform.localPosition = Vector3.Lerp(startPos, endPos, fraction);

      if (tempCamRotation) {
        tempCam.transform.localRotation = Quaternion.Lerp( startRotation, endRotation, fraction );
      }
    }
  }

  protected void Awake() {
    //m_ExplosionPrefab = AssetDatabase.LoadAssetAtPath<GameObject>( "Assets/Prefabs/TankExplosion.prefab" );
    m_ExplosionPrefab = Resources.Load( "TankExplosion" ) as GameObject;
    //geroBeamPF = (GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath( "Assets/BasicBeamShot/" + "GeroBeam.prefab", typeof( GameObject ) );
    geroBeamPF = Resources.Load( "BasicBeamShot/GeroBeam" ) as GameObject;

    myAnimation = GetComponent<Animation>();
  }

  public void Setup( Transform fightArea, Transform modelSide, Transform opposModelSide, GameObject opposModelGO, string sideDir, GameObject camRight, GameObject camLeft,
                     Vector3 originPos, Quaternion originRot, DamageController damageController, Transform battleCanvas,
                     UnitInfo unitInfo, UnitInfo opposUnitInfo,
                     IUnitPlayWeapon opposPlay/*, List<string> dialogueList, List<string> opposDialogueList*/ ) {
    this.fightArea = fightArea;
    this.modelSide = modelSide;
    this.opposModelSide = opposModelSide;
    this.opposPlay = opposPlay;
    this.sideDir = sideDir;

    this.opposSideDir = sideDir == "R" ? "L" : "R";
    animator = modelSide.GetComponent<Animator>();
    animator.enabled = false;

    this.opposModelGO = opposModelGO;
    opposAnimator = opposModelSide.GetComponent<Animator>();
    opposAnimation = opposModelGO.GetComponent<Animation>();
    opposAnimator.enabled = false;

    this.camRight = camRight;
    this.camLeft = camLeft;
    this.originPos = originPos;
    this.originRot = originRot;

    this.opposOriginPos = opposModelSide.localPosition;
    this.opposOriginRot = opposModelSide.localRotation;

    this.damageController = damageController;

    this.UnitInfo = unitInfo;
    this.opposUnitInfo = opposUnitInfo;
    //this.pilotImage = pilotImage;
    this.battleCanvas = battleCanvas;
    //this.dialogueList = dialogueList;
    //this.opposDialogueList = opposDialogueList;

    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = "";
    battleCanvas.Find( "Talking1" ).GetComponent<Text>().text = "";
    battleCanvas.Find( "Talking2" ).GetComponent<Text>().text = "";
    battleCanvas.Find( "Talking3" ).GetComponent<Text>().text = "";

    SetupWeapon();
    middlePoint = new Vector3( 0, 0, sideDir == "R" ? middleZ : -middleZ );
  }

  public virtual void SetupWeapon() { }
  public virtual Transform GetTarget() { return transform.Find( "Target" ); }

  public void PlayWeapon( AttackData attData, Action callBack ) {
    this.callBack = callBack;
    this.attData = attData;
    motionName = null;
    backMotionName = null;

    //var myType = this.GetType();
    var method = this.GetType().GetMethod( "playWeapon" + attData.WeaponInfo.WeaponInstance.Weapon.PlayIndex );
    var arguments = new object[] {};

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    CoroutineCommon.CallWaitForSeconds( .6f, () => method.Invoke( this, arguments ) );
    //method.Invoke( this, arguments );

  }

  public void PlayWeapon( WeaponInfo wpInfo, Action callBack ) {
    this.callBack = callBack;
    //this.attData = attData;
    motionName = null;
    backMotionName = null;

    var method = this.GetType().GetMethod( "playWeapon" + wpInfo.WeaponInstance.Weapon.PlayIndex );
    var arguments = new object[] { };
    //method.Invoke( this, arguments );
    CoroutineCommon.CallWaitForSeconds( .6f, () => method.Invoke( this, arguments ) );
  }

  public void PlayWeapon( int i, int totalDamge, int spendEn, bool hitMiss, List<PilotDialog> pdList, List<PilotDialog> opposPdList, Action callBack ) {
    this.callBack = callBack;
    this.hitMiss = hitMiss;
    if (i < 0) {
      m_ExplosionParticles = Instantiate( m_ExplosionPrefab ).GetComponent<ParticleSystem>();
      m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
      //m_ExplosionParticles.gameObject.SetActive( false );

      m_ExplosionParticles.transform.position = transform.position;
      m_ExplosionParticles.gameObject.SetActive( true );
      m_ExplosionParticles.Play();
      EffectSoundController.PLAY( m_ExplosionAudio.clip, 10 );
      gameObject.SetActive( false );

      Destroy( m_ExplosionParticles, 2 );

      CoroutineCommon.CallWaitForSeconds( 1f, () => {
        callBack();
      } );
    }
    else {
      var method = this.GetType().GetMethod( "playWeapon" + i );
      var arguments = new object[] { totalDamge, spendEn, pdList, opposPdList };
      method.Invoke( this, arguments );
    }
  }

  public void PlayDefeat( Action callBack ) {
    m_ExplosionParticles = Instantiate( m_ExplosionPrefab ).GetComponent<ParticleSystem>();
    m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
    //m_ExplosionParticles.gameObject.SetActive( false );

    m_ExplosionParticles.transform.position = transform.position;
    m_ExplosionParticles.gameObject.SetActive( true );
    m_ExplosionParticles.Play();
    EffectSoundController.PLAY( m_ExplosionAudio.clip, 10 );
    gameObject.SetActive( false );

    Destroy( m_ExplosionParticles, 2 );

    CoroutineCommon.CallWaitForSeconds( 1f, () => {
      callBack();
    } );
  }

  public void PlayUnable( List<PilotDialog> pdList, Action callBack ) {
    this.callBack = callBack;

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( pdList );
    CoroutineCommon.CallWaitForSeconds( 1f, () => {
      callBack();
    } );
  }

  protected IEnumerator BackToOrignPos( Vector3 middlePoint, Action callback ) {
    float startTime = Time.time;
    var startPos = modelSide.localPosition;

    while (Time.time < startTime + backToTime) {
      var vector3Lerp = Vector3.Lerp( startPos, middlePoint, (Time.time - startTime) / backToTime );
      modelSide.localPosition = vector3Lerp; //Vector3.Lerp( startPos, middlePoint, (Time.time - startTime) / backToTime );
      yield return null;
    }

    modelSide.localPosition = originPos;
    modelSide.localRotation = originRot;

    callback();

    yield return null;
  }

  protected IEnumerator BackToOrignPos_Opp( Action callback ) {
    float startTime = Time.time;
    var startPos = opposModelSide.localPosition;

    while (Time.time < startTime + .8f) {
      opposModelSide.localPosition = Vector3.Lerp( startPos, opposOriginPos, (Time.time - startTime) / .8f );
      yield return null;
    }

    opposModelSide.localPosition = opposOriginPos;
    opposModelSide.localRotation = opposOriginRot;

    damageController.clear();
    callback();

    yield return null;
  }

  protected IEnumerator RandomAvoid( Action callback ) {
    float startTime = Time.time;
    var startPos = opposModelSide.localPosition;

    int avoidZ = sideDir == "R" ? 5 : -5;
    var targetPos = new Vector3( opposModelSide.localPosition.x + 5, opposModelSide.localPosition.y, opposModelSide.localPosition.z + avoidZ );
    
    while (Time.time < startTime + .5f) {
      //var a = Vector3.Lerp( startPos, targetPos, (Time.time - startTime) / 1f );
      opposModelSide.localPosition = Vector3.Lerp( startPos, targetPos, (Time.time - startTime) / .5f );
      yield return null;
    }

    callback?.Invoke();

    yield return null;
  }

  /*
  protected void playMeleeWeapon1() {
    //CoroutineCommon.CallWaitForSeconds( 2, () => {
    CoroutineCommon.CallWaitForFrames( 120, () => {
      animator.Play( "MeleeAttack_" + sideDir );

      CoroutineCommon.CallWaitForAnimator( animator, "MeleeAttack_" + sideDir, () => {
        camRight.SetActive( !camRight.activeSelf );
        camLeft.SetActive( !camLeft.activeSelf );

        animator.Play( "MeleeAttack2_" + sideDir );
        CoroutineCommon.CallWaitForAnimator( animator, "MeleeAttack2_" + sideDir, () => {
          animator.Stop();
          myAnimation.Play( "weapon1" );

          CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
            StartCoroutine( BackToOrignPos( middlePoint, () => {
              //myAnimation.Play( "weapon1" );
              callBack();
            } ) );
          } );

        } );

      } );

    } );
  }*/

  private void showActiveSkill() {
    for (int i=0; i<attData.ActiveSkillList.Count; i++) {
      var skillStr = attData.ActiveSkillList[i];
      var ast = battleCanvas.Find( $"ActiveSkillText{opposSideDir}{i+1}" );
      ast.GetComponent<Text>().text = skillStr;
      ast.gameObject.SetActive( true );
    }

  }

  protected void playGetHitCommon( float waitTime, bool needMoveBack, List<HitData> hitDataList, Action playGetHitCommonCallback ) {
    float totalTime = hitDataList.Sum( h => h.WaitTime ) + hitDataList.Sum( h => h.DelayTime );

    if (attData.CutType > 0 || attData.IsDodge) {
      waitTime = waitTime -  14f/25;
    }

    CoroutineCommon.CallWaitForSeconds( waitTime, () => {

      battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + opposUnitInfo.PilotInfo.PicNo + "_1" );
      battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = opposUnitInfo.PilotInfo.ShortName;
      setDialog( attData.DefDialogs );
      showActiveSkill();

      if (attData.CutType > 0) {    //切彿
        Vector3 tempSize = GetComponent<BoxCollider>().size;
        motionName = "ReadyCut";
        backMotionName = "CutEnd";
        EffectSoundController.PLAY_1( (AudioClip)Resources.Load( "SFX/Weapon/RobotWakeUp" ), 5 );
        playGetHitOrAvoid( 1, () => {
          GetComponent<BoxCollider>().size = tempSize * 100;
          playGetHitByAttacker( 1 );
          playShake( gameObject, new List<HitData>() { new HitData { WaitTime = 0f, DelayTime = 12f/25, Damage = 0 } }, () => {
            //playGetHitByAttaccker( -1 );
            GetComponent<BoxCollider>().size = tempSize;
            moveBack( needMoveBack, playGetHitCommonCallback );
          } );
        } );
      }
      else if (!attData.IsDodge) {  //命中
        GameObject barrierPf;
        if (attData.BarrierID > 0) {
          if ((new List<int> { 2, 3 }).Any( id => id == attData.BarrierID )) {
            //var barrierPos = opposModelGO.transform.Find( "BarrierPos" );
            barrierPf = Resources.Load( "Battle/Barriers/Barrier" ) as GameObject;
          }
          else {
            barrierPf = Resources.Load( "Battle/Barriers/Barrier" ) as GameObject;
          }

          var barrierPos = opposModelGO.transform.Find( "BarrierPos" );
          var barrierGo = Instantiate( barrierPf );
          barrierGo.transform.SetParent( barrierPos );
          barrierGo.transform.localPosition = new Vector3( 0, 0, 0 );
          barrierGo.transform.localEulerAngles = new Vector3( 0, 0, 0 );
          EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Weapon/Barrier" ), 4 );
          CoroutineCommon.CallWaitForSeconds( totalTime, () => {
            Destroy( barrierGo );
          } );
        }

        if (attData.TotalDamage > 0) {  //傷害有效
          //isHitMotion = true;
          motionName = (attData.IsShield || attData.CounterType == CounterTypeEnum.Defense) ? "defense" : "getHit";
          playGetHitOrAvoid( 1 );
          //EffectSoundController.PLAY_1( (AudioClip)Resources.Load( $"SFX/Weapon/{hitEffect?? "BeHit"}" ), 5 );   //播放被彈音效

          playShake( opposModelGO, hitDataList, () => {
            CoroutineCommon.CallWaitForSeconds( .5f, () => {
              moveBack( needMoveBack, playGetHitCommonCallback );
            } );
          } );
        }
        else {
          CoroutineCommon.CallWaitForSeconds( totalTime, () => {
            moveBack( needMoveBack, playGetHitCommonCallback );
          } );
        }
      }
      else {       //回避動作
        //isVoidMotion = true;
        motionName = "avoid";
        playGetHitOrAvoid( 1 );
        EffectSoundController.PLAY_1( (AudioClip)Resources.Load( "SFX/Weapon/RobotWakeUp" ), 3 );
        StartCoroutine( RandomAvoid( null ) );

        CoroutineCommon.CallWaitForSeconds( totalTime, () => {
          moveBack( needMoveBack, playGetHitCommonCallback );
        } );

        return;
      }
    } );

  }

  private void moveBack( bool needMoveBack, Action playGetHitCommonCallback ) {
    if (needMoveBack)
      StartCoroutine( BackToOrignPos( middlePoint, () => {
        playGetHitCommonCallback?.Invoke();
      } ) );
    else {
      modelSide.localPosition = originPos;
      modelSide.localRotation = originRot;
      playGetHitCommonCallback?.Invoke();
    }
  }

  private void playGetHitOrAvoid( int speed, Action callBack = null ) {     //By Common
    //string motionName = isVoidMotion ? "getHit" : (isHitMotion? "getHit" : null );

    if (motionName != null) {
      //EffectSoundController.PLAY_1( (AudioClip)Resources.Load( "SFX/Weapon/RobotWakeUp" ), 5 );
      try {
        opposAnimation[motionName].speed = speed;
        opposAnimation[motionName].time = speed > 0 ? 0 : opposAnimation[motionName].length;
        opposAnimation.Play( motionName );
        CoroutineCommon.CallWaitForAnimation( opposAnimation, () => { callBack?.Invoke(); } );
        return;
      }
      catch {}
    }

    callBack?.Invoke();
  }

  private void playGetHitByAttacker( int speed, Action callBack = null ) {     //By Common
    //EffectSoundController.PLAY_1( (AudioClip)Resources.Load( "SFX/Weapon/RobotWakeUp" ), 5 );
    try {
      myAnimation["getHit"].speed = speed;
      myAnimation["getHit"].time = speed > 0 ? 0 : myAnimation["getHit"].length;
      myAnimation.Play( "getHit" );
      CoroutineCommon.CallWaitForAnimation( opposAnimation, () => { callBack?.Invoke(); } );
    }
    catch { callBack?.Invoke(); }
  }

  private void backFromMotion( Action callBack = null ) {     //By Common
    //EffectSoundController.PLAY_1( (AudioClip)Resources.Load( "SFX/Weapon/RobotWakeUp" ), 5 );

    string myMotionName = backMotionName != null ? backMotionName : motionName;
    int speed = 1;
    float time = 0;
    if (backMotionName != null) {
      myMotionName = backMotionName;
    }
    else if (motionName != null) {
      myMotionName = motionName;
      speed = -1;
      time = opposAnimation[myMotionName].length;
    }
    else {
      callBack?.Invoke();
      return;
    }

    opposAnimation[myMotionName].speed = speed;
    opposAnimation[myMotionName].time = time;
    opposAnimation.Play( myMotionName );
    CoroutineCommon.CallWaitForAnimation( opposAnimation, () => { callBack?.Invoke(); } );
    return;
  }

  protected void switchCam() {
    camRight.SetActive( !camRight.activeSelf );
    camLeft.SetActive( !camLeft.activeSelf );
    enabled = false;

    foreach (var missile in missileGoList)
      Destroy( missile );
  }

  protected GameObject getCurrentCam() {
    if (camRight.activeSelf)
      return camRight;
    else
      return camLeft;
  }

  /*
  protected void playGetHit( int speed, float waitTime, Action callBack = null) {
    CoroutineCommon.CallWaitForSeconds( waitTime, () => {

      battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + opposUnitInfo.PilotInfo.PicNo + "_1" );
      battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = opposUnitInfo.PilotInfo.ShortName;
      setDialog( attData.DefDialogs[0].Text );

      opposAnimation["getHit"].speed = speed;
      opposAnimation["getHit"].time = speed > 0 ? 0 : opposAnimation["getHit"].length;
      opposAnimation.Play( "getHit" );

      if (!attData.IsDodge) {
        //battleCanvas.Find( "Talking1" ).GetComponent<Text>().text = opposDialogueList[1];
        
        //opposAnimation["getHit"].speed = speed;
        //opposAnimation["getHit"].time = speed > 0 ? 0 : opposAnimation["getHit"].length;
        //opposAnimation.Play( "getHit" );
        
        if (speed == 1) EffectSoundController.PLAY_1( (AudioClip)Resources.Load( "SFX/Weapon/BeHit" ), 10 );
      } else {
        //battleCanvas.Find( "Talking1" ).GetComponent<Text>().text = opposDialogueList[2];   
        
        //opposAnimation["getHit"].speed = speed;
        //opposAnimation["getHit"].time = speed > 0 ? 0 : opposAnimation["getHit"].length;
        //opposAnimation.Play( "getHit" );
        
        EffectSoundController.PLAY_1( (AudioClip)Resources.Load( "SFX/Weapon/RobotWakeUp" ), 5 );
        if (speed == 1) {
          StartCoroutine( RandomAvoid( callBack ) );
          return;
        } 
        
        //opposAnimator.Play( "Avoid_" + opposSideDir );
        //CoroutineCommon.CallWaitForAnimator( opposAnimator, "Avoid_" + opposSideDir, () => {
          //opposAnimator.Stop();
        //});
      }
      callBack?.Invoke();
    } );
  }
  */

  MyShake shaker;
  List<HitData> hitDataList;
  //int index;
  Action hitCallBack;

  protected void playShake( /*float shakeAmount,*/ GameObject modelGo, List<HitData> hitDataList, Action hitCallBack ) {
    this.hitDataList = hitDataList;
    this.hitCallBack = hitCallBack;
    shaker = modelGo.GetComponent<MyShake>();
    ///shaker = opposModelGO.GetComponent<MyShake>();
    //shaker.Setup( shakeAmount );
    //index = 0;
    //DoPlayShake( 0 );

    DoPlayShake( 0, 0 );
  }

  private void DoPlayShake( int index, int startDam ) {
    if (index >= hitDataList.Count) {
      hitCallBack();
      return;
    }

    HitData hitData = hitDataList[index];
    CoroutineCommon.CallWaitForSeconds( hitData.WaitTime, () => {
      //if (isHitMotion) {
        shaker.enabled = true;
        EffectSoundController.PLAY_1( (AudioClip)Resources.Load( $"SFX/Weapon/{hitData.HitEffect?? "BeHit"}" ), 3 );   //播放被彈音效
        damageController.Play( hitData.DelayTime, startDam, hitData.Damage );
      //}

      CoroutineCommon.CallWaitForSeconds( hitData.DelayTime, () => {
        shaker.enabled = false;
        DoPlayShake( index + 1, hitData.Damage );
      } );

    } );
  }

  protected void ShootBeam( Vector3 fromPos, Color beamColor, float scale, 
                            /*float shakeAmount,*/ List<HitData> hitDataList, float deadTime = 0, string sfx = "MegaBeam" ) {
    var midPos = new Vector3( fromPos.x, fromPos.y, middlePoint.z );

    var geroBeamGO_to = Instantiate( geroBeamPF ) as GameObject;
    var beamParam = geroBeamGO_to.GetComponent<BeamParam>();
    beamParam.BeamColor = beamColor;
    beamParam.Scale = scale;

    geroBeamGO_to.transform.SetParent( fightArea );
    geroBeamGO_to.transform.localPosition = midPos;
    //geroBeamGO.transform.LookAt( opposModelGO.transform.Find( "Target" ) );
    geroBeamGO_to.transform.LookAt( opposPlay.GetTarget() );
    //geroBeamGO.transform.localRotation = Quaternion.Euler( new Vector3( 0, 0, 0 ) );
    geroBeamGO_to.SetActive( true );

    EffectSoundController.PLAY( (AudioClip)Resources.Load( $"SFX/Weapon/{sfx}" ), 2 );

    if (hitDataList != null && hitDataList.Count > 0) {
      playGetHitCommon( hitDataList[0].WaitTime, false, hitDataList,
        () => {
          geroBeamGO_to.GetComponent<GeroBeam>().SmoothDestroy( () => {
            geroBeamGO_to.SetActive( false );
            BaseCallback();
          } );
        } );
    }
    else if (deadTime > 0) {
      CoroutineCommon.CallWaitForSeconds( 0.2f + deadTime,
        () => {
          geroBeamGO_to.GetComponent<GeroBeam>().SmoothDestroy( () => {
            geroBeamGO_to.SetActive( false );
          } );
      } );
    }

  }

  protected void BaseCallback() {  //對方返回原點, 接着callback到 fightAnimController, 繼續下一輪攻擊
    fightArea.GetComponent<FightAnimController>().Stop = false;

    if (motionName == "getHit" || motionName == "avoid") {
      playGetHitOrAvoid( -1, () => { } );
      StartCoroutine( BackToOrignPos_Opp( () => {
        callBack();
      } ) );
    }
    else { //if (motionName == "ReadyCut") {
      backFromMotion( () => {
        StartCoroutine( BackToOrignPos_Opp( () => { callBack(); } ) );
      } );
    }
    /*
    if (hitMiss) {
      callBack();
    } else {
      //如 Miss 時有回避動作, 先返回原始位置
      StartCoroutine( BackToOrignPos_Opp( () => {
        callBack();
      } ) );
    }
    */
  }

  protected void setDialog( List<PilotDialog> dialogs ) {

    string lines = dialogs[0].Text;
    EffectSoundController.PLAY_1( dialogs[0].ID, 5 );

    string[] lineArr = lines.Split( '\n' ); 
    battleCanvas.Find( "Talking1" ).GetComponent<Text>().text = lineArr.Length > 0? lineArr[0] : "";//dialogueList[0];
    battleCanvas.Find( "Talking2" ).GetComponent<Text>().text = lineArr.Length > 1 ? lineArr[1] : "";//dialogueList[0];
    battleCanvas.Find( "Talking3" ).GetComponent<Text>().text = lineArr.Length > 2 ? lineArr[2] : "";//dialogueList[0];
  }

  protected GameObject playBigMachineGunFrom( Transform gunPos ) {
    var gunPrefab = Resources.Load( "Battle/Weapons/MachineGun" ) as GameObject;
    var gunGo = Instantiate( gunPrefab );
    gunGo.transform.SetParent( gunPos );
    gunGo.transform.localPosition = new Vector3( 0, 0, 0 );
    //gunGo.transform.localRotation = Quaternion.Euler( new Vector3( 0, 0, 0 ) );
	  gunGo.transform.localEulerAngles = gunPrefab.transform.localEulerAngles;

    gunGo.GetComponent<MachineGunShoot>().enabled = true;
    return gunGo;
  }

  List<GameObject> mgGoList;
  protected void playMachineGunFrom( List<Transform> mgList, Vector3? direction = null, string BulletPrefabName = null ) {
    mgGoList = new List<GameObject>();
    var gunPrefab = Resources.Load( $"Battle/Weapons/MachineGunPoint" ) as GameObject;
    bool playSound = true;
    foreach (var mg in mgList) {
      var gunPointGo = Instantiate( gunPrefab );
      gunPointGo.transform.SetParent( mg );
      gunPointGo.transform.localPosition = new Vector3( 0, 0, 0 );
      gunPointGo.transform.localEulerAngles = gunPrefab.transform.localEulerAngles;

      gunPointGo.GetComponent<MachineGunPointShoot>().enabled = true;
      gunPointGo.GetComponent<MachineGunPointShoot>().playSound = playSound;
      gunPointGo.GetComponent<MachineGunPointShoot>().Setup( direction, BulletPrefabName );

      playSound = !playSound;
      mgGoList.Add( gunPointGo );
    }
  }
  protected void stopMachineGunFrom() {
    foreach (var mgGo in mgGoList) Destroy( mgGo );
  }


  protected void playBigMachineGunTo( Vector3 fromPos, List<HitData> hitDataList, GunType? gunType = null, string BulletPrefabName = null,
    float shootTime = 4f) {
    var gunPrefab = Resources.Load( "Battle/Weapons/MachineGun" ) as GameObject;
	  var gunGo = Instantiate( gunPrefab, fightArea );

    var midPos = new Vector3( fromPos.x, fromPos.y, middlePoint.z );

    gunGo.transform.localPosition = midPos;
    //gunGo.transform.LookAt( opposModelGO.transform.Find( "Target" ) );
    //var a = opposPlay.GetTarget();
    gunGo.transform.LookAt( opposPlay.GetTarget() );
    gunGo.GetComponent<MachineGunShoot>().enabled = true;

    if (gunType != null) gunGo.GetComponent<MachineGunShoot>().SetGunType( gunType.Value, BulletPrefabName );

    if (hitDataList != null && hitDataList.Count > 0) {
      playGetHitCommon( 0.6f, false, hitDataList,
        () => {
          gunGo.GetComponent<MachineGunShoot>().enabled = false;
          CoroutineCommon.CallWaitForSeconds( 0f, () => { Destroy( gunGo ); } );
          BaseCallback();
        } );
      /*
      CoroutineCommon.CallWaitForSeconds( .5f, () => { 

        playGetHit( 1, 0 );
        playShake( hitDataList, () => {
          gunGo.GetComponent<MachineGunShoot>().enabled = false;
          CoroutineCommon.CallWaitForSeconds( 4f, () => { Destroy( gunGo ); } );

          playGetHit( -1, 0,
            () => {
              BaseCallback();
            } );

        } );

      } );
      */
    }
  }

  private List<Transform> missileTfList;
  private int missileCount;
  private Action missileCallback;
  private GameObject missilePf;
  private float missileRate;  
  private float missileSpeed;
  private string missileSfx;  //Missile
  private bool trail;
  private string trailName;
  private float addSec;
  private bool show;
  private float trailTime, width;
  private List<GameObject> missileGoList = new List<GameObject>();
  private bool lookAtTarget;

  protected void playMissileFrom( List<Transform> missileTfList, float missileRate, float missileSpeed, string missileSfx, Action callback, 
    bool trail = false, bool show = true, float addSec = 0, float trailTime = 1f, float width = .3f, string trailName = "BeamTrail", string customPf = "Battle/Weapons/Missile",
    bool lookAtTarget = true) {
    this.missileTfList = missileTfList;
    this.missileRate = missileRate;
    this.missileSpeed = missileSpeed;
    this.missileSfx = missileSfx;
    this.trail = trail;
    this.trailName = trailName;
    this.addSec = addSec;
    this.show = show;
    this.trailTime = trailTime;
    this.width = width;
    this.lookAtTarget = lookAtTarget;

    missileCount = 0;
    this.missileCallback = callback;
    //missilePf = Resources.Load( "Battle/Weapons/Missile" ) as GameObject;
    missilePf = Resources.Load( customPf ) as GameObject;
    playMissileFrom();
  }

  protected void playMissileFrom() {
    if (missileCount >= missileTfList.Count) {
      CoroutineCommon.CallWaitForSeconds( 1f + addSec, () => {
        missileCallback();
      } );
      return;
    }

    var missTf = missileTfList[missileCount++];
    //EffectSoundController.PLAY( (AudioClip)Resources.Load( $"SFX/Weapon/{missileSfx}" ), 5f );

    if (!string.IsNullOrWhiteSpace( missileSfx ))
      EffectSoundController.PLAY( (AudioClip)Resources.Load( $"SFX/Weapon/{missileSfx}" ), 2f );  //M4A1_Single
    CoroutineCommon.CallWaitForSeconds( missileRate, () => {
      var missileGo = Instantiate( missilePf, missTf.position, missilePf.transform.localRotation );   //防止光線出現兩條
      missileGo.transform.SetParent( missTf );    
      
      /*
      missileGo.transform.localScale = new Vector3(
        1f/missTf.localScale.x * missileGo.transform.localScale.x,
        1f/missTf.localScale.y * missileGo.transform.localScale.y,
        1f/missTf.localScale.z * missileGo.transform.localScale.z 
      );   //回復本身size
      */
      missileGo.transform.localPosition = new Vector3( 0, 0, 0 );
      missileGo.transform.localEulerAngles = missilePf.transform.localEulerAngles;
      if (lookAtTarget)
        missileGo.transform.LookAt( opposPlay.GetTarget() );
      missileGo.GetComponent<BulletShoot>().direction = Vector3.forward;
      missileGo.GetComponent<BulletShoot>().speed = missileSpeed;
      missileGo.GetComponent<BulletShoot>().SfxName = ""; //missileSfx;
      missileGo.GetComponent<BulletShoot>().PlaySound = 3f;
      missileGo.GetComponent<BulletShoot>().Trail = trail;
      missileGo.GetComponent<BulletShoot>().TrailName = trailName;
      missileGo.GetComponent<BulletShoot>().Show = show;
      missileGo.GetComponent<BulletShoot>().TrailTime = trailTime;
      missileGo.GetComponent<BulletShoot>().Width = width;

      missileGo.GetComponent<CapsuleCollider>().enabled = false;

      missileGoList.Add( missileGo );
      playMissileFrom();
    } );

  }

  protected void playMissileTo( float waitTime, List<HitData> hitDataList, int loop, int row, bool trail = false, int speed = 10, 
    bool isCollider = true, bool show = true, string missileSfx = "Missile", float trailTime = 1, float width = 0.2f, 
    string customPf = "Battle/Weapons/Missile", string trailName = "BeamTrail") {

    var midPos = new Vector3( -2f, 3.4f, middlePoint.z );

    var missileShooterPf = Resources.Load( "Battle/Weapons/MissileShooter" ) as GameObject;
    var missileShooterGo = Instantiate( missileShooterPf, fightArea );
    missileShooterGo.transform.localPosition = midPos;
    missileShooterGo.transform.LookAt( opposPlay.GetTarget() );
    missileShooterGo.GetComponent<MissileShooter>().playMissileFrom( opposPlay.GetTarget(), loop, row, speed, trail, isCollider, show, missileSfx, trailTime, width, customPf, trailName);

    if (hitDataList != null && hitDataList.Count > 0) {
      playGetHitCommon( waitTime, false, hitDataList,
        () => {
          //gunGo.GetComponent<MachineGunShoot>().enabled = false;
          CoroutineCommon.CallWaitForSeconds( 8f, () => { Destroy( missileShooterGo ); } );
          BaseCallback();
        } );
    }
  }

}

public class HitData {

  public float WaitTime { set; get; }

  /// <summary> 傷害持續時間 </summary>
  public float DelayTime { set; get; }

  public int Damage { set; get; }

  public string HitEffect = null;
}