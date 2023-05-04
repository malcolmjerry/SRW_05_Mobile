using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Play_MazingaZ : UnitPlayWeaponBase {

  // Use this for initialization
  void Start() {
  }

  private List<Transform> w1List = new List<Transform>();
  private List<Transform> w2List = new List<Transform>();
  private List<Transform> w5List = new List<Transform>();

  public override void SetupWeapon() {
  }

  //格鬥
  public void playWeapon1() {
    damageController.PlayEn( attData.FromUseEN );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    myAnimation.Play( "weapon1_1" );
    //EffectSoundController.PLAY_1( (AudioClip)Resources.Load( $"SFX/Weapon/BeamSaberHit" ), 2 );

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
          myAnimation.Play( "weapon1_2" );
          //EffectSoundController.PLAY_1( (AudioClip)Resources.Load( $"SFX/Weapon/BeamSaberStart" ), 3 );

          playGetHitCommon( 4 / 25f, true, new List<HitData>() { new HitData { WaitTime = 0f, DelayTime = 1.5f, Damage = attData.TotalDamage, HitEffect = "SaberHit" } }, BaseCallback );
        } );

      } );

    } );

  }

  //飛彈
  public void playWeapon2() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    float missileRate = 0.2f;
    int loop = 1;
    int row = 0;
    float totalTime = (loop * (row * 2) - 1) * 0.2f + 1;

    myAnimation.Stop();
    myAnimation["weapon2"].speed = 1f;
    myAnimation.Play( "weapon2" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {

      playMissileFrom( new List<Transform>() { transform.Find( "Armature/Bone.009/Bone.027/Waist/MissilePos" ) }, missileRate, 10f, "missile", () => {
        switchCam();

        damageController.SetSide( opposSideDir, false );
        playMissileTo( 1.5f, new List<HitData>() { new HitData { WaitTime = 0, DelayTime = totalTime, Damage = attData.TotalDamage } }, loop, row );
      } );

    } );

  }

  //火箭飛拳
  public void playWeapon3() {
    string rocketPunchPfPath = "Battle/Weapons/RocketPunch";

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    //myAnimation.Stop();
    //myAnimation["weapon3_1"].speed = 1f;
    myAnimation.Play( "weapon3_1" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    damageController.PlayEn( attData.FromUseEN );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
      var pos = transform.Find( "Armature/Bone.009/Bone.027/Bone.028/Bone.007/Bone.015/Bone.018/Bone.019/Bone.020/FrontHand.R/RocketPunchPos" );
      GameObject go = new GameObject();
      go.transform.position = pos.position;
      go.transform.rotation = pos.rotation;
      go.transform.parent = modelSide;

      myAnimation.Play( "weapon3_2" );

      CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
        playMissileFrom( new List<Transform>() { go.transform }, 0, 15f, "Missile", () => {
          switchCam();
          damageController.SetSide( opposSideDir, false );
          playMissileTo( 1.5f, new List<HitData>() { new HitData { WaitTime = 0, DelayTime = 1.2f, Damage = attData.TotalDamage } }, loop: 1, row: 0, customPf: rocketPunchPfPath, isCollider: false );
        }, customPf: rocketPunchPfPath, lookAtTarget: true );
      } );

    } );

  }

  //高熱火焰
  public void playWeapon4() {
    geroBeamGO = Instantiate( geroBeamPF );
    var shootPoint = transform.Find( "Armature/Bone.009/Bone.027/Bone.028/Chest/FirePos" );
    geroBeamGO.transform.SetParent( shootPoint );
    geroBeamGO.transform.localPosition = new Vector3( 0, 0, 0 );
    geroBeamGO.transform.localRotation = Quaternion.Euler( new Vector3( 0, 0, 0 ) );

    var beamParam = geroBeamGO.GetComponent<BeamParam>();
    beamParam.BeamColor = Color.red;
    beamParam.Scale = 4f;
    geroBeamGO.SetActive( false );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    //damageController.StatusRect.SetActive( true );

    myAnimation.Stop();
    //myAnimation["weapon4"].speed = 1f;
    myAnimation.Play( "weapon4" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {

      damageController.PlayEn( attData.FromUseEN );

      CoroutineCommon.CallWaitForSeconds( .5f, () => {
        geroBeamGO.SetActive( true );
        EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Weapon/BigCannon" ), 3 );

        CoroutineCommon.CallWaitForSeconds( 1.5f, () => {
          geroBeamGO.GetComponent<GeroBeam>().SmoothDestroy( () => {
            geroBeamGO.SetActive( false );

            CoroutineCommon.CallWaitForSeconds( .3f, () => {
              camRight.SetActive( !camRight.activeSelf );
              camLeft.SetActive( !camLeft.activeSelf );
              //myAnimation["weapon2"].time = 0f;
              //myAnimation["weapon2"].speed = 0f;
              //myAnimation.Play( "weapon2" );

              damageController.SetSide( opposSideDir, false );
              ShootBeam( shootPoint.position, Color.red, 4f, new List<HitData>() { new HitData { WaitTime = 0.2f, DelayTime = 1.5f, Damage = attData.TotalDamage } }, sfx: "BigCannon" );
            } );

          } );
        } );
      } );

    } );

  }

  //光子力射線
  public void playWeapon5() {
    float scale = 12f;
    geroBeamGO = Instantiate( geroBeamPF );
    geroBeamGO.name = "MazingaZ_Weapon5";
    CoroutineCommon.CallWaitForSeconds( 5f, () => geroBeamGO.SetActive( false ) );
    var shootPoint = transform.Find( "Armature/Bone.009/Bone.027/Bone.028/Bone.016/Bone.017/Head/IceBeamPos" );

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

    var currentCam = getCurrentCam();
    tempCam = Instantiate( currentCam.GetComponent<Camera>(), currentCam.transform.parent );
    startPos = tempCam.transform.localPosition;
    endPos = new Vector3( tempCam.transform.localPosition.x, tempCam.transform.localPosition.y, tempCam.transform.localPosition.z - 6f );
    startRotation = tempCam.transform.localRotation;
    endRotation = Quaternion.Euler( startRotation.eulerAngles.x, startRotation.eulerAngles.y - 50f, startRotation.eulerAngles.z );
    tempCamRotation = true;
    startTime = Time.time;
    fractionLen = 3f;
    enabled = true;

    //myAnimation.Stop();
    myAnimation["weapon5"].speed = 1f;
    myAnimation.Play( "weapon5" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {

      damageController.PlayEn( attData.FromUseEN );

      CoroutineCommon.CallWaitForSeconds( .5f, () => {
        geroBeamGO.SetActive( true );
        EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Weapon/MegaBeam" ), 3 );

        CoroutineCommon.CallWaitForSeconds( 2.5f, () => {
          geroBeamGO.GetComponent<GeroBeam>().SmoothDestroy( () => {
            geroBeamGO.SetActive( false );

            CoroutineCommon.CallWaitForSeconds( .3f, () => {
              camRight.SetActive( !camRight.activeSelf );
              camLeft.SetActive( !camLeft.activeSelf );
              enabled = false;
              damageController.SetSide( opposSideDir, false );
              ShootBeam( shootPoint.position, beamColor, scale, new List<HitData>() { new HitData { WaitTime = 0.2f, DelayTime = 2f, Damage = attData.TotalDamage } } );
            } );

          } );
        } );
      } );

    } );

  }

}
