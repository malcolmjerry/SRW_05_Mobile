using System;
using UnityORM;

[Serializable]
public class PilotSkillInstance {

  public int PilotInstanceSaveSlot;

  public int PilotInstanceSeqNo;

  public int OrderSort;  //1~7

  public int PilotSkillID;

  [Ignore] //For SQLite
  //[NonSerialized]
  public PilotSkill PilotSkill;

  public int Level;

  public int DefaultPilotID;

  public int DefaultOrder;

}

