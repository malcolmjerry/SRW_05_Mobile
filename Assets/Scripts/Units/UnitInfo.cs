using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using static TerrainHelper;
using UnityEngine.UI;
using UnityEngine.AI;

public class UnitInfo : MonoBehaviour {

  public enum TeamEnum { Enermy = 0, Player, NPC_Friend, NPC_Yellow }

  //public int Team;  // 0 敵方(紅)  1 味方(藍)  2 味方 NPC(紫)  3 中立 (黃)

  /// <summary> 地圖結束後, 加入我方 (放入 HouseUnit) </summary>
  public bool IsNewToPlayer;

  public bool IsDestroyed;

  public bool IsOnShip;

  public TeamEnum Team;

  public int RemainMoveAttackTurns;

  public MapFightingUnit MapFightingUnit;

  public RobotInfo RobotInfo { get { return MapFightingUnit.RobotInfo; } }

  public PilotInfo PilotInfo { get { return MapFightingUnit.PilotInfo; } }

  //所處地形   1 空 2 陸  3 海  宇
  public TerrainEnum Terrain;

  //public bool Ready { set; get; }

  public UnitInfo OtherUnit = null; //挑發專用

  public UnitInfo ShipUnit = null; //鄰近的戰艦

  public List<UnitInfo> StoringUnits = null;  //戰艦中的機體

  public List<int> DropParts = new List<int>();  //被擊墜時 (HP=0) 掉落的零件

  public List<WeaponInfo> GetUseableWeapon() {
    /*
    var usableWeapons = MapFightingUnit.WeaponList.Where(
       w => (w.EN < RobotInfo.EN || w.RemainBullets > 0) && (w.WillPower <= 0 || w.WillPower <= PilotInfo.Willpower)
    ).ToList();
    */
    var usableWeapons = MapFightingUnit.WeaponList.Where( w => w.IsUsable ).ToList();

    if (RobotInfo.IsMoved)
      usableWeapons = usableWeapons.Where( w => w.IsMove == true ).ToList();

    return usableWeapons;
  }
  
  public void Setup( TeamEnum team, MapFightingUnit mapFightingUnit, int remainMoveAttackTurns = 0, List<int> dropParts = null, bool isNewToPlayer = false ) {
    Team = team;
    RemainMoveAttackTurns = remainMoveAttackTurns;
    MapFightingUnit = mapFightingUnit;
    IsNewToPlayer = isNewToPlayer;
    if (dropParts != null)
      DropParts = dropParts;
    Transform canvasTransform = transform.Find( "Canvas" );
    canvasTransform.localScale = new Vector3( RobotInfo.Radius, RobotInfo.Radius, 1 );

    canvasTransform.Find( "HealthSlider/Fill Area/Fill" ).GetComponent<Image>().color = getColorByTeam( team, 255 );

    GetComponent<CapsuleCollider>().radius = RobotInfo.Radius;
    //GetComponent<CapsuleCollider>().height *= RobotInfo.Radius;  //效能問題
    GetComponent<CapsuleCollider>().height = 4;
    GetComponent<CapsuleCollider>().center = new Vector3( 0, RobotInfo.Radius, 0 );  //很重要, 不能讓物理碰撞器接觸到地面, 否則會很卡
    GetComponent<NavMeshAgent>().radius = RobotInfo.Radius;
    GetComponent<NavMeshAgent>().agentTypeID = GetMeshAgentTypeId();
    GetComponent<NavMeshObstacle>().radius = RobotInfo.Radius;

    if (RobotInfo.RobotInstance.Robot.IsShip == true) {
      var ship = transform.Find( "Ship" );
      ship.gameObject.SetActive( true );
      ship.GetComponent<CapsuleCollider>().radius = RobotInfo.Radius + 0.5f;
      StoringUnits = new List<UnitInfo>();
    }

    return;
  }

  private Color getColorByTeam( TeamEnum teamEnum, byte alpha = 200 ) {
    switch (teamEnum) {
      // 0 敵方( 紅 )  1 味方( 藍 )  2 味方 NPC( 紫)  3 中立( 黃 )
      case TeamEnum.Enermy: return new Color32( 255, 0, 0, alpha );
      case TeamEnum.Player: return new Color32( 77, 128, 230, alpha );
      case TeamEnum.NPC_Friend: return new Color32( 139, 0, 255, alpha );
      case TeamEnum.NPC_Yellow: return new Color32( 255, 255, 0, alpha );
      default: return new Color32( 0, 0, 255, 200 );
    }
  }

  public WeaponInfo FindWeaponCounter( UnitInfo fromUnitInfo ) {
    WeaponInfo lastWeaponInfo = null;
    RobotInfo fromRobot = fromUnitInfo.RobotInfo;

    var usableWeapons = MapFightingUnit.WeaponList.Where( w => w.IsUsable ).ToList();

    //先把射程內的武器找出來
    List<WeaponInfo> inRangeWeapons = new List<WeaponInfo>();

    foreach (var wInfo in usableWeapons) {
      if (PreBattleFormula.IS_IN_RANGE( this, fromUnitInfo, wInfo.MaxRange, wInfo.MinRange )) {
        inRangeWeapons.Add( wInfo );
      }
    }

    if (inRangeWeapons.Count == 0) {   // 沒有可用武器
      return null;
    }

    //計算武器的攻擊價值
    float maxScore = 0;

    //foreach (var weaponInfo in fromUnitInfo.CurrentRobotInfo.WeaponList) {
    foreach (var wInfo in inRangeWeapons.OrderByDescending( w => w.HitPoint )) {
      //float score = PreBattleFormula.ATTACK_SCORE( this, fromUnitInfo, wInfo );
      var attData = new AttackData( this, wInfo, fromUnitInfo, true, AttackData.AttackTypeEnum.Normal, AttackData.CounterTypeEnum.Normal );
      float score = attData.ExpectedDamage * attData.HitRate * (1 + (0.25f * attData.CriRate));

      if (score >= maxScore) {
        maxScore = score;
        lastWeaponInfo = wInfo;
      }
    }


    return lastWeaponInfo;
  }

  public int GetMeshAgentTypeId() {
    var sizeIndex = RobotInfo.RobotInstance.Robot.Size - 1;
    return NavMesh.GetSettingsByIndex( sizeIndex ).agentTypeID;
  }

  /*
  public void Normalize() {
    RobotInfo.Update();
    PilotInfo.Update();
  }*/

}
