using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Play_Jefty : UnitPlayWeaponBase {

  //public override float middleZ { get; set; } = 7;

  private Transform w1Point;
  public GameObject EnergyBallPf;
  private List<Transform> mgList = new List<Transform>();
  //private List<Transform> missileList = new List<Transform>();

  // Use this for initialization
  void Start() {
  }



  private new void Awake() {
    //EnergyBall = Resources.Load( "Battle/Weapons/EnergyBall" ) as GameObject;
    base.Awake();
    w1Point = transform.Find( "metarig/spine/spine.001/spine.002/spine.003/shoulder.L/arm.IK.L/hand.L/Hand.L/w1Point" );
    mgList.Add( w1Point );
  }

  /*
  public override void SetupWeapon() {
    w1Point = transform.Find( "metarig/spine/spine.001/spine.002/spine.003/shoulder.L/arm.IK.L/hand.L/Hand.L/w1Point" );
    mgList.Add( w1Point );

    missileList.Add( transform.Find( "Missile1" ) );
    missileList.Add( transform.Find( "Missile2" ) );
  }*/

  //手部機關槍
  public void playWeapon1() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    myAnimation.Stop();
    myAnimation["weapon1"].speed = 1f;
    myAnimation.Play( "weapon1" );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {

      playMachineGunFrom( mgList, Vector3.up, "BulletSphereWhite" );

      CoroutineCommon.CallWaitForSeconds( 3f, () => {
        stopMachineGunFrom();

        camRight.SetActive( !camRight.activeSelf );
        camLeft.SetActive( !camLeft.activeSelf );

        damageController.SetSide( opposSideDir, false );
        playBigMachineGunTo( mgList[0].position,
          new List<HitData>() { new HitData { WaitTime = 0.2f, DelayTime = 2f, Damage = attData.TotalDamage } },
          MachineGunShoot.GunType.Single, "BulletSphereWhite", 1f );
      } );
    } );

  }

  //擴散雷射炮
  public void playWeapon2() {
    List<Transform> shootPoints = new List<Transform>();
    int count = 20;
    for (int i = 1; i <= count; i++) {
      shootPoints.Add( transform.Find( $"metarig/spine/spine.001/spine.002/spine.003/shoulder.L/arm.IK.L/hand.L/Hand.L/w2Point/S{i}" ) );
    }

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    float missileRate = .1f;
    float totalTime = 1.2f;

    myAnimation.Stop();
    myAnimation["weapon2"].speed = 1f;
    myAnimation.Play( "weapon2" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
      fightArea.GetComponent<FightAnimController>().Stop = true;


      CoroutineCommon.CallWaitForSeconds( 0.1f, () => {
        for (int i = 0; i < count - 1; i++) {
          //EffectSoundController.PLAY( (AudioClip)Resources.Load( $"SFX/Weapon/BeamRifle2" ), 5 );
          playMissileFrom( new List<Transform>() { shootPoints[i] }, missileRate, 8f, "", () => {},
            true,    //Trail
            false,   //Show
            .3f,     //After wait
            .35f, 0.3f, "WhiteBeamTrail", lookAtTarget: false
          );     //Trail Time, width
        }
      } );

      CoroutineCommon.CallWaitForSeconds( 0.1f, () => {
        //EffectSoundController.PLAY( (AudioClip)Resources.Load( $"SFX/Weapon/BeamRifle2" ), 5 );
        playMissileFrom( new List<Transform>() { shootPoints[count-1] }, missileRate, 8f, "BeamRifle2", () => {
          switchCam();
          damageController.SetSide( opposSideDir, false );
          playWeapon2To( 1f, new List<HitData>() { new HitData { WaitTime = 0, DelayTime = totalTime, Damage = attData.TotalDamage } },
            loop: 1, row: 0, trail: true, speed: 20, isCollider: false, show: false, missileSfx: "BeamRifle2", trailTime: .35f, width: .3f );
        },
        true,    //Trail
        false,   //Show
        1.5f,     //After wait
        1f, 0.1f, "WhiteBeamTrail", lookAtTarget: false );     //Trail Time, width
      } );
    } );
  }

  protected void playWeapon2To( float waitTime, List<HitData> hitDataList, int loop, int row, bool trail = false, int speed = 10,
    bool isCollider = true, bool show = true, string missileSfx = "Missile", float trailTime = 1, float width = 0.2f,
    string customPf = "Battle/Weapons/Missile", string trailName = "BeamTrail" ) {

    var midPos = new Vector3( -2f, 3.4f, middlePoint.z );

    var missileShooterPf = Resources.Load( "Battle/Weapons/DiffusionShooter" ) as GameObject;
    var missileShooterGo = Instantiate( missileShooterPf, fightArea );
    missileShooterGo.transform.localPosition = midPos;
    missileShooterGo.transform.LookAt( opposPlay.GetTarget() );

    missileSfx = "";
    //List<Transform> shootPoints = new List<Transform>();
    int count = 20;
    for (int i = 1; i <= count; i++) {
      Transform point = missileShooterGo.transform.Find( $"A{i}" );
      point.LookAt( opposPlay.GetTarget() );
      //shootPoints.Add( point );

      if (i == count)
        missileSfx = "BeamRifle2";

      playMissileFrom( new List<Transform>() { point }, 0, speed, missileSfx, () => { }, 
        true,    //Trail
        false,   //Show
        .3f,     //After wait
        .35f,    //Trail Time
        0.3f,    //Width
        "WhiteBeamTrail" );     //TrailName
    }

    if (hitDataList != null && hitDataList.Count > 0) {
      playGetHitCommon( waitTime, false, hitDataList,
        () => {
          //gunGo.GetComponent<MachineGunShoot>().enabled = false;
          CoroutineCommon.CallWaitForSeconds( 10f, () => { Destroy( missileShooterGo ); } );
          BaseCallback();
        } );
    }
  }

  //斬擊
  public void playWeapon3() {
    damageController.PlayEn( attData.FromUseEN );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    myAnimation.Play( "weapon3_1" );

    animator.enabled = true;
    animator.Play( "MeleeAttack_" + sideDir );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    CoroutineCommon.CallWaitForAnimator( animator, "MeleeAttack_" + sideDir, () => {
      camRight.SetActive( !camRight.activeSelf );
      camLeft.SetActive( !camLeft.activeSelf );

      animator.Play( "MeleeAttack2_" + sideDir );
      EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );
      CoroutineCommon.CallWaitForAnimator( animator, "MeleeAttack2_" + sideDir, () => {
        animator.enabled = false;

        damageController.SetSide( opposSideDir, false );
        myAnimation.Play( "weapon3_2" );
        //EffectSoundController.PLAY_1( (AudioClip)Resources.Load( $"SFX/Weapon/BeamSaberStart" ), 3 );

        playGetHitCommon( 8 / 25f, true, new List<HitData>() { new HitData { WaitTime = 0f, DelayTime = 1f, Damage = attData.TotalDamage, HitEffect = "SaberHit" } }, BaseCallback );
      } );

    } );

  }

  //能量球投擲
  public void playWeapon4() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    myAnimation.Stop();
    myAnimation["weapon4_1"].speed = 1f;
    myAnimation.Play( "weapon4_1" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/readyMove" ), 2f );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
      Transform w1Point = transform.Find( "metarig/spine/spine.001/spine.002/spine.003/shoulder.L/arm.IK.L/hand.L/Hand.L/w1Point" );
      var energyBallGo = Instantiate( EnergyBallPf );
      energyBallGo.name = "EnergyBall";
      energyBallGo.transform.position = w1Point.position;
      energyBallGo.transform.SetParent( w1Point, true );
      energyBallGo.transform.LookAt( opposPlay.GetTarget() );
      damageController.PlayEn( attData.FromUseEN, 1.5f );

      var currentCam = getCurrentCam();
      tempCam = Instantiate( currentCam.GetComponent<Camera>(), currentCam.transform.parent );
      startPos = tempCam.transform.localPosition;
      endPos = new Vector3( tempCam.transform.localPosition.x, tempCam.transform.localPosition.y + 1.5f, tempCam.transform.localPosition.z );
      startTime = Time.time;
      fractionLen = .7f;
      enabled = true;

      energyBallGo.GetComponent<EnergyBall>().Charge( () => {

        myAnimation.Stop();
        myAnimation["weapon4_2"].speed = 1f;
        myAnimation.Play( "weapon4_2" );

        CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
          energyBallGo.transform.SetParent( fightArea, true );

          energyBallGo.GetComponent<EnergyBall>().Attack( opposPlay.GetTarget(), false, () => { } );

          CoroutineCommon.CallWaitForSeconds( 1.5f, () => {
            switchCam();
            Destroy( tempCam.gameObject );
            Destroy( energyBallGo );
            damageController.SetSide( opposSideDir, false );
            playWeapon4_2( new List<HitData>() { new HitData { WaitTime = 0f, DelayTime = 2f, Damage = attData.TotalDamage } } );
            //} );
          } );
        } );

      } );
    } );

  }

  // 能量球投擲 (後半)
  protected void playWeapon4_2( List<HitData> hitDataList ) {
    var energyBallGo = Instantiate( EnergyBallPf, fightArea );

    var midPos = new Vector3( -2f, 3.8f, middlePoint.z );
    energyBallGo.name = "EnergyBall";
    energyBallGo.transform.localPosition = midPos;
    //energyBallGo.transform.LookAt( opposPlay.GetTarget() );
    energyBallGo.GetComponent<EnergyBall>().Attack( opposPlay.GetTarget(), attData.CutType > 0,
      () => {
        playGetHitCommon( 0, false, hitDataList,
          () => {
            Destroy( energyBallGo );
            BaseCallback();
          } );
        //}
      }
    );

    if (attData.CutType > 0 || attData.IsDodge) {
      playGetHitCommon( 1.5f, false, hitDataList, () => {
        Destroy( energyBallGo );
        BaseCallback();
      } );
    }
  }

  //空間壓縮破碎砲
  public void playWeapon5() {
    float scale = 12f;
    geroBeamGO = Instantiate( geroBeamPF );
    var shootPoint = transform.Find( "metarig/spine/vectorCannon/VectorCannonMesh/w5Point" );

    geroBeamGO.transform.SetParent( shootPoint );
    geroBeamGO.transform.localPosition = new Vector3( 0, 0, 0 );
    geroBeamGO.transform.localRotation = Quaternion.Euler( new Vector3( 0, 0, 0 ) );

    Color beamColor = Color.grey;

    var beamParam = geroBeamGO.GetComponent<BeamParam>();
    beamParam.BeamColor = beamColor;
    beamParam.Scale = scale;
    geroBeamGO.SetActive( false );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    var currentCam = getCurrentCam();
    tempCam = Instantiate( currentCam.GetComponent<Camera>(), currentCam.transform.parent );
    startPos = tempCam.transform.localPosition;
    endPos = new Vector3( tempCam.transform.localPosition.x, tempCam.transform.localPosition.y, tempCam.transform.localPosition.z - 6f );
    startRotation = tempCam.transform.localRotation;
    endRotation = Quaternion.Euler( startRotation.eulerAngles.x, startRotation.eulerAngles.y - 50f, startRotation.eulerAngles.z );
    tempCamRotation = true;
    startTime = Time.time;
    fractionLen = 2f;
    enabled = true;

    //myAnimation.Stop();
    myAnimation["weapon5"].speed = 1f;
    myAnimation.Play( "weapon5" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {

      damageController.PlayEn( attData.FromUseEN );

      CoroutineCommon.CallWaitForSeconds( .5f, () => {
        geroBeamGO.SetActive( true );
        EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Weapon/MegaBeam" ), 3 );

        CoroutineCommon.CallWaitForSeconds( 2f, () => {
          geroBeamGO.GetComponent<GeroBeam>().SmoothDestroy( () => {
            geroBeamGO.SetActive( false );

            CoroutineCommon.CallWaitForSeconds( .3f, () => {
              camRight.SetActive( !camRight.activeSelf );
              camLeft.SetActive( !camLeft.activeSelf );
              enabled = false;
              damageController.SetSide( opposSideDir, false );
              ShootBeam( shootPoint.position, beamColor, scale, new List<HitData>() { new HitData { WaitTime = 0.2f, DelayTime = 2f, Damage = attData.TotalDamage } } );
            } );

          } );
        } );
      } );

    } );

  }

}
