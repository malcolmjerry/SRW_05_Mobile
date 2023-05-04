using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System;

public class EnermyAI : MonoBehaviour {

  private StageManager stageManager;

  public void SetStageManager( StageManager stageManager ) {
    this.stageManager = stageManager;
  }

  private UnitInfo targetUnitInfo = null;
  private WeaponInfo fromWeaponInfo;

  private bool needMove = false;

  public void DoAction() {
    GameObject enermyFrom = stageManager.StageUnits_Enemy.FirstOrDefault( u => u.GetComponent<UnitController>().status == UnitController.UnitStatusEnum.READY );
    stageManager.mapManager.MoveCamToCenterObject( enermyFrom );

    if (enermyFrom.GetComponent<UnitInfo>().RemainMoveAttackTurns <= 0)
      nextAction( enermyFrom );
    else
      stayAction( enermyFrom );
  }

  private void nextAction( GameObject enermyFrom ) {
    //GameObject enermyFrom = stageManager.StageUnits_Enemy.FirstOrDefault( u => u.GetComponent<UnitController>().status == UnitController.UnitStatusEnum.READY );
    //stageManager.mapManager.MoveCamToCenterObject( enermyFrom );
    
    CoroutineCommon.CallWaitForSeconds( .5f, () => {

      var fromUnitInfo = enermyFrom.GetComponent<UnitInfo>();
      targetUnitInfo = null;
      needMove = false;

      if (!findUnitToAttack( fromUnitInfo ) || targetUnitInfo == null) {
        //沒有可用武器 或 找不到目標(地圖上沒有敵人)
        endActionNext( fromUnitInfo );
        /*
        CoroutineCommon.CallWaitForSeconds( .3f, () => {
          fromUnitInfo.GetComponent<UnitController>().EndAction();
          CoroutineCommon.CallWaitForSeconds( .3f, stageManager.HandlePhase );
        } );
        */
        return;
      }
      else if (needMove) {
        //嘗試移動
        fromUnitInfo.RobotInfo.IsMoved = true;
        fromUnitInfo.GetComponent<UnitAutoMove>().Setup(
          fromUnitInfo.GetComponent<CapsuleCollider>().radius,
          fromUnitInfo.GetComponent<UnitInfo>().MapFightingUnit.MovePower,
          fromUnitInfo.transform.position,
          targetUnitInfo.transform.position,
          () => {
            // 向目標移動後
            if (!findUnitToAttack( fromUnitInfo )) {
              endActionNext( fromUnitInfo );
              return;
            }
            doAttack( fromUnitInfo );
          }
        );
        fromUnitInfo.GetComponent<UnitAutoMove>().enabled = true;
      }
      else doAttack( fromUnitInfo );

    } );

  }

  private void stayAction( GameObject enermyFrom ) {
    //GameObject enermyFrom = stageManager.StageUnits_Enemy.FirstOrDefault( u => u.GetComponent<UnitController>().status == UnitController.UnitStatusEnum.READY );
    //stageManager.mapManager.MoveCamToCenterObject( enermyFrom );

    CoroutineCommon.CallWaitForSeconds( 0f, () => {

      var fromUnitInfo = enermyFrom.GetComponent<UnitInfo>();
      targetUnitInfo = null;
      needMove = false;

      if (!findUnitToAttack( fromUnitInfo ) || targetUnitInfo == null) {
        //沒有可用武器 或 找不到目標(地圖上沒有敵人)
        endActionNext( fromUnitInfo );
        return;
      }
      else if (needMove) {
        endActionNext( fromUnitInfo );
      }
      else doAttack( fromUnitInfo );
    } );

  }

  private void doAttack( UnitInfo fromUnitInfo ) {
    fromUnitInfo.RobotInfo.IsMoved = false;
    //stageManager.mapManager.GetComponent<MakeBattleManager>().SetupByAI( fromUnitInfo, targetUnitInfo, fromWeaponInfo, null, null );
    stageManager.CounterSummary.SetupByAI( fromUnitInfo, targetUnitInfo, fromWeaponInfo, null, null );
    return;
  }

