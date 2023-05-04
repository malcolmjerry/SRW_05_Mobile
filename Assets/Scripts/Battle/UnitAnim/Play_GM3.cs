using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Play_GM3 : UnitPlayWeaponBase {

  private Transform riflePoint;

  // Use this for initialization
  void Start() {
  }

  private List<Transform> w1List = new List<Transform>();
  private List<Transform> w2List = new List<Transform>();
  private List<Transform> w5List = new List<Transform>();

  public override void SetupWeapon() {
    riflePoint = transform.Find( "Armature/arm_ik.R/Bone.021/rifle/Rifle/w4Pos" );

    w1List.Add( transform.Find( "Armature/Bone.009/Bone.008/Bone.016/Bone.017/Head/w1Pos.L" ) );
    w1List.Add( transform.Find( "Armature/Bone.009/Bone.008/Bone.016/Bone.017/Head/w1Pos.R" ) );
    w2List.Add( transform.Find( "Armature/Bone.009/Bone.008/Bone.001/Bone.002/Shoulder.L/w2Pos.L" ) );
    w2List.Add( transform.Find( "Armature/Bone.009/Bone.008/Bone.007/Bone.015/Shoulder.R/w2Pos.R" ) );
    w5List.Add( transform.Find( "Armature/Bone.009/Hip/w5Pos.L" ) );
    w5List.Add( transform.Find( "Armature/Bone.009/Hip/w5Pos.R" ) ); 
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

    playMachineGunFrom( w1List );

    CoroutineCommon.CallWaitForSeconds( 2f, () => {
      stopMachineGunFrom();

      camRight.SetActive( !camRight.activeSelf );
      camLeft.SetActive( !camLeft.activeSelf );

      damageController.SetSide( opposSideDir, false );
      playBigMachineGunTo( w1List[1].position, new List<HitData>() { new HitData { WaitTime = 0.2f, DelayTime = 2f, Damage = attData.TotalDamage } }, MachineGunShoot.GunType.HEAD );
    } );

  }

  //肩部飛彈
  public void playWeapon2() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    float missileRate = 0.2f;
    int loop = 1;
    int row = 1;
    float totalTime = (loop * (row * 2) - 1) * 0.2f + 1;

    myAnimation.Stop();
    myAnimation["weapon2"].speed = 1f;
    myAnimation.Play( "weapon2" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {

      playMissileFrom( w2List, missileRate, 10f, "missile", () => {
        switchCam();

        damageController.SetSide( opposSideDir, false );
        playMissileTo( 1.5f, new List<HitData>() { new HitData { WaitTime = 0, DelayTime = totalTime, Damage = attData.TotalDamage } }, loop, row );
      } );

    } );

  }

  //光束劍
  public void playWeapon3() {
    damageController.PlayEn( attData.FromUseEN );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    myAnimation.Play( "weapon3_1" );
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
        EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );
        CoroutineCommon.CallWaitForAnimator( animator, "MeleeAttack2_" + sideDir, () => {
          animator.enabled = false;

          damageController.SetSide( opposSideDir, false );
          myAnimation.Play( "weapon3_2" );
          EffectSoundController.PLAY_1( (AudioClip)Resources.Load( $"SFX/Weapon/BeamSaberStart" ), 3 );

          playGetHitCommon( 4 / 25f, true, new List<HitData>() { new HitData { WaitTime = 0f, DelayTime = 1.5f, Damage = attData.TotalDamage } }, BaseCallback );
        } );

      } );

    } );

  }

  //光束槍 
  public void playWeapon4() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    float missileRate = .2f;
    float totalTime = 1.2f;

    myAnimation.Stop();
    myAnimation["weapon4"].speed = 1f;
    myAnimation.Play( "weapon4" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
      fightArea.GetComponent<FightAnimController>().Stop = true;
      CoroutineCommon.CallWaitForSeconds( 0.1f, () => {
        //EffectSoundController.PLAY( (AudioClip)Resources.Load( $"SFX/Weapon/BeamRifle2" ), 5 );
        playMissileFrom( new List<Transform>() { riflePoint }, missileRate, 8f, "BeamRifle1", () => {
          switchCam();
          //fightArea.GetComponent<FightAnimController>().Stop = false;

          damageController.SetSide( opposSideDir, false );
          playMissileTo( .8f, new List<HitData>() { new HitData { WaitTime = 0, DelayTime = totalTime, Damage = attData.TotalDamage } },
            loop: 1, row: 0, trail: true, speed: 20, isCollider: false, show: false, missileSfx: "BeamRifle1", trailTime: .35f, width: .3f, trailName: "WhiteBeamTrail" );
        },
        true,    //Trail
        false,   //Show
        .3f,     //After wait
        .35f, 0.3f, "WhiteBeamTrail" );     //Trail Time, width
      } );
    } );
  }

  //腰部飛彈
  public void playWeapon5() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    float missileRate = 0.2f;
    int loop = 1;
    int row = 1;
    float totalTime = (loop * (row * 2) - 1) * 0.2f + 1;

    myAnimation.Stop();
    myAnimation["weapon5"].speed = 1f;
    myAnimation.Play( "weapon5" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {

      playMissileFrom( w5List, missileRate, 10f, "missile", () => {
        switchCam();

        damageController.SetSide( opposSideDir, false );
        playMissileTo( 1.5f, new List<HitData>() { new HitData { WaitTime = 0, DelayTime = totalTime, Damage = attData.TotalDamage } }, loop, row );
      } );

    } );
  }

}
