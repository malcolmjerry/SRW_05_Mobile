using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Play_DevilRobot : UnitPlayWeaponBase {

  //public override float middleZ { get; set; } = 7;

  private Transform riflePoint;

  public override void SetupWeapon() {
    //middlePoint = new Vector3(0, 0, sideDir == "R" ? middleZ : -middleZ);
    riflePoint = transform.Find( "Armature/Hip/Waist/Chest/Chest 1/ChestCannonPos" );
    //mgList.Add(transform.Find("Armature/Bone.009/Bone.008/Bone.016/Bone.017/Head/headGun.L"));
    //mgList.Add(transform.Find("Armature/Bone.009/Bone.008/Bone.016/Bone.017/Head/headGun.R"));
  }

  //電漿炮
  public void playWeapon1() {
    battleCanvas.Find("PilotImage").GetComponent<Image>().sprite = Resources.Load<Sprite>("Character/" + UnitInfo.PilotInfo.PicNo + "_1");
    battleCanvas.Find("PilotName").GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog(attData.AtkDialogs);

    float missileRate = 0f;
    float totalTime = 1.2f;

    myAnimation.Stop();
    myAnimation["weapon1"].speed = 1f;
    myAnimation.Play("weapon1");
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    //Debug.Log( "camRight.transform: " + camRight.transform.localPosition);
    tempCam = Instantiate(camRight.GetComponent<Camera>(), camRight.transform.parent);
    startPos = tempCam.transform.localPosition;
    endPos = new Vector3(tempCam.transform.localPosition.x, tempCam.transform.localPosition.y + 0f, tempCam.transform.localPosition.z);
    startTime = Time.time;
    fractionLen = .7f;
    enabled = true;
    //Debug.Log("startPos: " + startPos);
    //Debug.Log("endPos: " + endPos);

    CoroutineCommon.CallWaitForAnimation(myAnimation, () => {
      fightArea.GetComponent<FightAnimController>().Stop = true;
      CoroutineCommon.CallWaitForSeconds(0.1f, () => {
        //EffectSoundController.PLAY( (AudioClip)Resources.Load( $"SFX/Weapon/BeamRifle2" ), 5 );
        playMissileFrom(new List<Transform>() { riflePoint }, missileRate, 8f, "BeamReflect", () => {
          switchCam();
          //fightArea.GetComponent<FightAnimController>().Stop = false;
          Destroy(tempCam.gameObject);

          damageController.SetSide(opposSideDir, false);
          playMissileTo(.8f, new List<HitData>() { new HitData { WaitTime = 0, DelayTime = totalTime, Damage = attData.TotalDamage } },
            loop: 1, row: 0, trail: true, speed: 30, isCollider: false, show: false, missileSfx: "BeamReflect", trailTime: .35f, width: .8f, trailName: "PlasmaTrail");
        },
        true,    //Trail
        false,   //Show
        .3f,     //After wait
        .35f, .8f, "PlasmaTrail" );     //Trail Time, width, Trail Name
      });
    });
  }

  //重斬擊
  public void playWeapon2() {
    damageController.PlayEn( attData.FromUseEN );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    myAnimation.Play( "weapon2_1" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {

      animator.enabled = true;
      animator.Play( "MeleeAttack_" + sideDir );

      CoroutineCommon.CallWaitForAnimator( animator, "MeleeAttack_" + sideDir, () => {
        switchCam();

        animator.Play( "MeleeAttack2_" + sideDir );
        EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );
        CoroutineCommon.CallWaitForAnimator( animator, "MeleeAttack2_" + sideDir, () => {
          animator.enabled = false;

          damageController.SetSide( opposSideDir, false );
          myAnimation.Play( "weapon2_2" );

          playGetHitCommon( 4 / 25f, true, new List<HitData>() { 
            new HitData { WaitTime = 0, DelayTime = 25/25f, Damage = attData.TotalDamage / 2, HitEffect = "SaberHit" },
            new HitData { WaitTime = 13/25f, DelayTime = 1f, Damage = attData.TotalDamage, HitEffect = "SaberHit" } },
            BaseCallback 
          );
        } );

      } );

    } );

  }

}
