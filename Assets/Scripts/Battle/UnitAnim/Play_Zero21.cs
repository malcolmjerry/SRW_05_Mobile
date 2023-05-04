using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Play_Zero21 : UnitPlayWeaponBase {

  private Transform riflePoint;

  // Use this for initialization
  void Start() {
  }

  private List<Transform> mgList = new List<Transform>();

  public override void SetupWeapon() {
    riflePoint = transform.Find( "Armature/Bone.009/Bone.008/Bone.007/Bone.015/Bone.018/Bone.019/Bone.020/Bone.021/rifle/Rifle/GunPos" );

    mgList.Add( transform.Find( "Armature/Bone/Zero2/w1Pos.L" ) );
    mgList.Add( transform.Find( "Armature/Bone/Zero2/w1Pos.R" ) );
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

    CoroutineCommon.CallWaitForSeconds( 3.5f, () => {
      stopMachineGunFrom();

      camRight.SetActive( !camRight.activeSelf );
      camLeft.SetActive( !camLeft.activeSelf );

      damageController.SetSide( opposSideDir, false );
      playBigMachineGunTo( mgList[1].position, new List<HitData>() { new HitData { WaitTime = 0.2f, DelayTime = 2f, Damage = attData.TotalDamage } }, MachineGunShoot.GunType.HEAD );
    } );

  }

  //空對地導彈
  public void playWeapon2() {
    List<Transform> w2List = new List<Transform>();
    w2List.Add( transform.Find( "Armature/Bone/Zero2/w2Pos.L" ) );
    w2List.Add( transform.Find( "Armature/Bone/Zero2/w2Pos.R" ) );

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

  //神風特攻
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

}
