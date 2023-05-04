using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityORM;

[Serializable]
public class RobotInstance {

  public int SaveSlot = 0;

  public int SeqNo = 0;

  public int RobotID;

  [Ignore] //For SQLite
  //[NonSerialized]
  public Robot Robot;

  public int MovePowerLv = 0;

  public int HPLv = 0;

  public int ENLv = 0;

  public int MotilityLv = 0;

  public int HitLv = 0;

  public int ArmorLv = 0;

  public int TerrainSkyLv = 0;

  public int TerrainLandLv = 0;

  public int TerrainSeaLv = 0;

  public int TerrainSpaceLv = 0;

  public int MoveSky = 0;

  public int MoveLand = 0;

  public int MoveSea = 0;

  public int MoveSpace = 0;

  public int Bonus = 0;  //1:HP+3000 2:EN+120 3.運動性+20 4.裝甲+300 5.照準+40 6.移動力+2 7.全地形適性A・全移動適性  8.強化Parts增加1格 9.HP回復(小) 10.EN回復(微) 11.能量護罩消費EN半減

  public string BGM;

  public int WeaponBonus = 0; //1.最小射程-1 最大射程+1 (不含MAP)  2.全武器可P(不包含MAP)  3.消耗EN-20%  4.彈藥1.5倍  5.全地形適性A  6.命中+40  7.CRI+20  8.氣力限制降至100(不含MAP)

  //public int WeaponLevel = 0;
 
  public int Enable;

  public int? Skill1ID;

  [Ignore] //For SQLite
  public RobotSkill RobotSkill1;

  public int? Skill2ID;

  [Ignore] //For SQLite
  public RobotSkill RobotSkill2;

  public int? Skill3ID;

  [Ignore] //For SQLite
  public RobotSkill RobotSkill3;

  public int? Skill4ID;

  [Ignore] //For SQLite
  public RobotSkill RobotSkill4;

  public int? Skill5ID;

  [Ignore] //For SQLite
  public RobotSkill RobotSkill5;

  public int? Skill6ID;

  [Ignore] //For SQLite
  public RobotSkill RobotSkill6;

  [Ignore] //For SQLite
  public List<PartsInstance> PartsInstanceList = new List<PartsInstance>(); //new List<PartsInstance>() { null, null, null, null };

  /*
  public int PartsInstanceSaveSlot1;
  public int PartsInstanceSeqNo1;
  //[Ignore] public PartsInstance PartsInstance1 { get; set; }
  public RobotInstance SetupParts1( PartsInstance partsInstance ) {
    if (partsInstance == null) {
      return this;
    }
    PartsInstanceSaveSlot1 = partsInstance.SaveSlot;
    PartsInstanceSeqNo1 = partsInstance.SeqNo;
    PartsInstance1 = partsInstance;
    partsInstance.RobotInstanceSaveSlot = SaveSlot;
    partsInstance.RobotInstanceSeqNo = SeqNo;
    return this;
  }

  public int PartsInstanceSaveSlot2;
  public int PartsInstanceSeqNo2;
  //[Ignore] public PartsInstance PartsInstance2 { get; set; }
  public RobotInstance SetupParts2( PartsInstance partsInstance ) {
    if (partsInstance == null) return this;
    PartsInstanceSaveSlot2 = partsInstance.SaveSlot;
    PartsInstanceSeqNo2 = partsInstance.SeqNo;
    PartsInstance2 = partsInstance;
    partsInstance.RobotInstanceSaveSlot = SaveSlot;
    partsInstance.RobotInstanceSeqNo = SeqNo;
    return this;
  }

  public int PartsInstanceSaveSlot3;
  public int PartsInstanceSeqNo3;
  //[Ignore] public PartsInstance PartsInstance3 { get; set; }
  public RobotInstance SetupParts3( PartsInstance partsInstance ) {
    if (partsInstance == null) return this;
    PartsInstanceSaveSlot3 = partsInstance.SaveSlot;
    PartsInstanceSeqNo3 = partsInstance.SeqNo;
    PartsInstance3 = partsInstance;
    partsInstance.RobotInstanceSaveSlot = SaveSlot;
    partsInstance.RobotInstanceSeqNo = SeqNo;
    return this;
  }

  public int PartsInstanceSaveSlot4;
  public int PartsInstanceSeqNo4;
  //[Ignore] public PartsInstance PartsInstance4 { get; set; }
  public RobotInstance SetupParts4( PartsInstance partsInstance ) {
    if (partsInstance == null) return this;
    PartsInstanceSaveSlot4 = partsInstance.SaveSlot;
    PartsInstanceSeqNo4 = partsInstance.SeqNo;
    PartsInstance4 = partsInstance;
    partsInstance.RobotInstanceSaveSlot = SaveSlot;
    partsInstance.RobotInstanceSeqNo = SeqNo;
    return this;
  }*/

  //public int PilotInstanceSaveSlot = 0;
  //public int PilotInstanceSeqNo = 0;

  //[Ignore]
  //public List<PilotInstance> PilotInstanceList { get; set; }

  [Ignore]
  public List<WeaponInstance> WeaponInstanceList;

}
