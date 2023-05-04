using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityORM;

[Serializable]
public class Robot {

  [Key(AutoIncrement = true) ]
  public int ID;

  public string Name;

  public float MovePower;

  public string FullName;

  public int HP;

  public int EN;

  public float Radius;

  public int Motility;

  public int HitRate;

  public int Armor;

  //public string Terrain;

  public int TerrainSky;

  public int TerrainLand;

  public int TerrainSea;

  public int TerrainSpace;

  public int MoveSky;

  public int MoveLand;

  public int MoveSea;

  public int MoveSpace;

  public string BGM;

  public int Size;

  public int RepairPrice;

  public int? Skill1ID;

  public int? Skill2ID;

  public int? Skill3ID;

  public int? Skill4ID;

  public int? Skill5ID;

  public int? Skill6ID;

  public int PartsSlot;

  public bool Shield;

  public bool Cutter;

  public int? DriveType;

  public bool? IsShip;

  /*
  public RobotSkill RobotSkill1;

  public RobotSkill RobotSkill2;

  public RobotSkill RobotSkill3;

  public RobotSkill RobotSkill4;
  */

  public List<Weapon> WeaponList;

}
