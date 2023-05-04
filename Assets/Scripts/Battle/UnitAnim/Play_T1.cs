using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class Play_T1 : UnitPlayWeaponBase {

  //public override float middleZ { get; set; } = 7;

  private Transform riflePoint;

  // Use this for initialization
  void Start() {
  }

  /*
  public new void Setup( Transform fightArea, Transform modelSide, Transform opposModelSide, GameObject opposModelGO, string sideDir,  GameObject camRight, GameObject camLeft,
                 Vector3 originPos, Quaternion originRot, DamageController damageController, int pilotID ) {
    base.Setup( fightArea, modelSide, opposModelSide, opposModelGO, sideDir, camRight, camLeft, originPos, originRot, damageController, pilotID );

    middlePoint = new Vector3( 0, 0, sideDir == "R" ? middleZ : -middleZ );

    riflePoint = transform.Find( "Armature/Bone/Bone.005/Bone.016/Bone.017/Bone.018/Bone.019/Mesh92/RiflePoint" );
  }*/

  private List<Transform> missileList = new List<Transform>();

  public override void SetupWeapon() {
    //middlePoint = new Vector3( 0, 0, sideDir == "R" ? middleZ : -middleZ );
    riflePoint = transform.Find( "Armature/Bone.016/Mesh92/RiflePoint" );

    missileList.Add( transform.Find( "Missile1" ) );
    missileList.Add( transform.Find( "Missile2" ) );
  }

  //飛彈
  public void playWeapon3() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    float missileRate = 0.2f;
    int loop = 1;
    int row = 1;
    float totalTime = (loop * (row * 2) -1) * 0.2f + 1;

    myAnimation.Stop();
    myAnimation["weapon3"].speed = 1f;
    myAnimation.Play( "weapon3" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
      CoroutineCommon.CallWaitForSeconds( 0.1f, () => {
        playMissileFrom( missileList, missileRate, 10f, "missile", () => {
          switchCam();

          damageController.SetSide( opposSideDir, false );
          playMissileTo( 1.5f, new List<HitData>() { new HitData { WaitTime = 0, DelayTime = totalTime, Damage = attData.TotalDamage } }, loop, row );
          //} );
        } );
      } );
    });
  }

  //光劍
  public void playWeapon1() {
    damageController.PlayEn( attData.FromUseEN );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    myAnimation.Play( "weapon1_1" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );
    EffectSoundController.PLAY_1( (AudioClip)Resources.Load( $"SFX/Weapon/BeamSaberHit" ), 2 );

    animator.enabled = true;
    animator.Play( "MeleeAttack_" + sideDir );

    CoroutineCommon.CallWaitForAnimator( animator, "MeleeAttack_" + sideDir, () => {
      camRight.SetActive( !camRight.activeSelf );
      camLeft.SetActive( !camLeft.activeSelf );

      animator.Play( "MeleeAttack2_" + sideDir );
      EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );
      CoroutineCommon.CallWaitForAnimator( animator, "MeleeAttack2_" + sideDir, () => {
        animator.enabled = false;

        damageController.SetSide( opposSideDir, false );
        myAnimation.Play( "weapon1_2" );
        EffectSoundController.PLAY_1( (AudioClip)Resources.Load( $"SFX/Weapon/BeamSaberStart" ), 3 );

        playGetHitCommon( 8 / 25f, true, new List<HitData>() { new HitData { WaitTime = 0f, DelayTime = 1f, Damage = attData.TotalDamage } }, BaseCallback );
      } );

    } );
  }

  //火箭炮
  public void playWeapon2() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    float missileRate = 0.2f;
    float totalTime = 1.2f;

    myAnimation.Stop();
    myAnimation["weapon2"].speed = 1f;
    myAnimation.Play( "weapon2" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
      CoroutineCommon.CallWaitForSeconds( 0.1f, () => {
        playMissileFrom( new List<Transform>() { riflePoint }, missileRate, 15f, "Bazooka", customPf: "Battle/Weapons/BazookaBall", callback: () => {
          switchCam();
          damageController.SetSide( opposSideDir, false );
          playMissileTo( 1.5f, new List<HitData>() { new HitData { WaitTime = 0, DelayTime = totalTime, Damage = attData.TotalDamage } }, loop: 1, row: 0, customPf: "Battle/Weapons/BazookaBall" );
        } );
      } );
    } );
  }

  //光束照射
  public void playWeapon4() {
    //playMeleeWeapon1( /*side*/ );

    geroBeamGO = Instantiate( geroBeamPF ) as GameObject;
    geroBeamGO.transform.SetParent( riflePoint );
    geroBeamGO.transform.localPosition = new Vector3( 0, 0, 0 );
    geroBeamGO.transform.localRotation = Quaternion.Euler( new Vector3( 0, 0, 0 ) );

    var beamParam = geroBeamGO.GetComponent<BeamParam>();
    beamParam.BeamColor = Color.yellow;
    beamParam.Scale = 3f;
    geroBeamGO.SetActive( false );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    //damageController.StatusRect.SetActive( true );

    myAnimation.Stop();
    myAnimation["weapon2"].speed = 1f;
    myAnimation.Play( "weapon2" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {

      damageController.PlayEn( attData.FromUseEN );

      CoroutineCommon.CallWaitForSeconds( .5f, () => {
        geroBeamGO.SetActive( true );
        EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Weapon/MegaBeam" ), 3 );

        CoroutineCommon.CallWaitForSeconds( 1.5f, () => {
          geroBeamGO.GetComponent<GeroBeam>().SmoothDestroy( () => {
            geroBeamGO.SetActive( false );

            CoroutineCommon.CallWaitForSeconds( .3f, () => {
              //camRight.SetActive( !camRight.activeSelf );
              //camLeft.SetActive( !camLeft.activeSelf );
              //myAnimation["weapon2"].time = 0f;
              //myAnimation["weapon2"].speed = 0f;
              switchCam();
              myAnimation.Play( "Idle" );

              damageController.SetSide( opposSideDir, false );
              ShootBeam( riflePoint.position, Color.yellow, 3f, new List<HitData>() { new HitData { WaitTime = 0.2f, DelayTime = 1.5f, Damage = attData.TotalDamage } } );
            } );

          } );
        } );
      } );

    } );

  }

}
