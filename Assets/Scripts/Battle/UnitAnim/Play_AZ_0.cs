using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Play_AZ_0 : UnitPlayWeaponBase {

  private Transform bigCannonPos;
  private Transform w4Point;
  private List<Transform> mgList = new List<Transform>();

  public override void SetupWeapon() {
    bigCannonPos = transform.Find( "Armature/Hip/Body/Shoulder.R/UpperArm.R/MidArm.R/Forearm.R/Hand.R/BigGun/BigCannonPos" );

    w4Point = transform.Find( "Armature/Hip/Body/Body 1/ChestCannonPos" );

    mgList.Add( transform.Find( "Armature/Hip/Body/Head/Head 1/headGun.L" ) );
    mgList.Add( transform.Find( "Armature/Hip/Body/Head/Head 1/headGun.R" ) );
  }

  //ÀY¬¶
  public void playWeapon1() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    playMachineGunFrom( mgList );

    CoroutineCommon.CallWaitForSeconds( 2f, () => {
      stopMachineGunFrom();

      camRight.SetActive( !camRight.activeSelf );
      camLeft.SetActive( !camLeft.activeSelf );

      damageController.SetSide( opposSideDir, false );
      playBigMachineGunTo( mgList[1].position, new List<HitData>() { new HitData { WaitTime = 0.2f, DelayTime = 2f, Damage = attData.TotalDamage } }, MachineGunShoot.GunType.HEAD );
    } );

  }

  //¨Bºj
  public void playWeapon2() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    float missileRate = .6f;
    float totalTime = 1.2f;

    myAnimation.Stop();
    myAnimation["weapon2"].speed = 1f;
    myAnimation.Play( "weapon2" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
      fightArea.GetComponent<FightAnimController>().Stop = true;
      CoroutineCommon.CallWaitForSeconds( 0.1f, () => {
        //EffectSoundController.PLAY( (AudioClip)Resources.Load( $"SFX/Weapon/BeamRifle2" ), 5 );
        playMissileFrom( new List<Transform>() { bigCannonPos }, missileRate, 8f, "BeamRifle2", () => {
          switchCam();
          //fightArea.GetComponent<FightAnimController>().Stop = false;

          damageController.SetSide( opposSideDir, false );
          playMissileTo( .8f, new List<HitData>() { new HitData { WaitTime = 0, DelayTime = totalTime, Damage = attData.TotalDamage } },
            loop: 1, row: 0, trail: true, speed: 18, isCollider: false, show: false, missileSfx: "BeamRifle2", trailTime: .35f, width: .6f );
        },
        true,    //Trail
        false,   //Show
        .3f,     //After wait
        .35f, 0.6f );     //Trail Time, width
      } );
    } );
  }

  //¬ÞÀ»
  public void playWeapon3() {
    damageController.PlayEn( attData.FromUseEN );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    myAnimation.Play( "weapon3_1" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
      animator.enabled = true;
      animator.Play( "MeleeAttack_" + sideDir );

      CoroutineCommon.CallWaitForAnimator( animator, "MeleeAttack_" + sideDir, () => {
        camRight.SetActive( !camRight.activeSelf );
        camLeft.SetActive( !camLeft.activeSelf );

        animator.Play( "MeleeAttack2_" + sideDir );
        CoroutineCommon.CallWaitForAnimator( animator, "MeleeAttack2_" + sideDir, () => {
          animator.enabled = false;

          damageController.SetSide( opposSideDir, false );
          myAnimation.Play( "weapon3_2" );

          playGetHitCommon( 4 / 25f, true, new List<HitData>() { new HitData { WaitTime = 0f, DelayTime = 1.5f, Damage = attData.TotalDamage, HitEffect = "SaberHit" } }, BaseCallback );
        } );

      } );

    } );

  }

  //¹q¼ß¬¶
  public void playWeapon4() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    float missileRate = 0f;
    float totalTime = 1.2f;

    myAnimation.Stop();
    myAnimation["weapon4"].speed = 1f;
    myAnimation.Play( "weapon4" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/readyMove" ), 5 );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
      fightArea.GetComponent<FightAnimController>().Stop = true;
      CoroutineCommon.CallWaitForSeconds( 0.1f, () => {
        playMissileFrom( new List<Transform>() { w4Point }, missileRate, 8f, "BeamReflect", () => {
          switchCam();

          damageController.SetSide( opposSideDir, false );
          playMissileTo( .8f, new List<HitData>() { new HitData { WaitTime = 0, DelayTime = totalTime, Damage = attData.TotalDamage } },
            loop: 1, row: 0, trail: true, speed: 30, isCollider: false, show: false, missileSfx: "BeamReflect", trailTime: .35f, width: .8f, trailName: "PlasmaTrail" );
        },
        true,    //Trail
        false,   //Show
        .3f,     //After wait
        .35f, .8f, "PlasmaTrail" );     //Trail Time, width, Trail Name
      } );
    } );
  }

  //²ü¹q²É¤l¬¶
  public void playWeapon5() {
    float scale = 12f;
    geroBeamGO = Instantiate( geroBeamPF );
    geroBeamGO.transform.SetParent( bigCannonPos );
    geroBeamGO.transform.localPosition = new Vector3( 0, 0, 0 );
    geroBeamGO.transform.localRotation = Quaternion.Euler( new Vector3( 0, 0, 0 ) );

    Color beamColor = new Color32( 173, 216, 230, 255 );  //LightBlue , ²LÂÅ

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

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {

      damageController.PlayEn( attData.FromUseEN );

      CoroutineCommon.CallWaitForSeconds( .5f, () => {
        geroBeamGO.SetActive( true );
        EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Weapon/MegaBeam" ), 3 );

        CoroutineCommon.CallWaitForSeconds( 2f, () => {
          geroBeamGO.GetComponent<GeroBeam>().SmoothDestroy( () => {
            geroBeamGO.SetActive( false );

            CoroutineCommon.CallWaitForSeconds( .3f, () => {
              switchCam();
              damageController.SetSide( opposSideDir, false );
              ShootBeam( bigCannonPos.position, beamColor, scale, new List<HitData>() { new HitData { WaitTime = 0.2f, DelayTime = 2f, Damage = attData.TotalDamage } } );
            } );

          } );
        } );
      } );

    } );

  }

}
