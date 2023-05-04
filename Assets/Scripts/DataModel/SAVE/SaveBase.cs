using System;
using System.Collections.Generic;

[Serializable]
public class SaveBase {

  public List<PilotInstance> PilotInstanceList;

  public List<PilotInstance> HeroPilotInstanceList;

  public List<Hero> HeroList;

  public List<RobotInstance> RobotInstanceList;

  public List<PartsInstance> PartsInstanceList;

  public GameData GameData;

  public List<MapFightingUnit> HouseUnits;

}

