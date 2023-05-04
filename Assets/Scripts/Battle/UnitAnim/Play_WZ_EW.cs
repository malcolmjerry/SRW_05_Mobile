using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class Play_WZ_EW : UnitPlayWeaponBase {

  //public override float middleZ { get; set; } = 7;

  private Transform riflePoint;
  private Transform riflePointLeft;
  
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

  public override void SetupWeapon() {
    //middlePoint = new Vector3( 0, 0, sideDir == "R" ? middleZ : -middleZ );
    riflePoint     = transform.Find( "Armature/Bone.002/Bone.023/Bone.024/Bone.025/Bone.026/Bone.027/RightFinger/RightRifle/RiflePoint" );
    riflePointLeft = transform.Find( "Armature/Bone.002/Bone.010/Bone.014/Bone.015/Bone.016/Bone.019/LeftFinger/LeftRifle/RiflePointLeft" );
  }

  /*
  //StartCoroutine( unitPlay.PlayWeapon( 1 ) );
  public void PlayWeapon( int i, int totalDamge, Action callBack ) {
    this.callBack = callBack;
    var method = this.GetType().GetMethod( "playWeapon" + i );
    var arguments = new object[] { totalDamge };
    method.Invoke( this, arguments );
    //method.Invoke( this, null );
  }*/

  //格鬥
  public void playWeapon1() {

    //CoroutineCommon.CallWaitForSeconds( 2, () => {
    int dam1 = (int)(attData.TotalDamage * 0.3f);
    int dam2 = (int)(attData.TotalDamage * 0.8f);
    //int dam3 = totolDamage - dam1 - dam2;

    damageController.PlayEn( attData.FromUseEN );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    animator.enabled = true;
    animator.Play( "MeleeAttack_" + sideDir );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );

    CoroutineCommon.CallWaitForAnimator( animator, "MeleeAttack_" + sideDir, () => {
      camRight.SetActive( !camRight.activeSelf );
      camLeft.SetActive( !camLeft.activeSelf );

      animator.Play( "MeleeAttack2_" + sideDir );
      EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Booster" ), 1.5f );
      CoroutineCommon.CallWaitForAnimator( animator, "MeleeAttack2_" + sideDir, () => {
        //animator.Stop();
        animator.enabled = false;

        damageController.SetSide( opposSideDir, false ); //新追加
                                                         //Debug.Log( myAnimation["weapon1"].clip.frameRate );
        myAnimation.Play( "weapon1" );

        playGetHitCommon( 13 / 25f, true, new List<HitData>() {
            new HitData { WaitTime = 0f,   DelayTime = 7.5f/25, Damage = dam1 },
            new HitData { WaitTime = 17.5f/25, DelayTime = 13/25f,  Damage = dam2 },
            new HitData { WaitTime = 5f/25,    DelayTime = 7.5f /25, Damage = attData.TotalDamage } },
          BaseCallback
        );
        /*
        playGetHit( 1, 0.5f );

        playShake( new List<HitData>() { new HitData { WaitTime = 13/25f,   DelayTime = 7.5f/25, Damage = dam1 },
                         new HitData { WaitTime = 17.5f/25, DelayTime = 13/25f,  Damage = dam2 },
                         new HitData { WaitTime = 5f/25,    DelayTime = 7.5f /25, Damage = attData.TotalDamage } },
                  () => {
                    playGetHit( -1, 0 );

                    StartCoroutine( BackToOrignPos( middlePoint, () => {
                      BaseCallback();
                    } ) );
                  }
        );
        */
        /*
        CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
          playDefense( -1 );

          StartCoroutine( BackToOrignPos( middlePoint, () => {
            //myAnimation.Play( "weapon1" );
            callBack();
          } ) );
        } );
        */
      } );
    } );
  }

  //光束破壞步槍
  public void playWeapon2() {
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
              camRight.SetActive( !camRight.activeSelf );
              camLeft.SetActive( !camLeft.activeSelf );
              myAnimation["weapon2"].time = 0f;
              myAnimation["weapon2"].speed = 0f;
              myAnimation.Play( "weapon2" );

              damageController.SetSide( opposSideDir, false );
              ShootBeam( riflePoint.position, Color.yellow, 3f, new List<HitData>() { new HitData { WaitTime = 0.2f, DelayTime = 1.5f, Damage = attData.TotalDamage } } );
            } );

          } );
        } );
      } );

    } );

  }


  //雙管光束破壞步槍
  public void playWeapon4() {
    float scale = 6f;
    geroBeamGO = Instantiate( geroBeamPF ) as GameObject;
    geroBeamGO.transform.SetParent( riflePoint );
    geroBeamGO.transform.localPosition = new Vector3( 0, 0, 0 );
    geroBeamGO.transform.localRotation = Quaternion.Euler( new Vector3( 0, 0, 0 ) );

    var beamParam = geroBeamGO.GetComponent<BeamParam>();
    beamParam.BeamColor = Color.yellow;
    beamParam.Scale = scale;
    geroBeamGO.SetActive( false );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    //damageController.StatusRect.SetActive( true );

    myAnimation.Stop();
    myAnimation["weapon4"].speed = 1f;
    myAnimation.Play( "weapon4" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/readyMove" ), 5 );

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
              myAnimation["weapon4"].time = 0f;
              myAnimation["weapon4"].speed = 0f;
              myAnimation.Play( "weapon4" );

              damageController.SetSide( opposSideDir, false );
              ShootBeam( riflePoint.position, Color.yellow, scale, new List<HitData>() { new HitData { WaitTime = 0.2f, DelayTime = 2f, Damage = attData.TotalDamage } } );
            } );

          } );
        } );
      } );

    } );

  }

  private GameObject createBeamGo( Transform parentTransform, float scale ) {
    var beamGo = Instantiate( geroBeamPF );
    beamGo.transform.SetParent( parentTransform );
    beamGo.transform.localPosition = new Vector3( 0, 0, 0 );
    //beamGo.transform.localRotation = parentTransform.localRotation;
    beamGo.transform.rotation = parentTransform.rotation;//Quaternion.Euler( new Vector3( 0, 0, 0 ) );
    var beamParam = beamGo.GetComponent<BeamParam>();
    beamParam.BeamColor = Color.yellow;
    beamParam.Scale = scale;
    beamParam.AnimationSpd = 0.4f;
    beamGo.SetActive( false );
    return beamGo;
  }

  //旋轉炮 MapW
  public void playWeapon5() {
    geroBeamGO = createBeamGo( riflePoint, 8f );
    geroBeamGO_2 = createBeamGo( riflePointLeft, 8f );
    geroBeamGO.GetComponent<GeroBeam>().HitEffect = geroBeamGO_2.GetComponent<GeroBeam>().HitEffect = null;

    BGMController.SET_BGM( UnitInfo.RobotInfo.RobotInstance.BGM );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/readyMove" ), 5 );
    CoroutineCommon.CallWaitForOneFrame( () => {
      //damageController.StatusRect.SetActive( true );

      myAnimation.Stop();
      myAnimation["weapon5"].speed = 1f;
      myAnimation.Play( "weapon5" );

      CoroutineCommon.CallWaitForAnimation( myAnimation, () => {

        CoroutineCommon.CallWaitForSeconds( .5f, () => {
          geroBeamGO.SetActive( true );
          geroBeamGO_2.SetActive( true );
          EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Weapon/MegaBeam" ), 3 );
          
          CoroutineCommon.CallWaitForSeconds( 2f, () => {
            geroBeamGO.GetComponent<GeroBeam>().SmoothDestroy( () => {
              geroBeamGO.SetActive( false );
            } );
            geroBeamGO_2.GetComponent<GeroBeam>().SmoothDestroy( () => {
              geroBeamGO_2.SetActive( false );
            } );
          } );

          //自身旋轉
          CoroutineCommon.CallWaitForSeconds( 0f, () => {
            myAnimation["weapon5_2"].speed = 1f;
            myAnimation.Play( "weapon5_2" );
            CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
              myAnimation["weapon5"].time = 0f;
              myAnimation["weapon5"].speed = 0f;
              myAnimation.Play( "weapon5" );

              callBack();
            } );

          } );
          //自身旋轉
          
        } );

      } );

    } );
  }

  //雙管光束破壞步槍(全出力)
  public void playWeapon6() {
    float scale = 16f;
    geroBeamGO = Instantiate( geroBeamPF ) as GameObject;
    geroBeamGO.transform.SetParent( riflePoint );
    geroBeamGO.transform.localPosition = new Vector3( 0, 0, 0 );
    geroBeamGO.transform.localRotation = Quaternion.Euler( new Vector3( 0, 0, 0 ) );

    var beamParam = geroBeamGO.GetComponent<BeamParam>();
    beamParam.BeamColor = Color.yellow;
    beamParam.Scale = scale;
    geroBeamGO.SetActive( false );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    Camera tempCam = Instantiate( camRight.GetComponent<Camera>(), camRight.transform.parent );
    tempCam.transform.localPosition = new Vector3( -3.32f, 4.62f, -17.97f );
    tempCam.transform.localRotation = Quaternion.Euler( 17.04f, 120.577f, -2.571f );
    tempCam.gameObject.SetActive( false );

    /*
    Camera tempCam = Camera.Instantiate( camRight.GetComponent<Camera>(), new Vector3( -3.32f, 4.62f, -17.97f ),
                                         Quaternion.Euler( 17.04f, 120.577f, -2.571f ),
                                         camRight.transform.parent );*/

    //damageController.StatusRect.SetActive( true );

    myAnimation.Stop();
    myAnimation["weapon4"].speed = 1f;
    myAnimation.Play( "weapon4" );
    EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/readyMove" ), 3 );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
      tempCam.gameObject.SetActive( true );

      damageController.PlayEn( attData.FromUseEN );

      CoroutineCommon.CallWaitForSeconds( .5f, () => {
        geroBeamGO.SetActive( true );
        EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Weapon/MegaBeam" ), 2 );

        CoroutineCommon.CallWaitForSeconds( 2f, () => {
          geroBeamGO.GetComponent<GeroBeam>().SmoothDestroy( () => {
            geroBeamGO.SetActive( false );

            CoroutineCommon.CallWaitForSeconds( .3f, () => {
              Destroy( tempCam.gameObject );
              camRight.SetActive( !camRight.activeSelf );
              camLeft.SetActive( !camLeft.activeSelf );
              myAnimation["weapon4"].time = 0f;
              myAnimation["weapon4"].speed = 0f;
              myAnimation.Play( "weapon4" );

              damageController.SetSide( opposSideDir, false );
              ShootBeam( riflePoint.position, Color.yellow, scale, new List<HitData>() { new HitData { WaitTime = 0.2f, DelayTime = 2f, Damage = attData.TotalDamage } } );
            } );

          } );
        } );
      } );

    } );

  }

  //胸部火神炮
  public void playWeapon3() {
    var gunPos = transform.Find( "MachineGunPos" );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    var gunGo = playBigMachineGunFrom( gunPos );

    CoroutineCommon.CallWaitForSeconds( 2f, () => {
      Destroy( gunGo );

      camRight.SetActive( !camRight.activeSelf );
      camLeft.SetActive( !camLeft.activeSelf );

      damageController.SetSide( opposSideDir, false );
      playBigMachineGunTo( gunPos.position, new List<HitData>() { new HitData { WaitTime = 0.2f, DelayTime = 2f, Damage = attData.TotalDamage } } );
    } );

  }

}
