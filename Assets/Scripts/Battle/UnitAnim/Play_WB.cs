using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class Play_WB : UnitPlayWeaponBase {

  //public override float middleZ { get; set; } = 7;

  private Transform beamPos1;
  private Transform beamPos2;
  private Transform mainShoot;

  private List<Transform> mgList = new List<Transform>();
  private List<Transform> missileList = new List<Transform>();

  // Use this for initialization
  void Start() {
  }

  public override void SetupWeapon() {
    //middlePoint = new Vector3( 0, 0, sideDir == "R" ? middleZ : -middleZ );
    beamPos1 = transform.Find( "Armature/Bone/WB/shoot1" );
    beamPos2 = transform.Find( "Armature/Bone/WB/shoot2" );
    mainShoot = transform.Find( "Armature/Bone/WB/shoot3" );

    mgList.Add( transform.Find( "Armature/Bone/WB/mac1" ) );
    mgList.Add( transform.Find( "Armature/Bone/WB/mac2" ) );
    mgList.Add( transform.Find( "Armature/Bone/WB/mac3" ) );
    mgList.Add( transform.Find( "Armature/Bone/WB/mac4" ) );

    missileList.Add( transform.Find( "Armature/Bone/WB/missile1" ) );
    missileList.Add( transform.Find( "Armature/Bone/WB/missile2" ) );
    missileList.Add( transform.Find( "Armature/Bone/WB/missile3" ) );
    missileList.Add( transform.Find( "Armature/Bone/WB/missile4" ) );
    missileList.AddRange( missileList );
  }

  //機關炮
  public void playWeapon1() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    playMachineGunFrom( mgList );

    CoroutineCommon.CallWaitForSeconds( 3f, () => {
      stopMachineGunFrom();

      camRight.SetActive( !camRight.activeSelf );
      camLeft.SetActive( !camLeft.activeSelf );

      damageController.SetSide( opposSideDir, false );
      playBigMachineGunTo( mgList[1].position, new List<HitData>() { new HitData { WaitTime = 0.2f, DelayTime = 2f, Damage = attData.TotalDamage } }, MachineGunShoot.GunType.SHIP );
    } );

  }

  //飛彈
  public void playWeapon2() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    float missileRate = 0.2f;
    int loop = 2;
    int row = 2;
    float totalTime = (loop * (row * 2) - 1) * missileRate + 1;

    playMissileFrom( missileList, missileRate, 10f, "missile", () => {
      switchCam();
      damageController.SetSide( opposSideDir, false );
      playMissileTo( 1.5f, new List<HitData>() { new HitData { WaitTime = 0, DelayTime = totalTime, Damage = attData.TotalDamage } }, loop, row );
    } );

  }

  //2連裝米粒子炮
  public void playWeapon3() {
    geroBeamGO = Instantiate( geroBeamPF ) as GameObject;
    geroBeamGO.transform.SetParent( beamPos1 );
    geroBeamGO.transform.localPosition = new Vector3( 0, 0, 0 );
    geroBeamGO.transform.localRotation = Quaternion.Euler( new Vector3( 0, 0, 0 ) );
    geroBeamGO_2 = Instantiate( geroBeamPF ) as GameObject;
    geroBeamGO_2.transform.SetParent( beamPos2 );
    geroBeamGO_2.transform.localPosition = new Vector3( 0, 0, 0 );
    geroBeamGO_2.transform.localRotation = Quaternion.Euler( new Vector3( 0, 0, 0 ) );

    var beamParam = geroBeamGO.GetComponent<BeamParam>();
    beamParam.BeamColor = Color.yellow;
    beamParam.Scale = 2f;
    geroBeamGO.SetActive( false );
    var beamParam2 = geroBeamGO_2.GetComponent<BeamParam>();
    beamParam2.BeamColor = Color.yellow;
    beamParam2.Scale = 2f;
    geroBeamGO_2.SetActive( false );

    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    myAnimation.Stop();
    myAnimation["weapon2"].speed = 1f;
    myAnimation.Play( "weapon2" );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {

      damageController.PlayEn( attData.FromUseEN );

      CoroutineCommon.CallWaitForSeconds( .8f, () => {
        geroBeamGO.SetActive( true );
        geroBeamGO_2.SetActive( true );
        EffectSoundController.PLAY( (AudioClip)Resources.Load( "SFX/Weapon/MegaBeam" ), 3 );

        //炮1
        CoroutineCommon.CallWaitForSeconds( 1.5f, () => {
          geroBeamGO.GetComponent<GeroBeam>().SmoothDestroy( () => {
            geroBeamGO.SetActive( false );

            CoroutineCommon.CallWaitForSeconds( .3f, () => {
              switchCam();
              //myAnimation.Play( "Idle" );

              damageController.SetSide( opposSideDir, false );
              ShootBeam( beamPos1.position, Color.yellow, 2f, new List<HitData>() { new HitData { WaitTime = 0.2f, DelayTime = 1.5f, Damage = attData.TotalDamage } } );
            } );

          } );
        } );

        //炮2
        CoroutineCommon.CallWaitForSeconds( 1.5f, () => {
          geroBeamGO_2.GetComponent<GeroBeam>().SmoothDestroy( () => {
            geroBeamGO_2.SetActive( false );
            CoroutineCommon.CallWaitForSeconds( .3f, () => { ShootBeam( beamPos2.position, Color.yellow, 2f, null, 1.7f ); } );
          } );
        } );
      } );
    } );

  }

  //主炮
  public void playWeapon4() {
    battleCanvas.Find( "PilotImage" ).GetComponent<Image>().sprite = Resources.Load<Sprite>( "Character/" + UnitInfo.PilotInfo.PicNo + "_1" );
    battleCanvas.Find( "PilotName" ).GetComponent<Text>().text = UnitInfo.PilotInfo.ShortName;
    setDialog( attData.AtkDialogs );

    float missileRate = 0.2f;
    float totalTime = 1.2f;

    //myAnimation.Stop();
    //myAnimation["weapon2"].speed = 1f;
    //myAnimation.Play( "weapon2" );

    CoroutineCommon.CallWaitForAnimation( myAnimation, () => {
      CoroutineCommon.CallWaitForSeconds( 1f, () => {
        playMissileFrom( new List<Transform>() { mainShoot }, missileRate, 12f, "Bazooka", () => {
          switchCam();

          damageController.SetSide( opposSideDir, false );
          playMissileTo( 1.5f, new List<HitData>() { new HitData { WaitTime = 0, DelayTime = totalTime, Damage = attData.TotalDamage } }, loop: 1, row: 0, customPf: "Battle/Weapons/BazookaBall" );
        }, customPf: "Battle/Weapons/BazookaBall" );
      } );
    } );
  }



}