  private bool findUnitToAttack( UnitInfo fromUnitInfo ) {
    RobotInfo fromRobot = fromUnitInfo.RobotInfo;
    var usableWeapons = fromUnitInfo.GetUseableWeapon();

    if (usableWeapons == null || usableWeapons.Count == 0) {   // 沒有可用武器
      return false;
    }

    int maxRange = usableWeapons.Max( w => w.MaxRange );   //從可用武器中找出最大射程
    int minRange = usableWeapons.Min( w => w.MinRange );   //從可用武器中找出最小射程

    //把射程內的敵機找出來
    List<UnitInfo> toUnitList = new List<UnitInfo>();
    if (fromUnitInfo.OtherUnit) {
      if (PreBattleFormula.IS_IN_RANGE( fromUnitInfo, fromUnitInfo.OtherUnit, maxRange, minRange )) {
        toUnitList.Add( fromUnitInfo.OtherUnit );
      }
    }
    else {
      foreach (var playerGo in stageManager.StageUnits_Player_OnMap) {
        if (playerGo == null)
          continue;

        var toUnitInfo = playerGo.GetComponent<UnitInfo>();

        if (PreBattleFormula.IS_IN_RANGE( fromUnitInfo, toUnitInfo, maxRange, minRange )) {
          toUnitList.Add( toUnitInfo );
        }
      }
    }

    if (toUnitList.Count == 0) {  //射程內沒有敵機
      if (fromUnitInfo.RobotInfo.IsMoved) {
        needMove = false;
        return false;
      }

      //從所有敵機中找出最具攻擊價值的
      //findMaxScore( fromUnitInfo, usableWeapons, stageManager.StageUnits_Player.Select( u => u.GetComponent<UnitInfo>() ).ToList(), false );
      //從所有敵機中找出距離最近的
      findNearest( fromUnitInfo, stageManager.StageUnits_Player_OnMap.Select( u => u.GetComponent<UnitInfo>() ).ToList() );

      needMove = true;
      return true;
    }

    //射程內有敵機, 計算敵機的攻擊價值
    findMaxScore( fromUnitInfo, usableWeapons, toUnitList/*stageManager.MapFightingUnits_Player*/, true );
    needMove = false;
    return true;
  }

  private void findMaxScore( UnitInfo fromUnitInfo, List<WeaponInfo> usableWeapons, List<UnitInfo> toUnitList, bool needInRange ) {
    float maxScore = 0;
    foreach (var toUnitInfo in toUnitList) {
      if (!toUnitInfo) continue;

      //var toUnitInfo = playerGo.GetComponent<UnitInfo>();

      foreach (var weaponInfo in usableWeapons) {
        if (needInRange && !PreBattleFormula.IS_IN_RANGE( fromUnitInfo, toUnitInfo, weaponInfo.MaxRange, weaponInfo.MinRange )) {
          continue;
        }

        //檢查是否可攻擊地形 (先不實裝)

        float score = PreBattleFormula.ATTACK_SCORE( fromUnitInfo, toUnitInfo, weaponInfo );
        if (score >= maxScore) {
          maxScore = score;
          targetUnitInfo = toUnitInfo;
          fromWeaponInfo = weaponInfo;
        }
      }
    }
  }

  private void findNearest( UnitInfo fromUnitInfo, List<UnitInfo> toUnitList ) {
    float minDistance = 9999;
    foreach (var toUnitInfo in toUnitList) {
      if (!toUnitInfo)
        continue;

      var distance = Vector3.Distance( toUnitInfo.transform.position, fromUnitInfo.transform.position );

      if (distance < minDistance) {
        minDistance = distance;
        targetUnitInfo = toUnitInfo;
      }
    }
  }

  void endActionNext( UnitInfo fromUnitInfo ) {
    CoroutineCommon.CallWaitForSeconds( .3f, () => {
      fromUnitInfo.GetComponent<UnitController>().EndAction();
      //CoroutineCommon.CallWaitForSeconds( .3f, stageManager.HandlePhase );
      CoroutineCommon.CallWaitForSeconds( .3f, stageManager.StageBase.DoEvent );
    } );
  }

}
