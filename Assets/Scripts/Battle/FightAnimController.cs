using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public class FightAnimController : MonoBehaviour {

  Vector3 moveDir = new Vector3( 0, 0, 1 );
  public float defaultSpeed = 12;
  public float Speed;
  public bool Stop = false;

  GameObject fightArea, camR, camL;
  Transform rightSide, leftSide;
  IUnitPlayWeapon unitPlayR, unitPlayL;
  GameObject cloneR, cloneL;
  //UnitInfo unitInfoR, unitInfoL;

  Transform battleCanvas;
  //Text talking1;

  DamageController damageController;

  // Use this for initialization
  void Start () {
    fightArea = GameObject.Find( "FightArea" );
    rightSide = transform.Find( "RightSide" );
    leftSide = transform.Find( "LeftSide" );
    camR = transform.Find( "Cam_R" ).gameObject;
    camL = transform.Find( "Cam_L" ).gameObject;
    battleCanvas = GameObject.Find( "BattleCanvas" ).transform;
    //talking1 = GameObject.Find( "BattleCanvas" ).transform.FindChild( "Talking1" ).GetComponent<Text>();

    //damageController = GameObject.Find( "Canvas" ).transform.Find( "DamageTxt" ).GetComponent<DamageController>();//.PlayAttack();
    //damageController.gameObject.SetActive( false );
    //var battleCanvas = GameObject.Find( "BattleCanvas" );
    //Debug.Log( battleCanvas );
    //var damageTxt = battleCanvas.transform.Find( "DamageTxt" );
    //Debug.Log( damageTxt );
    //damageController = damageTxt.GetComponent<DamageController>();
    damageController = battleCanvas.GetComponent<DamageController>();
    //Debug.Log( damageController );
  }
	
  public void Setup( UnitInfo unitInfoR, UnitInfo unitInfoL ) {
    string modelNameR = unitInfoR.RobotInfo.RobotInstance.Robot.Name;
    cloneR = CreateClone( modelNameR, cloneR, rightSide );
    unitPlayR = (IUnitPlayWeapon)cloneR.GetComponent( "Play_" + modelNameR );

    string modelNameL = unitInfoL.RobotInfo.RobotInstance.Robot.Name;
    cloneL = CreateClone( modelNameL, cloneL, leftSide );
    unitPlayL = (IUnitPlayWeapon)cloneL.GetComponent( "Play_" + modelNameL );

    unitPlayR.Setup( fightArea.transform, rightSide, leftSide, cloneL, "R", camR, camL, rightSide.localPosition, rightSide.localRotation, damageController, battleCanvas,
                     unitInfoR, unitInfoL, unitPlayL /*, tempDialogueListR, tempDialogueListL*/ );
    unitPlayL.Setup( fightArea.transform, leftSide, rightSide, cloneR, "L", camR, camL, leftSide.localPosition, leftSide.localRotation, damageController, battleCanvas,
                     unitInfoL, unitInfoR, unitPlayR /*, tempDialogueListL, tempDialogueListR*/ );
  }

  GameObject CreateClone( string modelName, GameObject oldClone, Transform theSide ) {
    var prefab = Resources.Load( "Battle/Units/" + modelName ) as GameObject;
    Destroy( oldClone );
    var clone = Instantiate( prefab ) as GameObject;
    clone.transform.parent = theSide;
    clone.transform.localPosition = new Vector3( 0, 0, 0 );
    clone.transform.localRotation = prefab.transform.rotation;
    clone.AddComponent<MyShake>().enabled = false;
    clone.layer = LayerMask.NameToLayer( "Hitable" );
    return clone;
  }

  public void PlayBattle( AttackData attData, Action callBack ) {
    if (attData.FromRight) {   //From right
      Setup( attData.FromUnitInfo, attData.ToUnitInfo );
      damageController.SetupFromRight( attData );
      camL.SetActive( false );
      camR.SetActive( true );
      damageController.SetSide( "R", true );
      Speed = defaultSpeed;

      if (attData.AttackType == AttackData.AttackTypeEnum.Unable)
        unitPlayR.PlayUnable( attData.AtkDialogs, callBack );
      else if (attData.AttackType == AttackData.AttackTypeEnum.Quit)
        callBack();
      else
        unitPlayR.PlayWeapon( attData, () => {
          if (attData.IsDefeated) {
            Speed = 0;
            unitPlayL.PlayDefeat( callBack );
          }
          else callBack();
        } );
        /*
        unitPlayR.PlayWeapon( attData.WeaponInfo.WeaponInstance.Weapon.PlayIndex, attData.TotalDamage, attData.FromUseEN, !attData.IsDodge,
          attData.AtkDialogs, attData.DefDialogs,
          () => {
            if (attData.IsDefeated) {
              speed = 0;
              unitPlayL.PlayDefeat( callBack );
            }
            else callBack();
          } 
        );*/
    }
    else {     //From Left
      Setup( attData.ToUnitInfo, attData.FromUnitInfo );
      damageController.SetupFromLeft( attData );
      camL.SetActive( true );
      camR.SetActive( false );

      damageController.SetSide( "L", true );
      Speed = -defaultSpeed;

      if (attData.AttackType == AttackData.AttackTypeEnum.Unable) 
        unitPlayL.PlayUnable( attData.AtkDialogs, callBack );
      else if (attData.AttackType == AttackData.AttackTypeEnum.Quit)
        callBack();
      else
        unitPlayL.PlayWeapon( attData,
          () => {
            if (attData.IsDefeated) {
              Speed = 0;
              unitPlayR.PlayDefeat( callBack );
            }
            else callBack();
          }
        );
      /*
      unitPlayL.PlayWeapon( attData.WeaponInfo.WeaponInstance.Weapon.PlayIndex, attData.TotalDamage, attData.FromUseEN, !attData.IsDodge,
        attData.AtkDialogs, attData.DefDialogs,
        () => {
          if (attData.IsDefeated) {
            speed = 0;
            unitPlayR.PlayDefeat( callBack );
          }
          else callBack();
        } 
      );*/
    }
  }

  /*
  public void PlayBattle_old( int weapon, string side, int totalDamage, int spendEn, bool hitMiss, Action callBack ) {
    switch (side) {
      case "R":
        camL.SetActive( false );
        camR.SetActive( true );
        unitPlayR.PlayWeapon( weapon, totalDamage, spendEn, hitMiss, callBack );
        damageController.SetSide( "R", true );
        speed = weapon > 0 ? 12 : 0;
        break;
      case "L":
        camL.SetActive( true );
        camR.SetActive( false );
        unitPlayL.PlayWeapon( weapon, totalDamage, spendEn, hitMiss, callBack );
        damageController.SetSide( "L", true );
        speed = weapon > 0 ? -12 : 0;
        break;
    }
  }*/

  // Update is called once per frame
  void Update () {
    if (Speed != 0 && !Stop)
      MoveArea();
  }

  private void FixedUpdate() {
    //Turn();
    //MoveCamera();
  }

  private void MoveArea() {
    var direction = moveDir * Speed * Time.deltaTime;
    transform.Translate( direction );
    boundary();
  }

  /*
  private void boundary() {
    float leftLimit = 50, rightLimit = -50;

    if (transform.Find( "Cam_R" ).gameObject.activeSelf) {
      leftLimit = 80;
      rightLimit = -30;
    } else if (transform.Find( "Cam_L" ).gameObject.activeSelf) {
      leftLimit = 30;
      rightLimit = -80;
    }

    if (transform.position.z >= leftLimit) {
      //Debug.Log( "Left Limit: " + leftLimit );
      transform.position = new Vector3( transform.position.x, transform.position.y, rightLimit );
    }
    else if (transform.position.z <= rightLimit) {
      //Debug.Log( "Right Limit: " + rightLimit );
      transform.position = new Vector3( transform.position.x, transform.position.y, leftLimit );
    }
  }*/

  private void boundary() {
    float leftLimit = 50, rightLimit = -50;

    if (transform.Find( "Cam_R" ).gameObject.activeSelf) {
      leftLimit = 370;
      rightLimit = -30;
    }
    else if (transform.Find( "Cam_L" ).gameObject.activeSelf) {
      leftLimit = 320;
      rightLimit = -80;
    }

    if (transform.position.z > leftLimit) {
      //Debug.Log( "Left Limit: " + leftLimit );
      transform.position = new Vector3( transform.position.x, transform.position.y, rightLimit );
    }
    else if (transform.position.z < rightLimit) {
      //Debug.Log( "Right Limit: " + rightLimit );
      transform.position = new Vector3( transform.position.x, transform.position.y, leftLimit );
    }
  }

  /*
  public void StopAllThing() {
    StopAllCoroutines();
    unitPlayRR.StopAllCoroutines();
    unitPlayLL.StopAllCoroutines();
  }*/

}
