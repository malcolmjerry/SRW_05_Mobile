using UnityEngine;
using System.Collections;
using System;
using UnityORM;

[Serializable]
public class Weapon {

  public int HitPoint;

  public int MinRange;

  public int MaxRange;

  public int HitRate;

  public int EN;

  public int Bullets;

  public int WillPower;

  public bool IsMove;

  public int TerrainSky;

  public int TerrainLand;

  public int TerrainSea;

  public int TerrainSpace;

  public int CRI;

  //Fixed--------------------------------------------

  [Key( AutoIncrement = true )]
  public int ID;

  public int PlayIndex;

  public string Name;

  public int NameType;   //1 火神炮  2 Funnel

  public int RobotID;

  public int ImproveType;  
  public static readonly int[][] ImproveTypes = new int[][] {
    new int[] { 100, 100, 110, 110, 120, 120, 130, 130, 140, 140 },
    new int[] { 100, 100, 120, 120, 140, 140, 160, 160, 180, 180 },
    new int[] { 100, 100, 130, 130, 160, 160, 190, 190, 220, 220 },
    new int[] { 100, 100, 140, 140, 180, 180, 220, 220, 260, 260 }
  };

  public bool DefaultEnable;

  public bool IsMelee; //if false = shooting

  public bool IsBeam;

  public bool IsEnergy;

  public bool IsPhysical;

  public int CutType;  //1. 彈開零距離敵人  2. 切爆實體彈

  public bool IsBreak;

  public int IsNT;

  [HideInInspector] public enum RangeTypeEnum { NORMAL = 0, MAP_SELF_CIRCLE, MAP_SELF_LINE, MAP_SET_PLACE }
  public int RangeType;

  public float? RangeWidth;

  public bool IsFriendly;

  public bool IsMap;
}
