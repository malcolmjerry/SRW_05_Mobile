
public class TerrainHelper {


  public enum TerrainEnum { Land, Sky, Sea, Space };

  private static readonly string[] TERRAIN_RANK_ARRAY = { "E", "D", "C", "B", "A", "S" };
  public static string GET_TerrainRank( float rank ) {
    int rankInt = (int)rank;
    string plus = (rank-rankInt) > 0 ? "+" : "";
    return TERRAIN_RANK_ARRAY[rankInt] + plus;
  }

  public static float GetTerrain( MapFightingUnit mapFightUnit, TerrainEnum terrainEnum ) {
    if (mapFightUnit.RobotInfo == null)
      return 0;
    switch (terrainEnum) {
      case TerrainEnum.Sky: return (mapFightUnit.RobotInfo.TerrainSky + mapFightUnit.PilotInfo.TerrainSky)/2f;
      case TerrainEnum.Land: return (mapFightUnit.RobotInfo.TerrainLand + mapFightUnit.PilotInfo.TerrainLand)/2f;
      case TerrainEnum.Sea: return (mapFightUnit.RobotInfo.TerrainSea + mapFightUnit.PilotInfo.TerrainSea)/2f;
      case TerrainEnum.Space: return (mapFightUnit.RobotInfo.TerrainSpace + mapFightUnit.PilotInfo.TerrainSpace)/2f;
      default: return 1;
    }
  }

  public static float GetBuffRate( float terrainRank ) {
    string rankNum = terrainRank.ToString();
    switch (rankNum) {
      case "5": return 1.2f;     //S
      case "4.5": return 1.1f;   //A+
      case "4": return 1;        //A
      case "3.5": return 0.9f;   //B+
      case "3": return 0.8f;     //B
      case "2.5": return 0.7f;   //C+
      case "2": return 0.6f;     //C
      case "1.5": return 0.45f;  //D+
      case "1": return 0.3f;     //D
      case "0.5": return 0.15f;  //E+
      case "0": return 0f;       //E
      default: return 0;
    }
  }


  public static string AvergeTerrainStr( MapFightingUnit mapFightUnit, TerrainEnum terrainEnum ) {
    PilotInfo PilotInfo = mapFightUnit.PilotInfo;
    RobotInfo RobotInfo = mapFightUnit.RobotInfo;
    switch (terrainEnum) {
      case TerrainEnum.Sky: return $"{TERRAIN_RANK_ARRAY[RobotInfo.TerrainSky]} + {TERRAIN_RANK_ARRAY[PilotInfo.TerrainSky]} = {GET_TerrainRank( GetTerrain( mapFightUnit, TerrainEnum.Sky ) )}";
      case TerrainEnum.Land: return $"{TERRAIN_RANK_ARRAY[RobotInfo.TerrainLand]} + {TERRAIN_RANK_ARRAY[PilotInfo.TerrainLand]} = {GET_TerrainRank( GetTerrain( mapFightUnit, TerrainEnum.Land ) )}";
      case TerrainEnum.Sea: return $"{TERRAIN_RANK_ARRAY[RobotInfo.TerrainSea]} + {TERRAIN_RANK_ARRAY[PilotInfo.TerrainSea]} = {GET_TerrainRank( GetTerrain( mapFightUnit, TerrainEnum.Sea ) )}";
      case TerrainEnum.Space: return $"{TERRAIN_RANK_ARRAY[RobotInfo.TerrainSpace]} + {TERRAIN_RANK_ARRAY[PilotInfo.TerrainSpace]} = {GET_TerrainRank( GetTerrain( mapFightUnit, TerrainEnum.Space ) )}";
      default: return "------";
    }
  }

  public static float GetWeaponTerrainRate( WeaponInfo weapon, TerrainEnum terrainEnum1, TerrainEnum terrainEnum2 ) {
    float result1;
    float result2;
    switch (terrainEnum1) {
      case TerrainEnum.Sky:   result1 = weapon.TerrainSky; break;
      case TerrainEnum.Land:  result1 = weapon.TerrainLand; break;
      case TerrainEnum.Sea:   result1 = weapon.TerrainSea; break;
      case TerrainEnum.Space: result1 = weapon.TerrainSpace; break;
      default: result1 = 0; break;
    }
    switch (terrainEnum2) {
      case TerrainEnum.Sky: result2 = weapon.TerrainSky; break;
      case TerrainEnum.Land: result2 = weapon.TerrainLand; break;
      case TerrainEnum.Sea: result2 = weapon.TerrainSea; break;
      case TerrainEnum.Space: result2 = weapon.TerrainSpace; break;
      default: result2 = 0; break;
    }
    return GetBuffRate( ( result1 + result2) / 2f );
  }

}

