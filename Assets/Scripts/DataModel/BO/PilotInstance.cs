using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityORM;
using System;

[Serializable]
public class PilotInstance {

  public int SaveSlot;

  public int SeqNo;

  public int PilotID;

  [Ignore] //For SQLite 
  //[NonSerialized]
  public Pilot Pilot;

  public int ShootAdded = 0;

  public int MeleeAdded = 0;

  public int DodgeAdded = 0;

  public int HitAdded = 0;

  public int DexAdded = 0;

  public int DefenseAdded = 0;

  //public string ShortName;

  //public string FirstName;

  //public string LastName;

  public int PP = 0;

  public int Kills = 0;

  public int TerrainSkyAdded = 0;

  public int TerrainLandAdded = 0;

  public int TerrainSeaAdded = 0;

  public int TerrainSpaceAdded = 0;

  //public int MaxSp;

  public int Exp;

  public int Enable;

  //public int RobotInstanceSaveSlot;

  public int? RobotInstanceSeqNo;

  public int? HeroSeqNo;

  //[NonSerialized]
  [Ignore]
  public Hero Hero;

  public string DisplayFullName {
    get {
      if (HeroSeqNo.HasValue && Hero != null) {
        return Hero.FirstName + "=" + Hero.LastName;
      }
      return Pilot.DisplayFullName;
    }
  }

  [Ignore]
  public List<PilotSkillInstance> PilotSkillInstanceList;

  public PilotInfo CloneInfo() {
    PilotInfo pilotInfo = new PilotInfo( this );
    //pilotInfo.Init();
    //pilotInfo.InitValue();
    return pilotInfo;
  }
}