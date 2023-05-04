using UnityEngine;
using System.Linq;
using System.Collections;

public class PreBattleFormula {

  public static int HIT_RATE( UnitInfo fromUnit, UnitInfo toUnit, WeaponInfo fromWeapon ) {
    //return 100 + (fromUnit.PilotInfo.Hit + fromUnit.RobotInfo.HitRate + fromWeapon.HitRate - toUnit.PilotInfo.Dodge - toUnit.RobotInfo.Motility);
    //攻方命中值
    int fromHit = fromUnit.PilotInfo.Hit + fromUnit.RobotInfo.HitRate + fromWeapon.HitRate;

    //守方回避值
    int toDodge = toUnit.PilotInfo.Dodge + toUnit.RobotInfo.Motility;

    int terrainBuff = (int)((TerrainHelper.GetTerrain( fromUnit.MapFightingUnit, fromUnit.Terrain ) - TerrainHelper.GetTerrain( toUnit.MapFightingUnit, toUnit.Terrain )) * 20);
    int finalHitRate = 100 + fromHit - toDodge + terrainBuff;

    //精神
    if (fromUnit.PilotInfo.ActiveSPCommandList.Any( s => s.ID == 7 )) finalHitRate += 30;   //攻擊方掛集中
    if (toUnit.PilotInfo.ActiveSPCommandList.Any( s => s.ID == 7 )) finalHitRate -= 30;     //防守方掛集中
    if (toUnit.PilotInfo.ActiveSPCommandList.Any( s => s.ID == 6 )) finalHitRate = 0;       //防守方掛必閃

    finalHitRate = Mathf.Max( Mathf.Min( finalHitRate, 100 ), 0 );
    return finalHitRate;
  }

  public static int HIT_RATE_FINAL( AttackData attData ) {
    UnitInfo fromUnit = attData.FromUnitInfo;
    UnitInfo toUnit = attData.ToUnitInfo;

    //攻方命中值
    int fromHit = fromUnit.PilotInfo.Hit + fromUnit.RobotInfo.HitRate + attData.WeaponInfo.HitRate;

    //守方回避值
    int toDodge = toUnit.PilotInfo.Dodge + toUnit.RobotInfo.Motility;

    //地形修正
    int terrainBuff = (int)((TerrainHelper.GetTerrain( fromUnit.MapFightingUnit, fromUnit.Terrain ) - TerrainHelper.GetTerrain( toUnit.MapFightingUnit, toUnit.Terrain )) * 20);

    //計算命中值
    int finalHitRate = 100 + fromHit - toDodge + terrainBuff + attData.HitRateAdd;
    finalHitRate = Mathf.Max( Mathf.Min( finalHitRate, 100 ), 0 );

    //修正最終命中率
    finalHitRate += attData.FinalHitRateAdd;
    finalHitRate = Mathf.Max( Mathf.Min( finalHitRate, 100 ), 0 );

    finalHitRate = (int)(finalHitRate * attData.HitRateMultiply);
    finalHitRate = Mathf.Max( Mathf.Min( finalHitRate, 100 ), 0 );

    return finalHitRate;
  }

  public static int CRI_RATE_FINAL( AttackData attData ) {
    UnitInfo fromUnit = attData.FromUnitInfo;
    UnitInfo toUnit = attData.ToUnitInfo;

    //攻方爆擊值
    int fromCri = fromUnit.PilotInfo.Dex + attData.WeaponInfo.CRI;

    //守方防爆擊值
    int toDex = toUnit.PilotInfo.Dex;

    //計算爆擊值
    int finalCriRate = fromCri + attData.CriAdd - toDex;
    finalCriRate = Mathf.Max( Mathf.Min( finalCriRate, 100 ), 0 );

    //修正最終爆擊值
    finalCriRate += attData.FinalCriAdd;
    finalCriRate = Mathf.Max( Mathf.Min( finalCriRate, 100 ), 0 );

    return finalCriRate;
  }

  //public static float DAMAGE_EXPECTED( AttackData attData ) {
  public static float DAMAGE_EXPECTED( UnitInfo fromUnit, UnitInfo toUnit, WeaponInfo weapon, float damageMultiply ) {
    //UnitInfo fromUnit = attData.FromUnitInfo;
    //UnitInfo toUnit = attData.ToUnitInfo;
    //WeaponInfo weapon = attData.WeaponInfo;

    float terrainRate = TerrainHelper.GetWeaponTerrainRate( weapon, fromUnit.Terrain, toUnit.Terrain );

    //武器攻擊力
    int attackPoint = weapon.WeaponInstance.Weapon.IsMelee ? fromUnit.PilotInfo.Melee : fromUnit.PilotInfo.Shoot;
    float fromDmg = (attackPoint + fromUnit.PilotInfo.Willpower) / 200f * weapon.HitPoint;

    //守方防禦力
    float toDef = (toUnit.PilotInfo.Defense + toUnit.PilotInfo.Willpower) / 200f * toUnit.RobotInfo.Armor;

    //基礎傷害值
    float damage = fromDmg - toDef;

    //計算乘算效果
    float result = damage /** attData.DamageRateAdd*/ * damageMultiply * terrainRate;
    
    return result;
  }

  public static readonly float BasicRadius = 1.8f;
  public static readonly float BasicRadiusMove = 2f;
  public static bool IS_IN_RANGE( UnitInfo fromUnit, UnitInfo toUnit, float maxRange, float minRange = 1 ) {
    float dist = Vector3.Distance( toUnit.transform.position, fromUnit.transform.position );
    bool insideMaxRound = IS_IN_ROUND( dist, fromUnit.RobotInfo.Radius, toUnit.RobotInfo.Radius, maxRange );
    bool insideMinRound = IS_IN_ROUND( dist, fromUnit.RobotInfo.Radius, toUnit.RobotInfo.Radius, minRange - 1 );
    return insideMaxRound && !insideMinRound;
  }

  private static bool IS_IN_ROUND( float distance, float radius1, float radius2, float range ) {
    float round = radius1 + 1f * BasicRadius * range;
    return distance < round + radius2 - 0.1;
  }

  public static float ATTACK_SCORE( UnitInfo fromUnit, UnitInfo toUnit, WeaponInfo fromWeapon ) {
    var hitRate = HIT_RATE( fromUnit, toUnit, fromWeapon );
    var expectedDamage = DAMAGE_EXPECTED( fromUnit, toUnit, fromWeapon, 1 ); //fromWeapon.HitPoint;
    var maxDamage = toUnit.RobotInfo.HP * 1.5f;
    var tooMuchDam1 = toUnit.RobotInfo.HP * 2f;
    var tooMuchDam2 = toUnit.RobotInfo.HP * 3f;

    var score = expectedDamage;
    if (expectedDamage >= toUnit.RobotInfo.HP) {
      if (expectedDamage < maxDamage) score *= 1.1f;
      else {
        score = maxDamage;
        if (expectedDamage >= tooMuchDam2) score *= 0.6f;
        else if( expectedDamage >= tooMuchDam1) score *= 0.7f;
      }
    }
    score *= hitRate;
    return score;
  }

  public static float ATTACK_SCORE_FINAL( AttackData attData ) {
    float score = attData.TotalDamage * attData.HitRate;
    return score;
  }

}
