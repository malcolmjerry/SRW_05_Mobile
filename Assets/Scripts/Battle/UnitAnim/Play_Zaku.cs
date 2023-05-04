using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Play_Zaku : UnitPlayWeaponBase {

  //電熱斧
  public void playWeapon1() {
    damageController.PlayEn( attData.FromUseEN );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    myAnimation.Play( "weapon1_1" );
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
          myAnimation.Play( "weapon1_2" );
          EffectSoundController.PLAY_1( (AudioClip)Resources.Load( $"SFX/Weapon/BeamSaberStart" ), 3 );

          playGetHitCommon( 4 / 25f, true, new List<HitData>() { new HitData { WaitTime = 0f, DelayTime = 1.5f, Damage = attData.TotalDamage } }, BaseCallback );
        } );

      } );

    } );

  }

  //機關槍
  public void playWeapon2() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    myAnimation.Stop();
    myAnimation["weapon2"].speed = 1f;
    myAnimation.Play( "weapon2" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    List<Transform> mgList = new List<Transform>();
    mgList.Add( transform.Find( "Armature/arm_ik.R/Bone.021/rifle/Gun/GunPos" ) );

    CoroutineCommon.CallWaitForSeconds( .5f, () => {   //先做點預備動作
      playMachineGunFrom( mgList, Vector3.up );
    });

    CoroutineCommon.CallWaitForSeconds( 3f, () => {
      stopMachineGunFrom();

      camRight.SetActive( !camRight.activeSelf );
      camLeft.SetActive( !camLeft.activeSelf );

      damageController.SetSide( opposSideDir, false );
      playBigMachineGunTo( mgList[0].position, new List<HitData>() { new HitData { WaitTime = 0.2f, DelayTime = 2f, Damage = attData.TotalDamage } }, MachineGunShoot.GunType.Single );
    } );

  }

  //飛彈
  public void playWeapon3() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    List<Transform> w3List = new List<Transform>();
    w3List.Add( transform.Find( "Armature/Bone.009/Bone.010/Bone.011/Bone.012/Leg.L/MissilePos/Missile1" ) );
    w3List.Add( transform.Find( "Armature/Bone.009/Bone.010/Bone.011/Bone.012/Leg.L/MissilePos/Missile2" ) );
    w3List.Add( transform.Find( "Armature/Bone.009/Bone.010/Bone.011/Bone.012/Leg.L/MissilePos/Missile3" ) );
    w3List.Add( transform.Find( "Armature/Bone.009/Bone.010/Bone.011/Bone.012/Leg.L/MissilePos/Missile4" ) );

    float missileRate = 0.2f;
    int loop = 1;
    int row = 2;
    float totalTime = (loop * (row * 2) - 1) * 0.2f + 1;

    myAnimation.Stop();
    myAnimation["weapon3"].speed = 1f;
    myAnimation.Play( "weapon3" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {

      playMissileFrom( w3List, missileRate, 10f, "missile", () => {
        switchCam();

        damageController.SetSide( opposSideDir, false );
        playMissileTo( 1.5f, new List<HitData>() { new HitData { WaitTime = 0, DelayTime = totalTime, Damage = attData.TotalDamage } }, loop, row );
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
        playMissileFrom( new List<Transform>() { transform.Find( "Armature/arm_ik.R/Bone.021/bazooka_bone/Bazooka/w4Pos" ) }, missileRate, 15f, "Bazooka", () => {
          switchCam();

          damageController.SetSide( opposSideDir, false );
          playMissileTo( 1.5f, new List<HitData>() { new HitData { WaitTime = 0, DelayTime = totalTime, Damage = attData.TotalDamage } }, loop: 1, row: 0, customPf: "Battle/Weapons/BazookaBall" );
        }, customPf: "Battle/Weapons/BazookaBall" );
      } );
    } );
  }

}
