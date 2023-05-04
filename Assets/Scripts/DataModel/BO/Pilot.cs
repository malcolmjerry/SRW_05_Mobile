using UnityEngine;
using System.Collections;
using System;
using UnityORM;
using System.Collections.Generic;

[Serializable]
public class Pilot {

  [Key( AutoIncrement = true )]
  public int ID;

  public int Shoot;

  public int Melee;

  public int Dodge;

  public int Hit;

  public int Dex;

  public int Defense;

  public string ShortName;

  public string FirstName;

  public string LastName;

  /// <summary>
  /// 0: 西式   1: 東亞式
  /// </summary>
  public int NameType; // 0: 西式   1: 東亞式

  public string DisplayFullName {
    get {
      if (NameType == 0)
        return FirstName + "=" + LastName;
      return LastName + FirstName;
    }
  }

  public int TerrainSky;

  public int TerrainLand;

  public int TerrainSea;

  public int TerrainSpace;

  public int MaxSp;

  public int Ace1;
  public int Ace2;
  public int Ace3;
  public int Ace4;

  public int ExpDead;

  public int? DriveType;

  public string BGM;

  public int BgmPriority;

  public List<SPComPilot> SPComPilots { set; get; }

  public List<PilotSkillDefault> PilotSkillDefaultList { set; get; }

  /*
  public PilotInstance Clone() {
    PilotInstance pilot = new PilotInstance();
    pilot.ID = this.ID;
    pilot.Shoot = this.Shoot;
    pilot.Melee = this.Melee;
    pilot.Dodge = this.Dodge;
    pilot.Hit = this.Hit;
    pilot.Skill = this.Skill;
    pilot.Defense = this.Defense;
    pilot.ShortName = this.ShortName;
    pilot.FirstName = this.FirstName;
    pilot.LastName = this.LastName;
    pilot.Terrain = this.Terrain;
    pilot.MaxSp = this.MaxSp;

    return pilot;
  }*/

}
