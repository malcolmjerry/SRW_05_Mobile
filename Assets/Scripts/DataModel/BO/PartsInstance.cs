using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityORM;
using System;

[Serializable]
public class PartsInstance {

  public int SaveSlot = 0;

  public int SeqNo = 0;

  [Ignore] //For SQLite
  public Parts Parts;

  public int PartsID;

  public int? RobotInstanceSaveSlot = null;

  public int? RobotInstanceSeqNo = null;

  public int? RobotOrder = null;

  [Ignore] //For SQLite
  [NonSerialized]
  public RobotInstance RobotInstance = null;

  //[Ignore] 
  public void UnplugFromRobot() {
    RobotInstanceSaveSlot = null;
    RobotInstanceSeqNo = null;
    RobotOrder = null;
  }

}
