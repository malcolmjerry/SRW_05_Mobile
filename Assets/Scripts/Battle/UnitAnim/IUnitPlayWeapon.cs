using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

public interface IUnitPlayWeapon {

  UnitInfo UnitInfo { get; set; }

  void PlayWeapon( AttackData attData, Action callBack );
  void PlayWeapon( WeaponInfo wpInfo, Action callBack );

  void PlayWeapon( int i, int totalDamge, int spendEn, bool hitMiss, List<PilotDialog> pdList, List<PilotDialog> opposPdList, Action callBack );

  void PlayDefeat( Action callBack );

  void PlayUnable( List<PilotDialog> pdList, Action callBack );

  void Setup( Transform fightArea, Transform modelSide, Transform opposModelSide, GameObject OpposModelGO, string sideDir, GameObject camRight, GameObject camLeft,
              Vector3 originPos, Quaternion originRot, DamageController damageController, Transform battleCanvas,
              UnitInfo unitInfo, UnitInfo opposUnitInfo,/*, List<string> dialogueList, List<string> opposDialogueList*/
              IUnitPlayWeapon opposPlay );

  void SetupWeapon();

  Transform GetTarget();

}
