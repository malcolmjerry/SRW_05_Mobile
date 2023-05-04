using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Play_RZ_0 : UnitPlayWeaponBase {

  private Transform w2Point;
  private Transform bigCannonPos;
  private List<Transform> missileList = new List<Transform>();

  // Start is called before the first frame update
  void Start() {

  }

  public override void SetupWeapon() {
    w2Point = transform.Find( "Armature/Hip/Body/Wing1.R/Cannon.R/Plasma.R/PlasmaPoint.R" );
    bigCannonPos = transform.Find( "Armature/Hip/Body/Body 1/BigCannonPos" );

    missileList.Add( transform.Find( "MissilePos.R" ) );
    missileList.Add( transform.Find( "MissilePos.L" ) );
    missileList.AddRange( missileList );
  }

  //飛彈
  public void playWeapon1() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    float missileRate = 0.2f;
    int loop = 2;
    int row = 1;
    float totalTime = (loop * (row * 2) - 1) * 0.2f + 1;

    myAnimation.Stop();
    myAnimation["weapon1"].speed = 1f;
    myAnimation.Play( "weapon1" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/readyMove" ), 5 );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
      CoroutineCommon.CallWaitForSeconds( 0.2f, () => {
        playMissileFrom( missileList, missileRate, 10f, "missile", () => {
          switchCam();

          damageController.SetSide( opposSideDir, false );
          playMissileTo( 1.5f, new List<HitData>() { new HitData { WaitTime = 0, DelayTime = totalTime, Damage = attData.TotalDamage } }, loop, row );
        } );
      } );
    } );
  }

  //電漿炮
  public void playWeapon2() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    float missileRate = 0f;
    float totalTime = 1.2f;

    myAnimation.Stop();
    myAnimation["weapon2"].speed = 1f;
    myAnimation.Play( "weapon2" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/readyMove" ), 5 );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
      fightArea.GetComponent<FightAnimController>().Stop = true;
      CoroutineCommon.CallWaitForSeconds( 0.1f, () => {
        playMissileFrom( new List<Transform>() { w2Point }, missileRate, 8f, "BeamReflect", () => {
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

  //斬擊
  public void playWeapon3() {
    damageController.PlayEn( attData.FromUseEN );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    myAnimation.Play( "weapon3_1" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    animator.enabled = true;
    animator.Play( "MeleeAttack_" + sideDir );

    CoroutineCommon.CallWaitForAnimator( animator, "MeleeAttack_" + sideDir, () => {
      switchCam();

      animator.Play( "MeleeAttack2_" + sideDir );
      EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

      CoroutineCommon.CallWaitForAnimator( animator, "MeleeAttack2_" + sideDir, () => {
        animator.enabled = false;

        damageController.SetSide( opposSideDir, false );
        myAnimation.Play( "weapon3_2" );

        playGetHitCommon( 8 / 25f, true, new List<HitData>() { new HitData { WaitTime = 0f, DelayTime = 2f, Damage = attData.TotalDamage, HitEffect = "SaberHit" } }, BaseCallback );
      } );

    } );

  }

  //牙突
  public void playWeapon4() {
    damageController.PlayEn( attData.FromUseEN );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    myAnimation.Play( "weapon4" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
      animator.enabled = true;
      animator.Play( "MeleeAttack_" + sideDir );

      CoroutineCommon.CallWaitForAnimator( animator, "MeleeAttack_" + sideDir, () => {
        switchCam();

        animator.Play( "MeleeAttack3_" + sideDir );
        EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

        CoroutineCommon.CallWaitForSeconds( .7f, () => {
          damageController.SetSide( opposSideDir, false );
          playGetHitCommon( .0f, needMoveBack: false, new List<HitData>() { new HitData { WaitTime = 0f, DelayTime = 1.5f, Damage = attData.TotalDamage } },
          () => {
            animator.enabled = false;
            BaseCallback();
          } );
        } );

      } );

    } );

  }

  //中性粒子炮
  public void playWeapon5() {
    float scale = 12f;
    geroBeamGO = Instantiate( geroBeamPF );
    geroBeamGO.transform.SetParent( bigCannonPos );
    geroBeamGO.transform.localPosition = new Vector3( 0, 0, 0 );
    geroBeamGO.transform.localRotation = Quaternion.Euler( new Vector3( 0, 0, 0 ) );

    Color beamColor = Color.white;

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
              camRight.SetActive( !camRight.activeSelf );
              camLeft.SetActive( !camLeft.activeSelf );
              enabled = false;
              damageController.SetSide( opposSideDir, false );
              ShootBeam( bigCannonPos.position, beamColor, scale, new List<HitData>() { new HitData { WaitTime = 0.2f, DelayTime = 2f, Damage = attData.TotalDamage } } );
            } );

          } );
        } );
      } );

    } );

  }

}
