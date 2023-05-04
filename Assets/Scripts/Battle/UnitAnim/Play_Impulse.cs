using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Play_Impulse : UnitPlayWeaponBase {

  //public override float middleZ { get; set; } = 7;

  private Transform riflePoint;

  // Use this for initialization
  void Start() {
  }

  /*
  public new void Setup( Transform fightArea, Transform modelSide, Transform opposModelSide, GameObject opposModelGO, string sideDir, GameObject camRight, GameObject camLeft,
                   Vector3 originPos, Quaternion originRot, DamageController damageController, int pilotID ) {
    base.Setup( fightArea, modelSide, opposModelSide, opposModelGO, sideDir, camRight, camLeft, originPos, originRot, damageController, pilotID );

    middlePoint = new Vector3( 0, 0, sideDir == "R"? middleZ : -middleZ );
    riflePoint = transform.Find( "Armature/Bone.002/Bone.023/Bone.024/Bone.025/Bone.026/Bone.027/RightFinger/RightRifle/RiflePoint" );
  }
  */
  private List<Transform> mgList = new List<Transform>();

  public override void SetupWeapon() {
    //middlePoint = new Vector3( 0, 0, sideDir == "R" ? middleZ : -middleZ );
    riflePoint     = transform.Find( "Armature/arm_ik.R/Bone.021/rifle/Rifle/RiflePoint" );

    mgList.Add( transform.Find( "Armature/Bone.009/Bone.008/Bone.016/Bone.017/Head/headGun.L" ) );
    mgList.Add( transform.Find( "Armature/Bone.009/Bone.008/Bone.016/Bone.017/Head/headGun.R" ) );
  }

  // Update is called once per frame
  void Update() {

  }

  //頭炮
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

  //光束槍 
  public void playWeapon2() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    float missileRate = .2f;
    float totalTime = 1.2f;

    myAnimation.Stop();
    myAnimation["weapon2"].speed = 1f;
    myAnimation.Play( "weapon2" );
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
            loop: 1, row: 0, trail: true, speed: 30, isCollider: false, show: false, missileSfx: "BeamRifle2", trailTime: .35f, width: .3f  );
        }, 
        true,    //Trail
        false,   //Show
        .3f,     //After wait
        .35f, 0.3f );     //Trail Time, width
      } );
    } );
  }


  /* 光束槍 looping
  public void playWeapon2() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs[0].Text );

    float missileRate = .6f;
    float totalTime = 1.2f;

    myAnimation.Stop();
    myAnimation["weapon2"].speed = 1f;
    myAnimation.Play( "weapon2" );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
      fightArea.GetComponent<FightAnimController>().Stop = true;
      CoroutineCommon.CallWaitForSeconds( 0.1f, () => {
        playMissileFrom( new List<Transform>() { riflePoint }, missileRate, 8f, "BeamRifle2", () => {
          playWeapon2();
        },
        true,    //Trail
        true,   //Show
        .3f,     //After wait
        .3f, 0.2f );     //Trail Time, width
      } );
    } );
  }
  */

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

  //對艦刀
  public void playWeapon4() {
    damageController.PlayEn( attData.FromUseEN );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    myAnimation.Play( "weapon4_1" );

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
          myAnimation.Play( "weapon4_2" );

          playGetHitCommon( 4/25f, true, new List<HitData>() { new HitData { WaitTime = 0f, DelayTime = 1.5f, Damage = attData.TotalDamage } }, BaseCallback );
        } );

      } );

    } );
  }

}
