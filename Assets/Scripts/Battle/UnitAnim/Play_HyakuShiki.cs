using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Play_HyakuShiki : UnitPlayWeaponBase {
  private Transform riflePoint;

  // Use this for initialization
  void Start() {
  }

  private List<Transform> mgList = new List<Transform>();

  public override void SetupWeapon() {
    riflePoint = transform.Find( "metarig/spine/spine.001/spine.002/spine.003/shoulder.R/upper_arm.R/mid_arm.R/forearm.R/hand.R/RifleBone/Rifle/RiflePoint" );

    mgList.Add( transform.Find( "metarig/spine/spine.001/spine.002/spine.003/spine.006/Head/HeadGun.L" ) );
    mgList.Add( transform.Find( "metarig/spine/spine.001/spine.002/spine.003/spine.006/Head/HeadGun.R" ) );
  }

  //頭炮
  public void playWeapon1() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    myAnimation.Stop();
    myAnimation["weapon1"].speed = 1f;
    myAnimation.Play( "weapon1" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    playMachineGunFrom( mgList, Vector3.up );

    CoroutineCommon.CallWaitForSeconds( 2f, () => {
      stopMachineGunFrom();

      camRight.SetActive( !camRight.activeSelf );
      camLeft.SetActive( !camLeft.activeSelf );

      damageController.SetSide( opposSideDir, false );
      playBigMachineGunTo( mgList[1].position, new List<HitData>() { new HitData { WaitTime = 0.2f, DelayTime = 2f, Damage = attData.TotalDamage } }, MachineGunShoot.GunType.HEAD );
    } );

  }

  //光束劍
  public void playWeapon2() {
    damageController.PlayEn( attData.FromUseEN );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    myAnimation.Play( "weapon2_1" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );
    EffectSoundController.PLAY_1( (AudioClip)Resources.Load( $"SFX/Weapon/BeamSaberHit" ), 2 );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {

      animator.enabled = true;
      animator.Play( "MeleeAttack_" + sideDir );
      EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

      CoroutineCommon.CallWaitForAnimator( animator, "MeleeAttack_" + sideDir, () => {
        camRight.SetActive( !camRight.activeSelf );
        camLeft.SetActive( !camLeft.activeSelf );

        animator.Play( "MeleeAttack2_" + sideDir );
        CoroutineCommon.CallWaitForAnimator( animator, "MeleeAttack2_" + sideDir, () => {
          animator.enabled = false;

          damageController.SetSide( opposSideDir, false );
          myAnimation.Play( "weapon2_2" );
          EffectSoundController.PLAY_1( (AudioClip)Resources.Load( $"SFX/Weapon/BeamSaberStart" ), 3 );

          playGetHitCommon( 4 / 25f, true, new List<HitData>() { new HitData { WaitTime = 0f, DelayTime = 1.5f, Damage = attData.TotalDamage } }, BaseCallback );
        } );

      } );

    } );

  }

  //光束槍 
  public void playWeapon3() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    float missileRate = .2f;
    float totalTime = 1.2f;

    myAnimation.Stop();
    myAnimation["weapon3"].speed = 1f;
    myAnimation.Play( "weapon3" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
      fightArea.GetComponent<FightAnimController>().Stop = true;
      CoroutineCommon.CallWaitForSeconds( 0.1f, () => {
        //EffectSoundController.PLAY( (AudioClip)Resources.Load( $"SFX/Weapon/BeamRifle2" ), 5 );
        playMissileFrom( new List<Transform>() { riflePoint }, missileRate, 8f, "BeamRifle2", () => {
          switchCam();
          //fightArea.GetComponent<FightAnimController>().Stop = false;

          damageController.SetSide( opposSideDir, false );
          playMissileTo( .8f, new List<HitData>() { new HitData { WaitTime = 0, DelayTime = totalTime, Damage = attData.TotalDamage } },
            loop: 1, row: 0, trail: true, speed: 20, isCollider: false, show: false, missileSfx: "BeamRifle2", trailTime: .35f, width: .3f );
        },
        true,    //Trail
        false,   //Show
        .3f,     //After wait
        .35f, 0.3f );     //Trail Time, width
      } );
    } );
  }

  //火箭炮
  public void playWeapon4() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    float missileRate = 0.2f;
    float totalTime = 1.2f;

    myAnimation.Stop();
    myAnimation["weapon4"].speed = 1f;
    myAnimation.Play( "weapon4" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
      CoroutineCommon.CallWaitForSeconds( 0.1f, () => {
        playMissileFrom( new List<Transform>() { transform.Find( "metarig/spine/spine.001/spine.002/spine.003/shoulder.R/upper_arm.R/mid_arm.R/forearm.R/hand.R/basuka/Basuka/BazookaPoint" ) }, missileRate, 15f, "Bazooka", () => {
          switchCam();

          damageController.SetSide( opposSideDir, false );
          playMissileTo( 1.5f, new List<HitData>() { new HitData { WaitTime = 0, DelayTime = totalTime, Damage = attData.TotalDamage } }, loop: 1, row: 0, customPf: "Battle/Weapons/BazookaBall" );
        }, customPf: "Battle/Weapons/BazookaBall" );
      } );
    } );
  }

  //米加炮發射器
  public void playWeapon5() {
    float scale = 15f;
    geroBeamGO = Instantiate( geroBeamPF );
    var shootPoint = transform.Find( "metarig/CannonBone/Cannon/CannonPoint" );

    geroBeamGO.transform.SetParent( shootPoint );
    geroBeamGO.transform.localPosition = new Vector3( 0, 0, 0 );
    geroBeamGO.transform.localRotation = Quaternion.Euler( new Vector3( 0, 0, 0 ) );

    Color beamColor = Color.yellow;

    var beamParam = geroBeamGO.GetComponent<BeamParam>();
    beamParam.BeamColor = beamColor;
    beamParam.Scale = scale;
    geroBeamGO.SetActive( false );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    //myAnimation.Stop();
    myAnimation["weapon5"].speed = 1f;
    myAnimation.Play( "weapon5" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );
    EffectSoundController.PLAY_1( (AudioClip)Resources.Load( "SFX/readyMove" ), 2f );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {

      fightArea.GetComponent<FightAnimController>().Stop = true;
      damageController.PlayEn( attData.FromUseEN );

      // 移動鏡頭
      var currentCam = getCurrentCam();
      tempCam = Instantiate( currentCam.GetComponent<Camera>(), currentCam.transform.parent );
      startPos = tempCam.transform.localPosition;
      endPos = new Vector3( tempCam.transform.localPosition.x + 7f, tempCam.transform.localPosition.y + 7, tempCam.transform.localPosition.z + 4f );
      startRotation = tempCam.transform.localRotation;
      endRotation = Quaternion.Euler( startRotation.eulerAngles.x + 30f, startRotation.eulerAngles.y + 75f, startRotation.eulerAngles.z );
      tempCamRotation = true;
      startTime = Time.time;
      fractionLen = 3f;
      enabled = true;
      // END OF 移動鏡頭

      geroBeamGO.SetActive( true );
      EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Weapon/MegaBeam" ), 3 );

      CoroutineCommon.CallWaitForSeconds( 3.5f, () => {
        geroBeamGO.GetComponent<GeroBeam>().SmoothDestroy( () => {
          geroBeamGO.SetActive( false );

          CoroutineCommon.CallWaitForSeconds( .3f, () => {
            switchCam();
            damageController.SetSide( opposSideDir, false );
            ShootBeam( shootPoint.position, beamColor, scale, new List<HitData>() { new HitData { WaitTime = 0.2f, DelayTime = 2f, Damage = attData.TotalDamage } } );
          } );
        } );
      } );

    } );

  }

}
