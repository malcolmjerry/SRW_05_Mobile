using System;
using System.Collections.Generic;

[Serializable]
public class SaveContinue : SaveBase {

  /*
  public List<PilotInstance> PilotInstanceList;

  public List<PilotInstance> HeroPilotInstanceList;

  public List<Hero> HeroList;

  public List<RobotInstance> RobotInstanceList;

  public List<PartsInstance> PartsInstanceList;

  public GameData GameData;

  public List<MapFightingUnit> HouseUnits;
  */

  public List<SaveMapUnit> SaveMapUnits = new List<SaveMapUnit>();

  public List<PartsInstance> StageParts = new List<PartsInstance>();

  public int EventStatus;
  public List<int> EventList;
  public List<int> TalkEventList;
  public string PlayerBGM;
  public string EnemyBGM;
  public int ObjectiveIndex;
  public MyVector3 CursorPos;

}

