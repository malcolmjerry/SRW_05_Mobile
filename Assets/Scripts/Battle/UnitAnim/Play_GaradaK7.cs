using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Play_GaradaK7 : UnitPlayWeaponBase {

  //public override float middleZ { get; set; } = 7;

  //private Transform sickleTf;
  private List<Transform> eyeMissileList = new List<Transform>();
  private GameObject sicklePf;

  void Start() {
    
  }

  public override void SetupWeapon() {
    //middlePoint = new Vector3( 0, 0, sideDir == "R" ? middleZ : -middleZ );
    //sickleTf = transform.Find( "Armature/Bone.009/Bone.027/Bone.028/Bone.016/Bone.017/saber_bone/Knife.R" );

    eyeMissileList.Add( transform.Find( "Armature/Bone.009/Bone.027/Bone.028/Bone.016/Bone.017/Head/Missile1" ) );
    eyeMissileList.Add( transform.Find( "Armature/Bone.009/Bone.027/Bone.028/Bone.016/Bone.017/Head/Missile2" ) );
    eyeMissileList.AddRange( eyeMissileList );
    sicklePf = Resources.Load( "Battle/Weapons/Sickle" ) as GameObject;
  }

  public override Transform GetTarget() {
    return transform.Find( "Armature/Bone.009/Bone.027/Bone.028/chest/Target" );
  }

  //眼部飛彈
  public void playWeapon1() {
    //var gunPos = transform.Find( "MachineGunPos" );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    float missileRate = 0.2f;
    int loop = 2;
    int row = 1;
    float totalTime = (loop * (row * 2) - 1) * 0.2f + 1;

    playMissileFrom( eyeMissileList, missileRate, 10f, "missile", () => {
      //CoroutineCommon.CallWaitForSeconds( 2f, () => {
      //Destroy( gunGo );
      switchCam();

      damageController.SetSide( opposSideDir, false );
      playMissileTo( 1.5f, new List<HitData>() { new HitData { WaitTime = 0, DelayTime = totalTime, Damage = attData.TotalDamage } }, loop, row );
      //} );
    } );

  }

  //鐮刀投擲
  public void playWeapon2() {
    //var gunPos = transform.Find( "MachineGunPos" );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    //damageController.StatusRect.SetActive( true );

    myAnimation.Stop();
    myAnimation["weapon2"].speed = 1f;
    myAnimation.Play( "weapon2" );

    damageController.PlayEn( attData.FromUseEN );

    CoroutineCommon.CallWaitForSeconds( 25.5f / 25f, () => {
      Transform sourceTf = transform.Find( "Armature/arm_ik.R/Bone.021/saber_active_bone/Knife.R.Flying" );
      var sickleGo = Instantiate( sicklePf );
      sickleGo.name = "SickleAttacking";
      sickleGo.transform.position = sourceTf.position;
      sickleGo.transform.SetParent( fightArea, true );
      sickleGo.transform.LookAt( opposPlay.GetTarget() );

      CoroutineCommon.CallWaitForSeconds( 1.5f, () => {
        switchCam();
        Destroy( sickleGo );
        damageController.SetSide( opposSideDir, false );
        playFlyAttackTo( new List<HitData>() { new HitData { WaitTime = 0f, DelayTime = 2f, Damage = attData.TotalDamage } } );
        //} );
      } );
    } );

  }

  protected void playFlyAttackTo( List<HitData> hitDataList ) {
    var sickleGo = Instantiate( sicklePf, fightArea );

    var midPos = new Vector3( -2f, 3.8f, middlePoint.z );
    sickleGo.name = "SickleAttackingTo";
    sickleGo.transform.localPosition = midPos;
    sickleGo.transform.LookAt( opposPlay.GetTarget() );
    sickleGo.GetComponent<FlyAttack>().Setup( opposPlay.GetTarget(), attData.CutType > 0,
      () => {
        //if (hitDataList != null && hitDataList.Count > 0) {
          playGetHitCommon( 0, false, hitDataList,
            () => {
              Destroy( sickleGo );
              BaseCallback();
            } );
        //}
      }
    );
    if (attData.CutType > 0 || attData.IsDodge) {
      playGetHitCommon( 1.5f, false, hitDataList, () => {
        Destroy( sickleGo );
        BaseCallback();
      } );
    }
  }

  //鐮刀斬擊
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
        EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );
        CoroutineCommon.CallWaitForAnimator( animator, "MeleeAttack2_" + sideDir, () => {
          animator.enabled = false;

          damageController.SetSide( opposSideDir, false );
          myAnimation.Play( "weapon3_2" );
          EffectSoundController.PLAY_1( (AudioClip)Resources.Load( $"SFX/Weapon/BeamSaberStart" ), 3 );

          playGetHitCommon( 4 / 25f, true, new List<HitData>() { new HitData { WaitTime = 0f, DelayTime = 1.5f, Damage = attData.TotalDamage, HitEffect = "SaberHit" } }, BaseCallback );
        } );

      } );

    } );

  }

}
